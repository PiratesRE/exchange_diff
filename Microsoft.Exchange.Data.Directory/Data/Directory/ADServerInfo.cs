using System;
using System.DirectoryServices.Protocols;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADServerInfo : ICloneable
	{
		protected ADServerInfo(string serverFqdn, string partitionFqdn, string writableNC, int port, int dnsWeight, AuthType authType, bool isServerSuitable = true)
		{
			ArgumentValidator.ThrowIfNegative("port", port);
			ArgumentValidator.ThrowIfNegative("dnsWeight", dnsWeight);
			if (authType != AuthType.Negotiate && authType != AuthType.Anonymous && authType != AuthType.Kerberos)
			{
				throw new ArgumentException("Invalid value for authType. Supported values 'Negotiate', 'Anonymous', 'Kerberos'. Actual " + authType.ToString());
			}
			this.Mapping = -1;
			this.PartitionFqdn = partitionFqdn;
			this.Fqdn = ADServerInfo.CurrentADServerInfoMapper.GetMappedFqdn(serverFqdn);
			this.Port = ADServerInfo.CurrentADServerInfoMapper.GetMappedPortNumber(serverFqdn, string.IsNullOrEmpty(this.PartitionFqdn) ? writableNC : this.PartitionFqdn, port);
			this.authType = ADServerInfo.CurrentADServerInfoMapper.GetMappedAuthType(serverFqdn, authType);
			this.writableNC = (writableNC ?? string.Empty);
			this.configNC = string.Empty;
			this.rootNC = string.Empty;
			this.schemaNC = string.Empty;
			this.DnsWeight = dnsWeight;
			this.IsServerSuitable = isServerSuitable;
			this.fqdnPlusPort = (string.IsNullOrEmpty(this.Fqdn) ? string.Empty : (this.Fqdn + ":" + this.Port));
		}

		public ADServerInfo(string serverFqdn, int port, string writableNC = null, int dnsWeight = 100, AuthType authType = AuthType.Kerberos) : this(serverFqdn, TopologyProvider.LocalForestFqdn, writableNC, port, dnsWeight, authType, true)
		{
		}

		public ADServerInfo(string serverFqdn, string partitionFqdn, int port, string writableNC = null, int dnsWeight = 100, AuthType authType = AuthType.Kerberos, bool isServerSuitable = true) : this(serverFqdn, partitionFqdn, writableNC, port, dnsWeight, authType, isServerSuitable)
		{
		}

		public static IADServerInfoMapper CurrentADServerInfoMapper
		{
			get
			{
				return ADServerInfo.hookableInstance.Value;
			}
		}

		public string Fqdn { get; private set; }

		public string SiteName
		{
			get
			{
				return this.siteName;
			}
			internal set
			{
				this.siteName = value;
			}
		}

		public string WritableNC
		{
			get
			{
				return this.writableNC;
			}
			internal set
			{
				this.writableNC = value;
			}
		}

		public string ConfigNC
		{
			get
			{
				return this.configNC;
			}
			internal set
			{
				this.configNC = value;
			}
		}

		public string RootDomainNC
		{
			get
			{
				return this.rootNC;
			}
			internal set
			{
				this.rootNC = value;
			}
		}

		public int Mapping
		{
			get
			{
				return this.mapping;
			}
			internal set
			{
				this.mapping = value;
			}
		}

		public string PartitionFqdn { get; private set; }

		public string SchemaNC
		{
			get
			{
				return this.schemaNC;
			}
			internal set
			{
				this.schemaNC = value;
			}
		}

		public int Port { get; private set; }

		public int TotalRequestCount
		{
			get
			{
				return this.totalRequests;
			}
		}

		public int DnsWeight { get; private set; }

		public string FqdnPlusPort
		{
			get
			{
				return this.fqdnPlusPort;
			}
		}

		public AuthType AuthType
		{
			get
			{
				return this.authType;
			}
		}

		internal static IDisposable SetTestHook(IADServerInfoMapper wrapper)
		{
			return ADServerInfo.hookableInstance.SetTestHook(wrapper);
		}

		internal void IncrementRequestCount()
		{
			Interlocked.Increment(ref this.totalRequests);
		}

		public ADRole GetADServerRole()
		{
			return ADServerInfo.hookableInstance.Value.GetADRole(this);
		}

		public override bool Equals(object rhs)
		{
			ADServerInfo adserverInfo = rhs as ADServerInfo;
			return adserverInfo != null && this.fqdnPlusPort.Equals(adserverInfo.fqdnPlusPort, StringComparison.OrdinalIgnoreCase) && this.AuthType.Equals(adserverInfo.AuthType);
		}

		public override int GetHashCode()
		{
			return this.fqdnPlusPort.GetHashCode();
		}

		public override string ToString()
		{
			return this.FqdnPlusPort;
		}

		public object Clone()
		{
			return this.CloneWithServerNameResolved(this.Fqdn);
		}

		public bool IsServerSuitable
		{
			get
			{
				return this.isServerSuitable;
			}
			internal set
			{
				this.isServerSuitable = value;
			}
		}

		public ADServerInfo CloneWithServerNameResolved(string serverFqdn)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			return this.InternalClone(serverFqdn, null);
		}

		public ADServerInfo CloneAsRole(ADRole role)
		{
			return this.InternalClone(null, new ADRole?(role));
		}

		private ADServerInfo InternalClone(string serverFqdn, ADRole? role = null)
		{
			if (string.IsNullOrEmpty(serverFqdn) && string.IsNullOrEmpty(this.Fqdn))
			{
				throw new NotSupportedException("ServerFqdn can't null if this.Fqdn is null.");
			}
			if (!string.IsNullOrEmpty(this.Fqdn) && !string.IsNullOrEmpty(serverFqdn) && !string.Equals(this.Fqdn, serverFqdn, StringComparison.OrdinalIgnoreCase))
			{
				throw new NotSupportedException("Current ADServerInfo already has serverName resolved. Names doesn't match");
			}
			int port = this.Port;
			if (role != null)
			{
				ADRole adserverRole = this.GetADServerRole();
				if (role.Value != adserverRole)
				{
					if (ADRole.GlobalCatalog == role.Value)
					{
						port = 3268;
					}
					else
					{
						port = 389;
					}
				}
			}
			ADServerInfo adserverInfo;
			if (!string.IsNullOrEmpty(this.PartitionFqdn))
			{
				adserverInfo = new ADServerInfo(serverFqdn ?? this.Fqdn, this.PartitionFqdn, port, this.writableNC, this.DnsWeight, this.authType, true);
			}
			else
			{
				adserverInfo = new ADServerInfo(serverFqdn ?? this.Fqdn, port, this.writableNC, this.DnsWeight, this.authType);
			}
			if (!string.IsNullOrEmpty(this.siteName))
			{
				adserverInfo.siteName = this.siteName;
			}
			if (!string.IsNullOrEmpty(this.ConfigNC))
			{
				adserverInfo.ConfigNC = this.ConfigNC;
			}
			if (!string.IsNullOrEmpty(this.RootDomainNC))
			{
				adserverInfo.RootDomainNC = this.RootDomainNC;
			}
			if (!string.IsNullOrEmpty(this.SchemaNC))
			{
				adserverInfo.SchemaNC = this.SchemaNC;
			}
			return adserverInfo;
		}

		internal LdapDirectoryIdentifier GetLdapDirectoryIdentifier()
		{
			bool flag = !string.IsNullOrEmpty(this.FqdnPlusPort);
			if (!flag)
			{
				flag = (this.PartitionFqdn == null || TopologyProvider.LocalForestFqdn.Equals(this.PartitionFqdn, StringComparison.OrdinalIgnoreCase));
			}
			string text = flag ? this.FqdnPlusPort : this.PartitionFqdn;
			if (string.IsNullOrEmpty(text))
			{
				return new LdapDirectoryIdentifier(text, this.Port, flag, false);
			}
			return new LdapDirectoryIdentifier(text, flag, false);
		}

		private static Hookable<IADServerInfoMapper> hookableInstance = Hookable<IADServerInfoMapper>.Create(true, new ADServerInfo.ADServerInfoMapper());

		private readonly string fqdnPlusPort;

		private string siteName;

		private string writableNC;

		private string configNC;

		private string rootNC;

		private string schemaNC;

		private AuthType authType;

		private bool isServerSuitable;

		[NonSerialized]
		private int totalRequests;

		[NonSerialized]
		private int mapping;

		internal class ADServerInfoMapper : IADServerInfoMapper
		{
			public ADRole GetADRole(ADServerInfo adServerInfo)
			{
				ArgumentValidator.ThrowIfNull("adServerInfo", adServerInfo);
				if (adServerInfo.Port != 389)
				{
					return ADRole.GlobalCatalog;
				}
				return ADRole.DomainController;
			}

			public string GetMappedFqdn(string serverFqdn)
			{
				return serverFqdn ?? string.Empty;
			}

			public int GetMappedPortNumber(string serverFqdn, string dnsDomainName, int portNumber)
			{
				return portNumber;
			}

			public AuthType GetMappedAuthType(string serverFqdn, AuthType authType)
			{
				return authType;
			}
		}
	}
}
