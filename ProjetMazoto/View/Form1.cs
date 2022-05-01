using Accord.IO;
using Accord.Math;
using Accord.Neuro;
using Accord.Statistics.Analysis;
using ProjetMazoto.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetMazoto
{
    public partial class Form1 : Form
    {
        private string path="";
        private DataTable data;
        private Accord.Neuro.ActivationNetwork network;
        public Form1()
        {
            InitializeComponent();
           

            Alpha.Text = "1";
            Seuil.Text = "1";

            button2.Click += new EventHandler(selectButton_Click);
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            //bouton importer fichier
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog1.FileName;
                    
                    using (Stream str = openFileDialog1.OpenFile())
                    {
                        path = filePath;
                        str.Dispose();
                    }
                    
                    ExcelReader ex = new ExcelReader();
                    
                    data = ex.importExcel(path);
                    dataGridView1.DataSource = data;
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }


        
       
       

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //bouton learning
            Double alpha = Double.Parse(Alpha.Text);
            Double seuil = Double.Parse(Seuil.Text);
            Learning learn = new Learning();
            this.network = learn.Trainig(data, alpha, seuil);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //predire avec formulaire
            double SepalL = Convert.ToDouble(textBox1.Text);
            double SepalW = Convert.ToDouble(textBox2.Text);
            double PetalL = Convert.ToDouble(textBox3.Text);
            double PetalW = Convert.ToDouble(textBox4.Text);


            double[] inputs = { SepalL, SepalW, PetalL, PetalW };
            Double[] output = this.network.Compute(inputs);

            Learning l = new Learning();
            int i = l.ConvertFormatOutput(output);
            label14.Text = l.ConvertIntToString(i);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //predire tableau
            Learning l = new Learning();
            double[][] inputs = data.ToJagged<double>("SepalLength", "SepalWidth", "PetalLength", "PetalWidth");
            double[][] outputs = data.ToJagged<double>("Setosa", "Versicolor", "Virginica");

            int[] expected = new int[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                expected[i] = l.ConvertFormatOutput(outputs[i]);
            }

            int[] predicted = new int[outputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                Double[] temp = this.network.Compute(inputs[i]);
                predicted[i] = l.ConvertFormatOutput(temp);
            }

            dataGridView3.Rows.Clear();
            dataGridView3.ColumnCount = 2;
            dataGridView3.Columns[0].Name = "Predicted";
            dataGridView3.Columns[1].Name = "Excepted";

            foreach (DataGridViewRow row in dataGridView1.Rows)
                dataGridView1.Rows[row.Index].DefaultCellStyle.BackColor = Color.White;

            dataGridView2.Rows.Clear();

            for (int i = 0; i < expected.Length; i++)
            {
                String expectedString = l.ConvertIntToString(expected[i]);
                String predictedString = l.ConvertIntToString(predicted[i]);
                string[] row = new string[] {  predictedString , expectedString };

                dataGridView3.Rows.Add(row);
                
                
            }

            foreach (DataGridViewRow row in dataGridView3.Rows)
                if (row.Cells[1].Value != row.Cells[0].Value)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    dataGridView1.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red; 
                }

            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //matrice de confusion
            try
            {
                Learning l = new Learning();

                if(this.network==null)
                {
                    throw new Exception("Reseau de neurone manquant");
                }

                double[][] inputs = data.ToJagged<double>("SepalLength", "SepalWidth", "PetalLength", "PetalWidth");
                double[][] outputs = data.ToJagged<double>("Setosa", "Versicolor", "Virginica");

                int[] expected = new int[outputs.Length];
                for (int i = 0; i < outputs.Length; i++)
                {
                    expected[i] = l.ConvertFormatOutput(outputs[i]);
                }

                int[] predicted = new int[outputs.Length];
                for (int i = 0; i < inputs.Length; i++)
                {
                    Double[] temp = this.network.Compute(inputs[i]);
                    predicted[i] = l.ConvertFormatOutput(temp);
                }

                int classes = 3;

                GeneralConfusionMatrix cm2 = new GeneralConfusionMatrix(classes, expected, predicted);
                int[,] matrix = cm2.Matrix;

                Console.WriteLine("Affichage matrice de confusion:");
                for (int i = 0; i < classes; i++)
                {
                    for (int j = 0; j < classes; j++)
                    {
                        if (j != (classes - 1))
                        {
                            Console.Write(matrix[i, j] + "//");
                        }
                        else
                        {
                            Console.Write(matrix[i, j]);
                        }
                    }
                    Console.WriteLine();
                }

                Double accuracy2 = cm2.Accuracy;
                Double pourcentage2 = accuracy2 * 100;

                Console.WriteLine("accuracy2 :" + pourcentage2 + "%");

                //Affichage de la matrice de confusion
                dataGridView2.ColumnCount = 4;
                dataGridView2.Columns[0].Name = "Pred/Exp";
                dataGridView2.Columns[1].Name = "Setoza";
                dataGridView2.Columns[2].Name = "Versicolor";
                dataGridView2.Columns[3].Name = "Virginica";

                dataGridView2.Rows.Clear();
                string[] row = new string[] { "Setoza", matrix[0, 0].ToString(), matrix[0, 1].ToString(), matrix[0, 2].ToString() };
                dataGridView2.Rows.Add(row);
                row = new string[] { "Versicolor", matrix[1, 0].ToString(), matrix[1, 1].ToString(), matrix[1, 2].ToString() };
                dataGridView2.Rows.Add(row);
                row = new string[] { "Virginica", matrix[2, 0].ToString(), matrix[2, 1].ToString(), matrix[2, 2].ToString() };
                dataGridView2.Rows.Add(row);

                label15.Text = "Precision:" + pourcentage2 + "%";
            }
            catch(Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //save
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog1.FileName;
                    Console.WriteLine("Filepath:" + filePath);


                    using (FileStream stream = File.Open(filePath, FileMode.Open))
                    {
                        path = filePath;
                        this.network.Save(stream);
                        stream.Dispose();
                    }
                    MessageBox.Show("Sauvegarde Reussie");
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //load
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog1.FileName;
                    Console.WriteLine("Filepath:" + filePath);


                    using (FileStream stream = File.Open(filePath, FileMode.Open))
                    {
                        path = filePath;
                        this.network = (ActivationNetwork)Network.Load(stream);
                        stream.Dispose();
                    }
                    MessageBox.Show("Chargement Reussie");
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
