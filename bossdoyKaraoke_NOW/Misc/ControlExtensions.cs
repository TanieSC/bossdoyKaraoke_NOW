using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bossdoyKaraoke_NOW.Misc
{
    static class ControlExtensions
    {
        static public void ExecuteAsync(this Control control, Action code)
        {
            if (control.IsDisposed)
                return;

            try
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(code);
                    return;
                }
            }
            catch { }

            code.Invoke();
        }

        static public void Execute(this Control control, Action code)
        {
            if (control.IsDisposed)
                return;

            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(code);
                    return;
                }
            }
            catch { }

            code.Invoke();
        }
    }
}
