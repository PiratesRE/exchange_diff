using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IFederationTrust
	{
		string Name { get; }

		Uri TokenIssuerUri { get; }
	}
}
