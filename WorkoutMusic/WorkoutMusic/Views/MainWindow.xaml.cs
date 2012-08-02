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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using WorkoutMusic.ViewModels;

namespace WorkoutMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object dummyNode = null;
        private MainWindowVM _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowVM();
            this.DataContext = _viewModel;
        }

        public string SelectedImagePath { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem(); 
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }

            this.SelectedImagePath = Properties.Settings.Default.LastDirectory;
            NavigateToFolder(this.SelectedImagePath);
            _viewModel.FolderSelected(this.SelectedImagePath);
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }

            _viewModel.FolderSelected(SelectedImagePath);
            //show user selected path
            //MessageBox.Show(SelectedImagePath);

        }

        private void NavigateToFolder(string path)
        {
            // split the path into directories
            string[] directories = path.Split('\\');
            var items = foldersItem.Items;
            TreeViewItem match = null;

            for (int i = 0; i < directories.Length; i++)
            {
                // Most directories don't need the trailing slash
                string directory = directories[i];

                // But the first drive does (i.e. C:\)
                if (i == 0)
                    directory = string.Concat(directory, "\\");

                match = items.Cast<TreeViewItem>().FirstOrDefault(item =>
                    {
                        string header = (string)item.Header;
                        return header == directory;
                    });

                if (match == null)
                    return;


                match.IsExpanded = true;
                items = match.Items;
            }

            // Focus the last match
            match.Focus();
            match.BringIntoView();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LastDirectory = this.SelectedImagePath;
            Properties.Settings.Default.Save();
        }

        private void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.Columns.Remove(dg.Columns.First(c => ((string)c.Header) == "File"));
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            ((MainWindowVM)this.DataContext).PlayCommand.Execute();
        }
    }
}
