using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public class Zone
    {

        private int mX;
        private int mY;
        private int sigmaX;
        private int sigmaY;

        public Zone(int mX, int mY, int sigmaX, int sigmaY)
        {
            this.mX = mX;
            this.mY = mY;
            this.sigmaX = sigmaX;
            this.sigmaY = sigmaY;
        }

        public int getmX()
        {
            return mX;
        }

        public void setmX(int mX)
        {
            this.mX = mX;
        }

        public int getmY()
        {
            return mY;
        }

        public void setmY(int mY)
        {
            this.mY = mY;
        }

        public int getSigmaX()
        {
            return sigmaX;
        }

        public void setSigmaX(int sigmaX)
        {
            this.sigmaX = sigmaX;
        }

        public int getSigmaY()
        {
            return sigmaY;
        }

        public void setSigmaY(int sigmaY)
        {
            this.sigmaY = sigmaY;
        }
    }
}
