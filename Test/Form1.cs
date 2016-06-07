using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using ServiceLib;
using KellService;
using ServiceLib.DAL;
using ServiceLib.BLL;

namespace Test
{
    public partial class Form1 : Form, ICallback
    {
        public Form1()
        {
            InitializeComponent();
        }

        ITest proxy;
        IDualTest proxyDual;
        IDataBase proxyDAL;
        ICustomBusiness proxyBLLcustom;
        ICommonBusiness proxyBLL;

        private void button1_Click(object sender, EventArgs e)
        {
            if (proxyBLL != null)
            {
                try
                {
                    double sum = proxyBLL.Add(double.Parse(textBox1.Text.Trim()), double.Parse(textBox2.Text.Trim()), double.Parse(textBox7.Text.Trim()));
                    MessageBox.Show(sum.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先创建代理，或者当前配置与服务器配置不一致造成创建代理失败，又或者当前代理通道出错无法进行通讯！");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (proxyDual != null)
                {
                    try
                    {
                        proxyDual.Say(textBox3.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请先创建代理，或者当前配置与服务器配置不一致造成创建代理失败，又或者当前代理通道出错无法进行通讯！");
                }
            }
            else
            {
                if (proxy != null)
                {
                    try
                    {
                        proxy.Say(textBox3.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请先创建代理，或者当前配置与服务器配置不一致造成创建代理失败，又或者当前代理通道出错无法进行通讯！");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                IContextChannel chn = ClientProxy<ITest>.GetProxyChannel(proxy);
                if (chn != null)
                    chn.Close();
                chn = ClientProxy<IDualTest>.GetProxyChannel(proxyDual);
                if (chn != null)
                    chn.Close();
                chn = ClientProxy<IDataBase>.GetProxyChannel(proxyDAL);
                if (chn != null)
                    chn.Close();
                chn = ClientProxy<ICommonBusiness>.GetProxyChannel(proxyBLL);
                if (chn != null)
                    chn.Close();
                chn = ClientProxy<ICustomBusiness>.GetProxyChannel(proxyBLLcustom);
                if (chn != null)
                    chn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("关闭客户端通道时出现异常：" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                    proxyDual = ClientProxy<IDualTest>.CreateProxy(textBox4.Text.Trim(), true, new InstanceContext(this));
                else
                    proxy = ClientProxy<ITest>.CreateProxy(textBox4.Text.Trim(), false);
                if (checkBox2.Checked)
                    proxyBLL = ClientProxy<ICommonBusiness>.CreateProxy(textBox11.Text.Trim(), true, new InstanceContext(this));
                else
                    proxyBLL = ClientProxy<ICommonBusiness>.CreateProxy(textBox11.Text.Trim(), false);
                if (checkBox3.Checked)
                    proxyDAL = ClientProxy<IDataBase>.CreateProxy(textBox12.Text.Trim(), true, new InstanceContext(this));
                else
                    proxyDAL = ClientProxy<IDataBase>.CreateProxy(textBox12.Text.Trim(), false);
                if (checkBox4.Checked)
                    proxyBLLcustom = ClientProxy<ICustomBusiness>.CreateProxy(textBox13.Text.Trim(), true, new InstanceContext(this));
                else
                    proxyBLLcustom = ClientProxy<ICustomBusiness>.CreateProxy(textBox13.Text.Trim(), false);

                MessageBox.Show("客户端代理创建成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建代理时出现异常：" + ex.Message);
            }
        }

        public void Invoke(params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                MessageBox.Show("收到来自服务器的回复：" + args[0].ToString());
            }
            else
            {
                MessageBox.Show("收到来自服务器的回复！");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (proxyDAL != null)
            {
                try
                {
                    string table = "Class";
                    DataTable dt = proxyDAL.GetDataTable("select * from " + table, table);
                    if (dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("表" + table + "中无任何数据！");
                    }
                    table = "Student";
                    DataTable dt2 = proxyDAL.GetDataTable("select * from " + table, table);
                    if (dt2.Rows.Count > 0)
                    {
                        dataGridView2.DataSource = dt2.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("表" + table + "中无任何数据！");
                    }
                    treeView1.Nodes.Clear();
                    LoadClass(dt, dt2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("获取数据失败：" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (proxyBLLcustom != null)
            {
                try
                {
                    List<string> input = new List<string>();
                    string inputStr = textBox10.Text.Trim();
                    string[] ss = inputStr.Split(',');
                    foreach (string s in ss)
                    {
                        input.Add(s);
                    }
                    object[] args = new object[1];
                    args[0] = input;
                    ResultObject result = null;
                    if (proxyBLLcustom.CompileAndInvoke(textBox5.Text.Trim(), textBox9.Text.Trim(), textBox8.Text.Trim(), args, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, out result))
                    {
                        if (result != null)
                        {
                            List<string> resultStr = result.Result as List<string>;
                            if (resultStr != null)
                            {
                                textBox6.Text = string.Join(",", resultStr);
                                MessageBox.Show("执行成功，得出正确结果！");
                            }
                            else
                            {
                                MessageBox.Show("执行成功，但结果有误！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("执行成功，无结果输出！");
                        }
                    }
                    else
                    {
                        if (result == null) result = new ResultObject("[无错误信息]");
                        MessageBox.Show(result.Result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + "可能是参数格式有问题！");
                }
            }
        }

        private void LoadClass(DataTable dt, DataTable dt2)
        {
            if (dt != null)
            {
                DataView dv = new DataView(dt);
                if (dv.Count > 0)
                {
                    dv.Sort = "gradeCount,className";
                    string lastGrade = "";
                    int lastNodeIndex = -1;
                    for (int i = 0; i < dv.Count; i++)
                    {
                        string clas = dv[i]["className"].ToString();
                        string curGrade = dv[i]["gradeCount"].ToString();
                        if (curGrade != lastGrade)
                        {
                            TreeNode nod = new TreeNode(curGrade + "年级");
                            nod.Name = "GRD_" + curGrade;

                            TreeNode[] nodes = treeView1.Nodes.Find("GRD_" + curGrade, false);
                            if (nodes == null || nodes.Length == 0)
                            {
                                lastNodeIndex = treeView1.Nodes.Add(nod);
                            }
                            if (lastNodeIndex > -1)
                            {
                                string key = "CLS_" + dv[i]["id"].ToString();
                                TreeNode nod2 = new TreeNode(clas + "班");
                                nod2.Name = key;
                                treeView1.Nodes[lastNodeIndex].Nodes.Add(nod2);
                                LoadStudent(dt2, key);
                            }
                        }
                        else
                        {
                            if (lastNodeIndex > -1)
                            {
                                string key = "CLS_" + dv[i]["id"].ToString();
                                TreeNode nod = new TreeNode(clas + "班");
                                nod.Name = key;
                                treeView1.Nodes[lastNodeIndex].Nodes.Add(nod);
                                LoadStudent(dt2, key);
                            }
                        }
                    }
                }
                treeView1.ExpandAll();
            }
        }

        private void LoadStudent(DataTable dt2, string key)
        {
            if (dt2 != null)
            {
                DataView dv = new DataView(dt2);
                if (dv.Count > 0)
                {
                    dv.Sort = "classId";
                    for (int i = 0; i < dv.Count; i++)
                    {
                        string id = dv[i]["id"].ToString();
                        string text = dv[i]["name"].ToString();
                        string cls = dv[i]["classId"].ToString();
                        if ("CLS_" + cls == key)
                        {
                            TreeNode[] nodes = treeView1.Nodes.Find(key, true);
                            foreach (TreeNode node in nodes)
                            {
                                TreeNode nod = new TreeNode(text);
                                nod.Name = "STD_" + id;
                                node.Nodes.Add(nod);
                            }
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataView dv = dataGridView1.DataSource as DataView;
            string cls_id = dv[e.RowIndex][0].ToString();
            TreeNode[] nodes = treeView1.Nodes.Find("CLS_" + cls_id, true);
            if (nodes != null && nodes.Length > 0)
            {
                TreeNode node = nodes[0];
                foreach (TreeNode n in treeView1.Nodes)
                {
                    foreach (TreeNode ne in n.Nodes)
                    {
                        if (ne != node)
                        {
                            ne.BackColor = Color.Transparent;
                        }
                    }
                }
                node.BackColor = SystemColors.Highlight;
            }
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataView dv = dataGridView2.DataSource as DataView;
            string std_id = dv[e.RowIndex][0].ToString();
            TreeNode[] nodes = treeView1.Nodes.Find("STD_" + std_id, true);
            if (nodes != null && nodes.Length > 0)
            {
                TreeNode node = nodes[0];
                foreach (TreeNode n in treeView1.Nodes)
                {
                    foreach (TreeNode ne in n.Nodes)
                    {
                        foreach (TreeNode nr in ne.Nodes)
                        {
                            if (nr != node)
                            {
                                nr.BackColor = Color.Transparent;
                            }
                        }
                    }
                }
                node.BackColor = SystemColors.Highlight;
            }
        }
    }
}