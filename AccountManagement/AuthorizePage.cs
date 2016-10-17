using System;
using Xamarin.Forms;

namespace Authy.AccountManagement
{
	public enum Services
	{
		GitHub,
		Facebook,
		Twitter,
		// 
	}
	
	public class AuthorizePage : ContentPage
	{
		public Services Service { get; private set; }

		public AuthorizePage(Services service)
		{
			Service = service;
		}
	}
}
