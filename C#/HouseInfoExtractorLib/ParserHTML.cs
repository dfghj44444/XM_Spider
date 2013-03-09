using System;
using System.Collections;
using System.Text;

namespace HouseInfoExtractorLib
{
	/// <summary>
	/// 
	/// </summary>
	public class Tag
	{
		//标记名称
		private string m_TagName="";
		//标记后的文本内容
		private string m_FollowedText="";
		//标记内的属性列表
		private ArrayList m_TagAttributes=new ArrayList();
		/// <summary>
		/// 标记名称属性
		/// </summary>
		public string TagName
		{
			set
			{
				m_TagName=value;
			}
			get
			{
				return m_TagName;
			}
		}
		/// <summary>
		/// 当前标记后的文本内容
		/// </summary>
		public string FollowedText
		{
			set
			{
				m_FollowedText=value;
			}
			get
			{
				return m_FollowedText;
			}
		}
		/// <summary>
		/// 标记内的属性列表,类型为Attribue类的列表
		/// </summary>
		public ArrayList TagAttributes
		{
			set
			{
				m_TagAttributes=value;
			}
			get
			{
				return m_TagAttributes;
			}
		}
	}
	/// <summary>
	/// Summary description for ParseHTML.
	/// </summary>
	public class ParseHTML:Parse 
	{
		//获取当前tag的名称
		protected string GetTagName()
		{
			return m_tag;
		}
		public AttributeList GetTag()
		{
			AttributeList tag = new AttributeList();
			tag.Name = m_tag;
			
			foreach(Attribute x in List)
			{
				tag.Add((Attribute)x.Clone());
			}
			
			return tag;
		}

		protected void ParseTag()
		{
			//初始化m_tag,并且将AttributeList(ArrayList)清空
			m_tag="";
			Clear();

			// 如果后三个字符是注释标记<!-- -->
			if ((GetCurrentChar()=='!') &&
				(GetCurrentChar(1)=='-')&&
				(GetCurrentChar(2)=='-')) 
			{
				while ( !Eof() ) 
				{
					//如果后三个字符是注释结束标记,则结束识别
					if ((GetCurrentChar()=='-') &&
						(GetCurrentChar(1)=='-')&&
						(GetCurrentChar(2)=='>'))
						break;
					//否则,一直向前,直到遇到>
					Advance();
				}
				//后移三个字符-->
				Advance(3);
				//分隔符置空
				ParseDelim = (char)0;
				return;
			}
			//如果遇到Style标记
			if( (char.ToUpper(GetCurrentChar()) == 'S') &&
				(char.ToUpper(GetCurrentChar(1)) == 'T') &&
				(char.ToUpper(GetCurrentChar(2)) == 'Y') &&
				(char.ToUpper(GetCurrentChar(3)) == 'L') &&
				(char.ToUpper(GetCurrentChar(4)) == 'E') )
			{
				while( !Eof())
				{
					//向前移动字符,直到遇到</STYLE>标记
					if( char.ToUpper(GetCurrentChar()) == '/'  && 
						char.ToUpper(GetCurrentChar(1)) == 'S' &&
						char.ToUpper(GetCurrentChar(2)) == 'T' &&
						char.ToUpper(GetCurrentChar(3)) == 'Y' &&
						char.ToUpper(GetCurrentChar(4)) == 'L')
						break;
					Advance();
				}
				//后移7个字符"/STYLE>"
				Advance(7);
				ParseDelim = (char)0;
				return;
			}
			//如果遇到Script标记,<SCRIPT>和</SCRIPT>之间的内容丢弃
			if( (char.ToUpper(GetCurrentChar()) == 'S') &&
				(char.ToUpper(GetCurrentChar(1)) == 'C') &&
				(char.ToUpper(GetCurrentChar(2)) == 'R') &&
				(char.ToUpper(GetCurrentChar(3)) == 'I') &&
				(char.ToUpper(GetCurrentChar(4)) == 'P') &&
				(char.ToUpper(GetCurrentChar(5)) == 'T') )
			{
				while( !Eof())
				{
					//向前移动字符,直到遇到</SCRIPT>标记
					if( char.ToUpper(GetCurrentChar()) == '/'  && 
						char.ToUpper(GetCurrentChar(1)) == 'S' &&
						char.ToUpper(GetCurrentChar(2)) == 'C' &&
						char.ToUpper(GetCurrentChar(3)) == 'R' &&
						char.ToUpper(GetCurrentChar(4)) == 'I')
						break;
					Advance();
				}
				//后移8个字符/SCRIPT>
				Advance(8);
				ParseDelim = (char)0;
				return;
			}
			
			//对普通标记,一直读到>或\r\t\n及空格标记
			while ( !Eof() ) 
			{
				//如果遇到\t\n\r空格或>,标记识别完成,退出循环
				if ( IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar()=='>') )
					break;
				
				//否则,顺次读取标记字符,并向移动字符
				m_tag+=char.ToUpper(GetCurrentChar());
				Advance();
			}
			//跳过后面的所有\r\n\t及空格
			EatWhiteSpace();
			
			// 读取Tag后的属性
			while ( GetCurrentChar() != '>' && !Eof()) 
			{
				ParseName = "";
				ParseValue = "";
				ParseDelim = (char)0;
				//分析属性名
				ParseAttributeName();
				//如果当前字符是">",则将属性添加到列表中,并结束当前标记解析
				if ( GetCurrentChar()=='>' ) 
				{
					AddAttribute();
					break;
				}
				//尚未遇到>前,解析属性值
				ParseAttributeValue();
				AddAttribute();
			}
			Advance();
		}

		/// <summary>
		/// HTML字符解析
		/// </summary>
		/// <returns></returns>
		protected char Parse()
		{
			if( GetCurrentChar()=='<' ) 
			{
				//如果获取的字符是"<",发现了Tag,前进一个字符,并开始Tag分析
				Advance();
				char ch=char.ToUpper(GetCurrentChar());
				//如果获取的字符是A-Z或者!或者/,则解析Tag标记
				if ( (ch>='A') && (ch<='Z') || (ch=='!') || (ch=='/') ) 
				//if(ch=='T')
				{
					ParseTag();
					return (char)0;
				} 
					//若当前字符不是A-Z或/则直接返回当前字符
				else 
					return(AdvanceCurrentChar());
			} 
				//对于一般字符,直接将其返回
			else 
				return(AdvanceCurrentChar());
		}


		/// <summary>
		/// HTML文档解析,结果是一个Tag列表集合
		/// </summary>
		/// <returns>Tag列表集合</returns>
		public ArrayList Parsing()
		{
			ArrayList tags = new ArrayList();
			StringBuilder tagText = new StringBuilder();
			Tag newTag = new Tag();
			while(!Eof())
			{
				//顺次解析每个字符
				char ch = Parse();
				//如果ch返回0,表示发现一个新的Tag标记
				if(ch==0)
				{
					//如果有文本内容,或者虽无文本内容,但有实际标记
					if(tagText.Length>0 || newTag.TagName != "")
					{
						newTag.FollowedText = tagText.ToString();
						tags.Add(newTag);
						tagText = new StringBuilder();
					}
					
					//则新产生一个Tag对象,将当前标记赋予新标记
					newTag = new Tag();
					string tagName = GetTagName();
					newTag.TagName = tagName;
					newTag.TagAttributes = (ArrayList)List.Clone();
				}
				else
				{
					//如果遇到空格标记(&nbsp;)前行5个字符并返回
					//ch=&,当前字符是n,后续字符是bsp;
					if( ch == '&' &&
						GetCurrentChar() =='n' &&
						GetCurrentChar(1) =='b' &&
						GetCurrentChar(2) =='s' &&
						GetCurrentChar(3) =='p' &&
						GetCurrentChar(4) ==';' )
					{
						Advance(5);
						continue;
					}
					if("\r\n\t ".IndexOf(ch) == -1)
						tagText.Append(ch);
				}
			}
			//如果最后遇到结束标记
			//*----------------暂时去除
			if(GetTagName() != null && GetTagName().StartsWith("/"))
			{
				//添加结束标记
				tags.Add(newTag);
				//如果结标记后还有文本,产生新标记,并将剩余文本写入空标记后
				if(tagText.Length>0)
				{
					newTag = new Tag();
					newTag.TagName = string.Empty;
					newTag.FollowedText = tagText.ToString();
					tags.Add(newTag);
				}
			}
			//如果没遇到任何标记,表明是纯文本
			//产生一空标记,并将文本写入空标记后
			if(tags.Count == 0  && tagText.Length>0)
			{
				newTag=new Tag();
				newTag.TagName="";
				newTag.FollowedText=tagText.ToString();
				tags.Add(newTag);
			}
			return tags;
		}
	}
}
