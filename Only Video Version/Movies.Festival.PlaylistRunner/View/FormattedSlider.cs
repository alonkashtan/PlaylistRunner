using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows;

namespace Movies.Festival.PlaylistRunner.View
{
    /// <summary>
    /// A Slider which provides a way to modify the 
    /// auto tooltip text by using a format string.
    /// </summary>
    public class FormattedSlider : Slider
    {
        private ToolTip _autoToolTip;
        
        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            this.FormatAutoToolTipContent();
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.FormatAutoToolTipContent();
        }

        private void FormatAutoToolTipContent()
        {
            if (this.AutoTooltipText!=null)
            {
                this.AutoToolTip.Content = AutoTooltipText;
            }
        }
        
        public object AutoTooltipText
        {
            get { return (object)GetValue(AutoTooltipTextProperty); }
            set { SetValue(AutoTooltipTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoTooltipText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoTooltipTextProperty =
            DependencyProperty.Register("AutoTooltipText", typeof(object), typeof(FormattedSlider), new UIPropertyMetadata(null));



        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField(
                        "_autoToolTip",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    _autoToolTip = field.GetValue(this) as ToolTip;
                }

                return _autoToolTip;
            }
        }
    }
}
