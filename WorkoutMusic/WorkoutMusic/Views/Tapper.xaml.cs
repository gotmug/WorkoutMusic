using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkoutMusic.ViewModels;

namespace WorkoutMusic.Views
{
    /// <summary>
    /// Interaction logic for Tapper.xaml
    /// </summary>
    public partial class Tapper : Window
    {
        public Tapper(string filename)
        {
            InitializeComponent();
            this.DataContext = new TapperVM(filename);
            this.tapButton.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((TapperVM)this.DataContext).Save();
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((TapperVM)this.DataContext).CleanUp();
        }

        private void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            ((TapperVM)this.DataContext).SelectedBPM = int.Parse(tb.Text);
        }
    }
}
