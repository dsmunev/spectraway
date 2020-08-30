using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Controls.Behavior
{
    public class SpectrometerInteractivityBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.PreviewKeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is Grid))
            {
                if (e.Key == Key.Escape)
                {
                    Keyboard.ClearFocus();
                }
                e.Handled = false;
                return;

            }
            if (e.Key == Key.Escape || e.Key == Key.F1)
            {
                var context = ((FrameworkElement) sender).DataContext as ExperimentViewModel;
                if (context != null)
                {
                    context.IsRealTimeSpectrometerData = e.Key == Key.F1;
                    e.Handled = false;
                    
                }
                
            }

        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.PreviewKeyDown -= AssociatedObject_KeyDown;
            base.OnDetaching();
        }


    }
}