using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class EdgeSyncServiceConfig : ADContainer
	{
		public string SiteName
		{
			get
			{
				if (string.IsNullOrEmpty(this.siteName))
				{
					string[] array = this.Identity.ToString().Split(new char[]
					{
						'/'
					});
					if (array.Length == 5)
					{
						this.siteName = array[3];
					}
				}
				return this.siteName;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConfigurationSyncInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.ConfigurationSyncInterval];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.ConfigurationSyncInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RecipientSyncInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.RecipientSyncInterval];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.RecipientSyncInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LockDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.LockDuration];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LockDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LockRenewalDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.LockRenewalDuration];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LockRenewalDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan OptionDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.OptionDuration];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.OptionDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan CookieValidDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.CookieValidDuration];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.CookieValidDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan FailoverDCInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.FailoverDCInterval];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.FailoverDCInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LogEnabled
		{
			get
			{
				return (bool)this[EdgeSyncServiceConfigSchema.LogEnabled];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[EdgeSyncServiceConfigSchema.LogMaxAge];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[EdgeSyncServiceConfigSchema.LogMaxDirectorySize];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[EdgeSyncServiceConfigSchema.LogMaxFileSize];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EdgeSyncLoggingLevel LogLevel
		{
			get
			{
				return (EdgeSyncLoggingLevel)this[EdgeSyncServiceConfigSchema.LogLevel];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LogPath
		{
			get
			{
				return (string)this[EdgeSyncServiceConfigSchema.LogPath];
			}
			set
			{
				this[EdgeSyncServiceConfigSchema.LogPath] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return EdgeSyncServiceConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchEdgeSyncServiceConfig";
			}
		}

		internal static bool ValidLogSizeCompatibility(Unlimited<ByteQuantifiedSize> logMaxFileSize, Unlimited<ByteQuantifiedSize> logMaxDirectorySize, ADObjectId siteId, ITopologyConfigurationSession session)
		{
			ByteQuantifiedSize value = new ByteQuantifiedSize(2147483647UL);
			if ((!logMaxFileSize.IsUnlimited && logMaxFileSize.Value > value) || (!logMaxDirectorySize.IsUnlimited && logMaxDirectorySize.Value > value))
			{
				AndFilter filter = new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId)
				});
				foreach (Server server in session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0))
				{
					ServerVersion a = new ServerVersion(server.VersionNumber);
					if (ServerVersion.Compare(a, EdgeSyncServiceConfig.LogSizeBreakageVersion) < 0)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private const string MostDerivedObjectClassInternal = "msExchEdgeSyncServiceConfig";

		private const char CommonNameSeperatorChar = '/';

		public const string CommonName = "EdgeSyncService";

		private static readonly ServerVersion LogSizeBreakageVersion = new ServerVersion(14, 1, 187, 0);

		private static readonly EdgeSyncServiceConfigSchema schema = ObjectSchema.GetInstance<EdgeSyncServiceConfigSchema>();

		private string siteName;
	}
}
