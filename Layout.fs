[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Aidos.UI.Layout

open System.Collections.Generic

let create
    (diagramSettings: DiagramSettings)
    (links: (Node * Node)[])
    =

    let nodeWidth = float32 diagramSettings.NodeWidth
    let nodeHeight = float32 diagramSettings.NodeHeight

    let dg = Dagre.DagreInputGraph()
    dg.VerticalLayout <- false
    let nodesAcc = Dictionary()
    let edgeAcc = Dictionary()

    for source, target in links do
        let sourceNode =
            match nodesAcc.TryGetValue source with
            | true, n -> n
            | false, _ ->
                let newNode = dg.AddNode (source, nodeWidth, nodeHeight)
                nodesAcc[source] <- newNode
                newNode
        let targetNode =
            match nodesAcc.TryGetValue target with
            | true, n -> n
            | false, _ ->
                let newNode = dg.AddNode (target, nodeWidth, nodeHeight)
                nodesAcc[target] <- newNode
                newNode

        let edge = dg.AddEdge (sourceNode, targetNode)
        edgeAcc[(source, target)] <- edge


    dg.Layout()

    let sitePoint =
        nodesAcc
        |> Seq.map (fun (KeyValue (site, node)) ->
            let p = Location (float node.X, float node.Y)
            site, p)
        |> Map

    let edgePoints =
        edgeAcc
        |> Seq.map (fun (KeyValue((source, label), edge)) ->
            let points =
                edge.Points
                |> Array.map (fun p ->
                Location (float p.X, float p.Y))
            (source, label), points)
        |> Map


    {
        SitePoint = sitePoint
        EdgePoints = edgePoints
    }
