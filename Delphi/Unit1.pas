unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, IdBaseComponent, IdAntiFreezeBase, IdAntiFreeze,
  IdComponent, IdTCPConnection, IdTCPClient, IdHTTP,StrUtils, DB,
  ADODB, Menus, RegExpr, Cut, ComCtrls;

type
  TForm1 = class(TForm)
    Button1: TButton;
    IdHTTP2: TIdHTTP;
    IdHTTP3: TIdHTTP;
    IdAntiFreeze1: TIdAntiFreeze;
    StatusBar1: TStatusBar;
    Memo1: TMemo;
    IdHTTP4: TIdHTTP;
    IdHTTP5: TIdHTTP;
    IdHTTP6: TIdHTTP;
    IdHTTP7: TIdHTTP;
    IdHTTP8: TIdHTTP;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;
  Form1CutString: TCutString;
implementation

{$R *.dfm}

procedure TForm1.FormCreate(Sender: TObject);
begin
  Memo1.Lines.Add('��ʼ��......');
  Form1CutString:= TCutString.Create;
  Form1CutString.InitUrlList;
  Form1CutString.iniFileInit;
  Form1CutString.DBConnect('127.0.0.1',true);
  Memo1.Lines.Clear;
  Memo1.Lines.Add('��ʼ��......�ɹ�');

end;


procedure TForm1.FormDestroy(Sender: TObject);
begin
  Form1CutString.Free;
end;

procedure TForm1.Button1Click(Sender: TObject);
var
  stream2 : TStringStream;
  str_source,str_source01 : string;
  str_1,str_url : string;
  str_01,str_02,str_03,str_04,str_05 : string;
  str_06,str_07,str_08,str_09,str_10 : string;
  str_11,str_12,str_13,str_14,str_15 : string;
  str_16,str_17 : string;
  i,j : integer;
  errorurl : string;
  errorurl1 : string;
  begin
  try
    begin
      for j := 1 to 40 do
      begin
      errorurl := 'http://newhouse.house365.com/search.php?page='+inttostr(j);
      stream2 := TStringStream.Create('');
      IdAntiFreeze1.OnlyWhenIdle := False;
      if IdHttp2.Connected then IdHttp2.Disconnect;
      sleep(1000);
      IdHTTP2.Get(errorurl,stream2);
      StatusBar1.Panels[0].Text :='��ǰ��ַ��'+errorurl;
      str_source :='';
      str_source := stream2.DataString;
      stream2.Free;

      str_source := Form1CutString.GetstrFromHTML('�Ƚ�</td>','��ҳ��ʾ',str_source);
      str_source := Form1CutString.StrReplace('<a href="http://bbs.house365.com','','','','http://','','list-','http://newhouse.house365.com/list-',str_source);

      str_url := Form1CutString.Parseurl(str_source);
      str_16 := Form1CutString.Parsezk(str_source);

      UList.Clear;
      Form1CutString.SeparateTerms(str_url,',',UList);
      DList.Clear;
      Form1CutString.SeparateTerms(str_16,',',DList);
      for i := 0  to UList.Count-1 do
        begin
          try
          begin
          //errorurl1 := Trim(UList.Strings[i]);
          errorurl1 := 'http://newhouse.house365.com/list-1289/';

          str_17 := Trim(DList.Strings[i]);
          str_17 := Trim(StringReplace(StringReplace(str_17,'[','',[rfReplaceAll]),']','',[rfReplaceAll]));

          //str_17 := 'δ֪';
          
          stream2 := TStringStream.Create('');
          IdAntiFreeze1.OnlyWhenIdle := False;
          if IdHttp3.Connected then IdHttp3.Disconnect;
          sleep(500);
          IdHttp3.Get(errorurl1,stream2);
          str_source01 :='';
          str_source01 := stream2.DataString;
          stream2.Free;

          str_15 := Form1CutString.GetstrFromHTML('<h1>','</h1>',str_source01);

          str_1 := Form1CutString.GetstrFromHTML('¥����Ƶ����','��������',str_source01);
          str_1 := Form1CutString.ClearAllHtml(str_1);
          str_1 := StringReplace(str_1,'��','',[rfReplaceAll]);

          // ����
          str_01 := Form1CutString.GetstrFromHTML('������','��飺',str_1);
          str_1 := StringReplace(str_1,'������'+str_01,'',[rfIgnoreCase]);
          str_01 := StringReplace(str_01,'ͬ����¥��','',[rfIgnoreCase]);

          // ��Ŀ��ַ
          str_02 := Form1CutString.GetstrFromHTML('��Ŀ��ַ��','������ͨ',str_1);
          str_1 := StringReplace(str_1,'��Ŀ��ַ��'+str_02,'',[rfIgnoreCase]);

          // ��Ŀ���
          str_03 := Form1CutString.GetstrFromHTML('��Ŀ���','�۸�',str_1);
          str_1 := StringReplace(str_1,'��Ŀ���'+str_03,'',[rfIgnoreCase]);

          // �۸�
          str_04 := Form1CutString.GetstrFromHTML('�۸�','����',str_1);
          str_04 := Form1CutString.GetstrFromHTML('���¼۸�','�ۼƾ��ۣ�',str_1);
          str_1 := StringReplace(str_1,'���¼۸�'+str_04,'',[rfIgnoreCase]);

          // ��ҵ������
          str_05 := Form1CutString.GetstrFromHTML('��ҵ�����̣�','�������֤��',str_1);
          str_1 := StringReplace(str_1,'��ҵ�����̣�'+str_05,'',[rfIgnoreCase]);

          str_06 := Form1CutString.GetstrFromHTML('�������֤��','����Ԥ��¥��',str_1);
          str_1 := StringReplace(str_1,'�������֤��'+str_06,'',[rfIgnoreCase]);

          // ��������
          str_07 := Form1CutString.GetstrFromHTML('�������ڣ�','����ʱ�䣺',str_1);
          str_1 := StringReplace(str_1,'�������ڣ�'+str_07,'',[rfIgnoreCase]);

          str_08 := Form1CutString.GetstrFromHTML('����ʱ�䣺','������׼��',str_1);
          str_1 := StringReplace(str_1,'����ʱ�䣺'+str_08,'',[rfIgnoreCase]);
          
          str_09 := Form1CutString.GetstrFromHTML('������׼��','���巿ͼ��',str_1);
          str_1 := StringReplace(str_1,'������׼��'+str_09,'',[rfIgnoreCase]);
          
          str_10 := Form1CutString.GetstrFromHTML('��ͨ״����','�ݻ��ʣ�',str_1);
          str_1 := StringReplace(str_1,'��ͨ״����'+str_10,'',[rfIgnoreCase]);
          
          str_11 := Form1CutString.GetstrFromHTML('�ݻ��ʣ�','��������',str_1);
          str_1 := StringReplace(str_1,'�ݻ��ʣ�'+str_11,'',[rfIgnoreCase]);
          
          str_12 := Form1CutString.GetstrFromHTML('�̻��ʣ�','���ʽ��',str_1);
          str_1 := StringReplace(str_1,'�̻��ʣ�'+str_11,'',[rfIgnoreCase]);

          str_1 := copy(str_1,pos('�������ڣ�',str_1)+10,length(str_1));
          str_1 := Form1CutString.ClearAllHtml(str_1);

          // ����ʱ��
          if(pos('���',str_1)>0) then
          begin
          str_13 := copy(str_1,0,pos('���',str_1)-1);
          str_1 := StringReplace(str_1,str_13,'',[rfIgnoreCase]);
          end
          else
          begin
          str_13 := copy(str_1,0,pos('��������',str_1)-1);
          str_1 := StringReplace(str_1,str_13,'',[rfIgnoreCase]);
          end;

          // �绰
          str_14 := copy(str_1,pos('�������ߣ�',str_1)+10,length(str_1));

          // ��ַ ���� ��Ŀ���� ��Ŀ��ַ ��Ŀ��� �۸� ��ҵ������ �������� �������� �绰
          Form1CutString.ExecQ(errorurl1,str_01,str_15,str_02,str_03,str_04,str_05,str_07,str_13,str_14,str_17);

          end;
          //stream2.Free;
          except on E:Exception do
            begin
            Memo1.Lines.Add(errorurl1+'����ʧ��');
            Form1CutString.WriteErrorSysLog(Formatdatetime('YYYY-MM-DD ' +
            'HH:MM:SS',Now) + errorurl1 +'����ʧ��2��' +Pchar(E.Message));
            end;
          end;
        end;
        end;
      end;
      except on E:Exception do
        begin
        Memo1.Lines.Add(errorurl+'����ʧ��');
        Form1CutString.WriteErrorSysLog(Formatdatetime('YYYY-MM-DD ' +
        'HH:MM:SS',Now) + errorurl +'����ʧ��1��' +Pchar(E.Message));
        end;
      end;
    end;
end.
