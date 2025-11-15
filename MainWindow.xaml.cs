using System;
using System.Collections.Generic; // Cần thư viện này
using System.Diagnostics;       // Cần thư viện này
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
using System.Windows.Threading;

namespace BTLT04_fromScratch
{
    public partial class MainWindow : Window
    {
        //Map
        public const int TILE_SIZE = 48; // Đổi thành public const để lớp khác (như Player) có thể thấy
        const int MAP_ROWS = 16;
        const int MAP_COLS = 16;
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

        // --- Logic Game ---
        bool isGameRunning = true;
        private Stopwatch gameTimer = new Stopwatch(); // Đồng hồ cho GameLoop

        // --- Player ---
        Player player;
        bool moveUp = false, moveDown = false, moveLeft = false, moveRight = false;
        int playerDefaultSpawnX, playerDefaultSpawnY;

        // --- Enemy (Tạm thời dùng List<Image> theo yêu cầu) ---
        DispatcherTimer spawnTimer;
        int currentWave = 0;
        int enemyAlive = 0, enemyToSpawn = 0;
        Random random = new Random();
        List<Image> activeEnemies = new List<Image>();

        // --- Sprites ---
        BitmapImage Desert1, Desert2, Desert3, Desert4, Desert6;
        BitmapImage Bullet;
        BitmapImage Orc0, Orc1;

        // --- Âm thanh ---
        MediaPlayer bgMusic = new MediaPlayer(); string musicPath;
        string gunShotPath;
        string destroyedPath;
        MediaPlayer GameOver = new MediaPlayer(); string gameOverPath;


        public MainWindow()
        {
            InitializeComponent();

            LoadAssets();
            DrawMap();
            PlayBackgroundMusic();

            // Player
            CreatePlayer(); // Tạo Player

            // Đăng ký GameLoop (Chỉ 1 lần)
            CompositionTarget.Rendering += GameLoop;
            gameTimer.Start();

            // Bắt input (Chỉ 1 bộ)
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;

            // Bắt đầu Wave
            StartNextWave();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!isGameRunning) return;

            // 1. Tính DeltaTime (Thời gian giữa các frame)
            TimeSpan ts = gameTimer.Elapsed;
            gameTimer.Restart();
            double deltaTime = ts.TotalSeconds;

            // 2. Xử lý Input và Di chuyển Player
            double dx = 0, dy = 0;
            if (moveUp) dy -= player.Speed;
            if (moveDown) dy += player.Speed;
            if (moveLeft) dx -= player.Speed;
            if (moveRight) dx += player.Speed;

            if (dx != 0 || dy != 0)
            {
                player.IsMoving = true;
                player.Move(dx, dy);
            }
            else
            {
                player.IsMoving = false;
            }

            // 3. Cập nhật Hoạt ảnh Player (DÙ ĐỨNG YÊN HAY DI CHUYỂN)
            player.Update(deltaTime);

            // 4. TODO: Cập nhật Kẻ địch (Nâng cấp sau)
            // (Khi bạn nâng cấp lên List<Enemy>, bạn sẽ lặp qua đây)
            // foreach (var enemy in activeEnemies)
            // {
            //     enemy.UpdateAnimation(deltaTime);
            //     enemy.UpdateMovement(player, deltaTime);
            // }

            // 5. TODO: Cập nhật Đạn
            // (Bạn cần tạo lớp Bullet.cs và List<Bullet>)
            // foreach (var bullet in activeBullets)
            // {
            //     bullet.Update(deltaTime);
            // }

            // 6. TODO: Xử lý Va chạm
            // (Đây là nơi bạn sẽ code logic va chạm)
            //
            // Va chạm Đạn vs Quái:
            // if (bullet.HitRect.IntersectsWith(enemy.HitRect))
            // {
            //     enemy.IsDead = true;
            //     bullet.IsDead = true;
            //     OnEnemyKilled(enemy); // Gọi hàm xử lý
            // }
            //
            // Va chạm Quái vs Player:
            // if (enemy.HitRect.IntersectsWith(player.HitRect))
            // {
            //     EndGame();
            // }

            // 7. TODO: Dọn dẹp
            // (Dọn dẹp quái và đạn đã chết)
        }

        void CreatePlayer()
        {
            player = new Player(playerDefaultSpawnX, playerDefaultSpawnY);

            // Thêm CẢ HAI visual vào Canvas
            GameCanvas.Children.Add(player.PlayerLegsVisual);
            GameCanvas.Children.Add(player.PlayerBodyVisual);

            // Cập nhật vị trí ban đầu
            player.Move(0, 0);

            // Đăng ký sự kiện OnShoot từ Player
            player.OnShoot += Player_OnShoot;
        }

        // Hàm này được gọi khi Player.Shoot() được kích hoạt
        private void Player_OnShoot(Point target)
        {
            PlayGunShot(); // Phát âm thanh

            // TODO: (Thành viên 3) Tạo logic đạn bay
            // SpawnBullet(player.X, player.Y, target);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W) { moveUp = true; player.Facing = Direction.Up; }
            if (e.Key == Key.Down || e.Key == Key.S) { moveDown = true; player.Facing = Direction.Down; }
            if (e.Key == Key.Left || e.Key == Key.A) { moveLeft = true; player.Facing = Direction.Left; }
            if (e.Key == Key.Right || e.Key == Key.D) { moveRight = true; player.Facing = Direction.Right; }

            // Sửa lỗi: Không cần gọi UpdateBodySprite ở đây,
            // vì hàm player.Move() đã tự gọi nó rồi.
            // player.UpdateBodySprite(); 
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W) moveUp = false;
            if (e.Key == Key.Down || e.Key == Key.S) moveDown = false;
            if (e.Key == Key.Left || e.Key == Key.A) moveLeft = false;
            if (e.Key == Key.Right || e.Key == Key.D) moveRight = false;
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isGameRunning) return; // Không cho bắn khi đã Game Over
            Point target = e.GetPosition(GameCanvas);
            player.Shoot(target); // Gọi hàm Shoot của Player
        }

        void LoadAssets()
        {
            // (Dùng Pack URI để đảm bảo load đúng)
            string envPath = "pack://application:,,,/BTLT04_fromScratch;component/Assets/Sprites/Sprite16x16/Environment/";
            Desert1 = new BitmapImage(new Uri(envPath + "Desert1.png"));
            Desert2 = new BitmapImage(new Uri(envPath + "Desert2.png"));
            Desert3 = new BitmapImage(new Uri(envPath + "Desert3.png"));
            Desert4 = new BitmapImage(new Uri(envPath + "Desert4.png"));
            Desert6 = new BitmapImage(new Uri(envPath + "Desert6.png"));

            string otherPath = "pack://application:,,,/BTLT04_fromScratch;component/Assets/Sprites/Sprite16x16/Other/";
            Bullet = new BitmapImage(new Uri(otherPath + "Bullet0.png"));

            string enemyPath = "pack://application:,,,/BTLT04_fromScratch;component/Assets/Sprites/Sprite16x16/Enemy/";
            Orc0 = new BitmapImage(new Uri(enemyPath + "Orc0.png"));
            Orc1 = new BitmapImage(new Uri(enemyPath + "Orc1.png"));

            // (Âm thanh là 'Content', dùng đường dẫn tương đối)
            musicPath = "Assets/Audio/Overworld.wav";
            gunShotPath = "Assets/Audio/GunShot.wav";
            destroyedPath = "Assets/Audio/Destroyed0.wav";
            gameOverPath = "Assets/Audio/GameOver.wav";

            playerDefaultSpawnX = 8 * TILE_SIZE;
            playerDefaultSpawnY = 8 * TILE_SIZE;
        }

        void DrawMap()
        {
            for (int r = 0; r < MAP_ROWS; r++)
            {
                for (int c = 0; c < MAP_COLS; c++)
                {
                    Image tile = new Image();
                    tile.Width = TILE_SIZE;
                    tile.Height = TILE_SIZE;
                    RenderOptions.SetBitmapScalingMode(tile, BitmapScalingMode.NearestNeighbor);

                    switch (mapMatrix[r, c])
                    {
                        case 1: tile.Source = Desert1; break;
                        case 2: tile.Source = Desert2; break;
                        case 3: tile.Source = Desert3; break;
                        case 4: tile.Source = Desert4; break;
                        case 6: tile.Source = Desert6; break;
                    }
                    // (Sửa lỗi: Xóa dấu ; thừa ở đây)

                    Canvas.SetLeft(tile, c * TILE_SIZE);
                    Canvas.SetTop(tile, r * TILE_SIZE);
                    Panel.SetZIndex(tile, -100);
                    GameCanvas.Children.Add(tile);
                }
            }
        }

        void StartNextWave()
        {
            currentWave++;
            enemyToSpawn = 5 + (currentWave - 1) * 3;
            enemyAlive = 0;

            if (spawnTimer == null)
            {
                spawnTimer = new DispatcherTimer();
                spawnTimer.Interval = TimeSpan.FromSeconds(1);
                spawnTimer.Tick += OnSpawnTimerTick;
            }
            spawnTimer.Start();
        }

        void OnSpawnTimerTick(object sender, EventArgs e)
        {
            if (enemyToSpawn > 0)
            {
                SpawnOneEnemy();
                enemyToSpawn--;
                enemyAlive++;
            }
            else
            {
                spawnTimer.Stop();
            }
        }

        void SpawnOneEnemy()
        {
            // Tạm thời tạo Image
            Image enemySprite = new Image
            {
                Source = Orc0, // Tạm thời dùng Orc0 (chưa có hoạt ảnh)
                Width = 48,
                Height = 48
            };
            RenderOptions.SetBitmapScalingMode(enemySprite, BitmapScalingMode.NearestNeighbor);

            // --- Tính toán Tọa độ ---
            double spawnX = 0;
            double spawnY = 0;
            int side = random.Next(0, 4);
            double mapSize = 768;

            switch (side)
            {
                case 0: // Cạnh TRÊN
                    spawnX = random.Next(288, 480);
                    spawnY = 0;
                    break;
                case 1: // Cạnh DƯỚI
                    spawnX = random.Next(288, 480);
                    spawnY = mapSize - 48;
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

            Canvas.SetLeft(enemySprite, spawnX);
            Canvas.SetTop(enemySprite, spawnY);
            Panel.SetZIndex(enemySprite, 10);
            GameCanvas.Children.Add(enemySprite);

            // Thêm quái này vào danh sách quản lý
            activeEnemies.Add(enemySprite);
            // (Sửa lỗi: Xóa 6 dòng code lặp lại ở cuối hàm cũ của bạn)
        }

        // Hàm này sẽ được gọi bởi logic va chạm (TODO 6)
        public async void OnEnemyKilled(Image enemyThatDied)
        {
            activeEnemies.Remove(enemyThatDied);
            GameCanvas.Children.Remove(enemyThatDied);
            PlayDestroyedSound(); // Sửa: Gọi hàm âm thanh mới

            enemyAlive--;

            if (enemyToSpawn == 0 && enemyAlive == 0)
            {
                await Task.Delay(3000);
                StartNextWave();
            }
        }

        public void EndGame()
        {
            if (isGameRunning == false) return;
            isGameRunning = false;

            // Dừng GameLoop và Timer
            CompositionTarget.Rendering -= GameLoop; // Dừng GameLoop
            gameTimer.Stop();
            if (spawnTimer != null) spawnTimer.Stop();

            bgMusic.Stop();
            PlayGameOverMusic();

            GameOverMenu.Visibility = Visibility.Visible;
        }

        public void RestartGame()
        {
            GameOverMenu.Visibility = Visibility.Collapsed;
            isGameRunning = true;

            // Dọn dẹp quái cũ
            foreach (var img in activeEnemies)
            {
                GameCanvas.Children.Remove(img);
            }
            activeEnemies.Clear();

            // Dọn dẹp Player cũ
            GameCanvas.Children.Remove(player.PlayerBodyVisual);
            GameCanvas.Children.Remove(player.PlayerLegsVisual);

            // Dọn dẹp Map (và mọi thứ còn sót lại)
            GameCanvas.Children.Clear();

            // Vẽ lại
            DrawMap();
            CreatePlayer(); // Tạo lại Player

            // Reset logic
            currentWave = 0;
            enemyToSpawn = 0;
            enemyAlive = 0;

            PlayBackgroundMusic();

            // Khởi động lại GameLoop
            CompositionTarget.Rendering += GameLoop;
            gameTimer.Restart();

            StartNextWave();
        }

        // --- HỆ THỐNG ÂM THANH (Sửa lỗi chồng chéo) ---
        void PlayBackgroundMusic()
        {
            bgMusic.Open(new Uri(musicPath, UriKind.Relative));
            bgMusic.Volume = 0.5;
            bgMusic.MediaEnded += (sender, e) =>
            {
                bgMusic.Position = TimeSpan.Zero;
                bgMusic.Play();
            };
            bgMusic.Play();
        }

        // SFX "Bắn và Quên" (Fire and Forget)
        void PlayGunShot()
        {
            MediaPlayer sfx = new MediaPlayer();
            sfx.Open(new Uri(gunShotPath, UriKind.Relative));
            sfx.Volume = 0.8;
            sfx.MediaEnded += (s, e) => { sfx = null; };
            sfx.Play();
        }

        void PlayDestroyedSound()
        {
            MediaPlayer sfx = new MediaPlayer();
            sfx.Open(new Uri(destroyedPath, UriKind.Relative));
            sfx.Volume = 1.0;
            sfx.MediaEnded += (s, e) => { sfx = null; };
            sfx.Play();
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