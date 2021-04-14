using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class SyncNotificationResult
	{
		public SyncNotificationResult(object resultObject = null)
		{
			this.ResultObject = resultObject;
		}

		public SyncNotificationResult(SyncAgentExceptionBase error)
		{
			this.Error = error;
		}

		public SyncAgentExceptionBase Error { get; set; }

		public object ResultObject { get; set; }

		public string AdditionalInformation { get; set; }

		public bool Success
		{
			get
			{
				return this.Error == null;
			}
		}
	}
}
