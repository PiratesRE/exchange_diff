using System;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class FederationTrust : IFederationTrust
	{
		public string Name { get; set; }

		public Uri TokenIssuerUri { get; set; }
	}
}
