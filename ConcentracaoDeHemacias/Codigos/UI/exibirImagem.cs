using System.Drawing.Imaging;

namespace ConcentracaoDeHemacias.Codigos.UI
{
    public partial class exibirImagem : Form
    {
        Bitmap bitmapImage = null;
        SaveFileDialog saveFileDialog = null;

        public exibirImagem(Bitmap image, SaveFileDialog saveDialog, String imageName = "imagem", bool show = true)
        {
            InitializeComponent();

            this.Text = imageName;
            imagem.Image = image;
            bitmapImage = image;
            saveFileDialog = saveDialog;

            if (show) Show();
        }

        private void ExportarButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = "output.png";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagem.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }
    }
}
