using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AuthenticateApplicationResponse
	{
		internal AuthenticateApplicationResponse()
		{
		}

		public HttpStatusCode Code { get; internal set; }

		public string Body { get; internal set; }
	}
}
