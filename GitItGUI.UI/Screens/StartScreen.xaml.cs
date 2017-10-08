﻿using GitItGUI.Core;
using GitItGUI.UI.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GitItGUI.UI.Screens
{
	/// <summary>
	/// Interaction logic for StartUserControl.xaml
	/// </summary>
	public partial class StartScreen : UserControl
	{
		public static StartScreen singleton;

		public StartScreen()
		{
			singleton = this;
			InitializeComponent();

			updateImage.Visibility = Visibility.Hidden;
			updateImage.MouseUp += UpdateImage_MouseUp;
		}

		public void Init()
		{
			RefreshHistory();
		}

		public void EnabledOutOfDate()
		{
			updateImage.Visibility = Visibility.Visible;
		}

		private void RefreshHistory()
		{
			historyListBox.Items.Clear();
			foreach (var repo in AppManager.repositories)
			{
				var item = new ListBoxItem();
				item.Content = Path.GetFileName(repo);
				item.ToolTip = repo;

				item.HorizontalContentAlignment = HorizontalAlignment.Center;
				item.MouseDoubleClick += Item_MouseDoubleClick;

				var menuItem = new MenuItem();
				menuItem.Header = "Open folder path";
				menuItem.ToolTip = repo;
				menuItem.Click += MenuItem_Click;
				item.ContextMenu = new ContextMenu();
				item.ContextMenu.Items.Add(menuItem);

				historyListBox.Items.Add(item);
			}
		}

		public void Refresh()
		{
			if (MainWindow.singleton.Dispatcher.CheckAccess())
			{
				RefreshHistory();
			}
			else
			{
				MainWindow.singleton.Dispatcher.InvokeAsync(delegate()
				{
					RefreshHistory();
				});
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var item = (MenuItem)sender;
			string repo = (string)item.ToolTip;
			if (!Tools.OpenFolderLocation(repo))
			{
				AppManager.RemoveRepoFromHistory(repo);
				RefreshHistory();
			}
		}

		private void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var item = (ListBoxItem)sender;
			string repo = (string)item.ToolTip;
			RepoScreen.singleton.OpenRepo(repo);
		}

		private void openButton_Click(object sender, RoutedEventArgs e)
		{
			if (PlatformUtils.SelectFolder(out string folderPath))
			{
				RepoScreen.singleton.OpenRepo(folderPath);
			}
		}

		private void cloneButton_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void createButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			SettingsScreen.singleton.Setup();
			MainWindow.singleton.Navigate(SettingsScreen.singleton);
		}

		private void UpdateImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			using (var process = Process.Start("https://github.com/reignstudios/Git-It-GUI/releases"))
			{
				process.WaitForExit();
			}
		}
	}
}
