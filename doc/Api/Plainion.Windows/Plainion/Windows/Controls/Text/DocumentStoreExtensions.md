
# Plainion.Windows.Controls.Text.DocumentStoreExtensions

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Methods

### Plainion.Windows.Controls.Text.Folder Folder(Plainion.Windows.Controls.Text.DocumentStore self,Plainion.Windows.Controls.Text.DocumentId id)

Returns the folder containing the given document.

### Plainion.Windows.Controls.Text.Folder Folder(Plainion.Windows.Controls.Text.Folder self,Plainion.Windows.Controls.Text.DocumentId id)

Returns the folder containing the given document.

### System.Collections.Generic.IEnumerable`1[[Plainion.Windows.Controls.Text.IStoreItem, Plainion.Windows, Version=2.9.0.0, Culture=neutral, PublicKeyToken=null]] Enumerate(Plainion.Windows.Controls.Text.Folder self)

### Plainion.Windows.Controls.Text.Document CreateDocument(System.String title)

Creates a new document with default settings.

### Plainion.Windows.Controls.Text.Document Create(Plainion.Windows.Controls.Text.DocumentStore self,Plainion.Windows.Controls.Text.Folder folder,System.String title)

Creates a new document under the given folder with the given title

### Plainion.Windows.Controls.Text.Document TryGet(Plainion.Windows.Controls.Text.DocumentStore self,System.String path)

Returns the document at the given path. Path elements are separated with "/". The last path element is the title of the document.

#### Return value

the document if found, null otherwise

### Plainion.Windows.Controls.Text.Document Get(Plainion.Windows.Controls.Text.DocumentStore self,System.String path)

Returns the document at the given path. Path elements are separated with "/". The last path element is the title of the document.

#### Exceptions

*System.ArgumentException*
If there is no document at that path

### Plainion.Windows.Controls.Text.Document Create(Plainion.Windows.Controls.Text.DocumentStore self,System.String path)

Creates a new document at the given path. Path elements are separated with "/". The last path element is the title of the document.
