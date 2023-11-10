namespace Aidos.UI

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open Avalonia
open Avalonia.Controls
open AvaloniaEdit
open AvaloniaEdit.Document
open Microsoft.FSharp.Core
open FSharp.UMX
open MessagePack
open MessagePack.Resolvers
open MessagePack.FSharp


module Measures =

    [<Measure>] type Buffer
    [<Measure>] type Constraint
    [<Measure>] type Split
    [<Measure>] type Merge
    [<Measure>] type Conveyor
    [<Measure>] type Conversion
    [<Measure>] type Experiment

type BufferId = int<Measures.Buffer>
type ConstraintId = int<Measures.Constraint>
type SplitId = int<Measures.Split>
type MergeId = int<Measures.Merge>
type ConveyorId = int<Measures.Conveyor>
type ConversionId = int<Measures.Conversion>
type ExperimentId = int<Measures.Experiment>

type BufferName = string<Measures.Buffer>
module BufferName =
    let create (s: string) : BufferName = %s

type ConstraintName = string<Measures.Constraint>
module ConstraintName =
    let create (s: string) : ConstraintName = %s

type SplitName = string<Measures.Split>
module SplitName =
    let create (s: string) : SplitName = %s

type MergeName = string<Measures.Merge>
module MergeName =
    let create (s: string) : MergeName = %s

type ConveyorName = string<Measures.Conveyor>
module ConveyorName =
    let create (s: string) : ConveyorName = %s

type ConversionName = string<Measures.Conversion>
module ConversionName =
    let create (s: string) : ConversionName = %s

type ExperimentName = string<Measures.Experiment>

[<Struct; RequireQualifiedAccess>]
type NameType =
    | Buffer
    | Constraint
    | Split
    | Merge
    | Conveyor
    | Conversion

[<RequireQualifiedAccess>]
type Node =
    | Buffer of string<Measures.Buffer>
    | Constraint of string<Measures.Constraint>
    | Split of string<Measures.Split>
    | Merge of string<Measures.Merge>
    | Conveyor of string<Measures.Conveyor>
    | Conversion of string<Measures.Conversion>

    static member ofNameType
        (name: string)
        (nameType: NameType)
        =

        match nameType with
        | NameType.Buffer -> Node.Buffer (% name)
        | NameType.Constraint -> Node.Constraint (% name)
        | NameType.Split -> Node.Split (% name)
        | NameType.Merge -> Node.Merge (% name)
        | NameType.Conveyor -> Node.Conveyor (% name)
        | NameType.Conversion -> Node.Conversion (% name)

    member n.Name : string =
        match n with
        | Buffer b -> %b
        | Constraint c -> %c
        | Split s -> %s
        | Merge m -> %m
        | Conveyor c -> %c
        | Conversion c -> %c


[<Struct>]
type Description = Description of string

[<Struct; MessagePackObject>]
type Location =
    struct
        [<Key(0)>]
        val X: float
        [<Key(1)>]
        val Y: float

        new (x: float, y: float) =
            {
                X = x
                Y = y
            }
    end

[<MessagePackObject>]
type Layout =
    {
        [<Key(0)>]
        SitePoint: Map<Node, Location>
        [<Key(1)>]
        EdgePoints: Map<Node * Node, Location[]>
    }

[<MessagePackObject>]
type Elements =
    {
        [<Key(0)>]
        Nodes: Node[]
        [<Key(1)>]
        Links: (Node * Node)[]
        [<Key(2)>]
        Buffers: Map<string<Measures.Buffer>, Description>
        [<Key(3)>]
        Constraints: Map<string<Measures.Constraint>, Description>
        [<Key(4)>]
        Splits: Map<string<Measures.Split>, Description>
        [<Key(5)>]
        Merges: Map<string<Measures.Merge>, Description>
        [<Key(6)>]
        Conveyors: Map<string<Measures.Conveyor>, Description>
        [<Key(7)>]
        Conversions: Map<string<Measures.Conversion>, Description>
    }

[<RequireQualifiedAccess; MessagePackObject>]
type ModelStatus =
    | None
    | Error of string
    | Success of elements: Elements * layout: Layout

[<RequireQualifiedAccess>]
type PointerState =
    | Neutral
    | Panning

type FlowchartSettings =
    {
        Origin: Point
        Zoom: float
        PointerState: PointerState
        PointerPosition: Point
    }
    static member init =
        {
            Origin = Point (0.0, 0.0)
            Zoom = 1.0
            PointerState = PointerState.Neutral
            PointerPosition = Point (0.0, 0.0)
        }

type DiagramSettings =
    {
        NodeWidth: float
        NodeHeight: float
        EdgeStroke: string
        EdgeWidth: float
        FontSize: float
    }
    static member init =
        {
            NodeWidth = 100.0
            NodeHeight = 50.0
            EdgeStroke = "Red"
            EdgeWidth = 10.0
            FontSize = 12.0
        }

type AidosMapping =
    {
        Buffers: ReadOnlyDictionary<BufferName, string>
        Constraints: ReadOnlyDictionary<ConstraintName, string>
    }

[<RequireQualifiedAccess>]
type ConstraintReport =
    | FlowRate of constraintName: ConstraintName

[<RequireQualifiedAccess>]
type BufferReport =
    | Level of bufferName: BufferName

[<RequireQualifiedAccess>]
type HistoryReport =
    | None
    | Constraint of ConstraintReport
    | Buffer of BufferReport

[<RequireQualifiedAccess>]
type SystemReport =
    | OEE

[<RequireQualifiedAccess>]
type HistoriesReport =
    | None
    | System of SystemReport


[<RequireQualifiedAccess>]
type EditScreen =
    | ModelEditor
    | Buffers
    | Constraints
    | Splits
    | Merges
    | Conveyors
    | Conversions

[<RequireQualifiedAccess>]
type Screen =
    | Edit of editScreen: EditScreen
    | Reports
    | Experiments

module TimeUnit =

    [<Literal>]
    let seconds = "Seconds"
    [<Literal>]
    let minutes = "Minutes"
    [<Literal>]
    let hours = "Hours"
    [<Literal>]
    let days = "Days"
    [<Literal>]
    let weeks = "Weeks"

type TimeUnit =
    | Seconds
    | Minutes
    | Hours
    | Days
    static member ofString (s: string) =
        match s with
        | TimeUnit.seconds -> Seconds
        | TimeUnit.minutes -> Minutes
        | TimeUnit.hours -> Hours
        | TimeUnit.days -> Days
        | _ ->
            invalidArg (nameof s) "Unknown TimeUnit type"

    override tu.ToString () =
        match tu with
        | Seconds -> TimeUnit.seconds
        | Minutes -> TimeUnit.minutes
        | Hours -> TimeUnit.hours
        | Days -> TimeUnit.days

    static member createTimeSpan (duration: float) (tu: TimeUnit) =
        match tu with
        | Seconds -> TimeSpan.FromSeconds duration
        | Minutes -> TimeSpan.FromMinutes duration
        | Hours -> TimeSpan.FromHours duration
        | Days -> TimeSpan.FromDays duration

[<RequireQualifiedAccess>]
module Parameters =

    // [<RequireQualifiedAccess>]
    // type Distribution =
    //     | Weibull of alpha: float * beta: float
    //     | LogNormal of mu: float * sigma: float
    //     | Normal of mean: float * stdDev: float
    //     | Uniform of min: float * max: float
    //     | Fixed of float

    [<RequireQualifiedAccess>]
    module DistributionType =

        [<Literal>]
        let weibullStr = "Weibull"
        [<Literal>]
        let logNormalStr = "LogNormal"
        [<Literal>]
        let normalStr = "Normal"
        [<Literal>]
        let uniformStr = "Uniform"
        [<Literal>]
        let fixedStr = "Fixed"

    [<RequireQualifiedAccess>]
    type DistributionType =
        | Weibull
        | LogNormal
        | Normal
        | Uniform
        | Fixed
        override d.ToString () =
            match d with
            | Weibull -> DistributionType.weibullStr
            | LogNormal -> DistributionType.logNormalStr
            | Normal -> DistributionType.normalStr
            | Uniform -> DistributionType.uniformStr
            | Fixed -> DistributionType.fixedStr

        static member parse s =
            match s with
            | DistributionType.weibullStr -> Weibull
            | DistributionType.logNormalStr -> LogNormal
            | DistributionType.normalStr -> Normal
            | DistributionType.uniformStr -> Uniform
            | DistributionType.fixedStr -> Fixed
            | _ -> invalidArg (nameof s) $"Unable to parse {s} as DistributionType"


    [<RequireQualifiedAccess>]
    module ClockType =

        [<Literal>]
        let availableStr = "Available"
        [<Literal>]
        let calendarStr = "Calendar"
        [<Literal>]
        let runningStr = "Running"

    [<RequireQualifiedAccess>]
    type ClockType =
        | Available
        | Calendar
        | Running
        override c.ToString () =
            match c with
            | Available -> ClockType.availableStr
            | Calendar -> ClockType.calendarStr
            | Running -> ClockType.runningStr

        static member parse (s: string) =
            match s with
            | ClockType.availableStr -> ClockType.Available
            | ClockType.calendarStr -> ClockType.Calendar
            | ClockType.runningStr -> ClockType.Running
            | _ -> invalidArg (nameof s) "Unable to parse {s} as ClockType"


    [<RequireQualifiedAccess>]
    module ResetType =

        [<Literal>]
        let cumulativeStr = "Cumulative"
        [<Literal>]
        let recoveryStr = "Recovery"
        [<Literal>]
        let restartStr = "Restart"

    [<RequireQualifiedAccess>]
    type ResetType =
        | Cumulative
        | Recovery
        | Restart
        override r.ToString () =
            match r with
            | Cumulative -> ResetType.cumulativeStr
            | Recovery -> ResetType.recoveryStr
            | Restart -> ResetType.restartStr

        static member parse (s: string) =
            match s with
            | ResetType.cumulativeStr -> Cumulative
            | ResetType.recoveryStr -> Recovery
            | ResetType.restartStr -> Restart
            | _ -> invalidArg (nameof s) $"Cannot parse {s} as ResetType"


    [<RequireQualifiedAccess>]
    module DowntimeType =

        [<Literal>]
        let plannedStr = "Planned"
        [<Literal>]
        let failureStr = "Failure"

    [<RequireQualifiedAccess>]
    type DowntimeType =
        | Planned
        | Failure
        override d.ToString () =
            match d with
            | Planned -> DowntimeType.plannedStr
            | Failure -> DowntimeType.failureStr

        static member parse s =
            match s with
            | DowntimeType.plannedStr -> Planned
            | DowntimeType.failureStr -> Failure
            | _ -> invalidArg (nameof s) $"Cannot parse {s} as DowntimeType"

    [<RequireQualifiedAccess; MessagePackObject>]
    type TimeInterrupt =
        {
            [<Key(0)>]
            Id: int
            [<Key(1)>]
            Name: string
            [<Key(2)>]
            ClockType: ClockType
            [<Key(3)>]
            ResetType: ResetType
            [<Key(4)>]
            DowntimeType: DowntimeType
            [<Key(5)>]
            TimeUntilDistributionType: DistributionType
            [<Key(6)>]
            TimeUntilParam1: string
            [<Key(7)>]
            TimeUntilParam2: string
            [<Key(8)>]
            TimeToRecoverDistributionType: DistributionType
            [<Key(9)>]
            TimeToRecoverParam1: string
            [<Key(10)>]
            TimeToRecoverParam2: string
        }

    [<MessagePackObject>]
    type Buffers =
        {
            [<Key(0)>]
            Level: Dictionary<BufferName, string>
            [<Key(1)>]
            Capacity: Dictionary<BufferName, string>
        }
        static member init () =
            {
                Level = Dictionary()
                Capacity = Dictionary()
            }

        member b.Copy() =
            {
                Level = Dictionary b.Level
                Capacity = Dictionary b.Capacity
            }

    [<MessagePackObject>]
    type Constraints =
        {
            [<Key(0)>]
            Limit: Dictionary<ConstraintName, string>
            [<Key(1)>]
            TimeInterrupts: Dictionary<ConstraintName, ResizeArray<TimeInterrupt>>
        }
        static member init () =
            {
                Limit = Dictionary()
                TimeInterrupts = Dictionary()
            }

        member c.Copy () =
            {
                Limit = Dictionary c.Limit
                TimeInterrupts = Dictionary c.TimeInterrupts
            }

    [<MessagePackObject>]
    type Experiment =
        {
            [<Key(0)>]
            Id: ExperimentId
            [<Key(1)>]
            Seed: string
            [<Key(2)>]
            TimeUnit: TimeUnit
            [<Key(3)>]
            Duration: string
            [<Key(4)>]
            Iterations: string
            [<Key(5)>]
            Name: string
            [<Key(6)>]
            SinkBuffers: string
            [<Key(7)>]
            TheoreticalLimit: string
        }
        static member init initialId =
            {
                Id = initialId
                Name = $"Experiment {initialId}"
                Seed = "123"
                TimeUnit = TimeUnit.Days
                Duration = "1.0"
                Iterations = "1"
                SinkBuffers = ""
                TheoreticalLimit = "1.0"
            }

[<MessagePackObject>]
type Parameters =
    {
        [<Key(0)>]
        Buffers: Parameters.Buffers
        [<Key(1)>]
        Constraints: Parameters.Constraints
    }
    static member init () =
        {
            Buffers = Parameters.Buffers.init()
            Constraints = Parameters.Constraints.init()
        }


[<RequireQualifiedAccess>]
type BufferChange =
    | Level of bufferName: BufferName * newLevel: string
    | Capacity of bufferName: BufferName * newCapacity: string


[<RequireQualifiedAccess>]
type TimeInterruptChange =
    | Name of string
    | ClockType of Parameters.ClockType
    | ResetType of Parameters.ResetType
    | DowntimeType of Parameters.DowntimeType
    | TimeUntilDistributionType of Parameters.DistributionType
    | TimeUntilParam1 of string
    | TimeUntilParam2 of string
    | TimeToRecoverDistributionType of Parameters.DistributionType
    | TimeToRecoverParam1 of string
    | TimeToRecoverParam2 of string

[<RequireQualifiedAccess>]
type ConstraintChange =
    | Limit of constraintName: ConstraintName * newLimit: string
    | TimeInterruptAdded of constraintName: ConstraintName * timeInterrupt: Parameters.TimeInterrupt
    | TimeInterruptChanged of constraintName: ConstraintName * timeInterruptId: int * TimeInterruptChange

[<RequireQualifiedAccess>]
type ExperimentChange =
    | Name of experimentId: ExperimentId * newName: string
    | Seed of experimentId: ExperimentId * newSeed: string
    | Duration of experimentId: ExperimentId * newDuration: string
    | TimeUnit of experimentId: ExperimentId * timeUnit: TimeUnit
    | Iterations of experimentId: ExperimentId * newIterations: string
    | SinkBuffers of experimentId: ExperimentId * newSinkBuffers: string
    | TheoreticalLimit of experimentId: ExperimentId * newTheoreticalLimit: string

[<RequireQualifiedAccess>]
type Cmd =
    | ModelTextChanged of string
    | BufferChanged of BufferChange
    | ConstraintChanged of ConstraintChange
    | ExperimentChanged of ExperimentChange
    | AddExperiment of Parameters.Experiment

[<MessagePackObject>]
type Model =
    {
        [<Key(0)>]
        Text: string
        [<Key(1)>]
        Status: ModelStatus
        [<Key(2)>]
        Parameters: Parameters
        [<Key(3)>]
        Experiments: Map<ExperimentId, Parameters.Experiment>
    }
    static member init () =
        {
            Text = String.Empty
            Status = ModelStatus.None
            Parameters = Parameters.init ()
            Experiments = Map.empty
        }

type EditHistory =
    {
        InitialModel: Model
        CmdHistory: Cmd list
        UndoHistory: Cmd list
    }
    static member init (initialModel: Model) =
        {
            CmdHistory = []
            UndoHistory = []
            InitialModel = initialModel
        }

module FileType =

    [<Literal>]
    let temp = "Temp"
    [<Literal>]
    let user = "User"

[<RequireQualifiedAccess>]
type FileType =
    | Temp
    | User
    static member tryParse (s: string) =
        match s with
        | FileType.temp -> Some Temp
        | FileType.user -> Some User
        | _ ->
            None

    override f.ToString () =
        match f with
        | Temp -> FileType.temp
        | User -> FileType.user

type ValidatedExperiment =
    {
        Seed: int
        TimeUnit: TimeUnit
        Duration: float
        Iterations: int
        SinkBuffers: BufferName[]
        TheoreticalLimit: float
    }

type ValidatedModel =
    {
        Elements: Elements
        AidosMapping: AidosMapping
        Model: int // Dummy value
        Experiment: ValidatedExperiment
    }

[<RequireQualifiedAccess>]
type ModelValidationResult =
    | None
    | Errors of string[]
    | Success of int option

type Session =
    {
        FileType: FileType
        FilePath: string
        ModelValidationResult: ModelValidationResult
    }
    static member init () =
        let tempFilePath = System.IO.Path.GetTempFileName()
        {
            FileType = FileType.Temp
            FilePath = tempFilePath
            ModelValidationResult = ModelValidationResult.None
        }

    static member save (path: string) (session: Session) =
        let saveText = $"{session.FileType.ToString()}{Environment.NewLine}{session.FilePath}"
        System.IO.File.WriteAllText (path, saveText)

[<RequireQualifiedAccess>]
type NewSelection =
    | Buffer of BufferName
    | Constraint of ConstraintName
    | Split of SplitName
    | Merge of MergeName
    | Conveyor of ConveyorName
    | Conversion of ConversionName
    | Experiment of ExperimentId
    | HistoryReport of HistoryReport
    | HistoriesReport of HistoriesReport

type Selection =
    {
        Buffer: BufferName option
        Constraint: ConstraintName option
        Split: SplitName option
        Merge: MergeName option
        Conveyor: ConveyorName option
        Conversion: ConversionName option
        Experiment: ExperimentId option
        HistoryReport: HistoryReport
        HistoriesReport: HistoriesReport
    }
    static member init =
        {
            Buffer = None
            Constraint = None
            Split = None
            Merge = None
            Conveyor = None
            Conversion = None
            Experiment = None
            HistoryReport = HistoryReport.None
            HistoriesReport = HistoriesReport.None
        }

type UI =
    {
        Screen: Screen
        Diagram: DiagramSettings
        Flowchart: FlowchartSettings
        Selection: Selection
    }
    static member init () =
        {
            Screen = Screen.Edit EditScreen.ModelEditor
            Diagram = DiagramSettings.init
            Flowchart = FlowchartSettings.init
            Selection = Selection.init
        }

type State =
    {
        Frame: int
        Session: Session
        UI: UI
        Model: Model
        EditHistory: EditHistory
    }
    static member init (doc: TextDocument) () =
        let session =
            if System.IO.File.Exists "session.txt" then
                let sessionLines = System.IO.File.ReadAllLines "session.txt"
                if sessionLines.Length <> 2 then
                    Session.init()
                else
                    let fileType = sessionLines[0]
                    let filePath = sessionLines[1]

                    match FileType.tryParse fileType, System.IO.File.Exists filePath with
                    | Some fileType, true ->
                        {
                            FileType = fileType
                            FilePath = filePath
                            ModelValidationResult = ModelValidationResult.None
                        }

                    | _, _ ->
                        Session.init()
            else
                Session.init ()

        let modelData = System.IO.File.ReadAllBytes session.FilePath
        let newModel, newSession =
            if modelData.Length > 0 then
                try
                    let resolver =
                        CompositeResolver.Create (
                            FSharpResolver.Instance,
                            StandardResolver.Instance)
                    let options = MessagePackSerializerOptions.Standard.WithResolver resolver
                    let newModel = MessagePackSerializer.Deserialize<Model>(modelData, options)

                    let newModel =
                        if isNull newModel.Parameters.Constraints.TimeInterrupts then
                            let newConstraint =
                                { newModel.Parameters.Constraints with
                                    TimeInterrupts = Dictionary() }
                            let newParameters =
                                { newModel.Parameters with
                                    Constraints = newConstraint }
                            { newModel with
                                Parameters = newParameters }
                        else
                            newModel

                    // let newModel =
                    //     if newModel.Experiments.IsEmpty then
                    //         let newExperiments =
                    //         { newModel with
                    //             Experiments = newExperiments }
                    //     else
                    //         newModel

                    newModel, session
                with
                | ex ->
                    let newSession = Session.init()
                    let newModel = Model.init()
                    newModel, newSession

            else
                Model.init(), session

        let sessionPath = System.IO.Path.Combine (Environment.CurrentDirectory, "session.txt")
        Session.save sessionPath newSession
        doc.Text <- newModel.Text
        {
            Session = newSession
            Frame = 0
            UI = UI.init ()
            Model = newModel
            EditHistory = EditHistory.init newModel
        }


[<RequireQualifiedAccess>]
type Msg =
    | Cmd of cmd: Cmd
    | Move of newPointerPoint: Avalonia.Point
    | ModelTextChanged of editor: TextEditor
    | PanningStarted
    | PanningStopped
    | ZoomIn
    | ZoomOut
    | ScreenChanged of Screen
    | Undo
    | Redo
    | Save
    | SaveAs of filePath: string
    | Open of filePath: string
    | SelectionChanged of NewSelection
    | ModelValidated of ModelValidationResult
