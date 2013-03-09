using System;

namespace HouseInfoExtractorLib
{
	/// <summary>
	/// Base class for parseing tag based files, such as HTML, HTTP headers
	/// or XML.
	/// </summary>
	public class Parse:AttributeList 
	{
		/// <summary>
		/// The source text that is being parsed.
		/// </summary>
		private string m_source;

		/// <summary>
		/// The current position inside of the text that
		/// is being parsed.
		/// </summary>
		private int m_idx;

		/// <summary>
		/// The most reciently parsed attribute delimiter.
		/// </summary>
		private char m_parseDelim;

		/// <summary>
		/// This most receintly parsed attribute name.
		/// </summary>
		private string m_parseName;

		/// <summary>
		/// The most reciently parsed attribute value.
		/// </summary>
		private string m_parseValue;

		/// <summary>
		/// The most reciently parsed tag.
		/// </summary>
		protected string m_tag;
		/// <summary>
		/// 文档总长度
		/// </summary>
		private int m_docLength=0;
		/// <summary>
		/// 网页的根地址
		/// </summary>
		private string m_baseURL="";
		/// <summary>
		/// 网页根地址属性
		/// </summary>
		/// <returns></returns>
		public string BaseURL
		{
			set
			{
				m_baseURL=value;
			}
			get
			{
				return m_baseURL;
			}
		}
		/// <summary>
		/// Determine if the specified character is whitespace or not.
		/// </summary>
		/// <param name="ch">A character to check</param>
		/// <returns>true if the character is whitespace</returns>
		protected static bool IsWhiteSpace(char ch)
		{
			return( "\t\n\r ".IndexOf(ch) != -1 );
		}


		/// <summary>
		/// Advance the index until past any whitespace.
		/// </summary>
		protected void EatWhiteSpace()
		{
			while ( !Eof() ) 
			{
				if ( !IsWhiteSpace(GetCurrentChar()) )
					return;
				m_idx++;
			}
		}

		/// <summary>
		/// Determine if the end of the source text has been
		/// reached. 
		/// </summary>
		/// <returns>True if the end of the source text has been
		/// reached.</returns>
		protected bool Eof()
		{
			return(m_idx>=m_docLength );
		}

		/// <summary>
		/// 解析属性名称
		/// </summary>
		protected void ParseAttributeName()
		{
			//去掉所有\r\n\t及空格
			EatWhiteSpace();
			//通过循环获取属性名称
			while ( !Eof() ) 
			{
				//判断属性:空格 = 或 遇到>
				if ( IsWhiteSpace(GetCurrentChar()) ||
					(GetCurrentChar()=='=') ||
					(GetCurrentChar()=='>') )
					break;
				m_parseName+=char.ToUpper(GetCurrentChar());
				m_idx++;
			}
			EatWhiteSpace();
		}


		/// <summary>
		/// Parse the attribute value
		/// </summary>
		protected void ParseAttributeValue()
		{
			if ( m_parseDelim != 0 )
				return;
			if ( GetCurrentChar()=='=' ) 
			{
				m_idx++;
				//Advance();
				EatWhiteSpace();
				// 如果有分隔符,是'或"
				if ( (GetCurrentChar()=='\'') ||
					(GetCurrentChar()=='\"') ) 
				{
					//保留当前的分隔符,并后移一字符
					m_parseDelim = GetCurrentChar();
					m_idx++;
					//若当前字符不是保存的分隔符或空格,则为属性值
					while ( GetCurrentChar() != m_parseDelim && !IsWhiteSpace(GetCurrentChar()) && !Eof()) 
					{
						m_parseValue+=GetCurrentChar();
						m_idx++;
					}
					//后移分隔符字符位()
					m_idx++;
				} 
					//否则直接读取属性值
				else 
				{
					while ( !Eof() &&
						!IsWhiteSpace(GetCurrentChar()) &&
						(GetCurrentChar()!='>') ) 
					{
						m_parseValue += GetCurrentChar();
						m_idx++;
					}
				}
				EatWhiteSpace();
			}
		}

		/// <summary>
		/// Add a parsed attribute to the collection.
		/// </summary>
		protected void AddAttribute()
		{
			Attribute a = new Attribute(m_parseName,m_parseValue,m_parseDelim);
			Add(a);
		}

		/// <summary>
		/// Get the current character that is being parsed.
		/// </summary>
		/// <returns></returns>
		protected char GetCurrentChar()
		{
			return GetCurrentChar(0);
		}

		/// <summary>
		/// Get a few characters ahead of the current character.
		/// </summary>
		/// <param name="peek">How many characters to peek ahead for.</param>
		/// <returns>The character that was retrieved.</returns>
		protected char GetCurrentChar(int peek)
		{
			if( (m_idx+peek)<m_docLength)
				return m_source[m_idx+peek];
			else
				return (char)0;
		}

		/// <summary>
		/// 获取下一个字符,并后移一字符.
		/// </summary>
		/// <returns>The next character</returns>
		protected char AdvanceCurrentChar()
		{
			return m_source[m_idx++];
		}

		/// <summary>
		/// 前进一字符.
		/// </summary>
		protected void Advance()
		{
			m_idx++;
		}
		
		/// <summary>
		///前进指定个字符 
		/// </summary>
		/// <param name="i"></param>
		protected void Advance(int i)
		{
			m_idx+=i;
		}

		/// <summary>
		/// 最后遇到的属性名称.
		/// </summary>
		protected string ParseName
		{
			get 
			{
				return m_parseName;
			}

			set 
			{
				m_parseName = value;
			}
		}

		/// <summary>
		/// 最后遇到的属性值.
		/// </summary>
		protected string ParseValue
		{
			get 
			{
				return m_parseValue;
			}

			set 
			{
				m_parseValue = value;
			}
		}

		/// <summary>
		/// The last attribute delimeter that was encountered.
		/// </summary>
		protected char ParseDelim
		{
			get 
			{
				return m_parseDelim;
			}

			set 
			{
				m_parseDelim = value;
			}
		}

		/// <summary>
		/// The text that is to be parsed.
		/// </summary>
		public string Source
		{
			get 
			{
				return m_source;
			}

			set 
			{
				m_source = value + " "; //防止最后一个字符是"<"而出现读字符错误
				m_docLength=value.Length;
			}
		}
	}
}
