using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SimpleBackup.ViewModels;

using Ookii.Dialogs.Wpf;
using SimpleBackup.Helpers;
using SimpleBackup.Services;
using SimpleBackup.Contracts.Services;

namespace SimpleBackup.Views;

public sealed partial class MainPage : Page
{
    private static async void SaveDirectories(IList<object> directories)
    {
        string[] selectedItems = Enumerable.Range(0, directories.Count).Select(i => directories[i].ToString()).ToArray();
        await App.GetService<ILocalSettingsService>().SaveSettingAsync("addedDirectories", selectedItems);
    }

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        var s = async () =>
        {
            var addedDirectories = await App.GetService<ILocalSettingsService>().ReadSettingAsync<string[]>("addedDirectories");
            
            foreach (var i in addedDirectories)
            {
                DirListView.Items.Add(i);
            }

            NoDirectoriesWarning.IsOpen = DirListView.Items.Count == 0;
        };

        s();

        DirListView.SelectionChanged += (object sender, SelectionChangedEventArgs args) => RemoveButton.IsEnabled = args.AddedItems.Count > 0;

        AddButton.Click += async (object sender, RoutedEventArgs args) =>
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog() { RootFolder = Environment.SpecialFolder.UserProfile };

            if ((bool) dialog.ShowDialog() && !DirListView.Items.Contains(dialog.SelectedPath))
            {
                DirListView.Items.Add(dialog.SelectedPath);
                NoDirectoriesWarning.IsOpen = false;

                SaveDirectories(DirListView.Items);
            }
        };

        RemoveButton.Click += async (object sender, RoutedEventArgs args) =>
        {
            bool multiSelect = DirListView.SelectedItems.Count > 1;

            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = (multiSelect ? "Main_DialogTitle_RemoveMultipleDirectories" : "Main_DialogTitle_RemoveSingleDirectory").GetLocalized(),
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                DefaultButton = ContentDialogButton.Secondary,
                Content = (multiSelect ? "Main_DialogContent_RemoveMultipleDirectories" : "Main_DialogContent_RemoveSingleDirectory").GetLocalized()
            };

            var result = await dialog.ShowAsync();


            if (result == ContentDialogResult.Primary)
            {
                for (int i = DirListView.SelectedItems.Count - 1; i >= 0; i--)
                {
                    DirListView.Items.Remove(DirListView.SelectedItems[i]);
                }

                if (DirListView.SelectedItems.Count == 0)
                {
                    NoDirectoriesWarning.IsOpen = true;
                }

                SaveDirectories(DirListView.Items);
            }
        };
        
    }
}
