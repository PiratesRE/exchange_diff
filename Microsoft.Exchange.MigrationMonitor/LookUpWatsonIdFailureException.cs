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
	internal class LookUpWatsonIdFailureException : LocalizedException
	{
		public LookUpWatsonIdFailureException() : base(MigrationMonitorStrings.ErrorLookUpWatsonId)
		{
		}

		public LookUpWatsonIdFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorLookUpWatsonId, innerException)
		{
		}

		protected LookUpWatsonIdFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
