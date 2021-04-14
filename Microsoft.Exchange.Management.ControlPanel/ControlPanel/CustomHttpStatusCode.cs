using System;
using System.Net;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class CustomHttpStatusCode
	{
		public const HttpStatusCode IdentityAccepted = (HttpStatusCode)241;

		public const HttpStatusCode NeedIdentity = (HttpStatusCode)441;

		public const HttpStatusCode DelegatedSecurityTokenExpired = (HttpStatusCode)443;
	}
}
