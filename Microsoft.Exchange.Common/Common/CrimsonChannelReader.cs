using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.Exchange.Common
{
	internal class CrimsonChannelReader : IDisposable
	{
		public CrimsonChannelReader(string channelName, string channelPath, string xPathQuery) : this(new EventLogQuery(CrimsonChannelReader.GetChannelPath(channelName, channelPath), PathType.LogName, xPathQuery), null)
		{
		}

		public CrimsonChannelReader(EventLogQuery query, EventLogReaderAdapter adapter = null)
		{
			this.Query = query;
			this.reader = (adapter ?? new EventLogReaderAdapter(this.Query));
		}

		public static string GenerateXPathFilterForTimeRange(string channelName, string channelPath, DateTime createTime, DateTime endTime, string additionalConstraints = null)
		{
			return string.Format("<QueryList><Query Id=\"0\" Path=\"{0}\"><Select Path=\"{0}\">*[System[TimeCreated[@SystemTime&gt;='{1}' and @SystemTime&lt;='{2}']]{3}]</Select></Query></QueryList>", new object[]
			{
				CrimsonChannelReader.GetChannelPath(channelName, channelPath),
				createTime.ToString("o"),
				endTime.ToString("o"),
				additionalConstraints ?? string.Empty
			});
		}

		public static string GenerateXPathFilterForFailedEventsInTimeRange(string channelName, string channelPath, DateTime createTime, DateTime endTime, string additionalConstraints = null)
		{
			return string.Format("<QueryList><Query Id=\"0\" Path=\"{0}\"><Select Path=\"{0}\">*[System[(Level=1  or Level=2 or Level=3) and TimeCreated[@SystemTime&gt;='{1}' and @SystemTime&lt;='{2}']]{3}]</Select></Query></QueryList>", new object[]
			{
				CrimsonChannelReader.GetChannelPath(channelName, channelPath),
				createTime.ToString("o"),
				endTime.ToString("o"),
				additionalConstraints ?? string.Empty
			});
		}

		public IEnumerable<EventRecord> ReadAll()
		{
			for (;;)
			{
				EventRecord record = this.reader.ReadEvent();
				if (record == null)
				{
					break;
				}
				yield return record;
			}
			yield break;
		}

		public EventRecord ReadOne()
		{
			return this.reader.ReadEvent();
		}

		public IEnumerable<EventRecord> ReadFirstNEvents(int n)
		{
			while (n > 0)
			{
				EventRecord record = this.reader.ReadEvent();
				if (record == null)
				{
					break;
				}
				n--;
				yield return record;
			}
			yield break;
		}

		public void Dispose()
		{
			if (this.reader != null)
			{
				this.reader.Dispose();
				this.reader = null;
			}
		}

		protected static string GetChannelPath(string channelName, string channelPath)
		{
			if (channelName == null)
			{
				throw new ArgumentNullException("channelName");
			}
			if (channelPath == null)
			{
				throw new ArgumentNullException("channelPath");
			}
			return string.Format("{0}/{1}", channelName, channelPath);
		}

		private const string XPathTimeRangeFilter = "<QueryList><Query Id=\"0\" Path=\"{0}\"><Select Path=\"{0}\">*[System[TimeCreated[@SystemTime&gt;='{1}' and @SystemTime&lt;='{2}']]{3}]</Select></Query></QueryList>";

		private const string XPathFailedEventsTimeRangeFilter = "<QueryList><Query Id=\"0\" Path=\"{0}\"><Select Path=\"{0}\">*[System[(Level=1  or Level=2 or Level=3) and TimeCreated[@SystemTime&gt;='{1}' and @SystemTime&lt;='{2}']]{3}]</Select></Query></QueryList>";

		private const string ChannelPathFormat = "{0}/{1}";

		protected readonly EventLogQuery Query;

		protected EventLogReaderAdapter reader;
	}
}
