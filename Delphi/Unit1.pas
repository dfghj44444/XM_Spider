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
  Memo1.Lines.Add('初始化......');
  Form1CutString:= TCutString.Create;
  Form1CutString.InitUrlList;
  Form1CutString.iniFileInit;
  Form1CutString.DBConnect('127.0.0.1',true);
  Memo1.Lines.Clear;
  Memo1.Lines.Add('初始化......成功');

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
      StatusBar1.Panels[0].Text :='当前地址：'+errorurl;
      str_source :='';
      str_source := stream2.DataString;
      stream2.Free;

      str_source := Form1CutString.GetstrFromHTML('比较</td>','分页显示',str_source);
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

          //str_17 := '未知';
          
          stream2 := TStringStream.Create('');
          IdAntiFreeze1.OnlyWhenIdle := False;
          if IdHttp3.Connected then IdHttp3.Disconnect;
          sleep(500);
          IdHttp3.Get(errorurl1,stream2);
          str_source01 :='';
          str_source01 := stream2.DataString;
          stream2.Free;

          str_15 := Form1CutString.GetstrFromHTML('<h1>','</h1>',str_source01);

          str_1 := Form1CutString.GetstrFromHTML('楼盘视频　』','我来纠错',str_source01);
          str_1 := Form1CutString.ClearAllHtml(str_1);
          str_1 := StringReplace(str_1,'　','',[rfReplaceAll]);

          // 区属
          str_01 := Form1CutString.GetstrFromHTML('区属：','板块：',str_1);
          str_1 := StringReplace(str_1,'区属：'+str_01,'',[rfIgnoreCase]);
          str_01 := StringReplace(str_01,'同区域楼盘','',[rfIgnoreCase]);

          // 项目地址
          str_02 := Form1CutString.GetstrFromHTML('项目地址：','点评交通',str_1);
          str_1 := StringReplace(str_1,'项目地址：'+str_02,'',[rfIgnoreCase]);

          // 项目类别
          str_03 := Form1CutString.GetstrFromHTML('项目类别：','价格：',str_1);
          str_1 := StringReplace(str_1,'项目类别：'+str_03,'',[rfIgnoreCase]);

          // 价格
          str_04 := Form1CutString.GetstrFromHTML('价格：','纠错',str_1);
          str_04 := Form1CutString.GetstrFromHTML('最新价格：','累计均价：',str_1);
          str_1 := StringReplace(str_1,'最新价格：'+str_04,'',[rfIgnoreCase]);

          // 物业开发商
          str_05 := Form1CutString.GetstrFromHTML('物业开发商：','销售许可证：',str_1);
          str_1 := StringReplace(str_1,'物业开发商：'+str_05,'',[rfIgnoreCase]);

          str_06 := Form1CutString.GetstrFromHTML('销售许可证：','最新预售楼盘',str_1);
          str_1 := StringReplace(str_1,'销售许可证：'+str_06,'',[rfIgnoreCase]);

          // 开盘日期
          str_07 := Form1CutString.GetstrFromHTML('开盘日期：','交付时间：',str_1);
          str_1 := StringReplace(str_1,'开盘日期：'+str_07,'',[rfIgnoreCase]);

          str_08 := Form1CutString.GetstrFromHTML('交付时间：','交付标准：',str_1);
          str_1 := StringReplace(str_1,'交付时间：'+str_08,'',[rfIgnoreCase]);
          
          str_09 := Form1CutString.GetstrFromHTML('交付标准：','样板房图库',str_1);
          str_1 := StringReplace(str_1,'交付标准：'+str_09,'',[rfIgnoreCase]);
          
          str_10 := Form1CutString.GetstrFromHTML('交通状况：','容积率：',str_1);
          str_1 := StringReplace(str_1,'交通状况：'+str_10,'',[rfIgnoreCase]);
          
          str_11 := Form1CutString.GetstrFromHTML('容积率：','购房工具',str_1);
          str_1 := StringReplace(str_1,'容积率：'+str_11,'',[rfIgnoreCase]);
          
          str_12 := Form1CutString.GetstrFromHTML('绿化率：','付款方式：',str_1);
          str_1 := StringReplace(str_1,'绿化率：'+str_11,'',[rfIgnoreCase]);

          str_1 := copy(str_1,pos('更新日期：',str_1)+10,length(str_1));
          str_1 := Form1CutString.ClearAllHtml(str_1);

          // 更新时间
          if(pos('相册',str_1)>0) then
          begin
          str_13 := copy(str_1,0,pos('相册',str_1)-1);
          str_1 := StringReplace(str_1,str_13,'',[rfIgnoreCase]);
          end
          else
          begin
          str_13 := copy(str_1,0,pos('销售热线',str_1)-1);
          str_1 := StringReplace(str_1,str_13,'',[rfIgnoreCase]);
          end;

          // 电话
          str_14 := copy(str_1,pos('销售热线：',str_1)+10,length(str_1));

          // 网址 区属 项目名称 项目地址 项目类别 价格 物业开发商 开盘日期 更新日期 电话
          Form1CutString.ExecQ(errorurl1,str_01,str_15,str_02,str_03,str_04,str_05,str_07,str_13,str_14,str_17);

          end;
          //stream2.Free;
          except on E:Exception do
            begin
            Memo1.Lines.Add(errorurl1+'下载失败');
            Form1CutString.WriteErrorSysLog(Formatdatetime('YYYY-MM-DD ' +
            'HH:MM:SS',Now) + errorurl1 +'下载失败2！' +Pchar(E.Message));
            end;
          end;
        end;
        end;
      end;
      except on E:Exception do
        begin
        Memo1.Lines.Add(errorurl+'下载失败');
        Form1CutString.WriteErrorSysLog(Formatdatetime('YYYY-MM-DD ' +
        'HH:MM:SS',Now) + errorurl +'下载失败1！' +Pchar(E.Message));
        end;
      end;
    end;
end.
