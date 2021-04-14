using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "FederationTrust", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveFederationTrust : RemoveSystemConfigurationObjectTask<FederationTrustIdParameter, FederationTrust>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveFederationTrust(base.DataObject.Name);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			FederatedOrganizationId[] array = this.IsAnyoneRelyingOnThisTrust();
			if (array == null || array.Length == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FederatedOrganizationId federatedOrganizationId in array)
			{
				stringBuilder.Append(federatedOrganizationId.DistinguishedName + "\n");
			}
			base.WriteError(new OrgsStillUsingThisTrustException(base.DataObject.Name, stringBuilder.ToString()), ErrorCategory.InvalidOperation, base.DataObject.Identity);
		}

		private FederatedOrganizationId[] IsAnyoneRelyingOnThisTrust()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, FederatedOrganizationIdSchema.DelegationTrustLink, base.DataObject.Id);
			return base.GlobalConfigSession.Find<FederatedOrganizationId>(null, QueryScope.SubTree, filter, null, 0);
		}
	}
}
