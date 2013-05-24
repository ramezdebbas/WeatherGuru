using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherGuru.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherGuru
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SmartDispatcher.Initialize(this.Dispatcher);
            this.SizeChanged += MainPage_SizeChanged;
        }

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SnappedBox.Visibility = e.NewSize.Width < 400 ? Visibility.Visible : Visibility.Collapsed;
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            bool afound = false;
            bool sfound = false;
            bool pfound = false;
            foreach (var command in args.Request.ApplicationCommands.ToList())
            {
                if (command.Label == "About")
                {
                    afound = true;
                }
                if (command.Label == "Settings")
                {
                    sfound = true;
                }
                if (command.Label == "Policy")
                {
                    pfound = true;
                }
            }
            if (!afound)
                args.Request.ApplicationCommands.Add(new SettingsCommand("s", "About", (p) => { cfoAbout.IsOpen = true; }));
            //if (!sfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Settings", (p) => { cfoSettings.IsOpen = true; }));
            //if (!pfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Policy", (p) => { cfoPolicy.IsOpen = true; }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", OpenPrivacyPolicy));
        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            var uri = new Uri("http://www.thatslink.com/privacy-statment/ ");
            await Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
