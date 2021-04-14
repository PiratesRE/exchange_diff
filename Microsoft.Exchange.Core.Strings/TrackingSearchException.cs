using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TrackingSearchException : LocalizedException
	{
		public TrackingSearchException(LocalizedString reason) : base(CoreStrings.TrackingSearchException(reason))
		{
			this.reason = reason;
		}

		public TrackingSearchException(LocalizedString reason, Exception innerException) : base(CoreStrings.TrackingSearchException(reason), innerException)
		{
			this.reason = reason;
		}

		protected TrackingSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (LocalizedString)info.GetValue("reason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public LocalizedString Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly LocalizedString reason;
	}
}
