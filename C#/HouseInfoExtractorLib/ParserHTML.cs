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
		//�������
		private string m_TagName="";
		//��Ǻ���ı�����
		private string m_FollowedText="";
		//����ڵ������б�
		private ArrayList m_TagAttributes=new ArrayList();
		/// <summary>
		/// �����������
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
		/// ��ǰ��Ǻ���ı�����
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
		/// ����ڵ������б�,����ΪAttribue����б�
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
		//��ȡ��ǰtag������
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
			//��ʼ��m_tag,���ҽ�AttributeList(ArrayList)���
			m_tag="";
			Clear();

			// ����������ַ���ע�ͱ��<!-- -->
			if ((GetCurrentChar()=='!') &&
				(GetCurrentChar(1)=='-')&&
				(GetCurrentChar(2)=='-')) 
			{
				while ( !Eof() ) 
				{
					//����������ַ���ע�ͽ������,�����ʶ��
					if ((GetCurrentChar()=='-') &&
						(GetCurrentChar(1)=='-')&&
						(GetCurrentChar(2)=='>'))
						break;
					//����,һֱ��ǰ,ֱ������>
					Advance();
				}
				//���������ַ�-->
				Advance(3);
				//�ָ����ÿ�
				ParseDelim = (char)0;
				return;
			}
			//�������Style���
			if( (char.ToUpper(GetCurrentChar()) == 'S') &&
				(char.ToUpper(GetCurrentChar(1)) == 'T') &&
				(char.ToUpper(GetCurrentChar(2)) == 'Y') &&
				(char.ToUpper(GetCurrentChar(3)) == 'L') &&
				(char.ToUpper(GetCurrentChar(4)) == 'E') )
			{
				while( !Eof())
				{
					//��ǰ�ƶ��ַ�,ֱ������</STYLE>���
					if( char.ToUpper(GetCurrentChar()) == '/'  && 
						char.ToUpper(GetCurrentChar(1)) == 'S' &&
						char.ToUpper(GetCurrentChar(2)) == 'T' &&
						char.ToUpper(GetCurrentChar(3)) == 'Y' &&
						char.ToUpper(GetCurrentChar(4)) == 'L')
						break;
					Advance();
				}
				//����7���ַ�"/STYLE>"
				Advance(7);
				ParseDelim = (char)0;
				return;
			}
			//�������Script���,<SCRIPT>��</SCRIPT>֮������ݶ���
			if( (char.ToUpper(GetCurrentChar()) == 'S') &&
				(char.ToUpper(GetCurrentChar(1)) == 'C') &&
				(char.ToUpper(GetCurrentChar(2)) == 'R') &&
				(char.ToUpper(GetCurrentChar(3)) == 'I') &&
				(char.ToUpper(GetCurrentChar(4)) == 'P') &&
				(char.ToUpper(GetCurrentChar(5)) == 'T') )
			{
				while( !Eof())
				{
					//��ǰ�ƶ��ַ�,ֱ������</SCRIPT>���
					if( char.ToUpper(GetCurrentChar()) == '/'  && 
						char.ToUpper(GetCurrentChar(1)) == 'S' &&
						char.ToUpper(GetCurrentChar(2)) == 'C' &&
						char.ToUpper(GetCurrentChar(3)) == 'R' &&
						char.ToUpper(GetCurrentChar(4)) == 'I')
						break;
					Advance();
				}
				//����8���ַ�/SCRIPT>
				Advance(8);
				ParseDelim = (char)0;
				return;
			}
			
			//����ͨ���,һֱ����>��\r\t\n���ո���
			while ( !Eof() ) 
			{
				//�������\t\n\r�ո��>,���ʶ�����,�˳�ѭ��
				if ( IsWhiteSpace(GetCurrentChar()) || (GetCurrentChar()=='>') )
					break;
				
				//����,˳�ζ�ȡ����ַ�,�����ƶ��ַ�
				m_tag+=char.ToUpper(GetCurrentChar());
				Advance();
			}
			//�������������\r\n\t���ո�
			EatWhiteSpace();
			
			// ��ȡTag�������
			while ( GetCurrentChar() != '>' && !Eof()) 
			{
				ParseName = "";
				ParseValue = "";
				ParseDelim = (char)0;
				//����������
				ParseAttributeName();
				//�����ǰ�ַ���">",��������ӵ��б���,��������ǰ��ǽ���
				if ( GetCurrentChar()=='>' ) 
				{
					AddAttribute();
					break;
				}
				//��δ����>ǰ,��������ֵ
				ParseAttributeValue();
				AddAttribute();
			}
			Advance();
		}

		/// <summary>
		/// HTML�ַ�����
		/// </summary>
		/// <returns></returns>
		protected char Parse()
		{
			if( GetCurrentChar()=='<' ) 
			{
				//�����ȡ���ַ���"<",������Tag,ǰ��һ���ַ�,����ʼTag����
				Advance();
				char ch=char.ToUpper(GetCurrentChar());
				//�����ȡ���ַ���A-Z����!����/,�����Tag���
				if ( (ch>='A') && (ch<='Z') || (ch=='!') || (ch=='/') ) 
				//if(ch=='T')
				{
					ParseTag();
					return (char)0;
				} 
					//����ǰ�ַ�����A-Z��/��ֱ�ӷ��ص�ǰ�ַ�
				else 
					return(AdvanceCurrentChar());
			} 
				//����һ���ַ�,ֱ�ӽ��䷵��
			else 
				return(AdvanceCurrentChar());
		}


		/// <summary>
		/// HTML�ĵ�����,�����һ��Tag�б���
		/// </summary>
		/// <returns>Tag�б���</returns>
		public ArrayList Parsing()
		{
			ArrayList tags = new ArrayList();
			StringBuilder tagText = new StringBuilder();
			Tag newTag = new Tag();
			while(!Eof())
			{
				//˳�ν���ÿ���ַ�
				char ch = Parse();
				//���ch����0,��ʾ����һ���µ�Tag���
				if(ch==0)
				{
					//������ı�����,���������ı�����,����ʵ�ʱ��
					if(tagText.Length>0 || newTag.TagName != "")
					{
						newTag.FollowedText = tagText.ToString();
						tags.Add(newTag);
						tagText = new StringBuilder();
					}
					
					//���²���һ��Tag����,����ǰ��Ǹ����±��
					newTag = new Tag();
					string tagName = GetTagName();
					newTag.TagName = tagName;
					newTag.TagAttributes = (ArrayList)List.Clone();
				}
				else
				{
					//��������ո���(&nbsp;)ǰ��5���ַ�������
					//ch=&,��ǰ�ַ���n,�����ַ���bsp;
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
			//�����������������
			//*----------------��ʱȥ��
			if(GetTagName() != null && GetTagName().StartsWith("/"))
			{
				//��ӽ������
				tags.Add(newTag);
				//������Ǻ����ı�,�����±��,����ʣ���ı�д��ձ�Ǻ�
				if(tagText.Length>0)
				{
					newTag = new Tag();
					newTag.TagName = string.Empty;
					newTag.FollowedText = tagText.ToString();
					tags.Add(newTag);
				}
			}
			//���û�����κα��,�����Ǵ��ı�
			//����һ�ձ��,�����ı�д��ձ�Ǻ�
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
