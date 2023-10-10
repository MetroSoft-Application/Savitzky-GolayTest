using System;
using MathNet.Numerics.LinearAlgebra;

namespace Savitzky_GolayTest
{
    public class SavitzkyGolayFilter
    {
        private readonly int sidePoints;
        private Matrix<double> coefficients;

        public SavitzkyGolayFilter(int sidePoints, int polynomialOrder)
        {
            this.sidePoints = sidePoints;
            Design(polynomialOrder);
        }

        public double[] Process(double[] samples)
        {
            int length = samples.Length;
            double[] output = new double[length];
            int frameSize = CalculateFrameSize();
            double[] frame = new double[frameSize];

            ProcessBeginning(samples, output, frame);
            ProcessMiddle(samples, output, frame);
            ProcessEnd(samples, output, frame);

            return output;
        }

        private void ProcessBeginning(double[] samples, double[] output, double[] frame)
        {
            Array.Copy(samples, frame, frame.Length);
            for (int i = 0; i < sidePoints; ++i)
            {
                output[i] = CalculateDotProduct(frame, i);
            }
        }

        private void ProcessMiddle(double[] samples, double[] output, double[] frame)
        {
            int length = samples.Length;
            int frameSize = CalculateFrameSize();
            for (int n = sidePoints; n < length - sidePoints; ++n)
            {
                Array.ConstrainedCopy(samples, n - sidePoints, frame, 0, frameSize);
                output[n] = CalculateDotProduct(frame, sidePoints);
            }
        }

        private void ProcessEnd(double[] samples, double[] output, double[] frame)
        {
            int length = samples.Length;
            Array.ConstrainedCopy(samples, length - frame.Length, frame, 0, frame.Length);
            for (int i = 0; i < sidePoints; ++i)
            {
                output[length - sidePoints + i] = CalculateDotProduct(frame, sidePoints + 1 + i);
            }
        }

        private double CalculateDotProduct(double[] frame, int columnIndex)
        {
            return coefficients.Column(columnIndex).DotProduct(Vector<double>.Build.DenseOfArray(frame));
        }

        private int CalculateFrameSize()
        {
            return (sidePoints << 1) + 1;
        }

        private void Design(int polynomialOrder)
        {
            var s = BuildMatrixS(polynomialOrder);
            coefficients = CalculateCoefficients(s);
        }

        private Matrix<double> BuildMatrixS(int polynomialOrder)
        {
            double[,] a = new double[CalculateFrameSize(), polynomialOrder + 1];
            for (int m = -sidePoints; m <= sidePoints; ++m)
            {
                for (int i = 0; i <= polynomialOrder; ++i)
                {
                    a[m + sidePoints, i] = Math.Pow(m, i);
                }
            }
            return Matrix<double>.Build.DenseOfArray(a);
        }

        private Matrix<double> CalculateCoefficients(Matrix<double> s)
        {
            var sTranspose = s.Transpose();
            var sTransposeTimesS = sTranspose.Multiply(s);
            var inverseOfSTransposeTimesS = sTransposeTimesS.Inverse();
            return s.Multiply(inverseOfSTransposeTimesS).Multiply(sTranspose);
        }
    }
}
