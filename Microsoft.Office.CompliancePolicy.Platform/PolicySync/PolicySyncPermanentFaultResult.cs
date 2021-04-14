using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[KnownType(typeof(PolicySyncPermanentFault))]
	public class PolicySyncPermanentFaultResult
	{
		[DataMember]
		public PolicySyncPermanentFault Detail { get; set; }

		[DataMember]
		public string FaultType { get; set; }

		[DataMember]
		public string Message { get; set; }
	}
}
