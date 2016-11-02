using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cellution
{
    public class DNA
    {
        public List<Tuple<int, double>> genes;

        public DNA()
        {
            genes = new List<Tuple<int, double>>();

            foreach (int i in Enumerable.Range(0, 8))
            {
                genes.Add(new Tuple<int, double>(i, 1));
            }
            recalcEpigenes();
        }

        public void recalcEpigenes()
        {
            double maxVal = Double.MinValue;
            double minVal = Double.MaxValue;
            double sum = 0;
            foreach (Tuple<int, double> gene in genes)
            {
                sum += gene.Item2;
                if (gene.Item2 > maxVal)
                {
                    maxVal = gene.Item2;
                }
                if (gene.Item2 < minVal)
                {
                    minVal = gene.Item2;
                }
            }
            double ratio = 1 / sum;
            List<Tuple<int, double>> tempList = new List<Tuple<int, double>>();
            foreach (Tuple<int, double> gene in genes)
            {
                tempList.Add(new Tuple<int, double>(gene.Item1, gene.Item2 * ratio));
            }
            genes = tempList;
        }

        public void print()
        {
            Console.Write("\n");
            foreach (Tuple<int, double> gene in genes)
            {
                switch (gene.Item1)
                {
                    case 0:
                        Console.Write(" EatA:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 1:
                        Console.Write(" EatC:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 2:
                        Console.Write(" EatT:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 3:
                        Console.Write(" EatG:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 4:
                        Console.Write(" EatA:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 5:
                        Console.Write(" Wander:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 6:
                        Console.Write(" Attack:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    case 7:
                        Console.Write(" Wait:" + Math.Round(gene.Item2, 3) * 100 + "%");
                        break;
                    default:
                        break;

                }
            }
            Console.Write("\n");
        }

        public void influenceGene(int index, double percent)
        {
            replaceGene(index, genes[index].Item1, genes[index].Item2 + percent / 100 / genes.Count);
            recalcEpigenes();
        }

        public void replaceGene(int index, int newBehavior, double newEpigene)
        {
            List<Tuple<int, double>> tempList = new List<Tuple<int, double>>();
            int tempIndex = 0;
            foreach (Tuple<int, double> gene in genes)
            {
                if (tempIndex == index)
                {
                    tempList.Add(new Tuple<int, double>(newBehavior, newEpigene));
                }
                else
                {
                    tempList.Add(new Tuple<int, double>(gene.Item1, gene.Item2));
                }
                tempIndex++;
            }
            genes = tempList;
        }

        public void Randomize()
        {
            int index = 0;
            foreach (Tuple<int, double> gene in genes)
            {
                replaceGene(index, gene.Item1, World.Random.Next(100));
                index++;
            }
            recalcEpigenes();
        }
    }
}
