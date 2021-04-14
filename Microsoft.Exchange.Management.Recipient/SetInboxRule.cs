using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("set", "InboxRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetInboxRule : SetTenantADTaskBase<InboxRuleIdParameter, InboxRule, InboxRule>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter AlwaysDeleteOutlookRulesBlob { get; set; }

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] From
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.From];
			}
			set
			{
				base.Fields[InboxRuleSchema.From] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfFrom
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.ExceptIfFrom];
			}
			set
			{
				base.Fields[InboxRuleSchema.ExceptIfFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageClassificationIdParameter[] HasClassification
		{
			get
			{
				return (MessageClassificationIdParameter[])base.Fields[InboxRuleSchema.HasClassification];
			}
			set
			{
				base.Fields[InboxRuleSchema.HasClassification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageClassificationIdParameter[] ExceptIfHasClassification
		{
			get
			{
				return (MessageClassificationIdParameter[])base.Fields[InboxRuleSchema.ExceptIfHasClassification];
			}
			set
			{
				base.Fields[InboxRuleSchema.ExceptIfHasClassification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.SentTo];
			}
			set
			{
				base.Fields[InboxRuleSchema.SentTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfSentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.ExceptIfSentTo];
			}
			set
			{
				base.Fields[InboxRuleSchema.ExceptIfSentTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxFolderIdParameter CopyToFolder
		{
			get
			{
				return (MailboxFolderIdParameter)base.Fields[InboxRuleSchema.CopyToFolder];
			}
			set
			{
				base.Fields[InboxRuleSchema.CopyToFolder] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ForwardAsAttachmentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.ForwardAsAttachmentTo];
			}
			set
			{
				base.Fields[InboxRuleSchema.ForwardAsAttachmentTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ForwardTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.ForwardTo];
			}
			set
			{
				base.Fields[InboxRuleSchema.ForwardTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxFolderIdParameter MoveToFolder
		{
			get
			{
				return (MailboxFolderIdParameter)base.Fields[InboxRuleSchema.MoveToFolder];
			}
			set
			{
				base.Fields[InboxRuleSchema.MoveToFolder] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] RedirectTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[InboxRuleSchema.RedirectTo];
			}
			set
			{
				base.Fields[InboxRuleSchema.RedirectTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AggregationSubscriptionIdentity[] FromSubscription
		{
			get
			{
				return (AggregationSubscriptionIdentity[])base.Fields[InboxRuleSchema.FromSubscription];
			}
			set
			{
				base.Fields[InboxRuleSchema.FromSubscription] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AggregationSubscriptionIdentity[] ExceptIfFromSubscription
		{
			get
			{
				return (AggregationSubscriptionIdentity[])base.Fields[InboxRuleSchema.ExceptIfFromSubscription];
			}
			set
			{
				base.Fields[InboxRuleSchema.ExceptIfFromSubscription] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			InboxRuleDataProvider.ValidateInboxRuleProperties(this.DataObject, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
		}

		protected override void InternalProcessRecord()
		{
			if (this.AlwaysDeleteOutlookRulesBlob.IsPresent)
			{
				InboxRuleDataProvider inboxRuleDataProvider = (InboxRuleDataProvider)base.DataSession;
				inboxRuleDataProvider.SetAlwaysDeleteOutlookRulesBlob(new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			else if (!this.Force.IsPresent)
			{
				InboxRuleDataProvider inboxRuleDataProvider = (InboxRuleDataProvider)base.DataSession;
				inboxRuleDataProvider.ConfirmDeleteOutlookBlob = (() => base.ShouldContinue(Strings.WarningInboxRuleOutlookBlobExists));
			}
			if (this.DataObject.FlaggedForAction != null)
			{
				InboxRuleDataProvider.CheckFlaggedAction(this.DataObject.FlaggedForAction, InboxRuleSchema.FlaggedForAction.Name, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (this.DataObject.ExceptIfFlaggedForAction != null)
			{
				InboxRuleDataProvider.CheckFlaggedAction(this.DataObject.ExceptIfFlaggedForAction, InboxRuleSchema.ExceptIfFlaggedForAction.Name, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (this.DataObject.SendTextMessageNotificationTo.Count > 0)
			{
				SmsSqmDataPointHelper.AddNotificationConfigDataPoint(SmsSqmSession.Instance, this.adUser.Id, this.adUser.LegacyExchangeDN, SMSNotificationType.Email);
			}
			ManageInboxRule.ProcessRecord(new Action(base.InternalProcessRecord), new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError), this.Identity);
		}

		protected override IConfigDataProvider CreateSession()
		{
			MailboxIdParameter mailboxIdParameter = null;
			if (this.Identity != null)
			{
				if (this.Identity.InternalInboxRuleId != null)
				{
					mailboxIdParameter = new MailboxIdParameter(this.Identity.InternalInboxRuleId.MailboxOwnerId);
				}
				else
				{
					mailboxIdParameter = this.Identity.RawMailbox;
				}
			}
			if (mailboxIdParameter != null && this.Mailbox != null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorConflictingMailboxes), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (mailboxIdParameter == null)
			{
				ADObjectId executingUserId;
				base.TryGetExecutingUserId(out executingUserId);
				mailboxIdParameter = (this.Mailbox ?? MailboxTaskHelper.ResolveMailboxIdentity(executingUserId, new Task.ErrorLoggerDelegate(base.WriteError)));
			}
			this.adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			if (this.Identity != null && this.Identity.InternalInboxRuleId == null)
			{
				this.Identity.InternalInboxRuleId = new InboxRuleId(this.adUser.Id, this.Identity.RawRuleName, this.Identity.RawRuleId);
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 323, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\InboxRule\\SetInboxRule.cs");
			base.VerifyIsWithinScopes(TaskHelper.UnderscopeSessionToOrganization(tenantOrRootOrgRecipientSession, this.adUser.OrganizationId, true), this.adUser, true, new DataAccessTask<InboxRule>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
			InboxRuleDataProvider inboxRuleDataProvider = new InboxRuleDataProvider(base.SessionSettings, this.adUser, "Set-InboxRule");
			this.mailboxOwner = inboxRuleDataProvider.MailboxSession.MailboxOwner.ObjectId.ToString();
			return inboxRuleDataProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			InboxRule inboxRule = this.LoadInboxRule();
			this.PrepareDataObjectFromParameters(inboxRule);
			inboxRule.ValidateInterdependentParameters(new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			return inboxRule;
		}

		private InboxRule LoadInboxRule()
		{
			InboxRule result;
			try
			{
				InboxRule inboxRule = (InboxRule)base.PrepareDataObject();
				inboxRule.Provider = (XsoMailboxDataProviderBase)base.DataSession;
				result = inboxRule;
			}
			catch (ObjectNotFoundException)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInboxRuleDoesNotExist), ErrorCategory.InvalidOperation, this.Identity);
				result = null;
			}
			return result;
		}

		private void PrepareDataObjectFromParameters(InboxRule inboxRule)
		{
			if (base.Fields.IsModified(InboxRuleSchema.From))
			{
				inboxRule.From = ManageInboxRule.ResolveRecipients(this.From, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ExceptIfFrom))
			{
				inboxRule.ExceptIfFrom = ManageInboxRule.ResolveRecipients(this.ExceptIfFrom, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.HasClassification))
			{
				inboxRule.HasClassification = ManageInboxRule.ResolveMessageClassifications(this.HasClassification, this.ConfigurationSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ExceptIfHasClassification))
			{
				inboxRule.ExceptIfHasClassification = ManageInboxRule.ResolveMessageClassifications(this.ExceptIfHasClassification, this.ConfigurationSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.SentTo))
			{
				inboxRule.SentTo = ManageInboxRule.ResolveRecipients(this.SentTo, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ExceptIfSentTo))
			{
				inboxRule.ExceptIfSentTo = ManageInboxRule.ResolveRecipients(this.ExceptIfSentTo, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.CopyToFolder))
			{
				inboxRule.CopyToFolder = ManageInboxRule.ResolveMailboxFolder(this.CopyToFolder, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<MailboxFolder>), base.TenantGlobalCatalogSession, base.SessionSettings, this.adUser, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ForwardAsAttachmentTo))
			{
				inboxRule.ForwardAsAttachmentTo = ManageInboxRule.ResolveRecipients(this.ForwardAsAttachmentTo, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ForwardTo))
			{
				inboxRule.ForwardTo = ManageInboxRule.ResolveRecipients(this.ForwardTo, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.MoveToFolder))
			{
				inboxRule.MoveToFolder = ManageInboxRule.ResolveMailboxFolder(this.MoveToFolder, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<MailboxFolder>), base.TenantGlobalCatalogSession, base.SessionSettings, this.adUser, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.RedirectTo))
			{
				inboxRule.RedirectTo = ManageInboxRule.ResolveRecipients(this.RedirectTo, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), base.TenantGlobalCatalogSession, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.FromSubscription))
			{
				inboxRule.FromSubscription = ManageInboxRule.ResolveSubscriptions(this.FromSubscription, this.adUser, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(InboxRuleSchema.ExceptIfFromSubscription))
			{
				inboxRule.ExceptIfFromSubscription = ManageInboxRule.ResolveSubscriptions(this.ExceptIfFromSubscription, this.adUser, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetInboxRule(this.Identity.ToString(), this.mailboxOwner);
			}
		}

		protected override void InternalStateReset()
		{
			ManageInboxRule.CleanupInboxRuleDataProvider(base.DataSession);
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			ManageInboxRule.CleanupInboxRuleDataProvider(base.DataSession);
			GC.SuppressFinalize(this);
		}

		private string mailboxOwner;

		private ADUser adUser;
	}
}
