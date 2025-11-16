using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace BTLT04_fromScratch
{
    public partial class MainWindow : Window
    {
        //--- Map ---
        BitmapImage Desert1, Desert2, Desert3, Desert4, Desert6;
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
        int playerDefaultSpawnX = 8 * TILE_SIZE, playerDefaultSpawnY = 8 * TILE_SIZE;

        // --- Enemy ---
        BitmapImage Orc0, Orc1;
        DispatcherTimer spawnTimer;
        int currentWave = 0;
        int enemyAlive = 0, enemyToSpawn = 0;
        Random random = new Random();
        List<Enemy> activeEnemies = new List<Enemy>();    
        
        // --- Âm thanh ---
        MediaPlayer bgMusic = new MediaPlayer(); string musicPath;
        string gunShotPath;
        string destroyedPath;
        MediaPlayer GameOver = new MediaPlayer(); string gameOverPath;

        //--- Đạn ---
        BitmapImage Bullet;
        List<Bullet> activeBullets = new List<Bullet>();// danh sách quản lý các viên đạn đang hoạt động

        public MainWindow()
        {
            InitializeComponent();

            LoadAssets();
            DrawMap();
            PlayBackgroundMusic();

            CreatePlayer();

            CompositionTarget.Rendering += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;
            StartNextWave();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            if (!isGameRunning) return;

            // 1. Tính DeltaTime
            TimeSpan ts = gameTimer.Elapsed;
            gameTimer.Restart();
            double delta = ts.TotalSeconds;
            if (delta <= 0) return;

            // 2. Xử lý Input và Di chuyển Player
            double dx = 0, dy = 0;
            if (moveUp) dy -= player.Speed;
            if (moveDown) dy += player.Speed;
            if (moveLeft) dx -= player.Speed;
            if (moveRight) dx += player.Speed;

            player.IsMoving = (dx != 0 || dy != 0);
            if (player.IsMoving) player.Move(dx, dy);
            player.Update(delta);

            // 3. Cập nhật bullet
            foreach (var b in activeBullets) b.Update(delta);

            // 4. Cập nhật kẻ địch
            foreach (var en in activeEnemies) en.Update(player, delta);

            // 5. Kiểm tra đạn va tường
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                var b = activeBullets[i];
                if (b.IsDead)
                {
                    DestroyBullet(b);
                    continue;
                }
                double bulletCenterX = b.X + b.BulletVisual.Width / 2;
                double bulletCenterY = b.Y + b.BulletVisual.Height / 2;
                int gridX = (int)(bulletCenterX / TILE_SIZE);
                int gridY = (int)(bulletCenterY / TILE_SIZE);

                if (gridX < 0 || gridX >= MAP_COLS || gridY < 0 || gridY >= MAP_ROWS)
                {
                    b.IsDead = true;
                    DestroyBullet(b);
                    continue;
                }
                int tileType = mapMatrix[gridY, gridX];
                if (tileType == 1 || tileType == 6)
                {
                    b.IsDead = true;
                    DestroyBullet(b);
                }
            }
            // 6. Kiểm tra va chạm: Đạn vs Kẻ địch
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                var b = activeBullets[i];
                if (b.IsDead) continue;

                Rect br = b.GetRect();
                foreach (var en in activeEnemies)
                {
                    if (en.IsDead) continue;
                    if (br.IntersectsWith(en.GetRect()))
                    {
                        b.IsDead = true;
                        en.IsDead = true;
                        DestroyBullet(b);
                        break;
                    }
                }
            }
            // 7. Kiểm tra va chạm: Kẻ địch vs Người chơi
            Rect playerRect = new Rect(player.X, player.Y, 48, 48);
            foreach (var en in activeEnemies)
            {
                if (en.IsDead) continue;
                if (en.GetRect().IntersectsWith(playerRect))
                {
                    EndGame();
                    return; // Thoát luôn, không cần xử lý tiếp
                }
            }

            // 8. Dọn dẹp kẻ địch đã chết (DUYỆT NGƯỢC)
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i].IsDead)
                {
                    DestroyEnemy(activeEnemies[i]);
                }
            }
        }
        void DestroyBullet(Bullet b)
        {
            var vis = b.BulletVisual;
            if (GameCanvas.Children.Contains(vis)) GameCanvas.Children.Remove(vis);
            activeBullets.Remove(b);
        }
        void DestroyEnemy(Enemy en)
        {
            if (GameCanvas.Children.Contains(en.EnemyVisual)) GameCanvas.Children.Remove(en.EnemyVisual);
            activeEnemies.Remove(en);

            OnEnemyKilled(en);
        }
        void CreatePlayer()
        {
            player = new Player(playerDefaultSpawnX, playerDefaultSpawnY);

            // Thêm CẢ HAI vào Canvas
            GameCanvas.Children.Add(player.LegsVisual);
            GameCanvas.Children.Add(player.BodyVisual);

            // Cập nhật vị trí ban đầu
            player.Move(0, 0);

            player.OnShoot += Player_OnShoot;
        }
        private void Player_OnShoot(Point target)
        {
            // Tâm của player
            double px = player.X + (player.BodyVisual.Width / 2);
            double py = player.Y + (player.BodyVisual.Height / 2);

            Vector dir = new Vector(target.X - px, target.Y - py);
            if (dir.LengthSquared == 0) return;
            dir.Normalize();

            SpawnBullet(px, py, dir);

        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W) { moveUp = true; player.Facing = Direction.Up; }
            if (e.Key == Key.Down || e.Key == Key.S) { moveDown = true; player.Facing = Direction.Down; }
            if (e.Key == Key.Left || e.Key == Key.A) { moveLeft = true; player.Facing = Direction.Left; }
            if (e.Key == Key.Right || e.Key == Key.D) { moveRight = true; player.Facing = Direction.Right; }
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
            if (!isGameRunning) return;
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

                    Canvas.SetLeft(tile, c * TILE_SIZE);
                    Canvas.SetTop(tile, r * TILE_SIZE);
                    Panel.SetZIndex(tile, -100);//sàn ở layer thấp nhất
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
            double spawnX = 0;
            double spawnY = 0;
            int side = random.Next(0, 4);
            double mapSize = 768;

            switch (side)
            {
                case 0: spawnX = random.Next(288, 480); spawnY = 0; break;// cạnh trên
                case 1: spawnX = random.Next(288, 480); spawnY = mapSize - 48; break;// cạnh dưới
                case 2: spawnX = 0; spawnY = random.Next(288, 480); break;//cạnh trái
                case 3: spawnX = mapSize - 48; spawnY = random.Next(288, 480); break;//cạnh phải
            }

            // SỬA LẠI: Tạo đối tượng Enemy mới (dùng ảnh Orc0 đã load)
            var enemyObj = new Enemy(spawnX, spawnY, Orc0, Orc1);

            // Thêm HÌNH ẢNH của nó vào Canvas
            GameCanvas.Children.Add(enemyObj.EnemyVisual);

            // Thêm ĐỐI TƯỢNG vào danh sách quản lý
            activeEnemies.Add(enemyObj);
        }
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

            double spawnX = centerX - bulletSprite.Width / 2;
            double spawnY = centerY - bulletSprite.Height / 2;

            Canvas.SetLeft(bulletSprite, spawnX);
            Canvas.SetTop(bulletSprite, spawnY);
            Panel.SetZIndex(bulletSprite, 15);
            GameCanvas.Children.Add(bulletSprite);

            // Tạo đối tượng Bullet quản lý logic
            var bullet = new Bullet(spawnX, spawnY, dir, bulletSprite);
            activeBullets.Add(bullet);

            PlayGunShot();
        }
        public async void OnEnemyKilled(Enemy enemyThatDied)
        {
            PlayDestroyedSound();
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
            foreach (var enemy in activeEnemies)
            {
                GameCanvas.Children.Remove(enemy.EnemyVisual);
            }
            activeEnemies.Clear();

            // Dọn dẹp đạn cũ
            foreach (var bullet in activeBullets)
            {
                GameCanvas.Children.Remove(bullet.BulletVisual);
            }
            activeBullets.Clear();

            // Dọn dẹp Player cũ
            GameCanvas.Children.Remove(player.BodyVisual);
            GameCanvas.Children.Remove(player.LegsVisual);

            // Dọn dẹp Map
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
        //Logic các button ở Menu
        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Retry_btn_Click(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }
        //Các hàm phát âm thanh
        void PlayBackgroundMusic()
        {
            bgMusic.Open(new Uri(musicPath, UriKind.Relative));
            bgMusic.Volume = 0.4;
            bgMusic.MediaEnded += (sender, e) =>
            {
                bgMusic.Position = TimeSpan.Zero;
                bgMusic.Play();
            };
            bgMusic.Play();
        }
        void PlayGunShot()
        {
            MediaPlayer sfx = new MediaPlayer(); // Tạo mới mỗi lần
            sfx.Open(new Uri(gunShotPath, UriKind.Relative));
            sfx.Volume = 0.8;
            sfx.MediaEnded += (s, e) => { sfx = null; }; // Tự hủy
            sfx.Play();
        }
        void PlayDestroyedSound()
        {
            MediaPlayer sfx = new MediaPlayer();
            sfx.Open(new Uri(destroyedPath, UriKind.Relative));
            sfx.Volume = 0.3;
            sfx.MediaEnded += (s, e) => { sfx = null; };
            sfx.Play();
        }
        void PlayGameOverMusic()
        {
            GameOver.Open(new Uri(gameOverPath, UriKind.Relative));
            GameOver.Volume = 0.7;
            GameOver.Play();
        }

    }
}