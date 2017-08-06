[<AutoOpen>]
module Plainion.Windows.Specs.Controls.Text.TextAssertions

open System
open System.Windows.Documents
open NUnit.Framework
open Plainion.Windows.Controls.Text

type SpecAttribute() =
    inherit TestFixtureAttribute()

let equalsI (lhs:string) rhs = lhs.Equals(rhs, StringComparison.OrdinalIgnoreCase)

let searchResults doc = 
    let visitor = new FlowDocumentVisitor(fun e -> e.GetType() = typeof<Run>)
    visitor.Accept(doc)

    visitor.Results
    |> Seq.cast<Run>
    |> Seq.filter(fun e -> e.Background = RichTextEditor.SearchHighlightBrush)
    |> List.ofSeq

let count n (results:Run list) =
    Assert.That(results.Length, Is.EqualTo(n))

let at offset (results:Run list) =
    let head = results |> Seq.head

    let container = head.Parent :?> Block
    Assert.That(container.ContentStart.GetOffsetToPosition(head.ContentStart), Is.EqualTo(offset))

let shouldMatch searchText f (results:Run list) =
    Assert.That(results |> Seq.forall(fun x -> equalsI x.Text searchText), Is.True)
    results |> f

let shouldBeEmpty (results:Run list) =
    Assert.That(results |> Seq.isEmpty, Is.True)

let succeeded (x:bool) =
    Assert.That(x, Is.True)

let failed (x:bool) =
    Assert.That(x, Is.False)


