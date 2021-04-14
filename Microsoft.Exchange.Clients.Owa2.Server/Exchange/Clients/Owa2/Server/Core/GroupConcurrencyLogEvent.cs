using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupConcurrencyLogEvent : ILogEvent
	{
		internal GroupConcurrencyLogEvent(string groupName, NotificationType notificationType, long elapsedTime, int oldConcurrency, int currentConcurrency)
		{
			if (groupName == null)
			{
				throw new ArgumentNullException("groupName");
			}
			this.groupNameValue = groupName;
			this.notificationType = notificationType;
			this.elapsedTime = elapsedTime;
			this.previousConcurrencyCountValue = oldConcurrency;
			this.currentConcurrencyCountValue = currentConcurrency;
		}

		public string EventId
		{
			get
			{
				return "GroupConcurrency";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("GN", this.groupNameValue),
				new KeyValuePair<string, object>("NT", this.notificationType.ToString()),
				new KeyValuePair<string, object>("GUCET", this.elapsedTime.ToString(CultureInfo.InvariantCulture)),
				new KeyValuePair<string, object>("GUCPC", this.previousConcurrencyCountValue),
				new KeyValuePair<string, object>("GUCC", this.currentConcurrencyCountValue)
			};
		}

		private const string EventIdValue = "GroupConcurrency";

		private const string GroupNameKey = "GN";

		private const string NotificationTypeKey = "NT";

		private const string ElapsedTimeKey = "GUCET";

		private const string CurrentConcurrencyCountKey = "GUCC";

		private const string PrevConcurrencyCountKey = "GUCPC";

		private readonly NotificationType notificationType;

		private readonly long elapsedTime;

		private readonly int currentConcurrencyCountValue;

		private readonly int previousConcurrencyCountValue;

		private readonly string groupNameValue;
	}
}
