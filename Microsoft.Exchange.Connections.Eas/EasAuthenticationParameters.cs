using System;
using System.Net;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class EasAuthenticationParameters : AuthenticationParameters
	{
		public EasAuthenticationParameters(NetworkCredential networkCredential, string local, string domain, string endpointOverride = null) : base(networkCredential)
		{
			this.UserSmtpAddress = new UserSmtpAddress(local, domain);
			this.EndpointOverride = endpointOverride;
		}

		internal UserSmtpAddress UserSmtpAddress { get; private set; }

		internal string EndpointOverride { get; private set; }
	}
}
