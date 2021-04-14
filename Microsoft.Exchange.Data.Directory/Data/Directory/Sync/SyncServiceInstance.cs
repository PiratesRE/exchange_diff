using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class SyncServiceInstance : ADConfigurationObject
	{
		public ADObjectId AccountPartition
		{
			get
			{
				return (ADObjectId)this[SyncServiceInstanceSchema.AccountPartition];
			}
			set
			{
				this[SyncServiceInstanceSchema.AccountPartition] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version MinVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.MinVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.MinVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public Version MaxVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.MaxVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.MaxVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public int ActiveInstanceSleepInterval
		{
			get
			{
				return (int)this[SyncServiceInstanceSchema.ActiveInstanceSleepInterval];
			}
			set
			{
				this[SyncServiceInstanceSchema.ActiveInstanceSleepInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PassiveInstanceSleepInterval
		{
			get
			{
				return (int)this[SyncServiceInstanceSchema.PassiveInstanceSleepInterval];
			}
			set
			{
				this[SyncServiceInstanceSchema.PassiveInstanceSleepInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsEnabled
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsEnabled];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseCentralConfig
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.UseCentralConfig];
			}
			set
			{
				this[SyncServiceInstanceSchema.UseCentralConfig] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHalted
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsHalted];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsHalted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHaltRecoveryDisabled
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsHaltRecoveryDisabled];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsHaltRecoveryDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsMultiObjectCookieEnabled
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsMultiObjectCookieEnabled];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsMultiObjectCookieEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsNewCookieBlocked
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsNewCookieBlocked];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsNewCookieBlocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsUsedForTenantToServiceInstanceAssociation
		{
			get
			{
				return (bool)this[SyncServiceInstanceSchema.IsUsedForTenantToServiceInstanceAssociation];
			}
			set
			{
				this[SyncServiceInstanceSchema.IsUsedForTenantToServiceInstanceAssociation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version NewTenantMinVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.NewTenantMinVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.NewTenantMinVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public Version NewTenantMaxVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.NewTenantMaxVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.NewTenantMaxVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public Version TargetServerMinVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.TargetServerMinVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.TargetServerMinVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public Version TargetServerMaxVersion
		{
			get
			{
				return SyncServiceInstance.IntToVersion((int)this[SyncServiceInstanceSchema.TargetServerMaxVersion]);
			}
			set
			{
				this[SyncServiceInstanceSchema.TargetServerMaxVersion] = SyncServiceInstance.VersionToInt(value);
			}
		}

		[Parameter(Mandatory = false)]
		public string ForwardSyncConfigurationXML
		{
			get
			{
				return (string)this[SyncServiceInstanceSchema.ForwardSyncConfigurationXML];
			}
			set
			{
				this[SyncServiceInstanceSchema.ForwardSyncConfigurationXML] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncServiceInstance.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SyncServiceInstance.MostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static ADObjectId GetServiceInstanceObjectId(string serviceInstanceName)
		{
			return SyncServiceInstance.GetMsoSyncRootContainer().GetChildId(serviceInstanceName);
		}

		internal static ADObjectId GetMsoSyncRootContainer()
		{
			return SyncServiceInstance.MsoSyncRootContainer.Value;
		}

		internal static ADObjectId GetDivergenceContainerId(ADObjectId serviceInstanceObjectId)
		{
			return serviceInstanceObjectId.GetChildId("Divergence");
		}

		private static Version IntToVersion(int value)
		{
			int revision = value & 255;
			int build = value >> 8 & 4095;
			int minor = value >> 20 & 31;
			int major = value >> 25 & 63;
			return new Version(major, minor, build, revision);
		}

		private static int VersionToInt(Version value)
		{
			if (value.Major > SyncServiceInstance.MaxSupportedVersion.Major || value.Minor > SyncServiceInstance.MaxSupportedVersion.Minor || value.Build > SyncServiceInstance.MaxSupportedVersion.Build || value.Revision > SyncServiceInstance.MaxSupportedVersion.Revision)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			return (value.Revision & 255) | (value.Build & 4095) << 8 | (value.Minor & 31) << 20 | (value.Major & 63) << 25;
		}

		private const string DivergenceContainerName = "Divergence";

		internal static readonly string MSOSyncRootContainer = "Microsoft Exchange MSO Sync";

		internal static readonly string MostDerivedClass = "msExchMSOSyncServiceInstance";

		private static readonly SyncServiceInstanceSchema SchemaObject = ObjectSchema.GetInstance<SyncServiceInstanceSchema>();

		private static readonly Lazy<ADObjectId> MsoSyncRootContainer = new Lazy<ADObjectId>(delegate()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 265, "MsoSyncRootContainer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\SyncServiceInstance.cs");
			return topologyConfigurationSession.GetRootDomainNamingContext().GetChildId(SyncServiceInstance.MSOSyncRootContainer);
		}, LazyThreadSafetyMode.PublicationOnly);

		internal static readonly Version MaxSupportedVersion = new Version(63, 31, 4095, 255);
	}
}
