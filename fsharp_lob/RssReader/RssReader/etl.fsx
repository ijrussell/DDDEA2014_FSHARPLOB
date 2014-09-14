#r "bin/debug/FSharp.Data.SqlProvider.dll"

open FSharp.Data.Sql

[<Literal>] 
//let excelCs = @"Driver={Microsoft Excel Driver (*.xls, *.xlsx)};DriverId=1046;Dbq=C:\temp\test.xlsx;DefaultDir=c:\temp;"
let excelCs = @"DefaultDir=c:\temp;DRIVER={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};FIL=Excel 12.0;DriverID=1046;DBQ=c:\temp\test.xlsx;Readonly=True;"

type xl = SqlDataProvider<excelCs, Common.DatabaseProviderTypes.ODBC>

let spreadsheet = xl.GetDataContext()

//Circuit_Breakers
let circuitBreakers = 
    spreadsheet.``[].[Circuit_Breakers$]``
    |> Seq.iter (fun row -> printfn "%f = %s" row.``"OS_Record_No"`` row.``"Mnemonic"``)


