using System;
using System.Linq;
using Authy.AccountManagement;
using Authy.iOS.AccountManagement;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AuthorizePage), typeof(AuthorizePageRenderer))]
namespace Authy.iOS.AccountManagement
{
	public class AuthorizePageRenderer : PageRenderer
	{
		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			OAuth2Authenticator auth2 = null;
			OAuth1Authenticator auth1 = null;
			IKeys keys = null;

			var authPage = Element as AuthorizePage;

            switch (authPage.Service)
            {
                case Services.Facebook:
                    keys = new FacebookKeys();
                    auth2 = new OAuth2Authenticator(
                        clientId: keys.ClientId,
                        scope: keys.Scope,
                        authorizeUrl: new Uri(keys.AuthorizeUrl),
                        redirectUrl: new Uri(keys.RedirectUrl));
                    break;
                case Services.Twitter:
                    keys = new TwitterKeys();
                    auth1 = new OAuth1Authenticator(
                            consumerKey: keys.ConsumerKey,
                            consumerSecret: keys.ConsumerSecret,
                            requestTokenUrl: new Uri(keys.RequestTokenUrl),
                            authorizeUrl: new Uri(keys.AuthorizeUrl),
                            accessTokenUrl: new Uri(keys.AccessTokenUrl),
                            callbackUrl: new Uri(keys.CallbackUrl));
                    break;
                case Services.GitHub:
                    keys = new GitHubKeys();
                    auth2 = new OAuth2Authenticator(
                        clientId: keys.ClientId,
						clientSecret: keys.ClientSecret,
                        scope: keys.Scope,
                        authorizeUrl: new Uri(keys.AuthorizeUrl),
                        redirectUrl: new Uri(keys.RedirectUrl),
                        accessTokenUrl: new Uri(keys.AccessTokenUrl));
                    break;
                default:
                    throw new Exception("Service " + authPage.Service + " not yet supported");
            }
            if (auth2 != null)
			{
				auth2.Completed += async (sender, eventArgs) =>
				{
					DismissViewController(true, null);       
					if (eventArgs.IsAuthenticated)
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					await authPage.Navigation.PopAsync();
				};

				PresentViewController(auth2.GetUI(), true, null);
			}

			if (auth1 != null)
			{
				auth1.Completed += async (sender, eventArgs) =>
				{
					DismissViewController(true, null);
					if (eventArgs.IsAuthenticated)
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					await authPage.Navigation.PopAsync();
				};

				PresentViewController(auth1.GetUI(), true, null);
			}
		}
	}
}
