using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInTokenInformation
	{
		internal LinkedInTokenInformation()
		{
		}

		public string Token { get; internal set; }

		public string Secret { get; internal set; }

		public string OAuthAccessTokenUrl { get; internal set; }
	}
}
