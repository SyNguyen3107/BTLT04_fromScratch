using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BTLT04_fromScratch
{
    internal class Enemy
    {
        public double x;
        public double y;
        public double Speed = 1;
       public bool IsDead = false;
        public Image EnemyVisual { get; set; }
        private BitmapImage[] frames = new BitmapImage[2];
        private int currentFrame = 0;
        private double frameCounter = 0;
        private double frameDistance = 1;
        public Enemy(double x, double y)
        {
            this.x = x;
            this.y = y;

            frames[0] = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Enemy/Orc0.png", UriKind.Relative));
            frames[1] = new BitmapImage(new Uri("Assets/Sprites/Sprite16x16/Enemy/Orc1.png", UriKind.Relative));
            EnemyVisual = new Image
            {
                Width = 48,
                Height = 48,
                Source = frames[0]
            };
            Canvas.SetLeft(EnemyVisual, x);
            Canvas.SetTop(EnemyVisual, y);
        }
        public void Update(Player player)
        {
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
            Canvas.SetLeft(EnemyVisual, x);
            Canvas.SetTop(EnemyVisual, y);
            // Cập nhật hoạt ảnh
            frameCounter += Math.Sqrt(x * x + y * y) / frameDistance;
            if (frameCounter >= frameDistance)
            {
                frameCounter = 0;
                currentFrame = (currentFrame + 1) % frames.Length;
                EnemyVisual.Source = frames[currentFrame];
            }
        }
    

    public Rect GetRect()
        {
            return new Rect(x, y, EnemyVisual.Width, EnemyVisual.Height);
        }
    }
}
