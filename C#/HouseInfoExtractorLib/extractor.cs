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
	//房屋信息的结构体
	public struct HouseInfo
	{
		public string source;				//信息来源
		public int    cityID;				//城市编号
		public int    urltype;				//网站编号
		public string address;				//地址
		public string region;				//区域
		public string fitment;				//装潢情况
		public string houseType ;			//房屋类型
		public string property;				//产权
		public string doneDate;				//建造年代
		public int    room;					//几室
		public int    hall;					//几厅
		public int    washRoom;				//几卫
		public string managerName;  		//物业名称
		public string managerType;  		//物业类别
		public string floor;        		//楼层
		public string area;         		//面积
		public double price;        		//价格
		public string remark;       		//备注
		public string linkMan;      		//联系人
		public string linkPhone;    		//联系电话
		public DateTime registerDate;			//发布时间
		public string email;        		//email  -----暂时不考虑
		public string rentType;     		//租赁方式
		public string direction;    		//朝向			
		public string traffic;				//交通状况
		public string schoolInfo;   		//学区信息
		public string houseStructure;		//房屋结构
		public string baseEstablishment;	//基础设施
		public string equipment;			//设备
		public string environment;          //环境
		public string url;                  //用于存储下载网页的URL
	}

	/// <summary>
	/// Class1 的摘要说明。
	/// </summary>
	public class HouseInfoExtractor
	{
		public HouseInfoExtractor()
		{	
			//
			// TODO: 在此处添加构造函数逻辑
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
		/// 提取指定网页内容的信息
		/// </summary>
		/// <param name="pageContent">指定的网页内容</param>
		/// <returns>房屋信息的结构体</returns>
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
	            
			content = content.Replace(" ","　");
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
				key = tag.Text.Replace("。",".");
				key = key.Replace("、",",");
				key  = key.Replace("，",",");
				key  = key.Replace("；",";");
				key  = key.Replace("！","!");
				key  = key.Replace("？","?");
				key = Strings.StrConv(key,VbStrConv.Narrow,0); // 全角转半角
				key = key.Trim(new char[]{':','：'});
				if(key.Trim()=="")
					continue;
				string tempValue = null;
				if(key.StartsWith("·")){
					key = key.Remove(0,1);
				}
				if(key.EndsWith("·"))
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
				else if (tempKey.IndexOf("：") != -1&& tempKey.IndexOf("：") < tempKey.Length-1) 
				{
					tempValue = tempKey.Substring(tempKey.IndexOf("：") + 1);
					tempKey = tempKey.Substring(0, tempKey.IndexOf("："));
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
				#region -----判断添加开关----- 
				if(add==true)
				{
					for(int i = 0;i<keyWordsLists.Count;i++)
					{
						if(attribute.key == "产权"&&tempKey.Replace(" ","")=="产权")
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
				
				#region		---关键字匹配---
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
			
			if(houseInfo.region.IndexOf("·")!=-1)
				if(houseInfo.region.IndexOf("·")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("·")+1);
			if(houseInfo.region.IndexOf("·")!=-1)
				houseInfo.region = houseInfo.region.Substring(0,houseInfo.region.IndexOf("·"));
			if(houseInfo.region.IndexOf("/")!=-1)
				if(houseInfo.region.IndexOf("/")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("/")+1);
			if(houseInfo.region.IndexOf("\\")!=-1)
				if(houseInfo.region.IndexOf("\\")<houseInfo.region.Length)
					houseInfo.region = houseInfo.region.Substring(houseInfo.region.IndexOf("\\")+1);
			return houseInfo;
		}


		/// <summary>
		/// 将提取到的信息插入房屋信息结构体中
		/// </summary>
		/// <param name="attribute">提取到的信息</param>
		public void insert(Attribute attribute)
		{
			string floorType = null;
			attribute.keyvalue = attribute.keyvalue.Trim();
			if(attribute.key == "来源")
			{
				houseInfo.source = attribute.keyvalue;
				if(attribute.keyvalue == "是"||attribute.keyvalue=="中介"||attribute.keyvalue=="个人委托中介"||attribute.keyvalue=="委托中介")
					houseInfo.source = "中介";
				else if(attribute.keyvalue == "否"||attribute.keyvalue == "不是"||attribute.keyvalue.IndexOf("非中介")!=-1||attribute.keyvalue.IndexOf("不是中介")!=-1||attribute.keyvalue.IndexOf("个人")!=-1)
					houseInfo.source = "个人";
			}
			else if(attribute.key == "地址")
			{
				string region="";
				if(attribute.keyvalue.IndexOf("区")>=2)
				{
					region = attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("区")-2,3);
					if(houseInfo.region==""&attribute.keyvalue.Length>3)
						houseInfo.region = region;
				}
				if(houseInfo.address=="")
					houseInfo.address =  attribute.keyvalue;			
			}
			else if(attribute.key == "区域")
			{
				if(attribute.keyvalue.IndexOf("不限")==-1&attribute.keyvalue.IndexOf("任意")==-1){
					houseInfo.region = attribute.keyvalue;
					//hasRegion =true;
				}
			}
			else if(attribute.key == "城市与区域"){
				if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf(" ")!=-1&attribute.keyvalue.IndexOf(" ")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf(" ")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("市")!=-1&attribute.keyvalue.IndexOf("市")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("市")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("/")!=-1&attribute.keyvalue.IndexOf("/")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("/")+1);
				else if(attribute.keyvalue!=""&attribute.keyvalue.IndexOf("\\")!=-1&attribute.keyvalue.IndexOf("\\")<attribute.keyvalue.Length&houseInfo.region!="")
					houseInfo.region =  attribute.keyvalue.Substring(attribute.keyvalue.IndexOf("\\")+1);
				else if(houseInfo.region=="")
					houseInfo.region =  attribute.keyvalue;
			}
			else if(attribute.key == "装修")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.fitment = attribute.keyvalue;
			}
			//待改进 室、厅、卫
			else if(attribute.key == "房型")
			{	
				string type = attribute.keyvalue;
				if(type.IndexOf("居")!=-1|type.IndexOf("室")!=-1|type.IndexOf("厅")!=-1|type.IndexOf("卫")!=-1)
				{
					this.processHouseType(type);
				}
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.houseType =  attribute.keyvalue;
			}
			else if(attribute.key == "房屋结构")
			{	
				string type = attribute.keyvalue;
				if(type.IndexOf("居")!=-1|type.IndexOf("室")!=-1||type.IndexOf("厅")!=-1||type.IndexOf("卫")!=-1)
				{
					this.processHouseType(type);
				}
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.houseStructure =  attribute.keyvalue;
			}
			else if(attribute.key == "产权")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.property =  attribute.keyvalue;
			}
			else if(attribute.key == "建成日期")
			{
				if(attribute.keyvalue.IndexOf("年")!=-1)
					attribute.keyvalue = attribute.keyvalue.Replace("年","");
				else if(attribute.keyvalue == "未知")
					attribute.keyvalue = "";
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.doneDate = attribute.keyvalue;
			}
			else if(attribute.key == "物业名称")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.managerName =  attribute.keyvalue;
			}
			else if(attribute.key == "物业类别")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
					houseInfo.managerType =  attribute.keyvalue;
			}
			else if(attribute.key == "楼层")
			{			
				floorType = "at";
				houseInfo.floor = this.floorProcess(attribute.keyvalue,floorType);		
			}
			else if(attribute.key == "总楼层")
			{
				floorType = "all";
				houseInfo.floor = this.floorProcess(attribute.keyvalue,floorType);	
			}
			else if(attribute.key == "面积")
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
			else if(attribute.key == "租赁方式")
			{
				if(attribute.keyvalue != "" |attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("合租")!=-1|attribute.keyvalue.IndexOf("非整租")!=-1|attribute.keyvalue.IndexOf("不整租")!=-1|attribute.keyvalue.IndexOf("不是整租")!=-1)
						houseInfo.rentType = "合租";
					if(attribute.keyvalue.IndexOf("整租")!=-1|attribute.keyvalue.IndexOf("非合租")!=-1|attribute.keyvalue.IndexOf("不合租")!=-1|attribute.keyvalue.IndexOf("不是合租")!=-1)
						houseInfo.rentType = "整租";
					#region-------------------判断日租\长租和短租--------------------------
//					else if(attribute.keyvalue.IndexOf("日租")!=-1)
//						houseInfo.rentType = "日租";
//					else if(attribute.keyvalue.IndexOf("长租")!=-1)
//						houseInfo.rentType = "长租";
//					else if(attribute.keyvalue.IndexOf("短租")!=-1)
//						houseInfo.rentType = "短租";
				}
			}
//			else if(attribute.key == "短租")
//			{
//				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
//				{
//					if(attribute.keyvalue.IndexOf("是")!=-1||attribute.keyvalue.IndexOf("可")!=-1)
//						houseInfo.rentType =  "短租";
//					else if(attribute.keyvalue.IndexOf("否")!=-1)
//						houseInfo.rentType =  "长租";
//					else
//						houseInfo.rentType = attribute.keyvalue;
//				}
//			}
//			else if(attribute.key == "长租")
//			{
//				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
//				{
//					if(attribute.keyvalue.IndexOf("是")!=-1||attribute.keyvalue.IndexOf("可")!=-1)
//						houseInfo.rentType =  "长租";
//					else if(attribute.keyvalue.IndexOf("否")!=-1)
//						houseInfo.rentType =  "短租";
//					else
//						houseInfo.rentType = attribute.keyvalue;
//				}
//			}
				#endregion
			else if(attribute.key == "合租")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("是")!=-1|attribute.keyvalue.IndexOf("可")!=-1)
						houseInfo.rentType =  "合租";
					else if(attribute.keyvalue.IndexOf("否")!=-1|attribute.keyvalue.IndexOf("独")!=-1|attribute.keyvalue.IndexOf("不")!=-1)
						houseInfo.rentType =  "整租";
					else
						houseInfo.rentType = attribute.keyvalue;
				}
			}
			else if(attribute.key == "整租")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != null)
				{
					if(attribute.keyvalue.IndexOf("是")!=-1||attribute.keyvalue.IndexOf("可")!=-1|attribute.keyvalue.IndexOf("独")!=-1)
						houseInfo.rentType =  "整租";
					else if(attribute.keyvalue.IndexOf("否")!=-1)
						houseInfo.rentType =  "合租";
					else
						houseInfo.rentType = attribute.keyvalue;
				}
			}
			else if(attribute.key == "价格")
			{
				if(attribute.keyvalue != "" &&attribute.keyvalue.IndexOf("不限")==-1&&attribute.keyvalue.IndexOf("任意")==-1)
					houseInfo.price =  this.priceProcess(attribute.keyvalue);	
			}
			else if(attribute.key == "备注")
			{
				Regex r = new Regex("\\d{3}-\\d{8}|\\d{4}-\\d{7}|\\d{11}|\\d{7}|\\d{8}"); // 定义一个Regex对象实例
				Match m = r.Match(attribute.keyvalue); // 在字符串中匹配
				if (m.Success) 
				{
					if(houseInfo.linkPhone=="")
						houseInfo.linkPhone = m.Value;
					//输入匹配字符的位置
					attribute.keyvalue = attribute.keyvalue.Replace(m.Value,"");
				}
				
				if(houseInfo.remark !=""&&attribute.keyvalue!="")
				{
					houseInfo.remark += ";"  + attribute.keyvalue;	
				}
				else if(houseInfo.remark == ""&&attribute.keyvalue!="")
					houseInfo.remark =  attribute.keyvalue.Trim();
			}
			else if(attribute.key == "联系人")
			{
				if(houseInfo.linkMan != "")
					houseInfo.linkMan += ";"+attribute.keyvalue;
				else
					houseInfo.linkMan = attribute.keyvalue;
			}
			else if(attribute.key == "联系电话")
			{
				char[] tel = attribute.keyvalue.ToCharArray();
				attribute.keyvalue = "";
				char[] sign = new char[]{'/','\\',',',';','；','，'};
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
			else if(attribute.key == "学区")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.schoolInfo =  attribute.keyvalue;
			}
			
			else if(attribute.key == "登记日期")
			{
				int len=0;
				if(attribute.keyvalue.IndexOf("日")!=-1&attribute.keyvalue.IndexOf("日期")==-1)
					houseInfo.registerDate = Convert.ToDateTime(attribute.keyvalue.Substring(0,attribute.keyvalue.IndexOf("日")+1));
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
			else if(attribute.key == "朝向")
			{		
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.direction =  attribute.keyvalue;
			}

			else if(attribute.key == "交通")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.traffic =  attribute.keyvalue;
			}
			else if(attribute.key == "环境")
			{
				if(attribute.keyvalue != "" ||attribute.keyvalue != "")
					houseInfo.environment =  attribute.keyvalue;
			}
		
			else if(attribute.key == "基础设施")
			{
				if(houseInfo.baseEstablishment !=""&&attribute.keyvalue!="")
				{
					houseInfo.baseEstablishment += ";"  + attribute.keyvalue;	
				}
				else if(houseInfo.baseEstablishment==""&&attribute.keyvalue!="")
					houseInfo.baseEstablishment =  attribute.keyvalue;
			}
			else if(attribute.key == "设备")
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
		/// 处理楼层信息
		/// </summary>
		/// <param name="s">原楼层信息</param>
		/// <returns>处理后的楼层信息</returns>
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
				if(array[i]=='第')
				{
					allFloor = false;
					atFloor = true;
					continue;
				}
				else if(array[i]=='共'||array[i]=='\\'||array[i]=='/'||array[i]=='-')
				{
					allFloor = true;
					atFloor = false;
					continue;
				}
//				else if(array[i]=='楼'||array[i]=='层')
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
				else if(array[i]=='十'||array[i]=='拾')
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
		/// 提取价格，数值型
		/// </summary>
		/// <param name="s">价格字符串</param>
		/// <returns>数值型价格</returns>
		public double priceProcess(string s)
		{
			#region ----为charNums赋初值---------
			charNums[0].charnum = '一';
			charNums[0].bigCharnum = '壹';
			charNums[0].num = "1";
			charNums[1].charnum = '二';
			charNums[1].bigCharnum = '贰';
			charNums[1].num = "2";			
			charNums[2].charnum = '三';
			charNums[2].bigCharnum = '叁';
			charNums[2].num = "3";
			charNums[3].charnum = '四';
			charNums[3].bigCharnum = '肆';
			charNums[3].num = "4";
			charNums[4].charnum = '五';
			charNums[4].bigCharnum = '伍';
			charNums[4].num = "5";
			charNums[5].charnum = '六';
			charNums[5].bigCharnum = '陆';
			charNums[5].num = "6";
			charNums[6].charnum = '七';
			charNums[6].bigCharnum = '柒';
			charNums[6].num = "7";
			charNums[7].charnum = '八';
			charNums[7].bigCharnum = '捌';
			charNums[7].num = "8";
			charNums[8].charnum = '九';
			charNums[8].bigCharnum = '玖';
			charNums[8].num = "9";
			charNums[9].charnum = '两';
			charNums[9].bigCharnum = '贰';
			charNums[9].num = "2";
			#endregion
			char[] array = s.Replace(" ","").ToCharArray();
			string priceStr = "";
			double price = 0;
			for(int i = 0;i<array.Length;i++)
			{
				if(('0' <= array[i] &&array[i] <= '9')||array[i]=='.'||array[i]=='．'||array[i]==','||array[i]==' '||array[i]=='，')
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
				else if(array[i]=='万'||array[i]=='萬')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString(Convert.ToDouble(priceStr)*10000);
						price = Convert.ToDouble(priceStr);
						priceStr = "";
					}			
				}
				else if(array[i]=='千'||array[i]=='仟')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr)*1000));	
						price += Convert.ToDouble(priceStr);
						priceStr = "";
					}	
				}
				else if(array[i]=='佰'||array[i]=='百')
				{
					if(priceStr!="")
					{
						priceStr = Convert.ToString((Convert.ToDouble(priceStr)*100));	
						price += Convert.ToDouble(priceStr);
						priceStr = "";
					}		
				}
				else if(array[i]=='十'||array[i]=='拾')
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
				else if(array[i]=='元'){
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
		/// 如果房型为几厅几室几卫的
		/// </summary>
		/// <param name="type"></param>
		public void processHouseType(string type){
			if(type.IndexOf("室")!=-1){
				int i = type.IndexOf("室");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("居")!=-1)
			{
				int i = type.IndexOf("居");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("房")!=-1)
			{
				int i = type.IndexOf("房");
				if(i>0)
					houseInfo.room = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.room=0;
			}
			if(type.IndexOf("厅")!=-1){
				int i = type.IndexOf("厅");
				if(i>0)
					houseInfo.hall = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.hall=0;
			}
			
			if(type.IndexOf("卫")!=-1){
				int i = type.IndexOf("卫");
				if(i>0)
					houseInfo.washRoom = numProcesser(type.Substring(i-1,1));
				else
					houseInfo.washRoom=0;
			}
		}

		/// <summary>
		/// 汉字转化为int数字
		/// </summary>
		/// <param name="s">汉字型数字</param>
		/// <returns>int型数字</returns>
		public int numProcesser(string s){
			int i=0;
			if(s =="一" || s == "壹")
			{
				i=1;
			}
			else if(s =="二" || s == "贰")
			{
				i=2;
			}
			else if(s =="三" || s == "叁")
			{
				i=3;
			}
			else if(s =="四" || s == "肆")
			{
				i=4;
			}
			else if(s =="五" || s == "伍")
			{
				i=5;
			}
			else if(s =="零")
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
		/// 判断关键词库中是否包含此关键词
		/// </summary>
		/// <param name="lists">关键词库</param>
		/// <param name="key">关键词</param>
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
		/// 起初网页中不需要的标记和内容
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
