namespace ConcentracaoDeHemacias.Codigos.Utils
{
    internal class MatrixUtil
    {
        public static int[,] getEmptyMatrixFrom(int[,] originalMatrix, int valueToFill = 0)
        {
            int[,] output = new int[originalMatrix.GetLength(0), originalMatrix.GetLength(1)];

            if (valueToFill != 0)
            {
                ChannelIterationUtil.iterateOnChannel(output, i => valueToFill);
            }

            return output;
        }
    }
}
