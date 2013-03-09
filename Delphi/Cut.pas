unit Cut;


interface
uses
  Classes ,SysUtils ,Windows, StrUtils, RegExpr, DB, ADODB, IniFiles, Forms, QDialogs;
type
  TCutString = Class(TObject)
  private

  public
  //----------------------------------------------------------------------------
  Function CheckUrl(url:string):Boolean;
  function GetAll(source,str1,str2,str3,str4,str5,str6,str7,str8,str9,str10,str11,str12,str13,str14,str15,str16,str17,str18,str19,str20,str21,str22:string):string;
  function ClearAllHtml(source : string) : string;
  function ClearAllHtml1(source : string) : string;
  //ȡ����
  //ͨ��pos'>'
  Function  Deleteelse(str :string):string;
  //ֱ�ӶԱ�����
  Function  CNum(str :string):string;
  //ȡҳ��
  Function CutPage(str_left,str_right,str_source:string):string;overload;
  Function CutPage(str_left1,str_right1,str_left2,str_right2,
    str_source:string):string;overload;

  //ִ��SQL
  function ExecQ(str_url,str_01,str_02,str_03,str_04,str_05,str_06,str_07,str_08,str_09,str_10 : string): boolean;
  //��ʼ��LIST
  procedure InitUrlList;
  //��ʼ��ini�ļ�
  procedure iniFileInit();
  //���ݿ�����
  Function DBConnect(IP:string;ConFlag:boolean):boolean;
  //д������־
  procedure WriteErrorSysLog(Errorstring : string);
  //��ȡ������URL
  function Parseurl (const AInputString : string) : string;
  //��ȡ����������
  function Parsezk (const AInputString : string) : string;
  //��ȡ����������
  function ParseDate (const AInputString : string;num: integer) : string;
  //��ȡ�ַ�������ͷ��
  function GetstrFromHTML(prefix, posfix, htmlfile: string): string;
  // ��д�����滻Сд
  function StringReplaceNUM(source : string) : string;
  // ��ȡ����
  function DeleteIllegal(source : string) : string;
  // ������ת����ʱ���ʽ
  function StringReplaceDate(source : string) : string;
  //�ָ��ַ���
  procedure SeparateTerms(s : string;Separator : string;Terms : TStringList);
  //�滻�ַ���

  Function  StrReplace(str1,str2,str3,str4,source:string):string;overload;
  Function  StrReplace(str1,str2,str3,str4,str5,str6,
    source:string):string;overload;
  Function  StrReplace(str1,str2,str3,str4,str5,str6,str7,str8,source:string):string;overload;

  Function  ReplaceBlack(str:string):string;    
end;
var
  //---------------------------------------------------------------------------
  //���ݿ����ӱ���-
  ExecCon: TADOConnection;//����������
  ExecCmd: TADOQuery;
  IP : string;
  DBName :string;
  LoginUserID :string;
  Password :string;
  UList : TStringList;
  DList : TStringList;
  TList : TStringList;
  AllList : TStringList;
  errorurl1,errorurl2,errorurl3,errorurl4 : string;
implementation

{ CutString }

function TCutString.DBConnect(IP: string; ConFlag: boolean): boolean;
var
    ConnStr:string;
begin
  if (ExecCon<>nil) and (ExecCon.Connected) then
  begin
    ExecCon.Free;
    ExecCmd.Free;
    ExecCon := nil;
    ExecCmd := nil;
  end;
  try
    if ConFlag then
    begin
      ConnStr := 'Provider=SQLOLEDB.1';
      if IP='' then
      begin
        ConnStr := ConnStr+';Data Source=127.0.0.1';
      end
      else
      begin
        ConnStr := ConnStr+';Data Source='+IP;
      end;
      ConnStr := ConnStr+';Initial Catalog='+DBName;
      ConnStr := ConnStr+';User ID='+LoginUserID;
      ConnStr := ConnStr+';Password='+Password;
      ////
      ExecCon := TADOConnection.Create(nil);
      ExecCon.ConnectionString := ConnStr;
      ExecCon.LoginPrompt := false;
      ExecCon.Open;
      ExecCmd:=TADOQuery.Create(nil);
      ExecCmd.Connection := ExecCon;
    end;
    Result:=true;
  except on E: Exception do
    begin
      WriteErrorSysLog(Formatdatetime('YYYY-MM-DD HH:MM:SS',Now)+'   '+Pchar(E.Message));
      Result:=false;
    end;
  end;
end;

function TCutString.DeleteIllegal(source: string): string;
var 
  i : integer;
begin 
  Result := '';
  for i := 1 to Length(source) do
  if source[i] in [chr(128)..chr(255)] then
  Result := Result + source[i];
end;

function TCutString.GetstrFromHTML(prefix, posfix,
  htmlfile: string): string;
var
   iPos, ePos: integer;
begin
  try
    iPos := Pos(prefix, htmlfile);
    if iPos > 0 then
    begin
      if posfix <> '' then
      begin
        ePos := PosEx(posfix, htmlfile, iPos);
        //if ePos = 0 then
          //ePos := Length(htmlfile);
      end
      else
        ePos := Length(htmlfile);
      Result := Trim(Copy(htmlfile, iPos + Length(prefix), ePos - iPos - Length(prefix)));
    end;
  except
  end;
end;



procedure TCutString.iniFileInit;
var INIFile:TIniFile;
begin
  INIFile:=TIniFile.Create(ExtractFilePath(Application.ExeName)+'config.ini');
  IP :=INIFile.ReadString('LOCAL','IP','127.0.0.1');
  DBName :=INIFile.ReadString('LOCAL','DBName','monitor');
  LoginUserID :=INIFile.ReadString('LOCAL','LoginID','sa');
  Password :=INIFile.ReadString('LOCAL','Password','');
  INIFile.Free;
end;

procedure TCutString.InitUrlList;
begin
  UList := TStringList.Create;
  DList := TStringList.Create;
  TList := TStringList.Create;
  AllList := TStringList.Create;
end;

function TCutString.ParseDate(const AInputString: string;num: integer): string;
var
	r : TRegExpr;
  DateRE : string;
begin
  case num of
  1:
  begin
    DateRE :=  '2008-\d-\d\d*';
  end;
  2:
  begin
    DateRE :=  '2008-\d\d-\d\d*';
  end;
  end;
	Result := '';
	r := TRegExpr.Create;
	try
		r.Expression := DateRE;
		if r.Exec (AInputString) then
REPEAT
        if length(r.Match [0])>7 then
         begin
          Result := Result + r.Match [0] + ',';
         end
			UNTIL not r.ExecNext;
		finally r.Free;
	end;
end;

function TCutString.Parseurl(const AInputString: string): string;
const
  IntURLRE = 'http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?';
var
	r : TRegExpr;
  tempstr : string;
begin
  tempstr :='';
	Result := '';
	r := TRegExpr.Create; // Create object
	try // ensure memory clean-up
		r.Expression := IntURLRE;
		if r.Exec(AInputString) then
REPEAT
        if length(r.Match [0])>7 then
         begin
          tempstr := r.Match [0];
          //if pos(tempstr,Result)=0 then
				  Result := trim(Result) + Trim(r.Match [0]) + ',';
         end
			UNTIL not r.ExecNext;
		finally r.Free;
	end;
end;

function TCutString.Parsezk(const AInputString: string): string;
const
  IntURLRE = '\[(.*?)\]';
var
	r : TRegExpr;
  tempstr : string;
begin
  tempstr :='';
	Result := '';
	r := TRegExpr.Create; // Create object
	try // ensure memory clean-up
		r.Expression := IntURLRE;
		if r.Exec(AInputString) then
REPEAT
        //if length(r.Match [0])>7 then
         begin
          tempstr := r.Match [0];
          //if pos(tempstr,Result)=0 then
				  Result := trim(Result) + Trim(r.Match [0]) + ',';
         end
			UNTIL not r.ExecNext;
		finally r.Free;
	end;
end;

procedure TCutString.SeparateTerms(s: string; Separator: string;
  Terms: TStringList);
  var
  hs : string;
  p : integer;
begin
  Terms.Clear; // ����ַ����е�����
  if Length(s)=0 then   // ����Ϊ0
    Exit;
  p:=Pos(Separator,s);
  while P<>0 do
  begin
    hs:=Copy(s,1,p-1);   // �����ַ�
    Terms.Add(hs);       // ��ӵ��б�
    Delete(s,1,p);       // ɾ���ַ��ͷָ��
    p:=Pos(Separator,s); // ���ҷָ��
  end;
  if Length(s)>0 then
    Terms.Add(s);        // ���ʣ�µ���Ŀ
end;

function TCutString.StrReplace(str1, str2, str3, str4,
  source:string):string;
var
tempsource : string;
begin
  tempsource := StringReplace(source,str1,str2,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str3,str4,[rfReplaceAll]);
  result := Trim(tempsource);
end;

function TCutString.StringReplaceDate(source: string): string;
begin
  Result := Trim(StringReplace(source,'��','-',[rfReplaceAll]));
  Result := Trim(StringReplace(Result,'��','-',[rfReplaceAll]));
  Result := Trim(StringReplace(Result,'��','',[rfReplaceAll]));
end;

function TCutString.StringReplaceNUM(source: string): string;
begin

end;

procedure TCutString.WriteErrorSysLog(Errorstring: string);
var
  F : TextFile;
  FilePath : string;
begin
  FilePath := ExtractFilePath(Application.ExeName)+'ErrorSysLog.txt';
  Assign(F,FilePath);
  if FileExists(FilePath) then
    Append(F)
  else
    begin
      Rewrite(F);
      Append(F);
    end;
    Writeln(F,Errorstring);
    CloseFile(F);
end;

function TCutString.StrReplace(str1, str2, str3, str4, str5, str6,
  source: string): string;
var
tempsource : string;
begin
  tempsource := StringReplace(source,str1,str2,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str3,str4,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str5,str6,[rfReplaceAll]);
  result := Trim(tempsource);
end;

function TCutString.StrReplace(str1, str2, str3, str4, str5, str6, str7,
  str8, source: string): string;
var
tempsource : string;
begin
  tempsource := StringReplace(source,str1,str2,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str3,str4,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str5,str6,[rfReplaceAll]);
  tempsource := StringReplace(tempsource,str7,str8,[rfReplaceAll]);
  result := Trim(tempsource);
end;


  var
    str_sql : string;
//
function TCutString.ExecQ(str_url,str_01,str_02,str_03,str_04,str_05,str_06,str_07,str_08,str_09,str_10 : string): boolean;
begin
  Result := True;
  if DBConnect(IP,True) then
  begin
    with ExecCmd do
    begin
      if CheckUrl(str_url) then exit;
      Close;
      SQL.Clear;
      str_sql := 'insert into ¥����Ϣ(��ַ,����,��Ŀ����,��Ŀ��ַ,��Ŀ���,�۸�,��ҵ������,��������,��������,�绰,¥������)'+
      'values(:prm_str_url,:prm_str_01,:prm_str_02,:prm_str_03,:prm_str_04,:prm_str_05,:prm_str_06,:prm_str_07,:prm_str_08,:prm_str_09,:prm_str_10)';
      SQL.Add(str_sql);
      Parameters.ParamByName('prm_str_url').Value := str_url;
      Parameters.ParamByName('prm_str_01').Value := str_01;
      Parameters.ParamByName('prm_str_02').Value := str_02;
      Parameters.ParamByName('prm_str_03').Value := str_03;
      Parameters.ParamByName('prm_str_04').Value := str_04;
      Parameters.ParamByName('prm_str_05').Value := str_05;
      Parameters.ParamByName('prm_str_06').Value := str_06;
      Parameters.ParamByName('prm_str_07').Value := str_07;
      Parameters.ParamByName('prm_str_08').Value := str_08;
      Parameters.ParamByName('prm_str_09').Value := str_09;
      Parameters.ParamByName('prm_str_10').Value := str_10;
      Prepared;
      try
        ExecSQL;
      except on E: Exception do
        begin
          WriteErrorSysLog(Formatdatetime('YYYY-MM-DD HH:MM:SS',Now)+'   '+Pchar(E.Message));
          Result:=false;
        end;
      end;
    end;
  end
  else
    Result:=false;
end;

function TCutString.ReplaceBlack(str: string): string;
var
  tempstr : string;
begin
  tempstr := Trim(StringReplace(str,'<td>','',[rfReplaceAll]));
  tempstr := Trim(StringReplace(tempstr,'&nbsp;','',[rfReplaceAll]));
  tempstr := Trim(StringReplace(tempstr,'</td>','',[rfReplaceAll]));
  tempstr := Trim(StringReplace(tempstr,#10,' ',[rfReplaceAll]));
  Result := Trim(tempstr);
end;

function TCutString.CutPage(str_left, str_right,
  str_source: string): string;
begin
  Result := GetstrFromHTML(str_left,str_right,str_source);
end;

function TCutString.CutPage(str_left1, str_right1, str_left2, str_right2,
  str_source: string): string;
begin
  Result := GetstrFromHTML(str_left1,str_right1,str_source);
  Result := GetstrFromHTML(str_left2,str_right2,Result);
end;

function TCutString.Deleteelse(str: string): string;
begin
  Result := copy(str,pos('>',str)+1,length(str)-pos('>',str));
end;

function TCutString.CNum(str: string): string;
var
  i : integer;
begin
  for i := 1 to length(str) do
  if str[i] in ['0'..'9'] then
  begin
    Result := Result + str[i];
  end;
end;

function TCutString.ClearAllHtml1(source: string): string;
begin
     Result := StrReplace(''#$A'','','','',source);
end;

function TCutString.ClearAllHtml(source: string): string;
var
  pos_l,Pos_r,i : integer;
  tempstr,str : string;
begin
  str:='''';
  i :=1;
  source:= StrReplace(''#$D#$A'','',''#10'','',''#9'','','#$D#$A','',source);
  source:= StrReplace('<br />','$',' ','','&nbsp;','','','',source);

REPEAT
  if pos('>',source)>0 then
  begin
    i:= i+1;
    pos_l := pos('<',source);
    Pos_r := pos('>',source);
    tempstr := copy(source,pos_l,Pos_r-pos_l+1);
    source := Trim(StringReplace(source,tempstr,'',[rfReplaceAll]));
  end;
until (pos('>',source)=0) or (i >5000);
Result := StrReplace('$','<br />','','','','','','',source);
end;


function TCutString.GetAll(source, str1, str2, str3, str4, str5, str6,
  str7, str8, str9, str10, str11, str12, str13, str14, str15, str16, str17,
  str18, str19, str20, str21, str22: string): string;
begin
  Result := GetstrFromHTML(str1,str2,source);
  Result := Result +', '+ GetstrFromHTML(str2,str3,source);
  Result := Result +', '+ GetstrFromHTML(str3,str4,source);
  Result := Result +', '+ GetstrFromHTML(str4,str5,source);
  Result := Result +', '+ GetstrFromHTML(str5,str6,source);
  Result := Result +', '+ GetstrFromHTML(str6,str7,source);
  Result := Result +', '+ GetstrFromHTML(str7,str8,source);
  Result := Result +', '+ GetstrFromHTML(str8,str9,source);
  Result := Result +', '+ GetstrFromHTML(str9,str10,source);
  Result := Result +', '+ GetstrFromHTML(str10,str11,source);
  Result := Result +', '+ GetstrFromHTML(str11,str12,source);
  Result := Result +', '+ GetstrFromHTML(str12,str13,source);
  Result := Result +', '+ GetstrFromHTML(str13,str14,source);
  Result := Result +', '+ GetstrFromHTML(str14,str15,source);
  Result := Result +', '+ GetstrFromHTML(str15,str16,source);
  Result := Result +', '+ GetstrFromHTML(str16,str17,source);
  Result := Result +', '+ GetstrFromHTML(str17,str18,source);
  Result := Result +', '+ GetstrFromHTML(str18,str19,source);
  Result := Result +', '+ GetstrFromHTML(str19,str20,source);
  Result := Result +', '+ GetstrFromHTML(str20,str21,source);
  Result := Result +', '+ GetstrFromHTML(str21,str22,source);
end;

function TCutString.CheckUrl(url: string): Boolean;
var
  sqlstr : string;
begin
Result := false;
with ExecCmd do
    begin
      Close;
      SQL.Clear;
      sqlstr := 'select * from ¥����Ϣ where ��ַ ='''+url+'''';
      SQL.Add(sqlstr);
      Prepared;
      try
        Open();
        if RecordCount > 0 then
          Result := true;
      except on E: Exception do
        {begin
          WriteErrorSysLog(Formatdatetime('YYYY-MM-DD HH:MM:SS',Now)+'   '+
            Pchar(E.Message));
        end;}
      end;
end;
end;

end.
