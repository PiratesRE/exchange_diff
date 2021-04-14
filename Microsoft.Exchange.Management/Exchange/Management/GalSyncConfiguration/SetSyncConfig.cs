using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.GalSyncConfiguration
{
	[Cmdlet("Set", "SyncConfig", SupportsShouldProcess = true, DefaultParameterSetName = "Managed")]
	public sealed class SetSyncConfig : SetMultitenancySingletonSystemConfigurationObjectTask<SyncOrganization>
	{
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "Federated", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "Managed", ValueFromPipeline = true)]
		public new OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.FederatedTenant)
			{
				if (this.DataObject.IsChanged(SyncOrganizationSchema.DisableWindowsLiveID) || this.DataObject.IsChanged(SyncOrganizationSchema.PasswordFilePath) || this.DataObject.IsChanged(SyncOrganizationSchema.ResetPasswordOnNextLogon))
				{
					base.WriteError(new ArgumentException(Strings.ErrorParameterInvalidForFederatedTenant(this.DataObject.Identity.ToString())), (ErrorCategory)1000, null);
				}
			}
			else if (this.DataObject.IsChanged(SyncOrganizationSchema.FederatedIdentitySourceADAttribute))
			{
				base.WriteError(new ArgumentException(Strings.ErrorParameterInvalidForManagedTenant(this.DataObject.Identity.ToString())), (ErrorCategory)1000, null);
			}
			if (this.DataObject.IsChanged(SyncOrganizationSchema.ProvisioningDomain))
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				ADPagedReader<AcceptedDomain> adpagedReader = configurationSession.FindPaged<AcceptedDomain>(null, QueryScope.SubTree, null, null, 0);
				bool flag = false;
				using (IEnumerator<AcceptedDomain> enumerator = adpagedReader.GetEnumerator())
				{
					while (enumerator.MoveNext() && !flag)
					{
						if (enumerator.Current.DomainName.Match(this.DataObject.ProvisioningDomain.Address) >= 0)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					base.WriteError(new ArgumentException(Strings.ErrorProvisioningDomainNotMatchAcceptedDomain(this.DataObject.ProvisioningDomain.Address, this.DataObject.Identity.ToString())), (ErrorCategory)1000, null);
				}
			}
			if (this.DataObject.IsChanged(SyncOrganizationSchema.FederatedIdentitySourceADAttribute) && !string.Equals(this.DataObject.FederatedIdentitySourceADAttribute, "objectGuid", StringComparison.OrdinalIgnoreCase))
			{
				this.WriteWarning(Strings.WarningChangeOfFederatedIdentitySourceADAttribute("objectGuid"));
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSyncConfig(this.DataObject.Identity.ToString());
			}
		}
	}
}
