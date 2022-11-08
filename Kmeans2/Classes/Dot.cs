using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public class Dot
    {
        private int x;
        private int y;

        public int Y { get => y; set => y = value; }
        public int X { get => x; set => x = value; }

        public Dot(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int getX()
        {
            return X;
        }

        public void setX(int x)
        {
            this.X = x;
        }

        public int getY()
        {
            return Y;
        }

        public void setY(int y)
        {
            this.Y = y;
        }

        public override string? ToString()
        {
            return X.ToString() + " " + Y.ToString() + " ";
        }
    }
}
