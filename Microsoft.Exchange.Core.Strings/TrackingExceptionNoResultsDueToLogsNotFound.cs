using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingExceptionNoResultsDueToLogsNotFound : LocalizedException
	{
		public TrackingExceptionNoResultsDueToLogsNotFound() : base(CoreStrings.TrackingWarningNoResultsDueToLogsNotFound)
		{
		}

		public TrackingExceptionNoResultsDueToLogsNotFound(Exception innerException) : base(CoreStrings.TrackingWarningNoResultsDueToLogsNotFound, innerException)
		{
		}

		protected TrackingExceptionNoResultsDueToLogsNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
