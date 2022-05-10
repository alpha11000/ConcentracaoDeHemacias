namespace ConcentracaoDeHemacias.Codigos.Utils
{
    internal class MatrixUtil
    {
        public static int[,] getFilledMatrixFrom(int[,] originalMatrix, int valueToFill = 0)
        {
            int[,] output;

            output = ChannelIterationUtil.iterateOnChannel(originalMatrix, i => valueToFill);

            return output;
        }

        public static int[,] getFilledMatrix(int h, int w, int value = 0)
        {
            int[,] output = new int[h, w];
            output = getFilledMatrixFrom(output, value);

            return output;
        }
    }
}
