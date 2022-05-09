using ConcentracaoDeHemacias.Codigos.UI;
using ConcentracaoDeHemacias.Codigos.Core;

namespace ConcentracaoDeHemacias.Codigos.Utils
{
    internal class ConsoleUtil
    {

        public static void showImageWindow(int[,] channel, String imageName = "imagem")
        {
            Bitmap input = ColorProcessing.getBitmapFromColorChannel(channel);
            showImageWindow(input, imageName);
        }

        public static void showImageWindow((int[,] R, int[,] G, int[,] B) channels, String imageName = "imagem")
        {
            Bitmap input = ColorProcessing.getBitmapFromColorChannels(channels);
            showImageWindow(input, imageName);
        }

        public static void showImageWindow(Bitmap image, String imageName = "imagem", SaveFileDialog saveDialog = null)
        {
            if(saveDialog == null) saveDialog = new SaveFileDialog();
            Task.Run(() =>
            {
                Application.Run(new exibirImagem(image, saveDialog, imageName));
            });
        }

        public static bool yesNoOptionWrite(string question, int foreColor = -1, int backColor = -1)
        {
            writeColored(question, foreColor, backColor);
            Console.Write("[");
            writeColored("y", (int)ConsoleColor.Green);
            Console.Write("/");
            writeColored("n", (int)ConsoleColor.Red);
            Console.Write("]\n");

            while (true)
            {
                char response = Console.ReadKey().KeyChar;
                response = char.ToLower(response);

                Console.WriteLine();

                switch (response)
                {
                    case 'y': 
                        return true;
                    case 'n':
                        return false;
                    default:
                        writeColoredLine("Opção inválida.", (int)ConsoleColor.Red);
                        break;
                }
            }
        }
        public static void writeColoredLine(string text, int foreColor = -1, int backColor = -1)
        {
            writeColored(text + '\n', foreColor, backColor);
        }

        public static void writeColored(string text, int foreColor = -1, int backColor = -1)
        {
            ConsoleColor foreOriginalColor = Console.ForegroundColor;
            ConsoleColor backOriginalColor = Console.BackgroundColor;

            Console.ForegroundColor = (foreColor == -1) ? foreOriginalColor : (ConsoleColor)foreColor;
            Console.BackgroundColor = (backColor == -1) ? backOriginalColor : (ConsoleColor)backColor;

            Console.Write(text);

            Console.ForegroundColor = foreOriginalColor;
            Console.BackgroundColor = backOriginalColor;

        }
    }
}
