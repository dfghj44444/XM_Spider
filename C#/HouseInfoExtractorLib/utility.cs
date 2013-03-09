using System;
using System.IO;
using System.Net;
using System.Web;
using System.Data;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace HouseInfoExtractorLib
{
	/// <summary>
	/// utility 的摘要说明。
	/// </summary>
	public class utility
	{
		public utility()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
	}

	#region	读写 INI 文本文件
	/// <summary>
	/// 1、IniFile()		构造函数
	/// 2、GetKeyValue()	读取键值		
	/// 3、WriteKeyValue()	写入键值
	/// </summary>
	public class IniFile
	{
		private string m_IniFileName=null;
	
		//导入AIP函数读写INI文件
		[DllImport("Kernel32")]
		private static extern int GetPrivateProfileString(
			string lpAppName,string lpKeyName,string lpDefault,
			StringBuilder lpReturnedString,int nSize,string lpFileName);
		[DllImport("Kernel32")]
		private static extern bool WritePrivateProfileString(
			string lpAppName,string lpKeyNaem,string lpString,
			string lpFileName);
		/// <summary>
		/// 构造函数：提供INI文件名；INI文件位于应用程序所在文件夹下。
		/// </summary>
		/// <param name="IniFileName">INI文件名</param>
		public  IniFile(string IniFileName)
		{
			m_IniFileName=".\\"+IniFileName;
		}
		/// <summary>
		/// 按给定的节名称及键名，返回特定的健值。
		/// </summary>
		/// <param name="sectionName">节名</param>
		/// <param name="keyName">键名</param>
		/// <returns>键值</returns>
		public string GetKeyValue(string sectionName,string keyName)
		{
			StringBuilder keyValue=new StringBuilder(51000,51000);	// 最大可以处理 20 个 URL
			GetPrivateProfileString(sectionName,keyName,"",keyValue,500,m_IniFileName);
			return keyValue.ToString();
		}
		/// <summary>
		///按给定的节名/键名/键值更改键值。 
		/// </summary>
		/// <param name="sectionName">节名</param>
		/// <param name="keyName">键名</param>
		/// <param name="keyValue">键值</param>
		public void WriteKeyValue(string sectionName,string keyName,string keyValue)
		{
			WritePrivateProfileString(sectionName,keyName,keyValue,m_IniFileName);
		}
	}
	#endregion

	#region 从网络下载指定的 url
	/// <summary>
	/// 从网络下载指定的 url
	/// </summary>
	public class GetPage
	{
		#region 私有变量
		/// <summary>
		/// 网页URL地址
		/// </summary>
		private string url=null;
		/// <summary>
		/// 是否使用代码服务器：0 不使用  1 使用代理服务器
		/// </summary>
		private int proxyState=0;
		/// <summary>
		/// 代理服务器地址
		/// </summary>
		private string proxyAddress=null;
		/// <summary>
		/// 代理服务器端口
		/// </summary>
		private string proxyPort=null;
		/// <summary>
		/// 代理服务器用户名
		/// </summary>
		private string proxyAccount=null;
		/// <summary>
		/// 代理服务器密码
		/// </summary>
		private string proxyPassword=null;
		/// <summary>
		/// 代理服务器域
		/// </summary>
		private string proxyDomain=null;
		/// <summary>
		/// 输出文件路径
		/// </summary>
		private string outFilePath=null;
		/// <summary>
		/// 输出的字符串
		/// </summary>
		private string outString=null;
		/// <summary>
		/// 提示信息
		/// </summary>
		private string noteMessage=null;
		#endregion

		#region 公共属性
		/// <summary>
		/// 欲读取的URL地址
		/// </summary>
		public string Url
		{
			get{return url;}
			set{url=value;}
		}
		/// <summary>
		/// 是否使用代理服务器标志
		/// </summary>
		public int ProxyState
		{
			get{return proxyState;}
			set{proxyState=value;}
		}
		/// <summary>
		/// 代理服务器地址
		/// </summary>
		public string ProxyAddress
		{
			get{return proxyAddress;}
			set{proxyAddress=value;}
		}
		/// <summary>
		/// 代理服务器端口
		/// </summary>
		public string ProxyPort
		{
			get{return proxyPort;}
			set{proxyPort=value;}
		}
		/// <summary>
		/// 代理服务器账号
		/// </summary>
		public string ProxyAccount
		{
			get{return proxyAccount;}
			set{proxyAccount=value;}
		}
		/// <summary>
		/// 代理服务器密码
		/// </summary>
		public string ProxyPassword
		{
			get{return proxyPassword;}
			set{proxyPassword=value;}
		}
		/// <summary>
		/// 代理服务器域
		/// </summary>
		public string ProxyDomain
		{
			get{return proxyDomain;}
			set{proxyDomain=value;}
		}
		/// <summary>
		/// 输出文件路径
		/// </summary>
		public string OutFilePath
		{
			get{return outFilePath;}
			set{outFilePath=value;}
		}
		/// <summary>
		/// 返回的字符串
		/// </summary>
		public string OutString
		{
			get{return outString;}
		}
		/// <summary>
		/// 返回提示信息
		/// </summary>
		public string NoteMessage
		{
			get{return noteMessage;}
		}
		#endregion
  
		#region 构造函数
		public GetPage()
		{
		}
		#endregion

		#region 公共方法 GetHtml
		/// <summary>
		/// 读取指定URL地址，返回Html代码串
		/// </summary>
		public string GetHtml() 
		{ 
			string tempCode = null;
			if(this.url==null | this.url.Length==0)
			{
				noteMessage = "Url 不能为空...";
				return tempCode;
			}
			WebRequest request = WebRequest.Create(this.url);
			if(this.proxyState==1)//使用代理服务器的处理
			{
				//默认读取80端口的数据
				if(this.proxyPort==null)
					this.ProxyPort="80";
				WebProxy myProxy=new WebProxy(); 
				myProxy = (WebProxy)request.Proxy; 
				myProxy.Address = new Uri(this.ProxyAddress+":"+this.ProxyPort); 
				myProxy.Credentials = new NetworkCredential(this.proxyAccount, this.proxyPassword, this.ProxyDomain);
				request.Proxy = myProxy; 
			}
			try// 下载指定的 url
			{
				//request.Timeout = 2000;
				WebResponse response = request.GetResponse();									//请求服务
				Stream resStream = response.GetResponseStream();								//返回信息
				StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
				tempCode= sr.ReadToEnd();
				resStream.Close(); 
				sr.Close();
			}
			catch(Exception e1)
			{
				this.noteMessage = e1.Message.ToString();
			}
			return tempCode;
		}
		#endregion
	}
	#endregion

}
