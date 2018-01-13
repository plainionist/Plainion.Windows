namespace Plainion.Windows.Specs.Controls.Text

open NUnit.Framework
open FsUnit
open Plainion.Windows.Controls.Text

[<Scenario>]
module ``Given a Document`` =
    open System.Windows.Documents
    open System

    [<Test>]
    let ``<When> newly created <Then> IsModified is true``() =
        let doc = new Document(fun () -> new FlowDocument())

        doc.IsModified |> should be True


    [<Test>]
    let ``<When> restored from store and body is not instantiated <Then> IsModified is false``() =
        let meta = new StoreItemMetaInfo<DocumentId>(new DocumentId(Guid.NewGuid()), DateTime.UtcNow, DateTime.UtcNow);
        let doc = new Document(meta, fun () -> new FlowDocument())

        doc.IsModified |> should be False

