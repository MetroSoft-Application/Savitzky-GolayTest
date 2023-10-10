using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
namespace Savitzky_GolayTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int sidePoints = 7;
            int polynomialOrder = 2;

            var sav = new SavitzkyGolayFilter(sidePoints, polynomialOrder);

            int samplingNum = 4096;
            double samplingFreq = 8000;
            double samplingTime = 1 / samplingFreq;
            double amplitude = 10;
            //double offset = 0;
            //double cutoffFreq = 10;

            Debug.WriteLine("input");
            var input = new List<double>();
            for (int i = 0; i < samplingNum; i++)
            {
                input.Add(GetSwept(i * samplingTime, samplingTime, samplingNum, amplitude));
                Debug.WriteLine(input[i]);
            }

            Debug.WriteLine("output");
            var output = sav.Process(input.ToArray());
            foreach (var item in output)
            {
                Debug.WriteLine(item);
            }
        }

        private double GetSwept(double sec, double samplingSec, int samplingNum, double amplitude)
        {
            double p = sec / (samplingSec * samplingNum);
            double K = samplingNum * 0.5;
            double C = 2 * Math.PI / Math.Log(K);
            double rad = C * Math.Pow(K, p) - C;
            double omega = Math.Sin(rad) * amplitude;
            return omega;
        }
    }
}
