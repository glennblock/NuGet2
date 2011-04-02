﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NuGet.VisualStudio.Resources;

namespace NuGet.VisualStudio {
    public partial class ProductUpdateBar : UserControl {

        private readonly IProductUpdateService _productUpdateService;

        public ProductUpdateBar(IProductUpdateService productUpdateService) {
            InitializeComponent();

            Debug.Assert(productUpdateService != null);
            _productUpdateService = productUpdateService;
            _productUpdateService.UpdateAvailable += OnUpdateAvailable;
        }

        private void OnUpdateAvailable(object sender, ProductUpdateAvailableEventArgs e) {
            // this event handler will be invoked on background thread. Has to use Dispatcher to show update bar.
            Dispatcher.BeginInvoke(new Action<Version, Version>(ShowUpdateBar), e.CurrentVersion, e.NewVersion);
        }

        private void OnUpdateLinkClick(object sender, RoutedEventArgs e) {
            HideUpdateBar();

            // invoke with priority as Background so that our window is closed first before the Update method is called.
            Dispatcher.BeginInvoke(new Action(_productUpdateService.Update), DispatcherPriority.Background);
        }

        private void OnDeclineUpdateLinkClick(object sender, RoutedEventArgs e) {
            HideUpdateBar();
            _productUpdateService.DeclineUpdate();
        }

        public void ShowUpdateBar(Version currentVersion, Version newVersion) {
            if (IsVisible) {
                UpdateMessage.Text = String.Format(CultureInfo.CurrentCulture, VsResources.AskForUpdateMessage, newVersion, currentVersion);
                UpdateBar.Visibility = Visibility.Visible;
            }
        }

        private void HideUpdateBar() {
            UpdateBar.Visibility = Visibility.Collapsed;
        }
    }
}