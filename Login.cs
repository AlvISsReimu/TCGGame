using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Security;

namespace TCGGame
{
    public partial class Login : Form
    {
        private SqlConnection objSqlConnection = null;
        private string sql = null;
        private Form1 form1;

        public Login(Form1 form1)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            tabControl1.Left = 0;
            tabControl1.Top = 0;
            tabControl1.Width = this.Width;
            tabControl1.Height = this.Height;
            this.form1 = form1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sID = textBox1.Text;
            string sPassword = textBox2.Text;
            string sNewPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sPassword, "MD5").ToLower();
            objSqlConnection = Form1.GetSqlConnection();
            sql = "SELECT User_ID FROM tbl_User WHERE User_Name = '" + sID + "' AND User_Password = '" + sNewPassword + "';";
            SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
            DataSet objDataSet = new DataSet();
            SqlDataAdapter objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.Fill(objDataSet, "DataSet");
            SqlDataReader objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
            {
                int nID = Convert.ToInt32(objSqlReader.GetValue(0));
                objSqlReader.Close();
                SuccessLogin(nID);
            }
            else
            {
                MessageBox.Show("用户名或密码错误，请重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                objSqlReader.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sID = textBox3.Text;
            string sPassword = textBox4.Text;
            string sPasswordAgain = textBox5.Text;
            if (!CheckReg(sID, sPassword, sPasswordAgain))
                return ;
            objSqlConnection = Form1.GetSqlConnection();
            sql = "SELECT User_ID FROM tbl_User WHERE User_Name = '" + sID + "';";
            SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
            DataSet objDataSet = new DataSet();
            SqlDataAdapter objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.Fill(objDataSet, "DataSet");
            SqlDataReader objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
            {
                MessageBox.Show("用户名" + sID + "已被注册，请重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "";
                textBox3.Focus();
                objSqlReader.Close();
            }
            else
            {
                string sNewPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sPassword, "MD5").ToLower();
                objSqlReader.Close();
                sql = "insert into tbl_User(User_Name, User_Password) values('" + sID + "', '" + sNewPassword + "');";
                objSqlCommand = new SqlCommand(sql, objSqlConnection);
                try
                {
                    objSqlCommand.ExecuteNonQuery();
                    MessageBox.Show("注册成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    tabPage1.Show();
                    tabControl1.SelectedIndex = 0;
                    textBox1.Text = sID;
                    textBox2.Focus();
                }
                catch (Exception a)
                {
                    MessageBox.Show(a.ToString());
                }
            }
        }

        private bool CheckReg(string sID, string sPassword, string sPasswordAgain)
        {
            if (sID == "")
            {
                MessageBox.Show("请填写用户名。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return false;
            }
            else if (sPassword == "")
            {
                MessageBox.Show("请填写密码。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
                return false;
            }
            else if (sPasswordAgain == "")
            {
                MessageBox.Show("请再次输入密码。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Focus();
                return false;
            }
            else if (sPassword != sPasswordAgain)
            {
                MessageBox.Show("两次输入密码不一致，请重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Text = "";
                textBox5.Text = "";
                textBox4.Focus();
                return false;
            }
            else if (sPassword.Length<6)
            {
                MessageBox.Show("密码过短，请设置6~16位密码。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Text = "";
                textBox5.Text = "";
                textBox4.Focus();
                return false;
            }
            return true;
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                this.AcceptButton = button1;
                this.CancelButton = button2;
            }
            if (this.tabControl1.SelectedIndex == 1)
            {
                this.AcceptButton = button3;
                this.CancelButton = button4;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SuccessLogin(107);
        }

        private void SuccessLogin(int nID)
        {
            this.Hide();
            this.form1.Show();
            this.form1.ShowInTaskbar = true;
            this.form1.Focus();
            this.form1.InitData(nID);
            this.form1.PassDay();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            button5.Visible = false;
        }

    }
}