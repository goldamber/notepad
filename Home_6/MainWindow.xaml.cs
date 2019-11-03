using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Home_6
{
    public partial class MainWindow : Window
    {
        string str = "";
        string filename = "";

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();

            CommandBinding binding = new CommandBinding();
            binding.Command = ApplicationCommands.Stop;
            binding.Executed += new ExecutedRoutedEventHandler(WordCommand_Executed);
            CommandBindings.Add(binding);

            binding = new CommandBinding();
            binding.Command = ApplicationCommands.Properties;
            binding.Executed += new ExecutedRoutedEventHandler(GoToCommand_Executed);
            CommandBindings.Add(binding);

            binding = new CommandBinding();
            binding.Command = ApplicationCommands.CorrectionList;
            binding.CanExecute += CmdFind_CanExecute;
            binding.Executed += new ExecutedRoutedEventHandler(CmdFindNext_Executed);
            CommandBindings.Add(binding);

            ApplicationCommands.Stop.InputGestures.Clear();
            InputGesture key = new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W");
            ApplicationCommands.Stop.InputGestures.Add(key);

            ApplicationCommands.Properties.InputGestures.Clear();
            key = new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G");
            ApplicationCommands.Properties.InputGestures.Add(key);

            ApplicationCommands.CorrectionList.InputGestures.Clear();
            key = new KeyGesture(Key.F3, ModifierKeys.None, "F3");
            ApplicationCommands.CorrectionList.InputGestures.Add(key);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            str = "";
            filename = "";
            txtMain.Text = "";
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "Text Files(*.txt)|*.txt|All(*.*)|*" };

            if (dialog.ShowDialog() == true)
            {
                filename = dialog.FileName;
                str = File.ReadAllText(dialog.FileName);
                txtMain.Text = str;
            }
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (filename != "")
                File.WriteAllText(filename, txtMain.Text);
        }
        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog { Filter = "Text Files(*.txt)|*.txt|All(*.*)|*" };

            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, txtMain.Text);
        }
        private void CmdDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int num = txtMain.SelectionStart;
            txtMain.Text = txtMain.Text.Substring(0, txtMain.SelectionStart) + txtMain.Text.Substring(txtMain.SelectionStart + txtMain.SelectionLength);
            txtMain.SelectionStart = num;
        }
        private void CmdReplace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int num = txtMain.SelectionStart;
            txtMain.Text = txtMain.Text.Replace(txtMain.SelectedText, Clipboard.GetText());
            txtMain.SelectionStart = num;
        }
        private void CmdFindNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
             if (txtMain.Text.Substring(txtMain.SelectionStart + txtMain.SelectionLength).Contains(txtFind.Text))
                 txtMain.Select(txtMain.SelectionStart + txtMain.SelectionLength + txtMain.Text.Substring(txtMain.SelectionStart + txtMain.SelectionLength).IndexOf(txtFind.Text), txtFind.Text.Length);
             else
                 MessageBox.Show("There is no string.", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void CmdFormat_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog();
            fd.ShowColor = true;
            System.Windows.Forms.DialogResult dr = fd.ShowDialog();
            
            if (dr != System.Windows.Forms.DialogResult.Cancel)
            {
                txtMain.FontFamily = new FontFamily(fd.Font.Name);
                txtMain.FontSize = fd.Font.Size * 96.0 / 72.0;
                txtMain.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                txtMain.Foreground = new SolidColorBrush(Color.FromArgb(fd.Color.A, fd.Color.R, fd.Color.G, fd.Color.B));
                if (fd.Font.Underline)
                    txtMain.TextDecorations.Add(TextDecorations.Underline);
                if (fd.Font.Strikeout)
                    txtMain.TextDecorations.Add(TextDecorations.Strikethrough);
                txtMain.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (txtMain.Text != str && filename != "");
            }
            catch { }
        }
        private void Binding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (txtMain.SelectedText != "" && txtFind.Text != "");
        }
        private void CmdCopy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (txtMain.SelectedText != "");
            }
            catch { }     
        }       
        private void CmdReplace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (txtMain.SelectedText != "") && Clipboard.ContainsText();
            }
            catch { }     
        }       
        private void CmdPaste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsText();
        }
        private void CmdFind_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = (txtFind.Text != "");
            }
            catch { }
        }

        private void WordCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WordWrapItem.IsChecked = !WordWrapItem.IsChecked;
            MenuItem_Click(WordWrapItem, null);
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            txtMain.TextWrapping = (sender as MenuItem).IsChecked? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
        private void GoToCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GoToItem_Click(null, null);
        }
        private void GoToItem_Click(object sender, RoutedEventArgs e)
        {
            GoToData.MaxLine = txtMain.LineCount;

            GoToForm gt = new GoToForm();
            gt.ShowDialog();

            if (GoToData.CurLine >= 0)
            {
                txtMain.SelectionStart = txtMain.GetCharacterIndexFromLineIndex(GoToData.CurLine);
                txtMain.SelectionLength = txtMain.GetLineLength(GoToData.CurLine);
                txtMain.CaretIndex = txtMain.SelectionStart;
                txtMain.ScrollToLine(GoToData.CurLine);
            }
        }

        private void txtMain_SelectionChanged(object sender, RoutedEventArgs e)
        {
            lLine.Content = $"Ln {txtMain.GetLineIndexFromCharacterIndex(txtMain.CaretIndex) + 1}";
            lCol.Content = $"Col {txtMain.CaretIndex - txtMain.GetCharacterIndexFromLineIndex(txtMain.GetLineIndexFromCharacterIndex(txtMain.CaretIndex)) + 1}";
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Authors...\nProgram...", "About");
        }
        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (txtMain.Text.Contains(txtFind.Text))
                    txtMain.Select(txtMain.Text.IndexOf(txtFind.Text), txtFind.Text.Length);
                else
                    MessageBox.Show("There is no string.", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtMain.Text != str)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    if (filename == "")
                        SaveAsCommand_Executed(null, null);
                    else
                        SaveCommand_Executed(null, null);
                }
            }
        }
    }
}