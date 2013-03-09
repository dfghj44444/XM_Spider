using System;
using System.Data;
using System.Data .SqlClient ;
using System.Configuration ;
using System.Diagnostics ;

namespace GeneralClass.SqlBaseLib
{
	/// <summary>
	/// general operation to the database
	/// 提供了一组操作sql数据库的公共方法
	/// </summary>
	public class SqlBase1
	{
		#region Connect Message
		/// <summary>
		/// <param name="connectionString">连接字符串</param>
		/// </summary>
		private  static string connectionString = "server=.;uid=sa;pwd=lquuvtkk;database=house";

		public static void setConnectionString(string connstr)
		{
			connectionString = connstr;
		}

		/// <summary>
		/// get the connect object
		/// </summary>
		/// <returns></returns>
		public static SqlConnection GetConnection()
		{
			return new SqlConnection(connectionString);
		}
		#endregion

		#region SqlDataReader Method
		/// <summary>
		/// 执行SqlDataReader方法
		/// </summary>
		/// <param name="con">连接对象</param>
		/// <param name="sql">SQl表达式。存储过程名|Sql语句</param>
		/// <returns>DataReader对象</returns>
		public static SqlDataReader ExecuteDataReader(string sql,CommandType type)
		{
			SqlConnection con = SqlBase.GetConnection();			
			if(sql == null || sql == "")
				throw new Exception("Sql Expression is null,please allocate it");			
			con.Open();														
			SqlCommand command = con.CreateCommand();
			command.CommandText = sql;
			command.CommandType = type;
			SqlDataReader read = command.ExecuteReader(CommandBehavior.CloseConnection);
			return read;
			
		}

		/// <summary>
		///	使用存储过程
		/// </summary>
		/// <param name="con"></param>
		/// <param name="sql">存储过程名|sql语句</param>
		/// <param name="param">存储过程参数集合</param>
		/// <returns>DataReader对象</returns>
		public static SqlDataReader ExecuteDataReader(string sql,CommandType type,SqlParameter[] param)
		{
			using(SqlConnection con = SqlBase.GetConnection())
			{
				if(sql == null || sql == "")
					throw new Exception("Sql Expression is null,please allocate it");
				//SqlConnection con = SqlBase.GetConnection();
				con.Open();
				SqlCommand command = con.CreateCommand();
				command.CommandText = sql;
				command.CommandType = type;
				foreach(SqlParameter p in param)
				{
					command.Parameters .Add(p);
				}
				SqlDataReader read = command.ExecuteReader(CommandBehavior.CloseConnection);
				return read;
			}
		}

		#endregion

		#region Return Data Object
		/// <summary>
		/// 返回dataset对象
		/// </summary>
		/// <param name="sql">存储过程|sql语句</param>
		/// <returns>DataSet</returns>
		public static DataSet ExecuteDataSet(string sql,CommandType type)
		{
			if(sql == null || sql == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				SqlDataAdapter da = new SqlDataAdapter();
				da.SelectCommand = new SqlCommand();
				da.SelectCommand .CommandType = type;
				da.SelectCommand .CommandText = sql;
				da.SelectCommand .Connection = con;
				DataSet ds = new DataSet();
				da.Fill(ds);
				return ds;
			}
		}

		/// <summary>
		/// 使用DataSet自带的分页方法
		/// </summary>
		/// <param name="sql">连接字符串</param>
		/// <param name="currentPage">当前页</param>
		/// <param name="pagesize">每页显示的数据量</param>
		/// <param name="tableName">表名</param>
		/// <param name="recordNo">总记录条数</param>
		/// <returns>返回一定数量的数据集</returns>
		public static DataView ExecuteDataView(string sql,int currentPage,int pagesize,string tableName)
		{
			if(sql == null || sql == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				int start = (currentPage-1)*pagesize;
				SqlDataAdapter da = new SqlDataAdapter(sql,con);
				DataSet ds = new DataSet();
				da.Fill(ds,start,pagesize,tableName);
				return ds.Tables[0].DefaultView;
			}
		}

		/// <summary>
		/// use SqlDataAdapter to fill the DataTable
		/// </summary>
		/// <param name="sql">存储过程|sql语句</param>
		/// <returns>DataTable</returns>
		public static DataTable ExecuteDataTable(string sql,CommandType type)
		{
			if(sql == null || sql == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				SqlDataAdapter da = new SqlDataAdapter();
				da.SelectCommand = new SqlCommand();
				da.SelectCommand .CommandText = sql;
				da.SelectCommand .CommandType = type;
				DataTable dt = new DataTable();
				da.Fill(dt);
				return dt;
			}
		}

		public static DataTable ExecuteDataTable(string sql,SqlParameter[] param,CommandType type)
		{
			if(sql == null || sql == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				SqlDataAdapter da = new SqlDataAdapter();
				da.SelectCommand = new SqlCommand ();
				da.SelectCommand .CommandText = sql;
				da.SelectCommand .CommandType = type;
				foreach(SqlParameter p in param)
				{
					da.SelectCommand .Parameters .Add(p);
				}
				DataTable dt = new DataTable();
				da.Fill(dt);
				return dt;
			}
		}
		#endregion

		#region Executscalar Method
		/// <summary>
		/// return a single value of the sql expression effected
		/// </summary>
		/// <param name="sqlString">CommandText</param>
		/// <returns>eturn a single value of the sql expression effected</returns>
		public static void ExecuteScalar(string sqlString,out int returnValue)
		{
			if(sqlString == null || sqlString == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				SqlCommand command = new SqlCommand (sqlString,con);
				returnValue = (int)command.ExecuteScalar();
			}
		}

		#endregion

		#region ExecutQuery方法
		/// <summary>
		/// execute the sql expression according to the sql
		/// </summary>
		/// <param name="con">连接对象</param>
		/// <param name="sqlString">存储过程|sql语句</param>
		/// <param name="type" >执行方式</param>
		public static void ExecuteQuery(string sqlString,CommandType type)
		{
			using(SqlConnection con = SqlBase.GetConnection())
			{
				if(sqlString == null || sqlString == "")
					throw new Exception("Sql Expression is null,please allocate it");
				con.Open();
				SqlCommand command = new SqlCommand ();
				command.Connection = con;
				command.CommandText = sqlString;
				command.CommandType = type;
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		///  带参数的执行过程
		/// </summary>
		/// <param name="con">连接对象</param>
		/// <param name="sqlString">存储过程|sql语句</param>
		/// <param name="param">Command参数数组</param>
		/// <param name="type" >执行方式</param>
		public static void ExecuteQuery(string sqlString,SqlParameter[] param,CommandType type)
		{
			if(sqlString == null || sqlString == "")
				throw new Exception("Sql Expression is null,please allocate it");
			using(SqlConnection con = SqlBase.GetConnection())
			{
				con.Open();
				SqlCommand command = con.CreateCommand();
				command.CommandText = sqlString;
				command.CommandType = type;
				foreach(SqlParameter p in param)
				{
					command.Parameters .Add(p);
				}
				command.ExecuteNonQuery();
			}
		}
		#endregion

		#region Create Param Method
		/// <summary>
		/// Create a SqlParameter and initialize it to the passed in values
		/// </summary>
		/// <param name="paramName">The name of the parameter being created.</param>
		/// <param name="dbType">The type in database this parameter represents.</param>
		/// <param name="size">The size of the parameter type if applicable.</param>
		/// <param name="direction">The direction the parameter is sending data.</param>
		/// <param name="oValue">The value of the parameter. Can Only assign a value if "direction" is Input or InputOutput.</param>
		/// <returns>An initialized SqlParameter</returns>
		public static SqlParameter CreateParam(string paramName, SqlDbType dbType, int size, ParameterDirection direction, object oValue)
		{
			SqlParameter sp;
			
			if (size != 0)
				sp = new SqlParameter(paramName, dbType, size);
			else
				sp = new SqlParameter(paramName, dbType);

			sp.Direction = direction;

			//Only assign a value for Input or InputOutput parameters
			if (oValue != null && (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput))
				sp.Value = oValue;

			return sp;
		}
		#endregion

		#region 数据库维护(备份,还原,压缩)
		/// <summary>
		/// 压缩数据库
		/// </summary>
		/// <param name="DBName">数据库名称</param>
		/// <param name="path">备份路径</param>
		public static void BackUpDatabase(string DBName,string path)
		{
			Debug.Assert (DBName != "" || DBName != null,"Invalid DBName,please check it and try again");
			Debug.Assert(path != "" || path != null,"Invalid file Path,please check it and try again");
			using(SqlConnection con = SqlBase .GetConnection())
			{
				con.Open();
				SqlBase.ExecuteQuery("BACKUP DATABASE "+DBName+" TO DISK = '"+ path +"'",CommandType.Text);
			}
		}
		/// <summary>
		/// 还原数据库
		/// 还原数据库的时候必须借助系统表来实现,即关闭原数据库的进程,在还原,否则会冲突
		/// </summary>
		/// <param name="DBName">数据库名</param>
		/// <param name="path">还原路径</param>
		public static void RestoreDataBase(string DBName,string path)
		{
			string sql = null;
			Debug.Assert (DBName != "" || DBName != null,"Invalid DBName,please check it and try again");
			Debug.Assert(path != "" || path != null,"Invalid file Path,please check it and try again");
			sql = "SELECT spid FROM master.dbo.sysprocesses WHERE dbid = DB_ID('"+ DBName +"')";
			SqlDataReader read = SqlBase.ExecuteDataReader(sql,CommandType.Text);
			while(read.Read())
			{
				sql += "kill " + read[0].ToString(); 
			}
			read.Close();
			SqlBase.ExecuteQuery(sql,CommandType.Text);

			sql = "RESTORE DATABASE "+DBName+" from DISK = '"+ path +"'";
			SqlBase.ExecuteQuery(sql,CommandType.Text);
			
		}
		/// <summary>
		/// 收缩数据库(需指定收缩大小)
		/// </summary>
		/// <param name="DBName">数据库名</param>
		/// <param name="shrinkSize" >收缩数据库的大小</param>
		public static void ShrinkDB(string DBName,int shrinkSize)
		{
			Debug.Assert (DBName != "" || DBName != null,"Invalid DBName,please check it and try again");
			string sql = "DBCC SHRINKDATABASE(" + DBName + "," +shrinkSize+")";
			SqlBase.ExecuteQuery(sql,CommandType.Text);
		}
		/// <summary>
		/// 收缩数据库(不需指定收缩大小,将收缩空间释放给系统)
		/// </summary>
		/// <param name="DBName">数据库名</param>
		public static void ShrinkDB(string DBName)
		{
			Debug.Assert (DBName != "" || DBName != null,"Invalid DBName,please check it and try again");
			string sql = "DBCC SHRINKDATABASE(" + DBName + ",TRUNCATEONLY)";
			SqlBase.ExecuteQuery(sql,CommandType.Text);
		}

		#endregion
	}
}
