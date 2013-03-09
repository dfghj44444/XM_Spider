using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions; 
using System.Xml;   
namespace EduWebSites
{
	/// <summary>
	/// ����:DataBase
	/// ����Ϊ��Ͳ���,�����ٱ�������̳�.
	/// ʵ����ʱ��Ҫָ��������IP/SQL Server����/�û���/�û����� 
	/// </summary>
	sealed public class DataBase:IDisposable
	{		
		#region ------------�������ִ���-------------
		private string m_ConnectionString;
		private SqlCommand m_SqlCommand=null;
		private SqlTransaction m_Transaction;   
		private bool m_hasTransaction=false;		//�Ƿ���������
		/// <summary>
		/// ���캯��
		/// ��Ҫ�ṩ������IP/���ݿ�����/�û���/�û������
		/// </summary>
		/// <param name="serverIP">������IP</param>
		/// <param name="serverName">SQL Server����</param>
		/// <param name="userName">�û���</param>
		/// <param name="password">�û�����</param>
		public DataBase(string serverIP,string databaseName,string userName,string password)
		{
			//����У��
			string strIP=serverIP.ToLower();
			if(databaseName.Trim()=="")
				throw new Exception("�Զ������:û��ָ��SQL Server����");
			if(userName.Trim()=="")
				throw new Exception("�Զ������:�û����Ʋ���Ϊ��!");
			
			//���������ַ���
			m_ConnectionString=String.Format("Server={0};Database={1};uid={2};pwd={3};connection timeout=90",serverIP,databaseName,userName,password);
			m_SqlCommand=new SqlCommand();
			m_SqlCommand.Connection=new SqlConnection(m_ConnectionString);
		}
		
		/// <summary>
		/// ���ٶ���ǰ��Ҫ�ƺ�Ĺ���
		/// </summary>
		public void Dispose()
		{
			if(m_SqlCommand!=null)
                if(m_SqlCommand.Connection.State==ConnectionState.Open)
                {
					m_SqlCommand.Connection.Close();
					m_SqlCommand.Connection.Dispose();
				}
				m_SqlCommand.Dispose(); 
		}
		/// <summary>
		/// ��Command����ָ����ͨ��SQL�ַ���;
		/// ͨ��������Ӧ��RunSQL����ִ�в�ѯ.
		/// </summary>
		/// <param name="commandText">��ѯ�����ַ���</param>
		public void SetCommandText(string commandText)
		{
			//���Command�����в���,�����֮
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();

			//ָ����������ΪSQL�ı�����
			m_SqlCommand.CommandType=CommandType.Text;
			m_SqlCommand.CommandText=commandText;			
		}
		//
		/// <summary>
		/// ���ԭ�е�parameter.���ִ�д洢����,�����ô˷���.
		/// </summary>
		public void ClearParameter()
		{
			m_SqlCommand.Parameters.Clear();
		}
		/// <summary>
		/// ���SQL��ѯ����.���ִ�д洢����,�����ô˷���
		/// </summary>
		/// <param name="paraObj">parameter��������</param>
		public void AddParameter(SqlParameter paraObj)
		{
			m_SqlCommand.Parameters.Add(paraObj);
		}
		/// <summary>
		/// �ڲ�����,��Ӵ洢���̷��ز���
		/// </summary>
		private void CreateReturnValueParam()
		{
			m_SqlCommand.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, 
				ParameterDirection.ReturnValue,false,0,0,string.Empty,DataRowVersion.Default,null)); 
		}
		/// <summary>
		/// ��Command����ָ���洢���̺Ͳ���;
		/// ͨ������Run����ִ�в�ѯ.
		/// </summary>
		/// <param name="sprocName">�洢��������</param>
		/// <param name="parameters">�����б�</param>
		public void SetCommandText(string sprocName,SqlParameter[] parameters)
		{
			//���ԭ��������������
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();
			
			//ָ���洢������
			m_SqlCommand.CommandText=sprocName;
			m_SqlCommand.CommandType=CommandType.StoredProcedure;
			//��Ӳ���
			if(parameters!=null)
                foreach(SqlParameter tmpParam in parameters)
                    m_SqlCommand.Parameters.Add(tmpParam);
			
			//��Ӵ洢���̷���ֵ����
			CreateReturnValueParam();
		}
		#endregion
		
		#region ----------- ִ����ͨ��SQL��ѯ��洢����---------------
		/// <summary>
		/// ִ��SQL��ѯ,������Ӱ�������;
		/// </summary>
		/// <returns>������Ӱ�������</returns>
		public int Run()
		{
			int i=0;
			string errStr="";
			if(m_SqlCommand.Connection.State ==ConnectionState.Closed)
				m_SqlCommand.Connection.Open(); 
			try
			{
				//��ȡ��Ӱ�������
				i=m_SqlCommand.ExecuteNonQuery();
			}
			catch(Exception Err)
			{
				//��¼�´�����Ϣ
				errStr="������Ϣ:"+Err.Message;
			}
			finally
			{  if(!m_hasTransaction)
                   m_SqlCommand.Connection.Close();
				
				//�����������,���ش�����Ϣ
				//if(errStr!="")
					//throw new Exception(errStr);
			}
			//���ִ�д洢�����򷵻ش洢���̵ķ���ֵ,���򷵻ط�Ӱ�������
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
                return i;
		}
		/// <summary>
		/// ִ��SQL��ѯ,ͨ����������һ��DataReader����
		/// </summary>
		/// <param name="getReaderObj">DataReader����(������)</param>
		public void Run(out SqlDataReader resultOfDataReader)
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Closed)
                m_SqlCommand.Connection.Open();
			try
			{
				//ͨ�����÷��ز�ѯ���
				resultOfDataReader=m_SqlCommand.ExecuteReader(CommandBehavior.CloseConnection); 
			}
			catch(SqlException error)
			{
				throw new Exception("[���ݿ����]�������󣬴�����ϢΪ:"+error.Message);
			}
			if(!m_hasTransaction)
				m_SqlCommand.Connection.Close();
		}
		/// <summary>
		/// ִ�в�ѯ,����һ��DataTable(��������)
		/// </summary>
		/// <param name="getDataTable">ͨ�����÷���DataTable</param>
		public int Run(DataTable resultOfDataTable)
		{
			SqlDataAdapter tmpAdapter=new SqlDataAdapter();
			tmpAdapter.SelectCommand=m_SqlCommand;
			try
			{
				tmpAdapter.Fill(resultOfDataTable);
			}
			catch(SqlException error)
			{
				throw new Exception(error.Message);
			}			
			//���ִ�д洢�����򷵻ش洢�����趨�ķ���ֵ,���򷵻�0
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
				return 0;
		}
		/*//Ϊʲô��γ������>????
		public int Run(DataSet resultDataSet)
		{
			SqlDataAdapter tmpAdapter=new SqlDataAdapter();
			tmpAdapter.SelectCommand=m_SqlCommand;
			tmpAdapter.Fill(resultDataSet);
			//���ִ�д洢�����򷵻ش洢���̵ķ���ֵ,���򷵻ط�Ӱ�������
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
				return 0;
		}/*/
		
		/// <summary>
		/// ��������ķ�ʽִ�в�ѯ���ʽ;�ô���������������ڷ����ģ�
		/// ������ִ������񽫻ع�����ȫ��������������ύ��
		/// </summary>
		/// <param name="batchQuery">�������ѯ���</param>
		/// <returns>������Ӱ�������</returns>
		public int Run(string[] batchQueryString)
		{
			int i=0;
			m_SqlCommand.CommandType=CommandType.Text;
			//��������
			Transaction_Begin();
			try
			{
				//��ʼѭ��,����ִ�и���/���/ɾ��
				foreach(string s in batchQueryString)
				{
					m_SqlCommand.CommandText=s;
					i+=m_SqlCommand.ExecuteNonQuery(); 
				}
				//����޴��������ύ����
				Transaction_Commit();
				m_hasTransaction=false;
			}
			catch(Exception e)
			{
				//�������������Ҫ�ع�����;
				string errMsg="";
				if(m_hasTransaction)
				{
					m_hasTransaction=false;
					errMsg=Transaction_Rollback();
				}
				throw new Exception("�����ύʱ��������,ϵͳ��������:"+e.Message+"\r"+errMsg);
			}
			return i;
		}
		/// <summary>
		///�������� 
		/// </summary>
		public void Transaction_Begin()
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Closed)
                m_SqlCommand.Connection.Open();
			m_Transaction=m_SqlCommand.Connection.BeginTransaction();
			m_SqlCommand.Transaction=m_Transaction;
			m_hasTransaction=true;
		}
		/// <summary>
		///�ύ���� 
		/// </summary>
		public void Transaction_Commit()
		{
			try
			{
				m_hasTransaction=false;
				m_Transaction.Commit();
				if(m_SqlCommand.Connection.State==ConnectionState.Open)
					m_SqlCommand.Connection.Close();
			}
			catch(Exception error)
			{
				throw new Exception("���ݿ��������ύ����ʧ�ܣ�������Ϣ��"+error.Message);
			}
			
		}
		/// <summary>
		/// �ع�����������
		/// </summary>
		public string Transaction_Rollback()
		{
			try
			{
				m_hasTransaction=false;
				m_Transaction.Rollback();
				if(m_SqlCommand.Connection.State==ConnectionState.Open)
					m_SqlCommand.Connection.Close();
			}
			catch(System.Data.SqlClient.SqlException error)
			{
				return "���ݿ������󣺻ع�����ʧ�ܣ�"+error.Message;
			}
            catch(Exception err)
			{
				return "���ݿ�������"+err.Message;
			}
			return "";
		}
		/// <summary>
		///����һ�����������ݿ�����;
		/// ע:���Ҫ��ʱ������ݽ���д����,���Կ���һ���־�����,
		/// ִ����BeginLastingConnection��,���Է������ִ��RunModify()����;
		///BeginLastingConnection/EndLastingConnection���ʹ��
		/// </summary>
		public void BeginLastingConnection()
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Closed)
				m_SqlCommand.Connection.Open();
		}
		/// <summary>
		/// ����ִ�в�ѯ;
		/// ִ��BeginLastingConnection��,�ڱ��ֳ�������״̬��,���Է������ִ�в�ѯ���
		/// ִ����Ϻ�,����EndLastingConnection�ر�����;
		/// </summary>
		/// <param name="queryString">SQL��ѯ���ʽ</param>
		/// <returns>������Ӱ�������</returns>
		public int RunModify(string queryString)
		{
			int i=0;
			m_SqlCommand.CommandType=CommandType.Text;
			m_SqlCommand.CommandText=queryString;
			try
			{
				//ִ�в�ѯ��������Ӱ�������
				i=m_SqlCommand.ExecuteNonQuery();
			}
			catch(SqlException error)
			{
				throw new Exception("������Ϣ:"+error.Message);
			}
			return i;
		}
		/// <summary>
		/// �������Ӻ�,�������ִ�д洢����
		/// ��ִ��BeginLastingConnection,Ȼ���ڱ�������״̬����,���ִ�д洢����
		/// ִ����Ϻ���Ҫ��EndLastingConnection�ر�����
		/// </summary>
		/// <param name="sprocName">�洢������(�޲���Ϊnull)</param>
		/// <param name="parameters">�洢�����������</param>
		/// <returns>�洢���̵ķ���ֵ</returns>
		public int RunModify(string sprocName,SqlParameter[] parameters)
		{
			m_SqlCommand.CommandType=CommandType.StoredProcedure;
			//�������ԭ�в��������֮
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();
			//��Ӳ���
			if(parameters!=null)
                foreach(SqlParameter tmpParam in parameters)
                    m_SqlCommand.Parameters.Add(tmpParam);
			//��ӷ���ֵ����
			CreateReturnValueParam();
			
			m_SqlCommand.CommandText=sprocName;
			try
			{
				m_SqlCommand.ExecuteNonQuery(); 
			}
			catch(SqlException error)
			{
				throw new Exception("������Ϣ:"+error.Message);
			}
			return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
		}
		/// <summary>
		/// �����־�����
		/// ����BeginLastingConnection��,������þ��ô˷����ر�����
		/// </summary>
		public void EndLastingConnection()
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Open)
				m_SqlCommand.Connection.Close();
		}
		#endregion
	}
}
