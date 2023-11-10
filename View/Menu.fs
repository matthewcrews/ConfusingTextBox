module Aidos.UI.View.Menu

open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI

let view
    (window: Window)
    (state: State)
    dispatch
    =
    View.createWithKey $"menu_{state.Frame}"
        Menu.create [
            Menu.dock Dock.Top
            Menu.horizontalAlignment HorizontalAlignment.Left
            Menu.viewItems [
                // File menu
                MenuItem.create [
                    MenuItem.header "File"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "New"
                        ]
                        MenuItem.create [
                            MenuItem.header "Open File"
                            MenuItem.onClick (fun _ ->
                                Open.openFile window state.Session dispatch)
                        ]
                        MenuItem.create [
                            MenuItem.header "Save"
                            MenuItem.onClick (fun _ ->
                                match state.Session.FileType with
                                | FileType.Temp ->
                                    Save.saveModelAs window state.Session dispatch
                                | FileType.User ->
                                    dispatch Msg.Save)
                        ]
                        MenuItem.create [
                            MenuItem.header "Save As"
                            MenuItem.onClick (fun _ ->
                                Save.saveModelAs window state.Session dispatch)
                        ]
                        MenuItem.create [
                            MenuItem.header "Close"
                        ]
                    ]
                ]
                // Edit menu
                MenuItem.create [
                    MenuItem.header "Edit"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "Undo"
                            MenuItem.onClick (fun _ ->
                                dispatch Msg.Undo)
                        ]
                        MenuItem.create [
                            MenuItem.header "Redo"
                            MenuItem.onClick (fun _ ->
                                dispatch Msg.Redo)
                        ]
                    ]
                ]
                // View menu
                MenuItem.create [
                    MenuItem.header "View"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "Zoom In"
                        ]
                        MenuItem.create [
                            MenuItem.header "Zoom Out"
                        ]
                    ]
                ]
                // View menu
                MenuItem.create [
                    MenuItem.header "Setup"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "Zoom In"
                        ]
                        MenuItem.create [
                            MenuItem.header "Zoom Out"
                        ]
                    ]
                ]
            ]
        ]
