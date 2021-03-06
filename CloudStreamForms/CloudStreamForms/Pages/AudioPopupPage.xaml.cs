﻿using CloudStreamForms.Core;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public int IconSize { set; get; } = 25;
        public int BigIconSize { set; get; } = 60;

        public AudioPopupPage()
        {
            InitializeComponent();

            LowVol.Source = App.GetImageSource("AudioVolLow0.png");//App.GetImageSource(ChromeCastPage.RoundedPrefix + "_volume_mute_white_48dp.png");
            MaxVol.Source = App.GetImageSource("AudioVolLow3.png"); //App.GetImageSource(ChromeCastPage.RoundedPrefix + "_volume_up_white_48dp.png");
            VolSolider.Value = (double)MainChrome.Volume;
            VolSolider.ValueChanged += (o, e) => {
                MainChrome.Volume = ((float)e.NewValue);
            };
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            CloudStreamCore.print("BG");
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
    }
}