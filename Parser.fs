module Aidos.UI.Parser

open System
open System.Collections.Generic
open FSharp.UMX

module private Helpers =

    module Headers =

        [<Literal>]
        let BUFFERS = "[BUFFERS]"
        [<Literal>]
        let CONSTRAINTS = "[CONSTRAINTS]"
        [<Literal>]
        let MERGES = "[MERGES]"
        [<Literal>]
        let SPLITS ="[SPLITS]"
        [<Literal>]
        let CONVEYORS = "[CONVEYORS]"
        [<Literal>]
        let CONVERSIONS = "[CONVERSIONS]"
        [<Literal>]
        let LINKS = "[LINKS]"


    [<RequireQualifiedAccess>]
    type Section =
        | Buffers
        | Constraints
        | Splits
        | Merges
        | Conveyors
        | Conversions
        | Links
        | None

    let (|IsWhitespace|_|) (x: string) =
        match String.IsNullOrWhiteSpace x with
        | true -> Some ()
        | false -> None

    let (|IsHeader|_|) (x: string) =
        match x.Contains '[' && x.Contains ']' with
        | true -> Some ()
        | false -> None

    let (|IsComment|_|) (x: string) =
        match x.Trim().StartsWith "//" with
        | true -> Some ()
        | false -> None

    let parseNameDescriptionPair (ln: int) (labelType: string) (b: string) =
        let parts = b.Split ':'
        match parts with
        | [||] ->
            Error $"Invalid {labelType} on line {ln}. Missing Value and Label. Format should be <{labelType} Label>: <{labelType} Name>"

        | [|labelName|] ->
            let newLabelName = labelName.Trim()
            Ok (newLabelName, newLabelName)

        | [|label; name|] ->
            let newName = name.Trim()
            Ok (label, newName)

        | _ ->
            Error $"Invalid {labelType} on line {ln}. Too many fields. Format should be <{labelType} Label>: <{labelType} Name>"

    let parseLink (ln: int) (s: string) =
        if s.Contains "->" then
            match s.Split "->" with
            | [||] ->
                Error $"Invalid link on line {ln}. Missing source and target"

            | [|_|] ->
                Error $"Invalid link on line {ln}. Missing source or target"

            | [|sources; targets|] ->
                let newSources =
                    sources.Split ','
                    |> Array.map (fun source ->
                    source.Trim())
                let newTargets =
                    targets.Split ','
                    |> Array.map (fun target ->
                        target.Trim())
                match sources.Length > 0, targets.Length > 0 with
                | true, true ->
                    Ok (newSources, newTargets)
                | true, false ->
                    Error $"Invalid link on line {ln}. Missing target."
                | false, true ->
                    Error $"Invalid link on line {ln}. Missing source."
                | false, false ->
                    Error $"Invalid link on line {ln}. Missing source and target."

            | _ ->
                Error $"Invalid link on line {ln}. Too many connections"

        else
            Error $"Link missing arrow `->` on line {ln}"


    let addNameAndDescription
        (nameLines: Dictionary<_,_>)
        (descriptionLines: Dictionary<_,_>)
        (nameTypes: Dictionary<_,_>)
        nameType
        (nameDescriptions: Dictionary<_,_>)
        labeler
        lineNumber
        input
        =

        input
        |> Result.bind (fun (name: string, description: string) ->
            match nameLines.TryGetValue name, descriptionLines.TryGetValue description with
            | (false, _), (false, _) ->
                nameTypes[name] <- nameType
                nameDescriptions[labeler name] <- Description description
                nameLines[name] <- lineNumber
                descriptionLines[description] <- lineNumber
                Ok ()
            | (false, _), (true, prevLineNumber) ->
                Error $"Duplicate name {name} on line {lineNumber}. Name previously declared on line {prevLineNumber}"

            | (true, prevLineNumber), (false, _) ->
                Error $"Duplicate label {name} on line {lineNumber}. Label previously declared on line {prevLineNumber}"

            | (true, labelPrevLineNumber), (true, namePrevLineNumber) ->
                Error $"Duplicate name {name} and description {description} on line {lineNumber}. Name previously declared on line {labelPrevLineNumber}. Name previously declared on line {namePrevLineNumber}")


    let parseHeader
        lineNumber
        (section: byref<Section>)
        (header: string)
        =

        match header.ToUpper() with
        | Headers.BUFFERS ->
            section <- Section.Buffers
            Ok ()
        | Headers.CONSTRAINTS ->
            section <- Section.Constraints
            Ok ()
        | Headers.SPLITS ->
            section <- Section.Splits
            Ok ()
        | Headers.MERGES ->
            section <- Section.Merges
            Ok ()
        | Headers.CONVEYORS ->
            section <- Section.Conveyors
            Ok ()
        | Headers.CONVERSIONS ->
            section <- Section.Conversions
            Ok ()
        | Headers.LINKS ->
            section <- Section.Links
            Ok ()
        | _ ->
            Error $"Unknown header {header} on line {lineNumber}"


open Helpers

let parse (text: string) =
    let nameLines = Dictionary()
    let descriptionLines = Dictionary()
    let linkLines = Dictionary()
    let buffers = Dictionary()
    let constraints = Dictionary()
    let splits = Dictionary()
    let merges = Dictionary()
    let conveyors = Dictionary()
    let conversions = Dictionary()
    let nameTypes = Dictionary<string, NameType>()

    let noSource = HashSet()
    let noTarget = HashSet()

    let mutable section = Section.None
    let mutable result = Ok ()
    let mutable lineNumber = 0
    let reader = new IO.StringReader(text)
    let mutable nextLine = reader.ReadLine()

    while not (isNull nextLine) && (Result.isOk result) do

        let cleanedLine =
            if nextLine.Contains "//" then
                let commentStart = nextLine.IndexOf "//"
                nextLine.Substring (0, commentStart)
            else
                nextLine

        match cleanedLine with
        | IsWhitespace ->
            ()

        | IsHeader ->
            result <- parseHeader lineNumber &section nextLine

        | line ->
            match section with
            | Section.None ->
                result <- Error $"Missing section header for line {lineNumber}"

            | Section.Buffers ->
                result <-
                    parseNameDescriptionPair lineNumber "Buffer" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Buffer buffers BufferName.create lineNumber)

            | Section.Constraints ->
                result <-
                    parseNameDescriptionPair lineNumber "Constraint" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Constraint constraints ConstraintName.create lineNumber)

            | Section.Splits ->
                result <-
                    parseNameDescriptionPair lineNumber "Split" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Split splits SplitName.create lineNumber)

            | Section.Merges ->
                result <-
                    parseNameDescriptionPair lineNumber "Merge" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Merge merges MergeName.create lineNumber)

            | Section.Conveyors ->
                result <-
                    parseNameDescriptionPair lineNumber "Conveyor" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Conveyor conveyors ConveyorName.create lineNumber)

            | Section.Conversions ->
                result <-
                    parseNameDescriptionPair lineNumber "Conversion" line
                    |> (addNameAndDescription nameLines descriptionLines nameTypes NameType.Conversion conversions ConversionName.create lineNumber)

            | Section.Links ->
                result <-
                    parseLink lineNumber line
                    |> Result.bind (fun (sources, targets) ->
                        // At this point I know that Sources and Targets have a length > 0
                        match sources.Length > 1, targets.Length > 1 with
                        | true, true ->
                            Error $"Invalid link on line {lineNumber}. Cannot have multi-way links."
                        | false, false ->
                            Ok [|sources[0], targets[0]|]
                        | false, true ->
                            let source = sources[0]
                            match nameTypes.TryGetValue source with
                            | true, labelType ->
                                match labelType with
                                | NameType.Split ->
                                    let links =
                                        targets
                                        |> Array.map (fun target ->
                                            source, target)
                                    Ok links
                                | _ ->
                                    Error $"Invalid link on line {lineNumber}. Only Split nodes can have multilple targets."

                            | false, _ ->
                                Error $"Invalid source on line {lineNumber}. The source has not been declared"
                        | true, false ->
                            let target = targets[0]
                            match nameTypes.TryGetValue target with
                            | true, labelType ->
                                match labelType with
                                | NameType.Merge ->
                                    let links =
                                        sources
                                        |> Array.map (fun source ->
                                            source, target)
                                    Ok links
                                | _ ->
                                    Error $"Invalid link on line {lineNumber}. Only Merge nodes can have multiple sources."

                            | false, _ ->
                                Error $"Invalid target on line {lineNumber}. The target has not been declared"
                    )
                    |> Result.bind (fun (links: (string * string)[]) ->

                        (Ok (), links)
                        ||> Array.fold (fun state (sourceName, targetName) ->

                            state
                            |> Result.bind (fun () ->
                                match nameTypes.TryGetValue sourceName, nameTypes.TryGetValue targetName with
                                | (true, sourceType), (true, targetType) ->
                                    match linkLines.TryGetValue ((sourceName, targetName)) with
                                    | true, prevLineNumber ->
                                        if lineNumber = prevLineNumber then
                                            Error $"Repeated link on line {lineNumber}. {sourceName} is connected to {targetName} multiple times on line {prevLineNumber}"
                                        else
                                            Error $"Duplicate link on line {lineNumber}. {sourceName} is already connected to {targetName} from line {prevLineNumber}"

                                    | false, _ ->

                                        match noTarget.Add sourceName, noSource.Add targetName with
                                        | true, true ->
                                            linkLines[(sourceName, targetName)] <- lineNumber
                                            Ok ()

                                        | true, false ->
                                            match targetType with
                                            | NameType.Merge ->
                                                linkLines[(sourceName, targetName)] <- lineNumber
                                                Ok ()
                                            | _ ->
                                                Error $"Invalid link on line {lineNumber}. The target {targetName} cannot have multiple sources"

                                        | false, true ->
                                            match sourceType with
                                            | NameType.Split ->
                                                linkLines[(sourceName, targetName)] <- lineNumber
                                                Ok ()
                                            | _ ->
                                                Error $"Invalid link on line {lineNumber}. The source {sourceName} cannot have multiple targets"

                                        | false, false ->
                                            match sourceType, targetType with
                                            | NameType.Merge, NameType.Split ->
                                                linkLines[(sourceName, targetName)] <- lineNumber
                                                Ok ()
                                            | NameType.Merge, _ ->
                                                Error $"Invalid link on line {lineNumber}. The target {targetName} cannot have multiple sources"
                                            | _, NameType.Split ->
                                                Error $"Invalid link on line {lineNumber}. The source {sourceName} cannot have multiple targets"
                                            | _, _ ->
                                                Error $"Invalid link on line {lineNumber}. The source {sourceName} cannot have multiple targets and the target {targetName} cannot have multilple sources."

                                | (false, _), (true, _) ->
                                    Error $"Invalid link on line {lineNumber}. The source {sourceName} does not exist"

                                | (true, _), (false, _) ->
                                    Error $"Invalid link on line {lineNumber}. The target {targetName} does not exist"

                                | (false, _), (false, _) ->
                                    Error $"Invalid link on line {lineNumber}. The source {sourceName} and the target {targetName} do not exist")))



        nextLine <- reader.ReadLine()
        lineNumber <- lineNumber + 1

    result
    |> Result.bind (fun () ->
        (Ok (), buffers)
        ||> Seq.fold (fun state (KeyValue(bufferName, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains %bufferName, noTarget.Contains %bufferName with
                | false, false ->
                    Error $"Buffer {bufferName} must have a source or a target"
                | _, _ ->
                    Ok ())))
    |> Result.bind (fun () ->
        (Ok (), constraints)
        ||> Seq.fold (fun state (KeyValue(constraintName, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains (% constraintName), noTarget.Contains (% constraintName) with
                | false, false ->
                    Error $"Constraint {constraintName} must have a source and a target"
                | true, false ->
                    Error $"Constraint {constraintName} must have a target"
                | false, true ->
                    Error $"Constraint {constraintName} must have a source"
                | true, true ->
                    Ok ())))
    |> Result.bind (fun () ->
        (Ok (), conversions)
        ||> Seq.fold (fun state (KeyValue(conversion, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains %conversion, noTarget.Contains %conversion with
                | false, false ->
                    Error $"Conversion {conversion} must have a source and a target"
                | true, false ->
                    Error $"Conversion {conversion} must have a target"
                | false, true ->
                    Error $"Conversion {conversion} must have a source"
                | true, true ->
                    Ok ())))
    |> Result.bind (fun () ->
        (Ok (), conveyors)
        ||> Seq.fold (fun state (KeyValue(conveyorName, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains %conveyorName, noTarget.Contains %conveyorName with
                | false, false ->
                    Error $"Conveyor {conveyorName} must have a source and a target"
                | true, false ->
                    Error $"Conveyor {conveyorName} must have a target"
                | false, true ->
                    Error $"Conveyor {conveyorName} must have a source"
                | true, true ->
                    Ok ())))
    |> Result.bind (fun () ->
        (Ok (), splits)
        ||> Seq.fold (fun state (KeyValue(splitName, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains %splitName, noTarget.Contains %splitName with
                | false, false ->
                    Error $"Split {splitName} must have a source and a target"
                | true, false ->
                    Error $"Split {splitName} must have a target"
                | false, true ->
                    Error $"Split {splitName} must have a source"
                | true, true ->
                    Ok ())))
    |> Result.bind (fun () ->
        (Ok (), merges)
        ||> Seq.fold (fun state (KeyValue(mergeName, _))->
            state
            |> Result.bind (fun () ->
                match noSource.Contains %mergeName, noTarget.Contains %mergeName with
                | false, false ->
                    Error $"Merge {mergeName} must have a source and a target"
                | true, false ->
                    Error $"Merge {mergeName} must have a target"
                | false, true ->
                    Error $"Merge {mergeName} must have a source"
                | true, true ->
                    Ok ())))
    |> Result.bind (fun () ->
        let newNodes = HashSet()

        let newLinks =
            linkLines.Keys
            |> Seq.map (fun (source, target) ->
                let newSource = Node.ofNameType source nameTypes[source]
                let newTarget = Node.ofNameType target nameTypes[target]
                newNodes.Add newSource |> ignore
                newNodes.Add newTarget |> ignore
                newSource, newTarget)
            |> HashSet

        if newNodes.Count = 0 then
            Error "No nodes in network"
        else

            Ok {
                Nodes = Array.ofSeq newNodes
                Links = Array.ofSeq newLinks
                Buffers = buffers |> Seq.map (|KeyValue|) |> Map
                Constraints = constraints |> Seq.map (|KeyValue|) |> Map
                Splits = splits |> Seq.map (|KeyValue|) |> Map
                Merges = merges |> Seq.map (|KeyValue|) |> Map
                Conveyors = conveyors |> Seq.map (|KeyValue|) |> Map
                Conversions = conversions |> Seq.map (|KeyValue|) |> Map
            })
