using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetMazoto.Class
{
    class Learning
    {
        //Fonction d'apprentissage
        public ActivationNetwork Trainig(DataTable data, double alpha, double seuil)
        {

            //Recuperation des donnees du tableau d'excel (entree et sortie)
            double[][] inputs = data.ToJagged<double>("SepalLength","SepalWidth","PetalLength","PetalWidth");
            double[][] outputs = data.ToJagged<double>("Setosa", "Versicolor","Virginica") ;
            //Donnee de prediction
            double[][] predicted = new double[outputs.Length][];

            bool stop = false;

            //Creation du reseau de neurone
             ActivationNetwork network = new ActivationNetwork(
             new SigmoidFunction(alpha),
             4, //nbr d'entre
             3, //nbr neurone 
             2, //nbr neurone 
             3); //nbr neurone 

            BackPropagationLearning teacher = new BackPropagationLearning(network);
            
            
            //Les fichiers de sauvegarde
            StreamWriter errorsFile = null;
            StreamWriter weightsFile = null;

            int iterationaff = 0;
            Double errorLimit = seuil;
            double error = 0;
            bool mande = true;
            try
            {

                
                // loop
                while (!stop)
                {

                    
                    // run epoch of learning procedure
                    error = teacher.RunEpoch(inputs, outputs);
                    


                    if (error < errorLimit)
                    {
                        stop = true;
                    }

                    if (iterationaff > 30000)
                    {
                        MessageBox.Show("Apprentissage echoué trop d'itération, veuillez recommencer");
                        stop =true;
                        mande = false;
                    }
                    iterationaff++;
                    
                }

                if (mande) {
                    MessageBox.Show("Learning Reussie avec " + iterationaff + " iteration et " + error + " de seuil d'erreur");
                }
                
            }
            catch (IOException)
            {
                MessageBox.Show("Failed writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // close files
                if (errorsFile != null)
                    errorsFile.Close();
                if (weightsFile != null)
                    weightsFile.Close();
            }
            return network;
        }

        

        public int ConvertFormatOutput(Double[] output)
        {
            int val=0;
            for(int i = 0; i < output.Length; i++)
            {
                if (Convert.ToInt32(output[i]) == 1)
                {
                    val = i;
                }
                else
                {
                    continue;
                }
            }
            return val;
        }

        public String ConvertIntToString(int i) {
            if (i == 0)
            {
                return "Setosa";
            }
            else if (i == 1)
            {
                return "Versicolor";
            }
            else return "Virginica";
        }

         


       

    }
}
