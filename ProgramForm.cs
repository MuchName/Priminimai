using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using System.Net.Mail;
using MailMessage = System.Net.Mail.MailMessage;

namespace Priminimai
{
    public partial class ProgramForm : Form
    {
        Action KeiciamForma;
        Action<string> Redaguoti;
        Action<string> Profilis;
        
        System.Data.DataTable dt = new System.Data.DataTable();
        List<string> masinuList = new List<string>();
        

        public ProgramForm(Action keiciamForma, Action<string> redaguoti)
        {
            InitializeComponent();
            label3.Text = MetuSavaitesNr() + "Savaitė";

            KeiciamForma = keiciamForma;
            Redaguoti = redaguoti;

            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
            string querry = @"SELECT (MasinosPavadinimas) FROM Masinos;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand command = new SqlCommand(querry, conn))
                    {
                        conn.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            masinuList.Add(MasinosPavadinimoStringKeitimas.BruksnelisTarpas(reader[0].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            

            foreach (var item in masinuList)
            {
                comboBox1.Items.Add(item);
            }
            
            
            dt.Columns.Add("Techninės operacijos aprašymas");
            //dt.PrimaryKey = new DataColumn[] { dt.Columns["Techninės operacijos aprašymas"] };

            for (int i = 1; i < 53; i++)
            {
                dt.Columns.Add(i.ToString());
            }
                    
            dataGridView1.DataSource = dt;            
            dataGridView1.AutoSize = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns["Techninės operacijos aprašymas"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;


        }


        public int MetuSavaitesNr()
        {
            CultureInfo myCult = new CultureInfo("lt");
            Calendar calendar = myCult.Calendar;
            CalendarWeekRule myCWR = myCult.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCult.DateTimeFormat.FirstDayOfWeek;
            int savaitesSk = calendar.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
            return savaitesSk;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString();
        }
        
        //Combobox itemo keitimas
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt.Rows.Clear();
            lblProcAprasymas.Text = "----";
            lblProcAprPilnas.Text = "----";            

            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                
                var querry = @"SELECT pavadinimas, periodiskumas FROM " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(comboBox1.SelectedItem.ToString()) + ";";

                try
                {
                    using (SqlCommand command = new SqlCommand(querry, conn))
                    {
                        conn.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            DataRow workRow = dt.NewRow();

                            workRow["Techninės operacijos aprašymas"] = reader[0].ToString();

                            for (int i = 1; i < 53; i++)
                            {
                                if (i % Convert.ToInt32(reader[1]) == 0)
                                {
                                    workRow[i] = "X";
                                }
                            }
                            dt.Rows.Add(workRow);
                        }

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                   
                
            }
        }        
                      

        private void btnGeneruoti_Click(object sender, EventArgs e)
        {
                try
                {
                    if (comboBox1.SelectedItem != null)
                    {
                        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel.Worksheet excelSheet = new Microsoft.Office.Interop.Excel.Worksheet();
                        excelApp.Visible = true;
                        string path = @"D:\Test.xlsx";
                        excelSheet = excelApp.Application.Workbooks.Open(path).Worksheets["Lapas1"];

                        string pakeisti = comboBox1.SelectedItem.ToString();
                        excelSheet.Cells.Replace("MASINOSPAVADINIMAS", pakeisti);

                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            excelSheet.Cells[6, i + 1] = dataGridView1.Columns[i].HeaderText;
                        }



                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                if (dataGridView1[j, i].ValueType == typeof(string))
                                {
                                    excelSheet.Cells[i + 7, j + 1] = "'" + dataGridView1[j, i].Value.ToString();

                                }
                                else
                                {
                                    excelSheet.Cells[i + 7, j + 1] = dataGridView1[j, i].Value.ToString();
                                }
                            }
                        }

                        int fsba = dataGridView1.RowCount + 5;

                        var excelSheetRange = excelSheet.get_Range("A6:BA" + fsba.ToString());
                        excelSheetRange.Borders.Color = System.Drawing.Color.Black.ToArgb();
                        excelSheetRange.Columns.AutoFit();                   

                }
                    else
                    {
                        MessageBox.Show("Pasirinkite įrenginį iš sąrašo.", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }                      
            
            
        }

        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lblProcAprasymas.Text = dataGridView1.SelectedCells[0].Value.ToString();

            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";

            

            using (SqlConnection conn = new SqlConnection(connString))
            {

                var querry = @"SELECT aprasymas FROM " + MasinosPavadinimoStringKeitimas.TarpasBruksnelis(comboBox1.SelectedItem.ToString()) + " WHERE pavadinimas='" + lblProcAprasymas.Text + "';";

                using (SqlCommand command = new SqlCommand(querry, conn))
                {
                    conn.Open();

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == string.Empty)
                        {
                            lblProcAprPilnas.Text = "Aprašymo nėra. Pridėkite aprašymą redaguodami įrenginio techninį aprašymą.";
                        }
                        else
                        {
                            lblProcAprPilnas.Text = reader[0].ToString();
                        }
                    }                
                    
                    conn.Close();
                }
            }
        }

        private void btnRedaguoti_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                Redaguoti(comboBox1.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Pasirinkite įrenginį iš sąrašo.", "Pranešimas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void btnNaujasIrenginys_Click_1(object sender, EventArgs e)
        {
            KeiciamForma();
        }
       

        public List<string> VartotojuEmail()
        {
            List<string> mailai = new List<string>();
            string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
            string querry2 = "SELECT email FROM vartotojai;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand command = new SqlCommand(querry2, conn))
                    {
                        conn.Open();

                        var readeris = command.ExecuteReader();

                        while (readeris.Read())
                        {
                            mailai.Add(readeris[0].ToString());
                        }

                        conn.Close();
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problema gaunant e-mail sąrašą", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return mailai;
        }

        public void PriminimuBuilderis()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                string darbai = "Ateinančios savaites darbai: \n";
                List<string> masinuList = new List<string>();
                string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
                string querry = @"SELECT MasinosPavadinimas FROM Masinos;";
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand command = new SqlCommand(querry, conn))
                        {
                            conn.Open();

                            var reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                masinuList.Add(reader[0].ToString());
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Įvyko klaida nuskaitant įrenginių sąrašą", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    foreach (var masina in masinuList)
                    {
                        string querry2 = "SELECT pavadinimas, periodiskumas FROM " + masina.ToString() + ";";
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            using (SqlCommand command = new SqlCommand(querry2, conn))
                            {
                                conn.Open();

                                var readeris = command.ExecuteReader();

                                while (readeris.Read())
                                {
                                    if ((MetuSavaitesNr() + 1) % (Convert.ToInt32(readeris[1]) + 1) == 0)
                                    {
                                        darbai += "- " + masina.ToString() + " - " + readeris[0].ToString() + ";\n";
                                    }
                                }
                                conn.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

                try
                {
                    using (MailMessage emailMessage = new MailMessage())
                    {
                        foreach (var item in VartotojuEmail())
                        {
                            emailMessage.From = new MailAddress("testinismail1@gmail.com");
                            emailMessage.To.Add(new MailAddress(item.ToString()));
                            emailMessage.Subject = "Savaites darbai";
                            emailMessage.Body = darbai;
                            emailMessage.Priority = MailPriority.Normal;
                            using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587))
                            {
                                MailClient.EnableSsl = true;
                                MailClient.Credentials = new System.Net.NetworkCredential("testinismail1@gmail.com", "testinis123");
                                MailClient.Send(emailMessage);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Įvyko klaida siunčiant e-mail.", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            });

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => { PriminimuBuilderis(); });
        }
    }
}
