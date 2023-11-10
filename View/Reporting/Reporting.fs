module Aidos.UI.View.Reporting

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI
open Avalonia.Layout

let view
    (state: State)
    dispatch
    =
    ()
    //
    // DockPanel.create [
    //     DockPanel.verticalAlignment VerticalAlignment.Stretch
    //     DockPanel.horizontalAlignment HorizontalAlignment.Stretch
    //     DockPanel.children [
    //
    //         match state.Model.Status with
    //         | ModelStatus.None ->
    //             TextBox.create [
    //                 TextBox.text "Please create model before trying to view reports."
    //             ]
    //         | ModelStatus.Error error ->
    //             TextBox.create [
    //                 TextBox.text "Please fix the following error."
    //             ]
    //             TextBox.create [
    //                 TextBox.text error
    //             ]
    //         | ModelStatus.Success (elements, _) ->
    //
    //             match state.Session.ModelValidationResult with
    //             | ModelValidationResult.None ->
    //                 match state.UI.Selection.Experiment with
    //                 | None ->
    //                     StackPanel.create [
    //                         StackPanel.orientation Orientation.Vertical
    //                         StackPanel.children [
    //                             TextBlock.create [
    //                                 TextBlock.text "Please select the experiment you would like to run."
    //                                 TextBlock.fontSize 18.0
    //                             ]
    //                             for KeyValue (experimentId, experiment) in state.Model.Experiments do
    //                                 TextBlock.create [
    //                                     TextBlock.text $"{experiment.Name}"
    //                                     TextBlock.onPointerPressed (fun e ->
    //                                         e.Handled <- true
    //                                         dispatch (Msg.SelectionChanged (NewSelection.Experiment experimentId)))
    //                                 ]
    //                         ]
    //                     ]
    //                 | Some experimentId ->
    //                     let experiment = state.Model.Experiments[experimentId]
    //                     let validatedModelResult = Validation.tryCreateModel elements state.Model.Parameters experiment
    //                     dispatch (Msg.ModelValidated validatedModelResult)
    //
    //             | ModelValidationResult.Errors errors ->
    //                 TextBox.create [
    //                     TextBox.text "Please fix the following errors."
    //                 ]
    //                 for error in errors do
    //                     TextBox.create [
    //                         TextBox.text error
    //                     ]
    //             | ModelValidationResult.Success (validatedModel, histories) ->
    //
    //                 match histories with
    //                 | None ->
    //                     let rng = System.Random validatedModel.Experiment.Seed
    //                     let simResults =
    //                         [| for i in 1..validatedModel.Experiment.Iterations do
    //                            Runner.run rng validatedModel |]
    //
    //                     let newModelValidationResult = ModelValidationResult.Success (validatedModel, Some simResults)
    //                     dispatch (Msg.ModelValidated newModelValidationResult)
    //
    //                 | Some histories ->
    //
    //                     match histories with
    //                     | [||] ->
    //                         let rng = System.Random validatedModel.Experiment.Seed
    //                         let simResults =
    //                             [| for i in 1..validatedModel.Experiment.Iterations do
    //                                Runner.run rng validatedModel |]
    //
    //                         let newModelValidationResult = ModelValidationResult.Success (validatedModel, Some simResults)
    //                         dispatch (Msg.ModelValidated newModelValidationResult)
    //
    //                     | [|history|] ->
    //                         Reports.History.view state dispatch elements validatedModel.AidosMapping history
    //
    //                     | histories ->
    //                         Reports.Histories.view state dispatch elements validatedModel.AidosMapping histories
    //
    //     ]
    // ]
