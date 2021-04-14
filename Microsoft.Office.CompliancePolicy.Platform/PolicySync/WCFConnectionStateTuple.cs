using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal class WCFConnectionStateTuple<TClient> : IPooledServiceProxy<TClient>
	{
		public TClient Client { get; set; }

		public DateTime LastUsed { get; set; }

		public string Tag { get; set; }
	}
}
