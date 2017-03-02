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

namespace TCGGame
{
    public partial class Rank : Form
    {
        private SqlConnection objSqlConnection = null;

        public Rank()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            objSqlConnection = Form1.GetSqlConnection();
        }

        private void Rank_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            string sql = "SELECT Rank_Number, Rank_ID, User_Name, Rank_Money, Rank_HP, Rank_Time FROM tbl_Rank, tbl_User WHERE Rank_ID = User_ID ORDER BY Rank_Number;";
            DataSet objDataSet = new DataSet();
            SqlDataAdapter objDataAdapter = new SqlDataAdapter(sql, objSqlConnection);
            objDataAdapter.Fill(objDataSet);
            dataGridView1.DataSource = objDataSet.Tables[0];
            dataGridView1.Columns[0].HeaderText = "排名";
            dataGridView1.Columns[1].HeaderText = "ID";
            dataGridView1.Columns[2].HeaderText = "用户名";
            dataGridView1.Columns[3].HeaderText = "金钱";
            dataGridView1.Columns[4].HeaderText = "健康值";
            dataGridView1.Columns[5].HeaderText = "记录时间";
            if (dataGridView1.RowCount >= 3)
            {
                dataGridView1.Rows[0].DefaultCellStyle.BackColor = System.Drawing.Color.Gold;
                dataGridView1.Rows[1].DefaultCellStyle.BackColor = System.Drawing.Color.Silver;
                dataGridView1.Rows[2].DefaultCellStyle.BackColor = System.Drawing.Color.DarkOrange;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}