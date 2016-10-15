using System;
using Xamarin.Forms;

namespace Authy
{
	public enum Service
	{
		GitHub,
		Facebook,
		Twitter,
		// 
	}
	
	public class AuthorizePage : ContentPage
	{
		public Service Service { get; private set; }

		public AuthorizePage(Service service)
		{
			Service = service;
		}
	}
}
