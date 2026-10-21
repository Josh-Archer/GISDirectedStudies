using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectedStudies
{
    public partial class Form1 : Form
    {
        double[][][] input = new double[366][][];
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                Stream stream = openFileDialog1.OpenFile();

                StreamReader reader = new StreamReader(stream);
                String line;

                //store the times
                Dictionary<double, int?> times = new Dictionary<double, int?>();

                //store each line
                List<double[]> lines = new List<double[]>();

                //while loop to go through each line and count the number of arrays for the 2 level and store the line
                while ((line = reader.ReadLine()) != null)
                {
                    //Skip blank, short, non-numeric or out-of-range (time outside 0..365) rows
                    //instead of throwing when the time is used as an index into input[366].
                    double[] arr;
                    if (!InputValidation.TryParseRow(line, out arr))
                    {
                        continue;
                    }

                    InputValidation.IncrementCount(times, arr[2]);

                    //put the line into lines variable
                    lines.Add(arr);
                }

                //Done with reader
                reader.Close();

                //go through and initalize the second level array to the right size
                foreach (KeyValuePair<double, int?> entry in times)
                {
                    if (!InputValidation.IsValidTime(entry.Key) || !entry.Value.HasValue)
                    {
                        continue;
                    }
                    input[(int) entry.Key] = new double[(int) entry.Value][];
                }

                //Need to go through the lines again and store the data
                foreach (double[] l in lines)
                {
                    if (!InputValidation.IsValidRow(l))
                    {
                        continue;
                    }
                    int slot = (int)l[2];
                    if (input[slot] == null)
                    {
                        continue;
                    }
                    //initialize the third level of the 3 dimensional array
                    for (int i = 0; i < input[slot].Length; i++) {
                        if (input[slot][i] == null)
                        {
                            input[slot][i] = new double[3];

                            //Now assign the values (X at index 0, Y at index 1, and C at index 2 of the array) i.e. to access x of first iteration at time 1: input[1][0][0]
                            input[slot][i][0] = l[0];
                            input[slot][i][1] = l[1];
                            input[slot][i][2] = l[3];

                            break;
                        }
                    }
                }
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            double fieldX, fieldY, fieldT;

            fieldX = (double) numericUpDownX.Value;
            fieldY = (double) numericUpDownY.Value;
            fieldT = (double) numericUpDownT.Value;

            //pass to the object to calculate
           // IDW(fieldX, fieldY, fieldT, input);
        }

        //----- Alex's Code begins -----//
        /* The inputs are the:
         * fieldX = the X value inputted on the interface
         * fieldY = the Y value inputted on the interface
         * fileX = all the X values in the list from a certain time period
         * fileY = all the Y values in the list from a certain time period
         * fileC = all the C (weighted values) in a list from a certain time period
         */
       /* public double IDW(double fieldX, double fieldY, double fieldT, Dictionary<double, Dictionary<char, decimal>[]> input)
        {
        
            //decimal x = input[12.0][0]['X'];
            double total = 0; //returned variable
            for(int i=0; i < fileX.Length; i++)
            {
                //The distance fomula is Square Root((x2 - x1)^2 - (y2 -y1)^2)
                //distY is the x2 - x1, distY is the y2 - y1
                double distX = fileX[i] - fieldX; double distY = fieldY - fileY[i];
                //in IDW, the weight loses strength over distance, so: (weighted value)/ (distance) 
                total += fileC[i]/(Math.Sqrt((distX * distX) - (distY * distY)));
            }
            //if we assume that beta in the IDW formula is 1 (giving the weighting a linear distribution),
            //then we can simply divide the total by how many objects are being calculated (a basic average)
            total /= fileX.Length;

            //since the value returned to the screen is a float, and Math.Sqrt outputs as a double, the value has
            //to be recast as a float when returning.
            return (float)total;
        }*/
        //----- Alex's Code Ends -----//

        
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
