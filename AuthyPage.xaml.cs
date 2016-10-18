using System.Linq;
using Authy.AccountManagement;
using Xamarin.Auth;
using Xamarin.Forms;
using ModernHttpClient;
using System.Net.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using PCLCrypto;
using System.Threading.Tasks;

namespace Authy
{
    public partial class AuthyPage : ContentPage
    {
        IAccountManagerService _services;
        public AuthyPage()
        {
            InitializeComponent();
			Title = "Conectar cuentas";

            var deleteButton = new ToolbarItem { Text = "Delete" };
            deleteButton.Clicked += (s, a) =>
            {
                _services.EraseAll();
            };

            ToolbarItems.Add(deleteButton);

            _services = DependencyService.Get<IAccountManagerService>();

                facebook.Clicked += ViewProfileButton_Clicked;
                github.Clicked += ViewProfileButton_Clicked;
                twitter.Clicked += ViewProfileButton_Clicked;
        }

        async void ViewProfileButton_Clicked(object sender, System.EventArgs e)
        {
            var sndr = sender as Button;

            var accounts = _services.Accounts;

#if DEBUG
            var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
#else
            var httpClient = new HttpClient(new NativeMessageHandler());
#endif

            if (sndr == facebook)
			{
				if (accounts.ContainsKey(Services.Facebook))
				{
					await ViewFacebookProfile(httpClient);
				}
				else
				{
					await Navigation.PushAsync(new AuthorizePage(Services.Facebook));
				}
			}
			else if (sndr == twitter)
			{
				if (accounts.ContainsKey(Services.Twitter))
				{
					await ViewTwitterProfile(httpClient);
				}
				else
				{
					await Navigation.PushAsync(new AuthorizePage(Services.Twitter));
				}
			}
			else if (sndr == github)
			{
				if (accounts.ContainsKey(Services.GitHub))
				{
					await ViewGitHubProfile(httpClient);
				}
				else
				{
					await Navigation.PushAsync(new AuthorizePage(Services.GitHub));
				}
			}
			else
			{

			}
		}

		async Task ViewGitHubProfile(HttpClient httpClient)
		{
			var uri = new Uri("https://api.github.com/user");
			var access_token = _services.GetPropertyFromAccount(Services.GitHub, "access_token");

			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", access_token);
			httpClient.DefaultRequestHeaders.Add("User-Agent",
								 "AB");
			var response = await httpClient.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var a = (JObject)JsonConvert.DeserializeObject(content);

				var login = a["login"];
				var image = a["avatar_url"];
				await Navigation.PushAsync(new ProfilePage(Services.GitHub, login.ToString(), image.ToString()));
			}
		}

		async Task ViewFacebookProfile(HttpClient httpClient)
		{
			var access_token = _services.GetPropertyFromAccount(Services.Facebook, "access_token");
			var fbUri = new Uri("https://graph.facebook.com/me?access_token=" + access_token);

			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", access_token);
			var response = await httpClient.GetAsync(fbUri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var a = (JObject)JsonConvert.DeserializeObject(content);

				var login = a["name"];
				var image = "https://graph.facebook.com/me/picture?access_token=" + access_token;
				await Navigation.PushAsync(new ProfilePage(Services.Facebook, login.ToString(), image.ToString()));
			}
		}

		async Task ViewTwitterProfile(HttpClient httpClient)
		{
			var screen_name = _services.GetPropertyFromAccount(Services.Twitter, "screen_name");
			var oauth_consumer_key = _services.GetPropertyFromAccount(Services.Twitter, "oauth_consumer_key");
			var oauth_token_secret = _services.GetPropertyFromAccount(Services.Twitter, "oauth_token_secret");
			var oauth_timestamp = ToUnixTime(DateTime.UtcNow).ToString();
			var oauth_token = _services.GetPropertyFromAccount(Services.Twitter, "oauth_token");
			var oauth_nonce = Guid.NewGuid().ToString("N");
			var oauth_signature_method = "HMAC-SHA1";
			var oauth_version = "1.0";

			var twitterUri = "https://api.twitter.com/1.1/users/show.json?screen_name=" + screen_name;

			#region Create signature
			// Collecting parameters
			var parameters = new Dictionary<string, string>()
				{
					{nameof(screen_name), screen_name },
					{nameof(oauth_consumer_key), oauth_consumer_key },
					{nameof(oauth_nonce), oauth_nonce },
					{nameof(oauth_signature_method), oauth_signature_method },
					{nameof(oauth_token), oauth_token },
					{nameof(oauth_timestamp), oauth_timestamp },
					{nameof(oauth_version), oauth_version },

				};

			var list = parameters.Keys.ToList();
			list.Sort();


			var queryString = String.Join("&", list.Select(key => String.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(parameters[key]))).ToArray());

			// Creating the signature base string
			var signatureBase = "GET&" + Uri.EscapeDataString("https://api.twitter.com/1.1/users/show.json") + "&" +
				Uri.EscapeDataString(queryString);

			var twitterKeys = new TwitterKeys();
			var signingkey = Uri.EscapeDataString(twitterKeys.ConsumerSecret) + "&" //;
			+ Uri.EscapeDataString(oauth_token_secret);

			var oauth_signature = HashHmac(signatureBase, signingkey);

			#endregion


			var authValue = "oauth_consumer_key=\"" + oauth_consumer_key + "\", oauth_nonce=\"" + oauth_nonce + "\""
				+ ", oauth_signature=\"" + Uri.EscapeDataString(oauth_signature) + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + oauth_timestamp +
				"\"," +
				" oauth_token=\"" + oauth_token + "\"," +
				" oauth_version=\"1.0\"";
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authValue);


			var twitterResponse = await httpClient.GetAsync(twitterUri);
			System.Diagnostics.Debug.WriteLine(twitterResponse);
			if (twitterResponse.IsSuccessStatusCode || twitterResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var twitterContent = await twitterResponse.Content.ReadAsStringAsync();
				var a = (JObject)JsonConvert.DeserializeObject(twitterContent);

				var login = a["screen_name"];
				var image = a["profile_image_url"];
				await Navigation.PushAsync(new ProfilePage(Services.GitHub, login.ToString(), image.ToString()));
			}
		}

        public static long ToUnixTime(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string HashHmac(string data, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha1);
            CryptographicHash hasher = algorithm.CreateHash(keyBytes);
            hasher.Append(dataBytes);
            byte[] mac = hasher.GetValueAndReset();

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < mac.Length; i++)
            {
                sBuilder.Append(mac[i].ToString("X2"));
            }
            return System.Convert.ToBase64String(mac);
        }

		protected override void OnAppearing()
		{
			var accounts = _services.Accounts;

			if (accounts.ContainsKey(Services.GitHub))
				github.Text = "Ver perfil de GitHub";
			else
				github.Text = "Conectar GitHub";

			if (accounts.ContainsKey(Services.Facebook))
				facebook.Text = "Ver perfil de Facebook";
			else
				facebook.Text = "Conectar Facebook";

			if (accounts.ContainsKey(Services.Twitter))
				twitter.Text = "Ver perfil de Twitter";
			else
				twitter.Text = "Conectar Twitter";

		}
    }
}
