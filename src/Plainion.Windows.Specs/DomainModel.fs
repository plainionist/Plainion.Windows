[<AutoOpen>]
module Plainion.Windows.Specs.Controls.Text.DomainModel

open System
open System.Windows.Documents
open NUnit.Framework
open Plainion.Windows.Controls.Text
open FsUnit
open System.Windows.Threading

type FeatureAttribute() =
    inherit Attribute()

type ScenarioAttribute() =
    inherit TestFixtureAttribute()

type WhenAttribute() =
    inherit TestAttribute()

let equalsI (lhs:string) rhs = lhs.Equals(rhs, StringComparison.OrdinalIgnoreCase)

let count n (results:Run list) =
    results |> should haveLength n

let at offset (results:Run list) =
    let head = results |> Seq.head

    let container = head.Parent :?> Block
    container.ContentStart.GetOffsetToPosition(head.ContentStart) |> should equal offset

let shouldHighlight searchText f (doc:FlowDocument) =
    let visitor = new FlowDocumentVisitor(fun e -> e.GetType() = typeof<Run>)
    visitor.Accept(doc)

    let results =
        visitor.Results
        |> Seq.cast<Run>
        |> Seq.filter(fun e -> e.Background = RichTextEditor.SearchHighlightBrush)
        |> List.ofSeq

    results |> Seq.forall(fun x -> equalsI x.Text searchText) |> should be True
    results |> f

let newDocument text =
    new FlowDocument(new Paragraph(new Run(text)))

let addText text (doc:FlowDocument) =
    doc.Blocks.Add(new Paragraph(new Run(text)))

let text (doc:FlowDocument) =
    let range = new TextRange(doc.ContentStart, doc.ContentEnd)
    range.Text

let DoEventsSync () =
    let exitFrame (frame:obj) =
        let dispatcherFrame = frame :?> DispatcherFrame
        dispatcherFrame.Continue <- false
        null :> obj

    let frame = new DispatcherFrame()
    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new DispatcherOperationCallback(exitFrame), frame) |> ignore
    Dispatcher.PushFrame(frame)

