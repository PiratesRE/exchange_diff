using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "RemoteAccountPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRemoteAccountPolicy : RemoveSystemConfigurationObjectTask<RemoteAccountPolicyIdParameter, RemoteAccountPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRemoteAccountPolicy(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.RemoteAccountPolicy, base.DataObject.Id),
				new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.ArbitrationMailbox))
			});
			ADRecipient[] array = base.TenantGlobalCatalogSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				base.WriteError(new InvalidOperationException(Strings.RemoveRemoteAccountPolicyFailedWithExistingMailboxes), ErrorCategory.InvalidOperation, this.Identity);
			}
		}
	}
}
