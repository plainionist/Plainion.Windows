
# Plainion.Windows.Controls.SelectFolderDialog

**Namespace:** Plainion.Windows.Controls

**Assembly:** Plainion.Windows

Provides WPF wrapper for System.Windows.Forms.FolderBrowserDialog. It derives from
*See:* T:System.Windows.Forms.CommonDialog
in order to support same handling as
*See:* T:System.Windows.Forms.OpenFileDialog
and
*See:* T:System.Windows.Forms.SaveFileDialog
.


## Constructors

### Constructor()


## Properties

### System.String Description

Gets or sets the descriptive text displayed above the tree view control in the dialog box.

### System.Environment+SpecialFolder RootFolder

Gets or sets the root folder where the browsing starts from.

### System.String SelectedPath

Gets or sets the path selected by the user.

### System.Boolean ShowNewFolderButton

Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box.


## Methods

### void Reset()

Resets properties to their default values.

### System.Boolean RunDialog(System.IntPtr hwndOwner)
