using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Plainion.RI.Controls
{
    [Export]
    public partial class NoteBookView : UserControl
    {
        [ImportingConstructor]
        internal NoteBookView(NoteBookViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
