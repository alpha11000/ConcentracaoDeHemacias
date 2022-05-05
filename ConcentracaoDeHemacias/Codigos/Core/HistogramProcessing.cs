using ConcentracaoDeHemacias.Codigos.Utils;

namespace ConcentracaoDeHemacias.Codigos.Core
{
    internal class HistogramProcessing
    {
        public static SortedDictionary<int, int> getHistogramFromChannel(int[,] channel)
        {
            Dictionary<int, int> histogram = new Dictionary<int, int>();

            //percorre todos os elementos da matriz
            for (int h = 0; h < channel.GetLength(0); h++)
            {
                for (int w = 0; w < channel.GetLength(1); w++)
                {
                    if (histogram.ContainsKey(channel[h, w]))
                    {
                        histogram[channel[h, w]]++; //caso o valor já esteja contido no dicionario, a quantidade é incrementada
                    }
                    else
                    {
                        histogram.Add(channel[h, w], 1); //adiciona o valor atual ao dicionário caso este ainda não esteja contido no mesmo
                    }
                }
            }

            return new SortedDictionary<int, int>(histogram);
        }

        public static Dictionary<int, int> getEqualizedValuesToHistogram(int[,] channel, SortedDictionary<int, int> histogram = null, int MN = 0)//não setar MN a menos que se esteja trabalhando com histograma de rgb
        {
            if (channel == null && MN == 0) return null;

            if (histogram == null)
            {
                histogram = getHistogramFromChannel(channel);
            }

            Dictionary<int, int> output = new Dictionary<int, int>();

            SortedDictionary<int, double> normalizedHistogram = new SortedDictionary<int, double>();

            MN = (MN == 0) ? channel.GetLength(0) * channel.GetLength(1) : MN; //total de elementos da matriz

            //normaliza as quantidades dos elementos (quantidade/MN)
            foreach (var peer in histogram)
            {
                double pr = peer.Value / (double)MN;
                normalizedHistogram.Add(peer.Key, pr);
            }

            double prSum = 0;
            //realiza a equalização do histograma
            foreach (var peer in normalizedHistogram)
            {
                prSum += peer.Value;
                output.Add(peer.Key, (int)(prSum * 255));
            }

            return output;
        }

        public static int[,] getEqualizedChannel(int[,] channel, Dictionary<int, int> valuesMap = null)
        {
            if (valuesMap == null)
            {
                valuesMap = getEqualizedValuesToHistogram(channel);
            }

            int[,] output;

            output = ChannelIterationUtil.iterateOnChannel(channel, i => (valuesMap.ContainsKey(i) ? valuesMap[i] : i));

            return output;
        }
    }
}
