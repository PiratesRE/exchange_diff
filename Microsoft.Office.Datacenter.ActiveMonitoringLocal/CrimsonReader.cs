using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal sealed class CrimsonReader<T> : CrimsonOperation<T> where T : IPersistence, new()
	{
		internal CrimsonReader() : this(null, null, null)
		{
		}

		internal CrimsonReader(string serviceName) : this(serviceName, null, null)
		{
		}

		internal CrimsonReader(string serviceName, EventBookmark bookmark) : this(serviceName, bookmark, null)
		{
		}

		internal CrimsonReader(string serviceName, EventBookmark bookmark, string channelName) : base(bookmark, channelName)
		{
			this.ServiceName = serviceName;
		}

		public string ServiceName { get; set; }

		internal bool EndOfEventsReached { get; set; }

		internal bool IsReverseDirection { get; set; }

		internal override void Cleanup()
		{
			if (this.reader != null)
			{
				this.reader.Dispose();
				this.reader = null;
			}
		}

		internal T ReadNext()
		{
			return this.ReadNext(false);
		}

		internal T ReadNext(bool isForce)
		{
			T result = default(T);
			if (!this.EndOfEventsReached || isForce)
			{
				this.Initialize(isForce);
				using (EventRecord eventRecord = this.reader.ReadEvent())
				{
					if (eventRecord != null)
					{
						result = base.EventToObject(eventRecord);
					}
					else
					{
						this.EndOfEventsReached = true;
					}
				}
			}
			return result;
		}

		internal T ReadLast()
		{
			T result = default(T);
			this.Initialize(false);
			this.reader.Seek(SeekOrigin.End, 0L);
			using (EventRecord eventRecord = this.reader.ReadEvent())
			{
				if (eventRecord != null)
				{
					result = base.EventToObject(eventRecord);
				}
			}
			return result;
		}

		internal IEnumerable<T> ReadAll()
		{
			T o = default(T);
			for (;;)
			{
				o = this.ReadNext(false);
				if (this.EndOfEventsReached)
				{
					break;
				}
				yield return o;
			}
			yield break;
		}

		protected override string GetDefaultXPathQuery()
		{
			return CrimsonHelper.BuildXPathQueryString(base.ChannelName, this.ServiceName, base.QueryStartTime, base.QueryEndTime, base.QueryUserPropertyCondition);
		}

		private void Initialize()
		{
			this.Initialize(false);
		}

		private void Initialize(bool force)
		{
			if (!base.IsInitialized || force)
			{
				this.Cleanup();
				EventLogQuery queryObject = base.GetQueryObject();
				if (queryObject != null)
				{
					queryObject.ReverseDirection = this.IsReverseDirection;
				}
				EventBookmark bookMark = base.BookMark;
				this.reader = new EventLogReader(queryObject, bookMark);
				base.IsInitialized = true;
				this.EndOfEventsReached = false;
			}
		}

		private EventLogReader reader;
	}
}
