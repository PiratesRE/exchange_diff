using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.RbacTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class RemoveMailUserOrRemoteMailboxBase<TIdentity> : RemoveMailUserBase<TIdentity> where TIdentity : MailUserIdParameterBase, new()
	{
		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.orgAdminHelper = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.DataObject != null)
			{
				RemoveMailbox.CheckManagedGroups(base.DataObject, base.TenantGlobalCatalogSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				if (this.orgAdminHelper.ShouldPreventLastAdminRemoval(this, base.DataObject.OrganizationId) && this.orgAdminHelper.IsLastAdmin(base.DataObject))
				{
					base.WriteError(new CannotRemoveLastOrgAdminException(Strings.ErrorCannotRemoveLastOrgAdmin(base.DataObject.Identity.ToString())), ExchangeErrorCategory.Client, base.DataObject.Identity);
				}
			}
		}

		private RoleAssignmentsGlobalConstraints orgAdminHelper;
	}
}
