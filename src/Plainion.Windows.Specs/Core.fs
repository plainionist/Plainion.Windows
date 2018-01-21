[<AutoOpen>]
module Core

open System
open NUnit.Framework
open System.Windows.Threading

type FeatureAttribute() =
    inherit Attribute()

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

