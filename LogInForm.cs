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

namespace Priminimai
{
    public partial class LogInForm : Form
    {
        Action KeistiLoginForma;

        public LogInForm(Action keisti)
        {
            InitializeComponent();
            KeistiLoginForma = keisti;
        }

        
        private void LogInForm_Load(object sender, EventArgs e)
        {

        }

        private void registerBtn_Click_1(object sender, EventArgs e)
        {
            RegisterForm regform = new RegisterForm(GriztiPoRegistracijos);
            regform.TopLevel = false;
            regform.AutoSize = false;
            panel1.Controls.Clear();
            panel1.Controls.Add(regform);

            regform.Show();
        }

        public void GriztiPoRegistracijos()
        {            
            LogInForm form = new LogInForm(KeistiLoginForma);
            form.TopLevel = false;
            form.AutoSize = false;
            panel1.Controls.Clear();
            panel1.Controls.Add(form);
            form.Show();
        }

        private void loginBtn_Click_1(object sender, EventArgs e)
        {
            if (emailText.Text == string.Empty || passwordText.Text == string.Empty)
            {
                MessageBox.Show("Įrašykite savo e-mail adresą.");
            }
            else if (emailText.Text.Contains("*") || emailText.Text.Contains("/") ||
                emailText.Text.Contains(@"\") || emailText.Text.Contains("_") ||
                emailText.Text.Contains("?")  || emailText.Text.Contains("="))
            {
                MessageBox.Show("Neteisingas e-mail formatas.");
            }
            else
            {
                try
                {
                    string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
                    string querry = @"SELECT COUNT(email) FROM vartotojai WHERE email='" + emailText.Text + "' AND password='" + passwordText.Text + "';";
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand command = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            string email = emailText.Text;

                            if (command.ExecuteScalar().ToString() == "1")
                            {
                                KeistiLoginForma();
                            }
                            else
                            {
                                MessageBox.Show("Toks vartotojas neegzistuoja. Užsiregistruokite.", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.ToString(), "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                
            }

            
        }
    }
}
