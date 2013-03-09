program Project1;

{%File 'Cut.~pas'}

uses
  Forms,
  Unit1 in 'Unit1.pas' {Form1},
  Cut in 'Cut.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.
