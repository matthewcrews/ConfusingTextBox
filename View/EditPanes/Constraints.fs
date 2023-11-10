module Aidos.UI.View.EditPanes.Constraints

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout

let view (state: State) dispatch =
    StackPanel.create [
        StackPanel.orientation Orientation.Horizontal
        StackPanel.children [
            StackPanel.create [
                StackPanel.orientation Orientation.Vertical
                StackPanel.children [
                    TextBlock.create [
                        TextBlock.text "Constraints"
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
                        let parameters = state.Model.Parameters.Constraints
                        StackPanel.create [
                            StackPanel.orientation Orientation.Vertical
                            StackPanel.children [
                                for KeyValue(constraintName, Description desc) in model.Constraints do
                                    TextBlock.create [
                                        TextBlock.text $"{constraintName}"
                                        TextBlock.fontSize 18.0
                                        TextBlock.onPointerPressed (fun _ ->
                                            dispatch (Msg.SelectionChanged (NewSelection.Constraint constraintName)))
                                    ]
                            ]
                        ]
                ]
            ]
            match state.UI.Selection.Constraint with
            | None -> ()
            | Some constraintName ->
                let limit = state.Model.Parameters.Constraints.Limit[constraintName]
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children [
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text "Limit:"
                                ]
                                TextBox.create [
                                    TextBox.verticalAlignment VerticalAlignment.Center
                                    TextBox.text $"{limit}"
                                    TextBox.onTextChanged (fun text ->
                                        dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.Limit (constraintName, text)))))
                                ]

                            ]
                        ]
                        TextBlock.create [
                            TextBlock.text "Interrupts"
                        ]
                        match state.Model.Parameters.Constraints.TimeInterrupts.TryGetValue constraintName with
                        | true, timeInterrupts ->
                            for ti in timeInterrupts do
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Name:"
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                        TextBox.create [
                                            TextBox.text ti.Name
                                            TextBox.onTextChanged (fun t ->
                                                dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.Name t)))))
                                        ]
                                    ]
                                ]
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Clock Type:"
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                        ComboBox.create [
                                            ComboBox.dataItems [
                                                Parameters.ClockType.availableStr
                                                Parameters.ClockType.calendarStr
                                                Parameters.ClockType.runningStr
                                            ]
                                            ComboBox.selectedItem (ti.ClockType.ToString())
                                            ComboBox.onSelectionChanged (fun e ->
                                                if e.AddedItems.Count = 1 then
                                                    let newClockType = e.AddedItems[0] :?> string |> Parameters.ClockType.parse
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.ClockType newClockType)))))
                                        ]
                                    ]
                                ]
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Reset Type:"
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                        ComboBox.create [
                                            ComboBox.dataItems [
                                                Parameters.ResetType.cumulativeStr
                                                Parameters.ResetType.recoveryStr
                                                Parameters.ResetType.restartStr
                                            ]
                                            ComboBox.selectedItem (ti.ResetType.ToString())
                                            ComboBox.onSelectionChanged (fun e ->
                                                if e.AddedItems.Count = 1 then
                                                    let newResetType = e.AddedItems[0] :?> string |> Parameters.ResetType.parse
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.ResetType newResetType)))))
                                        ]
                                    ]
                                ]
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Downtime Type:"
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                        ComboBox.create [
                                            ComboBox.dataItems [
                                                Parameters.DowntimeType.failureStr
                                                Parameters.DowntimeType.plannedStr
                                            ]
                                            ComboBox.selectedItem (ti.DowntimeType.ToString())
                                            ComboBox.onSelectionChanged (fun e ->
                                                if e.AddedItems.Count = 1 then
                                                    let newDowntimeType = e.AddedItems[0] :?> string |> Parameters.DowntimeType.parse
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.DowntimeType newDowntimeType)))))
                                        ]
                                    ]
                                ]
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Time Until:"
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                        ]
                                        ComboBox.create [
                                            ComboBox.dataItems [
                                                Parameters.DistributionType.weibullStr
                                                Parameters.DistributionType.logNormalStr
                                                Parameters.DistributionType.normalStr
                                                Parameters.DistributionType.uniformStr
                                                Parameters.DistributionType.fixedStr
                                            ]
                                            ComboBox.selectedItem (ti.TimeUntilDistributionType.ToString())
                                            ComboBox.onSelectionChanged (fun e ->
                                                e.Handled <- true
                                                if e.AddedItems.Count = 1 then
                                                    let newDistributionType = e.AddedItems[0] :?> string |> Parameters.DistributionType.parse
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilDistributionType newDistributionType)))))
                                        ]

                                        match ti.TimeUntilDistributionType with
                                        | Parameters.DistributionType.Weibull ->
                                            TextBlock.create [
                                                TextBlock.text "Scale:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam1
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam1 v)))))
                                            ]
                                            TextBlock.create [
                                                TextBlock.text "Shape:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam2
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam2 v)))))
                                            ]

                                        | Parameters.DistributionType.LogNormal ->
                                            TextBlock.create [
                                                TextBlock.text "Mu:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam1
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam1 v)))))
                                            ]
                                            TextBlock.create [
                                                TextBlock.text "Sigma:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam2
                                                TextBox.onTextChanged (fun v ->
                                                    let newTimeInterrupt =
                                                        { ti with
                                                            TimeUntilParam2 = v }
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam2 v)))))
                                            ]

                                        | Parameters.DistributionType.Normal ->
                                            TextBlock.create [
                                                TextBlock.text "Mean:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam1
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam1 v)))))
                                            ]
                                            TextBlock.create [
                                                TextBlock.text "StdDev:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam2
                                                TextBox.onTextChanged (fun v ->
                                                    let newTimeInterrupt =
                                                        { ti with
                                                            TimeUntilParam2 = v }
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam2 v)))))
                                            ]

                                        |  Parameters.DistributionType.Uniform ->
                                            TextBlock.create [
                                                TextBlock.text "Min:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam1
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam1 v)))))
                                            ]
                                            TextBlock.create [
                                                TextBlock.text "Max:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam2
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam2 v)))))
                                            ]
                                        | Parameters.DistributionType.Fixed ->
                                            TextBlock.create [
                                                TextBlock.text "Interval:"
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            TextBox.create [
                                                TextBox.text ti.TimeUntilParam1
                                                TextBox.onTextChanged (fun v ->
                                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeUntilParam1 v)))))
                                            ]
                                        ]
                                ]
                                StackPanel.create [
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        TextBlock.create [
                                            TextBlock.text "Time to Recover:"
                                        ]
                                        StackPanel.create [
                                            StackPanel.orientation Orientation.Horizontal
                                            StackPanel.children [
                                                ComboBox.create [
                                                    ComboBox.dataItems [
                                                        Parameters.DistributionType.weibullStr
                                                        Parameters.DistributionType.logNormalStr
                                                        Parameters.DistributionType.normalStr
                                                        Parameters.DistributionType.uniformStr
                                                        Parameters.DistributionType.fixedStr
                                                    ]
                                                    ComboBox.selectedItem (ti.TimeToRecoverDistributionType.ToString())
                                                    ComboBox.onSelectionChanged (fun e ->
                                                        e.Handled <- true
                                                        if e.AddedItems.Count = 1 then
                                                            let newDistributionType = e.AddedItems[0] :?> string |> Parameters.DistributionType.parse
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverDistributionType newDistributionType)))))
                                                ]

                                                match ti.TimeToRecoverDistributionType with
                                                | Parameters.DistributionType.Weibull ->
                                                    TextBlock.create [
                                                        TextBlock.text "Scale:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam1
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam1 v)))))
                                                    ]
                                                    TextBlock.create [
                                                        TextBlock.text "Shape:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam2
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam2 v)))))
                                                    ]

                                                | Parameters.DistributionType.LogNormal ->
                                                    TextBlock.create [
                                                        TextBlock.text "Mu:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam1
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam1 v)))))
                                                    ]
                                                    TextBlock.create [
                                                        TextBlock.text "Sigma:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam2
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam2 v)))))
                                                    ]

                                                | Parameters.DistributionType.Normal ->
                                                    TextBlock.create [
                                                        TextBlock.text "Mean:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam1
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam1 v)))))
                                                    ]
                                                    TextBlock.create [
                                                        TextBlock.text "StdDev:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam2
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam2 v)))))
                                                    ]

                                                |  Parameters.DistributionType.Uniform ->
                                                    TextBlock.create [
                                                        TextBlock.text "Min:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam1
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam1 v)))))
                                                    ]
                                                    TextBlock.create [
                                                        TextBlock.text "Max:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam2
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam2 v)))))
                                                    ]
                                                | Parameters.DistributionType.Fixed ->
                                                    TextBlock.create [
                                                        TextBlock.text "Interval:"
                                                        TextBlock.verticalAlignment VerticalAlignment.Center
                                                    ]
                                                    TextBox.create [
                                                        TextBox.text ti.TimeToRecoverParam1
                                                        TextBox.onTextChanged (fun v ->
                                                            dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptChanged (constraintName, ti.Id, TimeInterruptChange.TimeToRecoverParam1 v)))))
                                                    ]
                                            ]
                                        ]
                                    ]
                                ]
                            Button.create [
                                Button.content "+"
                                Button.onClick (fun _ ->
                                    let newId =
                                        timeInterrupts
                                        |> Seq.maxBy (fun ti -> ti.Id)
                                        |> fun ti -> ti.Id + 1
                                    let newName = $"{constraintName} Interrupt {newId}"
                                    let newTimeInterrupt : Parameters.TimeInterrupt = {
                                        Id = newId
                                        Name = newName
                                        ClockType = Parameters.ClockType.Calendar
                                        ResetType = Parameters.ResetType.Recovery
                                        DowntimeType = Parameters.DowntimeType.Failure
                                        TimeUntilDistributionType = Parameters.DistributionType.Weibull
                                        TimeUntilParam1 = "0.7"
                                        TimeUntilParam2 = "1.0"
                                        TimeToRecoverDistributionType = Parameters.DistributionType.Weibull
                                        TimeToRecoverParam1 = "0.7"
                                        TimeToRecoverParam2 = "1.0"
                                    }
                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptAdded (constraintName, newTimeInterrupt)))))
                            ]
                        | false, _ ->
                            Button.create [
                                Button.content "+"
                                Button.onClick (fun _ ->
                                    let newId = 1
                                    let newName = $"{constraintName} Interrupt {newId}"
                                    let newTimeInterrupt : Parameters.TimeInterrupt = {
                                        Id = newId
                                        Name = newName
                                        ClockType = Parameters.ClockType.Calendar
                                        ResetType = Parameters.ResetType.Recovery
                                        DowntimeType = Parameters.DowntimeType.Failure
                                        TimeUntilDistributionType = Parameters.DistributionType.Weibull
                                        TimeUntilParam1 = "0.7"
                                        TimeUntilParam2 = "1.0"
                                        TimeToRecoverDistributionType = Parameters.DistributionType.Weibull
                                        TimeToRecoverParam1 = "0.7"
                                        TimeToRecoverParam2 = "1.0"
                                    }
                                    dispatch (Msg.Cmd (Cmd.ConstraintChanged (ConstraintChange.TimeInterruptAdded (constraintName, newTimeInterrupt)))))
                            ]
                    ]
                ]
        ]
    ]
