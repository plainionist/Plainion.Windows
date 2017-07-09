using System.ComponentModel.Composition;
using System.Windows.Documents;
using Plainion.IO.MemoryFS;
using Plainion.Windows.Controls.Text;

namespace Plainion.RI.Controls
{
    [Export]
    class NoteBookViewModel
    {
        public NoteBookViewModel()
        {
            var fs = new FileSystemImpl();

            DocumentStore = new FileSystemDocumentStore(fs.Directory("/x"));
            DocumentStore.Initialize();

            DocumentStore.Create("/User documentation/Installation").Body.AddText("Installation");
            DocumentStore.Create("/User documentation/Getting started").Body.AddText("Getting started");
            DocumentStore.Create("/User documentation/FAQ").Body.AddText("Frequenty Asked Questions");
            DocumentStore.Create("/Developer documentation/Getting started").Body.AddText("Getting started as a developer");
            DocumentStore.Create("/Developer documentation/HowTos/MVC with F#").Body.AddText("MVC with F#");
            DocumentStore.Create("/Developer documentation/HowTos/WebApi with F#").Body.AddText("WebApi with F#");

            // just to test that it works :)
            DocumentStore.SaveChanges();

            DocumentStore = new FileSystemDocumentStore(fs.Directory("/x"));
            DocumentStore.Initialize();
        }

        public FileSystemDocumentStore DocumentStore { get; private set; }
    }

    static class FlowDocumentExtensions
    {
        public static FlowDocument AddText(this FlowDocument self, string text)
        {
            self.Blocks.Add(new Paragraph(new Run(text)));
            return self;
        }
    }
}

