using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BTLT04_fromScratch
{
    public class Enemy
    {
        public double x;
        public double y;
        public double Speed = 1;
        public bool IsDead = false;
        public Image EnemyVisual { get; set; }
        private BitmapImage[] frames = new BitmapImage[2];

        private int currentFrame = 0;
        private double animationTimer = 0;
        // 0.3 giây đổi 1 frame
        private const double TIME_PER_FRAME = 0.3;

        // Hàm này giờ đây "nhận" 2 frame ảnh từ MainWindow
        // thay vì tự load ảnh, giúp tối ưu game
        public Enemy(double x, double y, BitmapImage frame1, BitmapImage frame2)
        {
            this.x = x;
            this.y = y;
            this.frames[0] = frame1;
            this.frames[1] = frame2;

            // --- SỬA LỖI ---

            // BƯỚC 1: Tạo đối tượng Image TRƯỚC
            EnemyVisual = new Image
            {
                Width = 48,
                Height = 48,
                Source = frames[0]
            };

            // BƯỚC 2: SAU KHI đã có Image, mới set chống mờ
            RenderOptions.SetBitmapScalingMode(EnemyVisual, BitmapScalingMode.NearestNeighbor);

            // BƯỚC 3: Cập nhật vị trí ban đầu
            UpdateVisual();
        }
        public void Update(Player player, double deltaTime)
        {
            if (IsDead) return;
            if (currentFrame == 2) currentFrame = 0;
            //Di chuyển
            double dx = player.X - x;
            double dy = player.Y - y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // Chuẩn hóa vector và di chuyển về phía player
            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;
            }
            // Cập nhật vị trí
            x += dx * Speed;
            y += dy * Speed;
            UpdateVisual();

            animationTimer -= deltaTime; // Đếm ngược thời gian
            if (animationTimer <= 0)
            {
                animationTimer = TIME_PER_FRAME; // Reset đồng hồ
                currentFrame = (currentFrame + 1) % frames.Length; // Đổi frame (0 -> 1 -> 0)
                EnemyVisual.Source = frames[currentFrame];
            }
        }
        void UpdateVisual()
        {
            Canvas.SetLeft(EnemyVisual, x);
            Canvas.SetTop(EnemyVisual, y);
        }
        public Rect GetRect()
        {
            return new Rect(x, y, EnemyVisual.Width, EnemyVisual.Height);
        }
    }
}
