using lab11;
using System;
using System.Threading.Tasks;

class Program
{
    const int n = 6;
    const int m = 6;
    static double[,] A = new double[n, m];
    static double[] B = new double[m];
    static double[] resultRow = new double[n];
    static double[] resultBlock = new double[n];

    static void Main()
    {
        double x = 5;
        GenerateMatrixAndVector(x);

        MultiplyMatrixVectorRowWise();
        Console.WriteLine("\nMultiplication result (row division):");
        Print.Vector(resultRow, "Vector[C1]");

        MultiplyMatrixVectorBlockWise();
        Console.WriteLine("\nMultiplication result (block division):");
        Print.Vector(resultBlock, "Vector[C2]");

        Console.ReadKey();
    }

    static void GenerateMatrixAndVector(double x)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                A[i, j] = Math.Round((Math.Pow(i, 2) - Math.Pow(x, 3) + 1) / (j + Math.Sin(Math.Pow(x, 0.2))) - Math.Pow(x, 1.3), 4);
            }
        }

        Print.Matrix(A, "Matrix[A]");

        for (int j = 0; j < m; j++)
        {
            double denominator = j + Math.Pow(Math.Sin(4 * j), 0.2);

            if (denominator == 0 || double.IsNaN(denominator))
            {
                //Console.WriteLine($"Warning: Division by zero or NaN at j = {j}");
                denominator = 1;
                continue;
            }

            B[j] = Math.Round((Math.Pow(x, 2) - Math.Pow(2, j) + 1) / denominator, 4);
        }

        Print.Vector(B, "Vector[B]");
    }

    static void MultiplyMatrixVectorRowWise()
    {
        Parallel.For(0, n, i =>
        {
            double sum = 0;
            for (int j = 0; j < m; j++)
            {
                sum += A[i, j] * B[j];
            }
            resultRow[i] = sum;
        });
    }

    static void MultiplyMatrixVectorBlockWise()
    {
        int numThreads = Environment.ProcessorCount;
        int blockSize = n / numThreads;
        Parallel.For(0, numThreads, t =>
        {
            int start = t * blockSize;
            int end = (t == numThreads - 1) ? n : start + blockSize;

            for (int i = start; i < end; i++)
            {
                double sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += A[i, j] * B[j];
                }
                resultBlock[i] = sum;
            }
        });
    }
}
