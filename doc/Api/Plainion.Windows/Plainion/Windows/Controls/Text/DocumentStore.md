
# Plainion.Windows.Controls.Text.DocumentStore

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Constructors

### Constructor()


## Properties

### Plainion.Windows.Controls.Text.Folder Root


## Methods

### Plainion.Windows.Controls.Text.Document Get(Plainion.Windows.Controls.Text.DocumentId id)

### void SaveChanges()

### System.Collections.Generic.IReadOnlyCollection`1[[Plainion.Windows.Controls.Text.Document, Plainion.Windows, Version=1.23.0.0, Culture=neutral, PublicKeyToken=11fdbc7b87b9a0de]] Search(System.String text)

### Plainion.Windows.Controls.Text.Document GetCore(Plainion.Windows.Controls.Text.DocumentId id)

### Plainion.Windows.Controls.Text.Folder GetRootCore()

### void SaveCore(Plainion.Windows.Controls.Text.Folder folder)

Only handle the folder itself (title, associated documents). Children will be handled separately.

### void SaveCore(Plainion.Windows.Controls.Text.Document document)

Only handle the document. Related folder will be handled separately, if necessary.

### void DeleteCore(Plainion.Windows.Controls.Text.DocumentId id)

Only handle the document. Related folder will be handled separately.

### void DeleteCore(Plainion.Windows.Controls.Text.FolderId id)

Only handle the folder itself (title, associated documents). Children will be handled separately.
