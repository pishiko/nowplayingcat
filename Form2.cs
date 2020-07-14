using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordRPCTool
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Icon = Resource.rpclogo;

            var settings = Properties.Settings.Default;

            textboxAPIKey.Text = settings.LastFmAPIKey;
            textboxUserName.Text = settings.LastFmUserName;
            comboBoxCatImage.SelectedIndex = settings.CatImageIndex;
            checkBoxStartUp.Checked = settings.IsStartUp;

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var settings = Properties.Settings.Default;
            var isStartUpBefore = settings.IsStartUp;

            settings.IsStartUp = checkBoxStartUp.Checked;
            settings.LastFmAPIKey = textboxAPIKey.Text;
            settings.LastFmUserName = textboxUserName.Text;
            settings.CatImageIndex = comboBoxCatImage.SelectedIndex;

            settings.Save();
            if (isStartUpBefore != settings.IsStartUp)
            {
                if (settings.IsStartUp)
                {
                    Form1.SetStartUp();
                }
                else
                {
                    Form1.RemoveStartUp();
                }
            }
            

            Application.Restart();
        }
    }
}
