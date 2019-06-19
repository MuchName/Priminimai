using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Priminimai
{
    public partial class MasinosPavadinimas : Form
    {
        
        Action<string> TestiRegistracija;
        Action GriztiAtgal;
        

        public MasinosPavadinimas(Action<string> testi, Action griztiAtgal)
        {
            InitializeComponent();
            TestiRegistracija = testi;
            GriztiAtgal = griztiAtgal;
        }

        private void MasinosPavadinimas_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Įrašykite įrenginio pavadinimą.");
            }
            else if (textBox1.Text.Contains("*") || textBox1.Text.Contains("/") ||
                textBox1.Text.Contains(@"\") || textBox1.Text.Contains("_") ||
                textBox1.Text.Contains("?") || textBox1.Text.Contains("@") || textBox1.Text.Contains("="))
            {
                MessageBox.Show("Neteisingas įrenginio pavadinimo formatas. Venkite simbolių.");
            }
            else
            {
                TestiRegistracija(SiustiPavadinima());
            }

            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GriztiAtgal();
        }

        public string SiustiPavadinima()
        {
            return MasinosPavadinimoStringKeitimas.TarpasBruksnelis(textBox1.Text);
        }
    }
}
