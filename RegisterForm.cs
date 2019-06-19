using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Priminimai
{   

    public partial class RegisterForm : Form
    {
        Action GriztiPoRegistracijos;
        List<string> textBoxuSarasas = new List<string>();
        bool klaida;

        public RegisterForm()
        {
            InitializeComponent();
        }

        public RegisterForm(Action griztiPoRegistracijos)
        {
            InitializeComponent();
            GriztiPoRegistracijos = griztiPoRegistracijos;
            
        }

        public bool ArPastoAdresasTeisingas(String strToCheck)
        {
            Regex obj = new Regex(@"(.+)(@)(.+)$");
            return obj.IsMatch(strToCheck);
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            textBoxuSarasas.Add(vardasTxtBox.Text);
            textBoxuSarasas.Add(pavardeTxtBox.Text);
            textBoxuSarasas.Add(pareigosTxtBox.Text);
            textBoxuSarasas.Add(emailTxtBox.Text);
            textBoxuSarasas.Add(emailTxtBox.Text);
            
            foreach (var item in textBoxuSarasas)
            {
                klaida = false;

                if (item.Contains("*") || item.Contains("/") ||
                item.Contains(@"\") || item.Contains("_") ||
                item.Contains("?") || item.Contains("=") || item == string.Empty)
                {
                    klaida = true;
                }                
            }
            if (klaida == true)
            {
                MessageBox.Show("Teksto laukeliuose negali būti specialiųjų simbolių ir jie negali likti neužpildyti", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            if (emailTxtBox.Text == string.Empty || ArPastoAdresasTeisingas(emailTxtBox.Text) == false)
            {
                MessageBox.Show("Įveskite teisingą e-mail adresą.", "Klaida", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {

                if (slaptazodisTxtBox.Text != slaptazodisTxtBox2.Text)
                {
                    MessageBox.Show("Įvyko klaida. Patikrinkite slaptažodį.", "Klaida");
                }
                else
                {
                    string connString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marius\source\repos\Priminimai\Priminimai\PriminimuDB.mdf; Integrated Security = True";
                    string querry1 = @"INSERT INTO vartotojai (vardas, pavarde, pareigos, email, password) VALUES (@vardas, @pavarde, @pareigos, @email, @password)";
                    string querry2 = @"SELECT COUNT (*) FROM vartotojai WHERE email='" + emailTxtBox.Text + "';";
                    bool userExists = false;
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(querry2, conn))
                        {
                            if (command.ExecuteScalar().ToString() == "1")
                            {
                                MessageBox.Show("Toks el. pašto adresas jau panaudotas. Pasirinkite kitą el. pašto adresą.");
                                userExists = true;
                            }

                        }

                        if (userExists == false)
                        {
                            using (SqlCommand command = new SqlCommand(querry1, conn))
                            {
                                command.Parameters.AddWithValue("@vardas", vardasTxtBox.Text);
                                command.Parameters.AddWithValue("@pavarde", pavardeTxtBox.Text);
                                command.Parameters.AddWithValue("@pareigos", pareigosTxtBox.Text);
                                command.Parameters.AddWithValue("@email", emailTxtBox.Text);
                                command.Parameters.AddWithValue("@password", slaptazodisTxtBox.Text);

                                int result = command.ExecuteNonQuery();

                                if (result > 0)
                                {  
                                    DialogResult dialogResult = MessageBox.Show("Užsiregistravote sėkmingai! Galite prisijungti.", "Registracija", MessageBoxButtons.OK);
                                    if (dialogResult == DialogResult.OK)
                                    {
                                        GriztiPoRegistracijos();
                                    }
                                }
                            }
                        }
                    }
                }
            }



            
        }


        private void emailTxtBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GriztiPoRegistracijos();
        }
    }
}
