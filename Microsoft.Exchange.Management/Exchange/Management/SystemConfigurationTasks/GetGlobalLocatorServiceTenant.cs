using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "GlobalLocatorServiceTenant", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
	public sealed class GetGlobalLocatorServiceTenant : ManageGlobalLocatorServiceBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "DomainNameParameterSet")]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		public SwitchParameter ShowDomainNames
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowDomainNames"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowDomainNames"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		public SwitchParameter UseOfflineGLS
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseOfflineGLS"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseOfflineGLS"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
			GlobalLocatorServiceTenant globalLocatorServiceTenant;
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				Guid guid = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
				if (this.UseOfflineGLS)
				{
					if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(guid, out globalLocatorServiceTenant, GlsCacheServiceMode.CacheOnly))
					{
						base.WriteGlsTenantNotFoundError(guid);
					}
				}
				else if (!glsDirectorySession.TryGetTenantInfoByOrgGuid(guid, out globalLocatorServiceTenant))
				{
					base.WriteGlsTenantNotFoundError(guid);
				}
				if (this.ShowDomainNames)
				{
					globalLocatorServiceTenant.DomainNames = this.GetDomainNames(globalLocatorServiceTenant.ExternalDirectoryOrganizationId, glsDirectorySession, globalLocatorServiceTenant.AccountForest);
				}
			}
			else
			{
				SmtpDomain smtpDomain = (SmtpDomain)base.Fields["DomainName"];
				if (this.UseOfflineGLS)
				{
					if (!glsDirectorySession.TryGetTenantInfoByDomain(smtpDomain.Domain, out globalLocatorServiceTenant, GlsCacheServiceMode.CacheOnly))
					{
						base.WriteGlsTenantNotFoundError(smtpDomain.Domain);
					}
				}
				else if (!glsDirectorySession.TryGetTenantInfoByDomain(smtpDomain.Domain, out globalLocatorServiceTenant))
				{
					base.WriteGlsTenantNotFoundError(smtpDomain.Domain);
				}
				if (this.ShowDomainNames)
				{
					globalLocatorServiceTenant.DomainNames = this.GetDomainNames(globalLocatorServiceTenant.ExternalDirectoryOrganizationId, glsDirectorySession, globalLocatorServiceTenant.AccountForest, smtpDomain.Domain);
				}
			}
			base.WriteObject(globalLocatorServiceTenant);
		}

		private AcceptedDomain[] GetAcceptedDomainsInOrg(Guid orgGuid, string accountPartitionFqdn)
		{
			AcceptedDomain[] result;
			try
			{
				PartitionId partitionId = new PartitionId(accountPartitionFqdn);
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 133, "GetAcceptedDomainsInOrg", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\GLS\\GetGlobalLocatorServiceTenant.cs");
				ExchangeConfigurationUnit[] array = tenantConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, orgGuid), null, 0);
				if (array != null && array.Length > 0)
				{
					result = tenantConfigurationSession.FindAllAcceptedDomainsInOrg(array[0].OrganizationId.ConfigurationUnit);
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				base.WriteVerbose(Strings.WarningCannotGetGlsTenantFromOrgId(orgGuid.ToString(), ex.Message));
				result = null;
			}
			return result;
		}

		private MultiValuedProperty<string> GetDomainNames(Guid orgGuid, GlsDirectorySession glsSession, string accountPartitionFqdn)
		{
			return this.GetDomainNames(orgGuid, glsSession, accountPartitionFqdn, string.Empty);
		}

		private MultiValuedProperty<string> GetDomainNames(Guid orgGuid, GlsDirectorySession glsSession, string accountPartitionFqdn, string glsDomainName)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			AcceptedDomain[] acceptedDomainsInOrg = this.GetAcceptedDomainsInOrg(orgGuid, accountPartitionFqdn);
			bool flag = false;
			GlobalLocatorServiceDomain globalLocatorServiceDomain;
			if (acceptedDomainsInOrg != null)
			{
				foreach (AcceptedDomain acceptedDomain in acceptedDomainsInOrg)
				{
					if (!string.IsNullOrEmpty(glsDomainName) && acceptedDomain.Name.Equals(glsDomainName, StringComparison.OrdinalIgnoreCase))
					{
						multiValuedProperty.Add(string.Format("{0}:{1}", acceptedDomain.Name, "ADAndGLS"));
						flag = true;
					}
					else if (this.UseOfflineGLS)
					{
						if (!glsSession.TryGetTenantDomainFromDomainFqdn(acceptedDomain.Name, out globalLocatorServiceDomain, GlsCacheServiceMode.CacheOnly))
						{
							multiValuedProperty.Add(string.Format("{0}:{1}", acceptedDomain.Name, "ADOnly"));
						}
						else
						{
							multiValuedProperty.Add(string.Format("{0}:{1}", acceptedDomain.Name, "ADAndGLS"));
						}
					}
					else if (!glsSession.TryGetTenantDomainFromDomainFqdn(acceptedDomain.Name, out globalLocatorServiceDomain))
					{
						multiValuedProperty.Add(string.Format("{0}:{1}", acceptedDomain.Name, "ADOnly"));
					}
					else
					{
						multiValuedProperty.Add(string.Format("{0}:{1}", acceptedDomain.Name, "ADAndGLS"));
					}
				}
			}
			if (!flag && !string.IsNullOrEmpty(glsDomainName) && glsSession.TryGetTenantDomainFromDomainFqdn(glsDomainName, out globalLocatorServiceDomain))
			{
				multiValuedProperty.Add(string.Format("{0}:{1}", glsDomainName, "GlsOnly"));
			}
			return multiValuedProperty;
		}
	}
}
