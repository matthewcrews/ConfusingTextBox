module Aidos.UI.Chart


open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open LiveChartsCore
open LiveChartsCore.Measure
open LiveChartsCore.Defaults
open LiveChartsCore.Kernel
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.SkiaSharpView.Avalonia

module Mappings =

    let floatTuple =
        System.Action<float * float, ChartPoint>(fun (x, y) point ->
            let coordinate = Coordinate(x, y)
            point.Coordinate <- coordinate)

    let dateTimeFloat =
        System.Action<(System.DateTime * float), ChartPoint>(fun (x, y) point ->
            let dt = DateTimePoint (x, y)
            point.Coordinate <- dt.Coordinate)

    let timeSpanFloat =
        System.Action<System.TimeSpan * float, ChartPoint>(fun (x, y) point ->
            let ts = TimeSpanPoint (x, y)
            point.Coordinate <- ts.Coordinate)


[<AutoOpen>]
module CartesianChart =

    let create (attrs: IAttr<CartesianChart> list): IView<CartesianChart> =
        ViewBuilder.Create<CartesianChart>(attrs)

    type CartesianChart with

        static member series<'t when 't :> CartesianChart> (value: seq<ISeries>) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty<seq<ISeries>>(
                property = CartesianChart.SeriesProperty,
                value = value,
                comparer = ValueNone
            )

        static member xAxes<'t when 't :> CartesianChart> (value: seq<ICartesianAxis>) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty<seq<ICartesianAxis>>(
                property = CartesianChart.XAxesProperty,
                value = value,
                comparer = ValueNone
            )

        static member yAxes<'t when 't :> CartesianChart> (value: seq<ICartesianAxis>) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty<seq<ICartesianAxis>>(
                property = CartesianChart.YAxesProperty,
                value = value,
                comparer = ValueNone
            )

        static member title<'t when 't :> CartesianChart> (value: VisualElements.VisualElement<Drawing.SkiaSharpDrawingContext>) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty<_>(
                property = CartesianChart.TitleProperty,
                value = value,
                comparer = ValueNone
            )

        static member legendPosition<'t when 't :> CartesianChart> (value: LegendPosition) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty<_>(
                property = CartesianChart.LegendPositionProperty,
                value = value,
                comparer = ValueNone
            )


type Series () =

    static member stepLine (values: seq<_>, yAxis: int) =
        let series = StepLineSeries<_>()
        series.Values <- values
        series.ScalesYAt <- yAxis
        series

    static member line (values: seq<_>, yAxis: int) =
        let series = LineSeries<_>()
        series.Values <- values
        series.ScalesYAt <- yAxis
        series.LineSmoothness <- 0.0
        series
