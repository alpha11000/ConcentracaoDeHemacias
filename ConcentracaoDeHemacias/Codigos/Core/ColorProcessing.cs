using ConcentracaoDeHemacias.Codigos.Utils;

namespace ConcentracaoDeHemacias.Codigos.Core
{
    internal class ColorProcessing
    {


        public static int[,] getGrayscaleChannel((int[,] R, int[,] G, int[,] B) channels)
        {
            int[,] output = MatrixUtil.getEmptyMatrixFrom(channels.R);

            output = ChannelIterationUtil.iterateOnChannels(channels, (r, g, b) => (r + g + b) / 3);

            return output;
        }

        public static int[,] getLimiarizedChannel(int[,] channel, int limiar, int min = 0, int max = 255)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannel(channel, i => (i < limiar) ? min : max);

            return output;
        }

        public static (int[,] R, int[,] G, int[,] B) getAllColorChannels(Bitmap original)
        {
            int[,]
                outputR = new int[original.Height, original.Width],
                outputG = MatrixUtil.getEmptyMatrixFrom(outputR),
                outputB = MatrixUtil.getEmptyMatrixFrom(outputR);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    outputR[y, x] = pixelColor.R;
                    outputG[y, x] = pixelColor.G;
                    outputB[y, x] = pixelColor.B;
                }
            }

            return (outputR, outputG, outputB);
        }

        public static Bitmap getBitmapFromColorChannels(int[,] R, int[,] G, int[,] B)
        {
            Bitmap output = new Bitmap(R.GetLength(1), R.GetLength(0));

            for (int y = 0; y < R.GetLength(0); y++)
            {
                for (int x = 0; x < G.GetLength(1); x++)
                {
                    Color pixelColor = Color.FromArgb(R[y, x], G[y, x], B[y, x]);
                    output.SetPixel(x, y, pixelColor);
                }
            }

            return output;
        }


    }
}
