using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[KnownType(typeof(SyncCallerContext))]
	public class GetObjectRequest
	{
		[DataMember]
		public SyncCallerContext callerContext { get; set; }
	}
}
