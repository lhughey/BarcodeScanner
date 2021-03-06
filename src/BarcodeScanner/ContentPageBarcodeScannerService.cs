﻿using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    public class ContentPageBarcodeScannerService : ContentPage, IBarcodeScannerService
    {
        protected ZXingScannerView ScannerView { get; }

        private bool HasResult { get; set; }

        private Result Result { get; set; }

        public ContentPageBarcodeScannerService()
        {
            ScannerView = new ZXingScannerView
            {
                AutomationId = "zxingScannerView",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Options = GetScanningOptions(),
            };

            ScannerView.OnScanResult += OnScanResult;

            var overlay = GetScannerOverlay();

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            grid.Children.Add(ScannerView);

            if(overlay != null)
            {
                grid.Children.Add(overlay);
            }

            // The root page of your application
            Content = grid;
        }

        public async Task<string> ReadBarcodeAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(this);
            await Task.Run(() => { while (!HasResult) { } });
            Application.Current.MainPage.Navigation.RemovePage(this);
            return Result?.Text;
        }

        public async Task<Result> ReadBarcodeResultAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(this);
            await Task.Run(() => { while (!HasResult) { } });
            Application.Current.MainPage.Navigation.RemovePage(this);
            return Result;
        }

        protected virtual string TopText() => string.Empty;

        protected virtual string BottomText() => string.Empty;

        protected virtual View GetScannerOverlay()
        {
            var overlay = new ZXingDefaultOverlay
            {
                TopText = TopText(),
                BottomText = BottomText(),
                ShowFlashButton = ScannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };

            overlay.FlashButtonClicked += (sender, e) =>
            {
                ScannerView.IsTorchOn = !ScannerView.IsTorchOn;
            };

            return overlay;
        }

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return new MobileBarcodeScanningOptions()
            {
                AutoRotate = false,
                TryHarder = true,
                UseNativeScanning = true,
                UseFrontCameraIfAvailable = false
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            HasResult = false;
            Result = null;
            ScannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HasResult = true;
            ScannerView.IsScanning = false;
        }

        private void OnScanResult(Result result)
        {
            Result = result;
            HasResult = true;
        }
    }
}
