using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(RuleConfiguration))]
	[DataContract]
	public class GetObjectResultForRule
	{
		[DataMember]
		public RuleConfiguration GetObjectResult { get; set; }
	}
}
