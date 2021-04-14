using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Remove", "RoleGroupMember", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRoleGroupMember : RoleGroupMemberTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRoleGroupMember(this.Identity.ToString(), base.Member.ToString());
			}
		}

		protected override void PerformGroupMemberAction()
		{
			TaskLogger.LogEnter();
			MailboxTaskHelper.ValidateAndRemoveMember(base.TenantGlobalCatalogSession, this.DataObject, base.Member, null, false, new Task.TaskErrorLoggingDelegate(base.WriteError), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>));
			if (this.DataObject.Members.Changed)
			{
				ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(base.Member, base.TenantGlobalCatalogSession, this.DataObject.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound(base.Member.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(base.Member.ToString())));
				RoleAssignmentsGlobalConstraints roleAssignmentsGlobalConstraints = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
				roleAssignmentsGlobalConstraints.ValidateIsSafeToRemoveRoleGroupMember(this.DataObject, new List<ADObjectId>
				{
					adrecipient.Id
				});
			}
			TaskLogger.LogExit();
		}
	}
}
