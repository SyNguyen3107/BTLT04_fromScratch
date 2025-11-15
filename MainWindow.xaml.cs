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
<<<<<<< HEAD
using System.Collections.Generic;
using System.Diagnostics;
=======
using System.Windows.Threading;
>>>>>>> 73f80ddf1773fd73735da781ed7bf75173fc3042

namespace BTLT04_fromScratch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Player
        Player player;

        bool moveUp = false, moveDown = false, moveLeft = false, moveRight = false;

        const int TILE_SIZE = 48;
        const int MAP_ROWS = 16;
        const int MAP_COLS = 16;
        // số trong ma trận tương ứng với STT của ảnh trong thư mực Assets/Sprites/Sprite16x16/Environment/

        bool isGameRunning = true; // Biến trạng thái game
        System.Windows.Threading.DispatcherTimer spawnTimer; // Timer dùng để spawn kẻ địch định kỳ
        System.Windows.Threading.DispatcherTimer gameLoopTimer; // Timer điều khiển vòng lặp game chính
        int currentWave = 0;
        int enemyAlive = 0, enemyToSpawn = 0; // Biến đếm kẻ địch còn lại/tổng cần spawn
        Random random = new Random();

        // Thay danh sách Image thô bằng các đối tượng Enemy
        List<Enemy> activeEnemies = new List<Enemy>();
        List<Bullet> activeBullets = new List<Bullet>();

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

        //Sprite đạn
        BitmapImage Bullet;

        //Các sprite ảnh kẻ địch cần dùng
        BitmapImage Orc0, Orc1;

        // Biểu diễn người chơi
        Image PlayerSprite;
        double playerX = 384 - 24; // lấy vị trí xấp xỉ ở giữa
        double playerY = 384 - 24;
        Vector playerFacing = new Vector(0, -1); // hướng mặc định là lên trên

        // Mỗi âm thanh cần 1 MediaPlayer riêng
        MediaPlayer bgMusic = new MediaPlayer(); string musicPath;
        MediaPlayer GunShot = new MediaPlayer(); string gunShotPath;
        MediaPlayer Destroyed = new MediaPlayer(); string destroyedPath;
        MediaPlayer GameOver = new MediaPlayer(); string gameOverPath;

<<<<<<< HEAD
        // Bộ hỗ trợ đo thời gian cho vòng lặp game
        Stopwatch loopStopwatch = new Stopwatch();
        long lastTicks = 0;

=======
>>>>>>> 73f80ddf1773fd73735da781ed7bf75173fc3042
        public MainWindow()
        {
            InitializeComponent();

            LoadAssets();
            DrawMap();
            AddPlayerToCanvas();
            StartGameLoop();
            PlayBackgroundMusic();
            StartNextWave();

<<<<<<< HEAD
            // Sử dụng chuột để bắn theo vị trí con trỏ
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;

            // KeyDown để bắn bằng phím mũi tên (Up/Down/Left/Right)
            this.KeyDown += MainWindow_KeyDown;
=======
            // Player
            player = new Player(300, 300);
            GameCanvas.Children.Add(player.PlayerVisual);
            // Đăng ký GameLoop 
            CompositionTarget.Rendering += GameLoop; // Hàm GameLoop sẽ được gọi sau mỗi frame render

            // Bắt input
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;
        }
        private void GameLoop(object sender, EventArgs e)
        {
            if (!isGameRunning) return;
            // Player Movement
            double dx = 0, dy = 0;

            if (moveUp) dy -= player.Speed;
            if (moveDown) dy += player.Speed;
            if (moveLeft) dx -= player.Speed;
            if (moveRight) dx += player.Speed;

            if (dx != 0 || dy != 0)
                player.Move(dx, dy);
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.W) { moveUp = true; player.Facing = Direction.Up; }
            if (e.Key == Key.Down || e.Key == Key.S) { moveDown = true; player.Facing = Direction.Down; }
            if (e.Key == Key.Left || e.Key == Key.A) { moveLeft = true; player.Facing = Direction.Left; }
            if (e.Key == Key.Right || e.Key == Key.D) { moveRight = true; player.Facing = Direction.Right; }

            player.UpdateSprite();
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
            Point target = e.GetPosition(GameCanvas);
            player.Shoot(target);
>>>>>>> 73f80ddf1773fd73735da781ed7bf75173fc3042
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

            PlayerDown = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Player/PlayerDown.png", UriKind.Relative));
            PlayerUp = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Player/PlayerUp.png", UriKind.Relative));
            PlayerLeft = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Player/PlayerLeft.png", UriKind.Relative));
            PlayerRight = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Player/PlayerRight.png", UriKind.Relative));

            musicPath = "Assets/Audio/Overworld.wav";
            gunShotPath = "Assets/Audio/GunShot.wav";
            destroyedPath = "Assets/Audio/Destroyed0.wav";
            gameOverPath = "Assets/Audio/GameOver.wav";
        }

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

            // 2. Cập nhật kẻ địch
            foreach (var en in activeEnemies)
            {
                en.Update(delta);
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

            // 4. Kiểm tra va chạm: Kẻ địch vs Người chơi → Game Over
            Rect playerRect = new Rect(playerX, playerY, PlayerSprite.Width, PlayerSprite.Height);
            foreach (var en in activeEnemies)
            {
                if (en.IsDead) continue;
                if (en.GetRect().IntersectsWith(playerRect))
                {
                    EndGame();
                    break;
                }
            }

            // 5. Dọn dẹp các viên đạn
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                if (activeBullets[i].IsDead)
                {
                    var vis = activeBullets[i].BulletVisual;
                    if (GameCanvas.Children.Contains(vis)) GameCanvas.Children.Remove(vis);
                    activeBullets.RemoveAt(i);
                }
            }

            // 6. Dọn dẹp kẻ địch đã chết
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i].IsDead)
                {
                    var vis = activeEnemies[i].EnemyVisual;
                    if (GameCanvas.Children.Contains(vis)) GameCanvas.Children.Remove(vis);
                    activeEnemies.RemoveAt(i);
                    Destroyed.Play();
                    enemyAlive--;
                }
            }

            // 7. Nếu wave đã kết thúc và không còn kẻ địch sống → wave tiếp theo được xử lý ở OnEnemyKilled trước đó.
            // Logic StartNextWave hiện có vẫn hoạt động thông qua spawnTimer và bộ đếm kẻ địch.
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
            // Tạo image dùng để hiển thị kẻ địch
            Image enemySprite = new Image
            {
                Source = Orc0,
                Width = 48,
                Height = 48
            };
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
                    spawnY = 0; // Sát mép trên
                    break;

                case 1: // Cạnh DƯỚI
                    spawnX = random.Next(288, 480);
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

            // Thêm quái này vào danh sách quản lý (Enemy object)
            var enemyObj = new Enemy(enemySprite, spawnX, spawnY);
            activeEnemies.Add(enemyObj);
        }

        public async void OnEnemyKilled(Image enemyThatDied)
        {
            // Phương thức này được giữ lại để đảm bảo tương thích nhưng không được sử dụng trong luồng mới.
            // Logic xóa mới được thực hiện trong giai đoạn dọn dẹp của GameLoop.
            await Task.CompletedTask;
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

            // Dọn sạch các danh sách đang được quản lý
            activeBullets.Clear();
            activeEnemies.Clear();

            // 4. Vẽ lại Map và các đối tượng ban đầu
            DrawMap();
            AddPlayerToCanvas();

            // Reset các bộ đếm và bộ hẹn giờ
            currentWave = 0;
            StartNextWave();

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

        // Mouse shooting: tạo đạn từ vị trí người chơi bay về vị trí con trỏ
        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Lấy toạ độ con trỏ so với GameCanvas
            Point pos = e.GetPosition(GameCanvas);
            ShootAt(pos);
        }

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