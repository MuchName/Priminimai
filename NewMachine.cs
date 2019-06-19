using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Priminimai
{
    public partial class NewMachine : Form
    {
        Action GriztiPoRegistracijos;
        List<Procedura> proceduruList = new List<Procedura>();
        string MasinosPavadinimas;

        public NewMachine(Action grizti, string pavadinimas)
        {
            InitializeComponent();
            GriztiPoRegistracijos = grizti;
            MasinosPavadinimas = pavadinimas;
            lblIrengPavIsKitosFormos.Text = MasinosPavadinimoStringKeitimas.BruksnelisTarpas(MasinosPavadinimas);
        }
        public NewMachine(Action grizti)
        {
            InitializeComponent();
            GriztiPoRegistracijos = grizti;
        }
        

        private void btnPridetiIrengini_Click(object sender, EventArgs e)
        {
            Procedura procedura = new Procedura();            
           
            procedura.ProcedurosPavadinimas = txtPavadinimas.Text;
            procedura.Aprasymas = txtAprasymas.Text;
            procedura.SavaiciuSkaicius = (int)listPeriodiskumas.SelectedIndex + 1;

            proceduruList.Add(procedura);

            var listViewItem = new ListViewItem(procedura.ProcedurosPavadinimas);
            listViewProceduros.Items.Add(listViewItem);
            txtAprasymas.Clear();
            txtPavadinimas.Clear();
            
        }


        public bool TikrintiArMasinaYra()
        {
            bool arYra = false;
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "SELECT * FROM Masinos WHERE MasinosPavadinimas='" + MasinosPavadinimas + "';";

                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    arYra = true;
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return arYra;
        }

        private void btnIssaugoti_Click(object sender, EventArgs e)
        {           
            
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
            try
            {

                if (TikrintiArMasinaYra() == true)
                {
                    DuomenuIkelimasIsenaLentele();
                }
                else if (TikrintiArMasinaYra() == false)
                {
                    try
                    {
                        using (SqlConnection conn2 = new SqlConnection(connString))
                        {
                            var querry2 = @"CREATE TABLE " + MasinosPavadinimas + " (pavadinimas varchar(MAX), aprasymas varchar(MAX), periodiskumas int);";

                            using (SqlCommand command2 = new SqlCommand(querry2, conn2))
                            {
                                conn2.Open();
                                var result = command2.ExecuteNonQuery();
                                conn2.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString() + "C");
                    }

                    DuomenuIkelimasINaujaLentele();

                    GriztiPoRegistracijos();
                }
            }

            catch (Exception)
            {

                throw;
            }         
                        
        }

        private void btnGrizti_Click(object sender, EventArgs e)
        {
            GriztiPoRegistracijos();
        }

        public void DuomenuIkelimasINaujaLentele()
        {
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";


            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    foreach (var item in proceduruList)
                    {
                        var querry = @"INSERT INTO " + MasinosPavadinimas + " (pavadinimas, aprasymas, periodiskumas) VALUES ('" + item.ProcedurosPavadinimas + "', '" + item.Aprasymas + "', '" + item.SavaiciuSkaicius + "');";

                        using (SqlCommand command = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + "A");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {

                    var querry = @"INSERT INTO Masinos (MasinosPavadinimas) VALUES ('" + MasinosPavadinimas + "');";

                    using (SqlCommand command = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + " B");
            }

            MessageBox.Show("Sėkmingai išsaugota!", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
        }

        public void DuomenuIkelimasIsenaLentele()
        {
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    foreach (var item in proceduruList)
                    {
                        var querry = @"INSERT INTO " + MasinosPavadinimas + " (pavadinimas, aprasymas, periodiskumas) VALUES ('" + item.ProcedurosPavadinimas + "', '" + item.Aprasymas + "', '" + item.SavaiciuSkaicius + "');";

                        using (SqlCommand command = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + "A");
            }

            
            MessageBox.Show("Sėkmingai išsaugota!", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
        }
    }
}
