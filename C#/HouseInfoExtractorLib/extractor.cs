using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace HouseInfoExtractorLib
{
	//������Ϣ�Ľṹ��
	public struct HouseInfo
	{
		public string source;				//��Ϣ��Դ
		public int    cityID;				//���б��
		public int    urltype;				//��վ���
		public string address;				//��ַ
		public string region;				//����
		public string fitment;				//װ�����
		public string houseType ;			//��������
		public string property;				//��Ȩ
		public string doneDate;				//�������
		public int    room;					//����
		public int    hall;					//����
		public int    washRoom;				//����
		public string managerName;  		//��ҵ����
		public string managerType;  		//��ҵ���
		public string floor;        		//¥��
		public string area;         		//���
		public double price;        		//�۸�
		public string remark;       		//��ע
		public string linkMan;      		//��ϵ��
		public string linkPhone;    		//��ϵ�绰
		public DateTime registerDate;			//����ʱ��
		public string email;        		//email  -----��ʱ������
		public string rentType;     		//���޷�ʽ
		public string direction;    		//����			
		public string traffic;				//��ͨ״��
		public string schoolInfo;   		//ѧ����Ϣ
		public string houseStructure;		//���ݽṹ
		public string baseEstablishment;	//������ʩ
		public string equipment;			//�豸
		public string environment;          //����
		public string url;                  //���ڴ洢������ҳ��URL
	}

	/// <summary>
	/// Class1 ��ժҪ˵����
	/// </summary>
	public class HouseInfoExtractor
	{
		public HouseInfoExtractor()
		{	
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		

		public struct Attribute
		{
			public string key;			
			public string keyvalue;
		}
		
		public struct charNum{
			public char charnum;
			public char bigCharnum;
			public string num;
		}
		public HouseInfo houseInfo;
		public charNum[] charNums = new charNum[10];
		public bool hasRegion = false;		
	
	
		/// <summary>
		/// ��ȡָ����ҳ���ݵ���Ϣ
		/// </summary>
		/// <param name="pageContent">ָ������ҳ����</param>
		/// <returns>������Ϣ�Ľṹ��</returns>
		public HouseInfo extract(string content,XmlNodeList keyWordsLists){
		
			houseInfo = new HouseInfo();
		    houseInfo.source="";				
			houseInfo.address="";				
			houseInfo.region="";				
			houseInfo.fitment="";				
			houseInfo.houseType="" ;			
			houseInfo.property="";				
			houseInfo.doneDate="";							
			houseInfo.managerName="";  		
			houseInfo.managerType="";  		    		
			houseInfo.area="";         		
			houseInfo.remark="";       		
			houseInfo.linkMan="";      		
			houseInfo.linkPhone="";    		
			houseInfo.email="";        		
			houseInfo.rentType="";     		
			houseInfo.direction="";    		
			houseInfo.traffic="";				
			houseInfo.schoolInfo="";   		
			houseInfo.houseStructure="";		
			houseInfo.baseEstablishment="";	
			houseInfo.equipment="";			
			houseInfo.environment=""; 
			houseInfo.floor ="";
	            
			content = content.Replace(" ","��");
			string pageContent = this.CutHtml(content);			
			houseInfo.registerDate = DateTime.Today;
			HtmlParser parser = new HtmlParser(pageContent);
			ArrayList tags = parser.GetValidTag();

			bool add = false;			
			string key=null;
			Attribute attribute = new Attribute();
			for(int k=0;k<tags.Count;k++)
			{
				HtmlText tag = (HtmlText)tags[k];
				string tagName=tag.TagName;			
				key=null;
				key = tag.Text.Replace("��",".");
				key = key.Replace("��",",");
				key  = key.Replace("��",",");
				key  = key.Replace("��",";");
				key  = key.Replace("��","!");
				key  = key.Replace("��","?");
				key = Strings.StrConv(key,VbStrConv.Narrow,0); // ȫ��ת���
				key = key.Trim(new char[]{':','��'});
				if(key.Trim()=="")
					continue;
				string tempValue = null;
				if(key.StartsWith("��")){
					key = key.Remove(0,1);
				}
				if(key.EndsWith("��"))
				{
					key = key.Remove(key.Length-1,1);
				}
				if(key.IndexOf("[")!=-1)
				{
					key = key.Replace("[","");
				}
				string tempKey = key;
				if (tempKey.IndexOf(":") != -1 && tempKey.IndexOf(":") < tempKey.Length-1) 
				{
					tempValue = tempKey.Substring(tempKey.IndexOf(":") + 1);
					tempKey = tempKey.Substring(0, tempKey.IndexOf(":"));
					tempKey = tempKey.Replace(" ", "");
				}
				else if (tempKey.IndexOf("��") != -1&& tempKey.IndexOf("��") < tempKey.Length-1) 
				{
					tempValue = tempKey.Substring(tempKey.IndexOf("��") + 1);
					tempKey = tempKey.Substring(0, tempKey.IndexOf("��"));
					tempKey = tempKey.Replace(" ", "");
				}
				else if (tempKey.IndexOf("]") != -1&& tempKey.IndexOf("]") < tempKey.Length-1) 
				{
					tempValue = tempKey.Substring(tempKey.IndexOf("]") + 1);
					tempKey = tempKey.Substring(0, tempKey.IndexOf("]"));
					tempKey = tempKey.Replace(" ", "");
				}
				else 
				{
					tempKey = key.Replace(" ", "");
					tempValue = "";
				}
				bool isNull = false;
				#region -----�ж���ӿ���----- 
				if(add==true)
				{
					for(int i = 0;i<keyWordsLists.Count;i++)
					{
						if(attribute.key == "��Ȩ"&&tempKey.Replace(" ","")=="��Ȩ")
							break;
						string keyWordList = keyWordsLists[i].ChildNodes[0].Value;
						string[] lists = keyWordList.Split(new char[]{'#'});
						if(this.Contain(lists,tempKey.Replace(" ","")))
						{
							add = false;
							isNull = true;
							break;							
						}
					}
					if(!isNull)
					{
						attribute.keyvalue = key;
						add = false;
						insert(attribute);
						continue;
					}						
				}
				#endregion
				
				#region		---�ؼ���ƥ��---
				for(int i = 0;i<keyWordsLists.Count;i++)
				{
					string keyWordList = keyWordsLists[i].ChildNodes[0].Value;
					string[] lists = keyWordList.Split(new char[]{'#'});
					if(this.Contain(lists,tempKey))
					{						
						XmlNode node = keyWordsLists[i];
						XmlElement xe = (XmlElement)node;
						attribute.key = xe.GetAttribute("keyname");						
						if(tempValue.Length>0)
						{
							attribute.keyvalue = tempValue;
							insert(attribute);
						}
						else
							add = true;
						break;
					}
				
				}
				#endregion
			}
			
			if(houseInfo.region.IndexOf("��")!=-1)
				if(houseInfo.region.IndexOf("��")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("��")+1);
			if(houseInfo.region.IndexOf("��")!=-1)
				houseInfo.region = houseInfo.region.Substring(0,houseInfo.region.IndexOf("��"));
			if(houseInfo.region.IndexOf("/")!=-1)
				if(houseInfo.region.IndexOf("/")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("/")+1);
			if(houseInfo.region.IndexOf("\\")!=-1)
				if(houseInfo.region.IndexOf("\\")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("\\")+1);
			return houseInfo;
		}


		/// <summary>
		/// ����ȡ������Ϣ���뷿����Ϣ�ṹ����
		/// </summary>
		/// <param name="attribute">��ȡ������Ϣ</param>
		public void insert(Attribute attribute)
		{
			string floorType = null;
			attribute.keyvalue = attribute.keyvalue.Trim();
			if(attribute.key == "��Դ")
			{
				houseInfo.source = attribute.keyvalue;
				if(attribute.keyvalue == "��"||attribute.keyvalue=="�н�"||attribute.keyvalue=="����ί���н�"||attribute.keyvalue=="ί���н�")
					houseInfo.source = "�н�";
				else if(attribute.keyvalue == "��"||attribute.keyvalue == "����"||attribute.keyvalue.IndexOf("���н�")!=-1||attribute.keyvalue.IndexOf("�����н�")!=-1||attribute.keyvalue.IndexOf("����")!=-1)
					houseInfo.source = "����";
			}
			else if(attribute.key == "��ַ")
			{
				string region="";
				if(attribute.keyvalue.IndexOf("��")>=2)
				{
					region = attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("��")-2,3);
					if(houseInfo.region==""&attribute.keyvalue.Length>3)
						houseInfo.region = region;
				}
				if(houseInfo.address=="")
					houseInfo.address =  attribute.keyvalue;			
			}
			else if(attribute.key == "����")
			{
				if(attribute.keyvalue.IndexOf("����")==-1&attribute.keyvalue.IndexOf("����")==-1){
					houseInfo.region = attribute.keyvalue;
					//hasRegion =true;
				}
			}
			else if(attribute.key == "����������"){
				if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf(" ")!=-1&attribute.keyvalue.IndexOf(" ")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf(" ")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("��")!=-1&attribute.keyvalue.IndexOf("��")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("��")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("/")!=-1&attribute.keyvalue.IndexOf("/")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("/")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("\\")!=-1&attribute.keyvalue.IndexOf("\\")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("\\")+1);
				else if(houseInfo.region=="")
					houseInfo.region =  attribute.keyvalue;
			}
			else if(attribute.key == "װ��")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.fitment = attribute.keyvalue;
			}
			//���Ľ� �ҡ�������
			else if(attribute.key == "����")
			{	
				string type = attribute.keyvalue;
				if(type.IndexOf("��")!=-1|type.IndexOf("��")!=-1|type.IndexOf("��")!=-1|type.IndexOf("��")!=-1)
				{
					this.processHouseType(type);
				}
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.houseType =  attribute.keyvalue;
			}
			else if(attribute.key == "���ݽṹ")
			{	
				string type = attribute.keyvalue;
				if(type.IndexOf("��")!=-1|type.IndexOf("��")!=-1||type.IndexOf("��")!=-1||type.IndexOf("��")!=-1)
				{
					this.processHouseType(type);
				}
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.houseStructure =  attribute.keyvalue;
			}
			else if(attribute.key == "��Ȩ")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.property =  attribute.keyvalue;
			}
			else if(attribute.key == "��������")
			{
				if(attribute.keyvalue.IndexOf("��")!=-1)
					attribute.keyvalue = attribute.keyvalue.Replace("��","");
				else if(attribute.keyvalue == "δ֪")
					attribute.keyvalue = "";
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.doneDate = attribute.keyvalue;
			}
			else if(attribute.key == "��ҵ����")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.managerName =  attribute.keyvalue;
			}
			else if(attribute.key == "��ҵ���")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.managerType =  attribute.keyvalue;
			}
			else if(attribute.key == "¥��")
			{			
				floorType = "at";
				houseInfo.floor = this.floorProcess(attribute.keyvalue,floorType);		
			}
			else if(attribute.key == "��¥��")
			{
				floorType = "all";
				houseInfo.floor = this.floorProcess(attribute.keyvalue,floorType);	
			}
			else if(attribute.key == "���")
			{
				if(attribute.keyvalue != ""|attribute.keyvalue!= null&houseInfo.area=="")
				{
					char[] sizeArray = attribute.keyvalue.ToCharArray();
					string sizeValue = "";
					for(int i = 0;i<sizeArray.Length;i++)
					{
						if('0' <= sizeArray[i] &&sizeArray[i] <= '9'||sizeArray[i]=='.')
						{
							sizeValue +=sizeArray[i].ToString();					
						}
						else if(sizeValue =="")
						{
							continue;
						}
						else
						{
							break;
						}					
					}
					houseInfo.area = sizeValue;
				}

			}
			else if(attribute.key == "���޷�ʽ")
			{
				if(attribute.keyvalue != "" |attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("����")!=-1|attribute.keyvalue.IndexOf("������")!=-1|attribute.keyvalue.IndexOf("������")!=-1|attribute.keyvalue.IndexOf("��������")!=-1)
						houseInfo.rentType = "����";
					if(attribute.keyvalue.IndexOf("����")!=-1|attribute.keyvalue.IndexOf("�Ǻ���")!=-1|attribute.keyvalue.IndexOf("������")!=-1|attribute.keyvalue.IndexOf("���Ǻ���")!=-1)
						houseInfo.rentType = "����";
					#region-------------------�ж�����\����Ͷ���--------------------------
//					else if(attribute.keyvalue.IndexOf("����")!=-1)
//						houseInfo.rentType = "����";
//					else if(attribute.keyvalue.IndexOf("����")!=-1)
//						houseInfo.rentType = "����";
//					else if(attribute.keyvalue.IndexOf("����")!=-1)
//						houseInfo.rentType = "����";
				}
			}
//			else if(attribute.key == "����")
//			{
//				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
//				{
//					if(attribute.keyvalue.IndexOf("��")!=-1||attribute.keyvalue.IndexOf("��")!=-1)
//						houseInfo.rentType =  "����";
//					else if(attribute.keyvalue.IndexOf("��")!=-1)
//						houseInfo.rentType =  "����";
//					else
//						houseInfo.rentType = attribute.keyvalue;
//				}
//			}
//			else if(attribute.key == "����")
//			{
//				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
//				{
//					if(attribute.keyvalue.IndexOf("��")!=-1||attribute.keyvalue.IndexOf("��")!=-1)
//						houseInfo.rentType =  "����";
//					else if(attribute.keyvalue.IndexOf("��")!=-1)
//						houseInfo.rentType =  "����";
//					else
//						houseInfo.rentType = attribute.keyvalue;
//				}
//			}
				#endregion
			else if(attribute.key == "����")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("��")!=-1|attribute.keyvalue.IndexOf("��")!=-1)
						houseInfo.rentType =  "����";
					else if(attribute.keyvalue.IndexOf("��")!=-1|attribute.keyvalue.IndexOf("��")!=-1|attribute.keyvalue.IndexOf("��")!=-1)
						houseInfo.rentType =  "����";
					else
						houseInfo.rentType = attribute.keyvalue;
				}
			}
			else if(attribute.key == "����")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("��")!=-1||attribute.keyvalue.IndexOf("��")!=-1|attribute.keyvalue.IndexOf("��")!=-1)
						houseInfo.rentType =  "����";
					else if(attribute.keyvalue.IndexOf("��")!=-1)
						houseInfo.rentType =  "����";
					else
						houseInfo.rentType = attribute.keyvalue;
				}
			}
			else if(attribute.key == "�۸�")
			{
				if(attribute.keyvalue != "" &&attribute.keyvalue.IndexOf("����")==-1&&attribute.keyvalue.IndexOf("����")==-1)
					houseInfo.price =  this.priceProcess(attribute.keyvalue);	
			}
			else if(attribute.key == "��ע")
			{
				Regex r = new Regex("\\d{3}-\\d{8}|\\d{4}-\\d{7}|\\d{11}|\\d{7}|\\d{8}"); // ����һ��Regex����ʵ��
				Match m = r.Match(attribute.keyvalue); // ���ַ�����ƥ��
				if (m.Success) 
				{
					if(houseInfo.linkPhone=="")
						houseInfo.linkPhone = m.Value;
					//����ƥ���ַ���λ��
					attribute.keyvalue = attribute.keyvalue.Replace(m.Value,"");
				}
				
				if(houseInfo.remark !=""&&attribute.keyvalue!="")
				{
					houseInfo.remark += ";"  + attribute.keyvalue;	
				}
				else if(houseInfo.remark == ""&&attribute.keyvalue!="")
					houseInfo.remark =  attribute.keyvalue.Trim();
			}
			else if(attribute.key == "��ϵ��")
			{
				if(houseInfo.linkMan != "")
					houseInfo.linkMan += ";"+attribute.keyvalue;
				else
					houseInfo.linkMan = attribute.keyvalue;
			}
			else if(attribute.key == "��ϵ�绰")
			{
				char[] tel = attribute.keyvalue.ToCharArray();
				attribute.keyvalue = "";
				char[] sign = new char[]{'/','\\',',',';','��','��'};
				foreach(char c in tel)
				{
					if((c>='0'&&c<='9')||charContain(sign,c)||c==' ')
						attribute.keyvalue += c.ToString();	
				}
				if(attribute.keyvalue != ""&&houseInfo.linkPhone!="")
					houseInfo.linkPhone += ";"+ attribute.keyvalue;
				else
					houseInfo.linkPhone = attribute.keyvalue;
			}
			else if(attribute.key == "ѧ��")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.schoolInfo =  attribute.keyvalue;
			}
			
			else if(attribute.key == "�Ǽ�����")
			{
				int len=0;
				if(attribute.keyvalue.IndexOf("��")!=-1&attribute.keyvalue.IndexOf("����")==-1)
					houseInfo.registerDate = Convert.ToDateTime(attribute.keyvalue.Substring(0,attribute.keyvalue.IndexOf("��")+1));
				else if(attribute.keyvalue.LastIndexOf(".")!=-1)
				{
					try
					{
						int day = Convert.ToInt32(attribute.keyvalue.Substring(attribute.keyvalue.LastIndexOf(".")+1,2));
						if(day<=31)
						{
							len=3;
						}
						else
						{
							len=2;
						}
					}
					catch(Exception e)
					{
						len=3;
					}
					try
					{
						string temp = attribute.keyvalue.Substring(0,attribute.keyvalue.IndexOf(".")+len);
						houseInfo.registerDate = Convert.ToDateTime(temp);	
					}
					catch(Exception e)
					{
						houseInfo.registerDate = DateTime.Now;
					}	
	
				}	
				else if(attribute.keyvalue.LastIndexOf("-")!=-1)	
				{		
					try
					{						
						int day = Convert.ToInt32(attribute.keyvalue.Substring(attribute.keyvalue.LastIndexOf("-")+1,2));
						if(day<=31)						
							len=3;				
						else
							len=2;
					}
					catch(Exception e)
					{
						len=3;
					}
					try
					{
						string temp = attribute.keyvalue.Substring(0,attribute.keyvalue.LastIndexOf("-")+len);
						houseInfo.registerDate = Convert.ToDateTime(temp);		
					}
					catch(Exception e)
					{
						houseInfo.registerDate = DateTime.Now;
					}	
				}
				else
					houseInfo.registerDate = DateTime.Today;
			}
			else if(attribute.key == "Email")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.email =  attribute.keyvalue;
			}
			else if(attribute.key == "����")
			{		
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.direction =  attribute.keyvalue;
			}

			else if(attribute.key == "��ͨ")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.traffic =  attribute.keyvalue;
			}
			else if(attribute.key == "����")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.environment =  attribute.keyvalue;
			}
		
			else if(attribute.key == "������ʩ")
			{
				if(houseInfo.baseEstablishment !=""&&attribute.keyvalue!="")
				{
					houseInfo.baseEstablishment += ";"  + attribute.keyvalue;	
				}
				else if(houseInfo.baseEstablishment==""&&attribute.keyvalue!="")
					houseInfo.baseEstablishment =  attribute.keyvalue;
			}
			else if(attribute.key == "�豸")
			{
				if(houseInfo.equipment !=""&&attribute.keyvalue!="")
				{
					houseInfo.equipment += ";"  + attribute.keyvalue;	
				}
				else if(houseInfo.equipment == "")
					houseInfo.equipment =  attribute.keyvalue;
			}

		}

		/// <summary>
		/// ����¥����Ϣ
		/// </summary>
		/// <param name="s">ԭ¥����Ϣ</param>
		/// <returns>������¥����Ϣ</returns>
		public string floorProcess(string s,string type)
		{
			bool atFloor = false;
			bool allFloor = false;
			if(type == "at")
				atFloor=true;
			if(type == "all")
				allFloor=true;
			string floor = "";
			string floors = "";
			string floorInfo = "";
			char[] array = s.ToCharArray();
			for(int i = 0;i<array.Length;i++)
			{
				if(array[i]=='��')
				{
					allFloor = false;
					atFloor = true;
					continue;
				}
				else if(array[i]=='��'||array[i]=='\\'||array[i]=='/'||array[i]=='-')
				{
					allFloor = true;
					atFloor = false;
					continue;
				}
//				else if(array[i]=='¥'||array[i]=='��')
//				{
//					atFloor = false;
//					allFloor = false;
//					continue;
//				}
				else if('0' <= array[i] &array[i] <= '9'|array[i]=='+'|array[i]=='-')
				{
					if(atFloor)
					{
						floor +=array[i].ToString();
					}
					if(allFloor)
					{
						floors += array[i].ToString();
					}					
				}
				else if(this.charContain(charNums,array[i])!="")
				{
					if(atFloor)
					{
						if(floor!="")
						{
							floor = Convert.ToString((Convert.ToInt32(floor) + Convert.ToInt32(charContain(charNums,array[i]))));
						
						}
						else
						{
							floor += charContain(charNums,array[i]);
						
						}
					}
					if(allFloor)
					{
						if(floors!="")
						{
							floors = Convert.ToString((Convert.ToInt32(floor) + Convert.ToInt32(charContain(charNums,array[i]))));
						
						}
						else
						{
							floors += charContain(charNums,array[i]);
						
						}
					}
				}
				else if(array[i]=='ʮ'||array[i]=='ʰ')
				{
					if(atFloor)
					{
						if(floor!="")
						{
							floor = Convert.ToString((Convert.ToInt32(floor)*10));				
											
						}
						else
						{
							floor = "10";
						}
					}
					if(allFloor)
					{
						if(floors!="")
						{
							floors = Convert.ToString((Convert.ToInt32(floors)*10));				
											
						}
						else
						{
							floors = "10";
						}
					}
				}
			}
			if(floor!="")
				if( houseInfo.floor!="")
					floorInfo = floor + "/" + houseInfo.floor;
				else
					floorInfo = floor + "/";
			if(floors!="")
				if(houseInfo.floor!="")
					floorInfo = houseInfo.floor + floors;
				else
					floorInfo += floors;
			return floorInfo;
		}


		/// <summary>
		/// ��ȡ�۸���ֵ��
		/// </summary>
		/// <param name="s">�۸��ַ���</param>
		/// <returns>��ֵ�ͼ۸�</returns>
		public double priceProcess(string s)
		{
			#region ----ΪcharNums����ֵ---------
			charNums[0].charnum = 'һ';
			charNums[0].bigCharnum = 'Ҽ';
			charNums[0].num = "1";
			charNums[1].charnum = '��';
			charNums[1].bigCharnum = '��';
			charNums[1].num = "2";			
			charNums[2].charnum = '��';
			charNums[2].bigCharnum = '��';
			charNums[2].num = "3";
			charNums[3].charnum = '��';
			charNums[3].bigCharnum = '��';
			charNums[3].num = "4";
			charNums[4].charnum = '��';
			charNums[4].bigCharnum = '��';
			charNums[4].num = "5";
			charNums[5].charnum = '��';
			charNums[5].bigCharnum = '½';
			charNums[5].num = "6";
			charNums[6].charnum = '��';
			charNums[6].bigCharnum = '��';
			charNums[6].num = "7";
			charNums[7].charnum = '��';
			charNums[7].bigCharnum = '��';
			charNums[7].num = "8";
			charNums[8].charnum = '��';
			charNums[8].bigCharnum = '��';
			charNums[8].num = "9";
			charNums[9].charnum = '��';
			charNums[9].bigCharnum = '��';
			charNums[9].num = "2";
			#endregion
			char[] array = s.Replace(" ","").ToCharArray();
			string priceStr = "";
			double price = 0;
			for(int i = 0;i<array.Length;i++)
			{
				if(('0' <= array[i] &&array[i] <= '9')||array[i]=='.'||array[i]=='��'||array[i]==','||array[i]==' '||array[i]=='��')
				{
					if(array[i]==',')
						continue;
					priceStr +=array[i].ToString();					
				}
				else if(this.charContain(charNums,array[i])!="")
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr) + Convert.ToDouble(charContain(charNums,array[i]))));
						
					}
					else
					{
						priceStr += charContain(charNums,array[i]);
						
					}
				}
				else if(array[i]=='��'||array[i]=='�f')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString(Convert.ToDouble(priceStr)*10000);
						price = Convert.ToDouble(priceStr);
						priceStr = "";
					}			
				}
				else if(array[i]=='ǧ'||array[i]=='Ǫ')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr)*1000));	
						price += Convert.ToDouble(priceStr);
						priceStr = "";
					}	
				}
				else if(array[i]=='��'||array[i]=='��')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr)*100));	
						price += Convert.ToDouble(priceStr);
						priceStr = "";
					}		
				}
				else if(array[i]=='ʮ'||array[i]=='ʰ')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr)*10));						
						price += Convert.ToDouble(priceStr);
						//priceStr = "";
					}
					else
					{
						priceStr = "10";
					}
				}
				else if(array[i]=='Ԫ'){
					break;
				}
		
			}
			if(price==0&&priceStr!="")
				price = Convert.ToDouble(priceStr);
			return price;
		}


		public string charContain(charNum[] nums,char c){
			string returnValue="";
			for(int i = 0;i<nums.Length;i++){
				if(c.Equals(nums[i].charnum)||c.Equals(nums[i].bigCharnum)){
					returnValue = nums[i].num;
				}
			}
			return returnValue;
		}

		public bool charContain(char[] nums,char c)
		{		
			bool returnValue = false;
			for(int i = 0;i<nums.Length;i++)
			{
				if(c == nums[i])
					returnValue = true;
			}
			return returnValue;
		}

		/// <summary>
		/// �������Ϊ�������Ҽ�����
		/// </summary>
		/// <param name="type"></param>
		public void processHouseType(string type){
			if(type.IndexOf("��")!=-1){
				int i = type.IndexOf("��");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("��")!=-1)
			{
				int i = type.IndexOf("��");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("��")!=-1)
			{
				int i = type.IndexOf("��");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("��")!=-1){
				int i = type.IndexOf("��");
				if(i>0)
					houseInfo.hall = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.hall=0;
			}
			
			if(type.IndexOf("��")!=-1){
				int i = type.IndexOf("��");
				if(i>0)
					houseInfo.washRoom = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.washRoom=0;
			}
		}

		/// <summary>
		/// ����ת��Ϊint����
		/// </summary>
		/// <param name="s">����������</param>
		/// <returns>int������</returns>
		public int numProcesser(string s){
			int i=0;
			if(s =="һ" || s == "Ҽ")
			{
				i=1;
			}
			else if(s =="��" || s == "��")
			{
				i=2;
			}
			else if(s =="��" || s == "��")
			{
				i=3;
			}
			else if(s =="��" || s == "��")
			{
				i=4;
			}
			else if(s =="��" || s == "��")
			{
				i=5;
			}
			else if(s =="��")
			{
				i=0;
			}
			else if(Convert.ToChar(s)>='0'&&Convert.ToChar(s)<='9'){
				try
				{
					i = Convert.ToInt32(s);
				}
				catch(Exception e){
					i=0;
				}
			}
			return i;
		}

		/// <summary>
		/// �жϹؼ��ʿ����Ƿ�����˹ؼ���
		/// </summary>
		/// <param name="lists">�ؼ��ʿ�</param>
		/// <param name="key">�ؼ���</param>
		/// <returns>true or false</returns>
		public bool Contain(string[] lists,string key)
		{
			for(int i = 0;i<lists.Length;i++)
			{
				if(lists[i]==key.ToUpper())
				{
					return true;
				}
			}
			return false;
		}

		

		/// <summary>
		/// �����ҳ�в���Ҫ�ı�Ǻ�����
		/// </summary>
		/// <param name="pageContent"></param>
		/// <returns></returns>
		public string CutHtml(string pageContent)
		{
			string content = "";
			pageContent = pageContent.Replace("\n","");
			pageContent = pageContent.Replace("\r","");
			pageContent = pageContent.Replace("\t","");		

			content = pageContent.ToLower();
			int pos = 0;
			int tagEndPos = 0;
			int startPos = 0 ;	
			try
			{	
				while(content.IndexOf("<",startPos)!=-1)
				{					
					#region
					pos = content.IndexOf("<",startPos);
					int count = 0;
					string tempStr = content.Substring(pos+1,1).ToUpper();
					string tempEndStr = content.Substring(pos+1,2).ToUpper();
					if(tempStr=="T"||tempStr == "D" ||tempEndStr=="/T"||tempEndStr=="/D")
					{						
						startPos = content.IndexOf(">",pos);						
					}
						//cut <script></script>
					else if(content.Substring(pos+1,6).ToUpper()=="SCRIPT")
					{
						tagEndPos = content.IndexOf("/script>",pos);	
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+8);
						startPos = tagEndPos - count;
					}
					else if(content.Substring(pos+1,4).ToUpper()=="LINK")
					{
						tagEndPos = content.IndexOf(">",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
					}
						//cut <!-- -->
					else if(content.Substring(pos+1,1).ToUpper()=="!")
					{
						tagEndPos = content.IndexOf(">",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
					}
					else if(content.Substring(pos+1,3).ToUpper()=="MAP")
					{
						tagEndPos = content.IndexOf("/map>",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+5);
						startPos = tagEndPos - count;
					}
						//cut <style></style>
					else if(content.Substring(pos+1,5).ToUpper()=="STYLE")
					{
						tagEndPos = content.IndexOf( "/style>",pos);
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+7);
						startPos = tagEndPos - count;
					}		
					else if(content.Substring(pos+1,4).ToUpper()=="META")
					{
						tagEndPos = content.IndexOf(">",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
					}
					else if(content.Substring(pos+1,4).ToUpper()=="FORM")
					{
						if(content.IndexOf("<table")!=-1)
						{
							if(content.IndexOf("<table")<pos)
							{
								tagEndPos = content.IndexOf("/form>",pos);						
								count = tagEndPos - pos ;
								content = content.Remove(pos,count+6);
								startPos = tagEndPos - count;
							}
							else
							{
								startPos = content.IndexOf(">",startPos)+1;
							}
						}
						else
						{
							startPos = content.IndexOf(">",startPos)+1;
						}
					}
					else if(content.Substring(pos+1,4).ToUpper()=="AREA")
					{
						tagEndPos = content.IndexOf(">",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
					}
					else if(content.Substring(pos+1,2).ToUpper()=="UL")
					{
						tagEndPos = content.IndexOf("/ul>",pos);						
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+4);
						startPos = tagEndPos - count;
					}
						//cut <SELECT></SELECT>
					else if(content.Substring(pos+1,6).ToUpper()=="SELECT")
					{
						tagEndPos = content.IndexOf("/select>",pos);
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+8);
						startPos = tagEndPos - count;
					}
					else if(content.Substring(pos+1,6).ToUpper()=="OBJECT")
					{
						tagEndPos = content.IndexOf("/object>",pos);
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+8);
						startPos = tagEndPos - count;
					}
						//cut <INPUT>
					else if(content.Substring(pos+1,5).ToUpper()=="INPUT")
					{
						tagEndPos = content.IndexOf(">",pos);
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
					}
					else
					{
						tagEndPos = content.IndexOf(">",pos);
						count = tagEndPos - pos ;
						content = content.Remove(pos,count+1);
						startPos = tagEndPos - count;
						//startPos = content.IndexOf(">",startPos)+1;
					}
					#endregion	
					if(startPos==content.Length)
						break;
					}					
			}
			catch(Exception e)
			{
				content = pageContent;
			}		
			
			return content;
		}
	}
}
