﻿using GitCommander;
using GitItGUI.UI.Images;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace GitItGUI.UI.Screens.RepoTabs
{
    /// <summary>
    /// Interaction logic for ChangesTab.xaml
    /// </summary>
    public partial class ChangesTab : UserControl
    {
        public ChangesTab()
        {
            InitializeComponent();

			var p = previewTextBox.Document.Blocks.FirstBlock as Paragraph;
			p.LineHeight = 1;
			previewTextBox.Document.PageWidth = 1920;
		}

		public void Refresh()
		{
			stagedChangesListBox.Items.Clear();
			unstagedChangesListBox.Items.Clear();
			foreach (var fileState in RepoScreen.singleton.repoManager.GetFileStates())
			{
				var item = new ListBoxItem();
				item.Tag = fileState;

				var button = new Button();
				button.Width = 20;
				button.Height = 20;
				button.HorizontalAlignment = HorizontalAlignment.Left;
				button.Background = new SolidColorBrush(Colors.Transparent);
				button.BorderBrush = new SolidColorBrush(Colors.LightGray);
				button.BorderThickness = new Thickness(1);
				var image = new Image();
				image.Source = ImagePool.GetImage(fileState.state);
				button.Content = image;
				button.Tag = fileState;

				var label = new Label();
				label.Margin = new Thickness(20, 0, 0, 0);
				label.Content = fileState.filename;
				label.ContextMenu = new ContextMenu();
				var openFileMenu = new MenuItem();
				openFileMenu.Header = "Open file";
				openFileMenu.ToolTip = fileState.filename;
				openFileMenu.Click += OpenFileMenu_Click;
				var openFileLocationMenu = new MenuItem();
				openFileLocationMenu.Header = "Open file location";
				openFileLocationMenu.ToolTip = fileState.filename;
				openFileLocationMenu.Click += OpenFileLocationMenu_Click;
				label.ContextMenu.Items.Add(openFileMenu);
				label.ContextMenu.Items.Add(openFileLocationMenu);

				var grid = new Grid();
				grid.Children.Add(button);
				grid.Children.Add(label);
				item.Content = grid;
				if (fileState.IsStaged())
				{
					button.Click += StagedButton_Click;
					stagedChangesListBox.Items.Add(item);
				}
				else
				{
					button.Click += UnstagedButton_Click;
					unstagedChangesListBox.Items.Add(item);
				}
			}
		}

		private void ToolButton_Click(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			button.ContextMenu.IsOpen = true;
		}

		private void OpenFileMenu_Click(object sender, RoutedEventArgs e)
		{
			var item = (MenuItem)sender;
			RepoScreen.singleton.repoManager.OpenFile((string)item.ToolTip);
		}

		private void OpenFileLocationMenu_Click(object sender, RoutedEventArgs e)
		{
			var item = (MenuItem)sender;
			RepoScreen.singleton.repoManager.OpenFileLocation((string)item.ToolTip);
		}

		private void UnstagedButton_Click(object sender, RoutedEventArgs e)
		{
			var fileState = (FileState)((Button)sender).Tag;
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.StageFile(fileState)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to un-stage file");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void StagedButton_Click(object sender, RoutedEventArgs e)
		{
			var fileState = (FileState)((Button)sender).Tag;
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.UnstageFile(fileState)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to stage file");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void stageAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.StageAllFiles()) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to stage files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void unstageAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.UnstageAllFiles()) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to un-stage files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void stageSelectedMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// create list of selected files
			var fileStates = new List<FileState>();
			foreach (var item in unstagedChangesListBox.Items)
			{
				var i = (ListBoxItem)item;
				var fileState = (FileState)i.Tag;
				if (i.IsSelected) fileStates.Add(fileState);
			}

			// process selection
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.StageFileList(fileStates)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to un-stage files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void unstageSelectedMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// create list of selected files
			var fileStates = new List<FileState>();
			foreach (var item in stagedChangesListBox.Items)
			{
				var i = (ListBoxItem)item;
				var fileState = (FileState)i.Tag;
				if (i.IsSelected) fileStates.Add(fileState);
			}

			// process selection
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.UnstageFileList(fileStates)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to un-stage files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void revertAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.RevertAll()) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to revert files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void revertSelectedMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// create list of selected files
			var fileStates = new List<FileState>();
			foreach (var item in unstagedChangesListBox.Items)
			{
				var i = (ListBoxItem)item;
				var fileState = (FileState)i.Tag;
				if (i.IsSelected) fileStates.Add(fileState);
			}

			// process selection
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.RevertFileList(fileStates)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to revert files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void cleanupAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.DeleteUntrackedUnstagedFiles(true)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to cleanup files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void cleanupSelectedMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// create list of selected files
			var fileStates = new List<FileState>();
			foreach (var item in unstagedChangesListBox.Items)
			{
				var i = (ListBoxItem)item;
				var fileState = (FileState)i.Tag;
				if (i.IsSelected) fileStates.Add(fileState);
			}

			// process selection
			MainWindow.singleton.ShowProcessingOverlay();
			RepoScreen.singleton.repoManager.dispatcher.InvokeAsync(delegate()
			{
				if (!RepoScreen.singleton.repoManager.DeleteUntrackedUnstagedFiles(fileStates, true)) MainWindow.singleton.ShowMessageOverlay("Error", "Failed to cleanup files");
				MainWindow.singleton.HideProcessingOverlay();
			});
		}

		private void ProcessPreview(ListBoxItem item)
		{
			previewTextBox.Document.Blocks.Clear();
			var fileState = (FileState)item.Tag;
			var delta = RepoScreen.singleton.repoManager.GetQuickViewData(fileState);
			if (delta == null)
			{
				var range = new TextRange(previewTextBox.Document.ContentEnd, previewTextBox.Document.ContentEnd);
				range.Text = "<<< Unsuported Preview Type >>>";
			}
			else if (delta.GetType() == typeof(string))
			{
				using (var stream = new MemoryStream())
				using (var writer = new StreamWriter(stream))
				using (var reader = new StreamReader(stream))
				{
					// write all data into stream
					writer.Write((string)delta);
					writer.Flush();
					writer.Flush();
					stream.Position = 0;

					// read lines and write formatted blocks
					void WritePreviewText(string text, SolidColorBrush blockColor)
					{
						var end = previewTextBox.Document.ContentEnd;
						var range = new TextRange(end, end);
						range.Text = text;
						range.ApplyPropertyValue(TextElement.ForegroundProperty, blockColor);
					}
						
					int blockMode = 0;
					string line = null, normalBlock = null, addBlock = null, subBlock = null, secBlock = null;
					void CheckBlocks(bool isFinishMode)
					{
						if ((blockMode != 0 || (isFinishMode && blockMode == 0)) && !string.IsNullOrEmpty(normalBlock))
						{
							WritePreviewText(normalBlock, Brushes.Black);
							normalBlock = "";
						}
						else if ((blockMode != 1 || (isFinishMode && blockMode == 1)) && !string.IsNullOrEmpty(addBlock))
						{
							WritePreviewText(addBlock, Brushes.Green);
							addBlock = "";
						}
						else if ((blockMode != 2 || (isFinishMode && blockMode == 2)) && !string.IsNullOrEmpty(subBlock))
						{
							WritePreviewText(subBlock, Brushes.Red);
							subBlock = "";
						}
						else if ((blockMode != 3 || (isFinishMode && blockMode == 3)) && !string.IsNullOrEmpty(secBlock))
						{
							WritePreviewText(secBlock, Brushes.DarkOrange);
							secBlock = "";
						}
					}

					do
					{
						line = reader.ReadLine();
						if (string.IsNullOrEmpty(line))
						{
							CheckBlocks(true);
							continue;
						}

						if (line[0] == '+')
						{
							CheckBlocks(false);
							blockMode = 1;
							addBlock += line + '\r';
						}
						else if (line[0] == '-')
						{
							CheckBlocks(false);
							blockMode = 2;
							subBlock += line + '\r';
						}
						else if (line[0] == '*')
						{
							CheckBlocks(false);
							blockMode = 3;
							secBlock += "\r\r" + line + '\r';
						}
						else
						{
							CheckBlocks(false);
							blockMode = 0;
							normalBlock += line + '\r';
						}
					}
					while (line != null);
				}
			}
		}

		private void unstagedChangesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = unstagedChangesListBox.SelectedItem;
			if (item != null)
			{
				stagedChangesListBox.SelectedItem = null;
				ProcessPreview((ListBoxItem)item);
			}
			else
			{
				if (stagedChangesListBox.SelectedItem == null) previewTextBox.Document.Blocks.Clear();
			}
		}

		private void stagedChangesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = stagedChangesListBox.SelectedItem;
			if (item != null)
			{
				unstagedChangesListBox.SelectedItem = null;
				ProcessPreview((ListBoxItem)item);
			}
			else
			{
				if (unstagedChangesListBox.SelectedItem == null) previewTextBox.Document.Blocks.Clear();
			}
		}
	}
}
