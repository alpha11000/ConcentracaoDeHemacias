using ConcentracaoDeHemacias.Codigos.Utils;

namespace ConcentracaoDeHemacias.Codigos.Core
{
    internal class ColorProcessing
    {
        public static int[,] getInvertedChannel(int[,] channel)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannel(channel, i => 255 - i);

            return output;
        }

        public static int[,] getPixelsWithValue(int[,] channel, int value, int min = 0, int max = 255)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannel(channel, i => (i == value) ? min : max);

            return output;
        }

        public static int[,] getGrayscaleChannel((int[,] R, int[,] G, int[,] B) channels)
        {
            int[,] output = MatrixUtil.getFilledMatrixFrom(channels.R);

            output = ChannelIterationUtil.iterateOnChannels(channels, (r, g, b) => (r + g + b) / 3);

            return output;
        }

        public static int[,] getGrayscaleChannel((int[,] R, int[,] G, int[,] B) channels, bool max)
        {
            int[,] output = MatrixUtil.getFilledMatrixFrom(channels.R);

            if (max)
            {
                output = MorphologicalImageProcessing.getMaxFromChannels(channels.R, channels.G);
                output = MorphologicalImageProcessing.getMaxFromChannels(output, channels.B);
            }
            else
            {
                output = MorphologicalImageProcessing.getMinFromChannels(channels.R, channels.G);
                output = MorphologicalImageProcessing.getMinFromChannels(output, channels.B);
            }
            
            return output;
        }

        public static int[,] getLimiarizedChannel(int[,] channel, int limiar, int limiar2 = int.MaxValue, int min = 0, int max = 255)
        {
            int[,] output;
            output = ChannelIterationUtil.iterateOnChannel(channel, i => (i > limiar && i <limiar2) ? max : min);

            return output;
        }

        public static (int[,] R, int[,] G, int[,] B) getAllColorChannels(Bitmap original)
        {
            int[,]
                outputR = new int[original.Height, original.Width],
                outputG = MatrixUtil.getFilledMatrixFrom(outputR),
                outputB = MatrixUtil.getFilledMatrixFrom(outputR);

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

        public static int[,] getSaturationChannelFromChannels((int[,] R, int[,] G, int[,] B) channels)
        {
            int[,] output;

            Func<int, int, int, int> func = (R, G, B) =>
            {
                double r = R / 255.0, g = G / 255.0, b = B / 255.0;
                double maxValue = Math.Max(r, Math.Max(g, b)); //valor máximo entre os três canais
                double minValue = Math.Min(r, Math.Min(g, b)); //valor minimo entre os três canais
                double dif = maxValue - minValue;
                double h;

                if (dif == 0)
                {
                    h = 0;
                }
                else if (maxValue == r)
                {
                    if (g >= b)
                    {
                        h = (60 * (g - b)) / dif;
                    }
                    else
                    {
                        h = ((60 * (g - b)) / dif) + 360;
                    }
                }
                else if (maxValue == g)
                {
                    h = ((60 * (b - r)) / dif) + 120;
                }
                else
                {
                    h = ((60 * (r - g)) / dif) + 240;
                }

                return (int)((255.0 * h) / 360.0);
            };

            output = ChannelIterationUtil.iterateOnChannels(channels, func);

            return output;
        }

        public static Bitmap getBitmapFromColorChannel(int[,] gray)
        {
            return getBitmapFromColorChannels(gray, gray, gray);
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
