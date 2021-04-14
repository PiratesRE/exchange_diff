using System;
using System.Net;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ICredentialHandler
	{
		NetworkCredential Credential { get; }

		bool RequestCredential(Uri serviceEndpoint);

		void InvalidateCredential(Uri serviceEndpoint);
	}
}
