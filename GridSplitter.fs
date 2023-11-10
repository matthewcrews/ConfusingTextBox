[<AutoOpen>]
module Aidos.UI.GridSplitter

open Avalonia.Controls
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open AvaloniaEdit

type GridSplitter with

    static member resizeDirection<'t when 't :> GridSplitter> value: IAttr<'t> =
        AttrBuilder<'t>.CreateProperty<_>(
            property = GridSplitter.ResizeDirectionProperty,
            value = value,
            comparer = ValueNone
        )
