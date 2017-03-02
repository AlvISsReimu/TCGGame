/*
 * 12061036 李明浩 Trading Card Game
 * E-mail: 498704999@qq.com
 * 版本：20150102
 * */

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
    public partial class Form1 : Form
    {
        /*-------------访问DB-------------*/
        private const string ConnectionString = "server = localhost; uid = sa; pwd = 123456; database = TCGGame";
        private static SqlConnection objSqlConnection = null;
        private static SqlCommand objSqlCommand = null;
        private static DataSet objDataSet = null;
        private static SqlDataAdapter objDataAdapter = null;
        private static SqlDataReader objSqlReader = null;
        private static string sql = null;
        private static string proc = null;
        /*-------------访问DB end-------------*/


        /*-------------窗体-------------*/
        private static Login login;
        /*-------------窗体 end-------------*/


        /*-------------常量-------------*/
        private const int EVENT_GETMONEY = 10;              //事件：获得金钱概率
        private const int EVENT_LOSEMONEY = 10;             //事件：损失金钱概率
        private const int EVENT_LOSEHP = 10;                //事件：损失健康值概率
        private const int EVENT_GETCARD = 10;               //事件：获得卡片概率
        private const int EVENT_LOSECARD = 10;              //事件：丢失卡片概率
        private const int EVENT_PRICEUP = 10;               //事件：涨价概率
        private const int EVENT_PRICEDOWN = 10;             //事件：降价概率
        private const int HOSPITAL_PRICE = 150;             //医院单价
        private const int DUEL_PRICE = 500;                 //比赛单价
        /*-------------常量 end-------------*/


        /*-------------玩家信息变量-------------*/
        private static int sm_nUserID;
        private static string sm_sUserName;
        private static int sm_nUserHP;
        private static int sm_nUserMoney;
        private static int sm_nGameDate;
        private static int sm_nUserManageLevel;
        /*-------------玩家信息变量 end-------------*/


        public Form1()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            objSqlConnection = new SqlConnection(ConnectionString);
            objSqlConnection.Open();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            login = new Login(this);
            login.Show();
            this.Hide();
            this.ShowInTaskbar = false;
            InitHoldCardGrid();
            InitCardShopGrid();
        }

        public static SqlConnection GetSqlConnection()
        {
            return objSqlConnection;
        }

        public void InitData(int nUserID)                   //登陆后初始化信息与界面
        {
            textBox1.Text = "";
            sm_nUserID = nUserID;
            this.UserIDLabel.Text = "玩家ID: " + sm_nUserID.ToString();
            UpdateUserName();
            UpdateUserHP();
            UpdateUserMoney();
            UpdateGameDate();
            UpdateUserManageLevel();
            UpdateHoldCard();
            UpdatePowerSum();
            label3.Text = "";
        }

        private void UpdateUserManageLevel()
        {
            sql = "SELECT User_ManageLevel FROM tbl_User WHERE User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                sm_nUserManageLevel = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("User_ManageLevel")));
            objSqlReader.Close();
            if (sm_nUserManageLevel == 0)
                管理ToolStripMenuItem.Visible = false;
            else
                管理ToolStripMenuItem.Visible = true;
        }

        public void PassDay()
        {
            if (sm_nGameDate < 52)
            {
                sm_nGameDate++;
                sql = "update tbl_User set User_GameDate = " + sm_nGameDate.ToString() + "where User_ID = '" + sm_nUserID.ToString() + "';";
                objSqlCommand = new SqlCommand(sql, objSqlConnection);
                objSqlCommand.ExecuteNonQuery();
                UpdateGameDate();
                if (sm_nGameDate == 1)
                {
                    textBox1.AppendText("欢迎来到Trading Card Game！\r\n");
                    textBox1.ScrollToCaret();
                }
                textBox1.AppendText("迎来第" + sm_nGameDate.ToString() + "天！\r\n");
                textBox1.ScrollToCaret();
                sql = "EXEC prd_GenCardShop;";
                objDataSet = new DataSet();
                objDataAdapter = new SqlDataAdapter(sql, objSqlConnection);
                objDataAdapter.Fill(objDataSet);
                dataGridView2.DataSource = objDataSet.Tables[0];
                dataGridView2.Columns[0].HeaderText = "ID";
                dataGridView2.Columns[1].HeaderText = "卡名";
                dataGridView2.Columns[2].HeaderText = "战斗力";
                dataGridView2.Columns[3].HeaderText = "罕贵";
                dataGridView2.Columns[4].HeaderText = "价格";
                dataGridView2.Columns[5].HeaderText = "数量";
                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Width = 108;
                dataGridView2.Columns[2].Width = 64;
                dataGridView2.Columns[3].Width = 35;
                dataGridView2.Columns[4].Width = 35;
                dataGridView2.Columns[5].Width = dataGridView2.Width - dataGridView2.Columns[1].Width - dataGridView2.Columns[2].Width - dataGridView2.Columns[3].Width - dataGridView2.Columns[4].Width - 20;
                numericUpDown2.Value = 0;
                UpdatePack();
                UpdateHoldCard();
                UpdateSellPriceLabel();
                UpdateHoldCardColor();
                EventHappen();
            }
            else
                GameOver();
        }

        private void WriteMoney(int nMoney)
        {
            int nNewMoney = 0;
            if (nMoney + sm_nUserMoney > 0)
                nNewMoney = nMoney + sm_nUserMoney;
            sql = "update tbl_User set User_Money = " + nNewMoney.ToString() + "where User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objSqlCommand.ExecuteNonQuery();
            UpdateUserMoney();
        }

        private void WriteHP(int nHP)
        {
            int nNewHP = 0;
            if (nHP + sm_nUserHP > 0)
                nNewHP = nHP + sm_nUserHP;
            sql = "update tbl_User set User_HP = " + nNewHP.ToString() + "where User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objSqlCommand.ExecuteNonQuery();
            UpdateUserHP();
            if (sm_nUserHP == 0)
                GameOver();
        }

        private void EventHappen()
        {
            Random objRandom = new Random();
            int nRandom = objRandom.Next(0, 100);
            if (nRandom <= EVENT_LOSEMONEY)
            {
                int nMoneyLose = objRandom.Next(100, 600);
                if (nMoneyLose > sm_nUserMoney)
                    nMoneyLose = sm_nUserMoney;
                WriteMoney((-1) * nMoneyLose);
                if (nRandom <= EVENT_LOSEMONEY - EVENT_LOSEMONEY / 2)
                    textBox1.AppendText("在卡店时钱被人摸走，损失" + nMoneyLose.ToString() + "金钱！\r\n");
                else
                    textBox1.AppendText("被卡商颇回，损失" + nMoneyLose.ToString() + "金钱！\r\n");
                textBox1.ScrollToCaret();
            }
            else if (nRandom <= EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                int nMoneyGet = objRandom.Next(200, 800);
                WriteMoney(nMoneyGet);
                if (nRandom <= EVENT_GETMONEY + EVENT_LOSEMONEY - EVENT_GETMONEY / 2)
                    textBox1.AppendText("在卡店桌子上发现了遗留的" + nMoneyGet.ToString() + "金钱！\r\n");
                else
                    textBox1.AppendText("颇回小学生成功，获得" + nMoneyGet.ToString() + "金钱！\r\n");
                textBox1.ScrollToCaret();
            }
            else if (nRandom <= EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                int nHPLose = objRandom.Next(1, 10);
                if (nHPLose > sm_nUserHP)
                    nHPLose = sm_nUserHP;
                WriteHP((-1) * nHPLose);
                if (nRandom <= EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY - EVENT_LOSEHP / 2)
                    textBox1.AppendText("在卡店砍价被打了，损失" + nHPLose.ToString() + "健康值！\r\n");
                else
                    textBox1.AppendText("去卡店途中不小心受伤，损失" + nHPLose.ToString() + "健康值！\r\n");
                textBox1.ScrollToCaret();
            }
            else if (nRandom <= EVENT_GETCARD + EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                string sCard_Name = "";
                string sCard_Rarity = "";
                int nCard_Num = 0;
                proc = "prd_Event_GetCard";
                objSqlCommand = new SqlCommand(proc, objSqlConnection);
                objDataAdapter = new SqlDataAdapter();
                objDataAdapter.SelectCommand = objSqlCommand;
                objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int), new SqlParameter("@card_name", SqlDbType.VarChar, 30), new SqlParameter("@card_num", SqlDbType.Int), new SqlParameter("@card_rarity", SqlDbType.VarChar, 3) };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = sm_nUserID;
                objSqlCommand.Parameters.Add(parameters[0]);
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Value = sCard_Name;
                objSqlCommand.Parameters.Add(parameters[1]);
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Value = nCard_Num;
                objSqlCommand.Parameters.Add(parameters[2]);
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = sCard_Rarity;
                objSqlCommand.Parameters.Add(parameters[3]);
                objSqlCommand.ExecuteNonQuery();
                sCard_Name = parameters[1].Value.ToString();
                nCard_Num = Convert.ToInt32(parameters[2].Value);
                sCard_Rarity = parameters[3].Value.ToString();
                textBox1.AppendText("在别人卡堆中翻出了" + nCard_Num.ToString() + "张" + sCard_Rarity + "的" + sCard_Name + "！\r\n");
                textBox1.ScrollToCaret();
                UpdateHoldCard();
                UpdatePowerSum();
            }
            else if (nRandom <= EVENT_LOSECARD + EVENT_GETCARD + EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                if (dataGridView1.RowCount == 0)
                {
                    textBox1.AppendText("图谋不轨的玩家来翻你卡本，可你的卡本里什么也没有。\r\n");
                    textBox1.ScrollToCaret();
                    return;
                }
                string sCard_Name = "";
                string sCard_Rarity = "";
                int nLoseCard_Num = 0;
                proc = "prd_Event_LoseCard";
                objSqlCommand = new SqlCommand(proc, objSqlConnection);
                objDataAdapter = new SqlDataAdapter();
                objDataAdapter.SelectCommand = objSqlCommand;
                objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int), new SqlParameter("@card_name", SqlDbType.VarChar, 30), new SqlParameter("@losecard_num", SqlDbType.Int), new SqlParameter("@card_rarity", SqlDbType.VarChar, 3) };
                parameters[0].Direction = ParameterDirection.Input;
                parameters[0].Value = sm_nUserID;
                objSqlCommand.Parameters.Add(parameters[0]);
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Value = sCard_Name;
                objSqlCommand.Parameters.Add(parameters[1]);
                parameters[2].Direction = ParameterDirection.Output;
                parameters[2].Value = nLoseCard_Num;
                objSqlCommand.Parameters.Add(parameters[2]);
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Value = sCard_Rarity;
                objSqlCommand.Parameters.Add(parameters[3]);
                objSqlCommand.ExecuteNonQuery();
                sCard_Name = parameters[1].Value.ToString();
                nLoseCard_Num = Convert.ToInt32(parameters[2].Value);
                sCard_Rarity = parameters[3].Value.ToString();
                textBox1.AppendText("不小心被别人从卡本里顺走了" + nLoseCard_Num.ToString() + "张" + sCard_Rarity + "的" + sCard_Name + "！\r\n");
                textBox1.ScrollToCaret();
                UpdateHoldCard();
                UpdatePowerSum();
            }
            else if (nRandom <= EVENT_PRICEUP + EVENT_LOSECARD + EVENT_GETCARD + EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                int nRows = dataGridView2.RowCount;
                int nIndex = objRandom.Next(0, nRows);
                int nPrice = Convert.ToInt32(dataGridView2.Rows[nIndex].Cells[4].Value);
                nPrice = (int)(nPrice * (objRandom.NextDouble() + 1.0f));
                dataGridView2.Rows[nIndex].Cells[4].Value = nPrice;
                textBox1.AppendText("卡商大炒" + dataGridView2.Rows[nIndex].Cells[3].Value.ToString() + "的" + dataGridView2.Rows[nIndex].Cells[1].Value.ToString() + "，其单价猛增至" + nPrice.ToString() + "！\r\n");
                textBox1.ScrollToCaret();
            }
            else if (nRandom <= EVENT_PRICEDOWN + EVENT_PRICEUP + EVENT_LOSECARD + EVENT_GETCARD + EVENT_LOSEHP + EVENT_GETMONEY + EVENT_LOSEMONEY)
            {
                int nRows = dataGridView2.RowCount;
                int nIndex = objRandom.Next(0, nRows);
                int nPrice = Convert.ToInt32(dataGridView2.Rows[nIndex].Cells[4].Value);
                nPrice = (int)(nPrice * objRandom.NextDouble()) + 1;
                dataGridView2.Rows[nIndex].Cells[4].Value = nPrice;
                textBox1.AppendText(dataGridView2.Rows[nIndex].Cells[3].Value.ToString() + "的" + dataGridView2.Rows[nIndex].Cells[1].Value.ToString() + "没什么人采用，单价跌至" + nPrice.ToString() + "！\r\n");
                textBox1.ScrollToCaret();
            }
        }

        private void BtnEndDay_Click(object sender, EventArgs e)
        {
            PassDay();
        }

        private void Add2Rank()
        {
            sql = "INSERT INTO tbl_Rank(Rank_ID) VALUES(" + sm_nUserID.ToString() + ");";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objSqlCommand.ExecuteNonQuery();
        }

        private void GameOver()
        {
            Add2Rank();
            int nRank = 1;
            sql = "SELECT Rank_Number FROM tbl_Rank WHERE Rank_Money = '" + sm_nUserMoney.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                nRank = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Rank_Number")));
            objSqlReader.Close();
            DialogResult dr = MessageBox.Show("游戏结束，您的排名为" + nRank.ToString() + "，请再接再厉！\r\n是否重新开始游戏？", "Game Over", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                InitUser();
                InitData(sm_nUserID);
                PassDay();
            }
            else
            {
                InitUser();
                this.Hide();
                this.ShowInTaskbar = false;
                login = new Login(this);
                login.Show();
            }
        }

        private void InitUser()
        {
            proc = "prd_InitUser";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = sm_nUserID;
            objSqlCommand.Parameters.Add(parameters[0]);
            objSqlCommand.ExecuteNonQuery();
        }


        /*-------------玩家信息-------------*/
        private void UpdateUserName()
        {
            sql = "SELECT User_Name FROM tbl_User WHERE User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                sm_sUserName = objSqlReader.GetValue(objSqlReader.GetOrdinal("User_Name")).ToString();
            objSqlReader.Close();
            this.UserNameLabel.Text = "用户名: " + sm_sUserName;
        }

        private void UpdateUserHP()
        {
            sql = "SELECT User_HP FROM tbl_User WHERE User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                sm_nUserHP = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("User_HP")));
            objSqlReader.Close();
            this.UserHPLabel.Text = "健康值: " + sm_nUserHP.ToString();
        }

        private void UpdateUserMoney()
        {
            sql = "SELECT User_Money FROM tbl_User WHERE User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                sm_nUserMoney = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("User_Money")));
            objSqlReader.Close();
            this.UserMoneyLabel.Text = "金钱: " + sm_nUserMoney.ToString(); ;
        }

        private void UpdateGameDate()
        {
            sql = "SELECT User_GameDate FROM tbl_User WHERE User_ID = '" + sm_nUserID.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                sm_nGameDate = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("User_GameDate")));
            objSqlReader.Close();
            this.GameDateLabel.Text = "经过天数: " + sm_nGameDate.ToString();
        }

        private void UpdatePowerSum()
        {
            proc = "prd_CalcPowerSum";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int), new SqlParameter("@multpower", SqlDbType.Int) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = sm_nUserID;
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.ReturnValue;
            objSqlCommand.Parameters.Add(parameters[1]);
            objSqlCommand.ExecuteNonQuery();
            this.UserPowerLabel.Text = "总战斗力: " + parameters[1].Value.ToString();
        }
        /*-------------玩家信息 end-------------*/


        /*-------------卡本-------------*/
        private void InitHoldCardGrid()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void UpdateHoldCard()
        {
            sql = "EXEC prd_GetHoldCard " + sm_nUserID.ToString() + ";";
            objDataSet = new DataSet();
            DataTable objDataTable = new DataTable();
            objDataTable.Columns.Add("Name", typeof(System.String));
            objDataTable.Columns.Add("Power", typeof(System.Int32));
            objDataTable.Columns.Add("Rarity", typeof(System.String));
            objDataTable.Columns.Add("Quantity", typeof(System.Int32));
            objDataTable.Columns.Add("ID", typeof(System.Int32));
            DataSet objDataSet2 = new DataSet();
            objDataSet2.Tables.Add(objDataTable);
            objDataAdapter = new SqlDataAdapter(sql, objSqlConnection);
            objDataAdapter.Fill(objDataSet);
            if (checkBox1.Checked)
            {
                foreach (DataRow objDataRow in objDataSet.Tables[0].Rows)
                {
                    int nID = Convert.ToInt32(objDataRow[4]);
                    if (isInShop(nID))
                        objDataSet2.Tables[0].Rows.Add(objDataRow[0].ToString(), objDataRow[1].ToString(), objDataRow[2].ToString(), objDataRow[3].ToString(), objDataRow[4].ToString());
                }
                dataGridView1.DataSource = objDataSet2.Tables[0];
            }
            else
                dataGridView1.DataSource = objDataSet.Tables[0];
            UpdateSellPriceLabel();
            UpdateHoldCardColor();
            dataGridView1.Columns[0].HeaderText = "卡名";
            dataGridView1.Columns[1].HeaderText = "战斗力";
            dataGridView1.Columns[2].HeaderText = "罕贵";
            dataGridView1.Columns[3].HeaderText = "数量";
            dataGridView1.Columns[4].HeaderText = "ID";
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[0].Width = 95;
            dataGridView1.Columns[1].Width = 64;
            dataGridView1.Columns[2].Width = 64;
            dataGridView1.Columns[3].Width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[1].Width - dataGridView1.Columns[2].Width - 20;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHoldCard();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            numericUpDown1.Maximum = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value);
            numericUpDown1.Value = numericUpDown1.Maximum;
            UpdateSellPriceLabel();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (numericUpDown1.Value > Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value))
                    BtnSell.Enabled = false;
                else
                    BtnSell.Enabled = true;
            }
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value);
            numericUpDown1.Value = numericUpDown1.Maximum;
            UpdateSellPriceLabel();
            UpdateHoldCardColor();
        }

        private void BtnSell_Click(object sender, EventArgs e)
        {
            int nSellNum = (int)numericUpDown1.Value;
            if (nSellNum < 1)
                return;
            if (dataGridView1.RowCount < 1)
                return;
            int nID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value);
            string sName = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
            if (!isInShop(nID))
            {
                MessageBox.Show("此卡不在本日出售范围内，请重新选择。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int nPrice = 0;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView2.Rows[i].Cells[0].Value) == nID)
                {
                    nPrice = Convert.ToInt32(dataGridView2.Rows[i].Cells[4].Value);
                    dataGridView2.Rows[i].Cells[5].Value = Convert.ToInt32(dataGridView2.Rows[i].Cells[5].Value) + nSellNum;
                    break;
                }
            }
            proc = "prd_SellCard";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@card_id", SqlDbType.Int), new SqlParameter("@sell_num", SqlDbType.Int), new SqlParameter("@sell_price", SqlDbType.Int), new SqlParameter("@user_id", SqlDbType.Int) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = nID;
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.Input;
            parameters[1].Value = nSellNum;
            objSqlCommand.Parameters.Add(parameters[1]);
            parameters[2].Direction = ParameterDirection.Input;
            parameters[2].Value = nPrice;
            objSqlCommand.Parameters.Add(parameters[2]);
            parameters[3].Direction = ParameterDirection.Input;
            parameters[3].Value = sm_nUserID;
            objSqlCommand.Parameters.Add(parameters[3]);
            objSqlCommand.ExecuteNonQuery();
            UpdateHoldCard();
            UpdatePowerSum();
            UpdateUserMoney();
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            textBox1.AppendText("卖出" + nSellNum.ToString() + "张" + sName + "，共收入" + (nSellNum * nPrice).ToString() + "金钱。\r\n");
            textBox1.ScrollToCaret();
        }

        private bool isInShop(int nID)
        {
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView2.Rows[i].Cells[0].Value) == nID)
                    return true;
            }
            return false;
        }

        private void UpdateSellPriceLabel()
        {
            if (dataGridView1.RowCount <= 0)
            {
                label3.Text = "";
                return;
            }
            int nID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value);
            if (isInShop(nID))
            {
                int nPrice = 0;
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    if (Convert.ToInt32(dataGridView2.Rows[i].Cells[0].Value) == nID)
                    {
                        nPrice = Convert.ToInt32(dataGridView2.Rows[i].Cells[4].Value);
                        break;
                    }
                }
                label3.Text = "所选卡今日价格：" + nPrice.ToString();
            }
            else
                label3.Text = "此卡不在今日出售范围内。";
        }

        private void UpdateHoldCardColor()
        {
            int nRows = dataGridView1.RowCount;
            if (nRows != 0)
            {
                for (int i = 0; i < nRows; i++)
                {
                    int nID = Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
                    if (isInShop(nID))
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                    else
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Silver;
                }
            }
        }
        /*-------------卡本 end-------------*/


        /*-------------卡店-------------*/
        private void InitCardShopGrid()
        {
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            int nBuyNum = (int)numericUpDown2.Value;
            if (nBuyNum < 1)
                return;
            if (dataGridView2.RowCount < 1)
                return;
            int nID = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[0].Value);
            int nPrice = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[4].Value);
            if (nBuyNum * nPrice > sm_nUserMoney)
            {
                MessageBox.Show("金钱不足！", "穷", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
            proc = "prd_BuyCard";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@card_id", SqlDbType.Int), new SqlParameter("@buy_num", SqlDbType.Int), new SqlParameter("@buy_price", SqlDbType.Int), new SqlParameter("@user_id", SqlDbType.Int) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = nID;
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.Input;
            parameters[1].Value = nBuyNum;
            objSqlCommand.Parameters.Add(parameters[1]);
            parameters[2].Direction = ParameterDirection.Input;
            parameters[2].Value = nPrice;
            objSqlCommand.Parameters.Add(parameters[2]);
            parameters[3].Direction = ParameterDirection.Input;
            parameters[3].Value = sm_nUserID;
            objSqlCommand.Parameters.Add(parameters[3]);
            objSqlCommand.ExecuteNonQuery();
            UpdateHoldCard();
            UpdatePowerSum();
            UpdateUserMoney();
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value) - nBuyNum;
            textBox1.AppendText("买入" + nBuyNum.ToString() + "张" + dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[1].Value.ToString() + "，共花费" + (nBuyNum * nPrice).ToString() + "金钱。\r\n");
            textBox1.ScrollToCaret();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            numericUpDown2.Maximum = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value);
            numericUpDown2.Value = numericUpDown2.Maximum;
        }

        private void dataGridView2_Sorted(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value);
            numericUpDown2.Value = numericUpDown2.Maximum;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount != 0)
            {
                if (numericUpDown2.Value > Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value))
                    BtnBuy.Enabled = false;
                else
                    BtnBuy.Enabled = true;
            }
        }
        /*-------------卡店 end-------------*/


        /*-------------开包-------------*/
        private void UpdatePack()
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
            sql = "SELECT Pack_Name FROM tbl_Pack WHERE Pack_Date <= " + sm_nGameDate.ToString() + ";";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            while (objSqlReader.Read())
                listBox1.Items.Add(objSqlReader.GetValue(objSqlReader.GetOrdinal("Pack_Name")).ToString());
            objSqlReader.Close();
        }

        private void BtnOpenPack_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                MessageBox.Show("请选择要开的包！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                OpenPack(1);
            }
        }

        private void OpenPack(int nPackNum)
        {
            int nPrice = 0, nPack_ID = 0;
            sql = "SELECT Pack_Price, Pack_ID FROM tbl_Pack WHERE Pack_Name = '" + listBox1.SelectedItem.ToString() + "';";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
            {
                nPrice = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Pack_Price"))) * nPackNum;
                nPack_ID = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Pack_ID")));
            }
            objSqlReader.Close();
            if (nPackNum == 11)
                nPrice = (int)(nPrice * 0.8751) + 1;
            DialogResult dr = MessageBox.Show(nPackNum.ToString() + "包" + listBox1.SelectedItem.ToString() + "的价格是" + nPrice.ToString() + "，确定要开吗？", "开包", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                if (nPrice > sm_nUserMoney)
                {
                    MessageBox.Show("金钱不足！", "穷", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                WriteMoney((-1) * nPrice);
                for (int i = 0; i < nPackNum; i++)
                {
                    proc = "prd_OpenPack";
                    objSqlCommand = new SqlCommand(proc, objSqlConnection);
                    objDataAdapter = new SqlDataAdapter();
                    objDataAdapter.SelectCommand = objSqlCommand;
                    objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int), new SqlParameter("@pack_id", SqlDbType.Int) };
                    parameters[0].Direction = ParameterDirection.Input;
                    parameters[0].Value = sm_nUserID;
                    objSqlCommand.Parameters.Add(parameters[0]);
                    parameters[1].Direction = ParameterDirection.Input;
                    parameters[1].Value = nPack_ID;
                    objSqlCommand.Parameters.Add(parameters[1]);
                    objDataSet = new DataSet();
                    objDataAdapter.Fill(objDataSet);
                    DataTable objDataTable = objDataSet.Tables[0];
                    textBox1.AppendText("第" + (i + 1).ToString() + "包" + listBox1.SelectedItem.ToString() + "的结果为：\r\n");
                    foreach (DataRow objDataRow in objDataTable.Rows)
                        textBox1.AppendText(objDataRow["Temp_Card_Rarity"].ToString() + " " + objDataRow["Temp_Card_Name"].ToString() + "\r\n");
                    textBox1.ScrollToCaret();
                    UpdateHoldCard();
                    UpdatePowerSum();
                }
            }
        }

        private void BtnEleven_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                MessageBox.Show("请选择要开的包！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                OpenPack(11);
            }
        }

        private void BtnRareDist_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                MessageBox.Show("请选择要查看罕贵分布的包！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                int nPack_ID = 0;
                sql = "SELECT Pack_Price, Pack_ID FROM tbl_Pack WHERE Pack_Name = '" + listBox1.SelectedItem.ToString() + "';";
                objSqlCommand = new SqlCommand(sql, objSqlConnection);
                objDataAdapter = new SqlDataAdapter();
                objDataAdapter.SelectCommand = objSqlCommand;
                objSqlReader = objSqlCommand.ExecuteReader();
                if (objSqlReader.Read())
                    nPack_ID = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("Pack_ID")));
                objSqlReader.Close();
                RareDist raredist = new RareDist(nPack_ID);
                raredist.Show();
            }
        }
        /*-------------开包 end-------------*/


        /*-------------病院-------------*/
        private void BtnHospital_Click(object sender, EventArgs e)
        {
            if (sm_nUserHP == 100)
            {
                MessageBox.Show("您很健康，不需要治疗。", "西木野病院", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ;
            }
            int nPrice = HOSPITAL_PRICE * (100 - sm_nUserHP);
            DialogResult dr = MessageBox.Show("完全恢复健康需要花费" + nPrice.ToString() + "金钱，是否确认治疗？", "西木野病院", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                if (nPrice > sm_nUserMoney)
                {
                    MessageBox.Show("金钱不足！", "穷", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                WriteMoney((-1) * nPrice);
                WriteHP(100 - sm_nUserHP);
                textBox1.AppendText("去西木野病院治疗后，完全恢复了健康！\r\n");
                textBox1.ScrollToCaret();
            }
        }
        /*-------------病院 end-------------*/


        /*-------------比赛-------------*/
        private void BtnDuel_Click(object sender, EventArgs e)
        {
            if (sm_nUserHP <= 5)
            {
                MessageBox.Show("参加比赛至少需要健康值为6，请治疗后再来。", "比赛", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
            DialogResult dr = MessageBox.Show("参加比赛需要花费" + DUEL_PRICE.ToString() + "金钱以及消耗若干健康值，是否确认参加？", "比赛", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr != DialogResult.OK)
                return ;
            if (DUEL_PRICE > sm_nUserMoney)
            {
                MessageBox.Show("金钱不足！", "穷", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
            WriteMoney((-1) * DUEL_PRICE);
            Random objRandom = new Random();
            WriteHP((-1) * objRandom.Next(1, 6));
            int nOppoID = 0, nOppoPower = 0, nUserPower = 0;;
            sql = "SELECT TOP 1 User_ID FROM tbl_User WHERE User_ID != '" + sm_nUserID.ToString() + "' ORDER BY newid();";
            objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objSqlReader = objSqlCommand.ExecuteReader();
            if (objSqlReader.Read())
                nOppoID = Convert.ToInt32(objSqlReader.GetValue(objSqlReader.GetOrdinal("User_ID")));
            objSqlReader.Close();
            proc = "prd_CalcPowerSum";
            objSqlCommand = new SqlCommand(proc, objSqlConnection);
            objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objSqlCommand;
            objDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            IDataParameter[] parameters = { new SqlParameter("@user_id", SqlDbType.Int), new SqlParameter("@multpower", SqlDbType.Int) };
            parameters[0].Direction = ParameterDirection.Input;
            parameters[0].Value = nOppoID;
            objSqlCommand.Parameters.Add(parameters[0]);
            parameters[1].Direction = ParameterDirection.ReturnValue;
            objSqlCommand.Parameters.Add(parameters[1]);
            objSqlCommand.ExecuteNonQuery();
            nOppoPower = Convert.ToInt32(parameters[1].Value);
            parameters[0].Value = sm_nUserID;
            objSqlCommand.ExecuteNonQuery();
            nUserPower = Convert.ToInt32(parameters[1].Value);
            MessageBox.Show("您的对手战斗力为" + nOppoPower.ToString() + "，单击确定开始决斗！", "比赛", MessageBoxButtons.OK, MessageBoxIcon.Information);
            float fRate = (1.0f - (100 - sm_nUserHP) * 1.0f / 75.0f) * (nUserPower + 50) * 1.0f / ((nOppoPower + 50) * 1.0f);
            if (objRandom.NextDouble() <= fRate)
            {
                int nPrize = Convert.ToInt32((objRandom.Next(2, 6) + fRate) * DUEL_PRICE);
                textBox1.AppendText("参加比赛并获得胜利，赚取奖金" + nPrize.ToString() + "！\r\n");
                WriteMoney(nPrize);
            }
            else
                textBox1.AppendText("参加比赛不幸落败...\r\n");
            textBox1.ScrollToCaret();
        }
        /*-------------比赛 end-------------*/


        /*-------------菜单-------------*/
        private void 排行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rank rank = new Rank();
            rank.Show();
        }

        private void 注销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您真的要注销吗？游戏进度将自动保存。", "注销", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                login = new Login(this);
                login.Show();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manage manage = new Manage();
            manage.Show();
        }
        /*-------------菜单 end-------------*/

    }
}