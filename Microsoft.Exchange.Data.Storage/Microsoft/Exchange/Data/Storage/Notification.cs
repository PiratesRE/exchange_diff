using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Notification
	{
		internal Notification(NotificationType type)
		{
			EnumValidator.AssertValid<NotificationType>(type);
			this.type = type;
			this.createTime = Stopwatch.GetTimestamp();
		}

		public NotificationType Type
		{
			get
			{
				return this.type;
			}
		}

		public long CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		private readonly NotificationType type;

		private readonly long createTime;
	}
}
