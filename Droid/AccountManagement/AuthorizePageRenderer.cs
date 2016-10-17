using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using Authy.AccountManagement;
using Authy.Droid.AccountManagement;

[assembly: ExportRenderer(typeof(AuthorizePage), typeof(AuthorizePageRenderer))]
namespace Authy.Droid.AccountManagement
{
    public class AuthorizePageRenderer : PageRenderer
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            var activity = this.Context as Activity;

            OAuth2Authenticator auth2 = null;
            OAuth1Authenticator auth1 = null;

            var authPage = Element as AuthorizePage;

            var accounts = AccountStore.Create().FindAccountsForService(authPage.Service.ToString());
            if (accounts.Any())
            {
                await authPage.Navigation.PopAsync();
                return;
            }

            IKeys keys = null;

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
                        clientId:keys.ClientId,
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
                auth2.Completed += (sender, eventArgs) =>
                {

                    if (eventArgs.IsAuthenticated)
                    {
                        // Use eventArgs.Account to do wonderful things
                        // ...such as saving the token, and then getting some more detailed user info from the API
                        App.LogIn(authPage.Service);
                        AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
                    }
                    else
                    {
                        // The user cancelled
                    }
                    //activity.Finish();
                };
                activity.StartActivity(auth2.GetUI(activity));
            }

            if (auth1 != null)
            {
                auth1.Completed += (sender, eventArgs) =>
                {             

                    if (eventArgs.IsAuthenticated)
                    {
                        // Use eventArgs.Account to do wonderful things
                        // ...such as saving the token, and then getting some more detailed user info from the API
                        App.LogIn(authPage.Service);
                        AccountStore.Create().Save(eventArgs.Account, authPage.Service.ToString());
                    }
                    else
                    {

                    }
                    //activity.Finish();
                };
                activity.StartActivity(auth1.GetUI(activity));
            }
        }
    }
}