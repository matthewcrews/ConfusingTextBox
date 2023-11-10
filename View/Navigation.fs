module Aidos.UI.View.Navigation

open Aidos.UI
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL


module private Helpers =

    module Buttons =

        let background = "Transparent"
        let width = 30.0
        let height = 30.0

        let create icon editPane dispatch =
            Button.create [
                Button.content (
                    Image.create [
                        Image.source icon
                        Image.width width
                        Image.height height
                    ])
                Button.background background
                Button.width width
                Button.height height
                Button.onClick (fun _ ->
                    dispatch (Msg.ScreenChanged editPane))
            ]

open Helpers

let view (state: State) dispatch =
    StackPanel.create [
        StackPanel.dock Dock.Left
        StackPanel.orientation Orientation.Vertical
        StackPanel.spacing 16.0
        StackPanel.horizontalAlignment HorizontalAlignment.Center
        StackPanel.margin 8.0
        StackPanel.children [
            Buttons.create Icons.flowDiagramIcon (Screen.Edit EditScreen.ModelEditor) dispatch
            Buttons.create Icons.bufferIcon (Screen.Edit EditScreen.Buffers) dispatch
            Buttons.create Icons.constraintIcon (Screen.Edit EditScreen.Constraints) dispatch
            Buttons.create Icons.splitIcon (Screen.Edit EditScreen.Splits) dispatch
            Buttons.create Icons.mergeIcon (Screen.Edit EditScreen.Merges) dispatch
            Buttons.create Icons.conveyorIcon (Screen.Edit EditScreen.Conveyors) dispatch
            Buttons.create Icons.conversionIcon (Screen.Edit EditScreen.Conversions) dispatch
            Buttons.create Icons.reportsIcon Screen.Reports dispatch
            Buttons.create Icons.experimentsIcon Screen.Experiments dispatch
        ]
    ]
