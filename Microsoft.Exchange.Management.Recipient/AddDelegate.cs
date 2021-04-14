using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Add", "ExchangeAdministrator", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class AddDelegate : DelegateTask
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddDelegate(this.Identity.ToString(), this.DataObject.Id.ToString(), base.Role.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.Role == DelegateRoleType.ServerAdmin && this.server.IsEdgeServer && !ADSession.IsBoundToAdam)
			{
				base.WriteError(new CannotDelegateEdgeServerAdminException(base.Scope), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			if (this.DataObject.Guid == this.user.Guid)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorAddGroupToItself), ErrorCategory.InvalidData, this.Identity);
			}
			foreach (ADObjectId adobjectId in this.DataObject.Members)
			{
				if (this.user.Guid == adobjectId.ObjectGuid)
				{
					if (base.Role == DelegateRoleType.ServerAdmin)
					{
						flag = true;
						break;
					}
					base.WriteError(new RecipientTaskException(Strings.ErrorUserAlreadyDelegate((string)this.Identity, this.DataObject.Id.DistinguishedName)), ErrorCategory.InvalidData, this.DataObject.Identity);
				}
			}
			if (base.Role == DelegateRoleType.ServerAdmin)
			{
				this.GrantServerAdminRole();
			}
			else if (!flag)
			{
				this.DataObject.Members.Add(this.user.Id);
				try
				{
					base.InternalProcessRecord();
				}
				catch (ADObjectEntryAlreadyExistsException)
				{
				}
			}
			this.WriteResult(this.user.Id, base.Role, base.Scope);
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 128, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\permission\\AddDelegate.cs");
		}

		private void WriteResult(ADObjectId member, DelegateRoleType role, string scope)
		{
			string text = Strings.OrganizationWide.ToString();
			DelegateUser sendToPipeline = new DelegateUser(member.ToString(), role, (role != DelegateRoleType.ServerAdmin) ? text : scope);
			base.WriteObject(sendToPipeline);
		}

		private void GrantServerAdminRole()
		{
			try
			{
				ActiveDirectoryAccessRule[] acesToServerAdmin = PermissionTaskHelper.GetAcesToServerAdmin(this.ConfigurationSession, ((IADSecurityPrincipal)this.user).Sid);
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.server, acesToServerAdmin);
			}
			catch (SecurityDescriptorAccessDeniedException exception)
			{
				base.WriteError(exception, ErrorCategory.PermissionDenied, null);
			}
			this.WriteWarning(Strings.CouldNotFindLocalAdministratorGroup(this.server.Name, this.Identity.ToString()));
		}
	}
}
