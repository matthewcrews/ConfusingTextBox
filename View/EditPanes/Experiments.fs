module Aidos.UI.View.EditPanes.Experiments

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout

let view (state: State) dispatch =
    StackPanel.create [
        StackPanel.orientation Orientation.Horizontal
        StackPanel.children [
            // Experiment Selection Column
            StackPanel.create [
                StackPanel.orientation Orientation.Vertical
                StackPanel.children [
                for KeyValue (experimentId, experiment) in state.Model.Experiments do
                    TextBlock.create [
                        TextBlock.text experiment.Name
                        TextBlock.onPointerPressed ((fun e ->
                            e.Handled <- true
                            dispatch (Msg.SelectionChanged (NewSelection.Experiment experimentId)))
                            , SubPatchOptions.Always)
                    ]
                Button.create [
                    Button.content "+"
                    Button.onClick (fun e ->
                        let maxExperimentId =
                            if state.Model.Experiments.Count > 0 then
                                state.Model.Experiments.Keys
                                |> Seq.max
                            else
                                0<_>
                        let newExperimentId = maxExperimentId + 1<_>
                        let newExperimentName = $"Experiment {newExperimentId}"
                        let newExperiment : Parameters.Experiment = {
                            Id = newExperimentId
                            Name = newExperimentName
                            Seed = "123"
                            Iterations = "1"
                            Duration = "1.0"
                            TimeUnit = TimeUnit.Days
                            SinkBuffers = ""
                            TheoreticalLimit = "1.0"
                        }
                        e.Handled <- true
                        dispatch (Msg.Cmd (Cmd.AddExperiment newExperiment)))
                ]
                ]
            ]
            StackPanel.create [
                StackPanel.orientation Orientation.Vertical
                StackPanel.children [
                    match state.UI.Selection.Experiment with
                    | None ->
                        ()
                    | Some experimentId ->
                        let experiment =
                            state.Model.Experiments[experimentId]

                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Name:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.Name
                                    TextBox.onTextChanged (fun newName ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.Name (experimentId, newName)))))
                                ]
                            ]
                        ]

                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Seed:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.Seed
                                    TextBox.onTextChanged (fun newSeed ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.Seed (experimentId, newSeed)))))
                                ]
                            ]
                        ]

                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Time Units:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                ComboBox.create [
                                    ComboBox.dataItems [
                                        TimeUnit.seconds
                                        TimeUnit.minutes
                                        TimeUnit.hours
                                        TimeUnit.days
                                        TimeUnit.weeks
                                    ]
                                    ComboBox.selectedItem (experiment.TimeUnit.ToString())
                                    ComboBox.onSelectionChanged (fun e ->
                                        if e.AddedItems.Count = 1 then
                                            let newTimeUnit =
                                                e.AddedItems[0]
                                                :?> string
                                                |> TimeUnit.ofString
                                            dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.TimeUnit (experimentId, newTimeUnit)))))
                                ]
                            ]
                        ]
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Duration:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.Duration
                                    TextBox.onTextChanged (fun newDuration ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.Duration (experimentId, newDuration)))))
                                ]
                            ]
                        ]
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Iterations:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.Iterations
                                    TextBox.onTextChanged (fun newIterations ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.Iterations (experimentId, newIterations)))))
                                ]
                            ]
                        ]
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Sink Buffers:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.SinkBuffers
                                    TextBox.onTextChanged (fun newSinkBuffers ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.SinkBuffers (experimentId, newSinkBuffers)))))
                                ]
                            ]
                        ]
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Theoretical Limit:"
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBox.create [
                                    TextBox.text experiment.TheoreticalLimit
                                    TextBox.onTextChanged (fun newTheoreticalLimit ->
                                        dispatch (Msg.Cmd (Cmd.ExperimentChanged (ExperimentChange.TheoreticalLimit (experimentId, newTheoreticalLimit)))))
                                ]
                            ]
                        ]
                ]
            ]

        ]
    ]
