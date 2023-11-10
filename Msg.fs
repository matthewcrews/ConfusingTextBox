[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Aidos.UI.Msg

open System
open MessagePack
open MessagePack.FSharp
open MessagePack.Resolvers


let apply (msg: Msg) (state: State) =
    match msg with
    | Msg.Cmd cmd ->
        match state.EditHistory.CmdHistory with
        | [] ->
            let newModel = Cmd.handle state.UI.Diagram state.Model cmd
            let newHistory = { state.EditHistory with CmdHistory = cmd :: state.EditHistory.CmdHistory }
            let newSession = { state.Session with ModelValidationResult = ModelValidationResult.None }
            { state with
                Model = newModel
                EditHistory = newHistory
                Session = newSession
                Frame = state.Frame + 1 }
        | prevCmd :: _ ->
            if cmd <> prevCmd then
                let newModel = Cmd.handle state.UI.Diagram state.Model cmd
                let newHistory = { state.EditHistory with CmdHistory = cmd :: state.EditHistory.CmdHistory }
                let newSession = { state.Session with ModelValidationResult = ModelValidationResult.None }
                { state with
                    Model = newModel
                    EditHistory = newHistory
                    Session = newSession
                    Frame = state.Frame + 1 }
            else
                state

    | Msg.Undo ->
        match state.EditHistory.CmdHistory with
        | [] ->
            state
        | prevCmd :: newCmdHistory ->
            let newModel =
                let replayCmds = newCmdHistory |> List.rev
                let newInitState =
                    state.EditHistory.InitialModel
                (newInitState, replayCmds)
                ||> List.fold (Cmd.handle state.UI.Diagram)

            let newEditHistory =
                { state.EditHistory with
                    CmdHistory = newCmdHistory
                    UndoHistory = prevCmd :: state.EditHistory.UndoHistory }

            { state with
                EditHistory = newEditHistory
                Model = newModel }

    | Msg.Redo ->
        match state.EditHistory.UndoHistory with
        | [] ->
            state
        | prevCmd :: remainingCmds ->
            let newModel = Cmd.handle state.UI.Diagram state.Model prevCmd
            let newEditHistory =
                { state.EditHistory with
                    CmdHistory = prevCmd :: state.EditHistory.CmdHistory
                    UndoHistory = remainingCmds }
            { state with
                Model = newModel
                EditHistory = newEditHistory }

    | Msg.ModelTextChanged editor ->
        let cmd = (Cmd.ModelTextChanged editor.Text)
        let newModel = Cmd.handle state.UI.Diagram state.Model cmd
        let newHistory = { state.EditHistory with CmdHistory = cmd :: state.EditHistory.CmdHistory }
        { state with
            Model = newModel
            EditHistory = newHistory }

    | Msg.ScreenChanged screen ->
        let newUI = { state.UI with Screen = screen }
        { state with
            UI = newUI }

    | Msg.PanningStarted ->
        let ui = state.UI
        let newUI =
            { ui with
                Flowchart = { ui.Flowchart with
                                PointerState = PointerState.Panning }}
        { state with
            UI = newUI }

    | Msg.PanningStopped ->
        let ui = state.UI
        let newUI =
            { ui with
                Flowchart = { ui.Flowchart with
                                        PointerState = PointerState.Neutral }}
        { state with
            UI = newUI }


    | Msg.ZoomIn ->
        let ui = state.UI
        let flowchartView = ui.Flowchart
        let newZoom = Math.Clamp (flowchartView.Zoom * 1.2, 0.5, 10.0)
        let windowLocationDelta = flowchartView.Origin - flowchartView.PointerPosition
        let newWindowLocationPointerOffset = (newZoom / flowchartView.Zoom) * windowLocationDelta
        let newWindowPosition = flowchartView.PointerPosition + newWindowLocationPointerOffset
        let newUI =
            { ui with
                Flowchart = { ui.Flowchart with
                                Zoom = newZoom
                                Origin = newWindowPosition }}

        { state with
            UI = newUI }

    | Msg.ZoomOut ->
        let ui = state.UI
        let flowchartView = ui.Flowchart
        let newZoom = Math.Clamp (flowchartView.Zoom * 0.8, 0.5, 10.0)
        let windowLocationDelta = flowchartView.Origin - flowchartView.PointerPosition
        let newWindowLocationPointerOffset = (newZoom / flowchartView.Zoom) * windowLocationDelta
        let newWindowPosition = flowchartView.PointerPosition + newWindowLocationPointerOffset
        let newUI =
            { ui with
                Flowchart = { ui.Flowchart with
                                Zoom = newZoom
                                Origin = newWindowPosition }}

        { state with
            UI = newUI }

    | Msg.Move newPointerPoint ->
        let ui = state.UI
        let flowchartView = ui.Flowchart

        match flowchartView.PointerState with
        | PointerState.Panning ->
            let ui = state.UI
            let delta = newPointerPoint - flowchartView.PointerPosition
            let newUI =
                { ui with
                    Flowchart = { ui.Flowchart with
                                    PointerPosition = newPointerPoint
                                    Origin = flowchartView.Origin + delta }}

            { state with
                UI = newUI }

        | _ ->
            let ui = state.UI
            let newUI =
                { ui with
                    Flowchart = { ui.Flowchart with
                                    PointerPosition = newPointerPoint }}

            { state with
                UI = newUI }

    | Msg.Save ->
        let resolver =
            CompositeResolver.Create (
                FSharpResolver.Instance,
                StandardResolver.Instance)
        let options = MessagePackSerializerOptions.Standard.WithResolver resolver

        let modelData = MessagePackSerializer.Serialize (state.Model, options)
        System.IO.File.WriteAllBytes (state.Session.FilePath, modelData)
        state

    | Msg.SaveAs filePath ->
        let newSession =
            { state.Session with
                FilePath = filePath
                FileType = FileType.User }
        let sessionPath = System.IO.Path.Combine (Environment.CurrentDirectory, "session.txt")
        Session.save sessionPath newSession

        let resolver =
            CompositeResolver.Create (
                FSharpResolver.Instance,
                StandardResolver.Instance)
        let options = MessagePackSerializerOptions.Standard.WithResolver resolver

        let modelData = MessagePackSerializer.Serialize (state.Model, options)
        System.IO.File.WriteAllBytes (newSession.FilePath, modelData)

        { state with
            Session = newSession }

    | Msg.Open filePath ->
        let newSession =
            { state.Session with
                FilePath = filePath
                FileType = FileType.User }
        let sessionPath = System.IO.Path.Combine (Environment.CurrentDirectory, "session.txt")
        Session.save sessionPath newSession

        let resolver =
            CompositeResolver.Create (
                FSharpResolver.Instance,
                StandardResolver.Instance)
        let options = MessagePackSerializerOptions.Standard.WithResolver resolver
        let modelData = System.IO.File.ReadAllBytes newSession.FilePath
        let newModel = MessagePackSerializer.Deserialize<Model>(modelData, options)

        {
            Session = newSession
            Frame = state.Frame + 1
            UI = UI.init ()
            Model = newModel
            EditHistory = EditHistory.init newModel
        }

    | Msg.SelectionChanged selectionUpdate ->
        let oldSelection = state.UI.Selection
        let newSelection =
            match selectionUpdate with
            | NewSelection.Buffer s -> { oldSelection with Buffer = Some s }
            | NewSelection.Constraint s -> { oldSelection with Constraint =  Some s }
            | NewSelection.Split s -> { oldSelection with Split = Some s }
            | NewSelection.Merge s -> { oldSelection with Merge = Some s }
            | NewSelection.Conveyor s -> { oldSelection with Conveyor =  Some s }
            | NewSelection.Conversion s -> { oldSelection with Conversion =  Some s }
            | NewSelection.Experiment i -> { oldSelection with Experiment =  Some i }
            | NewSelection.HistoryReport historyReport -> { oldSelection with HistoryReport = historyReport }
            | NewSelection.HistoriesReport historiesReport -> { oldSelection with HistoriesReport = historiesReport }

        let newUI =
            { state.UI with
                Selection = newSelection }
        { state with
            UI = newUI
            Frame = state.Frame + 1 }

    | Msg.ModelValidated modelValidationResult ->
        let newSession =
            { state.Session with
                ModelValidationResult = modelValidationResult }
        { state with
            Session = newSession }
