using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetMailboxPermissionTaskBase : SetRecipientObjectTask<MailboxIdParameter, MailboxAcePresentationObject, ADUser>
	{
		protected int CurrentProcessedObject
		{
			get
			{
				return this.currentProcessedObject;
			}
			set
			{
				this.currentProcessedObject = value;
			}
		}

		protected List<List<ActiveDirectoryAccessRule>> ModifiedAcl
		{
			get
			{
				return this.modifiedAcl;
			}
		}

		protected List<ADUser> ModifiedObjects
		{
			get
			{
				return this.modifiedObjects;
			}
		}

		internal List<IConfigDataProvider> ModifyingRecipientSessions
		{
			get
			{
				return this.modifyingRecipientSessions;
			}
		}

		internal IADSecurityPrincipal SecurityPrincipal
		{
			get
			{
				return this.securityPrincipal;
			}
		}

		internal abstract void ApplyDelegation(bool fullAccess);

		protected override void StampChangesOn(IConfigurable dataObject)
		{
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "AccessRights")]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "Owner")]
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "Instance")]
		public override MailboxIdParameter Identity
		{
			get
			{
				if (base.Fields[SetMailboxPermissionTaskBase.paramIdentity] != null)
				{
					return (MailboxIdParameter)base.Fields[SetMailboxPermissionTaskBase.paramIdentity];
				}
				if (this.Instance != null)
				{
					MailboxIdParameter mailboxIdParameter = new MailboxIdParameter();
					mailboxIdParameter.Initialize((ADObjectId)this.Instance.Identity);
					return mailboxIdParameter;
				}
				return null;
			}
			set
			{
				base.Fields[SetMailboxPermissionTaskBase.paramIdentity] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Instance")]
		public new MailboxAcePresentationObject Instance
		{
			get
			{
				return base.Instance;
			}
			set
			{
				base.Instance = value;
			}
		}

		protected bool IsInherited
		{
			get
			{
				return this.Instance != null && this.Instance.IsInherited;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.modifiedAcl.Clear();
			this.modifiedObjects.Clear();
			this.modifyingRecipientSessions.Clear();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.CurrentTaskContext.CanBypassRBACScope)
			{
				base.VerifyIsWithinScopes((IRecipientSession)base.DataSession, this.DataObject, true, new DataAccessTask<ADUser>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
			}
			if (this.IsInherited)
			{
				this.WriteWarning(Strings.ErrorWillNotPerformOnInheritedAccessRight(this.Instance.Identity.ToString()));
				return;
			}
			if (base.ParameterSetName == "Owner")
			{
				return;
			}
			if (this.Instance.User != null)
			{
				this.securityPrincipal = SecurityPrincipalIdParameter.GetSecurityPrincipal(base.TenantGlobalCatalogSession, this.Instance.User, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (this.IsInherited)
			{
				return;
			}
			if (base.ParameterSetName == "Instance")
			{
				if (this.Instance.User == null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorUserNull, "User"), ErrorCategory.InvalidArgument, null);
				}
				if (this.Instance.AccessRights == null || this.Instance.AccessRights.Length == 0)
				{
					base.WriteError(new ArgumentException(Strings.ErrorAccessRightsEmpty, "AccessRights"), ErrorCategory.InvalidArgument, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			BackEndServer backEndServer = BackEndLocator.GetBackEndServer((ADUser)configurable);
			if (backEndServer != null && backEndServer.Version < Server.E15MinVersion)
			{
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, (ADUser)configurable, false, this.ConfirmationMessage, null);
			}
			return configurable;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.IsInherited)
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.InheritedAceIgnored);
				}
				TaskLogger.LogExit();
				return;
			}
			this.PutEachEntryInList();
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if ("Owner" != base.ParameterSetName)
			{
				this.CurrentProcessedObject = 0;
				while (this.CurrentProcessedObject < this.ModifiedObjects.Count)
				{
					try
					{
						this.ApplyModification(this.ModifiedObjects[this.CurrentProcessedObject], this.ModifiedAcl[this.CurrentProcessedObject].ToArray(), this.ModifyingRecipientSessions[this.CurrentProcessedObject]);
					}
					catch (OverflowException exception)
					{
						this.WriteError(exception, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_15F;
					}
					catch (DataValidationException exception2)
					{
						this.WriteError(exception2, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_15F;
					}
					catch (DataSourceOperationException exception3)
					{
						this.WriteError(exception3, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_15F;
					}
					catch (DataSourceTransientException exception4)
					{
						this.WriteError(exception4, ErrorCategory.WriteError, base.CurrentObjectIndex, false);
						goto IL_15F;
					}
					goto IL_D2;
					IL_15F:
					this.CurrentProcessedObject++;
					continue;
					IL_D2:
					if (this.CurrentProcessedObject + 1 < this.ModifiedObjects.Count)
					{
						this.WriteCurrentProgress(Strings.ProcessingAceActivity, Strings.ProcessingAceStatus(((ADRawEntry)this.ModifiedObjects[this.CurrentProcessedObject]).Id.ToString()), ProgressRecordType.Processing, this.CurrentProcessedObject * 100 / this.ModifiedObjects.Count);
					}
					this.WriteAces(((ADRawEntry)this.ModifiedObjects[this.CurrentProcessedObject]).Id, this.ModifiedAcl[this.CurrentProcessedObject]);
					goto IL_15F;
				}
				if (this.DataObject != null && this.SecurityPrincipal != null)
				{
					bool fullAccess = this.ToGrantFullAccess();
					this.ApplyDelegation(fullAccess);
				}
				this.WriteCurrentProgress(Strings.ProcessingAceActivity, Strings.CompletedAceActivity, ProgressRecordType.Completed, 100);
			}
			TaskLogger.LogExit();
		}

		internal abstract void ApplyModification(ADUser modifiedObject, ActiveDirectoryAccessRule[] modifiedAces, IConfigDataProvider modifyingSession);

		protected abstract void WriteAces(ADObjectId id, IEnumerable<ActiveDirectoryAccessRule> aces);

		protected void WriteCurrentProgress(LocalizedString activityDesc, LocalizedString status, ProgressRecordType recordType, int percent)
		{
			base.WriteProgress(new ExProgressRecord(0, activityDesc, status)
			{
				RecordType = recordType,
				PercentComplete = percent
			});
		}

		protected void ApplyDelegationInternal(bool removeDelegation)
		{
			ADRecipient adrecipient = this.SecurityPrincipal.Sid.IsWellKnown(WellKnownSidType.SelfSid) ? this.DataObject : ((ADRecipient)this.SecurityPrincipal);
			if (adrecipient.RecipientType == RecipientType.UserMailbox)
			{
				ADUser dataObject = this.DataObject;
				ADUser aduser = this.GetADUser(base.DataSession, dataObject);
				if (aduser != null)
				{
					try
					{
						PermissionTaskHelper.SetDelegation(aduser, adrecipient, base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), removeDelegation);
						if (!removeDelegation && ((ADUser)adrecipient).DelegateListBL.Count >= 16)
						{
							this.WriteWarning(Strings.WarningDelegatesExceededOutlookLimit);
						}
						return;
					}
					catch (DataValidationException exception)
					{
						base.WriteError(exception, ErrorCategory.WriteError, null);
						return;
					}
					catch (DataSourceOperationException exception2)
					{
						base.WriteError(exception2, ErrorCategory.WriteError, null);
						return;
					}
					catch (DataSourceTransientException exception3)
					{
						base.WriteError(exception3, ErrorCategory.WriteError, null);
						return;
					}
				}
				base.WriteVerbose(Strings.VerboseMailboxDelegateSkipNotADUser(this.DataObject.ToString()));
				return;
			}
			base.WriteVerbose(Strings.VerboseMailboxDelegateSkip(this.Instance.User.ToString()));
		}

		internal virtual void WriteErrorPerObject(LocalizedException exception, ExchangeErrorCategory category, object target)
		{
			if (target == null)
			{
				this.WriteError(exception, category, this.CurrentProcessedObject, false);
				return;
			}
			base.WriteError(exception, category, target);
		}

		protected void PutEachEntryInList()
		{
			List<ActiveDirectoryAccessRule> list = null;
			for (int i = 0; i < this.ModifiedObjects.Count; i++)
			{
				if (this.IsEqualEntry(i))
				{
					list = this.ModifiedAcl[i];
					break;
				}
			}
			if (list == null)
			{
				list = new List<ActiveDirectoryAccessRule>();
				this.ModifiedObjects.Add(this.DataObject);
				this.ModifiedAcl.Add(list);
				this.ModifyingRecipientSessions.Add(base.DataSession);
			}
			AccessControlType allowOrDeny = this.Instance.Deny ? AccessControlType.Deny : AccessControlType.Allow;
			MailboxRights mailboxRights = (MailboxRights)0;
			if (this.Instance.AccessRights != null)
			{
				foreach (MailboxRights mailboxRights2 in this.Instance.AccessRights)
				{
					mailboxRights |= mailboxRights2;
				}
			}
			if (mailboxRights != (MailboxRights)0)
			{
				this.UpdateAcl(list, allowOrDeny, mailboxRights);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is SecurityDescriptorAccessDeniedException || exception is BackEndLocatorException || exception is StoragePermanentException;
		}

		private bool IsEqualEntry(int Index)
		{
			return this.ModifiedObjects[Index].Id.Equals(this.DataObject.Id);
		}

		protected virtual void UpdateAcl(List<ActiveDirectoryAccessRule> modifiedAcl, AccessControlType allowOrDeny, MailboxRights mailboxRights)
		{
			TaskLogger.LogEnter();
			SecurityIdentifier identity = (this.SecurityPrincipal.MasterAccountSid != null && !this.SecurityPrincipal.MasterAccountSid.IsWellKnown(WellKnownSidType.SelfSid)) ? this.SecurityPrincipal.MasterAccountSid : this.SecurityPrincipal.Sid;
			modifiedAcl.Add(new ActiveDirectoryAccessRule(identity, (ActiveDirectoryRights)mailboxRights, allowOrDeny, Guid.Empty, this.Instance.InheritanceType, Guid.Empty));
			TaskLogger.LogExit();
		}

		protected bool ToGrantFullAccess()
		{
			bool flag = false;
			if (this.Instance.AccessRights != null)
			{
				foreach (MailboxRights mailboxRights in this.Instance.AccessRights)
				{
					flag = ((mailboxRights & MailboxRights.FullAccess) == MailboxRights.FullAccess);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		private ADUser GetADUser(IConfigDataProvider session, ADUser recipient)
		{
			if (recipient != null)
			{
				return session.Read<ADUser>(recipient.Id) as ADUser;
			}
			return null;
		}

		private IADSecurityPrincipal securityPrincipal;

		private static string paramIdentity = "Identity";

		private List<List<ActiveDirectoryAccessRule>> modifiedAcl = new List<List<ActiveDirectoryAccessRule>>();

		private List<ADUser> modifiedObjects = new List<ADUser>();

		private List<IConfigDataProvider> modifyingRecipientSessions = new List<IConfigDataProvider>();

		private int currentProcessedObject;
	}
}
