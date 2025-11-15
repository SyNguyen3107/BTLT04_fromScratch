using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BTLT04_fromScratch
{
    internal class Enemy
    {
        public double X;
        public double Y;
        public Image EnemyVisual;
        public bool IsDead = false;

        public Enemy(Image visual, double x, double y)
        {
            EnemyVisual = visual;
            X = x;
            Y = y;

            EnemyVisual.Width = 48;
            EnemyVisual.Height = 48;
            RenderOptions.SetBitmapScalingMode(EnemyVisual, BitmapScalingMode.NearestNeighbor);

            UpdateVisual();
        }

        public void Update(double deltaSeconds)
        {

        }

        public void UpdateVisual()
        {
            Canvas.SetLeft(EnemyVisual, X);
            Canvas.SetTop(EnemyVisual, Y);
        }

        public Rect GetRect()
        {
            return new Rect(X, Y, EnemyVisual.Width, EnemyVisual.Height);
        }
    }
}
