using System;
using Authy.AccountManagement;
using Xamarin.Forms;
using Xamarin.Auth;
#if __IOS__
using Xamarin.Forms.Platform.iOS;
#elif __ANDROID__
using Xamarin.Forms.Platform.Android;
#endif

#if __IOS__
[assembly: ExportRenderer(typeof(AuthorizePage), typeof(Authy.iOS.AccountManagement.AuthorizePageRenderer))]
namespace Authy.iOS.AccountManagement
#elif __ANDROID__
[assembly: ExportRenderer(typeof(AuthorizePage), typeof(Authy.Droid.AccountManagement.AuthorizePageRenderer))]
namespace Authy.Droid.AccountManagement
#endif
{
	public class AuthorizePageRenderer : PageRenderer
	{

#if __IOS__
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
#elif __ANDROID__
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);
			var activity = this.Context as Android.App.Activity;
#endif


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
#if __IOS__
					DismissViewController(true, null);
#endif
					if (eventArgs.IsAuthenticated)
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					await authPage.Navigation.PopAsync();
				};
#if __IOS__
				PresentViewController(auth2.GetUI(), true, null);
#elif __ANDROID__
				activity.StartActivity(auth2.GetUI(activity));
#endif
			}

			if (auth1 != null)
			{
				auth1.Completed += async (sender, eventArgs) =>
				{
#if __IOS__
					DismissViewController(true, null);
#endif
					if (eventArgs.IsAuthenticated)
						AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
					await authPage.Navigation.PopAsync();
				};
			
#if __IOS__
				PresentViewController(auth1.GetUI(), true, null);
#elif __ANDROID__
				activity.StartActivity(auth1.GetUI(activity));
#endif
			}

		}
	}

}