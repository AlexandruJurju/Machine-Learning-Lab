using Kmeans2.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Resources.ResXFileRef;

namespace Kmeans2
{
    public partial class MainForm : Form
    {
        int screenGraphWidth = 600;
        int screenGraphHeight = 600;
        int realGraphWidth = 600;
        int realGraphHeight = 600;
        int yOffset = 50;
        int xOffset = 50;
        int overflow = 30;
        List<MyPoint> points = new List<MyPoint>();
        Random random = new Random();
        Graphics graphics;

        List<Centroid> centroids = new List<Centroid>();
        int step = 0;
        int radius = 5;
        int bigRadius = 8;
        List<Color> colorList = new List<Color>();
        Dictionary<Color, SolidBrush> brushDictionary = new Dictionary<Color, SolidBrush>();

        int neuronMatrixSize = 10;
        Dot[,] neuronMatrix;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.graphics = this.CreateGraphics();
            this.BackColor = Color.FromArgb(47, 47, 47);
            initColors();

            points = readPointsFromFile();
            centroids = generateCentroids();

            neuronMatrix = initNeuronPositions(neuronMatrixSize);
        }


        // SOM
        private void buttonDrawNeurons_Click(object sender, EventArgs e)
        {
            //drawPoints(points);
            //drawNeightbourLines(neuronMatrix);
            //drawAxis();

            neuronMatrix[3, 2].setX(100);
            drawNeightbourLines(neuronMatrix);
            drawAxis();

            List<Dot> neightbours = findNeighbours(neuronMatrix, neuronMatrix[3, 3], 1);
            foreach (var neightbour in neightbours)
            {
                drawPoint(neightbour, 4);
            }

            textBoxPrinting.AppendText(Math.Exp(-10 / 10.0).ToString());
        }

        private void buttonSOMFullRun_Click(object sender, EventArgs e)
        {
            int N = 50;
            double learningRate = 0.7;
            double neighborhoodDistance = 6.1;
            int epoch = 0;

            while (learningRate > 0.01)
            {
                learningRate = 0.7 * Math.Exp((double)-epoch / N);
                neighborhoodDistance = 6.1 * Math.Exp((double)-epoch / N);

                textBoxPrinting.Text = ("EPOCH : " + epoch + Environment.NewLine);
                textBoxPrinting.AppendText("LEARNING RATE : " + learningRate + Environment.NewLine);
                textBoxPrinting.AppendText("NEIGHBOURHOOD : " + neighborhoodDistance + Environment.NewLine);
                textBoxPrinting.AppendText("====================" + Environment.NewLine);
                textBoxPrinting.AppendText(Environment.NewLine);

                foreach (var point in points)
                {
                    Dot nearestNeuron = findNearestNeuron(neuronMatrix, point);
                    int newX = (int)(nearestNeuron.getX() + learningRate * (point.getX() - nearestNeuron.getX()));
                    int newY = (int)(nearestNeuron.getY() + learningRate * (point.getY() - nearestNeuron.getY()));
                    nearestNeuron.setX(newX);
                    nearestNeuron.setY(newY);
                    graphics.Clear(Color.FromArgb(47, 47, 47));
                    drawAxis();
                    drawNeightbourLines(neuronMatrix);
                    Thread.Sleep(1000);

                    List<Dot> neightbours = findNeighbours(neuronMatrix, nearestNeuron, (int)neighborhoodDistance);
                    foreach (var neighbour in neightbours)
                    {
                        neighbour.X = newX;
                        neighbour.Y = newY;
                        graphics.Clear(Color.FromArgb(47, 47, 47));
                        drawAxis();
                        drawNeightbourLines(neuronMatrix);
                        Thread.Sleep(1000);
                    }

                }

                epoch++;
            }

            graphics.Clear(Color.FromArgb(47, 47, 47));
            drawAxis();
            drawNeightbourLines(neuronMatrix);
            drawPoints(points);
        }

        private Dot findNearestNeuron(Dot[,] neuronMatrix, Dot point)
        {
            double minDistance = 100000000;
            Dot nearest = null;
            foreach (var neuron in neuronMatrix)
            {
                double currentDistance = distance(point, neuron);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    nearest = neuron;
                }
            }

            return nearest;
        }

        private Dot[,] initNeuronPositions(int neuronMatrixSize)
        {
            Dot[,] output = new Dot[neuronMatrixSize, neuronMatrixSize];

            int xStep = realGraphWidth / neuronMatrixSize;
            int yStep = realGraphHeight / neuronMatrixSize;

            //for (int i = -realGraphWidth / 2 + xStep / 2, lineCount = 0; i < realGraphWidth / 2; i += xStep, lineCount++)
            //{
            //    for (int j = -realGraphHeight / 2 + yStep / 2, columnCount = 0; j < realGraphHeight / 2; j += yStep, columnCount++)
            //    {
            //        output[lineCount, 9 - columnCount] = new Dot(i, j);
            //    }
            //}

            for (int i = -realGraphWidth / 2 + xStep / 2, columnCount = 0; i < realGraphWidth / 2; i += xStep, columnCount++)
            {
                for (int j = realGraphHeight / 2 - yStep / 2, lineCount = 0; j > -realGraphHeight / 2; j -= yStep, lineCount++)
                {
                    output[lineCount, columnCount] = new Dot(i, j);
                }
            }

            return output;
        }

        public Tuple<int, int> CoordinatesOf(Dot[,] neuronMatrix, Dot point)
        {
            int w = neuronMatrix.GetLength(0); // width
            int h = neuronMatrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (neuronMatrix[x, y].Equals(point))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }

        private List<Dot> findNeighbours(Dot[,] neuronMatrix, Dot point, int neighbourhoodDistance)
        {
            List<Dot> neightbours = new List<Dot>();
            Tuple<int, int> origin = CoordinatesOf(neuronMatrix, point);

            int originX = origin.Item1;
            int originY = origin.Item2;

            if (neighbourhoodDistance == 0)
            {
                neightbours.Add(neuronMatrix[originX, originY]);
            }
            else
            {
                for (int i = originX - neighbourhoodDistance; i <= originX + neighbourhoodDistance; i++)
                {
                    for (int j = originY - neighbourhoodDistance; j <= originY + neighbourhoodDistance; j++)
                    {
                        if (i >= 0 && i < neuronMatrixSize)
                        {
                            if (j >= 0 && j < neuronMatrixSize)
                            {
                                if (neuronMatrix[i, j] != point)
                                {
                                    neightbours.Add(neuronMatrix[i, j]);
                                }
                            }
                        }

                    }
                }
            }

            return neightbours;
        }

        private void drawNeurons(Dot[,] neuroMatrix)
        {
            SolidBrush blackBrush = new SolidBrush(Color.White);

            for (int i = 0; i < neuronMatrixSize; i++)
            {
                for (int j = 0; j < neuronMatrixSize; j++)
                {
                    Dot converted = convertToScreen(neuronMatrix[i, j]);

                    graphics.FillEllipse(blackBrush, new Rectangle(converted.getX() - radius, converted.getY() - radius, 2 * radius, 2 * radius));
                }
            }
        }

        private void drawNeightbourLines(Dot[,] neuronMatrix)
        {
            Pen whitePen = new Pen(Color.Black, 2);

            for (int i = 0; i < neuronMatrixSize; i++)
            {
                for (int j = 0; j < neuronMatrixSize; j++)
                {

                    Dot convertedNeuron = convertToScreen(neuronMatrix[i, j]);

                    int screenXNeuron1 = neuronMatrix[i, j].getX() + screenGraphWidth / 2 + xOffset;
                    int screenYNeuron1 = screenGraphHeight / 2 - neuronMatrix[i, j].getY() + yOffset;

                    List<Dot> neightbours = new List<Dot>();

                    if (i - 1 >= 0)
                    {
                        Dot upNeuron = neuronMatrix[i - 1, j];
                        neightbours.Add(upNeuron);
                    }

                    if (i + 1 < neuronMatrixSize)
                    {
                        Dot downNeuron = neuronMatrix[i + 1, j];
                        neightbours.Add(downNeuron);
                    }

                    if (j - 1 >= 0)
                    {
                        Dot leftNeuron = neuronMatrix[i, j - 1];
                        neightbours.Add(leftNeuron);
                    }

                    if (j + 1 < neuronMatrixSize)
                    {
                        Dot rightNeuron = neuronMatrix[i, j + 1];
                        neightbours.Add(rightNeuron);
                    }

                    foreach (var neightbor in neightbours)
                    {
                        int neighbourX = neightbor.getX() + screenGraphWidth / 2 + xOffset;
                        int neighbourY = screenGraphHeight / 2 - neightbor.getY() + yOffset;

                        Dot convertedNeightbour = convertToScreen(neightbor);

                        graphics.DrawLine(whitePen, convertedNeuron.X, convertedNeuron.Y, convertedNeightbour.X, convertedNeightbour.Y);
                    }
                }
            }
        }

        private void drawPoint(Dot point, int size)
        {
            Dot converted = convertToScreen(point);
            Brush whiteBrush = new SolidBrush(Color.White);
            graphics.FillEllipse(whiteBrush, new Rectangle(converted.X - size, converted.Y - size, 2 * size, 2 * size));
        }

        private Dot convertToScreen(Dot input)
        {
            int screenX = input.X + screenGraphWidth / 2 + xOffset;
            int screenY = screenGraphHeight / 2 - input.Y + yOffset;

            return new Dot(screenX, screenY);
        }


        // Centroids
        private void drawAxis()
        {
            Pen whitePen = new Pen(Color.White, 1);

            int x1 = xOffset - overflow;
            int y1 = screenGraphHeight / 2 + yOffset;
            int x2 = screenGraphWidth + xOffset + overflow;
            int y2 = screenGraphHeight / 2 + yOffset;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);


            x1 = (int)(screenGraphWidth / 2.0 + xOffset);
            y1 = yOffset - overflow;
            x2 = (int)(screenGraphWidth / 2.0 + xOffset);
            y2 = screenGraphHeight + yOffset + overflow;
            graphics.DrawLine(whitePen, x1, y1, x2, y2);


            int xLimit = 300 + screenGraphWidth / 2;
            int yLimit = screenGraphHeight / 2 - 300;

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

        private List<MyPoint> readPointsFromFile()
        {
            List<MyPoint> readPoints = new List<MyPoint>();

            var lines = File.ReadLines(@"X:\School Repos\Invatare Automata\Kmeans2\points.txt");

            foreach (var line in lines)
            {
                string[] split = line.Split(" ");
                readPoints.Add(new MyPoint(Int16.Parse(split[0]), Int16.Parse(split[1]), Int16.Parse(split[2])));
            }

            return readPoints;
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

        private double distance(Dot point1, Dot point2)
        {
            int dX = point2.getX() - point1.getX();
            int dY = point2.getY() - point1.getY();
            return Math.Sqrt(dX * dX + dY * dY);
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


        private void drawPoints(List<MyPoint> points)
        {
            Brush whiteBrush = new SolidBrush(Color.White);
            foreach (var point in points)
            {
                Dot converted = convertToScreen(point);

                graphics.FillEllipse(whiteBrush, new Rectangle(converted.X, converted.Y, 2, 2));
            }
        }

        private void drawPointsFromCentroids(List<Centroid> centroidList)
        {
            foreach (var centroid in centroidList)
            {
                foreach (var point in centroid.getPointArrayList())
                {
                    int screenX = point.getX() + screenGraphWidth / 2 + xOffset;
                    int screenY = screenGraphHeight / 2 - point.getY() + yOffset;

                    graphics.FillEllipse(brushDictionary[centroid.getColor()], new Rectangle(screenX, screenY, 4, 4));
                }
            }
        }

        private void drawCentroids(List<Centroid> centroidList)
        {
            SolidBrush blackBrush = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.White);

            foreach (var centroid in centroidList)
            {
                int screenX = centroid.getX() + screenGraphWidth / 2 + xOffset;
                int screenY = screenGraphHeight / 2 - centroid.getY() + yOffset;

                graphics.FillEllipse(blackBrush, new Rectangle(screenX - bigRadius, screenY - bigRadius, 2 * bigRadius, 2 * bigRadius));
                graphics.FillEllipse(brushDictionary[centroid.getColor()], new Rectangle(screenX - radius, screenY - radius, 2 * radius, 2 * radius));
            }
        }

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

                    int screenX = x + screenGraphWidth / 2 + xOffset;
                    int screenY = screenGraphHeight / 2 - y + yOffset;

                    graphics.FillEllipse(brushDictionary[bestCentroid.getColor()], new Rectangle(screenX - radius, screenY - radius, 2 * radius, 2 * radius));

                }
            }
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