namespace FsSuccinctly.RobotsMvvm.ViewModel

open System
open System.Reflection
open System.Windows
open System.Windows.Markup
open System.Windows.Controls
open System.ComponentModel
open System.Collections.ObjectModel

type Robot =
    { Name: string
      Movement: string
      Weapon: string }

type RobotsRepository() =
    member x.GetAll() =
        seq{ yield {Name = "Kyrton" 
                    Movement = "2 Legs"
                    Weapon = "None" }
             yield {Name = "R2-D2"
                    Movement = "3 Legs with wheels"
                    Weapon = "Electric sparks" }    
             yield {Name = "Johnny 5" 
                    Movement = "Caterpillars"
                    Weapon = "Laser beam" }
             yield {Name = "Turminder Xuss"
                    Movement = "Fields"
                    Weapon = "Knife missiles" }
           }

 type RobotsViewModel(robotsRepository: RobotsRepository) =   
    // backing field of the on property change event
    let propertyChangedEvent = 
        new DelegateEvent<PropertyChangedEventHandler>()
        
    // our collection of robots
    let robots =
        let allRobots = robotsRepository.GetAll()
        new ObservableCollection<Robot>(allRobots)

    // the currently selected robot,
    // initialized to an empty robot
    let mutable selectedRobot = 
        {Name=""; Movement=""; Weapon= ""}
    
    // default constructor, which creates a repository
    new () = new RobotsViewModel(new RobotsRepository())

    // implementing the INotifyPropertyChanged interface
    // so the GUI can react to events
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChangedEvent.Publish

    // helper method to raise the property changed event
    member x.OnPropertyChanged propertyName = 
        let parameters: obj[] = 
            [| x; new PropertyChangedEventArgs(propertyName) |]
        propertyChangedEvent.Trigger(parameters)

    // collection of robots that the GUI will data bind to
    member x.Robots = 
        robots

    // currently selected robot that the GUI will data bind to
    member x.SelectedRobot 
        with get () = selectedRobot
        and set value = 
            selectedRobot <- value
            x.OnPropertyChanged "SelectedRobot"
              
module Program =                         
    // load a XAML file from the current assembly
    let loadWindowFromResources name =
        let currentAssem = Assembly.GetExecutingAssembly()
        use xamlStream = currentAssem.GetManifestResourceStream(name)
        // resource stream will be null if not found
        if xamlStream = null then
            failwithf "The resouce stream '%s' was not found" name 
        XamlReader.Load(xamlStream) :?> Window 
    
    // create the window
    let win = loadWindowFromResources "RobotsWindow.xaml"

    // create the application object and show the window
    let app = new Application()

    [<STAThread>]
    do app.Run win |> ignore  