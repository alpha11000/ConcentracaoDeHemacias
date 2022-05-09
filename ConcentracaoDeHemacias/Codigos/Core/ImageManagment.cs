using ConcentracaoDeHemacias.Codigos.Utils;

namespace ConcentracaoDeHemacias.Codigos.Core
{
    internal class ImageManagment
    {
        public static int[,] getImageBorders(int[,] original)
        {
            int[,] output = new int[original.GetLength(0), original.GetLength(1)];

            for (int w = 0; w < output.GetLength(0); w++){
                for (int h = 0; h < output.GetLength(1); h++){

                    if (w == 0 || w == output.GetLength(0) - 1 || h == 0 || h == output.GetLength(1) - 1){
                        output[w, h] = original[w, h];
                    }
                    else{
                        output[w, h] = 255;
                    }
                }
            }

            return output;
        }

        public static int countPixelsWithIntensity(int[,] channel, int intensity)
        {
            int sum = 0;

            foreach (int i in channel)
            {
                if (i == intensity) sum++;
            }
            return sum;
        }

        public static int[,] getAumentedChannel(int[,] originalChannelMatrix, int[] structSize, int intensityToFill = 50)
        {
            //armazenando o tamanho das estruturas para simplicidade nas chamadas
            int[] channelSize = { originalChannelMatrix.GetLength(0), originalChannelMatrix.GetLength(1) };

            int[,] aumented = new int[channelSize[0] + structSize[0], channelSize[1] + structSize[1]];
            aumented = MatrixUtil.getFilledMatrixFrom(aumented, intensityToFill);

            for (int h = structSize[0] / 2; h < channelSize[0] + (structSize[0] / 2); h++)
            {
                for (int w = structSize[1] / 2; w < channelSize[1] + (structSize[1] / 2); w++)
                {
                    aumented[h, w] = originalChannelMatrix[h - (structSize[0] / 2), w - (structSize[1] / 2)];
                }
            }

            return aumented;
        }

        public static bool compareChannels(int[,] A, int[,] B)//return true or false - equal or not
        {
            for (int h = 0; h < A.GetLength(0); h++){
                for (int w = 0; w < A.GetLength(1); w++){
                    if (A[h, w] != B[h, w])
                        return false;
                }
            }

            return true;
        }
    }
}
