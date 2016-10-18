﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unigram.Common;
using Unigram.Core.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Unigram.Controls.Views
{
    public sealed partial class SendPhotosView : ContentDialogBase, INotifyPropertyChanged
    {
        public ObservableCollection<StorageMedia> Items { get; set; }

        private StorageMedia _selectedItem;
        public StorageMedia SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        public SendPhotosView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Showing += InputPane_Showing;
            InputPane.GetForCurrentView().Hiding += InputPane_Hiding;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= InputPane_Showing;
            InputPane.GetForCurrentView().Hiding -= InputPane_Hiding;
        }

        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            var transform = TransformToVisual((FrameworkElement)Window.Current.Content);
            var point = transform.TransformPoint(new Point());

            var bottom = point.Y + ActualHeight;
            var difference = ((FrameworkElement)Window.Current.Content).ActualHeight - bottom;

            KeyboardPlaceholder.Height = new GridLength(args.OccludedRect.Height - difference);
        }

        private void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            KeyboardPlaceholder.Height = new GridLength(1, GridUnitType.Auto);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Hide(ContentDialogBaseResult.OK);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Hide(ContentDialogBaseResult.Cancel);
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Flip.Focus(FocusState.Programmatic);
            }
        }

        private async void More_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.AddRange(Constants.MediaTypes);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (Path.GetExtension(file.Name).Equals(".mp4"))
                    {
                        //Items.Add(new StorageVideo(file));
                    }
                    else
                    {
                        Items.Add(new StoragePhoto(file));
                    }
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                var index = Items.IndexOf(SelectedItem);
                var next = index > 0 ? Items[index - 1] : null;
                var previous = index < Items.Count - 1 ? Items[index + 1] : null;

                Items.Remove(SelectedItem);

                if (next != null)
                {
                    SelectedItem = next;
                }
                else
                {
                    SelectedItem = previous;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}