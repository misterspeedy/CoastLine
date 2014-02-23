module MainApp

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Threading
open System.Windows.Media
open System.Windows.Shapes
open System.Windows.Input
open FSharpx

open CoastLine

let Colours =
    [|
        Colors.Black
//        Colors.Blue
//        Colors.Red
//        Colors.Green
//        Colors.Aqua
//        Colors.Orange
//        Colors.BlueViolet
//        Colors.Cyan
    |]

type MainWindow = XAML<"MainWindow.xaml">

let win = MainWindow()
do win.Root.WindowState <- WindowState.Maximized
let dir = System.IO.Directory.GetCurrentDirectory()
let dataPath = System.IO.Path.Combine(dir, @"..\..\..\..\Data\norway.dat")
let rawData = Data.GetCoast dataPath

let ToPolyLine i points =
    let colorIndex = i % (Colours |> Array.length)

    let xScale = win.Root.ActualHeight / 50. * 1.7 
    let yScale = win.Root.ActualHeight / 30. * 1.7

    let scaledPoints = points |> Seq.map (fun p -> Point((p.Long * xScale), (72.- p.Lat) * yScale))

    let pl = Polyline()
    pl.StrokeThickness <- 1.5
    pl.Stroke <- SolidColorBrush Colours.[colorIndex]
    pl.StrokeEndLineCap <- Media.PenLineCap.Round
    pl.StrokeLineJoin <- Media.PenLineJoin.Round
    pl.Points <- PointCollection(scaledPoints)
    pl

let ShowPointCounts simpleCount rawCount =
    win.PointCount.Content <- sprintf "%i of %i" simpleCount rawCount

let Refresh epsilon data =
    // Force Epsilon background to change colour for feedback:
    win.Epsilon.Background <- new SolidColorBrush(Colors.DarkGray)
    win.Epsilon.Dispatcher.Invoke((fun _ -> ()), DispatcherPriority.Render)

    win.Map.Children.Clear()

    let simplified = data |> Data.Simplify epsilon

    simplified
    |> Seq.mapi ToPolyLine
    |> Seq.iter (fun pl -> win.Map.Children.Add(pl) |> ignore)
    
    ShowPointCounts (simplified |> Seq.concat |> Seq.length) (data |> Seq.concat |> Seq.length)

    // Give visual cue for completed:
    win.Epsilon.Background <- System.Windows.Media.Brushes.Transparent

let rec GetEpsilon() =
    let ok, result = Double.TryParse win.Epsilon.Text
    if ok then
        result
    else
        win.Epsilon.Text <- "0"
        GetEpsilon()

let MultiplyEpsilon m =
    let currentE = GetEpsilon()
    let newE = currentE * m
    win.Epsilon.Text <- newE.ToString()

let IncreaseEpsilon() =
    MultiplyEpsilon 2.

let DecreaseEpsilon() =
    MultiplyEpsilon 0.5

let LoadWindow() =
    win.Refresh.Click.Add(fun _ -> 
        Refresh (GetEpsilon()) rawData
    )

    win.EpsilonUp.Click.Add(fun _ ->
        IncreaseEpsilon()
        Refresh (GetEpsilon()) rawData
    )

    win.EpsilonDown.Click.Add(fun _ ->
        DecreaseEpsilon()
        Refresh (GetEpsilon()) rawData
    )

    win.Root.SizeChanged.Add(fun _ -> 
        Refresh (GetEpsilon()) rawData
    )

    win.Root

[<STAThread>]
(new Application()).Run(LoadWindow()) |> ignore