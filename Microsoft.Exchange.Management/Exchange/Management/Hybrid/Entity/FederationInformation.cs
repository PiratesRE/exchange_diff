using System;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class FederationInformation : IFederationInformation
	{
		public string TargetAutodiscoverEpr { get; set; }

		public string TargetApplicationUri { get; set; }
	}
}
