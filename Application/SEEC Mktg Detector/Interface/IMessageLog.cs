using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Seec.Marketing
{
    public interface IMessageLog
    {
        TextBox MessageTextBox { get; }
        void AddMessageLog(string category, string message, TextBox messageBox);
    }
}
