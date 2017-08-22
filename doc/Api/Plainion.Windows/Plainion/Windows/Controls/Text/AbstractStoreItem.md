
# Plainion.Windows.Controls.Text.AbstractStoreItem`1

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Constructors

### Constructor(Plainion.Windows.Controls.Text.StoreItemMetaInfo`1[TId] meta)


## Properties

###  Id

### System.String Title

### System.Boolean IsModified

### System.DateTime Created

### System.DateTime LastModified


## Methods

### System.Boolean SetProperty(T& storage,T value,System.String propertyName)

### void MarkAsModified()

### System.Boolean CheckModified()

### System.IDisposable SuppressChangeTracking()

Used during deserialization to suppress change tracking.
