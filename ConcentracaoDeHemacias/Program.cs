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

                        ConsoleUtil.setShow(ConsoleUtil.yesNoOptionWrite("Deseja exibir as imagens obtidas nos processos?"));

                        var channels = ColorProcessing.getAllColorChannels(bmpO);
                        var emptyChannel = MatrixUtil.getFilledMatrixFrom(channels.R);

                        var hChannel = ColorProcessing.getSaturationChannelFromChannels(channels);

                        ConsoleUtil.writeColoredLine("Limiarizando canal HUE [163, 214]...", (int)ConsoleColor.Yellow);
                        var limiarizedH = ColorProcessing.getLimiarizedChannel(hChannel, 163, 214); //75?
                        ConsoleUtil.showImageWindow(limiarizedH, "limiarização Hue");

                        ConsoleUtil.writeColoredLine("realizando fechamento no canal limiarizado (3x3)...", (int)ConsoleColor.Yellow);
                        var closingH = MorphologicalImageProcessing.getChannelClosing(limiarizedH, MatrixUtil.getFilledMatrix(3,3,255));
                        ConsoleUtil.showImageWindow(closingH, "fechamento de Hue");

                        ConsoleUtil.writeColoredLine("realizando abertura (9x9)...", (int)ConsoleColor.Yellow);
                        var openingH = MorphologicalImageProcessing.getChannelOpening(limiarizedH, MatrixUtil.getFilledMatrix(9,9,255));
                        ConsoleUtil.showImageWindow(openingH, "Mapeamento dos Leucócitos");

                        var removedleukocytes = MorphologicalImageProcessing.getMaxOnAllChannels(channels, openingH);
                        ConsoleUtil.showImageWindow(removedleukocytes, "Leucócitos removidos");

                        var minGrayScale = ColorProcessing.getGrayscaleChannel(removedleukocytes, false);
                        ConsoleUtil.showImageWindow(minGrayScale, "minimo entre os canais");

                        ConsoleUtil.writeColoredLine("Limiarizando o valor Min(R, G, B) [115, 255]...", (int)ConsoleColor.Yellow);
                        var limiarizedMinGrayScale = ColorProcessing.getLimiarizedChannel(minGrayScale, 0, 115);
                        ConsoleUtil.showImageWindow(limiarizedMinGrayScale, "limiarização de Min(R, G, B)");

                        var maxRemoveds = MorphologicalImageProcessing.getMaxOnAllChannels(removedleukocytes, limiarizedMinGrayScale);
                        ConsoleUtil.showImageWindow(maxRemoveds, "max all");

                        var grayScaleRemoveds = ColorProcessing.getGrayscaleChannel(maxRemoveds);
                        var limiarizedRemoveds = ColorProcessing.getLimiarizedChannel(grayScaleRemoveds, 207, 255);

                        var openingRemoveds = MorphologicalImageProcessing.getChannelOpening(limiarizedRemoveds, new int[3, 3], Filters.applyMedianFilter, 5);

                        var limiarizedEliminatedColored = MorphologicalImageProcessing.getMaxOnAllChannels(channels, openingRemoveds);


                        ConsoleUtil.showImageWindow(grayScaleRemoveds, "eliminateds");
                        ConsoleUtil.showImageWindow(limiarizedRemoveds, "limiar eliminateds");
                        ConsoleUtil.showImageWindow(openingRemoveds, "opening eliminateds");
                        ConsoleUtil.showImageWindow(limiarizedEliminatedColored, "limiar eliminateds all channels");

                        ConsoleUtil.writeColoredLine("Isolando canais azul e verde...", (int)ConsoleColor.Yellow);
                        var separatedBlueGreen = ColorProcessing.getBitmapFromColorChannels(emptyChannel, limiarizedEliminatedColored.G, limiarizedEliminatedColored.B);
                        
                        var reseparedChannels = ColorProcessing.getAllColorChannels(separatedBlueGreen);
                        ConsoleUtil.showImageWindow(separatedBlueGreen);

                        var grayScaleBlueGreen = ColorProcessing.getGrayscaleChannel(reseparedChannels, true);
                        ConsoleUtil.showImageWindow(grayScaleBlueGreen);

                        ConsoleUtil.writeColoredLine("Limiarizando...", (int)ConsoleColor.Yellow);
                        var limiarBlueGreen = ColorProcessing.getLimiarizedChannel(grayScaleBlueGreen, 198);
                        ConsoleUtil.showImageWindow(limiarBlueGreen, "limiar Blue Green");

                        ConsoleUtil.writeColoredLine("realizando abertura...", (int)ConsoleColor.Yellow);
                        var openingBlueGreen = MorphologicalImageProcessing.getChannelOpening(limiarBlueGreen, new int[3, 3]);
                        ConsoleUtil.showImageWindow(openingBlueGreen, "opening BlueGreen");

                        ConsoleUtil.writeColoredLine("realizando erosão...", (int)ConsoleColor.Yellow);
                        var erodeElim = MorphologicalImageProcessing.erodeChannel(openingBlueGreen, new int[7, 7], out _);
                        ConsoleUtil.showImageWindow(erodeElim, "eroded Elim");

                        var openingOld = MorphologicalImageProcessing.getChannelOpening(limiarizedRemoveds, new int[4, 3], Filters.applyMedianFilter, 3);
                        ConsoleUtil.showImageWindow(openingOld, "opening old");

                        ConsoleUtil.writeColoredLine("realizando dilatação geodésica...", (int)ConsoleColor.Yellow);
                        var geodesicalDilation = MorphologicalImageProcessing.geodesicDilation(erodeElim, openingOld);
                        ConsoleUtil.showImageWindow(geodesicalDilation, "geodesical Dilation");

                        var closingResult = MorphologicalImageProcessing.getChannelClosing(geodesicalDilation, new int[5, 5]);
                        ConsoleUtil.showImageWindow(closingResult, "closing final");

                        ConsoleUtil.writeColoredLine("preenchimento por fechamento...", (int)ConsoleColor.Yellow);
                        var closingFill = MorphologicalImageProcessing.getChannelClosing(closingResult, new int[11, 11]); //RESOLVER
                        ConsoleUtil.showImageWindow(closingFill, "closing fill");

                        ConsoleUtil.writeColoredLine("preenchimento por dilatações geodésicas...(isso pode demorar vários minutos)", (int)ConsoleColor.Yellow);
                        var filledResult = MorphologicalImageProcessing.fillHoles(closingResult);
                        ConsoleUtil.showImageWindow(filledResult, "fill result");

                        ConsoleUtil.writeColoredLine("Obtendo interseção entre os preenchimentos...", (int)ConsoleColor.Yellow);

                        var intersectFill = MorphologicalImageProcessing.getIntersection(closingFill, filledResult);
                        ConsoleUtil.showImageWindow(intersectFill, "intersect fill");

                        int contagem = ImageManagment.countPixelsWithIntensity(intersectFill, 0);
                        float porcentagem = (100.0f * contagem) / (intersectFill.GetLength(0) * intersectFill.GetLength(1));
                        ConsoleUtil.writeColoredLine($"As hemacias ocupam aproximadamente {contagem} pixels da imagem, representando {porcentagem}% da imagem.", (int)ConsoleColor.Green);

                        Console.Read();

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