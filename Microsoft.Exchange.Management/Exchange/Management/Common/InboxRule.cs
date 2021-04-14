using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public sealed class InboxRule : XsoMailboxConfigurationObject
	{
		public RuleDescription Description
		{
			get
			{
				return this.BuildDescription();
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[InboxRuleSchema.Enabled];
			}
			internal set
			{
				this[InboxRuleSchema.Enabled] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (ObjectId)this[InboxRuleSchema.Identity];
			}
		}

		public bool InError
		{
			get
			{
				return this.propertiesInError != null;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string Name
		{
			get
			{
				return (string)this[InboxRuleSchema.Name];
			}
			set
			{
				this[InboxRuleSchema.Name] = value;
			}
		}

		[Parameter]
		public int Priority
		{
			get
			{
				return (int)this[InboxRuleSchema.Priority];
			}
			set
			{
				this[InboxRuleSchema.Priority] = value;
			}
		}

		public ulong? RuleIdentity
		{
			get
			{
				return (ulong?)this[InboxRuleSchema.RuleIdentity];
			}
		}

		public bool SupportedByTask
		{
			get
			{
				return (bool)this[InboxRuleSchema.SupportedByTask];
			}
			internal set
			{
				this[InboxRuleSchema.SupportedByTask] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> BodyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.BodyContainsWords];
			}
			set
			{
				this[InboxRuleSchema.BodyContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfBodyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfBodyContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfBodyContainsWords] = value;
			}
		}

		[Parameter]
		public string FlaggedForAction
		{
			get
			{
				return (string)this[InboxRuleSchema.FlaggedForAction];
			}
			set
			{
				this[InboxRuleSchema.FlaggedForAction] = value;
			}
		}

		[Parameter]
		public string ExceptIfFlaggedForAction
		{
			get
			{
				return (string)this[InboxRuleSchema.ExceptIfFlaggedForAction];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfFlaggedForAction] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> FromAddressContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.FromAddressContainsWords];
			}
			set
			{
				this[InboxRuleSchema.FromAddressContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfFromAddressContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfFromAddressContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfFromAddressContainsWords] = value;
			}
		}

		public ADRecipientOrAddress[] From
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.From];
			}
			set
			{
				this[InboxRuleSchema.From] = value;
			}
		}

		public ADRecipientOrAddress[] ExceptIfFrom
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.ExceptIfFrom];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfFrom] = value;
			}
		}

		[Parameter]
		public bool HasAttachment
		{
			get
			{
				return (bool)this[InboxRuleSchema.HasAttachment];
			}
			set
			{
				this[InboxRuleSchema.HasAttachment] = value;
			}
		}

		[Parameter]
		public bool ExceptIfHasAttachment
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfHasAttachment];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfHasAttachment] = value;
			}
		}

		public MessageClassification[] HasClassification
		{
			get
			{
				return (MessageClassification[])this[InboxRuleSchema.HasClassification];
			}
			set
			{
				this[InboxRuleSchema.HasClassification] = value;
			}
		}

		public MessageClassification[] ExceptIfHasClassification
		{
			get
			{
				return (MessageClassification[])this[InboxRuleSchema.ExceptIfHasClassification];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfHasClassification] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> HeaderContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.HeaderContainsWords];
			}
			set
			{
				this[InboxRuleSchema.HeaderContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfHeaderContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfHeaderContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfHeaderContainsWords] = value;
			}
		}

		public AggregationSubscriptionIdentity[] FromSubscription
		{
			get
			{
				return (AggregationSubscriptionIdentity[])this[InboxRuleSchema.FromSubscription];
			}
			set
			{
				this[InboxRuleSchema.FromSubscription] = value;
			}
		}

		public AggregationSubscriptionIdentity[] ExceptIfFromSubscription
		{
			get
			{
				return (AggregationSubscriptionIdentity[])this[InboxRuleSchema.ExceptIfFromSubscription];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfFromSubscription] = value;
			}
		}

		[Parameter]
		public InboxRuleMessageType? MessageTypeMatches
		{
			get
			{
				return (InboxRuleMessageType?)this[InboxRuleSchema.MessageTypeMatches];
			}
			set
			{
				this[InboxRuleSchema.MessageTypeMatches] = value;
			}
		}

		[Parameter]
		public InboxRuleMessageType? ExceptIfMessageTypeMatches
		{
			get
			{
				return (InboxRuleMessageType?)this[InboxRuleSchema.ExceptIfMessageTypeMatches];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfMessageTypeMatches] = value;
			}
		}

		[Parameter]
		public bool MyNameInCcBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.MyNameInCcBox];
			}
			set
			{
				this[InboxRuleSchema.MyNameInCcBox] = value;
			}
		}

		[Parameter]
		public bool ExceptIfMyNameInCcBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfMyNameInCcBox];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfMyNameInCcBox] = value;
			}
		}

		[Parameter]
		public bool MyNameInToBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.MyNameInToBox];
			}
			set
			{
				this[InboxRuleSchema.MyNameInToBox] = value;
			}
		}

		[Parameter]
		public bool ExceptIfMyNameInToBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfMyNameInToBox];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfMyNameInToBox] = value;
			}
		}

		[Parameter]
		public bool MyNameInToOrCcBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.MyNameInToOrCcBox];
			}
			set
			{
				this[InboxRuleSchema.MyNameInToOrCcBox] = value;
			}
		}

		[Parameter]
		public bool ExceptIfMyNameInToOrCcBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfMyNameInToOrCcBox];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfMyNameInToOrCcBox] = value;
			}
		}

		[Parameter]
		public bool MyNameNotInToBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.MyNameNotInToBox];
			}
			set
			{
				this[InboxRuleSchema.MyNameNotInToBox] = value;
			}
		}

		[Parameter]
		public bool ExceptIfMyNameNotInToBox
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfMyNameNotInToBox];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfMyNameNotInToBox] = value;
			}
		}

		[Parameter]
		public ExDateTime? ReceivedAfterDate
		{
			get
			{
				return (ExDateTime?)this[InboxRuleSchema.ReceivedAfterDate];
			}
			set
			{
				this[InboxRuleSchema.ReceivedAfterDate] = value;
			}
		}

		[Parameter]
		public ExDateTime? ExceptIfReceivedAfterDate
		{
			get
			{
				return (ExDateTime?)this[InboxRuleSchema.ExceptIfReceivedAfterDate];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfReceivedAfterDate] = value;
			}
		}

		[Parameter]
		public ExDateTime? ReceivedBeforeDate
		{
			get
			{
				return (ExDateTime?)this[InboxRuleSchema.ReceivedBeforeDate];
			}
			set
			{
				this[InboxRuleSchema.ReceivedBeforeDate] = value;
			}
		}

		[Parameter]
		public ExDateTime? ExceptIfReceivedBeforeDate
		{
			get
			{
				return (ExDateTime?)this[InboxRuleSchema.ExceptIfReceivedBeforeDate];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfReceivedBeforeDate] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RecipientAddressContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.RecipientAddressContainsWords];
			}
			set
			{
				this[InboxRuleSchema.RecipientAddressContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfRecipientAddressContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfRecipientAddressContainsWords] = value;
			}
		}

		[Parameter]
		public bool SentOnlyToMe
		{
			get
			{
				return (bool)this[InboxRuleSchema.SentOnlyToMe];
			}
			set
			{
				this[InboxRuleSchema.SentOnlyToMe] = value;
			}
		}

		[Parameter]
		public bool ExceptIfSentOnlyToMe
		{
			get
			{
				return (bool)this[InboxRuleSchema.ExceptIfSentOnlyToMe];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfSentOnlyToMe] = value;
			}
		}

		public ADRecipientOrAddress[] SentTo
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.SentTo];
			}
			set
			{
				this[InboxRuleSchema.SentTo] = value;
			}
		}

		public ADRecipientOrAddress[] ExceptIfSentTo
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.ExceptIfSentTo];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfSentTo] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> SubjectContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.SubjectContainsWords];
			}
			set
			{
				this[InboxRuleSchema.SubjectContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfSubjectContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfSubjectContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfSubjectContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> SubjectOrBodyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.SubjectOrBodyContainsWords];
			}
			set
			{
				this[InboxRuleSchema.SubjectOrBodyContainsWords] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ExceptIfSubjectOrBodyContainsWords];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfSubjectOrBodyContainsWords] = value;
			}
		}

		[Parameter]
		public Microsoft.Exchange.Data.Storage.Importance? WithImportance
		{
			get
			{
				return (Microsoft.Exchange.Data.Storage.Importance?)this[InboxRuleSchema.WithImportance];
			}
			set
			{
				this[InboxRuleSchema.WithImportance] = value;
			}
		}

		[Parameter]
		public Microsoft.Exchange.Data.Storage.Importance? ExceptIfWithImportance
		{
			get
			{
				return (Microsoft.Exchange.Data.Storage.Importance?)this[InboxRuleSchema.ExceptIfWithImportance];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfWithImportance] = value;
			}
		}

		[Parameter]
		public ByteQuantifiedSize? WithinSizeRangeMaximum
		{
			get
			{
				return (ByteQuantifiedSize?)this[InboxRuleSchema.WithinSizeRangeMaximum];
			}
			set
			{
				this[InboxRuleSchema.WithinSizeRangeMaximum] = value;
			}
		}

		[Parameter]
		public ByteQuantifiedSize? ExceptIfWithinSizeRangeMaximum
		{
			get
			{
				return (ByteQuantifiedSize?)this[InboxRuleSchema.ExceptIfWithinSizeRangeMaximum];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfWithinSizeRangeMaximum] = value;
			}
		}

		[Parameter]
		public ByteQuantifiedSize? WithinSizeRangeMinimum
		{
			get
			{
				return (ByteQuantifiedSize?)this[InboxRuleSchema.WithinSizeRangeMinimum];
			}
			set
			{
				this[InboxRuleSchema.WithinSizeRangeMinimum] = value;
			}
		}

		[Parameter]
		public ByteQuantifiedSize? ExceptIfWithinSizeRangeMinimum
		{
			get
			{
				return (ByteQuantifiedSize?)this[InboxRuleSchema.ExceptIfWithinSizeRangeMinimum];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfWithinSizeRangeMinimum] = value;
			}
		}

		[Parameter]
		public Sensitivity? WithSensitivity
		{
			get
			{
				return (Sensitivity?)this[InboxRuleSchema.WithSensitivity];
			}
			set
			{
				this[InboxRuleSchema.WithSensitivity] = value;
			}
		}

		[Parameter]
		public Sensitivity? ExceptIfWithSensitivity
		{
			get
			{
				return (Sensitivity?)this[InboxRuleSchema.ExceptIfWithSensitivity];
			}
			set
			{
				this[InboxRuleSchema.ExceptIfWithSensitivity] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ApplyCategory
		{
			get
			{
				return (MultiValuedProperty<string>)this[InboxRuleSchema.ApplyCategory];
			}
			set
			{
				this[InboxRuleSchema.ApplyCategory] = value;
			}
		}

		public MailboxFolder CopyToFolder
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return this.RedactMailboxFolder((MailboxFolder)this[InboxRuleSchema.CopyToFolder]);
				}
				return (MailboxFolder)this[InboxRuleSchema.CopyToFolder];
			}
			internal set
			{
				this[InboxRuleSchema.CopyToFolder] = value;
			}
		}

		[Parameter]
		public bool DeleteMessage
		{
			get
			{
				return (bool)this[InboxRuleSchema.DeleteMessage];
			}
			set
			{
				this[InboxRuleSchema.DeleteMessage] = value;
			}
		}

		public ADRecipientOrAddress[] ForwardAsAttachmentTo
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.ForwardAsAttachmentTo];
			}
			internal set
			{
				this[InboxRuleSchema.ForwardAsAttachmentTo] = value;
			}
		}

		public ADRecipientOrAddress[] ForwardTo
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.ForwardTo];
			}
			internal set
			{
				this[InboxRuleSchema.ForwardTo] = value;
			}
		}

		[Parameter]
		public bool MarkAsRead
		{
			get
			{
				return (bool)this[InboxRuleSchema.MarkAsRead];
			}
			set
			{
				this[InboxRuleSchema.MarkAsRead] = value;
			}
		}

		[Parameter]
		public Microsoft.Exchange.Data.Storage.Importance? MarkImportance
		{
			get
			{
				return (Microsoft.Exchange.Data.Storage.Importance?)this[InboxRuleSchema.MarkImportance];
			}
			set
			{
				this[InboxRuleSchema.MarkImportance] = value;
			}
		}

		public MailboxFolder MoveToFolder
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return this.RedactMailboxFolder((MailboxFolder)this[InboxRuleSchema.MoveToFolder]);
				}
				return (MailboxFolder)this[InboxRuleSchema.MoveToFolder];
			}
			internal set
			{
				this[InboxRuleSchema.MoveToFolder] = value;
			}
		}

		public ADRecipientOrAddress[] RedirectTo
		{
			get
			{
				return (ADRecipientOrAddress[])this[InboxRuleSchema.RedirectTo];
			}
			internal set
			{
				this[InboxRuleSchema.RedirectTo] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<E164Number> SendTextMessageNotificationTo
		{
			get
			{
				return (MultiValuedProperty<E164Number>)this[InboxRuleSchema.SendTextMessageNotificationTo];
			}
			set
			{
				this[InboxRuleSchema.SendTextMessageNotificationTo] = value;
			}
		}

		[Parameter]
		public bool StopProcessingRules
		{
			get
			{
				return (bool)this[InboxRuleSchema.StopProcessingRules];
			}
			set
			{
				this[InboxRuleSchema.StopProcessingRules] = value;
			}
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return InboxRule.schema;
			}
		}

		internal RuleId RuleId
		{
			get
			{
				return (RuleId)this[InboxRuleSchema.RuleId];
			}
			set
			{
				this[InboxRuleSchema.RuleId] = value;
			}
		}

		internal XsoMailboxDataProviderBase Provider
		{
			get
			{
				return this.provider;
			}
			set
			{
				this.provider = value;
				this.culture = this.provider.MailboxSession.Culture;
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		internal ExTimeZoneValue DescriptionTimeZone
		{
			get
			{
				return this.descriptionTimeZone;
			}
			set
			{
				if (value == null || value.ExTimeZone == null || value.ExTimeZone.Id == ExTimeZone.UtcTimeZone.Id)
				{
					this.descriptionTimeZone = null;
					return;
				}
				this.descriptionTimeZone = value;
			}
		}

		internal string DescriptionTimeFormat
		{
			get
			{
				return this.descriptionTimeFormat;
			}
			set
			{
				this.descriptionTimeFormat = value;
			}
		}

		public InboxRule()
		{
			((SimplePropertyBag)this.propertyBag).SetObjectIdentityPropertyDefinition(InboxRuleSchema.Identity);
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			RuleId ruleId = (RuleId)propertyBag[InboxRuleSchema.RuleId];
			string name = (string)propertyBag[InboxRuleSchema.Name];
			if (adobjectId != null)
			{
				return new InboxRuleId(adobjectId, name, ruleId);
			}
			return null;
		}

		internal static object RuleIdentityGetter(IPropertyBag propertyBag)
		{
			RuleId ruleId = (RuleId)propertyBag[InboxRuleSchema.RuleId];
			if (ruleId != null)
			{
				return InboxRuleTaskHelper.GetRuleIdentity(ruleId);
			}
			return null;
		}

		internal void SetPropertyInError(ProviderPropertyDefinition property)
		{
			if (this.propertiesInError == null)
			{
				this.propertiesInError = new HashSet<ProviderPropertyDefinition>();
			}
			this.propertiesInError.Add(property);
		}

		internal bool IsPropertyInError(ProviderPropertyDefinition property)
		{
			return this.propertiesInError != null && this.propertiesInError.Contains(property);
		}

		internal void ValidateInterdependentParameters(ManageInboxRule.ThrowTerminatingErrorDelegate writeError)
		{
			if (base.IsModified(InboxRuleSchema.ReceivedAfterDate) || base.IsModified(InboxRuleSchema.ReceivedBeforeDate))
			{
				ManageInboxRule.VerifyRange<ExDateTime?>(this.ReceivedAfterDate, this.ReceivedBeforeDate, false, writeError, Strings.ErrorInvalidDateRangeCondition);
			}
			if (base.IsModified(InboxRuleSchema.ExceptIfReceivedAfterDate) || base.IsModified(InboxRuleSchema.ExceptIfReceivedBeforeDate))
			{
				ManageInboxRule.VerifyRange<ExDateTime?>(this.ExceptIfReceivedAfterDate, this.ExceptIfReceivedBeforeDate, false, writeError, Strings.ErrorInvalidDateRangeException);
			}
			if (base.IsModified(InboxRuleSchema.WithinSizeRangeMinimum) || base.IsModified(InboxRuleSchema.WithinSizeRangeMaximum))
			{
				ManageInboxRule.VerifyRange<ByteQuantifiedSize?>(this.WithinSizeRangeMinimum, this.WithinSizeRangeMaximum, true, writeError, Strings.ErrorInvalidSizeRangeCondition);
			}
			if (base.IsModified(InboxRuleSchema.ExceptIfWithinSizeRangeMinimum) || base.IsModified(InboxRuleSchema.ExceptIfWithinSizeRangeMaximum))
			{
				ManageInboxRule.VerifyRange<ByteQuantifiedSize?>(this.ExceptIfWithinSizeRangeMinimum, this.ExceptIfWithinSizeRangeMaximum, true, writeError, Strings.ErrorInvalidSizeRangeException);
			}
		}

		internal string GetDateString(ExDateTime dateTime)
		{
			ExDateTime exDateTime = dateTime;
			if (this.descriptionTimeZone != null && this.descriptionTimeZone.ExTimeZone != null)
			{
				exDateTime = this.descriptionTimeZone.ExTimeZone.ConvertDateTime(dateTime);
			}
			return exDateTime.ToString(this.descriptionTimeFormat);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			bool flag = false;
			foreach (InboxRuleDataProvider.ActionMappingEntry actionMappingEntry in InboxRuleDataProvider.ActionMappings)
			{
				ProviderPropertyDefinition propertyDefinition = actionMappingEntry.PropertyDefinition;
				if (this[propertyDefinition] != null && (propertyDefinition.Type != typeof(bool) || (bool)this[propertyDefinition]))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				errors.Add(new ObjectValidationError(Strings.ErrorInboxRuleHasNoAction, this.Identity, string.Empty));
			}
		}

		private RuleDescription BuildDescription()
		{
			RuleDescription ruleDescription = new RuleDescription();
			foreach (InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry in InboxRuleDataProvider.ConditionMappings)
			{
				ProviderPropertyDefinition propertyDefinition = conditionMappingEntry.PropertyDefinition;
				if (this[propertyDefinition] != null || this.IsPropertyInError(propertyDefinition))
				{
					ICollection collection = this[propertyDefinition] as ICollection;
					if (collection == null || collection.Count != 0 || this.IsPropertyInError(propertyDefinition))
					{
						string text = conditionMappingEntry.DescriptionStringDelegate(this);
						if (!string.IsNullOrEmpty(text))
						{
							if (conditionMappingEntry.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
							{
								ruleDescription.ConditionDescriptions.Add(text);
							}
							else if (conditionMappingEntry.PredicateType == InboxRuleDataProvider.PredicateType.Exception)
							{
								ruleDescription.ExceptionDescriptions.Add(text);
							}
						}
					}
				}
			}
			foreach (InboxRuleDataProvider.ActionMappingEntry actionMappingEntry in InboxRuleDataProvider.ActionMappings)
			{
				ProviderPropertyDefinition propertyDefinition2 = actionMappingEntry.PropertyDefinition;
				if (this[propertyDefinition2] != null || this.IsPropertyInError(propertyDefinition2))
				{
					string text2 = actionMappingEntry.DescriptionStringDelegate(this);
					if (!string.IsNullOrEmpty(text2))
					{
						ruleDescription.ActionDescriptions.Add(text2);
					}
				}
			}
			return ruleDescription;
		}

		internal List<string> GetClassificationNames(IList<string> classificationGuids)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2087, "GetClassificationNames", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\InboxRule.cs");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(base.MailboxOwnerId);
			IConfigurationSession systemConfigurationSession = this.provider.GetSystemConfigurationSession(adrecipient.OrganizationId);
			List<string> list = new List<string>();
			foreach (string text in classificationGuids)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Guid guid;
					try
					{
						guid = new Guid(text);
					}
					catch (OverflowException)
					{
						continue;
					}
					catch (FormatException)
					{
						continue;
					}
					ADObjectId entryId = new ADObjectId(guid);
					MessageClassification messageClassification = systemConfigurationSession.Read<MessageClassification>(entryId);
					if (messageClassification != null)
					{
						list.Add(messageClassification.DisplayName);
					}
				}
			}
			return list;
		}

		internal IList<string> GetSubscriptionEmailAddresses(IList<AggregationSubscriptionIdentity> subscriptions)
		{
			IList<string> result;
			InboxRuleDataProvider.TryGetSubscriptionEmailAddresses(this.provider.MailboxOwner, subscriptions, out result);
			return result;
		}

		private MailboxFolder RedactMailboxFolder(MailboxFolder folder)
		{
			folder.Name = SuppressingPiiData.Redact(folder.Name);
			string text;
			string text2;
			folder.FolderPath = SuppressingPiiData.Redact(folder.FolderPath, out text, out text2);
			folder.MailboxOwnerId = SuppressingPiiData.Redact(folder.MailboxOwnerId, out text, out text2);
			return folder;
		}

		private const string TabString = "\t";

		private const string CrLfString = "\r\n";

		private static InboxRuleSchema schema = ObjectSchema.GetInstance<InboxRuleSchema>();

		[NonSerialized]
		private XsoMailboxDataProviderBase provider;

		private CultureInfo culture;

		private ExTimeZoneValue descriptionTimeZone;

		private string descriptionTimeFormat;

		private HashSet<ProviderPropertyDefinition> propertiesInError;
	}
}
