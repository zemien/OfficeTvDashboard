using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private readonly DispatcherTimer mCarparkAndBuildTimer = new DispatcherTimer();
        private readonly DispatcherTimer mWeatherTimer = new DispatcherTimer();
        private readonly DispatcherTimer mQuotesTimer = new DispatcherTimer();
        private Settings mSettings;
        private DailyQuotesService mDailyQuotesService;
        private TeamCityService mTeamCityService;
        ApplicationDataContainer mLocalSettings;
        private string mLastWeatherIcon = null;

        public MainPage()
        {
            InitializeComponent();
            mLocalSettings = ApplicationData.Current.LocalSettings;
            TeamCityBuilds = new List<TeamCityBuild>();
            LoadSettings();
            UpdateServices();

            CarparkImageControl.ImageOpened +=
                (sender, args) => CarparkTimestampTextBlock.Text = $"Carpark as at {DateTime.Now.ToString("G")}";

            mCarparkAndBuildTimer.Interval = TimeSpan.FromSeconds(30);
            mCarparkAndBuildTimer.Tick += CarparkAndBuildTimerOnTick;
            mCarparkAndBuildTimer.Start();

            mWeatherTimer.Interval = TimeSpan.FromMinutes(10);
            mWeatherTimer.Tick += WeatherTimerOnTick;
            mWeatherTimer.Start();

            mQuotesTimer.Interval = TimeSpan.FromDays(1);
            mQuotesTimer.Tick += QuotesTimerOnTick;

            RefreshWeather();
            RefreshTeamCityBuilds();
            UpdateDailyQuotes();
        }

        private void QuotesTimerOnTick(object sender, object e)
        {
            UpdateDailyQuotes();
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
            mTeamCityService = new TeamCityService(mSettings.TeamCityServerUrl);
            //mTrafficService = new TrafficService("https://infoconnect1.highwayinfo.govt.nz/ic/jbi/", mSettings.MotorwayTrafficApiKey);
            mDailyQuotesService = new DailyQuotesService(mSettings.DailyQuotesUrl);
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
                    var uri = new Uri($"http://openweathermap.org/img/w/{weather.Icon}.png");
                    var bitmap = new BitmapImage(uri);
                    WeatherIconImage.Source = bitmap;
                    mLastWeatherIcon = weather.Icon;
                }

                WeatherTitleTextBlock.Text = weather.Date.ToString("ddd MMM dd");
                WeatherTempTextBlock.Text = $"{weather.Temp}\u00B0C";
                WeatherSummaryTextBlock.Text = $"{weather.Description.ToTitleCase()}{Environment.NewLine}Relative Humidity: {weather.Humidity}%{Environment.NewLine}Wind Speed: {weather.WindSpeed} m/s";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void CarparkAndBuildTimerOnTick(object sender, object o)
        {
            RefreshCarparkImage();
        }

        public void RefreshCarparkImage()
        {
            if (!string.IsNullOrEmpty(mSettings.GarageCamImageUrl))
            {
                var bitmapImage = new BitmapImage(new Uri(mSettings.GarageCamImageUrl, UriKind.Absolute));
                
                CarparkImageControl.Source = bitmapImage;
            }
        }

        public async void RefreshTeamCityBuilds()
        {
            if (!string.IsNullOrEmpty(mSettings.TeamCityServerUrl))
            {
                TeamCityBuilds = await mTeamCityService.GetMostRecentBuilds();                
            }
        }

        public async void UpdateDailyQuotes()
        {
            if (!string.IsNullOrEmpty(mSettings.DailyQuotesUrl))
            {
                var quoteTuple = await mDailyQuotesService.GetDailyQuote();
                DailyQuoteTextBox.Text = $"\"{quoteTuple.Item1}\"";
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
    }
}
