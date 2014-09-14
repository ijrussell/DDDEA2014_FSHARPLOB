#r @"C:\Users\Ian-Laptop\Dropbox\dddnorth\RssReader\RssReader\bin\debug\FSharp.Data.dll"

open Microsoft.FSharp.Data

type DbConnection = SqlDataConnection<ConnectionStringName="FsMvcAppExample", ConfigFile="web.config">

type GuitarsRepository2() =
    member x.GetAll () =
        use context = DbConnection.GetDataContext()
        query { for g in context.Guitars do
                select (Guitar(Id = g.Id, Name = g.Name)) }
        |> Seq.toList