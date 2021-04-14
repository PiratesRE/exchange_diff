using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal interface IPooledServiceProxy<out TClient>
	{
		TClient Client { get; }

		string Tag { get; set; }
	}
}
