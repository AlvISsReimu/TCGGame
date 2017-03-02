using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TCGGame
{
    public partial class Manage : Form
    {
        private SqlConnection objSqlConnection = null;
        private static string sql = null;
        private static string proc = null;
        private static SqlCommand objSqlCommand = null;
        private static SqlDataAdapter objDataAdapter = null;
        private static SqlDataReader objSqlReader = null;

        public Manage()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            objSqlConnection = Form1.GetSqlConnection();
        }

        private void Manage_Load(object sender, EventArgs e)
        {
            UpdateCards();
            UpdatePacks();
        }

        private void UpdateCards()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            string sql = "SELECT Card_ID, Card_Name, Card_Rarity FROM tbl_Card ORDER BY Card_ID;";
            DataSet objDataSet = new DataSet();
            SqlDataAdapter objDataAdapter = new SqlDataAdapter(sql, objSqlConnection);
            objDataAdapter.Fill(objDataSet);
            dataGridView1.DataSource = objDataSet.Tables[0];
            dataGridView1.Columns[0].HeaderText = "ID";
            dataGridView1.Columns[1].HeaderText = "卡名";
            dataGridView1.Columns[2].HeaderText = "罕贵";
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            sql = "SELECT COUNT(*) total FROM tbl_Card;";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            objSqlReader.Read();
            label1.Text = "卡片总数：" + objSqlReader["total"].ToString();
            objSqlReader.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            } 
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            } 
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || comboBox1.Text == "" || comboBox2.Text == "")
                MessageBox.Show("不能有空项！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                SaveCard(textBox1.Text, Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), comboBox1.Text, comboBox2.Text);
                UpdateCards();
            }
        }

        private void SaveCard(string sName, int nPower, int nPrice, string sRarity, string sPackName)
        {
            proc = "prd_SaveCard";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@card_id", SqlDbType.Int), new SqlParameter("@card_name", SqlDbType.VarChar, 30), new SqlParameter("@card_power", SqlDbType.Int), new SqlParameter("@card_price", SqlDbType.Int), new SqlParameter("@card_rarity", SqlDbType.VarChar, 3), new SqlParameter("@pack_name", SqlDbType.VarChar, 4) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.Input;
            parameters[1].Value = sName;
            objSqlCommand.Parameters.Add(parameters[1]);
            parameters[2].Direction = ParameterDirection.Input;
            parameters[2].Value = nPower;
            objSqlCommand.Parameters.Add(parameters[2]);
            parameters[3].Direction = ParameterDirection.Input;
            parameters[3].Value = nPrice;
            objSqlCommand.Parameters.Add(parameters[3]);
            parameters[4].Direction = ParameterDirection.Input;
            parameters[4].Value = sRarity;
            objSqlCommand.Parameters.Add(parameters[4]);
            parameters[5].Direction = ParameterDirection.Input;
            parameters[5].Value = sPackName;
            objSqlCommand.Parameters.Add(parameters[5]);
            objSqlCommand.ExecuteNonQuery();
            MessageBox.Show("保存成功。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("真的要删除" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value.ToString() + "的" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString() + "吗？\r\n所有拥有此卡的玩家将会遭受损失。", "删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                try
                {
                    sql = "DELETE FROM tbl_Card where Card_ID = " + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString() + ";";
                    objSqlCommand = new SqlCommand(sql, objSqlConnection);
                    objSqlCommand.ExecuteNonQuery();
                    MessageBox.Show("删除成功。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    UpdateCards();
                }
                catch (Exception a)
                {
                    MessageBox.Show("删除失败。\r\n" + a.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdatePacks()
        {
            sql = "SELECT Pack_Name FROM tbl_Pack;";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            while (objSqlReader.Read())
            {
                comboBox2.Items.Add(objSqlReader[0].ToString());
            }
            objSqlReader.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string sName = "", sRarity = "", sPack = "";
            int nPower = 0, nPrice = 0, nPackID = 0; ;
            sql = "SELECT Card_Name, Card_Rarity, Card_Power, Card_Price, Card_Pack FROM tbl_Card, tbl_Pack WHERE Card_ID = " + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString() + ";";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
            {
                sName = objSqlReader.GetValue(objSqlReader.GetOrdinal("Card_Name")).ToString();
                sRarity = objSqlReader.GetValue(objSqlReader.GetOrdinal("Card_Rarity")).ToString();
                nPower = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Card_Power")));
                nPrice = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Card_Price")));
                nPackID = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Card_Pack")));
            }
            objSqlReader.Close();
            textBox1.Text = sName;
            textBox2.Text = nPower.ToString();
            textBox3.Text = nPrice.ToString();
            comboBox1.Text = sRarity;

            sql = "SELECT Pack_Name FROM tbl_Pack WHERE Pack_ID = " + nPackID + ";";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
            {
                sPack = objSqlReader.GetValue(objSqlReader.GetOrdinal("Pack_Name")).ToString();
            }
            objSqlReader.Close();
            comboBox2.Text = sPack;
        }

        private void Btn_NewCard_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || comboBox1.Text == "" || comboBox2.Text == "")
                MessageBox.Show("不能有空项！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                NewCard(textBox1.Text, Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), comboBox1.Text, comboBox2.Text);
                UpdateCards();
            }
        }

        private void NewCard(string sName, int nPower, int nPrice, string sRarity, string sPackName)
        {
            proc = "prd_NewCard";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@card_name", SqlDbType.VarChar, 30), new SqlParameter("@card_power", SqlDbType.Int), new SqlParameter("@card_price", SqlDbType.Int), new SqlParameter("@card_rarity", SqlDbType.VarChar, 3), new SqlParameter("@pack_name", SqlDbType.VarChar, 4) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = sName;
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.Input;
            parameters[1].Value = nPower;
            objSqlCommand.Parameters.Add(parameters[1]);
            parameters[2].Direction = ParameterDirection.Input;
            parameters[2].Value = nPrice;
            objSqlCommand.Parameters.Add(parameters[2]);
            parameters[3].Direction = ParameterDirection.Input;
            parameters[3].Value = sRarity;
            objSqlCommand.Parameters.Add(parameters[3]);
            parameters[4].Direction = ParameterDirection.Input;
            parameters[4].Value = sPackName;
            objSqlCommand.Parameters.Add(parameters[4]);
            objSqlCommand.ExecuteNonQuery();
            MessageBox.Show("添加成功。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
        }
    }
}