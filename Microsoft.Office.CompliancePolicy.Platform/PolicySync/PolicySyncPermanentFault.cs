using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[Serializable]
	public class PolicySyncPermanentFault : PolicySyncFaultBase
	{
		public PolicySyncPermanentFault(int errorCode, string message, string serverIdentifier, SyncCallerContext callerContext) : base(errorCode, message, serverIdentifier, callerContext)
		{
		}

		public const int Unknown = 0;

		public const int TenantNotFoundInRegion = 1;

		public const int UnauthorizedAccess = -1;

		public const int GlsError = 2;
	}
}
