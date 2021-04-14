using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class DumpsterStatisticsEntry
	{
		internal DumpsterStatisticsEntry(string server, long ticksOldestItem, long queueSize, int numberOfItems)
		{
			this.m_server = server;
			this.m_queueSize = queueSize;
			this.m_numberOfItems = numberOfItems;
			this.m_oldestItem = DumpsterStatisticsEntry.ToNullableLocalDateTime(new DateTime(ticksOldestItem));
		}

		public string Server
		{
			get
			{
				return this.m_server;
			}
		}

		public DateTime? OldestItem
		{
			get
			{
				return this.m_oldestItem;
			}
		}

		public long QueueSize
		{
			get
			{
				return this.m_queueSize;
			}
		}

		public int NumberOfItems
		{
			get
			{
				return this.m_numberOfItems;
			}
		}

		public override string ToString()
		{
			if (this.m_oldestItem != null)
			{
				return string.Format("{0}({1};{2};{3}KB)", new object[]
				{
					this.m_server,
					this.m_oldestItem,
					this.m_numberOfItems,
					this.m_queueSize
				});
			}
			return string.Format("{0}({1};{2}KB)", this.m_server, this.m_numberOfItems, this.m_queueSize);
		}

		internal static bool IsValidDateTime(DateTime dateTime)
		{
			return dateTime > DumpsterStatisticsEntry.s_minDateTime && dateTime < DateTime.MaxValue;
		}

		internal static DateTime? ToNullableLocalDateTime(DateTime dateTime)
		{
			if (DumpsterStatisticsEntry.IsValidDateTime(dateTime))
			{
				return new DateTime?(dateTime.ToLocalTime());
			}
			return null;
		}

		private string m_server;

		private DateTime? m_oldestItem;

		private long m_queueSize;

		private int m_numberOfItems;

		private static readonly DateTime s_minDateTime = DateTime.FromFileTimeUtc(0L);
	}
}
