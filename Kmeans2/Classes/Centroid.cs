using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public class Centroid : Dot
    {

        private Color color;
        private List<MyPoint> pointArrayList;

        public Centroid(int x, int y, Color color) : base(x, y)
        {
            this.color = color;
            pointArrayList = new List<MyPoint>();
        }

        public Color getColor()
        {
            return color;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        public List<MyPoint> getPointArrayList()
        {
            return pointArrayList;
        }

        public void setPointArrayList(List<MyPoint> pointArrayList)
        {
            this.pointArrayList = pointArrayList;
        }
    }
}
