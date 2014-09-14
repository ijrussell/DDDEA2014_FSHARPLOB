

type Agent<'T> = MailboxProcessor<'T>
 
let agent =
   Agent.Start(fun inbox ->
     async { while true do
               let! msg = inbox.Receive()
               printfn "got message '%s'" msg } )

for i in 1 .. 10000 do
   agent.Post (sprintf "message %d" i)


// -------------------------------------------------------------------
// Naive Agent communication

let mailAgent =
   Agent.Start(fun inbox ->
     async { while true do
               let! msg = inbox.Receive()
               printfn "Sending mail -> '%s'\n" msg } )

let agent =
   Agent.Start(fun inbox ->
     async { while true do
               let! msg = inbox.Receive()
               printfn "Processing message '%s'\n" msg 
               mailAgent.Post msg } )

for i in 1 .. 10000 do
   agent.Post (sprintf "message %d" i)

// -------------------------------------------------
// Lots of agents

let agents =
    [ for i in 0 .. 100000 ->
       Agent.Start(fun inbox ->
         async { while true do
                   let! msg = inbox.Receive()
                   if i % 10000 = 0 then
                       printfn "agent %d got message '%s'" i msg } ) ]
 
for agent in agents do
    agent.Post "ping!"