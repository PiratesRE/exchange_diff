using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(PolicyConfiguration))]
	[DataContract]
	public class GetObjectResultForPolicy
	{
		[DataMember]
		public PolicyConfiguration GetObjectResult { get; set; }
	}
}
