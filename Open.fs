module Aidos.UI.Open

open System
open Aidos.UI
open Avalonia.Controls
open Avalonia.Platform.Storage

let openFile (window: Window) (session: Session) dispatch =
    task {
        let! defaultFolder =
            match session.FileType with
            | FileType.Temp ->
                window.StorageProvider.TryGetFolderFromPathAsync (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            | FileType.User ->
                let prevFolder = System.IO.Path.GetDirectoryName session.FilePath
                window.StorageProvider.TryGetFolderFromPathAsync prevFolder
        let options =
            FilePickerOpenOptions(
                Title = "Open Aidos File"
            )
        let aidosFilePickerType = FilePickerFileType "aidos"
        aidosFilePickerType.Patterns <- ["*.aidos"]
        options.SuggestedStartLocation <- defaultFolder
        options.AllowMultiple <- false
        let! files = window.StorageProvider.OpenFilePickerAsync options
        if files.Count = 1 then
            dispatch (Msg.Open files[0].Path.LocalPath)
    } |> ignore
