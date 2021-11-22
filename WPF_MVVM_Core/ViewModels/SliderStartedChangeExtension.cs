using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPF_MVVM_Core.ViewModels
{
    /// <summary>
    /// Класс позволяющий отслеживать начало изменения слайдера не нарушая паттерн MVVM
    /// </summary>
    internal class SliderStartedChangeExtension
    {
        public static readonly DependencyProperty DragStartedCommandProperty = DependencyProperty.RegisterAttached(
            "DragStartedCommand",
            typeof(ICommand),
            typeof(SliderStartedChangeExtension),
            new PropertyMetadata(default(ICommand), OnDragStartedCommandChanged));

        private static void OnDragStartedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = d as Slider;
            if (slider == null)
            {
                return;
            }

            if (e.NewValue is ICommand)
            {
                slider.Loaded += SliderOnLoadedStarted;
            }
        }

        private static void SliderOnLoadedStarted(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider == null)
            {
                return;
            }
            slider.Loaded -= SliderOnLoadedStarted;

            Track track = slider.Template.FindName("PART_Track", slider) as Track;
            if (track == null)
            {
                return;
            }
            track.Thumb.DragCompleted += (dragCompletedSender, dragCompletedArgs) =>
            {
                ICommand command = GetDragStartedCommand(slider);
                command.Execute(null);
            };
        }

        public static void SetDragStartedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(DragStartedCommandProperty, value);
        }

        public static ICommand GetDragStartedCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(DragStartedCommandProperty);
        }
    }
}