using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Analisis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //LoadImageFromPath(@"");
        }

        private void Button_Click_Load(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                LoadImageFromPath(filename);

            }
        }

        private void LoadImageFromPath(string filename)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filename, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            ImageLeft.Source = src;
            ImageLeft.Stretch = Stretch.Uniform;
        }

        private void Button_Click_Analize(object sender, RoutedEventArgs e)
        {
            var src = ImageLeft.Source as BitmapImage;

            int stride = src.PixelWidth * 4;
            int size = src.PixelHeight * stride;
            byte[] pixels = new byte[size];
            src.CopyPixels(pixels, stride, 0);
            
            Results.Text = "El Resultado del análisis ha sido...\n" + AnalizePixels(pixels, src.PixelHeight, src.PixelWidth); 
        }

        private string AnalizePixels(byte[] pixels, int pixelHeight, int pixelWidth)
        {
            //int size = pixelHeight * stride;
            var msg = new StringBuilder();

            for (int x = 0; x < pixelWidth; x++)
            {
                for (int y = 0; y < pixelHeight; y++)
                {
                    var positionClass = CheckAround(x, y, pixels, pixelHeight, pixelWidth);

                    if(positionClass != PositionClass.None)
                    {
                        msg.AppendLine($"Pixel x:{x} e y:{y} chungele");
                    }

                }
            }

            if(msg.Length == 0)
            {
                msg.AppendLine("Ningún pixel chungele");
            }

            return msg.ToString();
        }

        private PositionClass CheckAround(int xCenter, int yCenter, byte[] pixels, int pixelHeight, int pixelWidth)
        {
            int stride = GetStridePixel(pixelWidth);
            int indexCenter = GetIndexPixel(xCenter, yCenter, stride);


            var shape = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(-1,1),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,1),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,-1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,-1)
            };

            var alertPixels = new List<Tuple<int, int>>();


            byte redCenter = pixels[indexCenter];
            byte greenCenter = pixels[indexCenter + 1];
            byte blueCenter = pixels[indexCenter + 2];
            byte alphaCenter = pixels[indexCenter + 3];


            foreach (var point in shape)
            {
                var x = xCenter + point.Item1;
                var y = yCenter + point.Item2;

                if (x >= 0 && x < pixelWidth && y >= 0 && y < pixelHeight)
                {
                    var indexPoint = GetIndexPixel(x, y, stride);

                    byte redPoint = pixels[indexCenter];
                    byte greenPoint = pixels[indexCenter + 1];
                    byte bluePoint = pixels[indexCenter + 2];
                    byte alphaPoint = pixels[indexCenter + 3];


                    if ((redPoint ^ redCenter ) == 1
                        || (bluePoint ^ blueCenter ) == 1
                        || (greenPoint ^ greenCenter) == 1
                        || (alphaPoint ^ alphaCenter ) == 1)
                    {
                        alertPixels.Add(point);
                    }
                }
            }

            if(shape.Count() < (alertPixels.Count() + shape.Count() / 2))
            {
                return PositionClass.Plausible;
            }


            return PositionClass.None;
        }

        private int GetStridePixel(int pixelWidth)
        {
            return pixelWidth * 4;
        }

        private int GetIndexPixel(int x, int y, int stride)
        {
            return y * stride + 4 * x;
        }
    }
}
