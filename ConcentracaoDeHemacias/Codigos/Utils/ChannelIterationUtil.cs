namespace ConcentracaoDeHemacias.Codigos.Utils
{
    internal class ChannelIterationUtil
    {
        public static int[,] iterateOnChannels((int[,] R, int[,] G, int[,] B) channels, Func<int, int, int, int> function)
        {
            int[,] output = new int[channels.R.GetLength(0), channels.R.GetLength(1)];

            for (int h = 0; h < output.GetLength(0); h++)
            {
                for (int w = 0; w < output.GetLength(1); w++)
                {
                    output[h, w] = function(channels.R[h, w], channels.G[h, w], channels.B[h, w]);
                }
            }

            return output;
        }

        public static (int[,] R, int[,] G, int[,] B) iterateOnChannels((int[,] R, int[,] G, int[,] B) channels, Func<int, int> function)
        {
            int[,] outputR = new int[channels.R.GetLength(0), channels.R.GetLength(1)],
                   outputG = new int[channels.G.GetLength(0), channels.G.GetLength(1)],
                   outputB = new int[channels.B.GetLength(0), channels.B.GetLength(1)];

            for (int h = 0; h < outputR.GetLength(0); h++)
            {
                for (int w = 0; w < outputR.GetLength(1); w++)
                {
                    outputR[h, w] = function(channels.R[h, w]);
                    outputG[h, w] = function(channels.G[h, w]);
                    outputB[h, w] = function(channels.B[h, w]);
                }
            }

            return (outputR, outputG, outputB);
        }

        public static int[,] iterateOnChannels(int[,] A, int[,] B, Func<int, int, int> function)
        {
            int[,] output =  new int[A.GetLength(0), A.GetLength(1)];

            for (int h = 0; h < output.GetLength(0); h++)
            {
                for (int w = 0; w < output.GetLength(1); w++)
                {
                    output[h, w] = function(A[h,w], B[h,w]);
                }
            }

            return output;
        }

        public static int[,] iterateOnChannel(int[,] channel, Func<int, int> function)
        {
            int[,] output = new int[channel.GetLength(0), channel.GetLength(1)];

            for (int h = 0; h < output.GetLength(0); h++)
            {
                for (int w = 0; w < output.GetLength(1); w++)
                {
                    output[h, w] = function(channel[h, w]);
                }
            }

            return output;
        }
    }
}
