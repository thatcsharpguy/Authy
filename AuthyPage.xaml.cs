using System.Linq;
using Authy.Acc;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Authy
{
	public partial class AuthyPage : ContentPage
	{
		public AuthyPage()
		{
			InitializeComponent();

			facebook.Clicked += AuthenticateButton_Clicked;

			var service = DependencyService.Get<IAccounts>();



			facebook.Clicked += AuthenticateButton_Clicked;
			twitter.Clicked += AuthenticateButton_Clicked;
			github.Clicked += AuthenticateButton_Clicked;
		}

		async void AuthenticateButton_Clicked(object sender, System.EventArgs e)
		{
			var sndr = sender as Button;

			if (sndr == facebook)
			{
				await Navigation.PushAsync(new AuthorizePage(Service.Facebook));
			}
			else if (sndr == twitter)
			{
				await Navigation.PushAsync(new AuthorizePage(Service.Twitter));
			}
			else if (sndr == github)
			{
				await Navigation.PushAsync(new AuthorizePage(Service.GitHub));
			}
			else 
			{
				
			}
		}
	}
}
