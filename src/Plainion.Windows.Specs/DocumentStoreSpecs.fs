namespace Plainion.Windows.Specs.Controls.Text

open System.Threading
open NUnit.Framework
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text
open System.IO
open Plainion.IO.MemoryFS
open FsUnit

[<Apartment(ApartmentState.STA)>]
[<Spec>]
module ``Given a DocumentStore`` =

    let create (fs:FileSystemImpl) = 
        let store = new FileSystemDocumentStore(fs.Directory("/x"))
        store.Initialize()
        store

    [<Test>]
    let ``<When> adding documents <And> triggering save <Then> the new documents are saved``() =
        let fs = new FileSystemImpl()
        let store = create fs

        let doc = store.Create("/doc1")
        doc.Body |> addText "test1"

        doc.IsModified |> should be True
        store.Root.IsModified |> should be True

        store.SaveChanges()
        let store = create fs

        store.Root.IsModified |> should be False
        store.Get("/doc1") |> should not' (be Null)

        store.Create("/sub/doc2").Body |> addText "test2"

        store.SaveChanges()
        let store = create fs

        store.Get("/doc1") |> should not' (be Null)
        store.Get("/sub/doc2") |> should not' (be Null)


