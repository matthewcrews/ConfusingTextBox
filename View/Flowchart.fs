module Aidos.UI.Flowchart

open Aidos.UI.View
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Shapes
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Input
open Avalonia.Layout
open FSharp.UMX

module private Helpers =

    let drawNode
        (flowchartSettings: FlowchartSettings)
        (diagramSettings: DiagramSettings)
        (node: Node)
        (location: Location)
        =
        let point = Point (location.X, location.Y)
        let zoom = flowchartSettings.Zoom
        let origin = flowchartSettings.Origin

        // Map the zoom and pan settings
        let nodeWidth = zoom * diagramSettings.NodeWidth
        let nodeHeight = zoom * diagramSettings.NodeHeight
        let newPoint = zoom * point + origin

        // TODO: Have this map to the image to render
        let name, icon =
            match node with
            | Node.Buffer b -> %b, Icons.bufferIcon
            | Node.Constraint c -> %c, Icons.constraintIcon
            | Node.Split s -> %s, Icons.splitIcon
            | Node.Merge m -> %m, Icons.mergeIcon
            | Node.Conveyor c -> %c, Icons.conveyorIcon
            | Node.Conversion c -> %c, Icons.conveyorIcon

        StackPanel.create [
            StackPanel.top newPoint.Y
            StackPanel.left newPoint.X
            StackPanel.orientation Orientation.Vertical
            StackPanel.children [
                Image.create [
                    Image.horizontalAlignment HorizontalAlignment.Center
                    Image.source icon
                    Image.height nodeHeight
                    Image.width nodeWidth
                ]
                TextBlock.create [
                    TextBlock.text name
                    TextBlock.fontSize (zoom * diagramSettings.FontSize)
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                ]
            ]
        ]

        // Button.create [
        //     Button.top newPoint.Y
        //     Button.left newPoint.X
        //     Button.width nodeWidth
        //     Button.height nodeHeight
        //     Button.verticalContentAlignment VerticalAlignment.Center
        //     Button.horizontalContentAlignment HorizontalAlignment.Center
        //     Button.content name
        // ]

    // let drawEdge
    //     (diagramSettings: DiagramSettings)
    //     (points: Point[])
    //     =
    //
    //     for startPoint, endPoint in Array.pairwise points do
    //         Line.create [
    //             Line.stroke diagramSettings.EdgeStroke
    //             Line.width diagramSettings.EdgeWidth
    //             Line.startPoint startPoint
    //             Line.endPoint endPoint
    //         ]
    //

open Helpers

let view
    (state: State)
    dispatch
    =

    let canvasName = "flowchart"
    let mutable canvas = Unchecked.defaultof<_>

    Canvas.create [
        Grid.row 0
        Grid.column 2
        Canvas.name canvasName
        Canvas.init (fun newCanvas ->
            canvas <- newCanvas)
        Canvas.background "White"
        Canvas.dock Dock.Left
        Canvas.clipToBounds true
        Canvas.onPointerMoved (fun e ->
            let source : Control = e.Source :?> Control
            if source.Name = canvasName then
                e.Handled <- true
                let newPointerLocation = e.GetPosition canvas
                Msg.Move newPointerLocation |> dispatch)

        Canvas.onPointerReleased (fun e ->
            if e.InitialPressMouseButton = MouseButton.Middle then
                e.Handled <- true
                Msg.PanningStopped |> dispatch)

        Canvas.onPointerPressed (fun e ->
            let point = e.GetCurrentPoint canvas
            if point.Properties.IsMiddleButtonPressed then
                e.Handled <- true
                Msg.PanningStarted |> dispatch)

        Canvas.onPointerWheelChanged (fun e ->
            e.Handled <- true
            let zoomDelta = e.Delta.Y
            if zoomDelta > 0 then
                Msg.ZoomIn |> dispatch
            elif zoomDelta < 0 then
                Msg.ZoomOut |> dispatch)


        Canvas.children [
            match state.Model.Status with
            | ModelStatus.None -> ()
            | ModelStatus.Error err ->
                TextBlock.create [
                    TextBlock.text err
                ]

            | ModelStatus.Success (model, layout) ->
                for KeyValue (node, point) in layout.SitePoint do
                    drawNode state.UI.Flowchart state.UI.Diagram node point

                for KeyValue (_, points) in layout.EdgePoints do
                    let zoom = state.UI.Flowchart.Zoom
                    let origin = state.UI.Flowchart.Origin

                    for sourceLocation, targetLocation in Array.pairwise points do
                        let startPoint = Point (sourceLocation.X, sourceLocation.Y)
                        let endPoint = Point (targetLocation.X, targetLocation.Y)
                        let newStartPoint = startPoint * zoom + origin
                        let newEndPoint = endPoint * zoom + origin
                        Line.create [
                            Line.stroke state.UI.Diagram.EdgeStroke
                            Line.width (zoom * state.UI.Diagram.EdgeWidth)
                            Line.startPoint newStartPoint
                            Line.endPoint newEndPoint
                        ]

        ]
    ]
