using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Core;
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
	[Cmdlet("Remove", "MailboxPermission", DefaultParameterSetName = "AccessRights", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxPermission : SetMailboxPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxPermissionAccessRights(this.Identity.ToString(), base.FormatMultiValuedProperty(base.Instance.AccessRights), base.Instance.User.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (Constants.IsPowerShellWebService && base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.ConfigurationSettings.EncodeDecodeKey && base.DynamicParametersInstance.IsModified(AcePresentationObjectSchema.User))
			{
				SecurityPrincipalIdParameter securityPrincipalIdParameter = base.DynamicParametersInstance[AcePresentationObjectSchema.User] as SecurityPrincipalIdParameter;
				IIdentityParameter identityParameter;
				if (securityPrincipalIdParameter != null && PswsPropertyConverterModule.TryDecodeIIdentityParameter(securityPrincipalIdParameter, out identityParameter))
				{
					base.DynamicParametersInstance[AcePresentationObjectSchema.User] = (identityParameter as SecurityPrincipalIdParameter);
				}
			}
			base.InternalBeginProcessing();
		}

		internal override void ApplyModification(ADUser modifiedObject, ActiveDirectoryAccessRule[] modifiedAces, IConfigDataProvider modifyingSession)
		{
			PermissionTaskHelper.SetMailboxAces(modifiedObject, modifyingSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.ErrorLoggerDelegate(this.WriteErrorPerObject), PermissionTaskHelper.GetReadOnlySession(base.DomainController), ref this.storeSession, true, modifiedAces);
		}

		internal override void ApplyDelegation(bool fullAccess)
		{
			if (fullAccess)
			{
				base.ApplyDelegationInternal(true);
			}
		}

		protected override void UpdateAcl(List<ActiveDirectoryAccessRule> modifiedAcl, AccessControlType allowOrDeny, MailboxRights mailboxRights)
		{
			TaskLogger.LogEnter();
			base.UpdateAcl(modifiedAcl, allowOrDeny, mailboxRights);
			foreach (SecurityIdentifier identity in base.SecurityPrincipal.SidHistory)
			{
				modifiedAcl.Add(new ActiveDirectoryAccessRule(identity, (ActiveDirectoryRights)mailboxRights, allowOrDeny, Guid.Empty, base.Instance.InheritanceType, Guid.Empty));
			}
			TaskLogger.LogExit();
		}

		protected override void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces)
		{
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
	}
}
