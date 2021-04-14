using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingExceptionNoResultsDueToLogsExpired : LocalizedException
	{
		public TrackingExceptionNoResultsDueToLogsExpired() : base(CoreStrings.TrackingWarningNoResultsDueToLogsExpired)
		{
		}

		public TrackingExceptionNoResultsDueToLogsExpired(Exception innerException) : base(CoreStrings.TrackingWarningNoResultsDueToLogsExpired, innerException)
		{
		}

		protected TrackingExceptionNoResultsDueToLogsExpired(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
