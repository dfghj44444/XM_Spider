using System;
using System.Collections;
using System.Text;

namespace HouseInfoExtractorLib
{
	/// <summary>
	/// ê�ṹ��
	/// </summary>
	public struct Anchor
	{
		public string AnchorUri;
		public string AnchorText;
	}
	/// <summary>
	/// HTML�ı��ṹ��
	/// </summary>
	public struct HtmlText
	{
		public string TagName;
		public string Text;
	}


	/// <summary>
	/// pretreament ��ժҪ˵����
	/// </summary>
	public class HtmlParser
	{
		//HTML������
		private ParseHTML m_parser;
		//tag�б�
		private ArrayList m_Tags;
		/// <summary>
		/// ���캯��,����ΪHTMLԴ�ı�
		/// </summary>
		/// <param name="source">���������ı�</param>
		public HtmlParser(string source)
		{
			//����HTML������ʵ��
			m_parser = new ParseHTML();
			m_parser.Source = source;
			//����HTML
			m_Tags = m_parser.Parsing();
		}		

		/// <summary>
		/// ��ȡHtml�ڵ����б��,����һ��Tag�͵��б�
		/// </summary>
		/// <returns></returns>
		public ArrayList GetTags()
		{
			return m_Tags;
		}
		/// <summary>
		/// ��ȡ��ҳ�е�URL��ê�ı�,�����Anchor���б���
		/// </summary>
		/// <returns>Anchor�б�</returns>
		public ArrayList GetURLs()
		{
			ArrayList urls = new ArrayList();
			//
			Anchor anchor = new Anchor();
			bool isAnchor =false;//�������ӱ��
			
			//���������
			foreach( Tag tag in m_Tags )
			{
				if(tag.TagName == "A")
				{
					//�������Լ���,����HREF
					foreach( Attribute a in tag.TagAttributes )
					{
						if( a.Name == "HREF" )
							anchor.AnchorUri = a.Value;
					}
					isAnchor = true;
				}
				//����/A���,��HREF����#,��������ӵ�URL�б���
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
		/// ��ȡ��ҳKeywords,���صĹؼ����ַ����ÿո����
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
		/// ��ȡ��ҳmeta��Desction
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
		/// ��ȡHTMLҳ��������ı�����
		/// </summary>
		/// <returns></returns>
		public string GetPageText()
		{
			StringBuilder sb=new  StringBuilder();
			bool isDiscarded=false;
			//�������б��,��ҪSCRIPT STYLE���� 
			foreach(Tag tag in m_Tags)
			{
				
				//�������SCRIPT���,��־���沢����,ֱ������/Script���
				if(tag.TagName=="SCRIPT")
				{
					isDiscarded=true;
					continue;
				}
				if(tag.TagName=="/SCRIPT")
				{
					isDiscarded=false;
				}
				//�������STYLE���,��������沢����,ֱ������/Style���
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
			//����Tag���,����ҵ�TITLE,�򷵻ر�ǵ��ı�
			foreach(Tag tag in m_Tags)
			{
				if(tag.TagName == "TITLE")
				{
					title = tag.FollowedText;
					break;
				}
				//����ҵ�BODY����û�ҵ�TITLE���,��ֱ�ӷ��ؿ��ַ���
				if(tag.TagName=="BODY")
					break;
			}
			return title;
		}
		/// <summary>
		/// ��ȡ��ҳ�е���Ч(��ʵ������)���,����Tag���͵�ArrayList.
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
