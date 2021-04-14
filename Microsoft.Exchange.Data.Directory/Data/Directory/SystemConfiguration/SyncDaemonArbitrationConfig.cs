using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class SyncDaemonArbitrationConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncDaemonArbitrationConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SyncDaemonArbitrationConfig.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public Version MinVersion
		{
			get
			{
				return SyncDaemonArbitrationConfig.IntToVersion((int)this[SyncDaemonArbitrationConfigSchema.MinVersion]);
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.MinVersion] = SyncDaemonArbitrationConfig.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public Version MaxVersion
		{
			get
			{
				return SyncDaemonArbitrationConfig.IntToVersion((int)this[SyncDaemonArbitrationConfigSchema.MaxVersion]);
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.MaxVersion] = SyncDaemonArbitrationConfig.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public int ActiveInstanceSleepInterval
		{
			get
			{
				return (int)this[SyncDaemonArbitrationConfigSchema.ActiveInstanceSleepInterval];
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.ActiveInstanceSleepInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PassiveInstanceSleepInterval
		{
			get
			{
				return (int)this[SyncDaemonArbitrationConfigSchema.PassiveInstanceSleepInterval];
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.PassiveInstanceSleepInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsEnabled
		{
			get
			{
				return (bool)this[SyncDaemonArbitrationConfigSchema.IsEnabled];
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.IsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHalted
		{
			get
			{
				return (bool)this[SyncDaemonArbitrationConfigSchema.IsHalted];
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.IsHalted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHaltRecoveryDisabled
		{
			get
			{
				return (bool)this[SyncDaemonArbitrationConfigSchema.IsHaltRecoveryDisabled];
			}
			set
			{
				this[SyncDaemonArbitrationConfigSchema.IsHaltRecoveryDisabled] = value;
			}
		}

		private static Version IntToVersion(int value)
		{
			int revision = value & 255;
			int build = value >> 8 & 8191;
			int minor = value >> 21 & 15;
			int major = value >> 25 & 63;
			return new Version(major, minor, build, revision);
		}

		private static int VersionToInt(Version value)
		{
			return (value.Revision & 255) | (value.Build & 8191) << 8 | (value.Minor & 15) << 21 | (value.Major & 63) << 25;
		}

		private static SyncDaemonArbitrationConfigSchema schema = ObjectSchema.GetInstance<SyncDaemonArbitrationConfigSchema>();

		private static string mostDerivedClass = "msExchSyncDaemonArbitrationConfig";
	}
}
