using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "InboxRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewInboxRule : NewTenantADTaskBase<InboxRule>
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

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Identity")]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int Priority
		{
			get
			{
				return this.DataObject.Priority;
			}
			set
			{
				this.DataObject.Priority = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> BodyContainsWords
		{
			get
			{
				return this.DataObject.BodyContainsWords;
			}
			set
			{
				this.DataObject.BodyContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfBodyContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfBodyContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfBodyContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string FlaggedForAction
		{
			get
			{
				return this.DataObject.FlaggedForAction;
			}
			set
			{
				this.DataObject.FlaggedForAction = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string ExceptIfFlaggedForAction
		{
			get
			{
				return this.DataObject.ExceptIfFlaggedForAction;
			}
			set
			{
				this.DataObject.ExceptIfFlaggedForAction = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> FromAddressContainsWords
		{
			get
			{
				return this.DataObject.FromAddressContainsWords;
			}
			set
			{
				this.DataObject.FromAddressContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfFromAddressContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfFromAddressContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfFromAddressContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool HasAttachment
		{
			get
			{
				return this.DataObject.HasAttachment;
			}
			set
			{
				this.DataObject.HasAttachment = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfHasAttachment
		{
			get
			{
				return this.DataObject.ExceptIfHasAttachment;
			}
			set
			{
				this.DataObject.ExceptIfHasAttachment = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> HeaderContainsWords
		{
			get
			{
				return this.DataObject.HeaderContainsWords;
			}
			set
			{
				this.DataObject.HeaderContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfHeaderContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfHeaderContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfHeaderContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public InboxRuleMessageType MessageTypeMatches
		{
			get
			{
				return this.DataObject.MessageTypeMatches.Value;
			}
			set
			{
				this.DataObject.MessageTypeMatches = new InboxRuleMessageType?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public InboxRuleMessageType ExceptIfMessageTypeMatches
		{
			get
			{
				return this.DataObject.ExceptIfMessageTypeMatches.Value;
			}
			set
			{
				this.DataObject.ExceptIfMessageTypeMatches = new InboxRuleMessageType?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool MyNameInCcBox
		{
			get
			{
				return this.DataObject.MyNameInCcBox;
			}
			set
			{
				this.DataObject.MyNameInCcBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfMyNameInCcBox
		{
			get
			{
				return this.DataObject.ExceptIfMyNameInCcBox;
			}
			set
			{
				this.DataObject.ExceptIfMyNameInCcBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool MyNameInToBox
		{
			get
			{
				return this.DataObject.MyNameInToBox;
			}
			set
			{
				this.DataObject.MyNameInToBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfMyNameInToBox
		{
			get
			{
				return this.DataObject.ExceptIfMyNameInToBox;
			}
			set
			{
				this.DataObject.ExceptIfMyNameInToBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool MyNameInToOrCcBox
		{
			get
			{
				return this.DataObject.MyNameInToOrCcBox;
			}
			set
			{
				this.DataObject.MyNameInToOrCcBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfMyNameInToOrCcBox
		{
			get
			{
				return this.DataObject.ExceptIfMyNameInToOrCcBox;
			}
			set
			{
				this.DataObject.ExceptIfMyNameInToOrCcBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool MyNameNotInToBox
		{
			get
			{
				return this.DataObject.MyNameNotInToBox;
			}
			set
			{
				this.DataObject.MyNameNotInToBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfMyNameNotInToBox
		{
			get
			{
				return this.DataObject.ExceptIfMyNameNotInToBox;
			}
			set
			{
				this.DataObject.ExceptIfMyNameNotInToBox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ExDateTime ReceivedAfterDate
		{
			get
			{
				return this.DataObject.ReceivedAfterDate.Value;
			}
			set
			{
				this.DataObject.ReceivedAfterDate = new ExDateTime?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ExDateTime ExceptIfReceivedAfterDate
		{
			get
			{
				return this.DataObject.ExceptIfReceivedAfterDate.Value;
			}
			set
			{
				this.DataObject.ExceptIfReceivedAfterDate = new ExDateTime?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ExDateTime ReceivedBeforeDate
		{
			get
			{
				return this.DataObject.ReceivedBeforeDate.Value;
			}
			set
			{
				this.DataObject.ReceivedBeforeDate = new ExDateTime?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ExDateTime ExceptIfReceivedBeforeDate
		{
			get
			{
				return this.DataObject.ExceptIfReceivedBeforeDate.Value;
			}
			set
			{
				this.DataObject.ExceptIfReceivedBeforeDate = new ExDateTime?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> RecipientAddressContainsWords
		{
			get
			{
				return this.DataObject.RecipientAddressContainsWords;
			}
			set
			{
				this.DataObject.RecipientAddressContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfRecipientAddressContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfRecipientAddressContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool SentOnlyToMe
		{
			get
			{
				return this.DataObject.SentOnlyToMe;
			}
			set
			{
				this.DataObject.SentOnlyToMe = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool ExceptIfSentOnlyToMe
		{
			get
			{
				return this.DataObject.ExceptIfSentOnlyToMe;
			}
			set
			{
				this.DataObject.ExceptIfSentOnlyToMe = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> SubjectContainsWords
		{
			get
			{
				return this.DataObject.SubjectContainsWords;
			}
			set
			{
				this.DataObject.SubjectContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfSubjectContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfSubjectContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfSubjectContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> SubjectOrBodyContainsWords
		{
			get
			{
				return this.DataObject.SubjectOrBodyContainsWords;
			}
			set
			{
				this.DataObject.SubjectOrBodyContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return this.DataObject.ExceptIfSubjectOrBodyContainsWords;
			}
			set
			{
				this.DataObject.ExceptIfSubjectOrBodyContainsWords = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Microsoft.Exchange.Data.Storage.Importance WithImportance
		{
			get
			{
				return this.DataObject.WithImportance.Value;
			}
			set
			{
				this.DataObject.WithImportance = new Microsoft.Exchange.Data.Storage.Importance?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Microsoft.Exchange.Data.Storage.Importance ExceptIfWithImportance
		{
			get
			{
				return this.DataObject.ExceptIfWithImportance.Value;
			}
			set
			{
				this.DataObject.ExceptIfWithImportance = new Microsoft.Exchange.Data.Storage.Importance?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ByteQuantifiedSize? WithinSizeRangeMaximum
		{
			get
			{
				return this.DataObject.WithinSizeRangeMaximum;
			}
			set
			{
				this.DataObject.WithinSizeRangeMaximum = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ByteQuantifiedSize? ExceptIfWithinSizeRangeMaximum
		{
			get
			{
				return this.DataObject.ExceptIfWithinSizeRangeMaximum;
			}
			set
			{
				this.DataObject.ExceptIfWithinSizeRangeMaximum = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ByteQuantifiedSize? WithinSizeRangeMinimum
		{
			get
			{
				return this.DataObject.WithinSizeRangeMinimum;
			}
			set
			{
				this.DataObject.WithinSizeRangeMinimum = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ByteQuantifiedSize? ExceptIfWithinSizeRangeMinimum
		{
			get
			{
				return this.DataObject.ExceptIfWithinSizeRangeMinimum;
			}
			set
			{
				this.DataObject.ExceptIfWithinSizeRangeMinimum = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Sensitivity WithSensitivity
		{
			get
			{
				return this.DataObject.WithSensitivity.Value;
			}
			set
			{
				this.DataObject.WithSensitivity = new Sensitivity?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Sensitivity ExceptIfWithSensitivity
		{
			get
			{
				return this.DataObject.ExceptIfWithSensitivity.Value;
			}
			set
			{
				this.DataObject.ExceptIfWithSensitivity = new Sensitivity?(value);
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

		[ValidateNotNullOrEmpty]
		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> ApplyCategory
		{
			get
			{
				return this.DataObject.ApplyCategory;
			}
			set
			{
				this.DataObject.ApplyCategory = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool DeleteMessage
		{
			get
			{
				return this.DataObject.DeleteMessage;
			}
			set
			{
				this.DataObject.DeleteMessage = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(ParameterSetName = "Identity")]
		public bool MarkAsRead
		{
			get
			{
				return this.DataObject.MarkAsRead;
			}
			set
			{
				this.DataObject.MarkAsRead = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Microsoft.Exchange.Data.Storage.Importance MarkImportance
		{
			get
			{
				return this.DataObject.MarkImportance.Value;
			}
			set
			{
				this.DataObject.MarkImportance = new Microsoft.Exchange.Data.Storage.Importance?(value);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(ParameterSetName = "Identity")]
		public MultiValuedProperty<E164Number> SendTextMessageNotificationTo
		{
			get
			{
				return this.DataObject.SendTextMessageNotificationTo;
			}
			set
			{
				this.DataObject.SendTextMessageNotificationTo = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FromMessage")]
		public MailboxStoreObjectIdParameter FromMessageId
		{
			get
			{
				return (MailboxStoreObjectIdParameter)base.Fields["FromMessageId"];
			}
			set
			{
				base.Fields["FromMessageId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FromMessage")]
		public SwitchParameter ValidateOnly
		{
			get
			{
				return (SwitchParameter)base.Fields["ValidateOnly"];
			}
			set
			{
				base.Fields["ValidateOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool StopProcessingRules
		{
			get
			{
				return this.DataObject.StopProcessingRules;
			}
			set
			{
				this.DataObject.StopProcessingRules = value;
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 760, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\InboxRule\\NewInboxRule.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			InboxRuleDataProvider.ValidateInboxRuleProperties(this.DataObject, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
		}

		protected override void InternalProcessRecord()
		{
			if (this.FromMessageId != null)
			{
				if (!base.HasErrors)
				{
					this.WriteResult(this.DataObject);
				}
			}
			else
			{
				InboxRuleDataProvider inboxRuleDataProvider = (InboxRuleDataProvider)base.DataSession;
				if (this.AlwaysDeleteOutlookRulesBlob.IsPresent)
				{
					inboxRuleDataProvider.SetAlwaysDeleteOutlookRulesBlob(new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
				}
				else if (!inboxRuleDataProvider.IsAlwaysDeleteOutlookRulesBlob())
				{
					if (!inboxRuleDataProvider.HandleOutlookBlob(this.Force, () => base.ShouldContinue(Strings.WarningInboxRuleOutlookBlobExists)))
					{
						return;
					}
				}
				ManageInboxRule.ProcessRecord(new Action(base.InternalProcessRecord), new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError), this.Name);
			}
			if (this.DataObject.SendTextMessageNotificationTo.Count > 0)
			{
				SmsSqmDataPointHelper.AddNotificationConfigDataPoint(SmsSqmSession.Instance, this.adUser.Id, this.adUser.LegacyExchangeDN, SMSNotificationType.Email);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MailboxIdParameter mailboxIdParameter = null;
			if (this.FromMessageId != null && this.FromMessageId.RawOwner != null)
			{
				mailboxIdParameter = this.FromMessageId.RawOwner;
			}
			if (mailboxIdParameter == null)
			{
				ADObjectId executingUserId;
				base.TryGetExecutingUserId(out executingUserId);
				mailboxIdParameter = (this.Mailbox ?? MailboxTaskHelper.ResolveMailboxIdentity(executingUserId, new Task.ErrorLoggerDelegate(base.WriteError)));
			}
			this.adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 867, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\InboxRule\\NewInboxRule.cs");
			base.VerifyIsWithinScopes(TaskHelper.UnderscopeSessionToOrganization(tenantOrRootOrgRecipientSession, this.adUser.OrganizationId, true), this.adUser, true, new DataAccessTask<InboxRule>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
			XsoMailboxDataProviderBase xsoMailboxDataProviderBase;
			if (this.FromMessageId != null)
			{
				xsoMailboxDataProviderBase = new MailMessageDataProvider(base.SessionSettings, this.adUser, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, "New-InboxRule");
				this.FromMessageId.InternalStoreObjectId = new MailboxStoreObjectId((ADObjectId)this.adUser.Identity, this.FromMessageId.RawStoreObjectId);
			}
			else
			{
				xsoMailboxDataProviderBase = new InboxRuleDataProvider(base.SessionSettings, this.adUser, "New-InboxRule");
			}
			this.mailboxOwner = xsoMailboxDataProviderBase.MailboxSession.MailboxOwner.ObjectId.ToString();
			return xsoMailboxDataProviderBase;
		}

		protected override IConfigurable PrepareDataObject()
		{
			InboxRule inboxRule = (InboxRule)base.PrepareDataObject();
			inboxRule.Provider = (XsoMailboxDataProviderBase)base.DataSession;
			inboxRule.MailboxOwnerId = this.adUser.Id;
			if (this.FromMessageId != null)
			{
				this.PrepareDataObjectFromMessage(inboxRule);
			}
			else
			{
				this.PrepareDataObjectFromParameters(inboxRule);
				inboxRule.ValidateInterdependentParameters(new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			return inboxRule;
		}

		private void PrepareDataObjectFromMessage(InboxRule inboxRule)
		{
			inboxRule.Name = Guid.NewGuid().ToString();
			inboxRule.StopProcessingRules = true;
			MailMessage mailMessage = (MailMessage)base.GetDataObject<MailMessage>(this.FromMessageId, base.DataSession, null, null, new LocalizedString?(LocalizedString.Empty));
			if (!string.IsNullOrEmpty(mailMessage.Subject))
			{
				inboxRule.SubjectContainsWords = new MultiValuedProperty<string>(new string[]
				{
					mailMessage.Subject
				});
			}
			if (mailMessage.From != null || mailMessage.Sender != null)
			{
				inboxRule.From = new ADRecipientOrAddress[]
				{
					mailMessage.From ?? mailMessage.Sender
				};
			}
			if (mailMessage.To != null || mailMessage.Cc != null)
			{
				List<ADRecipientOrAddress> list = new List<ADRecipientOrAddress>(((mailMessage.To == null) ? 0 : mailMessage.To.Length) + ((mailMessage.Cc == null) ? 0 : mailMessage.Cc.Length));
				if (mailMessage.To != null)
				{
					list.AddRange(mailMessage.To);
				}
				if (mailMessage.Cc != null)
				{
					list.AddRange(mailMessage.Cc);
				}
				if (list.Count > 0)
				{
					inboxRule.SentTo = list.Distinct<ADRecipientOrAddress>().ToArray<ADRecipientOrAddress>();
				}
			}
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
			if (this.FlaggedForAction != null)
			{
				InboxRuleDataProvider.CheckFlaggedAction(this.FlaggedForAction, InboxRuleSchema.FlaggedForAction.Name, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
			if (this.ExceptIfFlaggedForAction != null)
			{
				InboxRuleDataProvider.CheckFlaggedAction(this.ExceptIfFlaggedForAction, InboxRuleSchema.ExceptIfFlaggedForAction.Name, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
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
				return Strings.ConfirmationMessageNewInboxRule(this.Name, this.mailboxOwner);
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
