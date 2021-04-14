using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(UnifiedPolicyStatus))]
	[DataContract]
	[KnownType(typeof(SyncCallerContext))]
	[KnownType(typeof(PolicyVersion))]
	public class PublishStatusRequest
	{
		[DataMember]
		public SyncCallerContext CallerContext { get; set; }

		[DataMember]
		public IEnumerable<UnifiedPolicyStatus> ConfigurationStatuses { get; set; }
	}
}
