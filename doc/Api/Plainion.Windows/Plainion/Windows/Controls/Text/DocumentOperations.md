
# Plainion.Windows.Controls.Text.DocumentOperations

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Methods

### System.Windows.Documents.TextRange GetWordRange(System.Windows.Documents.TextPointer position)

Returns a TextRange covering a word containing or following this TextPointer.

#### Remarks

If this TextPointer is within a word or at start of word, the containing word range is returned. If this TextPointer is between two words, the following word range is returned. If this TextPointer is at trailing word boundary, the following word range is returned.

### System.Collections.Generic.IEnumerable`1[[System.Windows.Documents.TextRange, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]] Search(System.Windows.Documents.FlowDocument document,System.Windows.Documents.TextPointer currentPosition,System.String searchText,Plainion.Windows.Controls.Text.SearchMode mode)
