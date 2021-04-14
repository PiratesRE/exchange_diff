using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingWarningNoResultsDueToTrackingTooEarly : LocalizedException
	{
		public TrackingWarningNoResultsDueToTrackingTooEarly() : base(CoreStrings.TrackingWarningNoResultsDueToTrackingTooEarly)
		{
		}

		public TrackingWarningNoResultsDueToTrackingTooEarly(Exception innerException) : base(CoreStrings.TrackingWarningNoResultsDueToTrackingTooEarly, innerException)
		{
		}

		protected TrackingWarningNoResultsDueToTrackingTooEarly(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
