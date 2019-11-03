using System;
using System.Windows;

namespace Home_6
{
    public partial class GoToForm : Window
    {
        public GoToForm()
        {
            InitializeComponent();
            GoToData.CurLine = -1;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lMaxLine.Content = $"Line number (1 - {GoToData.MaxLine}):";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int num = Convert.ToInt32(txtResul.Text) - 1;
                if (num >= 0 && num < GoToData.MaxLine)
                    GoToData.CurLine = num;
            }
            catch { }

            Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}