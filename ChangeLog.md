## 2.7.0 - 2018-01-17

- FileSystemDocumentStore: save unloaded documents fixed

## 2.6.0 - 2018-01-17

- Document: IsModified handling fixed for not loaded bodies
- Plainion.Core updated (fix for File.Stream)

## 2.5.0 - 2017-12-27

- DocumentStore: default font changed to Calibri, size = 11pt
- AutoCorrection refactored to support multiple auto-corrections
- AutoCorrections added for
  - Arrows
  - MarkDown headlines
  - MarkDown bullet lists
- DocumentStore: format changed to Xaml to be able to store more semantical information

## 2.4.0 - 2017-08-27

- Fixed DataContext handling in TreeEditor which occationally occured
- NoteBook: fixed selection of newly created document

## 2.3.0 - 2017-08-26

- Plainion.Core updated
- Notebook: first document selected on startup
- DocumentStore: default font changed to Arial, size = 13
- Notebook: is readonly if no document is available initially

## 2.2.0 - 2017-08-22

- Mvvm.IPrintRequestAware: added from Plainion.Prism

## 2.1.0 - 2017-08-22

- Mvvm.DelegateCommand: non generic version released

## 2.0.0 - 2017-08-22

- Mvvm.DelegateCommand, Mvvm.BindableBase added to avoid referencing Prism library for small projects
- PropertyBinding.Unbind added
- Search functionality added to RichTextEditor & NotePad
- First version of NoteBook added

## 1.22.0 - 2016-09-06

- TreeEditor
  - context menu handling improved
  