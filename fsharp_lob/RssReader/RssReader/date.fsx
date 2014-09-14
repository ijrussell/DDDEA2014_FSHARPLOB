#r "bin/debug/FSharp.Date.dll"

open FSharp.Date

type Date = DateProvider<2014>

let today = Date.``2014``.September.Monday.``08``