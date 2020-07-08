using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyRedisUI
{
    public partial class FrmShowHashValue : Form
    {
        public string Key { get; set; }
        public string Field { get; set; }

        public RedisKey RedisKey { get; set; }
        public RedisValue RedisValue { get; set; }
        public IDatabase db { get; set; }
        public FrmShowHashValue()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                var result = "";
                //            Plain Text
                //JSON
                //lz4
                //Snappy
                //gzip
                //base64
                if (comboBox1.Text == "lz4")
                {
                    //var inputBytes = (byte[])RedisValue;
                    //var outputBytes = LZ4.LZ4Codec.Decode(inputBytes, 0, inputBytes.Length - 1, inputBytes.Length * 20);
                    //result = Encoding.UTF8.GetString(outputBytes);
                }
                else if (comboBox1.Text == "Plain Text")
                {
                    result = RedisValue.ToString();
                }
                else if (comboBox1.Text == "JSON")
                {
                    result = RedisValue.ToString().JsonFormat();
                }

                richTextBox1.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }

        private void FrmShowHashValue_Load(object sender, EventArgs e)
        {
            this.Text = Key + "            ESC可以关闭此窗口";
            RedisKey = new RedisKey(Key);
            db = RedisHelper.Redis.GetDatabase();
            RedisValue = db.HashGet(RedisKey, new RedisValue(Field));
            comboBox1.SelectedItem = "JSON";
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
