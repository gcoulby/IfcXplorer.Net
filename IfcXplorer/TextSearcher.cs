using System;
using IfcXplorer.CustomControls;

namespace IfcXplorer
{
    /// <summary>
    /// Used to find text within the AvalonEdit text editor
    /// </summary>
    internal class TextSearcher
    {
        private readonly IfcTextEditor _textEditor;
        private string _previousSearch = "";

        internal TextSearcher(IfcTextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        internal void SearchForward(string searchString)
        {
            var secondAttempt = false;
            while (true)
            {
                if (searchString != _previousSearch || secondAttempt)
                {
                    _textEditor.CaretOffset = 0;
                    _previousSearch = searchString;
                }
                var offset = _textEditor.GetCaretPosition();
                var text = _textEditor.Document.Text;

                var subString = text.Substring(offset);

                var index = StringPos(subString, searchString);

                var newOffset = index + offset;
                if (index == -1)
                {
                    if (secondAttempt) break;
                    //Reached end or no match found - go back to the start and search again
                    secondAttempt = true;
                    continue;
                }

                _textEditor.SelectText(newOffset, searchString.Length);
                break;
            }
        }

        internal static int StringPos(string text, string searchString, bool searchForward = true)
        {
            return searchForward
                ? text.ToLower().IndexOf(searchString.ToLower(), StringComparison.Ordinal)
                : text.ToLower().LastIndexOf(searchString.ToLower(), StringComparison.Ordinal);
        }
    }
}