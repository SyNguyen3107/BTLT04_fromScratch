using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BTLT04_fromScratch
{
    internal class Bullet
    {
        public double X;
        public double Y;
        public Image BulletVisual;
        public Vector Direction; // chuẩn hoá hướng vector
        public double Speed = 600.0; // số pixels / giây
        public bool IsDead = false;

        public Bullet(double x, double y, Vector direction, Image visual)
        {
            X = x;
            Y = y;
            Direction = direction;
            Direction.Normalize();
            BulletVisual = visual;

            // Các giá trị hiển thị mặc định, phù hợp với đồ họa pixel-art
            BulletVisual.Width = 16;
            BulletVisual.Height = 16;
            RenderOptions.SetBitmapScalingMode(BulletVisual, BitmapScalingMode.NearestNeighbor);

            UpdateVisual();
        }

        // deltaSeconds: thời gian đã trôi qua kể từ lần cập nhật trước (tính bằng giây)
        public void Update(double deltaSeconds)
        {
            if (IsDead) return;
            X += Direction.X * Speed * deltaSeconds;
            Y += Direction.Y * Speed * deltaSeconds;

            UpdateVisual();
        }

        void UpdateVisual()
        {
            Canvas.SetLeft(BulletVisual, X);
            Canvas.SetTop(BulletVisual, Y);
        }

        public Rect GetRect()
        {
            return new Rect(X, Y, BulletVisual.Width, BulletVisual.Height);
        }
    }
}
