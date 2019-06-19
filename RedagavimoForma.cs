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
    public partial class RedagavimoForma : Form
    {
        Action GriztiAction;
        

        public RedagavimoForma(string pavadinimas, Action grizti)
        {
            InitializeComponent();
            GriztiAction = grizti;

            lblIrenginioPavadinimas.Text = pavadinimas;
            UzpildytiProceduruList();
        }

        public void UzpildytiProceduruList()
        {
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                var querry = @"SELECT pavadinimas FROM " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(lblIrenginioPavadinimas.Text) + ";";

                using (SqlCommand command = new SqlCommand(querry, conn))
                {
                    conn.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        listProceduros.Items.Add(reader[0].ToString());
                    }

                    conn.Close();
                }

            }
        }

        private void btnIssaugotiPocedura_Click(object sender, EventArgs e)
        {
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                var querry = @"UPDATE " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(lblIrenginioPavadinimas.Text) +
                    " SET pavadinimas='" + txtKeistiPavadinima.Text + "', aprasymas='" + txtKeistiAprasyma.Text + "', periodiskumas='" + listPeriodiškumas.SelectedItem.ToString() + "' WHERE pavadinimas='" + listProceduros.SelectedItem.ToString() + "';";

                using (SqlCommand command = new SqlCommand(querry, conn))
                {
                    conn.Open();

                    command.ExecuteNonQuery();

                    conn.Close();
                }

                MessageBox.Show("Išsaugota sėkmingai!", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAtgal_Click(object sender, EventArgs e)
        {
            GriztiAction();
        }

        private void listProceduros_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    var querry = @"SELECT pavadinimas, aprasymas, periodiskumas FROM " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(lblIrenginioPavadinimas.Text) + " WHERE pavadinimas='" + listProceduros.SelectedItem.ToString() + "';";

                    using (SqlCommand command = new SqlCommand(querry, conn))
                    {
                        conn.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            txtKeistiPavadinima.Text = reader[0].ToString();
                            txtKeistiAprasyma.Text = reader[1].ToString();
                            listPeriodiškumas.SelectedIndex = Convert.ToInt32(reader[2]) + 1;
                        }

                        conn.Close();
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Įvyko klaida nuskaitant procedūras.", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIstrintiIrengini_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ištrinsite šį įrenginį ir prarasite visas jo procedūras bei duomenis. Ar norite tęsti?", "Įspėjimas", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        var querry = @"DROP TABLE " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(lblIrenginioPavadinimas.Text) + ";";

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
                    MessageBox.Show(ex.ToString(), "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        var querry = @"DELETE FROM Masinos WHERE MasinosPavadinimas='" + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(lblIrenginioPavadinimas.Text) + "';";

                        using (SqlCommand command = new SqlCommand(querry, conn))
                        {
                            conn.Open();

                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Įrenginys ištrintas.", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    GriztiAction();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnPridetiProcedura_Click(object sender, EventArgs e)
        {
            
        }
    }
}
