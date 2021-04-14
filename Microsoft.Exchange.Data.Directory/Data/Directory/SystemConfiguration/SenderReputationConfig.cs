using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class SenderReputationConfig : MessageHygieneAgentConfig
	{
		public SenderReputationConfig()
		{
			this.Socks4Ports = new MultiValuedProperty<int>(new object[]
			{
				1080,
				1081
			});
			this.Socks5Ports = new MultiValuedProperty<int>(new object[]
			{
				1080,
				1081
			});
			this.HttpConnectPorts = new MultiValuedProperty<int>(new object[]
			{
				80,
				3128,
				6588
			});
			this.HttpPostPorts = new MultiValuedProperty<int>(new object[]
			{
				80,
				3128,
				6588
			});
			this.TelnetPorts = new MultiValuedProperty<int>(new object[]
			{
				23
			});
			this.CiscoPorts = new MultiValuedProperty<int>(new object[]
			{
				23
			});
			this.WingatePorts = new MultiValuedProperty<int>(new object[]
			{
				23
			});
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SenderReputationConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SenderReputationConfig.mostDerivedClass;
			}
		}

		public int MinMessagesPerDatabaseTransaction
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MinMessagesPerDatabaseTransaction];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MinMessagesPerDatabaseTransaction] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SrlBlockThreshold
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.SrlBlockThreshold];
			}
			set
			{
				this[SenderReputationConfigSchema.SrlBlockThreshold] = value;
			}
		}

		public int MinMessagesPerTimeSlice
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MinMessagesPerTimeSlice];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MinMessagesPerTimeSlice] = value;
			}
		}

		public int TimeSliceInterval
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.TimeSliceInterval];
			}
			internal set
			{
				this[SenderReputationConfigSchema.TimeSliceInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OpenProxyDetectionEnabled
		{
			get
			{
				return this.GetFlag(1);
			}
			set
			{
				this.SetFlag(1, value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool SenderBlockingEnabled
		{
			get
			{
				return this.GetFlag(2);
			}
			set
			{
				this.SetFlag(2, value);
			}
		}

		public int OpenProxyRescanInterval
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.OpenProxyRescanInterval];
			}
			internal set
			{
				this[SenderReputationConfigSchema.OpenProxyRescanInterval] = value;
			}
		}

		public int MinReverseDnsQueryPeriod
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MinReverseDnsQueryPeriod];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MinReverseDnsQueryPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SenderBlockingPeriod
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.SenderBlockingPeriod];
			}
			set
			{
				this[SenderReputationConfigSchema.SenderBlockingPeriod] = value;
			}
		}

		public int MaxWorkQueueSize
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MaxWorkQueueSize];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MaxWorkQueueSize] = value;
			}
		}

		public int MaxIdleTime
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MaxIdleTime];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MaxIdleTime] = value;
			}
		}

		public MultiValuedProperty<int> Socks4Ports
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.Socks4Ports];
			}
			internal set
			{
				this[SenderReputationConfigSchema.Socks4Ports] = value;
			}
		}

		public MultiValuedProperty<int> Socks5Ports
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.Socks5Ports];
			}
			internal set
			{
				this[SenderReputationConfigSchema.Socks5Ports] = value;
			}
		}

		public MultiValuedProperty<int> WingatePorts
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.WingatePorts];
			}
			internal set
			{
				this[SenderReputationConfigSchema.WingatePorts] = value;
			}
		}

		public MultiValuedProperty<int> HttpConnectPorts
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.HttpConnectPorts];
			}
			internal set
			{
				this[SenderReputationConfigSchema.HttpConnectPorts] = value;
			}
		}

		public MultiValuedProperty<int> HttpPostPorts
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.HttpPostPorts];
			}
			internal set
			{
				this[SenderReputationConfigSchema.HttpPostPorts] = value;
			}
		}

		public MultiValuedProperty<int> TelnetPorts
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.TelnetPorts];
			}
			internal set
			{
				this[SenderReputationConfigSchema.TelnetPorts] = value;
			}
		}

		public MultiValuedProperty<int> CiscoPorts
		{
			get
			{
				return (MultiValuedProperty<int>)this[SenderReputationConfigSchema.CiscoPorts];
			}
			internal set
			{
				this[SenderReputationConfigSchema.CiscoPorts] = value;
			}
		}

		public int TablePurgeInterval
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.TablePurgeInterval];
			}
			internal set
			{
				this[SenderReputationConfigSchema.TablePurgeInterval] = value;
			}
		}

		public int MaxPendingOperations
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MaxPendingOperations];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MaxPendingOperations] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ProxyServerName
		{
			get
			{
				return (string)this[SenderReputationConfigSchema.ProxyServerIP];
			}
			set
			{
				this[SenderReputationConfigSchema.ProxyServerIP] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ProxyServerPort
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.ProxyServerPort];
			}
			set
			{
				this[SenderReputationConfigSchema.ProxyServerPort] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyType ProxyServerType
		{
			get
			{
				return (ProxyType)this[SenderReputationConfigSchema.ProxyServerType];
			}
			set
			{
				this[SenderReputationConfigSchema.ProxyServerType] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		public int MinDownloadInterval
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MinDownloadInterval];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MinDownloadInterval] = value;
			}
		}

		public int MaxDownloadInterval
		{
			get
			{
				return (int)this[SenderReputationConfigSchema.MaxDownloadInterval];
			}
			internal set
			{
				this[SenderReputationConfigSchema.MaxDownloadInterval] = value;
			}
		}

		public string SrlSettingsDatabaseFileName
		{
			get
			{
				return (string)this[SenderReputationConfigSchema.SrlSettingsDatabaseFileName];
			}
			internal set
			{
				this[SenderReputationConfigSchema.SrlSettingsDatabaseFileName] = value;
			}
		}

		public string ReputationServiceUrl
		{
			get
			{
				return (string)this[SenderReputationConfigSchema.ReputationServiceUrl];
			}
			internal set
			{
				this[SenderReputationConfigSchema.ReputationServiceUrl] = value;
			}
		}

		private bool GetFlag(int mask)
		{
			return 0 != (mask & (int)this[SenderReputationConfigSchema.OpenProxyFlags]);
		}

		private void SetFlag(int mask, bool bit)
		{
			int num = (int)this[SenderReputationConfigSchema.OpenProxyFlags];
			this[SenderReputationConfigSchema.OpenProxyFlags] = (bit ? (num | mask) : (num & ~mask));
		}

		private const int OpenProxyDetectionEnabledMask = 1;

		private const int SenderBlockingEnabledMask = 2;

		private static SenderReputationConfigSchema schema = ObjectSchema.GetInstance<SenderReputationConfigSchema>();

		private static string mostDerivedClass = "msExchSenderReputation";
	}
}
