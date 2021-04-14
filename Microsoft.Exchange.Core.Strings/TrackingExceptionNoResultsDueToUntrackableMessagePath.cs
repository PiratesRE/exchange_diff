using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingExceptionNoResultsDueToUntrackableMessagePath : LocalizedException
	{
		public TrackingExceptionNoResultsDueToUntrackableMessagePath() : base(CoreStrings.TrackingWarningNoResultsDueToUntrackableMessagePath)
		{
		}

		public TrackingExceptionNoResultsDueToUntrackableMessagePath(Exception innerException) : base(CoreStrings.TrackingWarningNoResultsDueToUntrackableMessagePath, innerException)
		{
		}

		protected TrackingExceptionNoResultsDueToUntrackableMessagePath(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
