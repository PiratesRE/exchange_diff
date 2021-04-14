using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal class TimeEntry : ITimeEntry, IDisposable
	{
		private static IDictionary<TimeId, string> BuildShortenedIds()
		{
			Type typeFromHandle = typeof(TimeId);
			Type typeFromHandle2 = typeof(TimeIdAttribute);
			int length = Enum.GetValues(typeFromHandle).Length;
			IDictionary<TimeId, string> dictionary = new Dictionary<TimeId, string>(length);
			foreach (FieldInfo fieldInfo in from x in typeFromHandle.GetTypeInfo().DeclaredFields
			where x.IsStatic && x.IsPublic
			select x)
			{
				TimeIdAttribute[] array = (TimeIdAttribute[])fieldInfo.GetCustomAttributes(typeFromHandle2, false);
				TimeId key = (TimeId)fieldInfo.GetValue(null);
				dictionary.Add(key, array[0].Name);
			}
			return dictionary;
		}

		public TimeId TimeId { get; private set; }

		public DateTime StartTime { get; private set; }

		public DateTime EndTime { get; private set; }

		public int ThreadId { get; private set; }

		public TimeEntry(TimeId timeId, Action<TimeEntry> onRelease)
		{
			this.TimeId = timeId;
			this.ThreadId = ThreadIdProvider.ManagedThreadId;
			this.StartTime = TimeProvider.UtcNow;
			this.EndTime = DateTime.MinValue;
			if (onRelease == null)
			{
				throw new ArgumentNullException("onRelease");
			}
			this.onRelease = onRelease;
		}

		public void Dispose()
		{
			this.VerifyThread();
			if (this.EndTime == DateTime.MinValue)
			{
				lock (this.instanceLock)
				{
					if (this.EndTime == DateTime.MinValue)
					{
						GC.SuppressFinalize(this);
						this.EndTime = TimeProvider.UtcNow;
						this.onRelease(this);
						this.onRelease = null;
					}
				}
			}
		}

		public TimeSpan ElapsedInclusive
		{
			get
			{
				if (!(this.EndTime == DateTime.MinValue))
				{
					return this.EndTime - this.StartTime;
				}
				return TimeSpan.Zero;
			}
		}

		public TimeSpan ElapsedExclusive
		{
			get
			{
				if (this.EndTime == DateTime.MinValue)
				{
					return TimeSpan.Zero;
				}
				TimeSpan timeSpan = TimeSpan.Zero;
				if (this.children != null)
				{
					lock (this.instanceLock)
					{
						if (this.children != null)
						{
							foreach (TimeEntry timeEntry in this.children)
							{
								timeSpan += timeEntry.ElapsedInclusive;
							}
						}
					}
				}
				TimeSpan timeSpan2 = this.ElapsedInclusive - timeSpan;
				if (!(timeSpan2 > TimeSpan.Zero))
				{
					return TimeSpan.Zero;
				}
				return timeSpan2;
			}
		}

		public void AddChild(TimeEntry childEntry)
		{
			this.VerifyThread();
			lock (this.instanceLock)
			{
				if (this.children == null)
				{
					this.children = new List<TimeEntry>();
				}
				this.children.Add(childEntry);
			}
		}

		public override string ToString()
		{
			string result;
			lock (this.instanceLock)
			{
				string text = "NONE";
				if (this.children != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (TimeEntry timeEntry in this.children)
					{
						stringBuilder.Append(timeEntry.ToString() + ",");
					}
					text = stringBuilder.ToString();
				}
				if (this.TimeId == TimeId.Root)
				{
					result = string.Format("TID:{0}>>[{1}]", this.ThreadId, text);
				}
				else
				{
					result = string.Format("ID:{0},Start:{1},End:{2},Excl:{3} ms,Child:[{4}]", new object[]
					{
						TimeEntry.ShortenedIds[this.TimeId],
						this.StartTime.ToString("T"),
						(this.EndTime == DateTime.MinValue) ? "<NOT DONE>" : this.EndTime.ToString("T"),
						(int)this.ElapsedExclusive.TotalMilliseconds,
						text
					});
				}
			}
			return result;
		}

		internal void VerifyThread()
		{
			if (ThreadIdProvider.ManagedThreadId != this.ThreadId)
			{
				throw new InvalidOperationException(string.Format("[TimeEntry] Start was called on thread {0} and was then used on thread {1}.  Must be used on single, consistent thread.", this.ThreadId, ThreadIdProvider.ManagedThreadId));
			}
		}

		internal List<TimeEntry> GetChildrenForTest()
		{
			return this.children;
		}

		private static readonly IDictionary<TimeId, string> ShortenedIds = TimeEntry.BuildShortenedIds();

		private object instanceLock = new object();

		private Action<TimeEntry> onRelease;

		private List<TimeEntry> children;
	}
}
