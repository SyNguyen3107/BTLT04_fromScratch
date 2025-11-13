using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BTLT04_fromScratch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int TILE_SIZE = 48;
        const int MAP_ROWS = 16;
        const int MAP_COLS = 16;
        // số trong ma trận tương ứng với STT của ảnh trong thư mực Assets/Sprites/Sprite16x16/Environment/
        int[,] mapMatrix = new int[MAP_ROWS, MAP_COLS] {
                { 6, 6, 6, 6, 6, 6, 6, 1, 1, 1, 6, 6, 6, 6, 6, 6 },
                { 6, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 2, 6 },
                { 6, 2, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 1, 2, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 2, 1 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 1 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 2, 1 },
                { 6, 2, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 2, 6 },
                { 6, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6 },
                { 6, 6, 6, 6, 6, 6, 6, 1, 1, 1, 6, 6, 6, 6, 6, 6 }
            };
        BitmapImage Desert1, Desert2, Desert3, Desert4, Desert6;
        public MainWindow()
        {
            InitializeComponent();
            
            LoadAssets();
            DrawMap();

        }
        void LoadAssets()
        {
            // UriKind.Relative giúp tìm đúng file trong project
            Desert1 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert1.png", UriKind.Relative));
            Desert2 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert2.png", UriKind.Relative));
            Desert3 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert3.png", UriKind.Relative));
            Desert4 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert4.png", UriKind.Relative));
            Desert6 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert6.png", UriKind.Relative));
            
        }
        void DrawMap()
        {
            for (int r = 0; r < MAP_ROWS; r++)
            {
                for (int c = 0; c < MAP_COLS; c++)
                {
                    // Tạo đối tượng Image mới
                    Image tile = new Image();

                    // Set kích thước ĐÃ PHÓNG TO (32x32)
                    tile.Width = TILE_SIZE;
                    tile.Height = TILE_SIZE;

                    // CHỐNG MỜ ẢNH (Pixel Art Scale)
                    RenderOptions.SetBitmapScalingMode(tile, BitmapScalingMode.NearestNeighbor);

                    switch (mapMatrix[r, c])
                    {
                        case 1:
                            tile.Source = Desert1;
                            break;
                        case 2:
                            tile.Source = Desert2;
                            break;
                        case 3:
                            tile.Source = Desert3;
                            break;
                        case 4:
                            tile.Source = Desert4;
                            break;
                        case 6:
                            tile.Source = Desert6;
                            break;
                    };

                    // Đặt vị trí trên Canvas (X = Cột * 32, Y = Hàng * 32)
                    Canvas.SetLeft(tile, c * TILE_SIZE);
                    Canvas.SetTop(tile, r * TILE_SIZE);

                    // Đưa tile xuống lớp dưới cùng (Z-Index) để nhân vật đi đè lên
                    Panel.SetZIndex(tile, -100);

                    // Thêm vào Canvas
                    GameCanvas.Children.Add(tile);
                }
            }
        }
    }
}