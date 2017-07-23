using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    class Snapshot
    {
        public Snapshot(Folder root)
        {
            Documents = root.Enumerate()
                .OfType<Document>()
                .Select(doc=>doc.Id)
                .ToList();
        }

        public IReadOnlyCollection<DocumentId> Documents { get; private set; }
    }
}
