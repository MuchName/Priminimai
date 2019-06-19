using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;


namespace Priminimai
{
    public partial class Form1 : Form
    {
        public string Usermail { get; set; }
        public bool laiskasIssiustas = false;

        public Form1()
        {            
            InitializeComponent();
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;

            LogInForm form = new LogInForm(IsjungtiLoginoForma);
            form.TopLevel = false;
            form.AutoSize = false;
            panel1.Controls.Add(form);
            
            form.Show();

            Task.Factory.StartNew(() => { timer1.Start(); });
            
        }

        public void IsjungtiLoginoForma()
        {            

            this.MaximizeBox = true;
            panel1.Controls.Clear();
            ProgramForm form = new ProgramForm(formosKeitimas, Redaguoti)
            {
                Dock = DockStyle.Fill
            };
            form.TopLevel = false;
            form.AutoSize = false;

            panel1.Controls.Add(form);
            form.Show();
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
            
        }               
                

        public void GriztiIsMasinosPavadinimas()
        {
            ProgramForm form = new ProgramForm(formosKeitimas, Redaguoti)
            {
                Dock = DockStyle.Fill
            };
            form.TopLevel = false;
            form.AutoSize = false;

            panel1.Controls.Clear();
            panel1.Controls.Add(form);
            form.Show();
        }

        public void EitiIRegistracijosForma(string masPavadinimas)
        {
            NewMachine form2 = new NewMachine(griztiPoRegitracijos, masPavadinimas)
            {
                Dock = DockStyle.Fill
            };
            form2.TopLevel = false;
            form2.AutoSize = false;

            panel1.Controls.Clear();
            panel1.Controls.Add(form2);
            form2.Show();
        }

        public void Redaguoti(string pavadinimas)
        {
            panel1.Controls.Clear();
            RedagavimoForma form = new RedagavimoForma(pavadinimas, griztiPoRegitracijos)
            {
                Dock = DockStyle.Fill
            };
            form.TopLevel = false;
            form.AutoSize = false;

            panel1.Controls.Add(form);
            form.Show(); 
        }
        

        public void formosKeitimas()
        {
            MasinosPavadinimas form = new MasinosPavadinimas(EitiIRegistracijosForma, GriztiIsMasinosPavadinimas)
            {
                Dock = DockStyle.Fill
            };
            form.TopLevel = false;
            form.AutoSize = false;

            panel1.Controls.Clear();
            panel1.Controls.Add(form);
            form.Show();
            
        }

        public void griztiPoRegitracijos()
        {
            ProgramForm form = new ProgramForm(formosKeitimas, Redaguoti)
            {
                Dock = DockStyle.Fill
            };
            form.TopLevel = false;
            form.AutoSize = false;

            panel1.Controls.Clear();
            panel1.Controls.Add(form);
            form.Show();
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

            string darbai = "Sios savaites darbai: \n";
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

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && laiskasIssiustas == false)
            {
                Task.Factory.StartNew(() => { PriminimuBuilderis(); });                
            }
            else
            {
                laiskasIssiustas = true;
            }

                        
        }
    }
}
