using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[Serializable]
	public class PolicySyncTransientFault : PolicySyncFaultBase
	{
		public PolicySyncTransientFault(int errorCode, string message, string serverIdentifier, SyncCallerContext callerContext) : base(errorCode, message, serverIdentifier, callerContext)
		{
		}
	}
}
