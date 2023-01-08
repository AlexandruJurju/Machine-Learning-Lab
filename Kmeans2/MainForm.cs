using Kmeans2.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Resources.ResXFileRef;

namespace Kmeans2
{
	public partial class MainForm : Form
	{
		int screenGraphWidth = 700;
		int screenGraphHeight = 700;
		int realGraphWidth = 600;
		int realGraphHeight = 600;
		int yOffset = 100;
		int xOffset = 50;
		List<MyPoint> points = new List<MyPoint>();
		Random random = new Random();
		Graphics graphics;

		List<Centroid> centroids = new List<Centroid>();
		int step = 0;
		int radius = 4;
		int bigRadius = 8;
		List<Color> colorList = new List<Color>();
		Dictionary<Color, SolidBrush> brushDictionary = new Dictionary<Color, SolidBrush>();

		int neuronMatrixSize = 10;
		Neuron[,] neuronMatrix;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			this.graphics = this.CreateGraphics();
			this.BackColor = Color.FromArgb(47, 47, 47);
			initColors();

			points = readPointsFromFile();
			centroids = generateCentroids();

			neuronMatrix = initNeuronPositions(neuronMatrixSize);
		}

		private void buttonDrawOutputPoints_Click(object sender, EventArgs e)
		{
			graphics.Clear(Color.FromArgb(47, 47, 47));
			drawAxis();
			//foreach (var point in points)
			//{
			//	drawPoint(point, Color.Black, 2);
			//}

			List<MyPoint> outputPoints = readPointsFromOutputFile();

			Dictionary<int, Color> zoneDictionary = new Dictionary<int, Color>();
			for (int i = 0; i < outputPoints.Count; i += 9)
			{
				MyPoint point = outputPoints[i];
				int zone = point.getZone();
				if (!zoneDictionary.ContainsKey(zone))
				{
					textBoxPrinting.Text += zoneDictionary.Count + " " + colorList[zoneDictionary.Count] + Environment.NewLine;
					zoneDictionary.Add(zone, colorList[zoneDictionary.Count]);
				}

				drawPoint(point, zoneDictionary[zone], 2);
			}

			/*			foreach (var point in outputPoints)
						{
							int zone = point.getZone();
							if (!zoneDictionary.ContainsKey(zone))
							{
								textBoxPrinting.Text += zoneDictionary.Count + " " + colorList[zoneDictionary.Count] + Environment.NewLine;
								zoneDictionary.Add(zone, colorList[zoneDictionary.Count]);
							}

							drawPoint(point, zoneDictionary[zone], 1);
						}*/

			/*			MyPoint pointTest = new MyPoint(176, -142, 3);
						drawPoint(pointTest, Color.Purple, 5);*/
		}

		private void buttonDrawInputPoints_Click(object sender, EventArgs e)
		{
			drawAxis();

			Dictionary<int, Color> zoneDictionary = new Dictionary<int, Color>();

			foreach (var point in points)
			{
				int zone = point.getZone();
				if (!zoneDictionary.ContainsKey(zone))
				{
					zoneDictionary.Add(zone, colorList[zoneDictionary.Count]);
				}

				drawPoint(point, zoneDictionary[zone], 1);
			}
		}


		private List<MyPoint> readPointsFromOutputFile()
		{
			List<MyPoint> readPoints = new List<MyPoint>();

			var lines = File.ReadLines(@"X:\School Repos\Invatare Automata\Kmeans2\point_output.txt");

			foreach (var line in lines)
			{
				string[] split = line.Split(" ");
				readPoints.Add(new MyPoint(Int16.Parse(split[0]), Int16.Parse(split[1]), Int16.Parse(split[2])));
			}

			return readPoints;
		}

		// SOM
		private void buttonDrawNeurons_Click(object sender, EventArgs e)
		{
			drawAxis();

			MyPoint pt = new MyPoint(-300, 300, 1);
			drawNeuron(pt, Color.Crimson);
		}
		private void buttonSOMFullRun_Click(object sender, EventArgs e)
		{
			double learningRate = 0.6;
			double neighbourhoodDistance = 6.1;
			int epoch = 0;
			double epochsLearning = 10.0;

			while (learningRate > 0.01)
			{
				learningRate = 0.6 * Math.Exp(-epoch / epochsLearning);
				neighbourhoodDistance = 6.1 * Math.Exp(-epoch / epochsLearning);

				textBoxPrinting.Text = "Epoch " + epoch + Environment.NewLine;
				textBoxPrinting.AppendText("Learning Rate : " + learningRate + Environment.NewLine);
				textBoxPrinting.AppendText("Distance : " + (int)Math.Round(neighbourhoodDistance) + Environment.NewLine);

				foreach (var point in points)
				{
					Neuron closest = findClosestNeuron(neuronMatrix, point);

					double newWeightX = closest.WeightX + learningRate * (point.X - closest.WeightX);
					double newWeightY = closest.WeightY + learningRate * (point.Y - closest.WeightY);

					closest.WeightX = newWeightX;
					closest.WeightY = newWeightY;

					List<Neuron> neighbors = findNeighbors(neuronMatrix, closest, (int)Math.Round(neighbourhoodDistance));

					if (neighbors.Count > 0)
					{
						foreach (var neighbor in neighbors)
						{
							double neighborNewWeightX = neighbor.WeightX + learningRate * (point.X - neighbor.WeightX);
							double neighborNewWeightY = neighbor.WeightY + learningRate * (point.Y - neighbor.WeightY);
							neighbor.WeightX = neighborNewWeightX;
							neighbor.WeightY = neighborNewWeightY;
						}
					}
				}
				epoch++;
			}

			foreach (var neuron in neuronMatrix)
			{
				neuron.X = (int)neuron.WeightX;
				neuron.Y = (int)neuron.WeightY;
			}

			//drawNeuronsOneByOne();

			graphics.Clear(Color.FromArgb(47, 47, 47));
			drawAxis();
			drawPoints(points);
			drawNeightbourLines(neuronMatrix);
		}
		private void drawNeuronsOneByOne()
		{
			List<Neuron> prevNeurons = new List<Neuron>();
			foreach (var neuron in neuronMatrix)
			{
				graphics.Clear(Color.FromArgb(47, 47, 47));

				foreach (var neuros in prevNeurons)
				{
					drawNeuron(neuros, Color.Red);
				}
				drawNeuron(neuron, Color.Cyan);
				prevNeurons.Add(neuron);
				Thread.Sleep(1000);
			}
		}
		private double neuronDistanceToPoint(Dot point, Neuron neuron)
		{
			double dX = neuron.WeightX - point.X;
			double dY = neuron.WeightY - point.Y;
			return Math.Sqrt(dX * dX + dY * dY);
		}

		// find closest neuron should use neuron weight
		private Neuron findClosestNeuron(Neuron[,] neuronMatrix, Dot point)
		{
			double minDistance = double.MaxValue;
			Neuron closest = null;
			for (int i = 0; i < neuronMatrixSize; i++)
			{
				for (int j = 0; j < neuronMatrixSize; j++)
				{
					Neuron currentNeuron = neuronMatrix[i, j];
					double currentDistance = neuronDistanceToPoint(point, currentNeuron);
					if (currentDistance < minDistance)
					{
						minDistance = currentDistance;
						closest = currentNeuron;
					}
				}
			}

			//foreach (var neuron in neuronMatrix)
			//{
			//    double currentDistance = distance(point, neuron);
			//    if (currentDistance < minDistance)
			//    {
			//        minDistance = currentDistance;
			//        closest = neuron;
			//    }
			//}

			if (closest == null)
			{
				throw new Exception("ERROR");
			}

			return closest;
		}
		private Neuron[,] initNeuronPositions(int neuronMatrixSize)
		{
			Neuron[,] output = new Neuron[neuronMatrixSize, neuronMatrixSize];

			int xStep = realGraphWidth / neuronMatrixSize;
			int yStep = realGraphHeight / neuronMatrixSize;

			for (int i = -realGraphWidth / 2 + xStep / 2, columnCount = 0; i < realGraphWidth / 2; i += xStep, columnCount++)
			{
				for (int j = realGraphHeight / 2 - yStep / 2, lineCount = 0; j > -realGraphHeight / 2; j -= yStep, lineCount++)
				{
					output[lineCount, columnCount] = new Neuron(i, j, i, j);
				}
			}

			return output;
		}
		private Tuple<int, int> findCoordinate(Dot[,] neuronMatrix, Dot origin)
		{
			for (int i = 0; i < neuronMatrixSize; i++)
			{
				for (int j = 0; j < neuronMatrixSize; j++)
				{
					if (neuronMatrix[i, j] == origin)
					{
						return new Tuple<int, int>(i, j);
					}
				}
			}
			return null;
		}
		private List<Neuron> findNeighbors(Neuron[,] neuronMatrix, Neuron origin, int neighborhood)
		{
			List<Neuron> neighbors = new List<Neuron>();

			Tuple<int, int> originCoordinates = findCoordinate(neuronMatrix, origin);

			for (int i = originCoordinates.Item1 - neighborhood; i <= originCoordinates.Item1 + neighborhood; i++)
			{
				for (int j = originCoordinates.Item2 - neighborhood; j <= originCoordinates.Item2 + neighborhood; j++)
				{
					if (i >= 0 && i < neuronMatrixSize)
					{
						if (j >= 0 && j < neuronMatrixSize)
						{
							if (neuronMatrix[i, j] != origin)
							{
								neighbors.Add(neuronMatrix[i, j]);
							}
						}
					}

				}
			}


			return neighbors;
		}

		/*        private List<Neuron> findNeighborsByDistance(Neuron[,] neuronMatrix, Neuron origin, int neighborhood)
				{
					List<Neuron> tempNeigh = new List<Neuron>();

					List<Neuron> neightbors = new List<Neuron>();

					Dictionary<Neuron, double> neuronDistances = new Dictionary<Neuron, double>();

					foreach (var neuron in neuronMatrix)
					{
						neuronDistances[neuron] = distance(origin, neuron);
					}

					var sortedDict = from entry in neuronDistances orderby entry.Value ascending select entry;


					foreach (var elem in neuronDistances)
					{
						tempNeigh.Add(elem.Key);
					}

					foreach(var neuron in neuronDistances)
					{

					}

				}*/

		private void drawNeightbourLines(Dot[,] neuronMatrix)
		{
			Pen linePen = new Pen(Color.Black, 3);

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

						graphics.DrawLine(linePen, convertedNeuron.X, convertedNeuron.Y, convertedNeightbour.X, convertedNeightbour.Y);
					}
				}
			}
		}
		private void drawNeuron(Dot neuron, Color color)
		{
			SolidBrush brush = new SolidBrush(color);
			Dot converted = convertToScreen(neuron);
			int bigRadius = 2;
			graphics.FillEllipse(brush, new Rectangle(converted.X - bigRadius, converted.Y - bigRadius, 2 * bigRadius, 2 * bigRadius));
			graphics.FillEllipse(brush, new Rectangle(converted.X - radius, converted.Y - radius, 2 * radius, 2 * radius));
		}
		private Dot convertToScreen(Dot input)
		{
			int screenX = (int)(input.X + screenGraphWidth / 2);
			int screenY = (int)((screenGraphHeight / 2 - input.Y));

			return new Dot(screenX, screenY);
		}

		// General
		private void drawAxis()
		{
			Pen whitePen = new Pen(Color.White, 1);

			// x axis
			Dot point1 = convertToScreen(new Dot(0, 300));
			Dot point2 = convertToScreen(new Dot(0, -300));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);

			// y axis
			point1 = convertToScreen(new Dot(300, 0));
			point2 = convertToScreen(new Dot(-300, 0));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);


			//top
			point1 = convertToScreen(new Dot(-300, 300));
			point2 = convertToScreen(new Dot(300, 300));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);

			// bottom
			point1 = convertToScreen(new Dot(-300, -300));
			point2 = convertToScreen(new Dot(300, -300));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);

			// left
			point1 = convertToScreen(new Dot(-300, 300));
			point2 = convertToScreen(new Dot(-300, -300));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);

			// right
			point1 = convertToScreen(new Dot(300, 300));
			point2 = convertToScreen(new Dot(300, -300));
			graphics.DrawLine(whitePen, point1.X, point1.Y, point2.X, point2.Y);
		}
		private void initColors()
		{
			colorList.Add(Color.Blue);
			colorList.Add(Color.Green);
			colorList.Add(Color.Red);
			colorList.Add(Color.Yellow);
			colorList.Add(Color.Cyan);
			colorList.Add(Color.Honeydew);
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
		private double distance(Dot point1, Dot point2)
		{
			return euclidianDistance(point1, point2);
		}
		private double euclidianDistance(Dot point1, Dot point2)
		{
			double dX = point2.X - point1.X;
			double dY = point2.Y - point1.Y;
			return Math.Sqrt(dX * dX + dY * dY);
		}
		private double manhattanDistance(Dot point1, Dot point2)
		{
			return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
		}

		// Centroids
		private void drawPoint(Dot point, Color color, int radius)
		{
			SolidBrush brush = new SolidBrush(color);
			Dot converted = convertToScreen(point);
			graphics.FillEllipse(brush, new Rectangle(converted.X - radius, converted.Y - radius, 2 * radius, 2 * radius));
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


			List<Centroid> noiseCentroids = auxCentroids.FindAll(c => c.getPointArrayList().Count < 10000 / 100);
			foreach (var centroid in noiseCentroids)
			{
				foreach (var point in centroid.getPointArrayList())
				{
					points.Remove(point);
				}
			}
			auxCentroids.RemoveAll(c => c.getPointArrayList().Count < 10000 / 100);
			arrangeCentroids(auxCentroids);
			centroids = auxCentroids;

			textBoxPrinting.AppendText("DONE");

		}
		private void buttonDrawOutlines_Click(object sender, EventArgs e)
		{
			drawOutline(centroids);
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
			for (int x = -300; x < 300; x += 4)
			{
				for (int y = -300; y < 300; y += 4)
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

		private async void buttonGeneratePoints_Click(object sender, EventArgs e)
		{
			List<MyZone> zoneList = new List<MyZone>();
			zoneList.Add(new MyZone(180, 220, 25, 15));
			zoneList.Add(new MyZone(50, 25, 20, 20));
			zoneList.Add(new MyZone(-150, -100, 20, 15));
			zoneList.Add(new MyZone(200, -125, 30, 10));

			List<MyPoint> myPoints = GeneratePoints.getPoints(zoneList, 10000, true);

			using StreamWriter file = new(@"X:\\School Repos\\Invatare Automata\\Kmeans2\\points.txt");

			foreach (var point in myPoints)
			{
				await file.WriteLineAsync(point.X + " " + point.Y + " " + point.getZone());
			}

			textBoxPrinting.Text = "Done generating points";
		}


	}
}