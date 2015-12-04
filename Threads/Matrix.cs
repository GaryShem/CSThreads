using System;
using System.Threading;
using System.Threading.Tasks;

namespace Threads
{
    class Matrix
    {
        private int _rows, _columns;
        private int[,] matrix;
        private static int cpuCount = Environment.ProcessorCount;

        public Matrix(int nRows, int nColumns)
        {
            if (nRows <= 0 || nColumns <= 0)
                throw new ArgumentException("Size should be positive");
            _rows = nRows;
            _columns = nColumns;
            matrix = new int[_rows, _columns];
        }

        public Matrix(int[,] nMatrix)
        {
            _rows = nMatrix.GetLength(0);
            _columns = nMatrix.GetLength(1);
            matrix = (int[,]) nMatrix.Clone();
        }

        public void FillWithRandom(int minValue, int maxValue)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    matrix[i, j] = r.Next(minValue, maxValue);
        }
        public void FillWithRandom(int maxValue)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    matrix[i, j] = r.Next(maxValue);
        }
        public int[,] Values
        {
            get
            {
                return (int[,])matrix.Clone();
            }
        }

        public void Print()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(" {0}", matrix[i,j]);
                }
                Console.WriteLine();
            }
        }
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1 == null || m2 == null)
                throw new ArgumentNullException();
            if (m1._rows != m2._rows || m1._rows != m2._columns)
                throw new ArgumentException("Matrices have different sizes");
            SemaphoreSlim ss = new SemaphoreSlim(cpuCount, cpuCount);
            Matrix result = new Matrix(m1._rows, m1._columns);
            lock (m2) lock(m1)
            for (int i = 0; i < m1.matrix.GetLength(0); i++)
            {
                for (int j = 0; j < m1.matrix.GetLength(1); j++)
                {
                    var row = i;
                    var col = j;
                    Task t = new Task(() =>
                    {
                        ss.Wait();
                        result.matrix[row, col] = m1.matrix[row, col] + m2.matrix[row, col];
                        ss.Release();
                    });
                    t.Start();
                }
            }
            while (ss.CurrentCount < cpuCount)
                Thread.Sleep(1000);
            return result;
        }
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1 == null || m2 == null)
                throw new ArgumentNullException();
            if (m1._rows != m2._rows || m1._rows != m2._columns)
                throw new ArgumentException("Matrices have different sizes");
            SemaphoreSlim ss = new SemaphoreSlim(cpuCount, cpuCount);
            Matrix result = new Matrix(m1._rows, m1._columns);
            lock (m2) lock (m1)
                    for (int i = 0; i < m1.matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < m1.matrix.GetLength(1); j++)
                        {
                            var row = i;
                            var col = j;
                            Task t = new Task(() =>
                            {
                                ss.Wait();
                                result.matrix[row, col] = m1.matrix[row, col] - m2.matrix[row, col];
                                ss.Release();
                            });
                            t.Start();
                        }
                    }
            while (ss.CurrentCount < cpuCount)
                Thread.Sleep(1000);
            return result;
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1 == null || m2 == null)
                throw new ArgumentNullException();
            if (m1._columns != m2._rows)
                throw new ArgumentException("Matrices have incompatible sizes");
            SemaphoreSlim ss = new SemaphoreSlim(cpuCount, cpuCount);
            Matrix result = new Matrix(m1._rows, m2._columns);
            lock (m2) lock (m1)
                    for (int i = 0; i < m1.matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < m2.matrix.GetLength(1); j++)
                        {
                            var row = i;
                            var col = j;
                            Task t = new Task(() =>
                            {
                                ss.Wait();
                                int element = 0;
                                result.matrix[row, col] = 0;
                                for (int ii = 0; ii < m1.matrix.GetLength(1); ii++)
                                    element += m1.matrix[row, ii] * m2.matrix[ii, col];
                                result.matrix[row, col] = element;
                                ss.Release();
                            });
                            t.Start();
                        }
                    }
            while (ss.CurrentCount < cpuCount)
            {
                Thread.Sleep(1000);
            }
            return result;
        }
        
    }
}
