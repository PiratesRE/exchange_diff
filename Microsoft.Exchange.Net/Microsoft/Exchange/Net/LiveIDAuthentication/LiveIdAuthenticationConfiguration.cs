using System;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	public class LiveIdAuthenticationConfiguration
	{
		public string MsoTokenIssuerUri { get; set; }

		public Uri LiveServiceLogin1Uri { get; set; }

		public Uri LiveServiceLogin2Uri { get; set; }

		public Uri MsoServiceLogin2Uri { get; set; }

		public Uri MsoGetUserRealmUri { get; set; }
	}
}
