using System;
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
using System.Threading.Tasks;

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

        bool isGameRunning = true; // Biến trạng thái game
        System.Windows.Threading.DispatcherTimer spawnTimer;// Timer dùng để spawn kẻ địch định kỳ
        int currentWave = 0;
        int enemyAlive = 0, enemyToSpawn = 0; // Biến đếm kẻ địch còn lại/tổng cần spawn
        Random random = new Random();
        List<Image> activeEnemies = new List<Image>();

        int[,] mapMatrix = new int[MAP_ROWS, MAP_COLS] {
                { 6, 6, 6, 6, 6, 6, 1, 1, 1, 1, 6, 6, 6, 6, 6, 6 },
                { 6, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 3, 2, 6 },
                { 6, 2, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 1, 2, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 2, 1 },
                { 1, 2, 4, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 2, 1 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 1 },
                { 1, 2, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 2, 1 },
                { 6, 2, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 6 },
                { 6, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 2, 6 },
                { 6, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6 },
                { 6, 6, 6, 6, 6, 6, 1, 1, 1, 1, 6, 6, 6, 6, 6, 6 }
            };
        //Các sprite ảnh môi trường cần dùng
        BitmapImage Desert1, Desert2, Desert3, Desert4, Desert6;

        //Các sprite ảnh nhân vật cần dùng
        BitmapImage PlayerDown, PlayerUp, PlayerLeft, PlayerRight, PlayerLeg0, PlayerLeg1, PlayerLeg2, PlayerLeg3;

        //Spite đạn
        BitmapImage Bullet;

        //Các sprite ảnh kẻ địch cần dùng
        BitmapImage Orc0, Orc1;

        //Mỗi âm thanh cần 1 MediaPlayer riêng
        MediaPlayer bgMusic = new MediaPlayer(); string musicPath;
        MediaPlayer GunShot = new MediaPlayer(); string gunShotPath;
        MediaPlayer Destroyed = new MediaPlayer(); string destroyedPath;
        MediaPlayer GameOver = new MediaPlayer(); string gameOverPath;
        public MainWindow()
        {
            InitializeComponent();

            LoadAssets();
            DrawMap();
            PlayBackgroundMusic();
            StartNextWave();
        }
        void LoadAssets()
        {
            // UriKind.Relative giúp tìm đúng file trong project
            Desert1 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert1.png", UriKind.Relative));
            Desert2 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert2.png", UriKind.Relative));
            Desert3 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert3.png", UriKind.Relative));
            Desert4 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert4.png", UriKind.Relative));
            Desert6 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Environment/Desert6.png", UriKind.Relative));

            Bullet = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Other/Bullet0.png", UriKind.Relative));

            Orc0 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Enemy/Orc0.png", UriKind.Relative));
            Orc1 = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Enemy/Orc1.png", UriKind.Relative));

            musicPath = "Assets/Audio/Overworld.wav";
            gunShotPath = "Assets/Audio/GunShot.wav";
            destroyedPath = "Assets/Audio/Destroyed0.wav";
            gameOverPath = "Assets/Audio/GameOver.wav";
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
                    }
                    ;

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
        void StartNextWave()
        {
            currentWave++;

            // Tính toán số lượng quái cho đợt này
            // Ví dụ: 5 quái, rồi 8, rồi 11...
            enemyToSpawn = 5 + (currentWave - 1) * 3;

            // Lúc bắt đầu đợt, chưa có con nào sống
            enemyAlive = 0;

            // Nếu timer chưa được tạo, thì tạo mới
            if (spawnTimer == null)
            {
                spawnTimer = new System.Windows.Threading.DispatcherTimer();
                spawnTimer.Interval = TimeSpan.FromSeconds(1); // Tốc độ spawn (1 giây/con)
                spawnTimer.Tick += OnSpawnTimerTick; // Gắn sự kiện Tick
            }

            // Bật Timer để bắt đầu spawn
            spawnTimer.Start();
        }
        void OnSpawnTimerTick(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem còn quái trong đợt này không
            if (enemyToSpawn > 0)
            {
                SpawnOneEnemy();
                enemyToSpawn--;
                enemyAlive++;
            }
            else
            {
                // 2. Nếu đã spawn đủ số lượng của đợt này
                // Timer tự động dừng lại và đợi
                spawnTimer.Stop();
            }
        }
        void SpawnOneEnemy()
        {
            // LƯU Ý QUAN TRỌNG:
            //TẠM THỜI để là ảnh kẻ địch, sau này sẽ đổi thành đối tượng Enemy
            Image enemySprite = new Image();

            enemySprite.Source = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Enemy/Orc0.png", UriKind.Relative));
            enemySprite.Width = 48; 
            enemySprite.Height = 48;
            RenderOptions.SetBitmapScalingMode(enemySprite, BitmapScalingMode.NearestNeighbor);

            // --- THUẬT TOÁN TÍNH TOẠ ĐỘ ---
            double spawnX = 0;
            double spawnY = 0;

            // Random từ 0 đến 3 để chọn cạnh: 0=Trên, 1=Dưới, 2=Trái, 3=Phải
            int side = random.Next(0, 4);

            // Kích thước vùng chơi
            double mapSize = 768;

            switch (side)
            {
                case 0: // Cạnh TRÊN
                    spawnX = random.Next(288, 480); // Random chiều ngang
                    spawnY =0; // Sát mép trên
                    break;

                case 1: // Cạnh DƯỚI
                    spawnX = spawnX = random.Next(288, 480); ;
                    spawnY = mapSize - 48; // Sát mép dưới (trừ đi chiều cao quái)
                    break;

                case 2: // Cạnh TRÁI
                    spawnX = 0;
                    spawnY = random.Next(288, 480);
                    break;

                case 3: // Cạnh PHẢI
                    spawnX = mapSize - 48;
                    spawnY = random.Next(288, 480);
                    break;
            }

            // Đặt vị trí cho quái
            Canvas.SetLeft(enemySprite, spawnX);
            Canvas.SetTop(enemySprite, spawnY);

            // Đưa quái lên lớp trên cùng (trên sàn)
            Panel.SetZIndex(enemySprite, 10);

            // Thêm vào Canvas
            GameCanvas.Children.Add(enemySprite);

            //Thêm quái này vào danh sách quản lý
            activeEnemies.Add(enemySprite);
        }
        public async void OnEnemyKilled(Image enemyThatDied)
        {
            activeEnemies.Remove(enemyThatDied);
            GameCanvas.Children.Remove(enemyThatDied);
            Destroyed.Play();

            // 1. Trừ số quái đang sống
            enemyAlive--;

            // 2. KIỂM TRA QUA MÀN
            // Nếu timer đã dừng (spawn hết) VÀ quái cũng bị giết hết
            if (enemyToSpawn == 0 && enemyAlive == 0)
            {
                await Task.Delay(3000);
                // Bắt đầu đợt tiếp theo
                StartNextWave();
            }
        }
        public void EndGame()
        {
            // Nếu game đã kết thúc rồi thì không làm gì thêm (tránh gọi chồng chéo)
            if (isGameRunning == false) return;

            // 1. Đánh dấu game dừng
            isGameRunning = false;

            // 2. Dừng nhạc nền (Nếu muốn) hoặc phát nhạc Game Over
            bgMusic.Stop(); // Bạn cần viết thêm hàm Stop trong AudioPlayer nếu muốn
            PlayGameOverMusic();

            // 3. Hiện Menu Game Over
            GameOverMenu.Visibility = Visibility.Visible;
        }
        public void RestartGame()
        {
            // 1. Ẩn Menu Game Over
            GameOverMenu.Visibility = Visibility.Collapsed;

            // 2. Reset trạng thái
            isGameRunning = true;

            // 3. Dọn dẹp Map cũ (CỰC KỲ QUAN TRỌNG)
            // Nếu không có dòng này, map mới sẽ vẽ đè lên map cũ -> Tốn RAM, lag game
            GameCanvas.Children.Clear();

            // 4. Vẽ lại Map và các đối tượng ban đầu
            DrawMap();

            // 5. Reset nhạc nền (nếu nãy đã tắt)
            PlayBackgroundMusic();
        }
        void PlayBackgroundMusic()
        {
            // Đường dẫn tương đối tính từ file .exe (trong thư mục bin/Debug)
            bgMusic.Open(new Uri(musicPath, UriKind.Relative));
            bgMusic.Volume = 0.5;
            // Loop
            // MediaPlayer không có thuộc tính Loop=true, ta dùng sự kiện MediaEnded
            bgMusic.MediaEnded += (sender, e) =>
            {
                bgMusic.Position = TimeSpan.Zero;
                bgMusic.Play();
            };

            bgMusic.Play();
        }
        void PlayGunShot()
        {
            GunShot.Open(new Uri(gunShotPath, UriKind.Relative));
            GunShot.Volume = 0.8;
            GunShot.Play();
        }
        void PlayGameOverMusic()
        {
            GameOver.Open(new Uri(gameOverPath, UriKind.Relative));
            GameOver.Volume = 0.7;
            GameOver.Play();
        }
        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Retry_btn_Click(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }
    }
}