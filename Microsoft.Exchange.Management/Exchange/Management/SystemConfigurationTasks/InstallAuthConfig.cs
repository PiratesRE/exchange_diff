using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "AuthConfig")]
	public sealed class InstallAuthConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<AuthConfig>
	{
		protected override IConfigurable PrepareDataObject()
		{
			AuthConfig authConfig = (AuthConfig)base.PrepareDataObject();
			authConfig.Name = AuthConfig.ContainerName;
			authConfig.SetId((IConfigurationSession)base.DataSession, authConfig.Name);
			if (authConfig.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				FederationTrust[] array = base.GlobalConfigSession.Find<FederationTrust>(null, QueryScope.SubTree, null, null, 1);
				if (array.Length > 0)
				{
					authConfig.CurrentCertificateThumbprint = array[0].OrgPrivCertificate;
				}
			}
			return authConfig;
		}

		protected override void InternalProcessRecord()
		{
			ADObjectId childId = this.ConfigurationSession.GetOrgContainerId().GetChildId(AuthConfig.ContainerName);
			ADRawEntry adrawEntry = this.ConfigurationSession.ReadADRawEntry(childId, new ADPropertyDefinition[]
			{
				ADObjectSchema.ObjectClass
			});
			if (adrawEntry == null)
			{
				base.InternalProcessRecord();
				return;
			}
			MultiValuedProperty<string> source = adrawEntry[ADObjectSchema.ObjectClass] as MultiValuedProperty<string>;
			if (!source.Contains(AuthConfig.ObjectClassName, StringComparer.OrdinalIgnoreCase))
			{
				Container container = this.ConfigurationSession.Read<Container>(childId);
				container.propertyBag.SetField(ADObjectSchema.ExchangeVersion, container.MaximumSupportedExchangeObjectVersion);
				((IConfigurationSession)base.DataSession).DeleteTree(container, null);
				base.InternalProcessRecord();
			}
		}
	}
}
