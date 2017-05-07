
# Plainion.Windows.Controls.Text.IDocumentStore

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Properties

### System.Collections.Generic.IEnumerable`1[[Plainion.Windows.Controls.Text.Document, Plainion.Windows, Version=1.23.0.0, Culture=neutral, PublicKeyToken=11fdbc7b87b9a0de]] All

Returns an iterator to all documents in the store.


## Methods

### Plainion.Windows.Controls.Text.Document Create(Plainion.Windows.Controls.Text.DocumentPath path)

### Plainion.Windows.Controls.Text.Document Get(Plainion.Windows.Controls.Text.DocumentPath path)

### void Save(Plainion.Windows.Controls.Text.Document document)

### void Delete(Plainion.Windows.Controls.Text.Document document)

### Plainion.Windows.Controls.Text.Document Move(Plainion.Windows.Controls.Text.Document source,Plainion.Windows.Controls.Text.DocumentPath target)

Returns a new document from new location. The given source document becomes invalid.

### System.Collections.Generic.IReadOnlyCollection`1[[Plainion.Windows.Controls.Text.Document, Plainion.Windows, Version=1.23.0.0, Culture=neutral, PublicKeyToken=11fdbc7b87b9a0de]] Search(System.String text)
