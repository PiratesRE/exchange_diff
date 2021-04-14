using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Add", "ADPermission", DefaultParameterSetName = "AccessRights", SupportsShouldProcess = true)]
	public sealed class AddADPermission : SetADPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Owner" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageAddADPermissionOwner(this.Identity.ToString(), this.Owner.ToString());
				}
				return Strings.ConfirmationMessageAddADPermissionAccessRights(this.Identity.ToString(), base.Instance.User.ToString(), (base.Instance.AccessRights != null) ? base.FormatMultiValuedProperty(base.Instance.AccessRights) : base.FormatMultiValuedProperty(base.Instance.ExtendedRights));
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
				this.owner = SecurityPrincipalIdParameter.GetUserSid(base.GlobalCatalogRecipientSession, this.Owner, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
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
				IConfigurationSession writableSession = base.GetWritableSession(this.DataObject.Id);
				ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadAdSecurityDescriptor(this.DataObject, writableSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
				SecurityIdentifier sid = this.owner;
				activeDirectorySecurity.SetOwner(sid);
				RawSecurityDescriptor sd = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
				writableSession.SaveSecurityDescriptor(this.DataObject.Id, sd, true);
				string friendlyUserName = SecurityPrincipalIdParameter.GetFriendlyUserName(sid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				base.WriteObject(new OwnerPresentationObject(this.DataObject.Id, friendlyUserName));
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		protected override void ApplyModification(ADRawEntry modifiedObject, ActiveDirectoryAccessRule[] modifiedAces)
		{
			DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(this.WriteErrorPerObject), base.GetWritableSession(modifiedObject.Id), modifiedObject.Id, modifiedAces);
		}

		protected override void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces)
		{
			foreach (ActiveDirectoryAccessRule ace in aces)
			{
				ADAcePresentationObject adacePresentationObject = new ADAcePresentationObject(ace, id);
				adacePresentationObject.ResetChangeTracking(true);
				base.WriteObject(adacePresentationObject);
			}
		}

		private SecurityIdentifier owner;
	}
}
