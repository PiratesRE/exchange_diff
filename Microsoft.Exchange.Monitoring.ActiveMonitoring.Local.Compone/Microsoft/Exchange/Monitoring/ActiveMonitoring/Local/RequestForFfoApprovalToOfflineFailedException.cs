using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestForFfoApprovalToOfflineFailedException : RecoveryActionExceptionCommon
	{
		public RequestForFfoApprovalToOfflineFailedException() : base(Strings.RequestForFfoApprovalToOfflineFailed)
		{
		}

		public RequestForFfoApprovalToOfflineFailedException(Exception innerException) : base(Strings.RequestForFfoApprovalToOfflineFailed, innerException)
		{
		}

		protected RequestForFfoApprovalToOfflineFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
