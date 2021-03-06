
# Plainion.Windows.Controls.Text.DocumentOperations

**Namespace:** Plainion.Windows.Controls.Text

**Assembly:** Plainion.Windows


## Methods

### System.Windows.Documents.TextRange Content(System.Windows.Documents.FlowDocument self)

Returns a TextRange from ContentStart to ContentEnd.

### System.Windows.Documents.TextPointer GetPointerFromCharOffset(System.Windows.Documents.TextRange range,System.Int32 charOffset)

Returns a text pointer for the given character offset.

### System.Windows.Documents.TextRange GetWordAt(System.Windows.Documents.TextPointer position)

Returns a TextRange covering a word containing or following this TextPointer.

#### Remarks

If this TextPointer is within a word or at start of word, the containing word range is returned. If this TextPointer is between two words, the following word range is returned. If this TextPointer is at trailing word boundary, the preceding word range is returned.

### System.Collections.Generic.IEnumerable`1[[System.Windows.Documents.TextRange, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]] GetWords(System.Windows.Documents.TextRange range)

### System.Collections.Generic.IEnumerable`1[[System.Windows.Documents.TextRange, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]] GetLines(System.Windows.Documents.TextRange range)

### System.Windows.Documents.TextRange GetLineAt(System.Windows.Documents.TextPointer pos)

### System.Collections.Generic.IEnumerable`1[[System.Windows.Documents.TextRange, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]] Search(System.Windows.Documents.FlowDocument document,System.Windows.Documents.TextPointer currentPosition,System.String searchText,Plainion.Windows.Controls.Text.SearchMode mode)
