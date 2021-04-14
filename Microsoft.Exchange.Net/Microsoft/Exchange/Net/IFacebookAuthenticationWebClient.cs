using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFacebookAuthenticationWebClient
	{
		AuthenticateApplicationResponse AuthenticateApplication(Uri accessTokenEndpoint, TimeSpan requestTimeout);
	}
}
