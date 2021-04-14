using System;
using System.DirectoryServices.Protocols;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[DataContract]
	internal class ServerInfo : IExtensibleDataObject
	{
		public ServerInfo(string serverFqdn, string partitionFqdn, int port)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			ArgumentValidator.ThrowIfOutOfRange<int>("port", port, 0, 65535);
			this.fqdn = serverFqdn;
			this.partitionFqdn = partitionFqdn;
			this.Port = port;
		}

		[DataMember]
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

		[DataMember]
		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
			internal set
			{
				this.fqdn = value;
			}
		}

		[DataMember]
		public int Port { get; internal set; }

		[DataMember]
		public string PartitionFqdn
		{
			get
			{
				return this.partitionFqdn;
			}
			internal set
			{
				this.partitionFqdn = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
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

		[DataMember(EmitDefaultValue = false)]
		public int DnsWeight
		{
			get
			{
				return this.dnsWeight;
			}
			set
			{
				this.dnsWeight = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int AuthType
		{
			get
			{
				return this.authType;
			}
			set
			{
				this.authType = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
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

		[DataMember(EmitDefaultValue = false)]
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

		[DataMember(EmitDefaultValue = false)]
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

		[DataMember(EmitDefaultValue = false)]
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

		public ExtensionDataObject ExtensionData { get; set; }

		public ADServerInfo ToADServerInfo()
		{
			return new ADServerInfo(this.Fqdn, this.PartitionFqdn, this.Port, this.WritableNC, this.DnsWeight, (AuthType)this.AuthType, this.IsServerSuitable)
			{
				ConfigNC = this.ConfigNC,
				RootDomainNC = this.RootDomainNC,
				SchemaNC = this.SchemaNC,
				SiteName = this.SiteName,
				IsServerSuitable = this.IsServerSuitable
			};
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}", this.Fqdn, this.Port, this.PartitionFqdn ?? string.Empty);
		}

		private const int DefaultAuthType = 9;

		private string fqdn;

		private string partitionFqdn;

		private string siteName;

		private string writableNC;

		private string configNC;

		private string rootNC;

		private string schemaNC;

		private int authType = 9;

		private int dnsWeight = 100;

		private bool isServerSuitable;
	}
}
