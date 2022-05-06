using ConcentracaoDeHemacias.Codigos.Utils;
using ConcentracaoDeHemacias.Codigos.Core;

namespace ConcentracaoDeHemacias
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());

            OpenFileDialog _ofd = new OpenFileDialog();
            Bitmap bmpO = null;
            
            
            Console.Title = "Pratica 3 PDI";


            while(true){

                ConsoleUtil.writeColoredLine("Escolha uma imagem de entrada:", (int)ConsoleColor.Yellow);

                if(_ofd.ShowDialog() == DialogResult.OK){
                    try
                    {
                        bmpO = new Bitmap(_ofd.FileName);
                        Bitmap bmp = new Bitmap(bmpO);

                        ConsoleUtil.writeColoredLine("Imagem carregada com sucesso", (int)ConsoleColor.Green);

                        ConsoleUtil.showImageWindow(bmp, "Imagem de entrada");
                        var channels = ColorProcessing.getAllColorChannels(bmpO);

                        int[,] grayScale = ColorProcessing.getGrayscaleChannel(channels, true);
                        
                        ConsoleUtil.showImageWindow(grayScale, "gray scale");

                        var medianF = Filters.applyMedianFilter(grayScale, 9);
                        ConsoleUtil.showImageWindow(medianF, "median filter");

                        int[,] limiarized = ColorProcessing.getLimiarizedChannel(medianF, 200, 300, 255, 0);
                        //ConsoleUtil.showImageWindow(limiarized, "median limiarized");

                        int[,] limiarized2 = ColorProcessing.getLimiarizedChannel(grayScale, 0, 220, 0, 255);
                        //ConsoleUtil.showImageWindow(limiarized2, "limiarized2");

                        int[,] limiarizedRed = ColorProcessing.getLimiarizedChannel(channels.R, 145, 300, 0, 255);
                        ConsoleUtil.showImageWindow(limiarizedRed, "limiarized red");
                        limiarizedRed = MorphologicalImageProcessing.dilateChannel(limiarizedRed, MatrixUtil.getFilledMatrix(5,5,255));
                        ConsoleUtil.showImageWindow(limiarizedRed, "limiarized red dilated 5x5");
                        limiarizedRed = ColorProcessing.getInvertedChannel(limiarizedRed);
                        ConsoleUtil.showImageWindow(limiarizedRed, "inverted red limiar");


                        int[,] difference = MorphologicalImageProcessing.getChannelsDifference(limiarized2, limiarized);
                        ConsoleUtil.showImageWindow(difference, "difference");


                        var sum = MorphologicalImageProcessing.getMaxFromChannels(difference, limiarizedRed);
                        ConsoleUtil.showImageWindow(sum, "sum");

                        ///////////verificar o poder de estar realizando erosao e não dilatação
                        //dilata para preencher as formas, depois erode pra remover os indesejados, dps redilata pra forma original
                        var invertedSum = ColorProcessing.getInvertedChannel(sum);
                        //var dilatedSum = MorphologicalImageProcessing.dilateChannel(invertedSum, new int[5, 5]);
                        //dilatedSum = ColorProcessing.getInvertedChannel(dilatedSum);
                        //ConsoleUtil.showImageWindow(dilatedSum, "dilated sum");

                        Func<int[,], int, int[,]> medianFilter = Filters.applyMedianFilter;

                        var openingSum = MorphologicalImageProcessing.getChannelOpening(sum, new int[5, 5]);
                        ConsoleUtil.showImageWindow(openingSum, "first opening");

                        var closing = MorphologicalImageProcessing.getChannelClosing(openingSum, new int[21, 21], medianFilter, 7);
                        ConsoleUtil.showImageWindow(closing, "sum closing");

                        openingSum = MorphologicalImageProcessing.getChannelOpening(closing, new int[7, 7]);
                        ConsoleUtil.showImageWindow(openingSum, "sum opening");

                        while (true)
                        {
                            int limiar1 = int.Parse(Console.ReadLine());
                            int limiar2 = int.Parse(Console.ReadLine());

                            limiarized = ColorProcessing.getLimiarizedChannel(grayScale, limiar1, limiar2);
                            ConsoleUtil.showImageWindow(limiarized, $"limiar [{limiar1},{limiar2}]");
                        }
                        
                        break;
                        
                    }
                    catch
                    {
                        ConsoleUtil.writeColoredLine("Formato não suportado.", (int)ConsoleColor.Red);
                    }
                }
                else{
                    if (!ConsoleUtil.yesNoOptionWrite("\nNenhuma imagem escolhida. Permanecer no programa? ")) return;
                }
            }

            Console.Write("Pressione \"Enter\" para fechar...");
            Console.Read();
        }
    }
}