using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class PolicyChange
	{
		public IEnumerable<PolicyConfigurationBase> Changes { get; set; }

		public TenantCookie NewCookie { get; set; }
	}
}
