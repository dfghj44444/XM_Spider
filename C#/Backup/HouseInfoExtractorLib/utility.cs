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
	/// utility ��ժҪ˵����
	/// </summary>
	public class utility
	{
		public utility()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
	}

	#region	��д INI �ı��ļ�
	/// <summary>
	/// 1��IniFile()		���캯��
	/// 2��GetKeyValue()	��ȡ��ֵ		
	/// 3��WriteKeyValue()	д���ֵ
	/// </summary>
	public class IniFile
	{
		private string m_IniFileName=null;
	
		//����AIP������дINI�ļ�
		[DllImport("Kernel32")]
		private static extern int GetPrivateProfileString(
			string lpAppName,string lpKeyName,string lpDefault,
			StringBuilder lpReturnedString,int nSize,string lpFileName);
		[DllImport("Kernel32")]
		private static extern bool WritePrivateProfileString(
			string lpAppName,string lpKeyNaem,string lpString,
			string lpFileName);
		/// <summary>
		/// ���캯�����ṩINI�ļ�����INI�ļ�λ��Ӧ�ó��������ļ����¡�
		/// </summary>
		/// <param name="IniFileName">INI�ļ���</param>
		public  IniFile(string IniFileName)
		{
			m_IniFileName=".\\"+IniFileName;
		}
		/// <summary>
		/// �������Ľ����Ƽ������������ض��Ľ�ֵ��
		/// </summary>
		/// <param name="sectionName">����</param>
		/// <param name="keyName">����</param>
		/// <returns>��ֵ</returns>
		public string GetKeyValue(string sectionName,string keyName)
		{
			StringBuilder keyValue=new StringBuilder(51000,51000);	// �����Դ��� 20 �� URL
			GetPrivateProfileString(sectionName,keyName,"",keyValue,500,m_IniFileName);
			return keyValue.ToString();
		}
		/// <summary>
		///�������Ľ���/����/��ֵ���ļ�ֵ�� 
		/// </summary>
		/// <param name="sectionName">����</param>
		/// <param name="keyName">����</param>
		/// <param name="keyValue">��ֵ</param>
		public void WriteKeyValue(string sectionName,string keyName,string keyValue)
		{
			WritePrivateProfileString(sectionName,keyName,keyValue,m_IniFileName);
		}
	}
	#endregion

	#region ����������ָ���� url
	/// <summary>
	/// ����������ָ���� url
	/// </summary>
	public class GetPage
	{
		#region ˽�б���
		/// <summary>
		/// ��ҳURL��ַ
		/// </summary>
		private string url=null;
		/// <summary>
		/// �Ƿ�ʹ�ô����������0 ��ʹ��  1 ʹ�ô��������
		/// </summary>
		private int proxyState=0;
		/// <summary>
		/// �����������ַ
		/// </summary>
		private string proxyAddress=null;
		/// <summary>
		/// ����������˿�
		/// </summary>
		private string proxyPort=null;
		/// <summary>
		/// ����������û���
		/// </summary>
		private string proxyAccount=null;
		/// <summary>
		/// �������������
		/// </summary>
		private string proxyPassword=null;
		/// <summary>
		/// �����������
		/// </summary>
		private string proxyDomain=null;
		/// <summary>
		/// ����ļ�·��
		/// </summary>
		private string outFilePath=null;
		/// <summary>
		/// ������ַ���
		/// </summary>
		private string outString=null;
		/// <summary>
		/// ��ʾ��Ϣ
		/// </summary>
		private string noteMessage=null;
		#endregion

		#region ��������
		/// <summary>
		/// ����ȡ��URL��ַ
		/// </summary>
		public string Url
		{
			get{return url;}
			set{url=value;}
		}
		/// <summary>
		/// �Ƿ�ʹ�ô����������־
		/// </summary>
		public int ProxyState
		{
			get{return proxyState;}
			set{proxyState=value;}
		}
		/// <summary>
		/// �����������ַ
		/// </summary>
		public string ProxyAddress
		{
			get{return proxyAddress;}
			set{proxyAddress=value;}
		}
		/// <summary>
		/// ����������˿�
		/// </summary>
		public string ProxyPort
		{
			get{return proxyPort;}
			set{proxyPort=value;}
		}
		/// <summary>
		/// ����������˺�
		/// </summary>
		public string ProxyAccount
		{
			get{return proxyAccount;}
			set{proxyAccount=value;}
		}
		/// <summary>
		/// �������������
		/// </summary>
		public string ProxyPassword
		{
			get{return proxyPassword;}
			set{proxyPassword=value;}
		}
		/// <summary>
		/// �����������
		/// </summary>
		public string ProxyDomain
		{
			get{return proxyDomain;}
			set{proxyDomain=value;}
		}
		/// <summary>
		/// ����ļ�·��
		/// </summary>
		public string OutFilePath
		{
			get{return outFilePath;}
			set{outFilePath=value;}
		}
		/// <summary>
		/// ���ص��ַ���
		/// </summary>
		public string OutString
		{
			get{return outString;}
		}
		/// <summary>
		/// ������ʾ��Ϣ
		/// </summary>
		public string NoteMessage
		{
			get{return noteMessage;}
		}
		#endregion
  
		#region ���캯��
		public GetPage()
		{
		}
		#endregion

		#region �������� GetHtml
		/// <summary>
		/// ��ȡָ��URL��ַ������Html���봮
		/// </summary>
		public string GetHtml() 
		{ 
			string tempCode = null;
			if(this.url==null | this.url.Length==0)
			{
				noteMessage = "Url ����Ϊ��...";
				return tempCode;
			}
			WebRequest request = WebRequest.Create(this.url);
			if(this.proxyState==1)//ʹ�ô���������Ĵ���
			{
				//Ĭ�϶�ȡ80�˿ڵ�����
				if(this.proxyPort==null)
					this.ProxyPort="80";
				WebProxy myProxy=new WebProxy(); 
				myProxy = (WebProxy)request.Proxy; 
				myProxy.Address = new Uri(this.ProxyAddress+":"+this.ProxyPort); 
				myProxy.Credentials = new NetworkCredential(this.proxyAccount, this.proxyPassword, this.ProxyDomain);
				request.Proxy = myProxy; 
			}
			try// ����ָ���� url
			{
				//request.Timeout = 2000;
				WebResponse response = request.GetResponse();									//�������
				Stream resStream = response.GetResponseStream();								//������Ϣ
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
