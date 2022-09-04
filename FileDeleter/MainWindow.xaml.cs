using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace FileDeleter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void folderPathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextBox.Text = dialog.SelectedPath.Trim();
            }
        }

        private void updateFilenamesTextBox(List<string> filenames)
        {
            filenamesTextBox.Text = string.Join("\n", filenames);
        }

        private async Task deleteFilesImpl()
        {
            // Extract filenames.
            var filenames = filenamesTextBox.Text
                .Split('\n')
                .Select(filename => filename.Trim())
                .Where(filename => filename.Length > 0)
                .ToList();

            // Extract folder path.
            string folderPath = folderPathTextBox.Text.Trim();

            // Validate if folder path exists.
            if (! Directory.Exists(folderPath))
            {
                MessageBox.Show("Folder Path Doesn't Exist", Title);
                return; // Abort.
            }

            // Keep track of number of files successfully deleted.
            int deletedCount = 0;

            // Delete files.
            foreach (string filename in filenames)
            {
                await Task.Delay(1000); // DEBUG
                string filePath = Path.Combine(folderPath, filename);
                if (File.Exists(filePath))
                {
                    try
                    {
                        await Task.Run(() => File.Delete(filePath));
                        deletedCount++;
                        updateFilenamesTextBox(filenames.Where(fn => fn != filename).ToList());
                    } catch { }
                }
            }

            // Feedback
            MessageBox.Show($"{deletedCount}/{filenames.Count} file(s) deleted successfully",
                Title);
        }

        private async void eraseButton_Click(object sender, RoutedEventArgs e)
        {
            // Get user confirmation.
            var result = MessageBox.Show("Are you sure you want to delete?",
                Title,
                MessageBoxButton.YesNo);

            // If user has confirmed, proceed with deletion.
            if (result == MessageBoxResult.Yes)
            {
                await deleteFilesImpl();
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            folderPathTextBox.Clear();
            filenamesTextBox.Clear();
        }
    }
}
