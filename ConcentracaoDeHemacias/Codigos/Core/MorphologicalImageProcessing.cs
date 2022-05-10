using ConcentracaoDeHemacias.Codigos.Utils;

namespace ConcentracaoDeHemacias.Codigos.Core
{
    internal class MorphologicalImageProcessing
    {
        public static int[,] getChannelsDifference(int[,] A, int[,] B)
        {
            int[,] output = new int[A.GetLength(0), A.GetLength(1)];

            output = ChannelIterationUtil.iterateOnChannels(A, B, (i, j) =>
            {
                int v = i - j;
                if (v < 0) v = 0;
                return v;
            });

            return output;
        }

        public static (int[,] R, int[,] G, int[,] B) getMaxOnAllChannels((int[,] R, int[,] G, int[,] B) channels, int[,] mask)
        {
            int[,] outputR = getMaxFromChannels(channels.R, mask),
                   outputG = getMaxFromChannels(channels.G, mask),
                   outputB = getMaxFromChannels(channels.B, mask);

            return (outputR, outputG, outputB);
        }

        public static (int[,] R, int[,] G, int[,] B) getMinOnAllChannels((int[,] R, int[,] G, int[,] B) channels, int[,] mask)
        {
            int[,] outputR = getMinFromChannels(channels.R, mask),
                   outputG = getMinFromChannels(channels.G, mask),
                   outputB = getMinFromChannels(channels.B, mask);

            return (outputR, outputG, outputB);
        }

        public static int[,] getMaxFromChannels(int[,] A, int[,] B)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannels(A, B, (i, j) => Math.Max(i, j));

            return output;
        }

        public static int[,] getMinFromChannels(int[,] A, int[,] B)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannels(A, B, (i, j) => Math.Min(i, j));

            return output;
        }

        public static int[,] getIntersection(int[,] A, int[,] B, int fore = 255)
        {
            int[,] output;

            output = ChannelIterationUtil.iterateOnChannels(A, B, (i, j) => (i == j) ? i : 255);

            return output;
        }

        public static int[,] getChannelOpening(int[,] channel, int[,] structuringElement, Func<int[,], int, int[,]> filter = null, int filterSize = 3)
        {
            int[,] output = erodeChannel(channel, structuringElement, out _);

            if(filter != null){
                output = filter(output, filterSize);
            }

            output = dilateChannel(output, structuringElement);

            return output;
        }

        public static int[,] getChannelClosing(int[,] channel, int[,] structuringElement, Func<int[,], int, int[,]> filter = null, int filterSize = 3)
        {
            int[,] output = dilateChannel(channel, structuringElement);

           // ConsoleUtil.showImageWindow(output, "eroded");

            if (filter != null){
                output = filter(output, filterSize);
            }
            //ConsoleUtil.showImageWindow(output, "eroded w/ median");


            output = erodeChannel(output, structuringElement, out _);

            return output;
        }

        public static int[,] erodeChannel(int[,] originalChannelMatrix, int[,] structuringElement, out bool someHit, int foreIntensity = 0, int backIntesity = 255)
        {
            //armazenando o tamanho das estruturas para simplicidade nas chamadas
            int[] channelSize = { originalChannelMatrix.GetLength(0), originalChannelMatrix.GetLength(1) };
            int[] structSize = { structuringElement.GetLength(0), structuringElement.GetLength(1) };

            int[,] aumented = ImageManagment.getAumentedChannel(originalChannelMatrix, structSize, 50);
            int[,] output = new int[channelSize[0], channelSize[1]];

            int centerIntensity = structuringElement[structSize[0] / 2, structSize[1] / 2];

            someHit = false;

            for (int h = 0; h < channelSize[0]; h++){
                for (int w = 0; w < channelSize[1]; w++){

                    bool contained = true;

                    for (int sH = 0; sH < structSize[0]; sH++){
                        for (int sW = 0; sW < structSize[1]; sW++){

                            if (structuringElement[sH, sW] < 0) continue;

                            if (aumented[h + sH, w + sW] != structuringElement[sH, sW]){
                                contained = false;
                                break;
                            }
                        }
                    }
                    if (!someHit && contained) someHit = true;
                    output[h, w] = (contained) ? centerIntensity : 255 - centerIntensity;
                }
            }
            return output;
        }

        public static int[,] dilateChannel(int[,] originalChannelMatrix, int[,] structuringElement, int fore = 0, int back = 255)
        {
            //armazenando o tamanho das estruturas para simplicidade nas chamadas
            int[] channelSize = { originalChannelMatrix.GetLength(0), originalChannelMatrix.GetLength(1) };
            int[] structSize = { structuringElement.GetLength(0), structuringElement.GetLength(1) };

            int[,] aumented = ImageManagment.getAumentedChannel(originalChannelMatrix, structSize, 50);
            int[,] output = new int[channelSize[0], channelSize[1]];

            int midStructIntensity = structuringElement[structSize[0] / 2, structSize[1] / 2];


            for (int w = 0; w < channelSize[0]; w++){
                for (int h = 0; h < channelSize[1]; h++){
                    
                    bool present = false;

                    for (int sW = 0; sW < structSize[0]; sW++){
                        for (int sH = 0; sH < structSize[1]; sH++){

                            if (aumented[w + sW, h + sH] == structuringElement[sW, sH])
                            {
                                present = true;
                                break;
                            }
                        }
                    }


                    output[w, h] = (present) ? midStructIntensity : 255-midStructIntensity;
                }
            }
            return output;
        }

        public static int[,] geodesicDilation(int[,] channel, int[,] mask)
        {
            int[,] output = MatrixUtil.getFilledMatrixFrom(channel);
            int[,] structuringElement = new int[3, 3];

            int[,] dilation = channel;
            int[,] tempDilation;


            while (true)
            {
                tempDilation = dilateChannel(dilation, structuringElement);
                tempDilation = getIntersection(tempDilation, mask);

                if (ImageManagment.compareChannels(tempDilation, dilation))
                    break;

                dilation = tempDilation;

            }

            output = dilation;

            return output;
        }

        public static int[,] fillHoles(int[,] original, int iterations = -1)
        {
            int[,] inverted = ColorProcessing.getInvertedChannel(original);
            int[,] structure = new int[3, 3];

            int[,] dilation = ImageManagment.getImageBorders(inverted);
            int[,] tempDilation;

            int i = 0;

            while (true)
            {
                if (iterations > 0 && i > iterations) break;
                i++;

                tempDilation = dilateChannel(dilation, structure);
                tempDilation = getIntersection(tempDilation, inverted);

                if (ImageManagment.compareChannels(tempDilation, dilation))
                    break;

                dilation = tempDilation;

            }

            int[,] output = ColorProcessing.getInvertedChannel(dilation);
            output = getMinFromChannels(output, original);

            return output;
        }
    }
}
