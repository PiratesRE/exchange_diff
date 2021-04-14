using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "MailboxPlan", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxPlan : SetMailboxBase<MailboxPlanIdParameter, MailboxPlan>
	{
		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return base.AddressBookPolicy;
			}
			set
			{
				base.AddressBookPolicy = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? false);
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefaultForPreviousVersion
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefaultForPreviousVersion"] ?? false);
			}
			set
			{
				base.Fields["IsDefaultForPreviousVersion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPlanRelease MailboxPlanRelease
		{
			get
			{
				return (MailboxPlanRelease)base.Fields["MailboxPlanRelease"];
			}
			set
			{
				base.Fields["MailboxPlanRelease"] = value;
			}
		}

		private new SwitchParameter BypassLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BypassLiveId"] = value;
			}
		}

		private new NetID NetID
		{
			get
			{
				return (NetID)base.Fields["NetID"];
			}
			set
			{
				base.Fields["NetID"] = value;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFrom
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new SwitchParameter Arbitration
		{
			get
			{
				return base.Arbitration;
			}
		}

		internal new SwitchParameter PublicFolder
		{
			get
			{
				return base.PublicFolder;
			}
		}

		internal new MailboxIdParameter ArbitrationMailbox
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> BypassModerationFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new bool CreateDTMFMap
		{
			get
			{
				return false;
			}
		}

		internal new PSCredential LinkedCredential
		{
			get
			{
				return null;
			}
		}

		internal new RecipientIdParameter ForwardingAddress
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<RecipientIdParameter> GrantSendOnBehalfTo
		{
			get
			{
				return null;
			}
		}

		internal new string LinkedDomainController
		{
			get
			{
				return null;
			}
		}

		internal new UserIdParameter LinkedMasterAccount
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ModeratorIDParameter> ModeratedBy
		{
			get
			{
				return null;
			}
		}

		internal new SecureString Password
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFrom
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFromDLMembers
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new SwitchParameter RemoveManagedFolderAndPolicy
		{
			get
			{
				return base.RemoveManagedFolderAndPolicy;
			}
		}

		internal new SwitchParameter RemovePicture
		{
			get
			{
				return base.RemovePicture;
			}
		}

		internal new SwitchParameter RemoveSpokenName
		{
			get
			{
				return base.RemoveSpokenName;
			}
		}

		internal new string SecondaryAddress
		{
			get
			{
				return null;
			}
		}

		internal new UMDialPlanIdParameter SecondaryDialPlan
		{
			get
			{
				return null;
			}
		}

		internal new ConvertibleMailboxSubType Type
		{
			get
			{
				return base.Type;
			}
		}

		private new string FederatedIdentity
		{
			get
			{
				return null;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, this.Arbitration) || MailboxTaskHelper.ExcludePublicFolderMailbox(adrecipient, this.PublicFolder) || MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, true) || MailboxTaskHelper.ExcludeAuditLogMailbox(adrecipient, base.AuditLog))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			return adrecipient;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			base.StampChangesOn(dataObject);
			if (base.Fields.IsModified("IsDefault"))
			{
				ADUser aduser = (ADUser)dataObject;
				aduser.IsDefault = this.IsDefault;
			}
			if (base.Fields.IsModified("IsDefaultForPreviousVersion"))
			{
				ADUser aduser2 = (ADUser)dataObject;
				aduser2[MailboxPlanSchema.IsDefault_R3] = this.IsDefaultForPreviousVersion;
			}
			if (base.Fields.IsModified("MailboxPlanRelease"))
			{
				MailboxPlanRelease mailboxPlanRelease = (MailboxPlanRelease)((ADUser)dataObject)[MailboxPlanSchema.MailboxPlanRelease];
				MailboxPlanRelease mailboxPlanRelease2 = mailboxPlanRelease;
				if (mailboxPlanRelease2 != MailboxPlanRelease.AllReleases)
				{
					if (mailboxPlanRelease2 != MailboxPlanRelease.CurrentRelease)
					{
						if (mailboxPlanRelease2 == MailboxPlanRelease.NonCurrentRelease)
						{
							if (this.MailboxPlanRelease != MailboxPlanRelease.CurrentRelease)
							{
								((ADUser)dataObject)[MailboxPlanSchema.MailboxPlanRelease] = this.MailboxPlanRelease;
							}
							else
							{
								base.WriteError(new TaskInvalidOperationException(Strings.ErrorMbxPlanReleaseTransition(((ADUser)dataObject).Name, mailboxPlanRelease.ToString(), this.MailboxPlanRelease.ToString())), ExchangeErrorCategory.Client, null);
							}
						}
					}
					else if (this.MailboxPlanRelease != MailboxPlanRelease.NonCurrentRelease)
					{
						((ADUser)dataObject)[MailboxPlanSchema.MailboxPlanRelease] = this.MailboxPlanRelease;
					}
					else
					{
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorMbxPlanReleaseTransition(((ADUser)dataObject).Name, mailboxPlanRelease.ToString(), this.MailboxPlanRelease.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				else
				{
					((ADUser)dataObject)[MailboxPlanSchema.MailboxPlanRelease] = this.MailboxPlanRelease;
				}
			}
			if (base.ApplyMandatoryProperties)
			{
				ADUser aduser3 = (ADUser)dataObject;
				aduser3.Database = this.DatabasesContainerId;
			}
		}

		private ADObjectId DatabasesContainerId
		{
			get
			{
				if (this.databasesContainerId == null)
				{
					this.databasesContainerId = base.GlobalConfigSession.GetDatabasesContainerId();
				}
				return this.databasesContainerId;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			if (base.Fields.IsModified("Database"))
			{
				if (this.Database != null)
				{
					MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorMailboxDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorMailboxDatabaseNotUnique(this.Database.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADMailboxRecipientSchema.Database] = (ADObjectId)mailboxDatabase.Identity;
				}
				else
				{
					mailbox[ADMailboxRecipientSchema.Database] = null;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			ADUser aduser = null;
			if (this.IsDefault)
			{
				aduser = RecipientTaskHelper.ResetOldDefaultPlan((IRecipientSession)base.DataSession, this.DataObject.Id, this.DataObject.OrganizationalUnitRoot, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			bool flag = false;
			try
			{
				base.InternalProcessRecord();
				flag = true;
			}
			finally
			{
				if (!flag && aduser != null)
				{
					aduser.IsDefault = true;
					try
					{
						base.DataSession.Save(aduser);
					}
					catch (DataSourceTransientException exception)
					{
						this.WriteError(exception, ExchangeErrorCategory.ServerTransient, null, false);
					}
				}
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Microsoft.Exchange.Data.Directory.Management.MailboxPlan.FromDataObject((ADUser)dataObject);
		}

		private ADObjectId databasesContainerId;
	}
}
