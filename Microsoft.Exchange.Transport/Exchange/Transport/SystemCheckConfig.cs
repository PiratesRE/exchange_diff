using System;
using System.Collections.Specialized;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport
{
	internal class SystemCheckConfig : TransportAppConfig
	{
		public SystemCheckConfig(NameValueCollection appSettings = null) : base(appSettings)
		{
			this.isSystemCheckEnabled = base.GetConfigBool("SystemCheckEnabled", true);
			this.isDiskSystemCheckEnabled = base.GetConfigBool("DiskSystemCheckEnabled", true);
			this.lockedVolumeCheckRetryInterval = base.GetConfigTimeSpan("LockedVolumeCheckRetryInterval", SystemCheckConfig.MinLockedVolumeCheckRetryInterval, SystemCheckConfig.MaxLockedVolumeCheckRetryInterval, SystemCheckConfig.DefaultLockedVolumeCheckRetryInterval);
			this.lockedVolumeCheckRetryCount = base.GetConfigInt("LockedVolumeCheckRetryCount", 0, 20, 10);
		}

		public bool IsSystemCheckEnabled
		{
			get
			{
				return this.isSystemCheckEnabled;
			}
		}

		public bool IsDiskSystemCheckEnabled
		{
			get
			{
				return this.isDiskSystemCheckEnabled;
			}
		}

		public TimeSpan LockedVolumeCheckRetryInterval
		{
			get
			{
				return this.lockedVolumeCheckRetryInterval;
			}
		}

		public int LockedVolumeCheckRetryCount
		{
			get
			{
				return this.lockedVolumeCheckRetryCount;
			}
		}

		public const string SystemCheckEnabledLabel = "SystemCheckEnabled";

		public const string DiskSystemCheckEnabledLabel = "DiskSystemCheckEnabled";

		public const string LockedVolumeCheckRetryIntervalLabel = "LockedVolumeCheckRetryInterval";

		public const string LockedVolumeCheckRetryCountLabel = "LockedVolumeCheckRetryCount";

		public const bool DefaultSystemCheckEnabled = true;

		public const bool DefaultDiskSystemCheckEnabled = true;

		public const int MinLockedVolumeCheckRetryCount = 0;

		public const int DefaultLockedVolumeCheckRetryCount = 10;

		public const int MaxLockedVolumeCheckRetryCount = 20;

		public static readonly TimeSpan MinLockedVolumeCheckRetryInterval = TimeSpan.Zero;

		public static readonly TimeSpan DefaultLockedVolumeCheckRetryInterval = TimeSpan.FromSeconds(3.0);

		public static readonly TimeSpan MaxLockedVolumeCheckRetryInterval = TimeSpan.FromSeconds(10.0);

		private readonly bool isSystemCheckEnabled;

		private readonly bool isDiskSystemCheckEnabled;

		private readonly TimeSpan lockedVolumeCheckRetryInterval;

		private readonly int lockedVolumeCheckRetryCount;
	}
}
