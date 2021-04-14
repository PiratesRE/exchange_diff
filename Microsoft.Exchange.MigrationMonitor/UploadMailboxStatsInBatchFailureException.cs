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
	internal class UploadMailboxStatsInBatchFailureException : LocalizedException
	{
		public UploadMailboxStatsInBatchFailureException() : base(MigrationMonitorStrings.ErrorUploadMailboxStatsInBatch)
		{
		}

		public UploadMailboxStatsInBatchFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorUploadMailboxStatsInBatch, innerException)
		{
		}

		protected UploadMailboxStatsInBatchFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
