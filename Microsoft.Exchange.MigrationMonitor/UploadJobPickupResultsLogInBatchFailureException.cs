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
	internal class UploadJobPickupResultsLogInBatchFailureException : LocalizedException
	{
		public UploadJobPickupResultsLogInBatchFailureException() : base(MigrationMonitorStrings.ErrorUploadJobPickupResultsLogInBatch)
		{
		}

		public UploadJobPickupResultsLogInBatchFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorUploadJobPickupResultsLogInBatch, innerException)
		{
		}

		protected UploadJobPickupResultsLogInBatchFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
