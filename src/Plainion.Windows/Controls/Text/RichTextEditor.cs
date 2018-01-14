using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Plainion.Windows.Controls.Text.AutoCorrection;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    /// <summary>
    /// Extends RichtTextBox by auto-completion.
    /// See "AutoCorrection" namespace for more details
    /// </summary>
    public class RichTextEditor : RichTextBox
    {
        internal static Brush SearchHighlightBrush = Brushes.Yellow;

        private AutoCorrectionTrigger? myAutoCorrectionTrigger;

        // TextPointers that track the range covering content where words are added.
        private int mySelectionStartPosition;

        public RichTextEditor()
        {
            AddHandler(PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), handledEventsToo: true);

            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPasted);

            // required to get hyperlinks working
            IsDocumentEnabled = true;

            // allows indent of list items with tab/shift-tab
            AcceptsTab = true;

            ToggleBulletsCommand = new DelegateCommand(() => OnToggleList(EditingCommands.ToggleBullets));
            ToggleNumberingommand = new DelegateCommand(() => OnToggleList(EditingCommands.ToggleNumbering));
        }

        public ICommand ToggleBulletsCommand { get; private set; }
        public ICommand ToggleNumberingommand { get; private set; }

        private void OnToggleList(RoutedUICommand toggleList)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                toggleList.Execute(null, this);

                var visitor = new FlowDocumentVisitor(x => x is List);
                visitor.Accept(Document);
                foreach (var list in visitor.Results.Cast<List>())
                {
                    list.Margin = new Thickness(0, 0, 0, 0);
                    list.Padding = new Thickness(30, 0, 0, 0);
                }
            }));
        }

        public static readonly DependencyProperty AutoCorrectionProperty = DependencyProperty.Register("AutoCorrection",
            typeof(AutoCorrectionTable), typeof(RichTextEditor), new PropertyMetadata(new AutoCorrectionTable()));

        public AutoCorrectionTable AutoCorrection
        {
            get { return (AutoCorrectionTable)GetValue(AutoCorrectionProperty); }
            set { SetValue(AutoCorrectionProperty, value); }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back && e.Key != Key.Space && e.Key != Key.Return)
            {
                return;
            }

            if (!Selection.IsEmpty)
            {
                Selection.Text = string.Empty;
            }

            if (e.Key == Key.Space || e.Key == Key.Return)
            {
                myAutoCorrectionTrigger = e.Key == Key.Space ? AutoCorrectionTrigger.Space : AutoCorrectionTrigger.Return;

                // we get "next insertion position backwards" to "skip" the space or enter and want to remember "last content" typed
                var start = Selection.Start.GetNextInsertionPosition(LogicalDirection.Backward);
                if (start != null)
                {
                    mySelectionStartPosition = Document.ContentStart.GetOffsetToPosition(start);

                    // auto correction detection will be done in OnTextChanged()

                    if (e.Key == Key.Return)
                    {
                        // we have to trigger auto correction explicitly here
                        ApplyAutoCorrection();
                    }
                }
            }
            else // Key.Back
            {
                // Remember caretPosition with forward gravity. This is necessary since we might remove highlighting elements preceeding caretPosition 
                // and after deletion current caretPosition (with backward gravity) will follow content preceeding the highlighting. 
                // We want to remember content following the highlighting to set new caret position at.

                var newCaretPosition = Selection.Start.GetPositionAtOffset(0, LogicalDirection.Forward);

                if (AutoCorrection.Undo(Selection.Start).Success)
                {
                    // Update selection, since we deleted a auto correction element and caretPosition was at that auto correction's end boundary.
                    Selection.Select(newCaretPosition, newCaretPosition);
                    e.Handled = true;
                }
            }
        }

        private void OnPasted(object sender, DataObjectPastingEventArgs e)
        {
            myAutoCorrectionTrigger = AutoCorrectionTrigger.CopyAndPaste;
            mySelectionStartPosition = Document.ContentStart.GetOffsetToPosition(CaretPosition);

            // auto correction detection will be done in OnTextChanged()
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyAutoCorrection();
        }

        private void ApplyAutoCorrection()
        {
            if (myAutoCorrectionTrigger == null || Document == null)
            {
                return;
            }

            TextChanged -= OnTextChanged;

            var start = Document.ContentStart.GetPositionAtOffset(mySelectionStartPosition);
            if (start == null)
            {
                start = Document.ContentEnd;
            }

            var input = new AutoCorrectionInput(new TextRange(start, CaretPosition), myAutoCorrectionTrigger.Value);
            input.Editor = this;
            var result = AutoCorrection.Apply(input);

            if (result.Success && result.CaretPosition != null)
            {
                CaretPosition = result.CaretPosition;
            }

            TextChanged += OnTextChanged;

            myAutoCorrectionTrigger = null;
        }

        public bool Search(string searchText, SearchMode mode)
        {
            if (Document == null)
            {
                return false;
            }

            ClearSearch();

            var results = DocumentOperations.Search(Document, Selection.Start, searchText, mode).ToList();

            foreach (var result in results)
            {
                Selection.Select(result.Start, result.End);
                Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SearchHighlightBrush);
            }

            return results.Count > 0;
        }

        public void ClearSearch()
        {
            new TextRange(Document.ContentStart, Document.ContentEnd)
                .ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
        }
    }
}
