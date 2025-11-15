using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BTLT04_fromScratch
{
    // Lưu hướng đang nhìn
    public enum Direction { Up, Down, Left, Right }
    public class Player
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; } = 3;
        public Direction Facing { get; set; } = Direction.Down;
        public Image BodyVisual { get; set; }
        public Image LegsVisual { get; set; }//thêm hoạt ảnh chân của nhân vật
        public bool IsMoving { get; set; } = false; // Cờ kiểm tra di chuyển

        // Lắng nghe event này để tạo Bullet
        public event Action<Point>? OnShoot;

        BitmapImage PlayerDown, PlayerUp, PlayerLeft, PlayerRight;
        BitmapImage PlayerLeg0, PlayerLeg1, PlayerLeg2, PlayerLeg3;//thêm các sprite chân của nhân vật
        List<BitmapImage> legFrames = new List<BitmapImage>();

        private int currentLegFrame = 0;
        private double animationTimer = 0;
        private const double ANIMATION_SPEED = 0.1; // 0.1 giây đổi 1 frame chân
        public Player(double startX, double startY)
        {
            X = startX;
            Y = startY;

            // Load sprite 
            PlayerDown = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerDown.png", UriKind.Relative));
            PlayerUp = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerUp.png", UriKind.Relative));
            PlayerLeft = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeft.png", UriKind.Relative));
            PlayerRight = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerRight.png", UriKind.Relative));

            legFrames.Add(new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeg0.png", UriKind.Relative)));
            legFrames.Add(new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeg1.png", UriKind.Relative)));
            legFrames.Add(new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeg2.png", UriKind.Relative)));
            legFrames.Add(new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeg3.png", UriKind.Relative)));

            BodyVisual = new Image()
            {
                Width = 48,
                Height = 48,
                Source = PlayerDown
            };

            RenderOptions.SetBitmapScalingMode(BodyVisual, BitmapScalingMode.NearestNeighbor);
            Panel.SetZIndex(BodyVisual, 5);
            // Thân ở layer trên chân
            LegsVisual = new Image()
            {
                Width = 48,
                Height = 48,
                Source = legFrames[0], // Bắt đầu bằng frame chân 0
            };
            RenderOptions.SetBitmapScalingMode(LegsVisual, BitmapScalingMode.NearestNeighbor);
            Panel.SetZIndex(LegsVisual, 4); // Chân ở layer dưới thân
        }

        public void UpdateBodySprite()
        {
            switch (Facing)
            {
                case Direction.Up: BodyVisual.Source = PlayerUp; break;
                case Direction.Down: BodyVisual.Source = PlayerDown; break;
                case Direction.Left: BodyVisual.Source = PlayerLeft; break;
                case Direction.Right: BodyVisual.Source = PlayerRight; break;
            }
        }
        public void Update(double deltaTime)
        {
            // Chỉ chạy hoạt ảnh khi đang di chuyển
            if (IsMoving)
            {
                UpdateAnimation(deltaTime);
            }
            else
            {
                animationTimer = 0; // Reset timer
                currentLegFrame = 0; // Reset về frame 0
                LegsVisual.Source = legFrames[0];
            }
        }
        // (Hàm này được gọi bởi hàm Update ở trên)
        private void UpdateAnimation(double deltaTime)
        {
            // Nếu đang di chuyển, hiện chân lên
            LegsVisual.Visibility = Visibility.Visible;

            // Đếm ngược đồng hồ
            animationTimer -= deltaTime;

            // Nếu hết giờ, đổi frame
            if (animationTimer <= 0)
            {
                // Reset đồng hồ
                animationTimer = ANIMATION_SPEED;

                // Chuyển sang frame tiếp theo
                currentLegFrame = (currentLegFrame + 1) % legFrames.Count;

                // Cập nhật ảnh chân
                LegsVisual.Source = legFrames[currentLegFrame];
            }
        }
        // Di chuyển Player
        public void Move(double dx, double dy)
        {
            // Cập nhật vị trí
            X += dx;
            Y += dy;

            // Giới hạn trong Canvas 768x768 (trừ kích thước player)
            X = Math.Max(48, Math.Min(768 - BodyVisual.Width - 48, X));
            Y = Math.Max(48, Math.Min(768 - BodyVisual.Height - 48, Y));
            //thêm giới hạn của tường

            // Cập nhật hướng dựa trên dx dy
            if (dx > 0) Facing = Direction.Right;
            else if (dx < 0) Facing = Direction.Left;
            else if (dy > 0) Facing = Direction.Down;
            else if (dy < 0) Facing = Direction.Up;

            UpdateBodySprite();

            // Cập nhật vị trí hiển thị
            Canvas.SetLeft(BodyVisual, X);
            Canvas.SetTop(BodyVisual, Y);
            Canvas.SetLeft(LegsVisual, X);
            Canvas.SetTop(LegsVisual, Y + 19);
        }
        // Kích hoạt Bullet
        public void Shoot(System.Windows.Point mousePos)
        {
            // Nhiệm vụ kích hoạt Bullet
            OnShoot?.Invoke(mousePos);
            System.Diagnostics.Debug.WriteLine($"Shoot at {mousePos}");
        }
    }
}
