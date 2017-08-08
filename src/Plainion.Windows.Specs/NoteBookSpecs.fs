namespace Plainion.Windows.Specs.Controls.Text

open System
open System.Threading
open FsUnit
open NUnit.Framework
open Plainion.IO.MemoryFS
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text

[<Apartment(ApartmentState.STA)>]
[<Spec>]
module ``Given an empty NoteBook`` =

    [<Test>]
    let ``<When> adding a DocumentStore with documents and folders <Then> navigation page is building a tree accordingly``() =
        let notebook = new NoteBook()

        let fs = new FileSystemImpl()
        let store = new FileSystemDocumentStore(fs.Directory("/x"))
        store.Initialize()

        store.Create("/d1").Body |> addText "d1"
        store.Create("/s1/d2").Body |> addText "d2"
        store.Create("/s2/d3").Body |> addText "d3"

        notebook.DocumentStore <- store

        notebook.myNavigation.Root.Children |> should haveCount 3
        notebook.myNavigation.Root.Children.[1].Children |> should haveCount 1
        notebook.myNavigation.Root.Children.[2].Children |> should haveCount 1

[<Apartment(ApartmentState.STA)>]
[<Spec>]
module ``Given a NoteBook with documents`` =

    [<Test>]
    let ``<When> deleting a navigation node <Then> DocumentStore is updated accordingly``() =
        let notebook = new NoteBook()

        let fs = new FileSystemImpl()
        let store = new FileSystemDocumentStore(fs.Directory("/x"))
        store.Initialize()

        store.Create("/d1").Body |> addText "d1"
        store.Create("/s1/d2").Body |> addText "d2"

        notebook.DocumentStore <- store

        let node = notebook.myNavigation.Root.Children.[1].Children.[0]
        notebook.myNavigation.DeleteCommand.Execute(node)

        store.TryGet("/s1/d2") |> should be Null

    [<Test>]
    let ``<When> selecting a navigation node <Then> respective document is shown in NotePad``() =
        let notebook = new NoteBook()

        let fs = new FileSystemImpl()
        let store = new FileSystemDocumentStore(fs.Directory("/x"))
        store.Initialize()

        store.Create("/d1").Body |> addText "d1"
        store.Create("/s1/d2").Body |> addText "d2"

        notebook.DocumentStore <- store

        let node = notebook.myNavigation.Root.Children.[1].Children.[0]
        node.IsSelected <- true

        notebook.myNotePad.Document |> should equal (store.Get("/s1/d2").Body)

    [<Test>]
    let ``<When> searching in all documents <Then> all matching documents are shown in search results``() =
        let notebook = new NoteBook()

        let fs = new FileSystemImpl()
        let store = new FileSystemDocumentStore(fs.Directory("/x"))
        store.Initialize()

        store.Create("/d1").Body |> addText "we love f# more than c#"
        store.Create("/s1/d2").Body |> addText "this is about Ruby only"
        store.Create("/s1/d3").Body |> addText "this is about F# as well"

        notebook.DocumentStore <- store

        notebook.myNavigation.SearchText <- "f#"

        let searchResults = notebook.myNavigation.SearchResults |> List.ofSeq

        searchResults |> should contain (store.Get("/d1"))
        searchResults |> should not' (contain (store.Get("/s1/d2")))
        searchResults |> should contain (store.Get("/s1/d3"))


