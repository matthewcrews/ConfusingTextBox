﻿module Aidos.UI.View.EditPanes.Conversions

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Aidos.UI

let view (state: State) dispatch =
    TextBlock.create [
            TextBlock.text "Merges"
    ]
