using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class DNA
    {
        public List<Tuple<int, double>> genes;
        private Dictionary<string, TextItem> values;

        public DNA()
        {
            genes = new List<Tuple<int, double>>();
            values = new Dictionary<string, cellution.TextItem>();
            values.Add("a", new TextItem(World.fontManager["InfoFont"], "EatA"));
            values.Add("c", new TextItem(World.fontManager["InfoFont"], "EatC"));
            values.Add("g", new TextItem(World.fontManager["InfoFont"], "EatG"));
            values.Add("t", new TextItem(World.fontManager["InfoFont"], "EatT"));
            values.Add("wander", new TextItem(World.fontManager["InfoFont"], "Wander"));
            values.Add("attack", new TextItem(World.fontManager["InfoFont"], "Attack"));
            values.Add("wait", new TextItem(World.fontManager["InfoFont"], "Wait"));

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

        public void SetUpDNAValues(Vector2 cellPosition)
        {
            values["a"].Text = $"EatA: {(genes[0].Item2 * 100).ToString("0.##")}%";
            values["a"].position = cellPosition;
            values["c"].Text = $"EatC: {(genes[1].Item2 * 100).ToString("0.##")}%";
            values["c"].PositionBelow(values["a"]);
            values["g"].Text = $"EatG: {(genes[2].Item2 * 100).ToString("0.##")}%";
            values["g"].PositionBelow(values["c"]);
            values["t"].Text = $"EatT: {(genes[3].Item2 * 100).ToString("0.##")}%";
            values["t"].PositionBelow(values["g"]);
            values["wander"].Text = $"Wander: {(genes[5].Item2 * 100).ToString("0.##")}%";
            values["wander"].PositionBelow(values["t"]);
            values["attack"].Text = $"Attack: {(genes[6].Item2 * 100).ToString("0.##")}%";
            values["attack"].PositionBelow(values["wander"]);
            values["wait"].Text = $"Wait: {(genes[7].Item2 * 100).ToString("0.##")}%";
            values["wait"].PositionBelow(values["attack"]);
        }

        public void DrawDNAValues(SpriteBatch spriteBatch)
        {
            foreach (var value in values)
            {
                value.Value.Draw(spriteBatch);
            }
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
