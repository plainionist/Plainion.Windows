namespace Plainion.Windows.Text

open System.Threading
open NUnit.Framework
open Plainion.Windows.Controls.Text
open FsUnit

[<Feature>]
module ``Searching in a single document``=

    let create() =
        let editor = new RichTextEditor()
        editor.Document <- newDocument "f# is concise. F# is functional. F# is great ;-)"
        editor

    [<Apartment(ApartmentState.STA)>]
    [<Scenario>]
    module ``Given a multiple occurences of the searched text`` =
        [<When>]
        let ``<When> searching for all occurences <Then> all words should be highlighted``() =
            let editor = create()

            editor.Search("f#", SearchMode.All) |> should be True

            editor.Document |> shouldHighlight "f#" (count 3)

        [<When>]
        let ``<When> searching forward and backward <Then> correct offsets should be found``() =
            let editor = create()

            editor.Search("f#", SearchMode.Initial) |> should be True

            editor.Document |> shouldHighlight "f#" (at 1)
        
            editor.Search("f#", SearchMode.Next) |> should be True

            editor.Document |> shouldHighlight "f#" (at 18)

            editor.Search("f#", SearchMode.Next) |> should be True

            editor.Document |> shouldHighlight "f#" (at 36)

            editor.Search("f#", SearchMode.Previous) |> should be True

            editor.Document |> shouldHighlight "f#" (at 18)

            editor.Search("f#", SearchMode.Previous) |> should be True

            editor.Document |> shouldHighlight "f#" (at 1)

    [<Apartment(ApartmentState.STA)>]
    [<Scenario>]
    module ``Given zero occurences of a searched text`` =
        [<When>]
        let ``<When> searching <Then> nothing is found``() =
            let editor = create()

            editor.Search("java", SearchMode.Initial) |> should be False

            editor.Document |> shouldHighlight "java" (count 0)

[<Feature>]
module ``Searching in multiple documents``=
    open Plainion.IO.MemoryFS

    [<Apartment(ApartmentState.STA)>]
    [<Scenario>]
    module ``Given a NoteBook with documents`` =
        [<When>]
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
