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
		/// �ĵ��ܳ���
		/// </summary>
		private int m_docLength=0;
		/// <summary>
		/// ��ҳ�ĸ���ַ
		/// </summary>
		private string m_baseURL="";
		/// <summary>
		/// ��ҳ����ַ����
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
		/// ������������
		/// </summary>
		protected void ParseAttributeName()
		{
			//ȥ������\r\n\t���ո�
			EatWhiteSpace();
			//ͨ��ѭ����ȡ��������
			while ( !Eof() ) 
			{
				//�ж�����:�ո� = �� ����>
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
				// ����зָ���,��'��"
				if ( (GetCurrentChar()=='\'') ||
					(GetCurrentChar()=='\"') ) 
				{
					//������ǰ�ķָ���,������һ�ַ�
					m_parseDelim = GetCurrentChar();
					m_idx++;
					//����ǰ�ַ����Ǳ���ķָ�����ո�,��Ϊ����ֵ
					while ( GetCurrentChar() != m_parseDelim && !IsWhiteSpace(GetCurrentChar()) && !Eof()) 
					{
						m_parseValue+=GetCurrentChar();
						m_idx++;
					}
					//���Ʒָ����ַ�λ()
					m_idx++;
				} 
					//����ֱ�Ӷ�ȡ����ֵ
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
		/// ��ȡ��һ���ַ�,������һ�ַ�.
		/// </summary>
		/// <returns>The next character</returns>
		protected char AdvanceCurrentChar()
		{
			return m_source[m_idx++];
		}

		/// <summary>
		/// ǰ��һ�ַ�.
		/// </summary>
		protected void Advance()
		{
			m_idx++;
		}
		
		/// <summary>
		///ǰ��ָ�����ַ� 
		/// </summary>
		/// <param name="i"></param>
		protected void Advance(int i)
		{
			m_idx+=i;
		}

		/// <summary>
		/// �����������������.
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
		/// �������������ֵ.
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
				m_source = value + " "; //��ֹ���һ���ַ���"<"�����ֶ��ַ�����
				m_docLength=value.Length;
			}
		}
	}
}
