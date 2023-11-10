module Aidos.UI.View.Screen

open System.Reactive.Subjects
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Input
open AvaloniaEdit
open Aidos.UI
open AvaloniaEdit.Document

let view
    (window: Window)
    (editorChangeSubject: Subject<_>)
    (doc: TextDocument)
    (state: State)
    dispatch
    =

    let mutable editor = Unchecked.defaultof<TextEditor>

    DockPanel.create [
        DockPanel.children [
            Menu.view window state dispatch
            Navigation.view state dispatch
            match state.UI.Screen with
            | Screen.Edit editScreen ->
                Grid.create [
                    Grid.dock Dock.Left
                    Grid.columnDefinitions "*, 4, *"
                    Grid.rowDefinitions "*"
                    Grid.children [
                        match editScreen with
                        | EditScreen.ModelEditor ->
                            TextEditor.create [
                                TextEditor.init (fun newEditor ->
                                    editor <- newEditor)
                                Grid.column 0
                                Grid.row 0
                                TextEditor.document doc
                                TextEditor.dock Dock.Left
                                TextEditor.fontFamily "Consolas"
                                TextEditor.showLineNumbers true
                                TextEditor.onTextInput (fun e ->
                                    editorChangeSubject.OnNext editor)
                                TextEditor.onKeyUp (fun e ->
                                    if e.Key = Key.V && e.KeyModifiers = KeyModifiers.Control then
                                        e.Handled <- true
                                        editorChangeSubject.OnNext editor)
                            ]

                        | EditScreen.Buffers ->
                            EditPanes.Buffers.view state dispatch

                        | EditScreen.Constraints ->
                            EditPanes.Constraints.view state dispatch

                        | EditScreen.Splits ->
                            EditPanes.Splits.view state dispatch

                        | EditScreen.Merges ->
                            EditPanes.Merges.view state dispatch

                        | EditScreen.Conveyors ->
                            EditPanes.Conveyors.view state dispatch

                        | EditScreen.Conversions ->
                            EditPanes.Conversions.view state dispatch

                        GridSplitter.create [
                            Grid.column 1
                            GridSplitter.background "DarkGray"
                            GridSplitter.resizeDirection GridResizeDirection.Columns
                        ]
                        Flowchart.view state dispatch
                    ]
                ]

            | Screen.Experiments ->
                EditPanes.Experiments.view state dispatch

            | Screen.Reports ->
                Reporting.view state dispatch

        ]
    ]
