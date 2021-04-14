using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal abstract class CountConfig : ICountedConfig
	{
		protected CountConfig(bool isPromotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool isRemovable, TimeSpan idleCleanupInterval) : this(isPromotable, minActivityThreshold, timeToLive, idleTimeToLive, isRemovable, idleCleanupInterval, () => DateTime.UtcNow)
		{
		}

		protected CountConfig(bool isPromotable, int minActivityThreshold, TimeSpan timeToLive, TimeSpan idleTimeToLive, bool isRemovable, TimeSpan idleCleanupInterval, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("timeToLive", timeToLive, TimeSpan.Zero, TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleTimeToLive", idleTimeToLive, TimeSpan.Zero, TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleCleanupInterval", idleCleanupInterval, TimeSpan.Zero, TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.isPromotable = isPromotable;
			this.minActivityThreshold = minActivityThreshold;
			this.timeToLive = timeToLive;
			this.idleTimeToLive = idleTimeToLive;
			this.isRemovable = isRemovable;
			this.idleCleanupInterval = idleCleanupInterval;
			this.timeProvider = timeProvider;
			this.UpdateAccessTime();
		}

		public bool IsPromotable
		{
			get
			{
				this.UpdateAccessTime();
				return this.isPromotable;
			}
		}

		public int MinActivityThreshold
		{
			get
			{
				this.UpdateAccessTime();
				return this.minActivityThreshold;
			}
		}

		public TimeSpan TimeToLive
		{
			get
			{
				this.UpdateAccessTime();
				return this.timeToLive;
			}
		}

		public TimeSpan IdleTimeToLive
		{
			get
			{
				this.UpdateAccessTime();
				return this.idleTimeToLive;
			}
		}

		public bool IsRemovable
		{
			get
			{
				this.UpdateAccessTime();
				return this.isRemovable;
			}
		}

		public TimeSpan IdleCleanupInterval
		{
			get
			{
				this.UpdateAccessTime();
				return this.idleCleanupInterval;
			}
		}

		public bool Equals(CountConfig config)
		{
			return !object.ReferenceEquals(null, config) && (object.ReferenceEquals(this, config) || (this.isPromotable.Equals(config.isPromotable) && this.minActivityThreshold == config.minActivityThreshold && this.timeToLive.Equals(config.timeToLive) && this.idleTimeToLive.Equals(config.idleTimeToLive) && this.isRemovable.Equals(config.isRemovable) && this.idleCleanupInterval.Equals(config.idleCleanupInterval)));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is CountConfig && this.Equals(obj as CountConfig)));
		}

		public override int GetHashCode()
		{
			int num = this.isPromotable.GetHashCode();
			num = (num * 397 ^ this.minActivityThreshold);
			num = (num * 397 ^ this.timeToLive.GetHashCode());
			num = (num * 397 ^ this.idleTimeToLive.GetHashCode());
			num = (num * 397 ^ this.isRemovable.GetHashCode());
			return num * 397 ^ this.idleCleanupInterval.GetHashCode();
		}

		internal static void CleanupCachedConfig(DateTime currentTime, TimeSpan idleCleanupInterval)
		{
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleCleanupInterval", idleCleanupInterval, TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue);
			TimeSpan t = new TimeSpan(idleCleanupInterval.Ticks / 10L);
			if (currentTime - CountConfig.lastCleanupTime > t)
			{
				try
				{
					CountConfig.syncObject.EnterWriteLock();
					List<CountConfig> list = (from p in CountConfig.ExistingConfig
					where currentTime - p.Value.lastAccessTime > idleCleanupInterval
					select p.Key).ToList<CountConfig>();
					foreach (CountConfig key in list)
					{
						CountConfig.ExistingConfig.Remove(key);
					}
					CountConfig.lastCleanupTime = currentTime;
				}
				finally
				{
					if (CountConfig.syncObject.IsWriteLockHeld)
					{
						CountConfig.syncObject.ExitWriteLock();
					}
				}
			}
		}

		protected static CountConfig GetCachedObject(CountConfig config)
		{
			try
			{
				CountConfig.syncObject.EnterUpgradeableReadLock();
				CountConfig result;
				if (CountConfig.ExistingConfig.TryGetValue(config, out result))
				{
					return result;
				}
				try
				{
					CountConfig.syncObject.EnterWriteLock();
					CountConfig.ExistingConfig.Add(config, config);
				}
				finally
				{
					if (CountConfig.syncObject.IsWriteLockHeld)
					{
						CountConfig.syncObject.ExitWriteLock();
					}
				}
			}
			finally
			{
				if (CountConfig.syncObject.IsUpgradeableReadLockHeld)
				{
					CountConfig.syncObject.ExitUpgradeableReadLock();
				}
			}
			return config;
		}

		protected void UpdateAccessTime()
		{
			this.lastAccessTime = this.timeProvider();
		}

		private static readonly Dictionary<CountConfig, CountConfig> ExistingConfig = new Dictionary<CountConfig, CountConfig>();

		private static readonly ReaderWriterLockSlim syncObject = new ReaderWriterLockSlim();

		private static DateTime lastCleanupTime = DateTime.MinValue;

		private readonly bool isPromotable;

		private readonly int minActivityThreshold;

		private readonly TimeSpan timeToLive;

		private readonly TimeSpan idleTimeToLive;

		private readonly bool isRemovable;

		private readonly TimeSpan idleCleanupInterval;

		private Func<DateTime> timeProvider;

		private DateTime lastAccessTime;
	}
}
