using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UploadQueueStatsLogInBatchFailureException : LocalizedException
	{
		public UploadQueueStatsLogInBatchFailureException() : base(MigrationMonitorStrings.ErrorUploadQueueStatsLogInBatch)
		{
		}

		public UploadQueueStatsLogInBatchFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorUploadQueueStatsLogInBatch, innerException)
		{
		}

		protected UploadQueueStatsLogInBatchFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
