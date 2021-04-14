using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class TimedEventEntry
	{
		public DateTime EventTime
		{
			get
			{
				return this.eventTime;
			}
		}

		public long? UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public int? MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Guid EventSource
		{
			get
			{
				return this.eventSource;
			}
		}

		public int EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public TimedEventEntry.QualityOfServiceType QualityOfService
		{
			get
			{
				return this.qualityOfService;
			}
		}

		public byte[] EventData
		{
			get
			{
				return this.eventData;
			}
		}

		internal TimedEventEntry(DateTime datetime, int? mailboxNumber, Guid eventSource, int eventType, byte[] eventData)
		{
			this.eventTime = datetime;
			this.uniqueId = null;
			this.mailboxNumber = mailboxNumber;
			this.eventSource = eventSource;
			this.eventType = eventType;
			this.eventData = eventData;
			this.qualityOfService = TimedEventEntry.QualityOfServiceType.BestEfforts;
		}

		internal TimedEventEntry(DateTime datetime, long uniqueId, int? mailboxNumber, Guid eventSource, int eventType, TimedEventEntry.QualityOfServiceType qos, byte[] eventData)
		{
			this.eventTime = datetime;
			this.uniqueId = new long?(uniqueId);
			this.mailboxNumber = mailboxNumber;
			this.eventSource = eventSource;
			this.eventType = eventType;
			this.qualityOfService = qos;
			this.eventData = eventData;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append("(EventTime:");
			stringBuilder.Append(this.EventTime);
			stringBuilder.Append(")(UniqueId:");
			stringBuilder.Append((this.UniqueId != null) ? this.UniqueId.Value.ToString() : "<null>");
			stringBuilder.Append(")(MailboxNumber:");
			stringBuilder.Append((this.MailboxNumber != null) ? this.MailboxNumber.Value.ToString() : "<null>");
			stringBuilder.Append(")(EventSource:");
			stringBuilder.Append(this.EventSource);
			stringBuilder.Append(")(EventType:");
			stringBuilder.Append(this.EventType);
			stringBuilder.Append(")(QoS:");
			stringBuilder.Append(this.QualityOfService);
			if (this.EventData != null)
			{
				stringBuilder.Append(")(EventData:[Length=");
				stringBuilder.Append(this.EventData.Length);
				stringBuilder.Append("]");
				stringBuilder.AppendAsString(this.EventData, 0, (this.EventData.Length > 64) ? 64 : this.EventData.Length);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private readonly DateTime eventTime;

		private readonly long? uniqueId;

		private readonly int? mailboxNumber;

		private readonly Guid eventSource;

		private readonly int eventType;

		private readonly TimedEventEntry.QualityOfServiceType qualityOfService;

		private readonly byte[] eventData;

		public enum QualityOfServiceType
		{
			BestEfforts
		}
	}
}
