using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sap.Data.Hana;

namespace SOSequencing
{
    public partial class frmSOSeq : Form
    {
        int visableColumnsCount = 0;
        int columnWidth = 0;
        DataSet DS = null;
        DataTable DS1 = null;
        DataTable DS2 = null;
        int hideColumnsIndex = -1;
        public static string _str_columnname = "";
        public static int _int_cnt = 0;
        public static string _str_val = "";
 public static SAPbobsCOM.Company oCompany;
        public static SAPbobsCOM.BusinessPartners oBusinessPartnersA;
        public static SAPbobsCOM.Items oItemsA;
        public static SAPbouiCOM.Application SapApplication;
        public static SAPbobsCOM.CompanyService oCompService;
        public static SAPbobsCOM.GeneralService oGeneralService;
        HanaConnection hanaCon = new HanaConnection("Server=192.168.101.132:30015;Current Schema=BLVL_DEMO;UserID=SYSTEM;Password=Blvl12345");
        
        public frmSOSeq()
        {
            InitializeComponent();
          
        }

      
        private void SOSeq_Load(object sender, EventArgs e)
        {
           
            hanaCon.Open();
            string StrSql = @"select max(ifnull(""DocEntry"",0)) +1 DocEntry  from ""@IK_SOSQ"" ";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
               hanaReader.Read();
            txtDocNum.Text =hanaReader["DocEntry"].ToString();
         
        
           

           

           
        }
        internal void connectCompany(string Server, string CompanyDB, string SAPUser, string SAPPass, string SQLUser, string SQLPass)
        {
             string cookie, sErrorMsg;
                int iErrorCode = 0;
                string connStr;
                oCompany = new SAPbobsCOM.Company();
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                oCompany.Server = Server;
                oCompany.CompanyDB = CompanyDB;
                oCompany.UserName = SAPUser;
                oCompany.Password = SAPPass;
                oCompany.DbUserName = SQLUser;
                oCompany.DbPassword = SQLPass;
                oCompany.UseTrusted = false;
                oCompany.Connect();
            
       
        }
        
        private void btnUP_Click(object sender, EventArgs e)
        {
            if (dgvSOSeq.RowCount > 0)
            {
                btnAdd.Text = "Update";
                if (dgvSOSeq.SelectedRows[0].Index != 0)
                {
                    for (int j = 0; j < this.dgvSOSeq.Columns.Count; j++)
                    {
                        object tmp = this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index].Value;
                        this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index].Value = this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index - 1].Value;
                        this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index - 1].Value = tmp;
                    }
                    int a = dgvSOSeq.SelectedRows[0].Index;
                    dgvSOSeq.ClearSelection();
                    this.dgvSOSeq.Rows[a - 1].Selected = true;
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (dgvSOSeq.SelectedRows[0].Index != dgvSOSeq.Rows.Count - 1)
            {
                btnAdd.Text = "Update";
                for (int j = 0; j < this.dgvSOSeq.Columns.Count; j++)
                {
                    object tmp = this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index].Value;
                    this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index].Value = this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index + 1].Value;
                    this.dgvSOSeq[j, dgvSOSeq.SelectedRows[0].Index + 1].Value = tmp;
                }
                int i = dgvSOSeq.SelectedRows[0].Index;
                dgvSOSeq.ClearSelection();
                this.dgvSOSeq.Rows[i + 1].Selected = true;
            }
        }

        private void btnPOP_Click(object sender, EventArgs e)
        {
            //HanaConnection hanaCon = new HanaConnection("Server=192.168.101.132:30015;Current Schema=BLVL_DEMO;UserID=SYSTEM;Password=Blvl12345");
            //hanaCon.Open();
         //   IFormatProvider ifp = new IFormatProvider();
            DateTime FDate = Convert.ToDateTime(txtFrom.Text);
            DateTime TDate = Convert.ToDateTime(txtTo.Text);
            if (btnAdd.Text=="Add")
            {
            if (DateTime.Compare(FDate, TDate) > 0)
            {
                MessageBox.Show("Date Mismatch");
                return;
            }
            else
            {
                //  string strQry = @"Select ""DocEntry"" From ""@IK_SOSQ"" T1 Where ('" + FDate + "' >= T1.""U_FrmDate"" And '" + FDate + "' <= T1.""U_ToDate"")   Or ('" + TDt + "' >= T1.""U_FrmDate"" And '" + TDate + "' <= T1.""U_ToDate"")" ;
                //string _str_Query = @"Select ""DocEntry"" From ""@IK_SOSQ"" T1 Where ("+FDate+" >= T1.""U_FrmDate"" And "+FDate+" <= T1.""U_ToDate"")   Or ("+TDt+" >= T1.""U_FrmDate"" And '"+TDt+"' <= T1.""U_ToDate"")";
                string _str_Query = @"Call ""IK_SOSEQ_DATVALD"" ('" + FDate.ToString("yyyyMMdd") + "','" + TDate.ToString("yyyyMMdd") + "')";

             HanaCommand hanaCom1 = new HanaCommand(_str_Query, hanaCon);
            HanaDataReader hanaReader1 = hanaCom1.ExecuteReader();
            hanaReader1.Read();
            if (hanaReader1.HasRows)
            {
                MessageBox.Show("Already Sequenced for these dates");
                return;
            }

                                       
            }


            
            string strFrom = Convert.ToDateTime(txtFrom.Value).ToString("yyyyMMdd");
            string strTo = Convert.ToDateTime(txtTo.Value).ToString("yyyyMMdd"); 

            string StrSql = @"select ""DocNum"" SONo,""CardCode"",""CardName""  from ORDR where ""DocDate"" between '" + strFrom + "' and '" + strTo + "' ";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
            visableColumnsCount = 0;
            columnWidth = 0;
            var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);

            var commandBuilder = new HanaCommandBuilder(dataAdapter);
            var ds = new DataSet();
            dataAdapter.Fill(ds);
            dgvSOSeq.ReadOnly = true;
            dgvSOSeq.DataSource = ds.Tables[0];
            dgvSOSeq.AutoResizeColumns();
            foreach (DataGridViewRow row in dgvSOSeq.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
          
            dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            }
            else
            {
                 MessageBox.Show("Click Add new to Populate ");
            }
        }

        private void btnCan_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure To Exit ?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                connectCompany("192.168.101.132:30015", "BLVL_DEMO", "manager", "1234", "SYSTEM", "Blvl12345");
                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralData oChild;
                SAPbobsCOM.GeneralDataCollection oChildren;
                SAPbobsCOM.GeneralDataParams oGeneralParams;
                oCompService = oCompany.GetCompanyService();
                oCompany.StartTransaction();
                oGeneralService = oCompService.GetGeneralService("IK_SOSQ");
               
                if (btnAdd.Text == "Add")
                {
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
          
                oGeneralData.SetProperty("DocNum", txtDocNum.Text);
              
                oGeneralData.SetProperty("U_FrmDate", txtFrom.Value);
                oGeneralData.SetProperty("U_ToDate", txtTo.Value);
               
                oChildren = oGeneralData.Child("IK_OSQ1");

                foreach (DataGridViewRow row in dgvSOSeq.Rows)
                {
                    oChild = oChildren.Add();

                    oChild.SetProperty("U_SONo", row.Cells[0].Value.ToString());
                    oChild.SetProperty("U_CardCode", row.Cells[1].Value.ToString());
                    oChild.SetProperty("U_CardName", row.Cells[2].Value.ToString());
                   
                }
               
                oGeneralService.Add(oGeneralData);
                if (oCompany.InTransaction)
                {
                    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    MessageBox.Show("Successfully Added ");

                    btnAdd.Text = "Add";
                    dgvSOSeq.DataSource = null;
                    dgvSOSeq.Rows.Clear();
                    dgvSOSeq.Refresh();
                    txtFrom.Text = DateTime.Now.ToString();
                    txtTo.Text = DateTime.Now.ToString();
                    string StrSql = @"select max(ifnull(""DocEntry"",0)) +1 DocEntry  from ""@IK_SOSQ"" ";
                    HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
                    HanaDataReader hanaReader = hanaCom.ExecuteReader();
                    hanaReader.Read();
                    txtDocNum.Text = hanaReader["DocEntry"].ToString();
                }
                else
                {
                
                      oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }
                }
                else if (btnAdd.Text == "Update")
                {
                    oGeneralService = oCompService.GetGeneralService("IK_SOSQ");
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralParams = ((SAPbobsCOM.GeneralDataParams)(oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));
                    oGeneralParams.SetProperty("DocEntry", txtDocNum.Text);                  
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                   // oGeneralData.SetProperty("DocNum", txtDocNum.Text);
                 
                    oGeneralData.SetProperty("U_FrmDate", txtFrom.Value);
                    oGeneralData.SetProperty("U_ToDate", txtTo.Value);
               
                    oChildren = oGeneralData.Child("IK_OSQ1");
                    int I = 0;
                    foreach (DataGridViewRow row in dgvSOSeq.Rows)
                    {
                        I = I+1;
                        oChild = oChildren.Item(I - 1);
                        string strDocNum = oChild.GetProperty("U_SONo");
                        oChild.SetProperty("U_SONo", row.Cells[0].Value.ToString());
                        oChild.SetProperty("U_CardCode", row.Cells[1].Value.ToString());
                        oChild.SetProperty("U_CardName", row.Cells[2].Value.ToString());
                        // oChild.SetProperty("U_DocTotal", row.Cells[2].Value);
                        
                    }

                    oGeneralService.Update(oGeneralData);
                    if (oCompany.InTransaction)
                    {
                        oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        MessageBox.Show("Successfully Updated");
                        btnAdd.Text = "Ok";

                    }
                    else
                    {

                        oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                }
                else if (btnAdd.Text == "Ok")
                {
                    Application.Exit();
                }
                  
            }
            catch
            {
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            string StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" = (select max( ""DocEntry"")   from ""@IK_SOSQ"") ";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
            hanaReader.Read();
            txtDocNum.Text = hanaReader["DocEntry"].ToString();
            txtFrom.Text = hanaReader["U_FrmDate"].ToString();
            txtTo.Text = hanaReader["U_ToDate"].ToString();

            var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
            var commandBuilder = new HanaCommandBuilder(dataAdapter);
            var ds = new DataSet();
            dataAdapter.Fill(ds);
            dgvSOSeq.ReadOnly = true;
            dgvSOSeq.DataSource = ds.Tables[0];
            dgvSOSeq.AutoResizeColumns();
            dgvSOSeq.Columns.Remove("DocEntry");
            dgvSOSeq.Columns.Remove("U_FrmDate");
            dgvSOSeq.Columns.Remove("U_ToDate");
            btnAdd.Text = "Ok";
            foreach (DataGridViewRow row in dgvSOSeq.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }

            dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnFirst_Click(object sender, EventArgs e)
        {

            string StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" = (select Top 1 ""DocEntry""   from ""@IK_SOSQ"") ";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
            hanaReader.Read();
            txtDocNum.Text = hanaReader["DocEntry"].ToString();
            txtFrom.Text = hanaReader["U_FrmDate"].ToString();
            txtTo.Text = hanaReader["U_ToDate"].ToString();
            btnAdd.Text = "Ok";
            var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
            var commandBuilder = new HanaCommandBuilder(dataAdapter);
            var ds = new DataSet();
            dataAdapter.Fill(ds);
            dgvSOSeq.ReadOnly = true;
            dgvSOSeq.DataSource = ds.Tables[0];
            dgvSOSeq.AutoResizeColumns();
            dgvSOSeq.Columns.Remove("DocEntry");
            dgvSOSeq.Columns.Remove("U_FrmDate");
            dgvSOSeq.Columns.Remove("U_ToDate");
            foreach (DataGridViewRow row in dgvSOSeq.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
          
            dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            
        }

        private void btnPrvs_Click(object sender, EventArgs e)
        {
            string StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" = " + Convert.ToInt16(txtDocNum.Text) + " -1";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
            hanaReader.Read();
            if (hanaReader.HasRows)
            {
                txtDocNum.Text = hanaReader["DocEntry"].ToString();
                txtFrom.Text = hanaReader["U_FrmDate"].ToString();
                txtTo.Text = hanaReader["U_ToDate"].ToString();
                btnAdd.Text = "Ok";
                var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
                var commandBuilder = new HanaCommandBuilder(dataAdapter);
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                dgvSOSeq.ReadOnly = true;
                dgvSOSeq.DataSource = ds.Tables[0];
                dgvSOSeq.AutoResizeColumns();
                dgvSOSeq.Columns.Remove("DocEntry");
                dgvSOSeq.Columns.Remove("U_FrmDate");
                dgvSOSeq.Columns.Remove("U_ToDate");
                btnAdd.Text = "Ok";
                foreach (DataGridViewRow row in dgvSOSeq.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }

                dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            }
            else
            {
                StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" =  (select Top 1 ""DocEntry""   from ""@IK_SOSQ"") ";
                hanaCom = new HanaCommand(StrSql, hanaCon);
                hanaReader = hanaCom.ExecuteReader();
                hanaReader.Read();

                txtDocNum.Text = hanaReader["DocEntry"].ToString();
                txtFrom.Text = hanaReader["U_FrmDate"].ToString();
                txtTo.Text = hanaReader["U_ToDate"].ToString();
                btnAdd.Text = "Ok";
                var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
                var commandBuilder = new HanaCommandBuilder(dataAdapter);
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                dgvSOSeq.ReadOnly = true;
                dgvSOSeq.DataSource = ds.Tables[0];
                dgvSOSeq.AutoResizeColumns();
                dgvSOSeq.Columns.Remove("DocEntry");
                dgvSOSeq.Columns.Remove("U_FrmDate");
                dgvSOSeq.Columns.Remove("U_ToDate");
                foreach (DataGridViewRow row in dgvSOSeq.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }

                dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            string StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" = " + Convert.ToInt16(txtDocNum.Text) + " +1";
            HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
            HanaDataReader hanaReader = hanaCom.ExecuteReader();
            hanaReader.Read();
           // int intFldCount =  hanaReader.FieldCount;
            if (hanaReader.HasRows)
            {
                txtDocNum.Text = hanaReader["DocEntry"].ToString();
                txtFrom.Text = hanaReader["U_FrmDate"].ToString();
                txtTo.Text = hanaReader["U_ToDate"].ToString();
                btnAdd.Text = "Ok";
                var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
                var commandBuilder = new HanaCommandBuilder(dataAdapter);
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                dgvSOSeq.ReadOnly = true;
                dgvSOSeq.DataSource = ds.Tables[0];
                dgvSOSeq.AutoResizeColumns();
                dgvSOSeq.Columns.Remove("DocEntry");
                dgvSOSeq.Columns.Remove("U_FrmDate");
                dgvSOSeq.Columns.Remove("U_ToDate");
                foreach (DataGridViewRow row in dgvSOSeq.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }

                dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            }
            else
            {
                StrSql = @"select T1.""DocEntry"",T1.""U_FrmDate"",T1.""U_ToDate"",T2.""U_SONo"" SoNo,T2.""U_CardCode"" CardCode,T2.""U_CardName"" CardName from ""@IK_SOSQ"" T1 inner join ""@IK_OSQ1"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T1.""DocEntry"" =  (select max( ""DocEntry"")   from ""@IK_SOSQ"") ";
                 hanaCom = new HanaCommand(StrSql, hanaCon);
                 hanaReader = hanaCom.ExecuteReader();
                 hanaReader.Read();
                 btnAdd.Text = "Ok";
                 txtDocNum.Text = hanaReader["DocEntry"].ToString();
                 txtFrom.Text = hanaReader["U_FrmDate"].ToString();
                 txtTo.Text = hanaReader["U_ToDate"].ToString();

                 var dataAdapter = new HanaDataAdapter(StrSql, hanaCon);
                 var commandBuilder = new HanaCommandBuilder(dataAdapter);
                 var ds = new DataSet();
                 dataAdapter.Fill(ds);
                 dgvSOSeq.ReadOnly = true;
                 dgvSOSeq.DataSource = ds.Tables[0];
                 dgvSOSeq.AutoResizeColumns();
                 dgvSOSeq.Columns.Remove("DocEntry");
                 dgvSOSeq.Columns.Remove("U_FrmDate");
                 dgvSOSeq.Columns.Remove("U_ToDate");
                 foreach (DataGridViewRow row in dgvSOSeq.Rows)
                 {
                     row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                 }

                 dgvSOSeq.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
         
            }
        }

        private void btnAddnew_Click(object sender, EventArgs e)
        {
                btnAdd.Text = "Add";
                dgvSOSeq.DataSource = null;
                dgvSOSeq.Rows.Clear();
                dgvSOSeq.Refresh();
                txtFrom.Text = DateTime.Now.ToString();
                txtTo.Text = DateTime.Now.ToString(); 
                string StrSql = @"select max(ifnull(""DocEntry"",0)) +1 DocEntry  from ""@IK_SOSQ"" ";
                HanaCommand hanaCom = new HanaCommand(StrSql, hanaCon);
                HanaDataReader hanaReader = hanaCom.ExecuteReader();
                hanaReader.Read();
                txtDocNum.Text = hanaReader["DocEntry"].ToString();
         

        }

        private void dgvSOSeq_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
