using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public class MyPoint : Dot
    {
        private int zone;

        public MyPoint(int x, int y, int zone) : base(x, y)
        {
            this.zone = zone;
        }

        public int getZone()
        {
            return zone;
        }

        public void setZone(int zone)
        {
            this.zone = zone;
        }
    }
}
