using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.AirSync
{
	internal class TimeTracker
	{
		private static bool ReadEnabled()
		{
			return GlobalSettings.TimeTrackingEnabled;
		}

		internal static void SetEnabledForTest(bool enabled)
		{
			TimeTracker.Enabled = enabled;
		}

		internal ConcurrentDictionary<int, PerThreadTimeTracker> GetPerThreadTrackersForTest()
		{
			return this.perThreadTimeTrackers;
		}

		public ITimeEntry Start(TimeId timeId)
		{
			if (!TimeTracker.Enabled)
			{
				return DummyTimeEntry.Singleton;
			}
			return this.GetTimeTracker().Start(timeId);
		}

		public override string ToString()
		{
			if (!TimeTracker.Enabled)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, PerThreadTimeTracker> keyValuePair in this.perThreadTimeTrackers)
			{
				stringBuilder.Append(keyValuePair.Value.ToString());
			}
			return stringBuilder.ToString();
		}

		public void Clear()
		{
			this.perThreadTimeTrackers.Clear();
		}

		private PerThreadTimeTracker GetTimeTracker()
		{
			return this.perThreadTimeTrackers.GetOrAdd(ThreadIdProvider.ManagedThreadId, (int threadId) => new PerThreadTimeTracker());
		}

		private ConcurrentDictionary<int, PerThreadTimeTracker> perThreadTimeTrackers = new ConcurrentDictionary<int, PerThreadTimeTracker>();

		private static bool Enabled = TimeTracker.ReadEnabled();
	}
}
