using Windows.Storage;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OfficeDashboard
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        private readonly ApplicationDataContainer mLocalSettings;

        public SettingsDialog()
        {
            InitializeComponent();
            MaxWidth = ActualWidth;
            mLocalSettings = ApplicationData.Current.LocalSettings;
            LoadSettings();
        }

        public Settings Settings { get; set; }

        private void LoadSettings()
        {
            Settings = new Settings();
            Settings.WeatherApiKey = mLocalSettings.Values["WeatherApiKey"]?.ToString();
            Settings.GarageCamImageUrl = mLocalSettings.Values["GarageCamImageUrl"]?.ToString();
            Settings.MotorwayTrafficApiKey = mLocalSettings.Values["MotorwayTrafficApiKey"]?.ToString();
            Settings.TeamCityServerUrl = mLocalSettings.Values["TeamCityServerUrl"]?.ToString();
            Settings.DailyQuotesUrl = mLocalSettings.Values["DailyQuotesUrl"]?.ToString();

            WeatherApiKeyTextBox.Text = Settings.WeatherApiKey ?? string.Empty;
            GarageCamImageUrlTextBox.Text = Settings.GarageCamImageUrl ?? string.Empty;
            MotorwayTrafficApiKeyTextBox.Text = Settings.MotorwayTrafficApiKey ?? string.Empty;
            TeamCityServerUrlTextBox.Text = Settings.TeamCityServerUrl ?? string.Empty;
            DailyQuoteUrlTextBox.Text = Settings.DailyQuotesUrl ?? string.Empty;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Settings.WeatherApiKey = WeatherApiKeyTextBox.Text;
            Settings.GarageCamImageUrl = GarageCamImageUrlTextBox.Text;
            Settings.MotorwayTrafficApiKey = MotorwayTrafficApiKeyTextBox.Text;
            Settings.TeamCityServerUrl = TeamCityServerUrlTextBox.Text;
            Settings.DailyQuotesUrl = DailyQuoteUrlTextBox.Text;

            mLocalSettings.Values["WeatherApiKey"] = Settings.WeatherApiKey;
            mLocalSettings.Values["GarageCamImageUrl"] = Settings.GarageCamImageUrl;
            mLocalSettings.Values["MotorwayTrafficApiKey"] = Settings.MotorwayTrafficApiKey;
            mLocalSettings.Values["TeamCityServerUrl"] = Settings.TeamCityServerUrl;
            mLocalSettings.Values["DailyQuotesUrl"] = Settings.DailyQuotesUrl;

            Debug.WriteLine("Settings saved");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Debug.WriteLine("Settings dialog cancelled");
        }
    }
}
