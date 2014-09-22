using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FM4CC.WPFGUI.Console
{    public class TextBoxAppender : AppenderSkeleton
    {
        private TextBox _textBox;

        public TextBoxAppender(TextBox textBox)
        {
            _textBox = textBox;
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (_textBox == null)
            {
                return;
            }

            _textBox.Dispatcher.Invoke(delegate
            {
                _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                _textBox.ScrollToEnd();
            });

        }
    }

}
