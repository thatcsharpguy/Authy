using System;
using System.Linq;
using Authy;
using Authy.iOS;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AuthorizePage), typeof(AuthorizePageRenderer))]
namespace Authy.iOS
{
	public class AuthorizePageRenderer : PageRenderer
	{
		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// I've used the values from your original post
			OAuth2Authenticator auth2 = null;
			OAuth1Authenticator auth1 = null;

			var authPage = Element as AuthorizePage;

			var accounts = AccountStore.Create().FindAccountsForService(authPage.Service.ToString());
			if (accounts.Any())
			{
				await authPage.Navigation.PopAsync();
				return;
			}

			switch (authPage.Service)
			{
				case Service.Facebook:
					auth2 = new OAuth2Authenticator(
						clientId: "",
						scope: "",
						authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
						redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));
					break;
				case Service.Twitter:
					auth1 = new OAuth1Authenticator(
							consumerKey: "",
							consumerSecret: "",
							requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
							authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
							accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
							callbackUrl: new Uri("https://mobile.twitter.com/home"));
					break;
				case Service.GitHub:
					auth2 = new OAuth2Authenticator(
						clientId: "",
						clientSecret: "",
						scope: "",
						authorizeUrl: new Uri("https://github.com/login/oauth/authorize"),
						redirectUrl: new Uri("https://github.com"),
						accessTokenUrl: new Uri("https://github.com/login/oauth/access_token"));
					break;
				default:
					throw new Exception("Service " + authPage.Service + " not yet supported");
			}
			if (auth2 != null)
			{
				auth2.Completed += (sender, eventArgs) =>
				{
				// We presented the UI, so it's up to us to dismiss it on iOS.
				DismissViewController(true, null);
				// you may want to also do something to dismiss THIS viewcontroller, 
				// or else you'll keep seeing the login screen              

				if (eventArgs.IsAuthenticated)
					{
					// Use eventArgs.Account to do wonderful things
					// ...such as saving the token, and then getting some more detailed user info from the API
					App.LogIn(authPage.Service);
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					}
					else {
					// The user cancelled
				}
				};

				PresentViewController(auth2.GetUI(), true, null);
			}

			if (auth1 != null)
			{
				auth1.Completed += (sender, eventArgs) =>
				{
					// We presented the UI, so it's up to us to dismiss it on iOS.
					DismissViewController(true, null);
					// you may want to also do something to dismiss THIS viewcontroller, 
					// or else you'll keep seeing the login screen              

					if (eventArgs.IsAuthenticated)
					{
						// Use eventArgs.Account to do wonderful things
						// ...such as saving the token, and then getting some more detailed user info from the API
						App.LogIn(authPage.Service);
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					}
					else {
						
					}
				};

				PresentViewController(auth1.GetUI(), true, null);
			}
		}
	}
}
