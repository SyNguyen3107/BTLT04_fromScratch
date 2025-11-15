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
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
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


        // Bộ hỗ trợ đo thời gian cho vòng lặp game
        Stopwatch loopStopwatch = new Stopwatch();
        long lastTicks = 0;

        public MainWindow()
        {
            InitializeComponent();

            LoadAssets();
            DrawMap();
            PlayBackgroundMusic();
            StartNextWave();



            // KeyDown để bắn bằng phím mũi tên (Up/Down/Left/Right)
            this.KeyDown += MainWindow_KeyDown;

            // Player
            CreatePlayer(); // Tạo Player

            // Đăng ký GameLoop (Chỉ 1 lần)
            CompositionTarget.Rendering += GameLoop;
            gameTimer.Start();

            // Bắt input (Chỉ 1 bộ)
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;




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
            foreach ( var enemy in activeEnemies)
            {
                enemy.Update(player);
            }    
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
            player.Shoot(target);

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

        void AddPlayerToCanvas()
        {
            PlayerSprite = new Image();
            PlayerSprite.Source = PlayerDown;
            PlayerSprite.Width = 48;
            PlayerSprite.Height = 48;
            RenderOptions.SetBitmapScalingMode(PlayerSprite, BitmapScalingMode.NearestNeighbor);
            Canvas.SetLeft(PlayerSprite, playerX);
            Canvas.SetTop(PlayerSprite, playerY);
            Panel.SetZIndex(PlayerSprite, 20);
            GameCanvas.Children.Add(PlayerSprite);
        }

        void StartGameLoop()
        {
            gameLoopTimer = new System.Windows.Threading.DispatcherTimer();
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            gameLoopTimer.Tick += GameLoop_Tick;
            loopStopwatch.Start();
            lastTicks = loopStopwatch.ElapsedMilliseconds;
            gameLoopTimer.Start();
        }

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            if (!isGameRunning) return;

            long now = loopStopwatch.ElapsedMilliseconds;
            double delta = (now - lastTicks) / 1000.0;
            if (delta <= 0) return;
            lastTicks = now;

            // 1. Cập nhật bullet
            foreach (var b in activeBullets)
            {
                b.Update(delta);
            }

           
            

            // 3. Kiểm tra va chạm: Đạn vs Kẻ địch
            // Đánh dấu cờ IsDead; KHÔNG được xóa trong lúc đang duyệt
            foreach (var b in activeBullets)
            {
                if (b.IsDead) continue;
                Rect br = b.GetRect();

                foreach (var en in activeEnemies)
                {
                    if (en.IsDead) continue;
                    Rect er = en.GetRect();

                    if (br.IntersectsWith(er))
                    {
                        b.IsDead = true;
                        en.IsDead = true;
                        // KHÔNG xóa ở đây; việc xóa sẽ thực hiện sau khi vòng lặp va chạm kết thúc
                    }
                }
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
            Enemy enemyObj = new Enemy(spawnX, spawnY);
            // Đặt vị trí cho quái
            Canvas.SetLeft(enemyObj.EnemyVisual, spawnX);
            Canvas.SetTop(enemyObj.EnemyVisual, spawnY);

            // Đưa quái lên lớp trên cùng (trên sàn)
            Panel.SetZIndex(enemyObj.EnemyVisual, 10);

            // Thêm vào Canvas
            GameCanvas.Children.Add(enemyObj.EnemyVisual);

            // Thêm quái này vào danh sách quản lý (Enemy object)
            
            activeEnemies.Add(enemyObj);
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

        // Mouse shooting: tạo đạn từ vị trí người chơi bay về vị trí con trỏ
       

        // Bắn theo vector đã cho (dùng cho phím mũi tên)
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Vector dir = new Vector(0, 0);
            switch (e.Key)
            {
                case Key.Up:
                    dir = new Vector(0, -1);
                    playerFacing = dir;
                    break;
                case Key.Down:
                    dir = new Vector(0, 1);
                    playerFacing = dir;
                    break;
                case Key.Left:
                    dir = new Vector(-1, 0);
                    playerFacing = dir;
                    break;
                case Key.Right:
                    dir = new Vector(1, 0);
                    playerFacing = dir;
                    break;
                default:
                    return; // không phải phím bắn
            }

            ShootInDirection(dir);
        }

        // Tạo viên đạn hướng về target (toạ độ trên Canvas)
        void ShootAt(Point target)
        {
            // Tâm của player
            double px = playerX + PlayerSprite.Width / 2;
            double py = playerY + PlayerSprite.Height / 2;

            Vector dir = new Vector(target.X - px, target.Y - py);
            if (dir.LengthSquared == 0) return;
            dir.Normalize();
            playerFacing = dir;

            SpawnBullet(px, py, dir);
        }

        // Tạo viên đạn theo hướng đã chuẩn hóa
        void ShootInDirection(Vector direction)
        {
            if (direction.LengthSquared == 0) return;
            direction.Normalize();

            double px = playerX + PlayerSprite.Width / 2;
            double py = playerY + PlayerSprite.Height / 2;

            SpawnBullet(px, py, direction);
        }

        // Tạo Image cho viên đạn, thêm vào Canvas và danh sách quản lý
        void SpawnBullet(double centerX, double centerY, Vector dir)
        {
            // Image hiển thị cho viên đạn
            Image bulletSprite = new Image
            {
                Source = Bullet,
                Width = 16,
                Height = 16
            };
            RenderOptions.SetBitmapScalingMode(bulletSprite, BitmapScalingMode.NearestNeighbor);

            // Spawn tại tâm player (đẩy ra 1 chút nếu cần)
            double spawnX = centerX - bulletSprite.Width / 2;
            double spawnY = centerY - bulletSprite.Height / 2;

            // Đặt vị trí và z-index rồi thêm vào Canvas
            Canvas.SetLeft(bulletSprite, spawnX);
            Canvas.SetTop(bulletSprite, spawnY);
            Panel.SetZIndex(bulletSprite, 15);
            GameCanvas.Children.Add(bulletSprite);

            // Tạo đối tượng Bullet quản lý logic
            var bullet = new Bullet(spawnX, spawnY, dir, bulletSprite);
            activeBullets.Add(bullet);

            PlayGunShot();
        }
    }
}