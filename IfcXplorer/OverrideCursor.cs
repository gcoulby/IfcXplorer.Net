using System;
using System.Windows.Input;

namespace IfcXplorer
{
    /// <summary>
    /// Used to override the cursor to fix a issue that
    /// causes cursors to disappear
    /// </summary>
    public class OverrideCursor : IDisposable
    {
        internal OverrideCursor(Cursor changeToCursor)
        {
            Mouse.OverrideCursor = changeToCursor;
        }
        #region IDisposable Members
        public void Dispose()
        {
            Mouse.OverrideCursor = null;
        }
        #endregion
    }
}
