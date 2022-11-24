using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public static class GeneratePoints
    {

        public static double roundAvoid(double value, int places)
        {
            double scale = Math.Pow(10, places);
            return Math.Round(value * scale) / scale;
        }

        public static double gauss(int coordinate, int m, int sigma)
        {
            double gaussRez = Math.Exp((double)-(((m - coordinate) * (m - coordinate)) / (2 * sigma * sigma)));
            return roundAvoid(gaussRez, 20);
        }

        public static List<MyPoint> getPoints(List<MyZone> zoneList, int pointsToFind, bool withNoise)
        {

            List<MyPoint> output = new List<MyPoint>();

            Random rand = new Random();

            bool foundX = false;
            bool foundY = false;
            int randZoneIndex;
            int xCoord = 0;
            int yCoord = 0;

            while (output.Count() < pointsToFind)
            {

                randZoneIndex = rand.Next(zoneList.Count());

                while (!foundX)
                {
                    int x = rand.Next(-300, 300);

                    double gaussRez = gauss(x, zoneList[randZoneIndex].getmX(), zoneList[randZoneIndex].getSigmaX());
                    if (gaussRez > rand.NextDouble())
                    {
                        xCoord = x;
                        foundX = true;
                    }
                    else if (withNoise && gaussRez == 0 && rand.NextDouble() < 0.00001)
                    {
                        xCoord = x;
                        foundX = true;
                    }
                }

                while (!foundY)
                {
                    int y = rand.Next(-300, 300);

                    double gaussRez = gauss(y, zoneList[randZoneIndex].getmY(), zoneList[randZoneIndex].getSigmaY());
                    if (gaussRez > rand.NextDouble())
                    {
                        yCoord = y;
                        foundY = true;
                    }
                    else if (withNoise && gaussRez == 0 && rand.NextDouble() < 0.00001)
                    {
                        yCoord = y;
                        foundY = true;
                    }
                }

                foundX = false;
                foundY = false;
                output.Add(new MyPoint(xCoord, yCoord, randZoneIndex));
            }

            return output;
        }

    }
}
