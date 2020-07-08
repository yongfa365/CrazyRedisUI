using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyRedisUI
{
    //from https://stackoverflow.com/a/34141065
    class TreeBuilder
    {
        public int Index { get; set; }
        public int Depth { get; set; }
        public string Text { get; set; }
        public string FullText { get; set; }
        public Dictionary<string, TreeBuilder> Children { get; set; }

        public static void FillTreeView(List<string> list, TreeView treeView1)
        {
            var root = new TreeBuilder();
            TreeBuilder son;
            root.Depth = 0;
            root.Index = 0;
            root.Text = "root";
            root.Children = new Dictionary<string, TreeBuilder>();

            foreach (string str in list)
            {
                string[] seperated = str.Split(':');
                son = root;
                int index = 0;
                for (int depth = 0; depth < seperated.Length; depth++)
                {
                    if (son.Children.ContainsKey(seperated[depth]))
                    {
                        son = son.Children[seperated[depth]];
                    }
                    else
                    {
                        son.Children.Add(seperated[depth], new TreeBuilder());
                        son = son.Children[seperated[depth]];
                        son.Index = ++index;
                        son.Depth = depth + 1;
                        son.Text = seperated[depth];
                        if (depth == seperated.Length - 1)
                        {
                            son.FullText = str;
                        }
                        son.Children = new Dictionary<string, TreeBuilder>();
                    }
                }
            }



            treeView1.Nodes.Clear();

            AddToTreeVeiw(treeView1.Nodes, root);
            Expend(treeView1.Nodes);
        }
        public static void AddToTreeVeiw(TreeNodeCollection root, TreeBuilder tb)
        {
            foreach (string key in tb.Children.Keys)
            {
                TreeNode t = root.Add(tb.Children[key].FullText ?? tb.Children[key].Text);
                AddToTreeVeiw(t.Nodes, tb.Children[key]);
            }
        }

        public static void Expend(TreeNodeCollection nodes)
        {
            if (nodes.Count == 1)
            {
                nodes[0].Expand();
                Expend(nodes[0].Nodes);
            }

        }
    }
}
