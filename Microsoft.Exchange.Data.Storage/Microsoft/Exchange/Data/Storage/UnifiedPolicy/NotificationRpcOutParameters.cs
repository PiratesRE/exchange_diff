using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NotificationRpcOutParameters : RpcParameters
	{
		public SyncNotificationResult Result { get; private set; }

		public NotificationRpcOutParameters(byte[] data) : base(data)
		{
			this.Result = (SyncNotificationResult)base.GetParameterValue("SyncNotificationResult");
		}

		public NotificationRpcOutParameters(SyncNotificationResult result)
		{
			this.Result = result;
			base.SetParameterValue("SyncNotificationResult", this.Result);
		}

		private const string SyncNotificationResultParameterName = "SyncNotificationResult";
	}
}
