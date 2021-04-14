using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.EdgeSync
{
	public class ExchangeTenantRecord
	{
		public ExchangeTenantRecord(OrganizationIdParameter identity, SyncedPerimeterConfig config, IEnumerable<SyncedAcceptedDomain> acceptedDomains)
		{
			this.organization = identity;
			this.perimeterConfig = config;
			this.acceptedDomains = new Dictionary<string, SyncedAcceptedDomain>();
			this.domains = new List<DomainSyncRecord>();
			foreach (SyncedAcceptedDomain syncedAcceptedDomain in acceptedDomains)
			{
				DomainSyncRecord domainSyncRecord = new DomainSyncRecord(syncedAcceptedDomain);
				this.domains.Add(domainSyncRecord);
				this.acceptedDomains.Add(domainSyncRecord.Name, syncedAcceptedDomain);
			}
		}

		public SyncedPerimeterConfig PerimeterConfig
		{
			get
			{
				return this.perimeterConfig;
			}
		}

		public Dictionary<string, SyncedAcceptedDomain> AcceptedDomains
		{
			get
			{
				return this.acceptedDomains;
			}
		}

		public OrganizationIdParameter Organization
		{
			get
			{
				return this.organization;
			}
		}

		public IList<string> GatewayIPAddresses
		{
			get
			{
				if (this.gatewayServerIpAddresses == null)
				{
					this.gatewayServerIpAddresses = Utils.ConvertIPAddresssesToStrings(this.PerimeterConfig.GatewayIPAddresses);
				}
				return this.gatewayServerIpAddresses;
			}
		}

		public IList<string> InternalIPAddresses
		{
			get
			{
				if (this.internalServerIpAddresses == null)
				{
					this.internalServerIpAddresses = Utils.ConvertIPAddresssesToStrings(this.PerimeterConfig.InternalServerIPAddresses);
				}
				return this.internalServerIpAddresses;
			}
		}

		public bool IPSkiplistingEnabled
		{
			get
			{
				return this.PerimeterConfig.IPSkiplistingEnabled;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public string CompanyId
		{
			get
			{
				return this.PerimeterConfig.PerimeterOrgId;
			}
		}

		public string CompanyName
		{
			get
			{
				string text = this.PerimeterConfig.Identity.ToString();
				return text.Substring(0, text.IndexOf('\\'));
			}
		}

		public Guid Guid
		{
			get
			{
				return this.PerimeterConfig.Guid;
			}
		}

		internal IList<DomainSyncRecord> Domains
		{
			get
			{
				if (this.domains == null)
				{
					this.domains = DomainSyncRecord.CreateDomainSyncRecordList(this.AcceptedDomains.Values);
				}
				return this.domains;
			}
		}

		private SyncedPerimeterConfig perimeterConfig;

		private OrganizationIdParameter organization;

		private Dictionary<string, SyncedAcceptedDomain> acceptedDomains;

		private IList<string> gatewayServerIpAddresses;

		private IList<string> internalServerIpAddresses;

		private IList<DomainSyncRecord> domains;
	}
}
