module Aidos.UI.View.Reports.Histories

open System
open System.Collections.Generic
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout
open Avalonia.Media
open LiveChartsCore.VisualElements
open FSharp.UMX

// module private Helpers =
//
//
//     let OEE
//         ()
//         (elements: Elements)
//         (aidosMapping: AidosMapping)
//         (histories: Aidos.Engine.Components.History[])
//         =
//
//         ()

let view
    (state: State)
    dispatch
    (elements: Elements)
    (aidosMapping: AidosMapping)
    // (histories: Aidos.Engine.Components.History[])
    =
    ()
    //
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
    //                     TextBlock.text "System Reports"
    //                     TextBlock.fontSize 18.0
    //                 ]
    //                 TextBlock.create [
    //                     TextBlock.text "OEE"
    //                     TextBlock.fontSize 14.0
    //                 ]
    //             //     TextBlock.create [
    //             //         TextBlock.text "Constraints Reports"
    //             //         TextBlock.fontSize 18.0
    //             //     ]
    //             //     for KeyValue (name, Description desc) in elements.Constraints do
    //             //         TextBlock.create [
    //             //             TextBlock.text $"{desc} Flow Rates"
    //             //             TextBlock.fontSize 12.0
    //             //             TextBlock.onPointerPressed (fun _ ->
    //             //                 dispatch (Msg.ScreenChanged (Screen.Reports (Report.Constraint (ConstraintReport.FlowRate name)))))
    //             //
    //             //         ]
    //             //     TextBlock.create [
    //             //         TextBlock.text "Buffer Reports"
    //             //         TextBlock.fontSize 18.0
    //             //     ]
    //             //     for KeyValue (name, Description desc) in elements.Buffers do
    //             //         TextBlock.create [
    //             //             TextBlock.text $"{desc} Levels"
    //             //             TextBlock.fontSize 12.0
    //             //             TextBlock.onPointerPressed (fun _ ->
    //             //                 dispatch (Msg.ScreenChanged (Screen.Reports (Report.Buffer (BufferReport.Level name)))))
    //             //
    //             //         ]
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
    //                 match state.UI.Selection.HistoriesReport with
    //                 | HistoriesReport.None ->
    //                     ()
    //                 | HistoriesReport.System systemReport ->
    //                     ()
    //                     // match systemReport with
    //                     // | SystemReport.OEE ->
    //                     //     Helpers.OEE elements aidosMapping histories
    //             ]
    //         ]
    //     ]
    // ]
