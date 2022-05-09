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
                        var emptyChannel = MatrixUtil.getFilledMatrixFrom(channels.R);

                        var hChannel = ColorProcessing.getSaturationChannelFromChannels(channels);
                        ConsoleUtil.showImageWindow(hChannel, "H channel");

                        ConsoleUtil.writeColoredLine("Limiarizando canal HUE [163, 214]...", (int)ConsoleColor.Yellow);
                        var limiarizedH = ColorProcessing.getLimiarizedChannel(hChannel, 163, 214); //75?
                        ConsoleUtil.showImageWindow(limiarizedH, "limiarized H");

                        ConsoleUtil.writeColoredLine("realizando abertura no canal limiarizado (3x3)...", (int)ConsoleColor.Yellow);
                        var opening = MorphologicalImageProcessing.getChannelOpening(limiarizedH, new int[3, 3]);
                        ConsoleUtil.showImageWindow(opening, "opening");

                        ConsoleUtil.writeColoredLine("realizando fechamento (9x9)...", (int)ConsoleColor.Yellow);
                        var closing = MorphologicalImageProcessing.getChannelClosing(opening, new int[9, 9]);
                        ConsoleUtil.showImageWindow(closing, "closing");
                        
                        int[,] grayScale = ColorProcessing.getGrayscaleChannel(channels, true);
                        ConsoleUtil.showImageWindow(grayScale, "gray scale");

                        var minGrayScale = ColorProcessing.getGrayscaleChannel(channels, false);
                        ConsoleUtil.showImageWindow(minGrayScale, "min Gray");

                        ConsoleUtil.writeColoredLine("Limiarizando o valor Min(R, G, B) [115, 255]...", (int)ConsoleColor.Yellow);
                        var limiarizedMinGray = ColorProcessing.getLimiarizedChannel(minGrayScale, 115);
                        ConsoleUtil.showImageWindow(limiarizedMinGray, "lim min gray");

                        ConsoleUtil.writeColoredLine("Dilatando (3x3)...", (int)ConsoleColor.Yellow);
                        var dilatedLimMin = MorphologicalImageProcessing.dilateChannel(limiarizedMinGray, new int[3, 3]);
                        ConsoleUtil.showImageWindow(dilatedLimMin, "dilated lim min");


                        var invLim = ColorProcessing.getInvertedChannel(dilatedLimMin);
                        ConsoleUtil.showImageWindow(invLim, "inv lim");


                        var maxOrig_Closing = MorphologicalImageProcessing.getMaxOnAllChannels(channels, closing);
                        ConsoleUtil.showImageWindow(maxOrig_Closing, "max original | closing");

                        var maxOrig_Closing_MinGray = MorphologicalImageProcessing.getMaxOnAllChannels(maxOrig_Closing, invLim);
                        ConsoleUtil.showImageWindow(maxOrig_Closing_MinGray, "max all");

                        var eliminateds = ColorProcessing.getGrayscaleChannel(maxOrig_Closing_MinGray);
                        var limiarizedEliminated = ColorProcessing.getLimiarizedChannel(eliminateds, 207, 255);

                        var openingEliminateds = MorphologicalImageProcessing.getChannelOpening(limiarizedEliminated, new int[3, 3], Filters.applyMedianFilter, 5);

                        var limiarizedEliminatedColored = MorphologicalImageProcessing.getMaxOnAllChannels(channels, openingEliminateds);

                        ConsoleUtil.showImageWindow(eliminateds, "eliminateds");
                        ConsoleUtil.showImageWindow(limiarizedEliminated, "limiar eliminateds");
                        ConsoleUtil.showImageWindow(openingEliminateds, "opening eliminateds");
                        ConsoleUtil.showImageWindow(limiarizedEliminatedColored, "limiar eliminateds all channels");

                        var separatedBlueGreen = ColorProcessing.getBitmapFromColorChannels(emptyChannel, limiarizedEliminatedColored.G, limiarizedEliminatedColored.B);
                        
                        var reseparedChannels = ColorProcessing.getAllColorChannels(separatedBlueGreen);
                        ConsoleUtil.showImageWindow(separatedBlueGreen);
                        var grayScaleBlueGreen = ColorProcessing.getGrayscaleChannel(reseparedChannels, true);
                        ConsoleUtil.showImageWindow(grayScaleBlueGreen);

                        var limiarBlueGreen = ColorProcessing.getLimiarizedChannel(grayScaleBlueGreen, 198);
                        ConsoleUtil.showImageWindow(limiarBlueGreen, "limiar Blue Green");

                        var openingBlueGreen = MorphologicalImageProcessing.getChannelOpening(limiarBlueGreen, new int[3, 3]);
                        ConsoleUtil.showImageWindow(openingBlueGreen, "opening BlueGreen");



                        var closingBlueGreen = MorphologicalImageProcessing.getChannelClosing(openingBlueGreen, new int[11, 11]);
                        ConsoleUtil.showImageWindow(closingBlueGreen, "closing blue green");


                        var limiarizedOriginalGreen = ColorProcessing.getLimiarizedChannel(channels.G, 220);
                        ConsoleUtil.showImageWindow(limiarizedOriginalGreen, "original relimiarized");

                        var dilatedOriginalRelimiarized = MorphologicalImageProcessing.dilateChannel(limiarizedOriginalGreen, new int[3, 3]);
                        ConsoleUtil.showImageWindow(dilatedOriginalRelimiarized, "dilated");

                        var intersect = MorphologicalImageProcessing.getIntersection(closingBlueGreen, dilatedOriginalRelimiarized);
                        ConsoleUtil.showImageWindow(intersect, "closing intersect");

                        var filled = MorphologicalImageProcessing.fillHoles(openingEliminateds);
                        ConsoleUtil.showImageWindow(filled, "filled");

                        var filledIntersect = MorphologicalImageProcessing.getIntersection(filled, closingBlueGreen);
                        ConsoleUtil.showImageWindow(filledIntersect, "filled Intersect");


                        var maxG = MorphologicalImageProcessing.getMaxOnAllChannels(channels, filledIntersect);
                        ConsoleUtil.showImageWindow(maxG, "resultado final...");

                        int contagem = ImageManagment.countPixelsWithIntensity(filledIntersect, 0);
                        float porcentagem = (100.0f * contagem) / (filledIntersect.GetLength(0) * filledIntersect.GetLength(1));
                        ConsoleUtil.writeColoredLine($"As hemacias ocupam aproximadamente {contagem} pixels da imagem, representando {porcentagem}% da imagem.", (int)ConsoleColor.Green);
                        //var maxL = MorphologicalImageProcessing.getMaxFromChannels(openingEliminateds, closingBlueGreen);
                        //ConsoleUtil.showImageWindow(maxL, "max");

                        //var minL = MorphologicalImageProcessing.getMinFromChannels(maxL, closingBlueGreen);
                        //ConsoleUtil.showImageWindow(minL, "min");

                        //var medianAplied = MorphologicalImageProcessing.getChannelOpening()
                        //var medianAplied = Filters.applyMedianFilter(limiarizedEliminatedColored, 13);
                        //ConsoleUtil.showImageWindow(medianAplied, " median");

                        //var grayMedian = ColorProcessing.getGrayscaleChannel(medianAplied, true);

                        //var limiarMedian = ColorProcessing.getLimiarizedChannel(grayMedian, 211, 255);
                        //ConsoleUtil.showImageWindow(limiarMedian, "limiar med");

                        //var openingLimiarMedian = MorphologicalImageProcessing.getChannelOpening(limiarMedian, new int[7, 3]);
                        //ConsoleUtil.showImageWindow(openingLimiarMedian, "opening limiar median");

                        //var openingLimiarMedian = MorphologicalImageProcessing.getChannelOpening(limiarizedEliminated, new int[3,3])

                        //var dilatedLimiarMedian = MorphologicalImageProcessing.dilateChannel(openingLimiarMedian, new int[7, 7]);
                        //ConsoleUtil.showImageWindow(dilatedLimiarMedian, "dilatedLimiarMedian");

                        //var limiarizedEliminatedMedian = MorphologicalImageProcessing.getMaxOnAllChannels(limiarizedEliminatedColored, dilatedLimiarMedian);
                        //ConsoleUtil.showImageWindow(limiarizedEliminatedMedian, "colored median eliminated");
                        /*
                        var redGreenIntersectio = MorphologicalImageProcessing.getIntersection(channels.R, channels.G);

                        var limiarizedBlue = ColorProcessing.getLimiarizedChannel(channels.B, 182);
                        ConsoleUtil.showImageWindow(limiarizedBlue, "limi Blue");

                        var limiarizedGreen = ColorProcessing.getLimiarizedChannel(channels.G, 121, 255, 0, 255);
                        ConsoleUtil.showImageWindow(limiarizedGreen, "limiarized green");


                        var medianF = Filters.applyMedianFilter(grayScale, 9);
                        ConsoleUtil.showImageWindow(medianF, "median filter");


                        int[,] limiarized = ColorProcessing.getLimiarizedChannel(medianF, 200, 300, 255, 0);
                        ConsoleUtil.showImageWindow(limiarized, "median limiarized");

                        int[,] limiarized2 = ColorProcessing.getLimiarizedChannel(grayScale, 0, 220, 0, 255);
                        ConsoleUtil.showImageWindow(limiarized2, "limiarized2");

                        int[,] limiarizedRed = ColorProcessing.getLimiarizedChannel(channels.R, 145, 300, 0, 255);
                        ConsoleUtil.showImageWindow(limiarizedRed, "limiarized red");
                        limiarizedRed = MorphologicalImageProcessing.dilateChannel(limiarizedRed, MatrixUtil.getFilledMatrix(5,5,255));
                        ConsoleUtil.showImageWindow(limiarizedRed, "limiarized red dilated 5x5");
                        int[,] invertedLimiarizedRed = ColorProcessing.getInvertedChannel(limiarizedRed);
                        ConsoleUtil.showImageWindow(limiarizedRed, "inverted red limiar");


                        int[,] difference = MorphologicalImageProcessing.getChannelsDifference(limiarized2, limiarized);
                        ConsoleUtil.showImageWindow(difference, "difference");


                        var sum = MorphologicalImageProcessing.getMaxFromChannels(difference, invertedLimiarizedRed);
                        ConsoleUtil.showImageWindow(sum, "sum");

                        ///////////verificar o poder de estar realizando erosao e não dilatação
                        //dilata para preencher as formas, depois erode pra remover os indesejados, dps redilata pra forma original
                        var invertedSum = ColorProcessing.getInvertedChannel(sum);
                        //var dilatedSum = MorphologicalImageProcessing.dilateChannel(invertedSum, new int[5, 5]);
                        //dilatedSum = ColorProcessing.getInvertedChannel(dilatedSum);
                        //ConsoleUtil.showImageWindow(dilatedSum, "dilated sum");

                        var openingSum = MorphologicalImageProcessing.getChannelOpening(sum, new int[5, 5]);
                        ConsoleUtil.showImageWindow(openingSum, "first opening");

                        var closing = MorphologicalImageProcessing.getChannelClosing(openingSum, new int[21, 21], Filters.applyMedianFilter, 7);
                        ConsoleUtil.showImageWindow(closing, "sum closing");

                        openingSum = MorphologicalImageProcessing.getChannelOpening(closing, new int[7, 7]);
                        ConsoleUtil.showImageWindow(openingSum, "sum opening");

                        var minOriginalSum = MorphologicalImageProcessing.getMaxFromChannels(openingSum, grayScale);
                        ConsoleUtil.showImageWindow(minOriginalSum, "max original | sum");

                        var eliminatedLinfos = ColorProcessing.getLimiarizedChannel(minOriginalSum, 206, 255, 255, 0);
                        ConsoleUtil.showImageWindow(eliminatedLinfos, "sem linfocitos");

                        var originalXEliminated = MorphologicalImageProcessing.getMinOnAllChannels(channels, eliminatedLinfos);
                        ConsoleUtil.showImageWindow(originalXEliminated, "sem linfocitos | colored");


                        var dilatedLimiarizedGreen = MorphologicalImageProcessing.dilateChannel(limiarizedGreen, new int[5, 5]);
                        ConsoleUtil.showImageWindow(dilatedLimiarizedGreen, "dilated green limiar");

                        originalXEliminated = MorphologicalImageProcessing.getMinOnAllChannels(originalXEliminated, dilatedLimiarizedGreen);
                        ConsoleUtil.showImageWindow(originalXEliminated, "sem plaquetas | colored");

                        var grayOxE = ColorProcessing.getGrayscaleChannel(originalXEliminated, true);
                        ConsoleUtil.showImageWindow(grayOxE, "gray OxE");

                        var limiarizedGrayOxE = ColorProcessing.getLimiarizedChannel(grayOxE, 1, 255, 255, 0);
                        ConsoleUtil.showImageWindow(limiarizedGrayOxE, "limiarized Gray OxE");



                        var openingWfill = MorphologicalImageProcessing.erodeChannel(limiarizedGrayOxE, new int[3, 3], out _);

                        //var fillHolles = MorphologicalImageProcessing.fillHoles(limiarizedGrayOxE);
                        ConsoleUtil.showImageWindow(openingWfill, "filled holes w erosion");

                       */

                        while (true)
                        {
                            int limiar1 = int.Parse(Console.ReadLine());
                            int limiar2 = int.Parse(Console.ReadLine());

                            //limiarized = ColorProcessing.getLimiarizedChannel(grayScale, limiar1, limiar2);
                            //ConsoleUtil.showImageWindow(limiarized, $"limiar [{limiar1},{limiar2}]");
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