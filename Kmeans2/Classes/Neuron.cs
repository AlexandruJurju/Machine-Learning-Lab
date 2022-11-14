using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans2.Classes
{
    public class Neuron : Dot
    {
        double weightX;
        double weightY;
        public Neuron(int x, int y, double weightX, double weightY) : base(x, y)
        {
            this.weightX = weightX;
            this.weightY = weightY;
        }

        public double WeightX { get => weightX; set => weightX = value; }
        public double WeightY { get => weightY; set => weightY = value; }
    }
}
