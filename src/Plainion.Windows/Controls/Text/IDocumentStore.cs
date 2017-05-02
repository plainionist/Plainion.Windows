using System.Collections.Generic;

namespace Plainion.Windows.Controls.Text
{
    public interface IDocumentStore
    {
        Document Create(DocumentPath path);

        Document Get(DocumentPath path);

        void Save(Document document);
        
        void Delete(Document document);
        
        /// <summary>
        /// Returns a new document from new location. The given source document becomes invalid.
        /// </summary>
        Document Move(Document source, DocumentPath target);

        IReadOnlyCollection<Document> Search(string text);
    }
}
