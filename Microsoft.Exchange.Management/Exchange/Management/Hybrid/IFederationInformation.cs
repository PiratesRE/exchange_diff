using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IFederationInformation
	{
		string TargetAutodiscoverEpr { get; }

		string TargetApplicationUri { get; }
	}
}
