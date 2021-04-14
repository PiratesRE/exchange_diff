using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PendingRequestChannelLogEvent : ILogEvent
	{
		internal PendingRequestChannelLogEvent(string smtpAddress, string channelId)
		{
			this.smtpAddress = smtpAddress;
			this.channelId = channelId;
		}

		public string EventId
		{
			get
			{
				return "PendingRequestChannel";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("SmtpAddress", ExtensibleLogger.FormatPIIValue(this.smtpAddress)),
				new KeyValuePair<string, object>("ChannelId", this.channelId)
			};
		}

		private readonly string smtpAddress;

		private readonly string channelId;
	}
}
