[<AutoOpen>]
module Core

open System
open NUnit.Framework
open System.Windows.Threading

[<AutoOpen>]
module BDDLite =
    type FeatureAttribute() =
        inherit Attribute()

        member val InOrderTo = "" with get, set
        member val AsA = "" with get, set
        member val IWantTo = "" with get, set


    type ScenarioAttribute() =
        inherit TestFixtureAttribute()

    type WhenAttribute() =
        inherit TestAttribute()

let DoEventsSync () =
    let exitFrame (frame:obj) =
        let dispatcherFrame = frame :?> DispatcherFrame
        dispatcherFrame.Continue <- false
        null :> obj

    let frame = new DispatcherFrame()
    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitFrame), frame) |> ignore
    Dispatcher.PushFrame(frame)

