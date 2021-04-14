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
	internal class LookUpEndpointIdFailureException : LocalizedException
	{
		public LookUpEndpointIdFailureException() : base(MigrationMonitorStrings.ErrorLookUpEndpointId)
		{
		}

		public LookUpEndpointIdFailureException(Exception innerException) : base(MigrationMonitorStrings.ErrorLookUpEndpointId, innerException)
		{
		}

		protected LookUpEndpointIdFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
