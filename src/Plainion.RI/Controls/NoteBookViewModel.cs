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
            fs.File("/a/1").Create();
            fs.File("/b/2/x").Create();
            fs.File("/b/2/y").Create();

            DocumentStore = new FileSystemDocumentStore(fs);
        }

        public IDocumentStore DocumentStore { get; private set; }
    }
}
