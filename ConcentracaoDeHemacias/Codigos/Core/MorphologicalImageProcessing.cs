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

            ConsoleUtil.showImageWindow(output, "eroded");

            if (filter != null){
                output = filter(output, filterSize);
            }
            ConsoleUtil.showImageWindow(output, "eroded w/ median");


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
                    output[h, w] = (contained) ? foreIntensity : backIntesity;
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

                    int midStructIntensity = structuringElement[structSize[0] / 2, structSize[1] / 2];

                    output[w, h] = (present) ? fore : back;
                }
            }
            return output;
        }
    }
}
