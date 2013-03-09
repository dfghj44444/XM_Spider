using System;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using HouseInfoExtractorLib;
using GeneralClass.SqlBaseLib;
using System.Data;
using System.Data.SqlClient ;
using System.Text ;
using System.Threading ;
using EduWebSites;

namespace ExtractDemo
{
	enum ExtractorState{Stop,Pause,Processing};
	/// <summary>
	/// Form1 ��ժҪ˵����
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		#region  �û�����ı���
		public  XmlNodeList keyWordsLists;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage extractPage;
		private System.Windows.Forms.TabPage logPage;
		private System.Windows.Forms.Button btnExtractor;
		private System.Windows.Forms.StatusBar statusBar1;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnPause;
	    //������ȡ�߳�
		private System.Data.SqlClient .SqlConnection conn ;
		private System.Data.SqlClient .SqlDataAdapter adapter;	
		private System.Windows.Forms.ListBox listInfo;
		private System.Windows.Forms.Label lblProcessedNum;
		private System.Windows.Forms.Label lblStartUp;
		private System.Windows.Forms.TextBox tbStartUp;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label lblStart;
		private System.Windows.Forms.TextBox tbStart;
		private System.Windows.Forms.Label label1;
		private ExtractorState m_extractorState = ExtractorState.Stop ;
		private string[] urlList = new string[200];       //�洢��Ҫ�����URL\

		// Url���ݿ����������
		private string UrlIP ;
		private string UrlDB ;
		private string UrlUser;
		private string UrlPwd ;
		
		
		// ��ȡ������ݿ����������
		private string InsertIP;
		private string InsertDB ;
		private string InsertUser ;
		private string InsertPwd ;
		
		private int ReadUrl=0;                        //һ�ζ�ȡURL��
		private ArrayList listURL = new ArrayList ();
		private System.Windows.Forms.Timer timer2;     //���ڴ��URL�б�
		private ArrayList Hosts  = new ArrayList();	// ��ǰ����� URL �������������ڿ��ƶ�ʱ���ڲ�����ͬһ��վ��
		private int UrlID        = 0;				// ��ǰ����� URL �� listURL �е�λ�ú�
		private int MaxHosts     = 3;				// ��󻺴�� Host ����
		//private string CurUrl = null;                 //��ǰ��Ҫ�����URL
		private int ThreadNumber = 1;               //��ǰ�߳���
		private ArrayList houseInfoList = new ArrayList ();    //�洢�����ؼ���Ϣ�б�
		//private HouseInfoExtractorLib.HouseInfo houseInfo;  //�洢�����ؼ���Ϣ�Ľṹ
		private HouseInfoExtractorLib.HouseInfoExtractor InfoExctractor = new HouseInfoExtractor ();  //������Ϣ��ȡ��
		private int maxHouseInfoNumber = 1;
		private System.Windows.Forms.TabPage configPage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox autoRunCB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericThreadNums;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnInsert;
		private System.Windows.Forms.TextBox tbInsertIP;
		private System.Windows.Forms.TextBox tbInsertDB;
		private System.Windows.Forms.TextBox tbInsertPwd;
		private System.Windows.Forms.TextBox tbInsertUser;
		private System.Windows.Forms.Button btnUrl;
		private System.Windows.Forms.TextBox txtUrlIP;
		private System.Windows.Forms.TextBox txtUrlDB;
		private System.Windows.Forms.TextBox txtUrlPwd;
		private System.Windows.Forms.TextBox txtUrlUser;
		private System.Windows.Forms.Button btnOther;
		private System.Windows.Forms.TextBox tbUseable;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbTotal;                //�����ؼ���Ϣ�б�һ�δ洢���ݿ���������
		private int num = 0;
		private System.Windows.Forms.Button btnLogClear;
		private System.Windows.Forms.Button btnSaveLog;
		private System.Windows.Forms.ListView lvLog;
		private System.Windows.Forms.ColumnHeader url;
		private System.Windows.Forms.ColumnHeader errInfo;
		private System.Windows.Forms.ColumnHeader time;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown NumOfRead;                       //��¼�Ѵ����URL��
		private int total = 0;					  //��������ҳ��
		private int sameRec = 0;  //�ظ�ҳ��
		private Hashtable hashtel = new Hashtable ();               //�洢�н�绰����
		private int agentNum = 0;
		private System.Windows.Forms.TextBox agentTb;
		private System.Windows.Forms.TextBox sameTb;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label14;                 //���н�
		private DataBase dataBase ;               //��URL������ݿ���
		#endregion

		public MainForm()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();
			XmlDocument doc = new XmlDocument();
			FileStream myStream = new FileStream("keywords.xml",FileMode.Open);
			//doc.Load(myStream);
			//this.keyWordsLists = doc.GetElementsByTagName("keyword");
		
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.extractPage = new System.Windows.Forms.TabPage();
			this.agentTb = new System.Windows.Forms.TextBox();
			this.tbTotal = new System.Windows.Forms.TextBox();
			this.tbStart = new System.Windows.Forms.TextBox();
			this.lblStart = new System.Windows.Forms.Label();
			this.tbStartUp = new System.Windows.Forms.TextBox();
			this.lblStartUp = new System.Windows.Forms.Label();
			this.tbUseable = new System.Windows.Forms.TextBox();
			this.lblProcessedNum = new System.Windows.Forms.Label();
			this.btnStop = new System.Windows.Forms.Button();
			this.listInfo = new System.Windows.Forms.ListBox();
			this.btnPause = new System.Windows.Forms.Button();
			this.btnExtractor = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.configPage = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.NumOfRead = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.btnOther = new System.Windows.Forms.Button();
			this.numericThreadNums = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.autoRunCB = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnInsert = new System.Windows.Forms.Button();
			this.tbInsertIP = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbInsertDB = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbInsertPwd = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbInsertUser = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnUrl = new System.Windows.Forms.Button();
			this.txtUrlIP = new System.Windows.Forms.TextBox();
			this.txtUrlDB = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtUrlPwd = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtUrlUser = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.logPage = new System.Windows.Forms.TabPage();
			this.lvLog = new System.Windows.Forms.ListView();
			this.url = new System.Windows.Forms.ColumnHeader();
			this.errInfo = new System.Windows.Forms.ColumnHeader();
			this.time = new System.Windows.Forms.ColumnHeader();
			this.btnLogClear = new System.Windows.Forms.Button();
			this.btnSaveLog = new System.Windows.Forms.Button();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			this.sameTb = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.extractPage.SuspendLayout();
			this.configPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumOfRead)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericThreadNums)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.logPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.extractPage);
			this.tabControl1.Controls.Add(this.configPage);
			this.tabControl1.Controls.Add(this.logPage);
			this.tabControl1.Location = new System.Drawing.Point(8, 72);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(656, 368);
			this.tabControl1.TabIndex = 12;
			// 
			// extractPage
			// 
			this.extractPage.Controls.Add(this.label14);
			this.extractPage.Controls.Add(this.label9);
			this.extractPage.Controls.Add(this.sameTb);
			this.extractPage.Controls.Add(this.agentTb);
			this.extractPage.Controls.Add(this.tbTotal);
			this.extractPage.Controls.Add(this.tbStart);
			this.extractPage.Controls.Add(this.lblStart);
			this.extractPage.Controls.Add(this.tbStartUp);
			this.extractPage.Controls.Add(this.lblStartUp);
			this.extractPage.Controls.Add(this.tbUseable);
			this.extractPage.Controls.Add(this.lblProcessedNum);
			this.extractPage.Controls.Add(this.btnStop);
			this.extractPage.Controls.Add(this.listInfo);
			this.extractPage.Controls.Add(this.btnPause);
			this.extractPage.Controls.Add(this.btnExtractor);
			this.extractPage.Controls.Add(this.label7);
			this.extractPage.Location = new System.Drawing.Point(4, 21);
			this.extractPage.Name = "extractPage";
			this.extractPage.Size = new System.Drawing.Size(648, 343);
			this.extractPage.TabIndex = 0;
			this.extractPage.Text = "��Ϣ��ȡ";
			this.extractPage.Click += new System.EventHandler(this.extractPage_Click);
			// 
			// agentTb
			// 
			this.agentTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.agentTb.Location = new System.Drawing.Point(568, 208);
			this.agentTb.Name = "agentTb";
			this.agentTb.Size = new System.Drawing.Size(56, 21);
			this.agentTb.TabIndex = 23;
			this.agentTb.Text = "";
			// 
			// tbTotal
			// 
			this.tbTotal.BackColor = System.Drawing.SystemColors.HighlightText;
			this.tbTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbTotal.Enabled = false;
			this.tbTotal.ForeColor = System.Drawing.Color.Red;
			this.tbTotal.Location = new System.Drawing.Point(520, 32);
			this.tbTotal.Name = "tbTotal";
			this.tbTotal.Size = new System.Drawing.Size(120, 21);
			this.tbTotal.TabIndex = 22;
			this.tbTotal.Text = "";
			this.tbTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbStart
			// 
			this.tbStart.BackColor = System.Drawing.SystemColors.HighlightText;
			this.tbStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbStart.Enabled = false;
			this.tbStart.ForeColor = System.Drawing.Color.Red;
			this.tbStart.Location = new System.Drawing.Point(520, 80);
			this.tbStart.Name = "tbStart";
			this.tbStart.Size = new System.Drawing.Size(120, 21);
			this.tbStart.TabIndex = 21;
			this.tbStart.Text = "";
			this.tbStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lblStart
			// 
			this.lblStart.Location = new System.Drawing.Point(520, 64);
			this.lblStart.Name = "lblStart";
			this.lblStart.TabIndex = 20;
			this.lblStart.Text = "ϵͳ����ʱ�䣺";
			// 
			// tbStartUp
			// 
			this.tbStartUp.BackColor = System.Drawing.SystemColors.HighlightText;
			this.tbStartUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbStartUp.Enabled = false;
			this.tbStartUp.ForeColor = System.Drawing.Color.Red;
			this.tbStartUp.Location = new System.Drawing.Point(520, 128);
			this.tbStartUp.Name = "tbStartUp";
			this.tbStartUp.Size = new System.Drawing.Size(120, 21);
			this.tbStartUp.TabIndex = 19;
			this.tbStartUp.Text = "";
			this.tbStartUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lblStartUp
			// 
			this.lblStartUp.Location = new System.Drawing.Point(520, 112);
			this.lblStartUp.Name = "lblStartUp";
			this.lblStartUp.TabIndex = 18;
			this.lblStartUp.Text = "������ʱ�䣺";
			// 
			// tbUseable
			// 
			this.tbUseable.BackColor = System.Drawing.SystemColors.HighlightText;
			this.tbUseable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbUseable.Enabled = false;
			this.tbUseable.ForeColor = System.Drawing.Color.Red;
			this.tbUseable.Location = new System.Drawing.Point(520, 176);
			this.tbUseable.Name = "tbUseable";
			this.tbUseable.Size = new System.Drawing.Size(120, 21);
			this.tbUseable.TabIndex = 17;
			this.tbUseable.Text = "";
			this.tbUseable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lblProcessedNum
			// 
			this.lblProcessedNum.Location = new System.Drawing.Point(520, 160);
			this.lblProcessedNum.Name = "lblProcessedNum";
			this.lblProcessedNum.TabIndex = 16;
			this.lblProcessedNum.Text = "��Ч��ҳ����";
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(520, 320);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(96, 24);
			this.btnStop.TabIndex = 15;
			this.btnStop.Text = "ֹͣ";
			// 
			// listInfo
			// 
			this.listInfo.ItemHeight = 12;
			this.listInfo.Location = new System.Drawing.Point(16, 19);
			this.listInfo.Name = "listInfo";
			this.listInfo.Size = new System.Drawing.Size(472, 304);
			this.listInfo.TabIndex = 14;
			// 
			// btnPause
			// 
			this.btnPause.Enabled = false;
			this.btnPause.Location = new System.Drawing.Point(520, 288);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(96, 23);
			this.btnPause.TabIndex = 13;
			this.btnPause.Text = "��ͣ";
			// 
			// btnExtractor
			// 
			this.btnExtractor.Location = new System.Drawing.Point(520, 256);
			this.btnExtractor.Name = "btnExtractor";
			this.btnExtractor.Size = new System.Drawing.Size(96, 23);
			this.btnExtractor.TabIndex = 12;
			this.btnExtractor.Text = "��ʼ";
			this.btnExtractor.Click += new System.EventHandler(this.btnExtractor_Click_1);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(520, 8);
			this.label7.Name = "label7";
			this.label7.TabIndex = 17;
			this.label7.Text = "��������ҳ����";
			// 
			// configPage
			// 
			this.configPage.Controls.Add(this.groupBox3);
			this.configPage.Controls.Add(this.groupBox2);
			this.configPage.Controls.Add(this.groupBox1);
			this.configPage.Location = new System.Drawing.Point(4, 21);
			this.configPage.Name = "configPage";
			this.configPage.Size = new System.Drawing.Size(648, 343);
			this.configPage.TabIndex = 2;
			this.configPage.Text = "��������";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.NumOfRead);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.btnOther);
			this.groupBox3.Controls.Add(this.numericThreadNums);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.autoRunCB);
			this.groupBox3.Location = new System.Drawing.Point(456, 16);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(184, 160);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "��������������Ϣ";
			// 
			// NumOfRead
			// 
			this.NumOfRead.Location = new System.Drawing.Point(112, 88);
			this.NumOfRead.Maximum = new System.Decimal(new int[] {
																	  300,
																	  0,
																	  0,
																	  0});
			this.NumOfRead.Name = "NumOfRead";
			this.NumOfRead.Size = new System.Drawing.Size(64, 21);
			this.NumOfRead.TabIndex = 6;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(32, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(88, 24);
			this.label8.TabIndex = 5;
			this.label8.Text = "url��ȡ������";
			// 
			// btnOther
			// 
			this.btnOther.Enabled = false;
			this.btnOther.Location = new System.Drawing.Point(80, 120);
			this.btnOther.Name = "btnOther";
			this.btnOther.Size = new System.Drawing.Size(96, 24);
			this.btnOther.TabIndex = 4;
			this.btnOther.Text = "��������";
			this.btnOther.Click += new System.EventHandler(this.btnOther_Click);
			// 
			// numericThreadNums
			// 
			this.numericThreadNums.Location = new System.Drawing.Point(128, 56);
			this.numericThreadNums.Minimum = new System.Decimal(new int[] {
																			  8,
																			  0,
																			  0,
																			  0});
			this.numericThreadNums.Name = "numericThreadNums";
			this.numericThreadNums.Size = new System.Drawing.Size(48, 21);
			this.numericThreadNums.TabIndex = 3;
			this.numericThreadNums.Value = new System.Decimal(new int[] {
																			8,
																			0,
																			0,
																			0});
			this.numericThreadNums.ValueChanged += new System.EventHandler(this.numericThreadNums_ValueChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "����߳�����";
			// 
			// autoRunCB
			// 
			this.autoRunCB.Location = new System.Drawing.Point(32, 24);
			this.autoRunCB.Name = "autoRunCB";
			this.autoRunCB.Size = new System.Drawing.Size(144, 24);
			this.autoRunCB.TabIndex = 0;
			this.autoRunCB.Text = "�Ƿ񿪻�ʱ����";
			this.autoRunCB.CheckedChanged += new System.EventHandler(this.autoRunCB_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnInsert);
			this.groupBox2.Controls.Add(this.tbInsertIP);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.tbInsertDB);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.tbInsertPwd);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.tbInsertUser);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Location = new System.Drawing.Point(16, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(424, 128);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "���ô洢�����";
			// 
			// btnInsert
			// 
			this.btnInsert.Enabled = false;
			this.btnInsert.Location = new System.Drawing.Point(322, 86);
			this.btnInsert.Name = "btnInsert";
			this.btnInsert.Size = new System.Drawing.Size(92, 28);
			this.btnInsert.TabIndex = 22;
			this.btnInsert.Text = "��������";
			this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
			// 
			// tbInsertIP
			// 
			this.tbInsertIP.Location = new System.Drawing.Point(74, 22);
			this.tbInsertIP.Name = "tbInsertIP";
			this.tbInsertIP.Size = new System.Drawing.Size(104, 21);
			this.tbInsertIP.TabIndex = 17;
			this.tbInsertIP.Text = "";
			this.tbInsertIP.TextChanged += new System.EventHandler(this.tbInsertIP_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 20);
			this.label3.TabIndex = 18;
			this.label3.Text = "��������";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbInsertDB
			// 
			this.tbInsertDB.Location = new System.Drawing.Point(250, 22);
			this.tbInsertDB.Name = "tbInsertDB";
			this.tbInsertDB.Size = new System.Drawing.Size(104, 21);
			this.tbInsertDB.TabIndex = 19;
			this.tbInsertDB.Text = "";
			this.tbInsertDB.TextChanged += new System.EventHandler(this.tbInsertDB_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(186, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 20);
			this.label4.TabIndex = 14;
			this.label4.Text = "���ݿ���";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbInsertPwd
			// 
			this.tbInsertPwd.Location = new System.Drawing.Point(250, 54);
			this.tbInsertPwd.Name = "tbInsertPwd";
			this.tbInsertPwd.PasswordChar = '*';
			this.tbInsertPwd.Size = new System.Drawing.Size(104, 21);
			this.tbInsertPwd.TabIndex = 21;
			this.tbInsertPwd.Text = "";
			this.tbInsertPwd.TextChanged += new System.EventHandler(this.tbInsertPwd_TextChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(186, 54);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 20);
			this.label5.TabIndex = 15;
			this.label5.Text = "����";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbInsertUser
			// 
			this.tbInsertUser.Location = new System.Drawing.Point(74, 54);
			this.tbInsertUser.Name = "tbInsertUser";
			this.tbInsertUser.Size = new System.Drawing.Size(104, 21);
			this.tbInsertUser.TabIndex = 20;
			this.tbInsertUser.Text = "";
			this.tbInsertUser.TextChanged += new System.EventHandler(this.tbInsertUser_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 54);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 20);
			this.label6.TabIndex = 16;
			this.label6.Text = "�û���";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnUrl);
			this.groupBox1.Controls.Add(this.txtUrlIP);
			this.groupBox1.Controls.Add(this.txtUrlDB);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.txtUrlPwd);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.txtUrlUser);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(424, 120);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "����url�����";
			// 
			// btnUrl
			// 
			this.btnUrl.Enabled = false;
			this.btnUrl.Location = new System.Drawing.Point(320, 80);
			this.btnUrl.Name = "btnUrl";
			this.btnUrl.Size = new System.Drawing.Size(92, 28);
			this.btnUrl.TabIndex = 13;
			this.btnUrl.Text = "��������";
			this.btnUrl.Click += new System.EventHandler(this.btnUrl_Click);
			// 
			// txtUrlIP
			// 
			this.txtUrlIP.Location = new System.Drawing.Point(80, 16);
			this.txtUrlIP.Name = "txtUrlIP";
			this.txtUrlIP.Size = new System.Drawing.Size(104, 21);
			this.txtUrlIP.TabIndex = 8;
			this.txtUrlIP.Text = "";
			this.txtUrlIP.TextChanged += new System.EventHandler(this.txtUrlIP_TextChanged);
			// 
			// txtUrlDB
			// 
			this.txtUrlDB.Location = new System.Drawing.Point(256, 16);
			this.txtUrlDB.Name = "txtUrlDB";
			this.txtUrlDB.Size = new System.Drawing.Size(104, 21);
			this.txtUrlDB.TabIndex = 10;
			this.txtUrlDB.Text = "";
			this.txtUrlDB.TextChanged += new System.EventHandler(this.txtUrlDB_TextChanged);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(192, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(60, 20);
			this.label11.TabIndex = 5;
			this.label11.Text = "���ݿ���";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtUrlPwd
			// 
			this.txtUrlPwd.Location = new System.Drawing.Point(256, 48);
			this.txtUrlPwd.Name = "txtUrlPwd";
			this.txtUrlPwd.PasswordChar = '*';
			this.txtUrlPwd.Size = new System.Drawing.Size(104, 21);
			this.txtUrlPwd.TabIndex = 12;
			this.txtUrlPwd.Text = "";
			this.txtUrlPwd.TextChanged += new System.EventHandler(this.txtUrlPwd_TextChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(192, 48);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(60, 20);
			this.label12.TabIndex = 6;
			this.label12.Text = "����";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtUrlUser
			// 
			this.txtUrlUser.Location = new System.Drawing.Point(80, 48);
			this.txtUrlUser.Name = "txtUrlUser";
			this.txtUrlUser.Size = new System.Drawing.Size(104, 21);
			this.txtUrlUser.TabIndex = 11;
			this.txtUrlUser.Text = "";
			this.txtUrlUser.TextChanged += new System.EventHandler(this.txtUrlUser_TextChanged);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(16, 16);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 20);
			this.label10.TabIndex = 9;
			this.label10.Text = "��������";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(16, 48);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(48, 20);
			this.label13.TabIndex = 7;
			this.label13.Text = "�û���";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// logPage
			// 
			this.logPage.Controls.Add(this.lvLog);
			this.logPage.Controls.Add(this.btnLogClear);
			this.logPage.Controls.Add(this.btnSaveLog);
			this.logPage.Location = new System.Drawing.Point(4, 21);
			this.logPage.Name = "logPage";
			this.logPage.Size = new System.Drawing.Size(648, 343);
			this.logPage.TabIndex = 1;
			this.logPage.Text = "��־��Ϣ";
			// 
			// lvLog
			// 
			this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					this.url,
																					this.errInfo,
																					this.time});
			this.lvLog.Location = new System.Drawing.Point(8, 16);
			this.lvLog.Name = "lvLog";
			this.lvLog.Size = new System.Drawing.Size(632, 240);
			this.lvLog.TabIndex = 18;
			this.lvLog.View = System.Windows.Forms.View.Details;
			// 
			// url
			// 
			this.url.Text = "URL";
			this.url.Width = 150;
			// 
			// errInfo
			// 
			this.errInfo.Text = "������Ϣ";
			this.errInfo.Width = 412;
			// 
			// time
			// 
			this.time.Text = "����ʱ��";
			this.time.Width = 61;
			// 
			// btnLogClear
			// 
			this.btnLogClear.Location = new System.Drawing.Point(400, 272);
			this.btnLogClear.Name = "btnLogClear";
			this.btnLogClear.Size = new System.Drawing.Size(88, 23);
			this.btnLogClear.TabIndex = 17;
			this.btnLogClear.Text = "��������¼";
			this.btnLogClear.Click += new System.EventHandler(this.btnLogClear_Click);
			// 
			// btnSaveLog
			// 
			this.btnSaveLog.Location = new System.Drawing.Point(536, 272);
			this.btnSaveLog.Name = "btnSaveLog";
			this.btnSaveLog.Size = new System.Drawing.Size(88, 23);
			this.btnSaveLog.TabIndex = 16;
			this.btnSaveLog.Text = "��������¼";
			this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 447);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(680, 22);
			this.statusBar1.TabIndex = 14;
			this.statusBar1.Text = "statusBar1";
			// 
			// timer1
			// 
			this.timer1.Interval = 3000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.Window;
			this.label1.Font = new System.Drawing.Font("����_GB2312", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label1.Location = new System.Drawing.Point(136, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(376, 48);
			this.label1.TabIndex = 15;
			this.label1.Text = "��Ϣ��ȡ��";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timer2
			// 
			this.timer2.Interval = 1000;
			this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
			// 
			// sameTb
			// 
			this.sameTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sameTb.Location = new System.Drawing.Point(568, 232);
			this.sameTb.Name = "sameTb";
			this.sameTb.Size = new System.Drawing.Size(56, 21);
			this.sameTb.TabIndex = 24;
			this.sameTb.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(512, 240);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(32, 16);
			this.label9.TabIndex = 25;
			this.label9.Text = "�ظ�";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(512, 216);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(48, 16);
			this.label14.TabIndex = 26;
			this.label14.Text = "���н�";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(680, 469);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.tabControl1);
			this.Name = "MainForm";
			this.Text = "��Ϣ��ȡ��";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.tabControl1.ResumeLayout(false);
			this.extractPage.ResumeLayout(false);
			this.configPage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumOfRead)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericThreadNums)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.logPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region ���������
		/// <summary>
		/// Ӧ�ó��������ڵ㡣
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}
		#endregion

		
		#region ҳ������
		/// <summary>
		/// ��ȡָ��url��Դ��
		/// </summary>
		/// <param name="url">ָ����url</param>
		/// <returns>ָ��ҳ���Դ��</returns>
		private string downLoad(string url)
		{
			GetPage gp = new GetPage();
			gp.Url = url;
			//System.Windows.Forms.Application.DoEvents();
			string content=null;
			try
			{
				content = gp.GetHtml().ToLower();
			}
			catch(Exception e)
			{
				this.statusBar1 .Text = "downLoad:������ҳ��ʱ��" + e.Message  ;
				content = "";
			}	
			//System.Windows.Forms.Application.DoEvents();
			return content;
		}
		#endregion

		private void btnExtractor_Click_1(object sender, System.EventArgs e)
		{
			this.timer1.Enabled =true;
			this.timer2 .Enabled = true;
			this.tbStart.Text = System.DateTime.Now.ToString();
			this.readUrl();
		}

		/// <summary>
		/// readUrl()�����ݿ��ж�ȡ1000����¼�����û���������򷵻�false
		/// </summary>
		/// <returns>bool</returns>
		private void  readUrl()
		{
			this.statusBar1 .Text = "readUrl:��ȡURL�б�";
			try
			{
				
				//System.Data.SqlClient.SqlDataReader  myReader;
				DataTable myReader = new DataTable();
				//һ�ζ�ȡURL��������(llq)
				//string SQL = "select top "+ReadUrl.ToString ()+" url,id,cityid,urltype from Urltest where isProcess is null order by id asc";					
				string SQL = "select top "+ReadUrl.ToString ()+" url,id,cityid,urltype from Url where isProcess is null order by id asc";					
				
				myReader = SqlBase.ExecuteDataSet(SQL,System.Data.CommandType.Text ).Tables [0];

				/*�ж��Ƿ���URL*/
				if(myReader.Rows.Count == 0)//if(!myReader.Read ())
				{
					this.statusBar1.Text = "�����Ѵ�����,�ȴ��µ�URL����";

					//�������ȴ�
					//this.timer1 .Enabled = false;
					this.timer2 .Enabled = false;

					//�ȴ���ʱ����Ϊ1000��
					this.timer1.Interval = 10000;
					return;
				}
				else
				{
                    this.timer1 .Interval = 500;
					this.timer2.Enabled = true;
				}
                
				for(int i = 0 ;i<myReader.Rows .Count ;i++)//while(myReader.Read())
				{
					string cityid = myReader.Rows [i][2].ToString ();//myReader.GetInt32(2).ToString();
					string urltype = myReader.Rows [i][3].ToString ();
					listURL.Add (cityid+"��"+urltype+"��" + myReader.Rows [i][0].ToString ());
				}
				myReader.Clear();
				myReader.Dispose();
			
			}
			catch(Exception e1)
			{
				this.AddErrInfo("readUrl����",e1.Message);
				statusBar1.Text = "readUrl:�ڶ�ȡ���� URL ����ʱ��" + e1.Message.ToString();				
			}		
		}


		/// <summary>
		/// extract1()�ṩ���ؼ���ȡ
		/// </summary>
		public void extract1()
		{
			// ������ҳHTML����
			string s=null;

			string CurUrl = "";

			HouseInfoExtractorLib.HouseInfo houseInfo1 = new HouseInfo ();

			/*�����û�ж�ȡURL�����ȡ*/
			if(Hosts.Count>MaxHosts) Hosts.RemoveAt(0);
			Random rnd = new System.Random();// ���������һ����ţ� �Ӹ���ſ�ʼȡһ�� URL,�����ʱ����Ƶ������ĳһվ��
			double a = listURL.Count * rnd.NextDouble();
			UrlID = (int)a;			
			if(listURL.Count == 0){
				ThreadNumber--;
				return;
			}
            string myUrl="";
			try
			{
				lock(this)
				{
					CurUrl = listURL[UrlID].ToString();
					listURL.Remove(CurUrl);
					myUrl=CurUrl.Substring(CurUrl.LastIndexOf("��")+1);
				}
			}
			catch(Exception ex)
			{
				this.AddErrInfo(myUrl,"extractor1937:@"+ex.Message);
				this.statusBar1.Text= "extract1����ȡ�ؼ���Ϣʱ��" + ex.Message ;
				ThreadNumber--;
				return;
			}

			//string sqlT = "update urltest set isprocess='1' where url='" + myUrl+"'";
			string sqlT = "update url set isprocess='1' where url='" + myUrl+"'";
			SqlBase.ExecuteQuery (sqlT,System.Data.CommandType.Text );	
		   
			string sql="";

			try
			{
			
				GetPage myGP = new GetPage();
				myGP.Url = myUrl;
				total++;
				this.tbTotal.Text = total.ToString();
				try	{
					s = myGP.GetHtml();
				}
				catch 
				{
					statusBar1.Text = "����ʱ����������Ϣ:" + myGP.NoteMessage;
					this.AddErrInfo(myUrl,"extractor1#:"+myGP.NoteMessage);
					//sql = "update urltest set isprocess='2' where url='" + myUrl+"'";
					sql = "update url set isprocess='2' where url='" + myUrl+"'";
					
//					dataBase.SetCommandText (sql);
					lock(this)
					{
						SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
//						dataBase.Run();
					}
					ThreadNumber--;
					return;
				}
				if(s==null||s=="")	
				{
					statusBar1.Text = "������ҳ��" + myUrl +"ʱ�������� " + myGP.NoteMessage;
					ThreadNumber--;
					sql = "update url set isprocess='2' where url='" + myUrl+"'";
					//sql = "update urltest set isprocess='2' where url='" + myUrl+"'";
//					dataBase.SetCommandText (sql);//.ExecuteQuery (sql,System.Data.CommandType.Text );
					lock(this)
					{
						SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
//						dataBase.Run ();
					}
					ThreadNumber--;
					return;
				}

				/*��Ϣ��ȡ*/				
				try
				{
				lock(this)
				 {
					 houseInfo1 = InfoExctractor.extract(s,keyWordsLists);
				 }
				}
				catch(Exception e1)
				{
					ThreadNumber--;
					sql = "update url set isprocess='2' where url='"+ myUrl+"'";
					//	sql = "update urltest set isprocess='2' where url='"+ myUrl+"'";
//					dataBase.SetCommandText (sql);
					lock(this)
					{
						SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
//						dataBase.Run ();
					}					
					this.AddErrInfo(myUrl,"extractor1:@"+e1.Message);
					this.statusBar1.Text= "extract1����ȡ�ؼ���Ϣʱ��" + e1.Message ;
					return;
				}

				if(houseInfo1.linkPhone==""||houseInfo1.linkPhone==null){
					ThreadNumber--;
					sql = "update url set isprocess='2' where url='"+ myUrl+"'";
					//sql = "update urltest set isprocess='2' where url='"+ myUrl+"'";
					SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
//					dataBase.SetCommandText (sql);//.ExecuteQuery (sql,System.Data.CommandType.Text );	
//					dataBase.Run ();
					return;
				}
				/*Ϊ�Ѵ����URL��ӱ��*/
				sql = "update url set isprocess='1' where url='" + myUrl+"'";
				//sql = "update urltest set isprocess='1' where url='"+ myUrl+"'";
//				dataBase.SetCommandText (sql);
				lock(this)
				{
					SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
//					dataBase.Run ();
     			}  // --------modify by cjx----------
				int cityid = CurUrl.IndexOf("��");
				int urltype = CurUrl.LastIndexOf("��");
				houseInfo1.cityID = Convert.ToInt32(CurUrl.Substring(0,cityid));
				houseInfo1.urltype = Convert.ToInt32(CurUrl.Substring(cityid+1,urltype-cityid-1));
				houseInfo1.url = CurUrl.Substring(CurUrl.LastIndexOf("��")+1);
				lock(this)
				{              //-------------------modify by cjx----------------
					houseInfoList.Add (houseInfo1);
				}
				//num++;
				ThreadNumber--;
				//this.tbUseable.Text = num.ToString();
			}
			catch(Exception e1)
			{
				ThreadNumber--;
				sql = "update url set isprocess='2' where url='"+ myUrl+"'";
				//sql = "update urltest set isprocess='2' where url='"+ myUrl+"'";
//				dataBase.SetCommandText (sql);
//				dataBase.Run ();
				lock(this)
				{
					SqlBase.ExecuteQuery (sql,System.Data.CommandType.Text );
				}
				//SqlBase1.ExecuteQuery (sql,System.Data.CommandType.Text );
				this.AddErrInfo(myUrl,"extractor1:"+e1.Message);
				statusBar1.Text = "Spider �ڱ��������У�" + e1.Message.ToString();
			}
		}



		/// <summary>
		/// ϵͳ�ȴ�ָ��������
		/// </summary>
		/// <param name="seconds">�ȴ���ʱ������Ϊ��Ϊ��λ</param>
		private void wait(double seconds)
		{
			int TimeLength = 0;					// ʱ����
			DateTime bTime = new DateTime();	// ��ʼ��ʱʱ��
			DateTime eTime = new DateTime();	// ������ʱʱ��			
			bTime = System.DateTime.Now;

			//this.statusBar1 .Text = "Wait:���ڵȴ�";
			while(TimeLength < seconds)
			{
				eTime = System.DateTime.Now;
				TimeSpan ts = new TimeSpan(eTime.Ticks-bTime.Ticks);
				if(ts.Seconds<seconds)
				{
					Application.DoEvents();
				}
				else
					return;
				
			}				
		}

		/// <summary>
		/// Delay()����ʹ�����̵߳ȴ�
		/// </summary>
		/// <param name="iSecond">�̵߳ȴ�������</param>
		private void Delay(int iSecond)
		{
			//��ָ����ʱ�������ʱ
			//Ϊ������ֹ�����ʱ���ֶν���
			string sStatus=this.statusBar1.Text;
			for(int i=0;i<=iSecond/5;i++)
			{
				//ÿ���ӳ�5s��Ȼ������ȡ��״̬�Ƿ�ֹͣ;
				//��ֹͣ���˳�
				StringBuilder sDisplayString=new StringBuilder(">>>");
				sDisplayString.Insert(1,">>>",i+1);
				statusBar1.Text=sStatus+"��ʱ�ȴ���"+sDisplayString.ToString();
				Thread.Sleep(new TimeSpan(0,0,5));
				//��������״̬���������ֹͣ״̬�����˳�ѭ��
				if(m_extractorState == ExtractorState.Stop)
				{
					statusBar1.Text="������״̬:ֹͣ����.";
					break;   
				}
			}

		}

		/*��ʾ��ǰʱ�䣬��� listUrl �е� Url �����Ѿ������URL����(��Ҫ�޸ĵĵط�)*/
		private void timer1_Tick(object sender, System.EventArgs e)
		{

			//�ú�����ʱ���´���[����ʱ��]�ı���
			System.TimeSpan tTime=DateTime.Now -DateTime.Parse(this.tbStart.Text); 
			string sTime=tTime.Days.ToString()+"��"+tTime.Hours.ToString()+"ʱ"+
				tTime.Minutes.ToString()+"��"+tTime.Seconds.ToString()+"��";  
			this.tbStartUp.Text=sTime;			
		
	
			if(listURL.Count <= 0)
			{
				this.readUrl ();
			}

			/*�ж���ȡ�������Ƿ��ѴﵽҪ�洢������*/
			if(houseInfoList.Count >= maxHouseInfoNumber )
			{
				try
				{ 
					/*ΪrentInfo�������ݿ�����*/
					string sql = "select top 1 * from rentinfo";
					DataSet houseInfoSet = new DataSet ();
					adapter = new SqlDataAdapter ();
					conn = SqlBase.GetConnection ();
					conn.Open ();
					adapter.SelectCommand = new SqlCommand (sql,conn);
					adapter.Fill (houseInfoSet,"house");
					SqlCommandBuilder sqlCb = new SqlCommandBuilder(adapter);	

					this.btnStop.Enabled = false;

					HouseInfoExtractorLib.HouseInfo houseInfo = new HouseInfo ();

					for(int i = 0; i < houseInfoList.Count ;i++)
					{
						/*��ARRARLIST�ж�ȡһ������*/
						houseInfo = (HouseInfoExtractorLib.HouseInfo)houseInfoList[i];
						

						DataRow houseInfoRow;
						if(houseInfo.linkPhone != "" )  //  || houseInfo.linkPhone.Trim ().Length != 0)
						{ 
							houseInfoRow = houseInfoSet.Tables [0].NewRow ();
							if(houseInfo.url.IndexOf("ehomeday")!=-1&houseInfo.region==""&houseInfo.address.Length>2)
								houseInfo.region = houseInfo.address.Substring(0,2);
							/*���¹ؼ���Ϣ*/
							houseInfoRow["url"] = houseInfo.url;
							houseInfoRow["district"] = houseInfo.region;
							houseInfoRow["area"] = houseInfo.address  ;
							houseInfoRow["price"] = houseInfo.price ;
							houseInfoRow["room"] = houseInfo.room;
							houseInfoRow["hall"] = houseInfo.hall ;
							houseInfoRow["washroom"] = houseInfo.washRoom ;
							houseInfoRow["provider"] = houseInfo.source ;
							houseInfoRow["contact"] = houseInfo.linkMan ;
							houseInfoRow["telephone"] = houseInfo.linkPhone ;
							houseInfoRow["buildyear"] = houseInfo.doneDate ;
							houseInfoRow["buildtype"] = houseInfo.houseType ;
							houseInfoRow["floor"] = houseInfo.floor ;
							houseInfoRow["towards"] = houseInfo.direction ;
							houseInfoRow["size"] = houseInfo.area ;
							houseInfoRow["base"] = houseInfo.baseEstablishment ;
							houseInfoRow["equipment"] = houseInfo.equipment ;
							houseInfoRow["selltype"] = houseInfo.rentType ;
							houseInfoRow["mright"] = houseInfo.property ;
							houseInfoRow["purpose"] = houseInfo.managerType ;
							houseInfoRow["structure"] = houseInfo.houseStructure ;
							houseInfoRow["fitment"] = houseInfo.fitment ;
							houseInfoRow["other"] = houseInfo.remark ;
							houseInfoRow["manager"] = houseInfo.managerName ;
							houseInfoRow["tracffic"] = houseInfo.traffic ;
							houseInfoRow["school"] = houseInfo.schoolInfo ;
							houseInfoRow["envirment"] = houseInfo.environment ; 
							houseInfoRow["registdate"] = houseInfo.registerDate;
							houseInfoRow["cityid"] = houseInfo.cityID;
							houseInfoRow["urltype"] = houseInfo.urltype;

							/*�ж��Ƿ�Ϊ�н�*/
							string flag = "0";
							if(houseInfo.linkPhone != "")
							{
								char[] split = {',',';','.','��',' ','/','��','��','\\'};
								string[] mulTel = houseInfo.linkPhone.Split (split);
								for(int n=0; n<mulTel.Length; n++)
								{
									if(hashtel.ContainsValue (mulTel[n]))
									{
										//houseInfoRow["agent"] = '1';
										agentNum++;
										this.agentTb.Text = agentNum.ToString ();
                                        flag = "1";
										break;
									}
								}
							}
							/*�ж��Ƿ����ظ��ļ�¼*/
							bool hasSameRec = false;   // cjx
							char[] splitC = {',',';','.','��'};
							string[] linkPhone = houseInfo.linkPhone.Split (splitC);
							for(int j = 0; j<linkPhone.Length ; j++)
							{
								int r = 0;
								//string sele = "select count(*) from rentinfo where area='" + houseInfo.address + "' and telephone like'%" + linkPhone[j] + "%'";
								string sele = "select count(*) from rentinfo where area='" + houseInfo.address + "' and telephone like'%" + linkPhone[j] + "%' and adddate>='"+DateTime.Today+"'";//cjx
								SqlBase.ExecuteScalar(sele,out r);

								/*ɾ���ظ���¼*/
								if(r != 0)
								{
									hasSameRec = true;  //cjx
									sameRec++;
									this.sameTb.Text = sameRec.ToString();
								}
							}

							//����ǲ����н���������ݿ�
							if(flag == "0"&&!hasSameRec)
							{
								/*�����ݼ�����Ӽ�¼*/
								houseInfoSet.Tables[0].Rows.Add (houseInfoRow);		
								this.listInfo.Items.Add(houseInfoRow["url"]);
								num++;
								this.tbUseable.Text = num.ToString();
							}	

						}
					}

					/*���houseInfoList�е�����*/
	                 houseInfoList.Clear();
					/*����house�������ݿ������޸ĵ�����*/
					this.statusBar1 .Text = "���ڸ������ݿ⡣������";		
					adapter.Update (houseInfoSet,"house");
					this.btnStop.Enabled = true;
					this.statusBar1 .Text = "���ݿ��Ѹ������!";
					conn.Close();
					adapter.Dispose ();
					houseInfoSet.Dispose ();
				}
				catch(Exception e2)
				{					
					statusBar1.Text = "timer1_Tick:�ڴ洢������Ϣʱ," + e2.Message.ToString ();
				}
			}
		}
	
		

		/// <summary>
		/// ���֩������У����������µ��߳�
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer2_Tick(object sender, System.EventArgs e)
		{
			if(Hosts.Count>MaxHosts) Hosts.RemoveAt(0);
			Random rnd = new System.Random();// ���������һ����ţ� �Ӹ���ſ�ʼȡһ�� URL,�����ʱ����Ƶ������ĳһվ��
			double a = listURL.Count * rnd.NextDouble();
			UrlID = (int)a;			
			if(listURL.Count == 0)return;
			while(UrlID<listURL.Count)
			{
//				CurUrl = listURL[UrlID].ToString();
//				string sqlT = "update urltest set isprocess='1' where url='" + CurUrl.Substring(CurUrl.LastIndexOf("��")+1)+"'";
//				SqlBase.ExecuteQuery (sqlT,System.Data.CommandType.Text );				

				try
				{
//					if(this.ThreadNumber <= this.numericThreadNums.Value)
//					{
						Thread SpiderThread = new Thread(new ThreadStart(extract1));
						SpiderThread.Start();
						ThreadNumber++;
						//listURL.Remove(CurUrl);
						return;
//					}
					
				}
				catch(Exception e1)
				{
					statusBar1.Text = "timer2_Tick:�ڴ��� URL ����ʱ�����֣�" + e1.Message.ToString(); 
				}
				UrlID++;
			}
		}


		#region ------------������������Ϣ------------------
		private void txtUrlIP_TextChanged(object sender, System.EventArgs e)
		{
			this.btnUrl.Enabled = true;
		}

		private void txtUrlDB_TextChanged(object sender, System.EventArgs e)
		{
			this.btnUrl.Enabled = true;
		}

		private void txtUrlUser_TextChanged(object sender, System.EventArgs e)
		{
			this.btnUrl.Enabled = true;
		}

		private void txtUrlPwd_TextChanged(object sender, System.EventArgs e)
		{
			this.btnUrl.Enabled = true;
		}

		private void tbInsertIP_TextChanged(object sender, System.EventArgs e)
		{
			this.btnInsert.Enabled = true;
		}

		private void tbInsertDB_TextChanged(object sender, System.EventArgs e)
		{
			this.btnInsert.Enabled = true;
		}

		private void tbInsertUser_TextChanged(object sender, System.EventArgs e)
		{
			this.btnInsert.Enabled = true;
		}

		private void tbInsertPwd_TextChanged(object sender, System.EventArgs e)
		{
			this.btnInsert.Enabled = true;
		}

		private void autoRunCB_CheckedChanged(object sender, System.EventArgs e)
		{
			this.btnOther.Enabled = true;
		}

		private void numericThreadNums_ValueChanged(object sender, System.EventArgs e)
		{
			this.btnOther.Enabled = true;
		}
		#endregion

		/// <summary>
		/// ���ú���������û�����ķ����������Ƿ���ȷ
		/// </summary>
		/// <param name="ServerIP">ip��ַ</param>
		/// <param name="DBName">���ݿ���</param>
		/// <param name="UserName">�û���</param>
		/// <param name="UserPwd">����</param>
		/// <returns>������Ϣ</returns>
		private string CheckServerSettings(string ServerIP, string DBName, string UserName, string UserPwd)
		{
			string ErrorMessage = "";
			if (ServerIP.Trim() == "") {ErrorMessage = ErrorMessage + "\n\n�� û��ָ����������IP��ַ...";}
			if (DBName.Trim() == "")   {ErrorMessage = ErrorMessage + "\n\n�� û��ָ�����ݿ���...";}
			if (UserName.Trim() == "") {ErrorMessage = ErrorMessage + "\n\n�� û��ָ�����ݿ��û���...";}
			if (UserPwd.Trim() == "")  {ErrorMessage = ErrorMessage + "\n\n�� û��ָ�����ݿ�����...";}
			return ErrorMessage;
		}

		/// <summary>
		/// ��URL��ȡ����Ϣ���������ļ�
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUrl_Click(object sender, System.EventArgs e)
		{
			string ErrMsg = CheckServerSettings(txtUrlIP.Text.Trim(), txtUrlDB.Text.Trim() ,txtUrlUser.Text.Trim(), txtUrlPwd.Text.Trim());
			if(ErrMsg!="")
			{
				MessageBox.Show("��������URL������ʱ�������´���" + ErrMsg, "���������ô��� ������",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
			}
			else
			{
				DialogResult result;
				result = MessageBox.Show("��ȷ��Ҫ�޸�URL�������������𣿰����ǡ������޸ģ������񡱷���...","ȷ���޸ķ���������...",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Information);
				if (result == DialogResult.No) 
				{
					this.btnUrl.Enabled = false;
					return;
				}
				else
				{
					IniFile myINI = new IniFile("config.ini");
					UrlIP = txtUrlIP.Text.Trim();			UrlDB = txtUrlDB.Text.Trim();
					UrlUser = txtUrlUser.Text.Trim();		UrlPwd = txtUrlPwd.Text.Trim();
					myINI.WriteKeyValue("UrlServer","UrlIP",UrlIP.ToString());
					myINI.WriteKeyValue("UrlServer","UrlDB",UrlDB.ToString());
					myINI.WriteKeyValue("UrlServer","UrlUser",UrlUser.ToString());
					myINI.WriteKeyValue("UrlServer","UrlPwd",UrlPwd.ToString());
				}
			}
			btnUrl.Enabled = false;
			txtUrlIP.Focus();
		}

		/// <summary>
		/// ���������Ϣ���������ļ���
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnInsert_Click(object sender, System.EventArgs e)
		{
			string ErrMsg = CheckServerSettings(tbInsertIP.Text.Trim(), tbInsertDB.Text.Trim() ,tbInsertUser.Text.Trim(), tbInsertPwd.Text.Trim());
			if(ErrMsg!="")
			{
				MessageBox.Show("����������ȡ���������ʱ�������´���" + ErrMsg, "���������ô��� ������",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
			}
			else
			{
				DialogResult result;
				result = MessageBox.Show("��ȷ��Ҫ�޸���ȡ����������������𣿰����ǡ������޸ģ������񡱷���...","ȷ���޸ķ���������...",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Information);
				if (result == DialogResult.No) 
				{
					btnInsert.Enabled = false;
					return;
				}
				else
				{
					IniFile myINI = new IniFile("config.ini");
					InsertIP = tbInsertIP.Text.Trim();			InsertDB = tbInsertDB.Text.Trim();
					InsertUser = tbInsertUser.Text.Trim();		InsertPwd = tbInsertPwd.Text.Trim();
					myINI.WriteKeyValue("InsertServer","InsertIP",InsertIP.ToString());
					myINI.WriteKeyValue("InsertServer","InsertDB",InsertDB.ToString());
					myINI.WriteKeyValue("InsertServer","InsertUser",InsertUser.ToString());
					myINI.WriteKeyValue("InsertServer","InsertPwd",InsertPwd.ToString());
				}
			}
			this.btnInsert.Enabled = false;
			this.tbInsertIP.Focus();
		}

		private void btnOther_Click(object sender, System.EventArgs e)
		{
			DialogResult result;
			result = MessageBox.Show("��ȷ��Ҫ�޸����ò����𣿰����ǡ������޸ģ������񡱷���...","ȷ���޸��ĵ���������...",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Information);
			if (result == DialogResult.No) 
			{				
				btnOther.Enabled = false;
				return;
			}
			else
			{
				try
				{
					//����ֵд��INI�ļ���
					IniFile iniWrite=new IniFile("config.ini"); 					
					iniWrite.WriteKeyValue("RunningEnvironment","ReadUrl",this.numericThreadNums.Value.ToString());
					//
					//����Ӧ�ó�����Ƿ��Զ�ִ�з���(ע:�Զ�ִ�л�û����)
					if(this.autoRunCB.Checked)
						iniWrite.WriteKeyValue("RunningEnvironment","AutoRun","1");
					else
						iniWrite.WriteKeyValue("RunningEnvironment","AutoRun","0");
				
					MessageBox.Show("��������������ɹ�","��Ϣ��ʾ",MessageBoxButtons.OK,MessageBoxIcon.Information);
					btnOther.Enabled=false;
				}
				catch(Exception err)
				{
					MessageBox.Show("�������������ó��ִ��󣬴�����Ϣ�����:\n"+err.Message,"������Ϣ",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				}
							
			}		
			btnOther.Enabled = false;
			this.numericThreadNums.Focus();
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			try
			{
				//
				//��ȡ���ط����������Ϣ
				IniFile myIni=new IniFile("config.ini");
				//string xx=(string)myIni.GetKeyValue("Downloadserver","IP");
				this.txtUrlIP.Text=(string)myIni.GetKeyValue("URLServer","UrlIP");
				this.txtUrlDB.Text =(string)myIni.GetKeyValue("URLServer","UrlDB");
				this.txtUrlUser.Text =(string)myIni.GetKeyValue("URLServer","UrlUser");
				this.txtUrlPwd.Text =(string)myIni.GetKeyValue("URLServer","UrlPwd");
				//
				//��ȡ���������������Ϣ
				this.tbInsertIP.Text= (string)myIni.GetKeyValue("InsertServer","InsertIP");
				this.tbInsertDB.Text= (string)myIni.GetKeyValue("InsertServer","InsertDB");
				this.tbInsertUser.Text= (string)myIni.GetKeyValue("InsertServer","InsertUser");
				this.tbInsertPwd.Text= (string)myIni.GetKeyValue("InsertServer","InsertPwd");
				//���ô洢���ݿ�����ַ���
				//���ö�url���ݱ�ķ�����
				//dataBase = new DataBase (this.txtUrlIP.Text.Trim (),this.txtUrlDB .Text.Trim (),this.txtUrlUser .Text.Trim (),this.txtUrlPwd.Text.Trim ());
				//SqlBase1.setConnectionString ( "server="+this.txtUrlIP.Text.Trim ()+";uid="+this.txtUrlUser .Text.Trim () +";pwd="+this.txtUrlPwd.Text.Trim () +";database="+this.txtUrlDB .Text.Trim ()); 
				
				//���ö�rentinfo���ݱ�ķ�����
				//SqlBase.setConnectionString ( "server="+this.tbInsertIP.Text.Trim ()+";uid="+this.tbInsertUser .Text.Trim () +";pwd="+this.tbInsertPwd.Text.Trim () +";database="+this.tbInsertDB .Text.Trim ()); 
				//һ�ζ�ȡurl��
				//NumOfRead.Value =Int32.Parse(myIni.GetKeyValue("RunningEnvironment","ReadUrl"));
				//this.ReadUrl = (int)NumOfRead.Value;
				//����߳���
				//this.numericThreadNums.Value=Int32.Parse(myIni.GetKeyValue("RunningEnvironment","MaxThreads"));
				short i=0;
				i=Int16.Parse(myIni.GetKeyValue("RunningEnvironment","AutoRun"));
				if(i==0)
					autoRunCB.Checked=false;
				else
				{
					this.autoRunCB.Checked=true;
					btnExtractor_Click_1(null,null);
				}
				
				//����ʱ�����ð�ťӦ������Ч״̬
				this.btnUrl.Enabled=false;
				this.btnInsert.Enabled=false;
				this.btnOther.Enabled =false;				
			}
			catch(Exception error)
			{
				MessageBox.Show(this,"�����ʼ��������μ�������Ϣ��"+error.Message,"������Ϣ��ʾ",
					MessageBoxButtons.OK,MessageBoxIcon.Warning);   			
			}

			//��ȡ�н�绰����
			string seleTel = "select id,telephone from agent";
			DataSet dsTel = new DataSet ();
			try
			{
				dsTel = SqlBase.ExecuteDataSet(seleTel,System.Data.CommandType.Text );
			
				foreach(DataRow telRow in dsTel.Tables [0].Rows )
				{
					//����¼����hashTable
					string id = telRow["id"].ToString ().Trim ();
					string tel = telRow["telephone"].ToString ().Trim ();
					hashtel.Add(id,tel);
				}
			}
			catch(Exception e1)
			{
				System.Windows.Forms.MessageBox.Show (e1.Message );
				
			}

			dsTel.Clear();
			dsTel.Dispose();
			
		}


		/// <summary>
		/// ��Ӵ�����־
		/// </summary>
		/// <param name="sErrID">url</param>
		/// <param name="sErrInfo">������Ϣ</param>
		private void AddErrInfo(string sErrID,string sErrInfo)
		{
			this.btnSaveLog.Enabled=true;
			//ֻ����ǰ100��������Ϣ.
			if(lvLog.Items.Count>100)
				lvLog.Items.Remove(lvLog.Items[0]);
			ListViewItem lvwErr=new ListViewItem();
			lvwErr.Text=sErrID;
			lvwErr.SubItems.Add(sErrInfo);
			lvwErr.SubItems.Add(DateTime.Now.ToString());
			this.lvLog.Items.Add(lvwErr);
		}


		#region  -----������Ϣ����-------
		private void btnLogClear_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("�Ƿ��������еĴ�����Ϣ��","��Ϣ��ʾ",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
				this.lvLog.Items.Clear();
		}

		private void btnSaveLog_Click(object sender, System.EventArgs e)
		{
			try
			{
				string sErrFile=Application.StartupPath +"\\Errors.Txt";
				System.IO.StreamWriter  fileErr=System.IO.File.CreateText(sErrFile);
				for(int i=0;i<this.lvLog.Items.Count;i++)
				{
					fileErr.WriteLine(lvLog.Items[i].SubItems[0].Text + ";" +lvLog.Items[i].SubItems[1].Text+lvLog.Items[i].SubItems[2].Text);
				}
				fileErr.Close();
			}
			catch(Exception err)
			{
				MessageBox.Show("����ʧ��,������Ϣ��ο�:\n"+err.Message,"������Ϣ",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}
			MessageBox.Show("����ɹ�!��鿴Ӧ�ó��������ļ����µ�Errors.Txt�ļ�.","���������Ϣ",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		#endregion

		private void extractPage_Click(object sender, System.EventArgs e)
		{
		
		}

	}
	
}
