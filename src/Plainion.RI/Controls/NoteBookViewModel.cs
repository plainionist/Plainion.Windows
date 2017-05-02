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
            DocumentStore = new FileSystemDocumentStore(fs);
        }

        public IDocumentStore DocumentStore { get; private set; }
    }
}
