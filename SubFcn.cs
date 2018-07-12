using System; //Array, Math, Convert
using System.Collections.Generic; //List

namespace PeakSearch
{
    class fcn
    {
        public static int val2ind(float[] x, float val)
        {
            int pindex = 0;
            pindex = Array.BinarySearch(x , val);
            return pindex;
        }
        //
        public static float[] deriv(float[] a)
        {
            /* First derivative of vector using 2 - point central difference method.*/
            int n;
            n = a.Length;
            float[] d = new float[n]; 
            d[0] = a[1] - a[0];
            d[n-1] = a[n-1] - a[n - 2];
            for (int j = 1; j<=n-2 ; j++)
            {
                d[j] = (a[j + 1] - a[j - 1]) / 2;
            }
            return d;
        }
        //
        public static float[] fastsmooth(float[] Y, int w, int type, int ends)
        {
            int n;
            n = Y.Length;
            float[] SmoothY = new float[n];
            switch (type)
            {
                case 1:
                    SmoothY = sa(Y, w, ends);
                    break;
                case 2:
                    SmoothY = sa(sa(Y, w, ends), w, ends);
                    break;
                case 3:
                    SmoothY = sa(sa(sa(Y, w, ends), w, ends), w, ends);
                    break;
                default:
                    SmoothY = sa(Y, w, ends);
                    break;
            }
            return SmoothY; //return float array type
        }
        //
        public static float[] sa(float[] Y, int w, int ends)
        {
            int L;
            L = Y.Length;
            double halfw;
            int int_halfw;
            float SumPoints;
            float[] s = new float[L];
            float[] SmoothY = new float[L];
            float[] Ysw = new float[w];
            List<float> list_head = new List<float>();
            List<float> list_end = new List<float>();
            halfw = (double) w / 2f;
            halfw = Math.Round(halfw, MidpointRounding.AwayFromZero); // function(halfw) needs double type
            int_halfw = Convert.ToInt32(halfw);
            //Console.WriteLine(halfw);
            //Console.ReadKey();
            Array.Copy(Y, Ysw, w); // copy Y to Ysw with w-length.            
            SumPoints = sum(Ysw);
            //
            for (int k = 0; k < L-w; k++)
            {
                s[k + int_halfw - 1] = SumPoints;
                SumPoints = SumPoints - Y[k];
                SumPoints = SumPoints + Y[k + w];
            }
            Array.Copy(Y, L-w, Ysw, 0, w);
            s[L-w-1 + int_halfw] = sum(Ysw);
            //
            for (int i = 0; i < s.Length; i++)
            {
                SmoothY[i] = s[i]/w;
            }
            //
            /* Taper the ends of the signal if ends=1. */
            //
            if (ends == 1)
            {
                w = (w + 1) / 2;
                SmoothY[0] = (Y[0] + Y[1]) / 2;
                for (int k = 1; k < w; k++)
                {
                    for (int i = 0; i <= 2*k; i++)
                    {
                        list_head.Add(Y[i]);
                    }
                    for (int i=L-2*k-1; i<=L-1; i++)
                    {
                        list_end.Add(Y[i]);
                    }
                    float[] Ytp_H = list_head.ToArray();
                    SmoothY[k] = sum(Ytp_H) / Ytp_H.Length;
                    float[] Ytp_E = list_end.ToArray();
                    SmoothY[L-k-1] = sum(Ytp_E) / Ytp_E.Length;
                    list_head.Clear();
                    list_end.Clear();
                }
                //
                SmoothY[L-1] = (Y[L-1]+Y[L-2])/2;
            }
            //
            return SmoothY;
        }
        //
        public static float sum(float[] Ysw)
        {
            float SumPoints = 0;
            for (int i = 0; i < Ysw.Length; i++)
            {
                SumPoints = SumPoints + Ysw[i];
            }
            return SumPoints; // return float var type
        }
       //
    }
}