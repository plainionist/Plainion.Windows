namespace Plainion.Windows.Specs.Controls.Text

open System.Threading
open System.Windows.Documents
open NUnit.Framework
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text

[<Apartment(ApartmentState.STA)>]
[<TestFixture>]
module ``Given text with multiple occurences`` =

    let create() =
        let editor = new RichTextEditor()
        editor.Document <- new FlowDocument(new Paragraph(new Run("f# is concise. F# is functional. F# is great ;-)")))
        editor

    [<Test>]
    let ``<When> searching for all occurences <Then> all words should be highlighted``() =
        let editor = create()

        editor.Search("f#", SearchMode.All) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (count 3)

    [<Test>]
    let ``<When> searching forward and backward <Then> correct offsets should be found``() =
        let editor = create()

        editor.Search("f#", SearchMode.Initial) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (at 1)
        
        editor.Search("f#", SearchMode.Next) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (at 18)

        editor.Search("f#", SearchMode.Next) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (at 36)

        editor.Search("f#", SearchMode.Previous) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (at 18)

        editor.Search("f#", SearchMode.Previous) |> succeeded

        editor.Document |> searchResults |> shouldMatch "f#" (at 1)

[<Apartment(ApartmentState.STA)>]
[<TestFixture>]
module ``Given text with zero occurences`` =

    let create() =
        let editor = new RichTextEditor()
        editor.Document <- new FlowDocument(new Paragraph(new Run("f# is concise. F# is functional. F# is great ;-)")))
        editor

    [<Test>]
    let ``<When> searching <Then> nothing is found``() =
        let editor = create()

        editor.Search("java", SearchMode.Initial) |> failed

        editor.Document |> searchResults |> shouldBeEmpty

