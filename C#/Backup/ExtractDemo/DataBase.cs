using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions; 
using System.Xml;   
namespace EduWebSites
{
	/// <summary>
	/// 类名:DataBase
	/// 该类为最低层类,不可再被其它类继承.
	/// 实例化时需要指定服务器IP/SQL Server名称/用户名/用户口令 
	/// </summary>
	sealed public class DataBase:IDisposable
	{		
		#region ------------公共部分代码-------------
		private string m_ConnectionString;
		private SqlCommand m_SqlCommand=null;
		private SqlTransaction m_Transaction;   
		private bool m_hasTransaction=false;		//是否开启有事务
		/// <summary>
		/// 构造函数
		/// 需要提供服务器IP/数据库名称/用户名/用户口令等
		/// </summary>
		/// <param name="serverIP">服务器IP</param>
		/// <param name="serverName">SQL Server名称</param>
		/// <param name="userName">用户名</param>
		/// <param name="password">用户口令</param>
		public DataBase(string serverIP,string databaseName,string userName,string password)
		{
			//数据校验
			string strIP=serverIP.ToLower();
			if(databaseName.Trim()=="")
				throw new Exception("自定义错误:没有指定SQL Server名称");
			if(userName.Trim()=="")
				throw new Exception("自定义错误:用户名称不能为空!");
			
			//生成连接字符串
			m_ConnectionString=String.Format("Server={0};Database={1};uid={2};pwd={3};connection timeout=90",serverIP,databaseName,userName,password);
			m_SqlCommand=new SqlCommand();
			m_SqlCommand.Connection=new SqlConnection(m_ConnectionString);
		}
		
		/// <summary>
		/// 销毁对象前需要善后的工作
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
		/// 给Command对象指定普通的SQL字符串;
		/// 通过调用相应的RunSQL方法执行查询.
		/// </summary>
		/// <param name="commandText">查询命令字符串</param>
		public void SetCommandText(string commandText)
		{
			//如果Command对象有参数,则清除之
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();

			//指定命令类型为SQL文本命令
			m_SqlCommand.CommandType=CommandType.Text;
			m_SqlCommand.CommandText=commandText;			
		}
		//
		/// <summary>
		/// 清除原有的parameter.如果执行存储过程,不调用此方法.
		/// </summary>
		public void ClearParameter()
		{
			m_SqlCommand.Parameters.Clear();
		}
		/// <summary>
		/// 添加SQL查询参数.如果执行存储过程,不调用此方法
		/// </summary>
		/// <param name="paraObj">parameter参数名称</param>
		public void AddParameter(SqlParameter paraObj)
		{
			m_SqlCommand.Parameters.Add(paraObj);
		}
		/// <summary>
		/// 内部过程,添加存储过程返回参数
		/// </summary>
		private void CreateReturnValueParam()
		{
			m_SqlCommand.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, 
				ParameterDirection.ReturnValue,false,0,0,string.Empty,DataRowVersion.Default,null)); 
		}
		/// <summary>
		/// 给Command对象指定存储过程和参数;
		/// 通过调用Run方法执行查询.
		/// </summary>
		/// <param name="sprocName">存储过程名称</param>
		/// <param name="parameters">参数列表</param>
		public void SetCommandText(string sprocName,SqlParameter[] parameters)
		{
			//如果原来存在则进行清除
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();
			
			//指定存储过程名
			m_SqlCommand.CommandText=sprocName;
			m_SqlCommand.CommandType=CommandType.StoredProcedure;
			//添加参数
			if(parameters!=null)
                foreach(SqlParameter tmpParam in parameters)
                    m_SqlCommand.Parameters.Add(tmpParam);
			
			//添加存储过程返回值参数
			CreateReturnValueParam();
		}
		#endregion
		
		#region ----------- 执行普通的SQL查询或存储过程---------------
		/// <summary>
		/// 执行SQL查询,返回受影响的行数;
		/// </summary>
		/// <returns>返回受影响的行数</returns>
		public int Run()
		{
			int i=0;
			string errStr="";
			if(m_SqlCommand.Connection.State ==ConnectionState.Closed)
				m_SqlCommand.Connection.Open(); 
			try
			{
				//获取受影响的行数
				i=m_SqlCommand.ExecuteNonQuery();
			}
			catch(Exception Err)
			{
				//记录下错误信息
				errStr="错误信息:"+Err.Message;
			}
			finally
			{  if(!m_hasTransaction)
                   m_SqlCommand.Connection.Close();
				
				//如果发生错误,返回错误信息
				//if(errStr!="")
					//throw new Exception(errStr);
			}
			//如果执行存储过程则返回存储过程的返回值,否则返回返影响的行数
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
                return i;
		}
		/// <summary>
		/// 执行SQL查询,通过参数返回一个DataReader引用
		/// </summary>
		/// <param name="getReaderObj">DataReader对象(引用型)</param>
		public void Run(out SqlDataReader resultOfDataReader)
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Closed)
                m_SqlCommand.Connection.Open();
			try
			{
				//通过引用返回查询结果
				resultOfDataReader=m_SqlCommand.ExecuteReader(CommandBehavior.CloseConnection); 
			}
			catch(SqlException error)
			{
				throw new Exception("[数据库对象]发生错误，错误信息为:"+error.Message);
			}
			if(!m_hasTransaction)
				m_SqlCommand.Connection.Close();
		}
		/// <summary>
		/// 执行查询,返回一个DataTable(引用类型)
		/// </summary>
		/// <param name="getDataTable">通过引用返回DataTable</param>
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
			//如果执行存储过程则返回存储过程设定的返回值,否则返回0
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
				return 0;
		}
		/*//为什么这段程序出错>????
		public int Run(DataSet resultDataSet)
		{
			SqlDataAdapter tmpAdapter=new SqlDataAdapter();
			tmpAdapter.SelectCommand=m_SqlCommand;
			tmpAdapter.Fill(resultDataSet);
			//如果执行存储过程则返回存储过程的返回值,否则返回返影响的行数
			if(m_SqlCommand.CommandType==CommandType.StoredProcedure)
				return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
			else
				return 0;
		}/*/
		
		/// <summary>
		/// 以批处理的方式执行查询表达式;该处理过程是在事务内发生的，
		/// 如果出现错误，事务将回滚，完全无误则进行事务提交。
		/// </summary>
		/// <param name="batchQuery">批处理查询语句</param>
		/// <returns>返回受影响的行数</returns>
		public int Run(string[] batchQueryString)
		{
			int i=0;
			m_SqlCommand.CommandType=CommandType.Text;
			//开启事务
			Transaction_Begin();
			try
			{
				//开始循环,逐条执行更新/添加/删除
				foreach(string s in batchQueryString)
				{
					m_SqlCommand.CommandText=s;
					i+=m_SqlCommand.ExecuteNonQuery(); 
				}
				//如果无错误发生则提交事务
				Transaction_Commit();
				m_hasTransaction=false;
			}
			catch(Exception e)
			{
				//如果发生错误需要回滚事务;
				string errMsg="";
				if(m_hasTransaction)
				{
					m_hasTransaction=false;
					errMsg=Transaction_Rollback();
				}
				throw new Exception("批量提交时发生错误,系统错误描述:"+e.Message+"\r"+errMsg);
			}
			return i;
		}
		/// <summary>
		///开启事务 
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
		///提交事务 
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
				throw new Exception("数据库对象错误：提交事务失败；错误信息："+error.Message);
			}
			
		}
		/// <summary>
		/// 回滚开启的事务
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
				return "数据库对象错误：回滚事务失败；"+error.Message;
			}
            catch(Exception err)
			{
				return "数据库对象错误："+err.Message;
			}
			return "";
		}
		/// <summary>
		///开启一个持续的数据库连接;
		/// 注:如果要长时间对数据进行写操作,可以开启一个持久连接,
		/// 执行完BeginLastingConnection后,可以反复多次执行RunModify()操作;
		///BeginLastingConnection/EndLastingConnection配对使用
		/// </summary>
		public void BeginLastingConnection()
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Closed)
				m_SqlCommand.Connection.Open();
		}
		/// <summary>
		/// 反复执行查询;
		/// 执行BeginLastingConnection后,在保持长久连接状态下,可以反复多次执行查询语句
		/// 执行完毕后,调用EndLastingConnection关闭连接;
		/// </summary>
		/// <param name="queryString">SQL查询表达式</param>
		/// <returns>返回受影响的行数</returns>
		public int RunModify(string queryString)
		{
			int i=0;
			m_SqlCommand.CommandType=CommandType.Text;
			m_SqlCommand.CommandText=queryString;
			try
			{
				//执行查询并返回受影响的行数
				i=m_SqlCommand.ExecuteNonQuery();
			}
			catch(SqlException error)
			{
				throw new Exception("错误信息:"+error.Message);
			}
			return i;
		}
		/// <summary>
		/// 开启连接后,反复多次执行存储过程
		/// 先执行BeginLastingConnection,然后在保持连接状态的下,多次执行存储过程
		/// 执行完毕后需要用EndLastingConnection关闭连接
		/// </summary>
		/// <param name="sprocName">存储过程名(无参数为null)</param>
		/// <param name="parameters">存储过程所需参数</param>
		/// <returns>存储过程的返回值</returns>
		public int RunModify(string sprocName,SqlParameter[] parameters)
		{
			m_SqlCommand.CommandType=CommandType.StoredProcedure;
			//如果存在原有参数则清除之
			if(m_SqlCommand.Parameters.Count>0)
				m_SqlCommand.Parameters.Clear();
			//添加参数
			if(parameters!=null)
                foreach(SqlParameter tmpParam in parameters)
                    m_SqlCommand.Parameters.Add(tmpParam);
			//添加返回值参数
			CreateReturnValueParam();
			
			m_SqlCommand.CommandText=sprocName;
			try
			{
				m_SqlCommand.ExecuteNonQuery(); 
			}
			catch(SqlException error)
			{
				throw new Exception("错误信息:"+error.Message);
			}
			return (int)m_SqlCommand.Parameters["ReturnValue"].Value;
		}
		/// <summary>
		/// 结束持久连接
		/// 开启BeginLastingConnection后,如果不用就用此方法关闭连接
		/// </summary>
		public void EndLastingConnection()
		{
			if(m_SqlCommand.Connection.State==ConnectionState.Open)
				m_SqlCommand.Connection.Close();
		}
		#endregion
	}
}
