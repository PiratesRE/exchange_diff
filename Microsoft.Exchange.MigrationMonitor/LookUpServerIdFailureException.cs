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
	internal class LookUpServerIdFailureException : LocalizedException
	{
		public LookUpServerIdFailureException() : base(MigrationMonitorStrings.ErrorLookUpServerId)
		{
		}

		public LookUpServerIdFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorLookUpServerId, innerException)
		{
		}

		protected LookUpServerIdFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
