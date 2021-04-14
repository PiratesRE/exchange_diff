using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal abstract class UcwaServerToServerClient
	{
		protected UcwaServerToServerClient(string ucwaUrl, ICredentials oauthCredentials)
		{
			this.UcwaUrl = ucwaUrl;
			this.OAuthCredentials = oauthCredentials;
		}

		protected string UcwaUrl { get; set; }

		protected ICredentials OAuthCredentials { get; set; }
	}
}
