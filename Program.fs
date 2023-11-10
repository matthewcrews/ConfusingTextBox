module Aidos.UI.Program

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open Avalonia
open Avalonia.Themes.Fluent
open AvaloniaEdit.Document
open Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes
open AvaloniaEdit
open LiveChartsCore
open LiveChartsCore.SkiaSharpView
open Aidos.UI
open Aidos.UI.View
open Avalonia.Controls
open Avalonia.FuncUI.DSL

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Aidos Viewer"
        base.Height <- 600.0
        base.Width <- 800.0

        let editorChangeSubject = new Subject<TextEditor>()

        let subscriptions (model: State) : Sub<Msg> =
            let editorChangeStream (dispatch: Msg -> unit) =
                editorChangeSubject
                    .Throttle(TimeSpan.FromSeconds 1)
                    .Subscribe(fun (editor: TextEditor) ->
                        dispatch (Msg.ModelTextChanged editor)
                    )

            [
                [ nameof editorChangeStream ], editorChangeStream
            ]

        let textDocument = TextDocument()

        Elmish.Program.mkSimple (State.init textDocument) Msg.apply (Screen.view this editorChangeSubject textDocument)
        |> Program.withHost this
        |> Program.withSubscription subscriptions
        |> Program.withConsoleTrace
        |> Program.runWithAvaloniaSyncDispatch ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.Styles.Load "avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml"
        this.RequestedThemeVariant <- Styling.ThemeVariant.Light
        LiveCharts.Configure(fun config ->
            config
                .AddSkiaSharp()
                .HasMap(Chart.Mappings.floatTuple)
                .HasMap(Chart.Mappings.dateTimeFloat)
                .HasMap(Chart.Mappings.timeSpanFloat)

            |> ignore
            ())

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            let mainWindow = MainWindow()
            desktopLifetime.MainWindow <- mainWindow
        | _ -> ()


module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args)
