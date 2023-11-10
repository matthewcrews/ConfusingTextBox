module Aidos.UI.Validation

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open FSharp.UMX
open Aidos.UI

module private Helpers =

    let parseBufferCapacities
        (errors: Stack<string>)
        (capacities: Dictionary<BufferName, string>)
        =
        let acc = Dictionary()
        for KeyValue(n, c) in capacities do
            match Double.TryParse c with
            | true, capacity ->
                acc[n] <- capacity
            | false, _ ->
                errors.Push $"Buffer {n} has invalid Capacity of {c}"

        acc

    let parseBufferLevels
        (errors: Stack<string>)
        (levels: Dictionary<BufferName, string>)
        =
        let acc = Dictionary()
        for KeyValue(n, c) in levels do
            match Double.TryParse c with
            | true, level ->
                acc[n] <- level
            | false, _ ->
                errors.Push $"Buffer {n} has invalid Level of {c}"

        acc

    let parseConstraintLimits
        (errors: Stack<string>)
        (limits: Dictionary<ConstraintName, string>)
        =
        let acc = Dictionary()
        for KeyValue(n, v) in limits do
            match Double.TryParse v with
            | true, v ->
                acc[n] <- v
            | false, _ ->
                errors.Push $"Constraint {n} has invalid Limit of {v}"

        acc


    let parseConstraintInterrupts
        (errors: Stack<String>)
        (interrupts: Dictionary<ConstraintName, ResizeArray<Parameters.TimeInterrupt>>)
        =
        Dictionary []
        // interrupts
        // |> Seq.map (fun (KeyValue (constraintName, timeInterrupts)) ->
        //     let newTimeInterrupts =
        //         timeInterrupts
        //         |> Seq.map (fun timeInterrupt ->
        //             let timeUntilParam1 =
        //                 match Double.TryParse timeInterrupt.TimeUntilParam1 with
        //                 | true, v -> v
        //                 | false, _ ->
        //                     errors.Push $"Constraint {constraintName} has invalid Time Until parameter of {timeInterrupt.TimeUntilParam1}"
        //                     0.0
        //             let timeUntilParam2 =
        //                 match Double.TryParse timeInterrupt.TimeUntilParam2 with
        //                 | true, v -> v
        //                 | false, _ ->
        //                     errors.Push $"Constraint {constraintName} has invalid Time Until parameter of {timeInterrupt.TimeUntilParam2}"
        //                     0.0
        //
        //             let timeUntilDistribution =
        //                 match timeInterrupt.TimeUntilDistributionType with
        //                 | Parameters.DistributionType.Fixed ->
        //                     Aidos.Distribution.Fixed timeUntilParam1
        //                 | Parameters.DistributionType.Normal ->
        //                     Aidos.Distribution.Normal (timeUntilParam1, timeUntilParam2)
        //                 | Parameters.DistributionType.Uniform ->
        //                     Aidos.Distribution.Uniform (timeUntilParam1, timeUntilParam2)
        //                 | Parameters.DistributionType.Weibull ->
        //                     Aidos.Distribution.Weibull (timeUntilParam1, timeUntilParam2)
        //                 | Parameters.DistributionType.LogNormal ->
        //                     Aidos.Distribution.LogNormal (timeUntilParam1, timeUntilParam2)
        //
        //             let timeToRecoverParam1 =
        //                 match Double.TryParse timeInterrupt.TimeToRecoverParam1 with
        //                 | true, v -> v
        //                 | false, _ ->
        //                     errors.Push $"Constraint {constraintName} has invalid Time Until parameter of {timeInterrupt.TimeToRecoverParam1}"
        //                     0.0
        //             let timeToRecoverParam2 =
        //                 match Double.TryParse timeInterrupt.TimeToRecoverParam2 with
        //                 | true, v -> v
        //                 | false, _ ->
        //                     errors.Push $"Constraint {constraintName} has invalid Time Until parameter of {timeInterrupt.TimeToRecoverParam2}"
        //                     0.0
        //
        //             let timeToRecoverDistribution =
        //                 match timeInterrupt.TimeUntilDistributionType with
        //                 | Parameters.DistributionType.Fixed ->
        //                     Aidos.Distribution.Fixed timeToRecoverParam1
        //                 | Parameters.DistributionType.Normal ->
        //                     Aidos.Distribution.Normal (timeToRecoverParam1, timeToRecoverParam2)
        //                 | Parameters.DistributionType.Uniform ->
        //                     Aidos.Distribution.Uniform (timeToRecoverParam1, timeToRecoverParam2)
        //                 | Parameters.DistributionType.Weibull ->
        //                     Aidos.Distribution.Weibull (timeToRecoverParam1, timeToRecoverParam2)
        //                 | Parameters.DistributionType.LogNormal ->
        //                     Aidos.Distribution.LogNormal (timeToRecoverParam1, timeToRecoverParam2)
        //
        //
        //             Aidos.Modeling.Interrupt.time timeInterrupt.Name {
        //                 downtimeType (
        //                     match timeInterrupt.DowntimeType with
        //                     | Parameters.DowntimeType.Failure ->
        //                         Aidos.Modeling.DowntimeType.Failure
        //                     | Parameters.DowntimeType.Planned ->
        //                         Aidos.Modeling.DowntimeType.Planned)
        //                 resetType (
        //                     match timeInterrupt.ResetType with
        //                     | Parameters.ResetType.Cumulative ->
        //                         Aidos.Modeling.ResetType.Cumulative
        //                     | Parameters.ResetType.Recovery ->
        //                         Aidos.Modeling.ResetType.Recovery
        //                     | Parameters.ResetType.Restart ->
        //                         Aidos.Modeling.ResetType.Restart)
        //                 clockType (
        //                     match timeInterrupt.ClockType with
        //                     | Parameters.ClockType.Available ->
        //                         Aidos.Modeling.ClockType.Available
        //                     | Parameters.ClockType.Calendar ->
        //                         Aidos.Modeling.ClockType.Calendar
        //                     | Parameters.ClockType.Running ->
        //                         Aidos.Modeling.ClockType.Running)
        //                 timeUntil timeUntilDistribution
        //                 timeToRecover timeToRecoverDistribution
        //             })
        //         |> List.ofSeq
        //     KeyValuePair (constraintName, newTimeInterrupts))
        // |> Dictionary


    let parseExperiment
        (errors: Stack<string>)
        (elements: Elements)
        (experiment: Parameters.Experiment)
        : ValidatedExperiment
        =

        let newSeed =
            match Int32.TryParse experiment.Seed with
            | true, newSeed -> newSeed
            | false, _ ->
                errors.Push $"Invalid seed for random numbers {experiment.Seed}"
                0

        let newDuration =
            match Double.TryParse experiment.Duration with
            | true, newDuration ->
                newDuration
            | false, _ ->
                errors.Push $"Invalid Duration {experiment.Duration}"
                0.0

        let newIterations =
            match Int32.TryParse experiment.Iterations with
            | true, newIterations -> newIterations
            | false, _ ->
                errors.Push $"Invalid iterations of {experiment.Iterations}"
                0

        let newTheoreticalLimit =
            match Double.TryParse experiment.TheoreticalLimit with
            | true, v -> v
            | false, _ ->
                errors.Push $"Experiment {experiment.Name} has invalid Theoretical Limit of {experiment.TheoreticalLimit}"
                0.0

        let newSinkBuffers =
            experiment.SinkBuffers.Split ','
            |> Array.map (fun s ->
                BufferName.create (s.Trim()))

        for buffer in newSinkBuffers do
            if elements.Buffers.Keys.Contains buffer then
                ()
            else
                errors.Push $"Sink Buffer {buffer} does not exist in model."

        {
            Seed = newSeed
            TimeUnit = experiment.TimeUnit
            Duration = newDuration
            Iterations = newIterations
            TheoreticalLimit = newTheoreticalLimit
            SinkBuffers = newSinkBuffers
        }

open Helpers


let tryCreateModel
    (elements: Elements)
    (parameters: Parameters)
    (experiment: Parameters.Experiment)
    =
    //
    // let errors = Stack()
    // let constraintLimits = parseConstraintLimits errors parameters.Constraints.Limit
    // let constraintInterrupts = parseConstraintInterrupts errors parameters.Constraints.TimeInterrupts
    // let bufferCapacities = parseBufferCapacities errors parameters.Buffers.Capacity
    // let bufferLevels = parseBufferLevels errors parameters.Buffers.Level
    // let experiment = parseExperiment errors elements experiment
    //
    // if errors.Count > 0 then
    //     ModelValidationResult.Errors (errors.ToArray())
    //
    // else
    //
    //     let tus =
    //         match experiment.TimeUnit with
    //         | TimeUnit.Seconds -> Aidos.Modeling.TimeUnits.Seconds
    //         | TimeUnit.Minutes -> Aidos.Modeling.TimeUnits.Minutes
    //         | TimeUnit.Hours -> Aidos.Modeling.TimeUnits.Hours
    //         | TimeUnit.Days -> Aidos.Modeling.TimeUnits.Days
    //
    //     let bufferMapping =
    //         elements.Buffers
    //         |> Seq.map (|KeyValue|)
    //         |> Seq.map fst
    //         |> Seq.map (fun name ->
    //             let newBuffer = Aidos.Modeling.Buffer %name
    //             KeyValuePair (name, newBuffer))
    //         |> Dictionary
    //
    //     let constraintMapping =
    //         elements.Constraints
    //         |> Seq.map (|KeyValue|)
    //         |> Seq.map fst
    //         |> Seq.map (fun name ->
    //             let newConstraint = Aidos.Modeling.Constraint %name
    //             KeyValuePair(name, newConstraint))
    //         |> Dictionary
    //
    //     let nodeMapping =
    //         elements.Nodes
    //         |> Seq.map (fun node ->
    //             let aidosNode =
    //                 match node with
    //                 | Node.Buffer n ->
    //                     let n = Aidos.Modeling.Buffer %n
    //                     n.Node
    //                 | Node.Constraint n ->
    //                     let n = Aidos.Modeling.Constraint %n
    //                     n.Node
    //                 | Node.Split n ->
    //                     let n = Aidos.Modeling.Split %n
    //                     n.Node
    //                 | Node.Merge n ->
    //                     let n = Aidos.Modeling.Merge %n
    //                     n.Node
    //                 | Node.Conveyor n ->
    //                     let n = Aidos.Modeling.Conveyor %n
    //                     n.Node
    //                 | Node.Conversion n ->
    //                     let n = Aidos.Modeling.Conversion %n
    //                     n.Node
    //             node, aidosNode)
    //         |> dict
    //
    //     let model =
    //         Aidos.Modeling.Model "Test" {
    //             timeUnits tus
    //             capacities [
    //                 for KeyValue (bufferName, aidosBuffer) in bufferMapping do
    //                     aidosBuffer, bufferCapacities[bufferName]
    //             ]
    //             levels [
    //                 for KeyValue (bufferName, aidosBuffer) in bufferMapping do
    //                     aidosBuffer, bufferLevels[bufferName]
    //             ]
    //             limits [
    //                 for KeyValue (constraintName, aidosConstraint) in constraintMapping do
    //                     aidosConstraint, constraintLimits[constraintName]
    //             ]
    //             structure [
    //                 for source, target in elements.Links do
    //                     nodeMapping[source] --> nodeMapping[target]
    //             ]
    //             interrupts [
    //                 for KeyValue (constraintName, timeInterrupts) in constraintInterrupts do
    //                     let aidosName = Aidos.Modeling.Constraint %constraintName
    //                     aidosName, timeInterrupts
    //             ]
    //         }
    //
    //     let aidosMapping : AidosMapping = {
    //         Buffers = ReadOnlyDictionary bufferMapping
    //         Constraints = ReadOnlyDictionary constraintMapping
    //     }
    //
    //     let validatedModel = {
    //         Elements = elements
    //         AidosMapping = aidosMapping
    //         Model = model
    //         Experiment = experiment
    //     }

        ModelValidationResult.Success (Some 1)

        // let rng = Random simulationSettings.Seed
        // let sim = Aidos.Simulation (rng, model)
        // let endTime = (Aidos.Modeling.Time.ofTimeUnits tus simulationSettings.Duration)
        // sim.SimulateTo endTime
        // let history = sim.GetHistory()
        // Result.Ok (elements, aidosMapping, history)
