﻿using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyRedisUI
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            new FrmInputConnectionStrings().ShowDialog();
            FillTree();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "JSON";
            //            丢个ImageList控件到窗体上， 设置TreeView的ImageList属性，

            //然后设置TreeNode

            //eg:

            ////TreeNode node = new TreeNode();
            ////node.ImageIndex = ....
            ////node.SelectedImageIndex =...
            ///

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            new FrmInputConnectionStrings().ShowDialog();
            FillTree();
        }

        public void FillTree()
        {
            var input = txtFilter.Text;
            var query = RedisHelper.Keys.AsQueryable();
            if (!string.IsNullOrEmpty(input))
            {
                query = query.Where(p => p.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1);
            }
            var list = query.OrderBy(p => p).ToList();

            TreeBuilder.FillTreeView(list, treeView1);

        }





        public DateTime? NextFilterTime { get; set; }
        public bool IsFiltering { get; set; }
        private readonly object objLock = new object();

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            var old = NextFilterTime;
            NextFilterTime = DateTime.Now.AddMilliseconds(800);

            lock (objLock)
            {
                if (!IsFiltering)
                {
                    IsFiltering = true;
                    Task t = Task.Run(async () =>
                    {
                        while (DateTime.Now < NextFilterTime)
                        {
                            await Task.Delay(805);
                        }

                    }).ContinueWith(m =>
                    {
                        FillTree();
                        IsFiltering = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }




        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                return;
            }

            var key = e.Node.Text;
            var db = RedisHelper.Redis.GetDatabase(0);
            var redisKey = new RedisKey(key);
            var keyType = db.KeyType(new RedisKey(key));
            lblType.Text = keyType.ToString();
            txtKey.Text = key;
            txtTTL.Text = db.KeyTimeToLive(redisKey)?.ToString();

            switch (keyType)
            {
                case RedisType.None:
                    break;
                case RedisType.String:
                    RenderString(db, redisKey);
                    break;
                case RedisType.List:
                    break;
                case RedisType.Set:
                    break;
                case RedisType.SortedSet:
                    break;
                case RedisType.Hash:
                    RenderHash(db, redisKey);
                    break;
                case RedisType.Stream:
                    break;
                case RedisType.Unknown:
                    break;
                default:
                    break;
            }

        }
        private void RenderString(IDatabase db, RedisKey redisKey)
        {
            txtValue.Visible = true;
            dataGridView1.Visible = false;
            try
            {
                txtValue.Text = db.StringGet(redisKey).ToString().JsonFormat();
            }
            catch
            {
                txtValue.Text = db.StringGet(redisKey).ToString();
            }
        }
        private void RenderHash(IDatabase db, RedisKey redisKey)
        {
            txtValue.Visible = false;
            dataGridView1.Visible = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (var item in db.HashGetAll(redisKey).OrderBy(p => p.Name.ToString()))
            {
                dataGridView1.Rows.Add(item.Name.ToString(), item.Value.ToString());
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            new FrmShowHashValue
            {
                Key = txtKey.Text,
                Field = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(),
            }.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var key = treeView1.SelectedNode.Text;
            if (key.Length < 3)
            {
                MessageBox.Show("deny delete:" + key);
                return;
            }
            var result = RedisHelper.Redis.GetDatabase(0).KeyDelete(new RedisKey(key));
            if (result)
            {
                var nextNode = treeView1.SelectedNode.NextNode;
                treeView1.SelectedNode.Remove();
                treeView1.SelectedNode = nextNode;

                RedisHelper.Keys.TryTake(out key);

            }
            else
            {
                MessageBox.Show("delete fail：" + key);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabel1.Text);
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode.ExpandAll();
        }

        private void btnCopyKeys_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Join("\r\n", RedisHelper.Keys.OrderBy(p => p)));
        }
    }
}
