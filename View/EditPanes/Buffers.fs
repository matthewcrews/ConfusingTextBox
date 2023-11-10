module Aidos.UI.View.EditPanes.Buffers

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout

let view (state: State) dispatch =
    StackPanel.create [
        StackPanel.orientation Orientation.Vertical
        StackPanel.horizontalAlignment HorizontalAlignment.Stretch
        StackPanel.children [
            TextBlock.create [
                TextBlock.text "Buffers"
                TextBlock.fontSize 18.0
            ]
            match state.Model.Status with
            | ModelStatus.None ->
                ()
            | ModelStatus.Error _ ->
                TextBlock.create [
                    TextBlock.text "Please fix network errors on Flow Diagram pane."
                    TextBlock.fontSize 18.0
                ]
            | ModelStatus.Success (model, _) ->
                let settings = state.Model.Parameters.Buffers
                for KeyValue(name, Description desc) in model.Buffers do
                    TextBlock.create [
                        TextBlock.text $"Name: {name}"
                        TextBlock.fontSize 18.0
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.verticalAlignment VerticalAlignment.Center
                        StackPanel.children [
                            TextBlock.create [
                                TextBlock.text $"Description: {desc}"
                                TextBlock.verticalAlignment VerticalAlignment.Center
                            ]
                        ]
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.verticalAlignment VerticalAlignment.Center
                        StackPanel.horizontalAlignment HorizontalAlignment.Stretch
                        StackPanel.children [
                            TextBlock.create [
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.text "Capacity:"
                            ]
                            TextBox.create [
                                TextBox.text $"{settings.Capacity[name]}"
                                TextBox.onTextChanged (fun text ->
                                    dispatch (Msg.Cmd (Cmd.BufferChanged (BufferChange.Capacity (name, text)))))
                            ]
                        ]
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.verticalAlignment VerticalAlignment.Center
                        StackPanel.children [
                            TextBlock.create [
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.text "Level:"
                            ]
                            TextBox.create [
                                TextBox.text $"{settings.Level[name]}"
                                TextBox.onTextChanged (fun text ->
                                    if not (text.Equals settings.Level[name]) then
                                        dispatch (Msg.Cmd (Cmd.BufferChanged (BufferChange.Level (name, text)))))
                            ]
                        ]
                    ]
        ]
    ]
