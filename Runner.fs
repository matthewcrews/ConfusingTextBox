module Aidos.UI.Runner

open System

let run
    (rng: Random)
    (validatedModel: ValidatedModel)
    =

    // let tus =
    //     match validatedModel.Experiment.TimeUnit with
    //     | TimeUnit.Seconds -> Aidos.Modeling.TimeUnits.Seconds
    //     | TimeUnit.Minutes -> Aidos.Modeling.TimeUnits.Minutes
    //     | TimeUnit.Hours -> Aidos.Modeling.TimeUnits.Hours
    //     | TimeUnit.Days -> Aidos.Modeling.TimeUnits.Days
    //
    // let sim = Aidos.Simulation (rng, validatedModel.Model)
    // let endTime = (Aidos.Modeling.Time.ofTimeUnits tus validatedModel.Experiment.Duration)
    // sim.SimulateTo endTime
    // let history = sim.GetHistory()
    // history
    ()
