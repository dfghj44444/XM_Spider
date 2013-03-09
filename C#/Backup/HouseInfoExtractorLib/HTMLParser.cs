using System;
using System.Collections;
using System.Text;

namespace HouseInfoExtractorLib
{
	/// <summary>
	/// 锚结构体
	/// </summary>
	public struct Anchor
	{
		public string AnchorUri;
		public string AnchorText;
	}
	/// <summary>
	/// HTML文本结构体
	/// </summary>
	public struct HtmlText
	{
		public string TagName;
		public string Text;
	}


	/// <summary>
	/// pretreament 的摘要说明。
	/// </summary>
	public class HtmlParser
	{
		//HTML解析器
		private ParseHTML m_parser;
		//tag列表
		private ArrayList m_Tags;
		/// <summary>
		/// 构造函数,参数为HTML源文本
		/// </summary>
		/// <param name="source">被解析的文本</param>
		public HtmlParser(string source)
		{
			//生成HTML解析器实例
			m_parser = new ParseHTML();
			m_parser.Source = source;
			//解析HTML
			m_Tags = m_parser.Parsing();
		}		

		/// <summary>
		/// 获取Html内的所有标记,返回一个Tag型的列表
		/// </summary>
		/// <returns></returns>
		public ArrayList GetTags()
		{
			return m_Tags;
		}
		/// <summary>
		/// 获取网页中的URL及锚文本,结果是Anchor型列表集合
		/// </summary>
		/// <returns>Anchor列表</returns>
		public ArrayList GetURLs()
		{
			ArrayList urls = new ArrayList();
			//
			Anchor anchor = new Anchor();
			bool isAnchor =false;//发现链接标记
			
			//遍历各标记
			foreach( Tag tag in m_Tags )
			{
				if(tag.TagName == "A")
				{
					//遍历属性集合,查找HREF
					foreach( Attribute a in tag.TagAttributes )
					{
						if( a.Name == "HREF" )
							anchor.AnchorUri = a.Value;
					}
					isAnchor = true;
				}
				//发现/A标记,且HREF不是#,则将链接添加到URL列表中
				if(tag.TagName == "/A" && anchor.AnchorUri != "#")
				{
					urls.Add( anchor );
					isAnchor = false;
					anchor = new Anchor();
				}
				if( isAnchor )
				{
					anchor.AnchorText += tag.FollowedText;
				}
			}
			return urls;
		}
		/// <summary>
		/// 获取网页Keywords,返回的关键词字符串用空格隔开
		/// </summary>
		/// <returns></returns>
		public string GetKeywords()
		{
			string keywords="";
			foreach(Tag tag in m_Tags)
			{
				if(tag.TagName=="META")
				{
					foreach(Attribute a in tag.TagAttributes)
					{
						if(a.Name=="KEYWORD")
							keywords+=a.Value+" ";
					}
				}
				if(tag.TagName=="BODY")
					break;
			}
			return keywords;
		}
		/// <summary>
		/// 获取网页meta的Desction
		/// </summary>
		/// <returns></returns>
		public string GetDescription()
		{
			string description="";
			foreach(Tag tag in m_Tags)
			{
				if(tag.TagName=="META")
				{
					foreach(Attribute a in tag.TagAttributes)
					{
						if(a.Name=="DESCRIPTION")
							description+=a.Value+" ";
					}
				}
				if(tag.TagName=="BODY")
					break;
			}
			return description;
		}
		/// <summary>
		/// 获取HTML页面的所有文本内容
		/// </summary>
		/// <returns></returns>
		public string GetPageText()
		{
			StringBuilder sb=new  StringBuilder();
			bool isDiscarded=false;
			//遍历所有标记,不要SCRIPT STYLE内容 
			foreach(Tag tag in m_Tags)
			{
				
				//如果遇到SCRIPT标记,标志置真并返回,直到遇到/Script标记
				if(tag.TagName=="SCRIPT")
				{
					isDiscarded=true;
					continue;
				}
				if(tag.TagName=="/SCRIPT")
				{
					isDiscarded=false;
				}
				//如果遇到STYLE标记,将标记置真并返回,直到遇到/Style标记
				if(tag.TagName=="STYLE")
				{
					isDiscarded=true;
					continue;
				}
				if(tag.TagName=="/STYLE")
				{
					isDiscarded=false;
				}
				if( !isDiscarded && tag.FollowedText !="" )
				{
					if(tag.FollowedText.StartsWith("&nbsp;"))
						tag.FollowedText.Replace("&nbsp;","");
					sb.Append(tag.FollowedText);
				}
			}
			return sb.ToString();
		}
		public string GetTitle()
		{
			string title="";
			//遍历Tag标记,如果找到TITLE,则返回标记的文本
			foreach(Tag tag in m_Tags)
			{
				if(tag.TagName == "TITLE")
				{
					title = tag.FollowedText;
					break;
				}
				//如果找到BODY后仍没找到TITLE标记,则直接返回空字符串
				if(tag.TagName=="BODY")
					break;
			}
			return title;
		}
		/// <summary>
		/// 获取网页中的有效(有实质内容)标记,返回Tag类型的ArrayList.
		/// </summary>
		/// <returns></returns>
		public ArrayList GetValidTag()
		{
			string test="";
			ArrayList al = new ArrayList();
			foreach(Tag tag in m_Tags)
			{
				//if( tag.FollowedText != "" &&tag.TagName != "SCRIPT" &&tag.TagName != "STYLE")	
				if(tag.FollowedText != "" && tag.TagName != "FORM" &&tag.TagName != "SCRIPT" &&tag.TagName != "STYLE")
				{
					HtmlText text = new HtmlText();
					text.TagName = tag.TagName;
					text.Text = tag.FollowedText;
					test = text.TagName + ":" + text.Text;
					al.Add(text);
				}
			}
			return al;
		}


	}
}
