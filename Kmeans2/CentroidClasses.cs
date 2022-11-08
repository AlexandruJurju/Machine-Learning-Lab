﻿using Kmeans2.Classes;

namespace Kmeans2
{
    public class CentroidFunctions
    {
        List<MyPoint> points = new List<MyPoint>();
        List<Centroid> centroids = new List<Centroid>();
        Random random = new Random();
        int step = 0;

        Graphics graphics;
        int graphWidth = 600;
        int graphHeight = 600;
        int yOffset = 50;
        int xOffset = 50;
        int overflow = 30;
        int radius = 5;
        int bigRadius = 8;
        List<Color> colorList = new List<Color>();
        Dictionary<Color, SolidBrush> brushDictionary = new Dictionary<Color, SolidBrush>();

        TextBox textBoxPrinting;

        public CentroidFunctions(Graphics graphics, TextBox textBoxPrinting)
        {
            this.graphics = graphics;
            this.textBoxPrinting = textBoxPrinting;
        }

        private bool areCentroidsTooClose(Centroid centroid1, Centroid centroid2)
        {
            if (centroid1 == centroid2)
            {
                return false;
            }

            foreach (var point1 in centroid1.getPointArrayList())
            {
                foreach (var point2 in centroid2.getPointArrayList())
                {
                    if (distance(point1, point2) <= 2.0)
                    {
                        //textBoxCost.AppendText(point1.getX() + " " + point1.getY() + " : " + point2.getX() + " " + point2.getY() + " ");
                        //textBoxCost.AppendText(Environment.NewLine);
                        //textBoxCost.AppendText("Distance " + distance(point1, point2).ToString());
                        //textBoxCost.AppendText(Environment.NewLine);
                        //textBoxCost.AppendText(Environment.NewLine);
                        //Thread.Sleep(1000);
                        return true;
                    }
                }
            }
            return false;
        }

        private void arrangeCentroids(List<Centroid> centroidList)
        {
            double previousCost = -1;
            double currentCost = 0;
            int epoch = 1;

            while (previousCost != currentCost)
            {
                groupPoints(points, centroidList);
                foreach (var centroid in centroidList)
                {
                    Dot centerOfGravity = calculateCenterOfGravity(centroid);
                    centroid.setX(centerOfGravity.getX());
                    centroid.setY(centerOfGravity.getY());
                }
                /*                graphics.Clear(Color.FromArgb(47, 47, 47));
                                drawAxis();
                                groupPoints(points, centroids);
                                drawPointsFromCentroids();
                                drawCentroids();
                                Thread.Sleep(500);*/

                previousCost = currentCost;
                currentCost = calculateCostForAllCentroids(centroidList);
                textBoxPrinting.AppendText("E : " + epoch.ToString() + "\t C : " + currentCost.ToString());
                textBoxPrinting.AppendText(Environment.NewLine);
                epoch++;
            }
            graphics.Clear(Color.FromArgb(47, 47, 47));
            drawAxis();
            groupPoints(points, centroidList);
            drawPointsFromCentroids(centroidList);
            drawCentroids(centroidList);
            Thread.Sleep(500);

            writeFinalCentroidDistances(centroidList);
        }

        private void buttonFullRun_MouseClick(object sender, MouseEventArgs e)
        {
            bool running = true;
            while (running)
            {
                arrangeCentroids(centroids);

                running = keepRunning();
                if (running)
                {
                    centroids = generateCentroids();
                }
            }


            List<Centroid> auxCentroids = new List<Centroid>();
            foreach (var centroid in centroids)
            {
                auxCentroids.Add(centroid);
            }

            List<Tuple<Centroid, Centroid>> tooCloseList = getCentroidsTooClose();

            if (tooCloseList.Count() > 0)
            {

                foreach (var centroidPair in tooCloseList)
                {
                    textBoxPrinting.AppendText(centroidPair.Item1.ToString() + " " + centroidPair.Item2.ToString());
                    textBoxPrinting.AppendText(Environment.NewLine);
                }

                foreach (var centroidPair in tooCloseList)
                {
                    auxCentroids.RemoveAll(centroid => centroid.getColor() == centroidPair.Item1.getColor());
                }

                arrangeCentroids(auxCentroids);
                foreach (var centroid in auxCentroids)
                {
                    textBoxPrinting.AppendText(centroid.ToString());
                    textBoxPrinting.AppendText(Environment.NewLine);
                }
                Thread.Sleep(1000);
            }


            auxCentroids.RemoveAll(c => c.getPointArrayList().Count < 10000 / 100);
            arrangeCentroids(auxCentroids);

            textBoxPrinting.AppendText("DONE");

        }

        private void buttonStep_MouseClick(object sender, MouseEventArgs e)
        {
            switch (step)
            {
                case 0:
                    graphics.Clear(Color.FromArgb(47, 47, 47));
                    drawAxis();
                    groupPoints(points, centroids);
                    drawPointsFromCentroids(centroids);
                    drawCentroids(centroids);
                    step = 1;
                    break;
                case 1:
                    graphics.Clear(Color.FromArgb(47, 47, 47));
                    drawAxis();
                    foreach (var centroid in centroids)
                    {
                        Dot centerOfGravity = calculateCenterOfGravity(centroid);
                        centroid.setX(centerOfGravity.getX());
                        centroid.setY(centerOfGravity.getY());
                    }
                    groupPoints(points, centroids);
                    drawPointsFromCentroids(centroids);
                    drawCentroids(centroids);
                    step = 2;
                    break;
                case 2:
                    graphics.Clear(Color.FromArgb(47, 47, 47));
                    drawOutline(centroids);
                    drawAxis();
                    step = 0;
                    break;
            }
        }

        private Dot calculateCenterOfGravity(Centroid centroid)
        {
            int xSum = 0;
            int ySum = 0;
            foreach (MyPoint point in centroid.getPointArrayList())
            {
                xSum += point.getX();
                ySum += point.getY();
            }

            if (xSum != 0 && ySum != 0)
            {
                int xMean = xSum / centroid.getPointArrayList().Count();
                int yMean = ySum / centroid.getPointArrayList().Count();
                return new Dot(xMean, yMean);
            }
            else
            {
                return new Dot(centroid.getX(), centroid.getY());
            }
        }

        private double calculateCostForAllCentroids(List<Centroid> centroidArrayList)
        {
            double Ec = 0;

            foreach (Centroid centroid in centroidArrayList)
            {
                foreach (MyPoint point in centroid.getPointArrayList())
                {
                    Ec += distance(point, centroid);
                }
            }
            return Ec;
        }

        private double distance(Dot point1, Dot point2)
        {
            int dX = point2.getX() - point1.getX();
            int dY = point2.getY() - point1.getY();
            return Math.Sqrt(dX * dX + dY * dY);
        }

        private void drawAxis()
        {
            Pen whitePen = new Pen(Color.White, 1);

            int x1 = xOffset - overflow;
            int y1 = graphHeight / 2 + yOffset;
            int x2 = graphWidth + xOffset + overflow;
            int y2 = graphHeight / 2 + yOffset;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);


            x1 = (int)(graphWidth / 2.0 + xOffset);
            y1 = yOffset - overflow;
            x2 = (int)(graphWidth / 2.0 + xOffset);
            y2 = graphHeight + yOffset + overflow;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);


            int xLimit = 300 + graphWidth / 2;
            int yLimit = graphHeight / 2 - 300;

            //top
            x1 = (xLimit - 600 + xOffset);
            y1 = yLimit + yOffset;
            x2 = (xLimit + xOffset);
            y2 = yLimit + yOffset;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);

            // bottom
            x1 = (xLimit - 600 + xOffset);
            y1 = yLimit + 600 + yOffset;
            x2 = xLimit + xOffset;
            y2 = yLimit + yOffset + 600;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);

            // left
            x1 = xLimit - 600 + xOffset;
            y1 = yLimit + yOffset;
            x2 = xLimit - 600 + xOffset;
            y2 = yLimit + yOffset + 600;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);

            // right
            x1 = xLimit + xOffset;
            y1 = yLimit + yOffset;
            x2 = xLimit + xOffset;
            y2 = yLimit + yOffset + 600;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);
        }

        private void drawCentroids(List<Centroid> centroidList)
        {
            SolidBrush blackBrush = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.White);

            foreach (var centroid in centroidList)
            {
                int screenX = centroid.getX() + graphWidth / 2 + xOffset;
                int screenY = graphHeight / 2 - centroid.getY() + yOffset;

                graphics.FillEllipse(blackBrush, new Rectangle(screenX - bigRadius, screenY - bigRadius, 2 * bigRadius, 2 * bigRadius));
                graphics.FillEllipse(brushDictionary[centroid.getColor()], new Rectangle(screenX - radius, screenY - radius, 2 * radius, 2 * radius));
            }
        }


        // SOM

        private void drawOutline(List<Centroid> centroidList)
        {
            for (int x = -300; x < 300; x += 7)
            {
                for (int y = -300; y < 300; y += 7)
                {
                    double minDist = 1000000;
                    Centroid bestCentroid = null;

                    foreach (Centroid centroid in centroidList)
                    {
                        double dist = distance(new Dot(x, y), centroid);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            bestCentroid = centroid;
                        }
                    }

                    int screenX = x + graphWidth / 2 + xOffset;
                    int screenY = graphHeight / 2 - y + yOffset;

                    graphics.FillEllipse(brushDictionary[bestCentroid.getColor()], new Rectangle(screenX - radius, screenY - radius, 2 * radius, 2 * radius));

                }
            }
        }

        private void drawPoints(List<MyPoint> points)
        {
            Brush whiteBrush = new SolidBrush(Color.White);
            foreach (var point in points)
            {
                int screenX = point.getX() + graphWidth / 2 + xOffset;
                int screenY = graphHeight / 2 - point.getY() + yOffset;

                graphics.FillEllipse(whiteBrush, new Rectangle(screenX, screenY, 2, 2));
            }
        }

        private void drawPointsFromCentroids(List<Centroid> centroidList)
        {
            foreach (var centroid in centroidList)
            {
                foreach (var point in centroid.getPointArrayList())
                {
                    int screenX = point.getX() + graphWidth / 2 + xOffset;
                    int screenY = graphHeight / 2 - point.getY() + yOffset;

                    graphics.FillEllipse(brushDictionary[centroid.getColor()], new Rectangle(screenX, screenY, 4, 4));
                }
            }
        }

        private Centroid findCentroidWithMinimialDistance(List<Centroid> centroidList, MyPoint point)
        {
            double minDistance = 1000000;
            Centroid bestCentroid = null;

            foreach (Centroid centroid in centroidList)
            {
                double dist = distance(point, centroid);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestCentroid = centroid;
                }
            }
            return bestCentroid;
        }

        private double findMinimalDistanceOfCentroid(Centroid centroid)
        {
            double minimDistance = 1000000000;
            foreach (var point in centroid.getPointArrayList())
            {
                double currentDistance = distance(point, centroid);
                if (currentDistance < minimDistance)
                {
                    minimDistance = currentDistance;
                }
            }
            return minimDistance;
        }

        private List<Centroid> generateCentroids()
        {
            List<Centroid> centroidList = new List<Centroid>();
            int centroidCount = random.Next(2, 10);
            for (int i = 0; i < centroidCount; i++)
            {
                int x = random.Next(-300, 300);
                int y = random.Next(-300, 300);
                centroidList.Add(new Centroid(x, y, colorList[i]));
            }
            return centroidList;
        }

        private List<Tuple<Centroid, Centroid>> getCentroidsTooClose()
        {
            List<Tuple<Centroid, Centroid>> tooClose = new List<Tuple<Centroid, Centroid>>();

            foreach (var centroid1 in centroids)
            {
                foreach (var centroid2 in centroids)
                {
                    if (centroid1 != centroid2)
                    {
                        if (areCentroidsTooClose(centroid1, centroid2))
                        {
                            //textBoxCost.AppendText(centroid1.getColor() + " " + centroid2.getColor());
                            //textBoxCost.AppendText(Environment.NewLine);

                            Tuple<Centroid, Centroid> tooClosePair = new Tuple<Centroid, Centroid>(centroid1, centroid2);
                            Tuple<Centroid, Centroid> tooClosePairReversed = new Tuple<Centroid, Centroid>(centroid2, centroid1);
                            if ((!tooClose.Contains(tooClosePair)) && (!tooClose.Contains(tooClosePairReversed)))
                            {
                                tooClose.Add(tooClosePair);
                            }
                        }
                    }
                }
            }

            return tooClose;
        }

        private void groupPoints(List<MyPoint> pointArrayList, List<Centroid> centroidArrayList)
        {
            foreach (Centroid centroid in centroidArrayList)
            {
                centroid.getPointArrayList().Clear();
            }

            foreach (MyPoint point in pointArrayList)
            {
                Centroid bestCentroid = findCentroidWithMinimialDistance(centroidArrayList, point);
                bestCentroid.getPointArrayList().Add(point);
            }
        }

        private void initColors()
        {
            colorList.Add(Color.Blue);
            colorList.Add(Color.SteelBlue);
            colorList.Add(Color.Cyan);
            colorList.Add(Color.MediumSpringGreen);
            colorList.Add(Color.Sienna);
            colorList.Add(Color.Red);
            colorList.Add(Color.HotPink);
            colorList.Add(Color.Lime);
            colorList.Add(Color.Purple);

            foreach (var color in colorList)
            {
                SolidBrush brush = new SolidBrush(color);
                brushDictionary.Add(color, brush);
            }
        }


        private bool keepRunning()
        {
            foreach (var centroid in centroids)
            {
                if (centroid.getPointArrayList().Count >= points.Count / 100)
                {
                    if (findMinimalDistanceOfCentroid(centroid) > 2.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<MyPoint> readPointsFromFile()
        {
            List<MyPoint> readPoints = new List<MyPoint>();

            var lines = File.ReadLines(@"X:\Projects\Visual Studio\Kmeans2\points.txt");

            foreach (var line in lines)
            {
                string[] split = line.Split(" ");
                readPoints.Add(new MyPoint(Int16.Parse(split[0]), Int16.Parse(split[1]), Int16.Parse(split[2])));
            }

            return readPoints;
        }

        private void writeFinalCentroidDistances(List<Centroid> centroidList)
        {
            textBoxPrinting.AppendText(Environment.NewLine);
            foreach (var centroid in centroidList)
            {
                if (centroid.getPointArrayList().Count >= points.Count / 100)
                {
                    double minDist = findMinimalDistanceOfCentroid(centroid);
                    textBoxPrinting.AppendText("Centroid : " + centroid.getColor() + " Min distance : " + minDist);
                    textBoxPrinting.AppendText(Environment.NewLine);
                }
            }
            textBoxPrinting.AppendText("====================================");
            textBoxPrinting.AppendText(Environment.NewLine);
            textBoxPrinting.AppendText(Environment.NewLine);
        }
    }
}