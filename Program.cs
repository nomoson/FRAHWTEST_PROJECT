using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeakSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            // for 7375 parameters
            float SlopeThreshold = 0.3F, AmpThreshold = 1F;
            int i = 0, counter = 0, smoothwidth = 5, peakgroup = 2, smoothtype = 1, ends = 0;
            string line;

            // Read the file and count & display lines.  
            FileStream fs = new FileStream(@"C:\Users\jay\Documents\Visual Studio 2015\Projects\PeakSearch\7375.txt", FileMode.Open);
            StreamReader file = new StreamReader(fs, Encoding.Default);
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                counter++;
            }
            fs.Position = 0;

            System.Console.WriteLine("There were {0} Raw lines.", counter);
            Console.WriteLine("-------------------------");
            // Suspend the screen.  
            System.Console.ReadLine();

            /* load txt file to x-y-z vector */
            string[] s2 = new string[3];
   
            // Assign x,y,z vector size
            float[] x = new float[counter];
            float[] y = new float[counter];
            float[] d = new float[counter];
            int[]   z = new int[counter];

            line = file.ReadLine();
            while(line != null)
            {
                s2 = line.Split(' ');
                x[i] = float.Parse(s2[0]);
                y[i] = float.Parse(s2[1]);
                z[i] = int.Parse(s2[2]);
                //System.Console.WriteLine(x[i]);
                i++;
                line = file.ReadLine();
            }
            // System.Console.ReadLine();
            file.Close();
            fs.Close();
            //
            /*  quote function */
            if (smoothwidth > 1)
            {
                d = fcn.fastsmooth(fcn.deriv(y), smoothwidth, smoothtype, ends);
            }
            else
            {
                d = fcn.deriv(y);
            }
            //
            // TEST from HENRY
            //foreach (float j in d)
            //{
            //    System.Console.WriteLine(j);
            //}
           // Console.WriteLine("Here are {0} first derivatives", counter);
           // Console.WriteLine("------------------------------");
           // Console.ReadKey();
            //
            // Assign result summary vector
            List<List<float>> Plist = new List<List<float>>(); //2D list 
            int vectorlength = y.Length;
            int peak = 1;
            int pindex;
            float AmpTest = AmpThreshold;
            float PeakX = 0, PeakY = 0, PeakZ = 0;
            // Round and double2int
            double smth = Math.Round((double) smoothwidth / 2f, MidpointRounding.AwayFromZero);
            int int_smth = Convert.ToInt32(smth);
            double pkgp = Math.Round((double) peakgroup / 2f, MidpointRounding.AwayFromZero);
            int int_pkgp = Convert.ToInt32(pkgp);
            double n = Math.Round(((double) peakgroup / 2f + 1f), MidpointRounding.AwayFromZero);
            int int_n = Convert.ToInt32(n);
            //
            for (int j = 2*int_smth-2; j < y.Length-smoothwidth-1; j++)
            {
                if ( Math.Sign(d[j]) > Math.Sign(d[j+1]) )
                {
                    if ((d[j] - d[j + 1]) > SlopeThreshold)
                    {
                        float[] xx = new float[peakgroup];
                        float[] yy = new float[peakgroup];
                        float[] zz = new float[peakgroup];
                        //
                        for (int k=0; k < peakgroup; k++)
                        {
                            int groupindex = j + k - int_n + 3;
                            if (groupindex < 1) groupindex = 1;
                            if (groupindex > vectorlength) groupindex = vectorlength;
                            xx[k] = x[groupindex];
                            yy[k] = y[groupindex];
                            zz[k] = z[groupindex];
                        }
                        //
                        if (peakgroup < 3)
                        {
                            if ( yy.Length == 2 && yy[0] > yy[1] )
                            {
                               PeakY = yy[0]; // max. value picked up by first
                            }
                            else
                            {
                                PeakY = yy[yy.Length - 1]; // max. value picked up by last
                            }
                        }
                        else
                        {
                            PeakY = fcn.sum(yy) / yy.Length;
                        }
                        pindex = fcn.val2ind(yy, PeakY);
                        PeakX = xx[pindex]; PeakZ = zz[pindex];
                        //
                        if ( Double.IsNaN(PeakX) || Double.IsNaN(PeakY) || Double.IsNaN(PeakZ) || PeakY < AmpThreshold )
                        {
                            //Skip this peak
                        }
                        else
                        {
                            Plist.Add(new List<float>(4){ peak, PeakX, PeakY, PeakZ });
                            peak = peak + 1; // Move on to next peak
                        }
                    }
                }
            }
            //List<float>[] PSUMY = Plist.ToArray();
            Console.WriteLine("PeakSearch SUMMARY");
            Console.WriteLine("Pkeak # Freq(Hz) Gain(dB) Phase(dg)");
            for (int k = 0; k < Plist.Count; k++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(Plist[k][j]+"      ");
                }
                Console.Write('\n');
            }
            //Console.WriteLine(Plist[1][3]);
            Console.ReadKey();
        }
    }
}
