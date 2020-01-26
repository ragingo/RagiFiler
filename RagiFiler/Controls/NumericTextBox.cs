using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RagiFiler.Controls
{
    class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            DataObject.AddPastingHandler(this, OnPaste);

            MaxLength = 9;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            string[] formats = e.SourceDataObject.GetFormats();

            if (formats.Length == 0)
            {
                e.Handled = true;
                return;
            }

            if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
            {
                e.Handled = true;
                return;
            }

            string text = e.SourceDataObject.GetData(DataFormats.UnicodeText, true) as string;
            if (!Regex.IsMatch(text, @"\d+"))
            {
                e.Handled = true;
                e.CancelCommand();
                return;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e.Text.Length == 0)
            {
                e.Handled = true;
                return;
            }

            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
                return;
            }

            base.OnPreviewTextInput(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            Text = Text.Replace(",", "");
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                e.Handled = true;
                return;
            }

            if (!int.TryParse(Text, out int value))
            {
                // 実装上来ないはず
                Debug.Assert(false);
                return;
            }

            Text = string.Format("{0:#,#}", value);

            base.OnLostFocus(e);
        }
    }
}
