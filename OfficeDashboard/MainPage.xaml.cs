using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WeatherNet;
using WeatherNet.Clients;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OfficeDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DispatcherTimer mFrequentTimer = new DispatcherTimer();
        private readonly DispatcherTimer mWeatherTimer = new DispatcherTimer();
        private Settings mSettings;
        private DailyQuotesService mDailyQuotesService;
        private TeamCityService mTeamCityService;
        readonly ApplicationDataContainer mLocalSettings;
        private string mLastWeatherIcon = null;
        private byte mLastHourQuoteRefreshed = 0;

        public MainPage()
        {
            InitializeComponent();
            mLocalSettings = ApplicationData.Current.LocalSettings;
            
            LoadSettings();
            UpdateServices();

            CarparkImageControl.ImageOpened +=
                (sender, args) => Debug.WriteLine($"Carpark image loaded {DateTime.Now.ToString("G")}");

            mFrequentTimer.Interval = TimeSpan.FromSeconds(30);
            mFrequentTimer.Tick += FrequentTimerOnTick;
            mFrequentTimer.Start();

            mWeatherTimer.Interval = TimeSpan.FromMinutes(10);
            mWeatherTimer.Tick += WeatherTimerOnTick;
            mWeatherTimer.Start();

            RefreshDailyQuote();
            RefreshCarparkImage();
            RefreshWeather();
            RefreshTeamCityBuilds();
        }

        public List<TeamCityBuild> TeamCityBuilds { get; set; }

        public void LoadSettings()
        {
            mSettings = new Settings();
            mSettings.WeatherApiKey = mLocalSettings.Values["WeatherApiKey"]?.ToString();
            mSettings.GarageCamImageUrl = mLocalSettings.Values["GarageCamImageUrl"]?.ToString();
            mSettings.MotorwayTrafficApiKey = mLocalSettings.Values["MotorwayTrafficApiKey"]?.ToString();
            mSettings.TeamCityServerUrl = mLocalSettings.Values["TeamCityServerUrl"]?.ToString();
            mSettings.DailyQuotesUrl = mLocalSettings.Values["DailyQuotesUrl"]?.ToString();
        }

        public void UpdateServices()
        {
            if (!string.IsNullOrWhiteSpace(mSettings.TeamCityServerUrl))
            {
                mTeamCityService = new TeamCityService(mSettings.TeamCityServerUrl);
            }

            //mTrafficService = new TrafficService("https://infoconnect1.highwayinfo.govt.nz/ic/jbi/", mSettings.MotorwayTrafficApiKey);
            if (!string.IsNullOrWhiteSpace(mSettings.DailyQuotesUrl))
            {
                mDailyQuotesService = new DailyQuotesService(mSettings.DailyQuotesUrl);
            }

            ClientSettings.ApiKey = mSettings.WeatherApiKey;
        }

        private void WeatherTimerOnTick(object sender, object e)
        {
            RefreshWeather();
        }

        private async void RefreshWeather()
        {
            try
            {
                var weatherTask = await CurrentWeather.GetByCoordinatesAsync(-36.8643566, 174.7577578, "english", "metric");
                var weather = weatherTask.Item;

                if (!string.IsNullOrWhiteSpace(weather.Icon) && (weather.Icon != mLastWeatherIcon))
                {
                    var currentHour = DateTime.Now.Hour;
                    string weatherPrefix;
                    if (currentHour >= 6 && currentHour <= 18)
                    {
                        weatherPrefix = "d";
                    }
                    else
                    {
                        weatherPrefix = "n";
                    }

                    weather.Icon = weather.Icon.Substring(0, weather.Icon.Length - 1) + weatherPrefix;

                    var uri = new Uri($"http://openweathermap.org/img/w/{weather.Icon}.png");
                    var bitmap = new BitmapImage(uri);
                    WeatherIconImage.Source = bitmap;
                    mLastWeatherIcon = weather.Icon;
                }

                WeatherTitleTextBlock.Text = weather.Date.ToLocalTime().ToString("ddd MMM dd");
                WeatherTempTextBlock.Text = $"{Math.Round(weather.Temp)}\u00B0C";
                WeatherSummaryTextBlock.Text = weather.Description.ToTitleCase();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void FrequentTimerOnTick(object sender, object o)
        {
            RefreshCarparkImage();
            RefreshTeamCityBuilds();
            RefreshDailyQuote();
        }

        public void RefreshCarparkImage()
        {
            if (!string.IsNullOrEmpty(mSettings.GarageCamImageUrl))
            {
                CarparkImageControlClone.Source = CarparkImageControl.Source; //to prevent flickering
                CarparkImageControl.Source = null; //Specifically unload the image to prevent caching on RasPi

                var bitmapImage = new BitmapImage(new Uri(mSettings.GarageCamImageUrl, UriKind.Absolute)) {CreateOptions = BitmapCreateOptions.IgnoreImageCache};
                CarparkImageControl.Source = bitmapImage;
            }
        }

        public async void RefreshTeamCityBuilds()
        {

            if (!string.IsNullOrEmpty(mSettings.TeamCityServerUrl))
            {
                try
                {
                    TeamCityBuilds = await mTeamCityService.GetUniqueProjectsMostRecentBuilds("AutomatedTesting");
                    TeamCityBuildListView.ItemsSource = TeamCityBuilds;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        public async void RefreshDailyQuote()
        {
            var hour = DateTime.Now.Hour;
         
            if (!string.IsNullOrEmpty(mSettings.DailyQuotesUrl) && mLastHourQuoteRefreshed != hour)
            {
                try
                {
                    var quoteTuple = await mDailyQuotesService.GetDailyQuote();
                    var quote = quoteTuple.Item1;

                    DailyQuoteTextBox.Text = $"\"{quote}\"";
                    DailyQuoteAuthorTextBox.Text = $"-{quoteTuple.Item2}";
                    mLastHourQuoteRefreshed = (byte)hour;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        private async void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsDialog();
            var result = await settingsDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                mSettings = settingsDialog.Settings;
                UpdateServices();
                RefreshCarparkImage();
                RefreshWeather();
            }
        }

        private void RefreshAll_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshDailyQuote();
            RefreshCarparkImage();
            RefreshWeather();
            RefreshTeamCityBuilds();

        }
    }
}
