using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Plainion.Windows.Interactivity
{
    public class UpdateIsFocusedOnFocusLostBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.LostFocus += OnFocusLost;
        }

        private void OnFocusLost( object sender, RoutedEventArgs e )
        {
            FocusExtension.SetIsFocused( ( FrameworkElement )sender, false );
        }

        protected override void OnDetaching()
        {
            AssociatedObject.LostFocus -= OnFocusLost;

            base.OnDetaching();
        }
    }
}
