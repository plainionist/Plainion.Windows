
# Plainion.Windows.Controls.Text.DocumentStore

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Constructors

### Constructor()


## Properties

### Plainion.Windows.Controls.Text.Folder Root


## Methods

### void SaveChanges()

### System.Collections.Generic.IReadOnlyCollection`1[[Plainion.Windows.Controls.Text.Document, Plainion.Windows, Version=2.5.0.0, Culture=neutral, PublicKeyToken=null]] Search(System.String text)

### Plainion.Windows.Controls.Text.Folder GetRoot()

### void SaveRoot()

Called during SaveChanges() if at least one folder has changed. Full folder hierarchy has to be saved with this single call.

### Plainion.Windows.Controls.Text.Document GetCore(Plainion.Windows.Controls.Text.DocumentId id)

### void Save(Plainion.Windows.Controls.Text.Document document)

Only handle the document. Related folder will be handled separately, if necessary.

### void Delete(Plainion.Windows.Controls.Text.DocumentId id)

Only handle the document. Related folder will be handled separately.
