using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Add", "MailboxPermission", DefaultParameterSetName = "AccessRights", SupportsShouldProcess = true)]
	public sealed class AddMailboxPermission : SetMailboxPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Owner" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageAddMailboxPermissionOwner(this.Identity.ToString(), this.Owner.ToString());
				}
				return Strings.ConfirmationMessageAddMailboxPermissionAccessRights(this.Identity.ToString(), base.Instance.User.ToString(), base.FormatMultiValuedProperty(base.Instance.AccessRights));
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Owner")]
		public SecurityPrincipalIdParameter Owner
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["Owner"];
			}
			set
			{
				base.Fields["Owner"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
		[Parameter(Mandatory = false, ParameterSetName = "AccessRights")]
		public bool? AutoMapping
		{
			get
			{
				return (bool?)base.Fields["AutoMapping"];
			}
			set
			{
				base.Fields["AutoMapping"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.IsInherited)
			{
				return;
			}
			if (this.Owner != null)
			{
				this.owner = SecurityPrincipalIdParameter.GetUserSid(base.TenantGlobalCatalogSession, this.Owner, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (base.ParameterSetName == "Instance" || base.ParameterSetName == "AccessRights")
			{
				if (!base.ToGrantFullAccess() && this.AutoMapping != null && this.AutoMapping.Value)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSpecifyAutoMappingOnNonFullAccess), ErrorCategory.InvalidOperation, null);
				}
				if (base.Instance.AccessRights != null && base.Instance.AccessRights.Length != 0)
				{
					if (Array.Exists<MailboxRights>(base.Instance.AccessRights, (MailboxRights right) => (right & MailboxRights.SendAs) == MailboxRights.SendAs))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorSetSendAsOnMailboxPermissionNotAllowed), ErrorCategory.InvalidOperation, null);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.IsInherited)
			{
				return;
			}
			if ("Owner" == base.ParameterSetName)
			{
				ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadMailboxSecurityDescriptor(this.DataObject, PermissionTaskHelper.GetReadOnlySession(base.DomainController), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				SecurityIdentifier sid = this.owner;
				activeDirectorySecurity.SetOwner(sid);
				new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
				PermissionTaskHelper.SaveMailboxSecurityDescriptor(this.DataObject, activeDirectorySecurity, base.DataSession, ref this.storeSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				string friendlyUserName = SecurityPrincipalIdParameter.GetFriendlyUserName(sid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				base.WriteObject(new OwnerPresentationObject(this.DataObject.Id, friendlyUserName));
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		internal override void ApplyModification(ADUser modifiedObject, ActiveDirectoryAccessRule[] modifiedAces, IConfigDataProvider modifyingSession)
		{
			PermissionTaskHelper.SetMailboxAces(modifiedObject, modifyingSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(base.WriteError), PermissionTaskHelper.GetReadOnlySession(base.DomainController), ref this.storeSession, false, modifiedAces);
		}

		internal override void ApplyDelegation(bool fullAccess)
		{
			if (fullAccess)
			{
				if (this.AutoMapping != null && !this.AutoMapping.Value)
				{
					base.ApplyDelegationInternal(true);
					return;
				}
				base.ApplyDelegationInternal(false);
			}
		}

		protected override void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces)
		{
			foreach (ActiveDirectoryAccessRule ace in aces)
			{
				MailboxAcePresentationObject mailboxAcePresentationObject = new MailboxAcePresentationObject(ace, id);
				mailboxAcePresentationObject.ResetChangeTracking(true);
				base.WriteObject(mailboxAcePresentationObject);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.storeSession != null)
			{
				this.storeSession.Dispose();
				this.storeSession = null;
			}
			base.Dispose(disposing);
		}

		private MapiMessageStoreSession storeSession;

		private SecurityIdentifier owner;
	}
}
