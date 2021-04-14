using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingExceptionSearchNotAuthorized : LocalizedException
	{
		public TrackingExceptionSearchNotAuthorized() : base(CoreStrings.TrackingSearchNotAuthorized)
		{
		}

		public TrackingExceptionSearchNotAuthorized(Exception innerException) : base(CoreStrings.TrackingSearchNotAuthorized, innerException)
		{
		}

		protected TrackingExceptionSearchNotAuthorized(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
