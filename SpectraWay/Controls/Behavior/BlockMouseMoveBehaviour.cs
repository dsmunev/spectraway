using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SpectraWay.Controls.Behavior
{
    public class BlockMouseMoveBehaviour : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }


    }
}