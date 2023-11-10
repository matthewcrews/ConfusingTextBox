module Aidos.UI.Save

open System
open Aidos.UI
open Avalonia.Controls
open Avalonia.Platform.Storage

let saveModelAs (window: Window) (session: Session) dispatch =
    task {
        let! defaultFolder =
            match session.FileType with
            | FileType.Temp ->
                window.StorageProvider.TryGetFolderFromPathAsync (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            | FileType.User ->
                let prevFolder = System.IO.Path.GetDirectoryName session.FilePath
                window.StorageProvider.TryGetFolderFromPathAsync prevFolder

        let options =
            FilePickerSaveOptions(
                Title = "Save Aidos File"
            )
        let aidosFilePickerType = FilePickerFileType "aidos"
        aidosFilePickerType.Patterns <- ["*.aidos"]
        options.FileTypeChoices <- [aidosFilePickerType]
        options.DefaultExtension <- "aidos"
        options.SuggestedStartLocation <- defaultFolder
        let! file = window.StorageProvider.SaveFilePickerAsync(options)
        dispatch (Msg.SaveAs file.Path.LocalPath)
    } |> ignore
