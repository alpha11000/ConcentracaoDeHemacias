using System.Drawing.Imaging;

namespace ConcentracaoDeHemacias.Codigos.UI
{
    public partial class exibirImagem : Form
    {
        Bitmap bitmapImage = null;

        public exibirImagem(Bitmap image,  String imageName = "imagem", bool show = true)
        {
            InitializeComponent();

            this.Text = imageName;
            imagem.Image = image;
            bitmapImage = image;

            if (show) Show();
        }

        private void ExportarButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "output.png";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagem.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }
    }
}
