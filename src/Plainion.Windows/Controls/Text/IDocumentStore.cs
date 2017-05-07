using System;
using System.Collections.Generic;

namespace Plainion.Windows.Controls.Text
{
    // TODO: add documents via path "/a/b/c[2]"
    public interface IDocumentStore
    {
        Document Get(DocumentId id);

        void Save(Document document);

        void Delete(DocumentId id);

        Folder Root { get; }

        IReadOnlyCollection<Document> Search(string text);
    }
}
