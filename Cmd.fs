[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Aidos.UI.Cmd


let handle
    (diagramSettings: DiagramSettings)
    (model: Model)
    (cmd: Cmd)
    : Model
    =

    match cmd with
    | Cmd.ModelTextChanged newText ->
        let newModelState =
            match Parser.parse newText with
            | Result.Ok newModel ->
                let parameters = model.Parameters
                for KeyValue(name, _) in newModel.Buffers do
                    if not (parameters.Buffers.Capacity.ContainsKey name) then
                        parameters.Buffers.Capacity[name] <- "100.0"

                    if not (parameters.Buffers.Level.ContainsKey name) then
                        parameters.Buffers.Level[name] <- "0.0"

                for KeyValue(name, _) in newModel.Constraints do
                    if not (parameters.Constraints.Limit.ContainsKey name) then
                        parameters.Constraints.Limit[name] <- "1.0"

                let newLayout = Layout.create diagramSettings newModel.Links
                ModelStatus.Success (newModel, newLayout)
            | Result.Error errors ->
                ModelStatus.Error errors

        { model with
            Text = newText
            Status = newModelState }

    | Cmd.ExperimentChanged change ->
        match change with
        | ExperimentChange.Duration (experimentId, newDuration) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with Duration = newDuration })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.Seed (experimentId, newSeed) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with Seed = newSeed })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.TimeUnit (experimentId, newTimeUnit) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with TimeUnit = newTimeUnit })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.Iterations (experimentId, newIterations) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with Iterations = newIterations })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.Name(experimentId, newName) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with Name = newName })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.SinkBuffers(experimentId, newSinkBuffers) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with SinkBuffers = newSinkBuffers })
            { model with
                Experiments = newExperiments }

        | ExperimentChange.TheoreticalLimit(experimentId, newTheoreticalLimit) ->
            let newExperiments = model.Experiments.Add (experimentId, { model.Experiments[experimentId] with TheoreticalLimit = newTheoreticalLimit })
            { model with
                Experiments = newExperiments }

    | Cmd.BufferChanged bc ->
        match bc with
        | BufferChange.Capacity (name, newCapacity) ->
            model.Parameters.Buffers.Capacity[name] <- newCapacity
            model

        | BufferChange.Level (name, newLevel) ->
            model.Parameters.Buffers.Level[name] <- newLevel
            model

    | Cmd.ConstraintChanged cc ->
        match cc with
        | ConstraintChange.Limit(constraintName, newLimit) ->
            model.Parameters.Constraints.Limit[constraintName] <- newLimit
            model

        | ConstraintChange.TimeInterruptAdded (constraintName, newTimeInterrupt) ->
            match model.Parameters.Constraints.TimeInterrupts.TryGetValue constraintName with
            | true, timeInterrupts ->
                timeInterrupts.Add newTimeInterrupt
            | false, _ ->
                let newTimeInterrupts = ResizeArray()
                newTimeInterrupts.Add newTimeInterrupt
                model.Parameters.Constraints.TimeInterrupts[constraintName] <- newTimeInterrupts

            model

        | ConstraintChange.TimeInterruptChanged (constraintName, interruptId, change) ->
            let interrupts = model.Parameters.Constraints.TimeInterrupts[constraintName]
            let idx = interrupts.FindIndex (fun it -> it.Id = interruptId)
            let oldInterrupt = interrupts[idx]

            let newInterrupt =
                match change with
                | TimeInterruptChange.Name newName ->
                    { oldInterrupt with
                        Name = newName }
                | TimeInterruptChange.ClockType newClockType ->
                    { oldInterrupt with
                        ClockType = newClockType }
                | TimeInterruptChange.ResetType newResetType ->
                    { oldInterrupt with
                        ResetType = newResetType }
                | TimeInterruptChange.DowntimeType newDowntimeType ->
                    { oldInterrupt with
                        DowntimeType = newDowntimeType }
                | TimeInterruptChange.TimeUntilParam1 newParam ->
                    { oldInterrupt with
                        TimeUntilParam1 = newParam }
                | TimeInterruptChange.TimeUntilParam2 newParam ->
                    { oldInterrupt with
                        TimeUntilParam2 = newParam }
                | TimeInterruptChange.TimeToRecoverParam1 newParam ->
                    { oldInterrupt with
                        TimeToRecoverParam1 = newParam }
                | TimeInterruptChange.TimeToRecoverParam2 newParam ->
                    { oldInterrupt with
                        TimeToRecoverParam2 = newParam }
                | TimeInterruptChange.TimeUntilDistributionType newType ->
                    { oldInterrupt with
                        TimeUntilDistributionType = newType }
                | TimeInterruptChange.TimeToRecoverDistributionType newType ->
                    { oldInterrupt with
                        TimeToRecoverDistributionType = newType }

            interrupts[idx] <- newInterrupt
            model

    | Cmd.AddExperiment newExperiment ->
        let newExperiments = model.Experiments.Add (newExperiment.Id, newExperiment)
        { model with
            Experiments = newExperiments }
