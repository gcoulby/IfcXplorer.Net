using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace IfcXplorer.CustomControls
{
    internal class IfcTextEditor : TextEditor
    {
        public IfcTextEditor()
        {
            Document.UndoStack.SizeLimit = 0;
            TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Color.FromRgb(38, 139, 210));

            MouseHover += TextEditor_MouseHover;
            MouseHoverStopped += TextEditor_MouseHoverStopped;
            PreviewMouseDown += TextEditor_PreviewMouseDown;
        }

        internal void SetTheme(bool isDarkTheme)
        {
            var syntax = isDarkTheme ? Properties.Resources.SyntaxDark : Properties.Resources.SyntaxLight;
            
            using (var sr = new MemoryStream(syntax))
            {
                using (var reader = new XmlTextReader(sr))
                {
                    SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }


        /// <summary>
        /// Selects a line based on the entity label provided
        /// used to quick navigation to a particular entity
        /// </summary>
        internal void SelectLine(string entityLabel)
        {
            if (string.IsNullOrEmpty(entityLabel)) return;

            var offset = GetOffsetByEntityLabel(entityLabel);
            if (offset <= -1) return;

            var line = Document.GetLineByOffset(offset);
            SelectText(line.Offset, line.Length);
        }

        /// <summary>
        /// Get's the caret offset of a particular
        /// entity label.
        /// </summary>
        /// <param name="entityLabel">The entity label to search for</param>
        /// <returns>Caret offset</returns>
        private int GetOffsetByEntityLabel(string entityLabel)
        {
            if (entityLabel.StartsWith("#"))
            {
                entityLabel = entityLabel.Substring(1);
            }

            var offset = Document.Text.IndexOf("#" + entityLabel + "=", StringComparison.Ordinal);
            return offset;
        }

        /// <summary>
        /// Highlights the a string of a specified size
        /// at the given offset. 
        /// </summary>
        /// <param name="offset">The caret position where the string starts</param>
        /// <param name="length">The length of the string</param>
        internal void SelectText(int offset, int length)
        {
            var line = Document.GetLineByOffset(offset);
            var lineNumber = line.LineNumber;
            SelectionStart = offset;
            SelectionLength = length;
            CaretOffset = offset + length;
            ScrollTo(lineNumber, (offset - line.Offset));
        }

        /// <summary>
        /// Gets the current caret position
        /// </summary>
        /// <returns>The current caret position</returns>
        internal int GetCaretPosition()
        {
            return CaretOffset;
        }

        /// <summary>
        /// Gets the total number of lines written to
        /// the output text box.
        /// </summary>
        /// <returns>Total number of lines</returns>
        internal int GetNumberOfLines()
        {
            return Document.LineCount;
        }

        /// <summary>
        /// Gets the total number of IFC entities
        /// </summary>
        /// <returns>Total number of IFC entities</returns>
        internal int GetNumberOfInstances()
        {
            var matches = Regex.Matches(Document.Text, "[#][0-9]+[=]");
            return matches.Count;
        }

        /// <summary>
        /// This tool tip is used to display entity definitions
        /// for the 'Peek Definition' functionality (Shows definition
        /// line when hovering over a reference tag).
        /// </summary>
        private readonly ToolTip _toolTip = new ToolTip();

        /// <summary>
        /// The entity label used for peek definition feature.
        /// </summary>
        private string _entityLabel;

        /// <summary>
        /// Checks to see if the mouse is hovering over a reference tag
        /// and returns the entity definition for displaying in the tool tip
        /// </summary>
        /// <param name="point">The current position of the mouse</param>
        /// <returns>Entity definition string</returns>
        private string GetMatchedLineFromPoint(Point point)
        {
            var pos = GetPositionFromPoint(point);
            if (pos == null) return null;

            var tvp = (TextViewPosition)pos;

            var line = Document.GetLineByNumber(tvp.Line);
            var lineText = Document.GetText(line.Offset, line.Length);

            var cursorPos = tvp.Column - 1;
            var matches = Regex.Matches(lineText, @"([#][0-9]+)(?![0-9=])");
            foreach (Match match in matches)
            {
                if (cursorPos <= match.Index || cursorPos >= match.Index + match.Length) continue;
                var matchedLine = Document.GetLineByOffset(GetOffsetByEntityLabel(match.Value));
                _entityLabel = match.Value;

                return Document.GetText(matchedLine.Offset, matchedLine.Length);
            }
            return null;
        }

        /// <summary>
        /// Mouse hover event used for checking to see if the mouse
        /// is hovering over a reference tag. If it is it then displays 
        /// the peek definition tool tip with the contents of the 
        /// entity definition.
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">event arguments</param>
        private void TextEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var matchedLineText = GetMatchedLineFromPoint(e.GetPosition(this));

            _toolTip.PlacementTarget = this; // required for property inheritance
            _toolTip.Content = matchedLineText;

            if (!string.IsNullOrEmpty(matchedLineText))
            {
                _toolTip.IsOpen = true;
                Mouse.OverrideCursor = Cursors.Hand;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Mouse event for determining whether the mouse has
        /// stopped hovering over the previous tag and closes
        /// the peek definition tool tip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _toolTip.IsOpen = false;
            Mouse.OverrideCursor = Cursors.Arrow;
            _entityLabel = null;
        }

        /// <summary>
        /// This is the mouse event for creating links for hashtags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectLine(_entityLabel);
            if (_entityLabel != null)
                e.Handled = true;
            _entityLabel = null;
        }
    }
}
