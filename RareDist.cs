using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Web.UI.DataVisualization.Charting;

namespace TCGGame
{
    public partial class RareDist : Form
    {
        private SqlConnection objSqlConnection = null;
        private int sm_nPack_ID = 0;

        public RareDist(int nPack_ID)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            objSqlConnection = Form1.GetSqlConnection();
            sm_nPack_ID = nPack_ID;
        }

        private void RareDist_Load(object sender, EventArgs e)
        {
            chart1.Width = (int)(this.Width * 0.98f);
            chart1.Height = (int)(this.Height * 0.96f);
            string sql = "EXEC prd_RarityDistribution " + sm_nPack_ID.ToString() + ";";
            DataSet objDataSet = new DataSet();
            SqlDataAdapter objDataAdapter = new SqlDataAdapter(sql, objSqlConnection);
            objDataAdapter.Fill(objDataSet);
            DataTable objDataTable = objDataSet.Tables[0];
            chart1.Series.Clear();
            chart1.Series.Add("Series1");
            chart1.DataSource = objDataSet;
            chart1.Series["Series1"].XValueMember = "Temp_Rarity";
            chart1.Series["Series1"].YValueMembers = "Temp_Num";
            chart1.DataBind();
            chart1.Series["Series1"].ToolTip = "#VALX: #VAL";
            chart1.Legends[0].Enabled = false;
            chart1.Series["Series1"].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.BrightPastel;
            chart1.Series["Series1"].IsValueShownAsLabel = true;
            chart1.Series["Series1"].Label = "#VAL";
            chart1.ChartAreas[0].Axes[0].MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.Title = "罕贵";
            chart1.ChartAreas[0].AxisY.Title = "卡片种数";
        }

        private void RareDist_Resize(object sender, EventArgs e)
        {
            chart1.Width = (int)(this.Width * 0.98f);
            chart1.Height = (int)(this.Height * 0.96f);
        }
    }
}
