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

        member val InOrderTo = "" with get, set
        member val AsA = "" with get, set
        member val IWantTo = "" with get, set

        /// Use to briefly describe the scenario. Use "inOrderTo", "AsA" and "IWantTo" alternatively if you 
        /// want to specify the scenario more precisely
        member val Caption = "" with get, set

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

