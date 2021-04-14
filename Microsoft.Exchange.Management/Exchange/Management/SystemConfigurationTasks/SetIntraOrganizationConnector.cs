using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "IntraOrganizationConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetIntraOrganizationConnector : SetSystemConfigurationObjectTask<IntraOrganizationConnectorIdParameter, IntraOrganizationConnector>
	{
		[Parameter]
		public MultiValuedProperty<SmtpDomain> TargetAddressDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)base.Fields[IntraOrganizationConnectorSchema.TargetAddressDomains];
			}
			set
			{
				base.Fields[IntraOrganizationConnectorSchema.TargetAddressDomains] = value;
			}
		}

		[Parameter]
		public Uri DiscoveryEndpoint
		{
			get
			{
				return (Uri)base.Fields[IntraOrganizationConnectorSchema.DiscoveryEndpoint];
			}
			set
			{
				base.Fields[IntraOrganizationConnectorSchema.DiscoveryEndpoint] = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields[IntraOrganizationConnectorSchema.Enabled];
			}
			set
			{
				base.Fields[IntraOrganizationConnectorSchema.Enabled] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetIntraOrganizationConnector(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			foreach (ADPropertyDefinition adpropertyDefinition in SetIntraOrganizationConnector.setProperties)
			{
				if (base.Fields.IsModified(adpropertyDefinition))
				{
					this.DataObject[adpropertyDefinition] = base.Fields[adpropertyDefinition];
				}
			}
			if (NewIntraOrganizationConnector.DomainExists(this.DataObject.TargetAddressDomains, this.ConfigurationSession, new Guid?(this.DataObject.Guid)))
			{
				base.WriteError(new DuplicateIntraOrganizationConnectorDomainException(base.FormatMultiValuedProperty(this.DataObject.TargetAddressDomains)), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		private static readonly PropertyDefinition[] setProperties = new PropertyDefinition[]
		{
			IntraOrganizationConnectorSchema.TargetAddressDomains,
			IntraOrganizationConnectorSchema.DiscoveryEndpoint,
			IntraOrganizationConnectorSchema.Enabled
		};
	}
}
