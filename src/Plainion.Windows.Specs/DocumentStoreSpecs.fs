namespace Plainion.Windows.Specs.Controls.Text

open System.Threading
open NUnit.Framework
open Plainion.Windows.Controls.Text
open Plainion.Windows.Specs.Controls.Text
open System.IO
open System.Linq
open Plainion.IO.MemoryFS
open FsUnit

[<Apartment(ApartmentState.STA)>]
[<Spec>]
module ``Given a DocumentStore`` =
    open System

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

    [<Test>]
    let ``<When> changing documents <And> triggering save <Then> the modified documents are saved``() =
        let fs = new FileSystemImpl()
        let store = create fs

        store.Create("/doc1").Body |> addText "test1"
        store.Create("/sub/doc2").Body |> addText "test2"

        store.SaveChanges()
        let store = create fs

        store.Get("/doc1").Body |> addText "MORE"
        store.Get("/sub/doc2").Body |> addText "MUCH-MORE"
        
        store.SaveChanges()
        let store = create fs

        store.Get("/doc1").Body |> text |> should haveSubstring "MORE"
        store.Get("/sub/doc2").Body |> text |> should haveSubstring "MUCH-MORE"
        
    [<Test>]
    let ``<When> removing documents <And> triggering save <Then> the removal is made persistent``() =
        let fs = new FileSystemImpl()
        let store = create fs

        store.Create("/doc1").Body |> addText "test1"
        store.Create("/sub/doc2").Body |> addText "test2"

        store.SaveChanges()
        let store = create fs

        store.Root.Entries.Remove(store.Get("/doc1")) |> ignore
        store.Root.IsModified |> should be True
        
        store.SaveChanges()
        let store = create fs

        store.Root.Entries.OfType<Document>() |> List.ofSeq |> should haveLength 0
        


