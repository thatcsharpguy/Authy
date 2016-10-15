using System;
namespace Authy
{
	public interface IKeys 
	{ 
		string ClientId { get; }
		string ClientSecret { get; }
		string Scope { get; }
		string AuthorizeUrl { get; }
		string RedirectUrl { get; }
		string AccessTokenUrl { get; }
	}

	public class FacebookKeys : IKeys
	{

		public string AccessTokenUrl { get; } = "";

		public string AuthorizeUrl { get; } = "https://m.facebook.com/dialog/oauth/";

		public string ClientId { get; } = "";

		public string ClientSecret { get; } = "";

		public string RedirectUrl { get; } = "http://www.facebook.com/connect/login_success.html";

		public string Scope { get; } = "";
	}
}
