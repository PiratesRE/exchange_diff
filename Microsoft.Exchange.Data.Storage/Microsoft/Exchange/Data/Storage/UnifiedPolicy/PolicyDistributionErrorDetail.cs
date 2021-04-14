using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PolicyDistributionErrorDetail
	{
		public Workload Workload { get; set; }

		public string Endpoint { get; set; }

		public UnifiedPolicyErrorCode ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public DateTime? ErrorTimeUTC { get; set; }
	}
}
