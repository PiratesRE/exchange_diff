using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(SyncCallerContext))]
	[DataContract]
	[KnownType(typeof(TenantCookieCollection))]
	[KnownType(typeof(TenantCookie))]
	public class GetChangesRequest
	{
		[DataMember]
		public SyncCallerContext CallerContext { get; set; }

		[DataMember]
		public TenantCookieCollection TenantCookies { get; set; }
	}
}
