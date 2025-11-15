using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace BTLT04_fromScratch
{
    // Lưu hướng đang nhìn
    public enum Direction { Up, Down, Left, Right }
    public class Player
    {
        public double X {  get; set; }
        public double Y { get; set; }
        public double Speed { get; set; } = 3;
        public Direction Facing { get; set; } = Direction.Up;
        public Image PlayerVisual { get; set; }
        // Lắng nghe event này để tạo Bullet
        public event Action<Point>? OnShoot;

        BitmapImage PlayerDown, PlayerUp, PlayerLeft, PlayerRight;
        public Player(double startX, double startY)
        {
            X = startX;
            Y = startY;

            // Load sprite 
            PlayerDown = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerDown.png", UriKind.Relative));
            PlayerUp = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerUp.png", UriKind.Relative));
            PlayerLeft = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerLeft.png", UriKind.Relative));
            PlayerRight = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Character/PlayerRight.png", UriKind.Relative));

            PlayerVisual = new Image()
            {
                Width = 48,
                Height = 48,
                Source = PlayerDown
            };
            // Giữ ảnh sắc nét
            RenderOptions.SetBitmapScalingMode(PlayerVisual, BitmapScalingMode.NearestNeighbor);
        }

        public void UpdateSprite()
        {
            switch(Facing)
            {
                case Direction.Up: PlayerVisual.Source = PlayerUp; break;
                case Direction.Down: PlayerVisual.Source = PlayerDown; break;
                case Direction.Left: PlayerVisual.Source = PlayerLeft; break;
                case Direction.Right: PlayerVisual.Source = PlayerRight; break;
            }
        }
        // Di chuyển Player
        public void Move(double dx, double dy)
        {
            // Cập nhật vị trí
            X += dx;
            Y += dy;

            // Giới hạn trong Canvas 768x768 (trừ kích thước player)
            X = Math.Max(0, Math.Min(768 - PlayerVisual.Width, X));
            Y = Math.Max(0, Math.Min(768 - PlayerVisual.Height, Y));

            // Cập nhật hướng dựa trên dx dy
            if (dx > 0) Facing = Direction.Right;
            else if (dx < 0) Facing = Direction.Left;
            else if (dy > 0) Facing = Direction.Down;
            else if (dy < 0) Facing = Direction.Up;

            UpdateSprite();

            // Cập nhật vị trí hiển thị
            Canvas.SetLeft(PlayerVisual, X);
            Canvas.SetTop(PlayerVisual, Y);
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
