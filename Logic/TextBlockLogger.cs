using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LanguageTxtToXlf.Logic
{
    public class TextBlockLogger
    {
        TextBlock _target;
        public TextBlockLogger(TextBlock target)
        {
            _target = target;
        }

        public void Log(string logEntry)
        {
            _target.Dispatcher.BeginInvoke(new Action(() => { _target.Text = logEntry + "\n" + _target.Text;  }));
        }
    }
}
