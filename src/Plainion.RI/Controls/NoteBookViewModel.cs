using System.ComponentModel.Composition;
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

            DocumentStore.Create("/User documentation/Installation");
            DocumentStore.Create("/User documentation/Getting started");
            DocumentStore.Create("/User documentation/FAQ");
            DocumentStore.Create("/Developer documentation/Getting started");
            DocumentStore.Create("/Developer documentation/HowTos/MVC with F#");
            DocumentStore.Create("/Developer documentation/HowTos/WebApi with F#");

            // just to test that it works :)
            DocumentStore.SaveChanges();

            DocumentStore = new FileSystemDocumentStore(fs.Directory("/x"));
            DocumentStore.Initialize();
        }

        public FileSystemDocumentStore DocumentStore { get; private set; }
    }
}
