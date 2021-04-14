using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(PolicySyncPermanentFault))]
	[DataContract]
	public class PolicySyncTransientFaultResult
	{
		[DataMember]
		public PolicySyncTransientFault Detail { get; set; }

		[DataMember]
		public string FaultType { get; set; }

		[DataMember]
		public string Message { get; set; }
	}
}
