using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyRedisUI
{
    public partial class FrmInputConnectionStrings : Form
    {
        public static string ConnPath = "Conn\\";
        public FrmInputConnectionStrings()
        {
            Directory.CreateDirectory(ConnPath);
            InitializeComponent();
            foreach (var filename in Directory.GetFiles(ConnPath))
            {
                var name = filename.Substring(filename.LastIndexOf("\\") + 1).Split('.')[0];
                comboBox1.Items.Add(name);
            };

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                textBox1.Text = @"
127.0.0.1:6379
127.0.0.1:6380";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RedisHelper.ConnectionStrings = textBox1.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            RedisHelper.Connect();
            RedisHelper.FillKeys();
            File.WriteAllText(ConnPath + comboBox1.Text + ".txt", textBox1.Text);
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = File.ReadAllText(ConnPath + comboBox1.Text + ".txt");
        }
    }
}
