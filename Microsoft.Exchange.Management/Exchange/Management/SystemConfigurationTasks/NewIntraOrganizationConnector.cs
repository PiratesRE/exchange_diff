using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "IntraOrganizationConnector", SupportsShouldProcess = true)]
	public sealed class NewIntraOrganizationConnector : NewMultitenancySystemConfigurationObjectTask<IntraOrganizationConnector>
	{
		[Parameter(Mandatory = true)]
		public MultiValuedProperty<SmtpDomain> TargetAddressDomains
		{
			get
			{
				return this.DataObject.TargetAddressDomains;
			}
			set
			{
				this.DataObject.TargetAddressDomains = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Uri DiscoveryEndpoint
		{
			get
			{
				return this.DataObject.DiscoveryEndpoint;
			}
			set
			{
				this.DataObject.DiscoveryEndpoint = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewIntraOrganizationConnector(base.Name, base.FormatMultiValuedProperty(this.TargetAddressDomains));
			}
		}

		internal static bool DomainExists(MultiValuedProperty<SmtpDomain> domains, IConfigurationSession configurationSession)
		{
			return NewIntraOrganizationConnector.DomainExists(domains, configurationSession, null);
		}

		internal static bool DomainExists(MultiValuedProperty<SmtpDomain> domains, IConfigurationSession configurationSession, Guid? objectToExclude)
		{
			List<ComparisonFilter> list = new List<ComparisonFilter>(domains.Count);
			foreach (SmtpDomain smtpDomain in domains)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, OrganizationRelationshipSchema.DomainNames, smtpDomain.Domain));
			}
			QueryFilter queryFilter;
			if (list.Count == 1)
			{
				queryFilter = list[0];
			}
			else
			{
				queryFilter = new OrFilter(list.ToArray());
			}
			if (objectToExclude != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, objectToExclude.Value),
					queryFilter
				});
			}
			IntraOrganizationConnector[] array = configurationSession.Find<IntraOrganizationConnector>(configurationSession.GetOrgContainerId(), QueryScope.SubTree, queryFilter, null, 1);
			return array.Length > 0;
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADObjectId containerId = IntraOrganizationConnector.GetContainerId(this.ConfigurationSession);
			if (this.ConfigurationSession.Read<Container>(containerId) == null)
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				OrganizationId currentOrganizationId = this.ConfigurationSession.SessionSettings.CurrentOrganizationId;
				Container container = new Container();
				container.OrganizationId = currentOrganizationId;
				container.SetId(containerId);
				configurationSession.Save(container);
			}
			IntraOrganizationConnector intraOrganizationConnector = (IntraOrganizationConnector)base.PrepareDataObject();
			intraOrganizationConnector.SetId((IConfigurationSession)base.DataSession, base.Name);
			return intraOrganizationConnector;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (NewIntraOrganizationConnector.DomainExists(this.DataObject.TargetAddressDomains, this.ConfigurationSession))
			{
				base.WriteError(new DuplicateIntraOrganizationConnectorDomainException(base.FormatMultiValuedProperty(this.DataObject.TargetAddressDomains)), ErrorCategory.InvalidOperation, base.Name);
			}
			TaskLogger.LogExit();
		}
	}
}
