using System;
using Xamarin.Forms;

namespace Authy
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			IsLogged = false;

			MainPage = new NavigationPage(new AuthyPage());
		}

		public static bool IsLogged;

		public const string AppName = "Authy";

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		public static void LogIn(Service service)
		{
			IsLogged = true;
		}
	}
}
