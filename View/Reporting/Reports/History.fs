module Aidos.UI.View.Reports.History

open System
open System.Collections.Generic
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout
open Avalonia.Media
open LiveChartsCore.VisualElements
open FSharp.UMX
open LiveChartsCore.SkiaSharpView
open SkiaSharp
open LiveChartsCore.SkiaSharpView.Avalonia
open LiveChartsCore.Measure
open LiveChartsCore.SkiaSharpView.Painting
open LiveChartsCore.SkiaSharpView.VisualElements
//
// module private Helpers =
//
//     let timeAxisLabeler value =
//         let v = int64 value
//         let ts = TimeSpan v
//         ts.ToString(@"dd\.hh\:mm\:ss")
//
//     module Buffers =
//
//         let levels
//             (history: Aidos.Engine.Components.History)
//             (aidosMapping: AidosMapping)
//             bufferName
//             =
//             let aidosName = aidosMapping.Buffers[bufferName]
//             let nodeName = Aidos.Modeling.Name.Node.Buffer aidosName
//
//             let levels =
//                 history.Levels aidosName
//                 |> Seq.map (fun (struct (time, level)) ->
//                     let time = TimeSpan (int64 time)
//                     time, level)
//
//             let flowRates =
//                 history.FlowRate nodeName
//                 |> Seq.map (fun (struct (time, flowRate)) ->
//                     let time = TimeSpan (int64 time)
//                     time, flowRate)
//
//             CartesianChart.create [
//                 CartesianChart.title (
//                     LabelVisual(
//                         Text = $"Buffer {bufferName} Level and Outlet Rate",
//                         TextSize = 20.0
//                     ))
//                 CartesianChart.legendPosition LegendPosition.Right
//                 CartesianChart.series [
//                     StepLineSeries<_>(
//                         Values = flowRates,
//                         ScalesYAt = 1,
//                         GeometrySize = 0,
//                         Name = "Outlet Rate",
//                         Fill = null
//                         // Stroke = new SolidColorPaint(StrokeThickness = 1f)
//                     )
//                     LineSeries<_>(
//                         Values = levels,
//                         ScalesYAt = 0,
//                         GeometrySize = 0,
//                         Name = "Level",
//                         LineSmoothness = 0.0,
//                         Fill = null)
//                 ]
//                 CartesianChart.xAxes [
//                     Axis(
//                         Labeler = timeAxisLabeler,
//                         Position = AxisPosition.Start
//                     )
//                 ]
//                 CartesianChart.yAxes [
//                     Axis(
//                         Name = "Level",
//                         Position = AxisPosition.Start)
//                     Axis(
//                         Name = "Flow Rate",
//                         ShowSeparatorLines = false,
//                         Position = AxisPosition.End)
//                 ]
//             ]
//
//
//     module Constraints =
//
//         let flowRates
//             (history: Aidos.Engine.Components.History)
//             (aidosMapping: AidosMapping)
//             constraintName
//             =
//             let aidosName = aidosMapping.Constraints[constraintName]
//             let nodeName = Aidos.Modeling.Name.Node.Constraint aidosName
//
//             let flowRates =
//                 history.FlowRate nodeName
//                 |> Seq.map (fun (struct (time, flowRate)) ->
//                     let time = TimeSpan (int64 time)
//                     time, flowRate)
//
//             let limits =
//                 history.Limit aidosName
//                 |> Seq.map (fun (struct (time, limit)) ->
//                     let time = TimeSpan (int64 time)
//                     time, limit)
//
//             CartesianChart.create [
//                 Grid.row 0
//                 Grid.column 0
//                 CartesianChart.title (
//                     LabelVisual(
//                         Text = $"Constraint {constraintName}",
//                         TextSize = 20.0,
//                         Paint = new SolidColorPaint(SKColors.Gray)
//                     ))
//                 CartesianChart.legendPosition LegendPosition.Right
//                 CartesianChart.series [
//                     StepLineSeries<_>(
//                         Values = limits,
//                         ScalesYAt = 0,
//                         GeometrySize = 0,
//                         Name = "Limit",
//                         Fill = null
//                     )
//                     StepLineSeries<_>(
//                         Values = flowRates,
//                         ScalesYAt = 0,
//                         GeometrySize = 0,
//                         Name = "Outlet Rate",
//                         Fill = null
//                     )
//                 ]
//                 CartesianChart.xAxes [
//                     Axis(
//                         Name = "Time",
//                         Labeler = timeAxisLabeler,
//                         Position = AxisPosition.Start
//                     )
//                 ]
//                 CartesianChart.yAxes [
//                     Axis(
//                         Name = "Flow Rate",
//                         Position = AxisPosition.Start)
//                 ]
//                 CartesianChart.legendPosition LegendPosition.Right
//             ]



// open Helpers

let view
    (state: State)
    dispatch
    (elements: Elements)
    (aidosMapping: AidosMapping)
    // (history: Aidos.Engine.Components.History)
    =
    ()
    // DockPanel.create [
    //     DockPanel.children [
    //
    //         // Report Selection
    //         StackPanel.create [
    //             StackPanel.dock Dock.Left
    //             StackPanel.orientation Orientation.Vertical
    //             StackPanel.children [
    //                 TextBlock.create [
    //                     TextBlock.text "History Reports"
    //                     TextBlock.fontSize 24.0
    //                 ]
    //                 TextBlock.create [
    //                     TextBlock.text "Constraints Reports"
    //                     TextBlock.fontSize 18.0
    //                 ]
    //                 for KeyValue (name, Description desc) in elements.Constraints do
    //                     TextBlock.create [
    //                         TextBlock.text $"{desc} Flow Rates"
    //                         TextBlock.fontSize 12.0
    //                         TextBlock.onPointerPressed (fun _ ->
    //                             dispatch (Msg.SelectionChanged (NewSelection.HistoryReport  (HistoryReport.Constraint (ConstraintReport.FlowRate name)))))
    //
    //                     ]
    //                 TextBlock.create [
    //                     TextBlock.text "Buffer Reports"
    //                     TextBlock.fontSize 18.0
    //                 ]
    //                 for KeyValue (name, Description desc) in elements.Buffers do
    //                     TextBlock.create [
    //                         TextBlock.text $"{desc} Levels"
    //                         TextBlock.fontSize 12.0
    //                         TextBlock.onPointerPressed (fun _ ->
    //                             dispatch (Msg.SelectionChanged (NewSelection.HistoryReport (HistoryReport.Buffer (BufferReport.Level name)))))
    //
    //                     ]
    //             ]
    //         ]
    //
    //         // Report Viewing
    //         Grid.create [
    //             Grid.dock Dock.Left
    //             Grid.horizontalAlignment HorizontalAlignment.Stretch
    //             Grid.verticalAlignment VerticalAlignment.Stretch
    //             Grid.columnDefinitions "*"
    //             Grid.rowDefinitions "*"
    //             Grid.children [
    //                 match state.UI.Selection.HistoryReport with
    //                 | HistoryReport.None ->
    //                     ()
    //                 | HistoryReport.Buffer bufferReport ->
    //                     match bufferReport with
    //                     | BufferReport.Level bufferName ->
    //                         Buffers.levels history aidosMapping bufferName
    //
    //                 | HistoryReport.Constraint constraintReport ->
    //                     match constraintReport with
    //                     | ConstraintReport.FlowRate constraintName ->
    //                         Constraints.flowRates history aidosMapping constraintName
    //             ]
    //         ]
    //     ]
    // ]
