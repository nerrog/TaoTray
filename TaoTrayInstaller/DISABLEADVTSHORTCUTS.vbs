Option Explicit

Const msiOpenDatabaseModeTransact = 1

Const msiViewModifyInsert = 1
Const msiViewModifyUpdate = 2

Dim msiPath : msiPath = Wscript.Arguments(0)

Dim installer
Set installer = Wscript.CreateObject("WindowsInstaller.Installer")
Dim database
Set database = installer.OpenDatabase(msiPath, msiOpenDatabaseModeTransact)

Dim query
query = "Select * FROM Property WHERE Property='DISABLEADVTSHORTCUTS'"
Dim view
Set view = database.OpenView(query)
view.Execute
Dim record
Set record = view.Fetch
Dim viewModify
viewModify = msiViewModifyUpdate
'DISABLEADVTSHORTCUTSがない時
If record Is Nothing Then
    Set record = installer.CreateRecord(2)
    viewModify = msiViewModifyInsert
End If
record.StringData(1) = "DISABLEADVTSHORTCUTS"
record.StringData(2) = "1"
view.Modify viewModify, record
database.Commit