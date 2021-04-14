using System;
using System.Collections.Generic;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.Management.EdgeSync
{
	internal class EhfCompanyRecord
	{
		internal EhfCompanyRecord(Company company, IEnumerable<Domain> domains)
		{
			this.company = company;
			this.domains = new List<DomainSyncRecord>(DomainSyncRecord.CreateDomainSyncRecordList(domains));
		}

		public IList<DomainSyncRecord> Domains
		{
			get
			{
				return this.domains;
			}
		}

		public IList<string> GatewayIPAddresses
		{
			get
			{
				if (this.company != null && this.company.Settings != null && this.company.Settings.OnPremiseGatewayIPList != null && this.company.Settings.OnPremiseGatewayIPList.IPList != null)
				{
					return this.company.Settings.OnPremiseGatewayIPList.IPList;
				}
				return EhfCompanyRecord.emptyIpAddressList;
			}
		}

		public IList<string> InternalIPAddresses
		{
			get
			{
				if (this.company != null && this.company.Settings != null && this.company.Settings.InternalServerIPList != null && this.company.Settings.InternalServerIPList.IPList != null)
				{
					return this.company.Settings.InternalServerIPList.IPList;
				}
				return EhfCompanyRecord.emptyIpAddressList;
			}
		}

		public bool IPSkiplistingEnabled
		{
			get
			{
				return this.company != null && this.company.Settings != null && this.company.Settings.SkipList != null && this.company.Settings.SkipList.Value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.company != null && this.company.IsEnabled;
			}
		}

		public string CompanyId
		{
			get
			{
				if (this.company != null)
				{
					return this.company.CompanyId.ToString();
				}
				return string.Empty;
			}
		}

		public string CompanyName
		{
			get
			{
				if (this.company != null)
				{
					return this.company.Name;
				}
				return string.Empty;
			}
		}

		public Guid Guid
		{
			get
			{
				if (this.company != null && this.company.CompanyGuid != null)
				{
					return this.company.CompanyGuid.Value;
				}
				return Guid.Empty;
			}
		}

		private static readonly List<string> emptyIpAddressList = new List<string>();

		private Company company;

		private List<DomainSyncRecord> domains;
	}
}
