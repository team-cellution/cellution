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
        public enum Genes
        {
            EatA,
            EatC,
            EatG,
            EatT,
            Divide,
            Wander,
            Attack,
            Wait
        }

        public Dictionary<Genes, double> genes;

        public DNA()
        {
            genes = new Dictionary<Genes, double>();

            for (int i = 0; i < 8; i++)
            {
                genes.Add((Genes)i, 1);
            }
            RecalcEpigenes();
        }

        // rebalance the genes probabilities to total to 1.00 (100%)
        public void RecalcEpigenes()
        {
            double maxVal = Double.MinValue;
            double minVal = Double.MaxValue;
            double sum = 0;
            foreach (var gene in genes)
            {
                sum += gene.Value;
                if (gene.Value > maxVal)
                {
                    maxVal = gene.Value;
                }
                if (gene.Value < minVal)
                {
                    minVal = gene.Value;
                }
            }
            double ratio = 1 / sum;
            Dictionary<Genes, double> tempList = new Dictionary<Genes, double>();
            foreach (var gene in genes)
            {
                tempList.Add(gene.Key, gene.Value * ratio);
            }
            genes = tempList;
        }

        // print out the dna in the debug console
        public void Print()
        {
            Console.Write("\n");
            foreach (var gene in genes)
            {
                Console.Write($"{gene.Key.ToString()}: {Math.Round(gene.Value, 3) * 100}%");
            }
            Console.Write("\n");
        }

        // increase the probability of a gene by a percent, then recalc the epigenes
        public void InfluenceGene(Genes gene, double percent)
        {
            ReplaceGene(gene, genes[gene] + percent / 100 / genes.Count);
            RecalcEpigenes();
        }

        public void ReplaceGene(Genes gene, double newEpigene)
        {
            genes[gene] = newEpigene;
        }

        // randomly assigns probabilities to the genes and recalcs the totals
        public void Randomize()
        {
            for (int i = 0; i < genes.Count; i++)
            {
                ReplaceGene((Genes)i, World.Random.Next(100));
            }
            RecalcEpigenes();
        }

        //public static List<T> Shuffle<T>(List<T> list)
        //{
        //    List<T> tempList = list; 
        //    int n = tempList.Count;
        //    while (n > 1)
        //    {
        //        n--;
        //        int k = World.Random.Next(n + 1);
        //        T value = tempList[k];
        //        tempList[k] = tempList[n];
        //        tempList[n] = value;
        //    }
        //    return tempList;
        //}
    }
}
