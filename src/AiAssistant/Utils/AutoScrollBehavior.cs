using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace AiAssistant.Utils
{
    /// <summary>
    /// Behavior that is used for AutoScrolling.
    /// </summary>
    public class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        private ScrollViewer? _scrollViewer;

        public static readonly StyledProperty<bool> AutoScrollProperty =
            AvaloniaProperty.Register<AutoScrollBehavior, bool>(nameof(AutoScroll));

        public bool AutoScroll
        {
            get => GetValue(AutoScrollProperty);
            set => SetValue(AutoScrollProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                _scrollViewer = AssociatedObject;
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged!;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged!;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (AutoScroll && e.ExtentDelta.Y != 0)
            {
                _scrollViewer?.ScrollToEnd();
            }
        }
    }
}
