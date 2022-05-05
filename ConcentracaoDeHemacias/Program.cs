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

                ConsoleUtil.writeColoredLine("Escolha uma imagem de entrada:", (int)ConsoleColor.Green);

                if(_ofd.ShowDialog() == DialogResult.OK){
                    try{
                        bmpO = new Bitmap(_ofd.FileName);
                        Bitmap bmp = new Bitmap(bmpO);
                        ConsoleUtil.showImageWindow(bmp, "Imagem de entrada");
                        var channels = ColorProcessing.getAllColorChannels(bmpO);
                        int[,] gray = ColorProcessing.getGrayscaleChannel(channels);
                        Bitmap grayB = ColorProcessing.getBitmapFromColorChannels(gray, gray, gray);

                        ConsoleUtil.showImageWindow(grayB, "gray");

                        int[,] limiar = ColorProcessing.getLimiarizedChannel(gray, 100, 255, 0);
                        Bitmap limiarB = ColorProcessing.getBitmapFromColorChannels(limiar, limiar, limiar);

                        ConsoleUtil.showImageWindow(limiarB, "limiar");

                        int[,] equalized = HistogramProcessing.getEqualizedChannel(gray);
                        Bitmap equalizedB = ColorProcessing.getBitmapFromColorChannels(equalized, equalized, equalized);

                        ConsoleUtil.showImageWindow(equalizedB, "Equalized");

                        int[,] limiar2 = ColorProcessing.getLimiarizedChannel(equalized, int.Parse(Console.ReadLine()), 255, 0);
                        Bitmap limiarB2 = ColorProcessing.getBitmapFromColorChannels(limiar2, limiar2, limiar2);

                        ConsoleUtil.showImageWindow(limiarB2, "Equalized limiar");

                        break;
                    }
                    catch{
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