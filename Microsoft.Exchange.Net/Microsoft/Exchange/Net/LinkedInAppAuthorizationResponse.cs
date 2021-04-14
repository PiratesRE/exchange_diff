using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class LinkedInAppAuthorizationResponse
	{
		internal LinkedInAppAuthorizationResponse()
		{
		}

		public string RequestToken { get; internal set; }

		public string RequestSecret { get; internal set; }

		public string OAuthVerifier { get; internal set; }

		public string AppAuthorizationRedirectUri { get; internal set; }

		public string OAuthProblem { get; internal set; }
	}
}
