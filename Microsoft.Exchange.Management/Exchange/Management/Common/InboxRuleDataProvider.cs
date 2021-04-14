using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class InboxRuleDataProvider : XsoMailboxDataProviderBase
	{
		internal ADUser ADMailboxOwner
		{
			get
			{
				return this.adMailboxOwner;
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

		internal bool IncludeHidden
		{
			get
			{
				return this.includeHidden;
			}
			set
			{
				this.includeHidden = value;
			}
		}

		internal Func<bool> ConfirmDeleteOutlookBlob { get; set; }

		public InboxRuleDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
			this.adMailboxOwner = mailboxOwner;
		}

		internal InboxRuleDataProvider()
		{
		}

		public static void CheckFlaggedAction(string flaggedAction, string parameterName, ManageInboxRule.ThrowTerminatingErrorDelegate writeErrorDelegate)
		{
			if (flaggedAction.Length > 100)
			{
				writeErrorDelegate(new LocalizedException(RulesTasksStrings.ErrorMaxParameterLengthExceeded(parameterName, 100)), ErrorCategory.InvalidArgument, flaggedAction);
			}
		}

		public bool HandleOutlookBlob(SwitchParameter force, Func<bool> shouldContinue)
		{
			if (force.IsPresent)
			{
				return true;
			}
			Rules inboxRules = base.MailboxSession.InboxRules;
			return !inboxRules.LegacyOutlookRulesCacheExists || this.IsAlwaysDeleteOutlookRulesBlob() || shouldContinue();
		}

		public void SetAlwaysDeleteOutlookRulesBlob(ManageInboxRule.ThrowTerminatingErrorDelegate writeErrorDelegate)
		{
			UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, "OWA.UserOptions", UserConfigurationTypes.Dictionary, true);
			if (mailboxConfiguration == null)
			{
				mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, "OWA.UserOptions", UserConfigurationTypes.Dictionary, true);
			}
			if (mailboxConfiguration != null)
			{
				using (mailboxConfiguration)
				{
					IDictionary dictionary = null;
					try
					{
						dictionary = mailboxConfiguration.GetDictionary();
					}
					catch (CorruptDataException exception)
					{
						writeErrorDelegate(exception, ErrorCategory.InvalidData, null);
					}
					catch (InvalidOperationException exception2)
					{
						writeErrorDelegate(exception2, ErrorCategory.InvalidOperation, null);
					}
					int? num = dictionary["NewEnabledPonts"] as int?;
					if (num == null)
					{
						num = new int?(0);
					}
					num |= 16;
					dictionary["NewEnabledPonts"] = num;
					mailboxConfiguration.Save();
				}
			}
		}

		public bool IsAlwaysDeleteOutlookRulesBlob()
		{
			UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(base.MailboxSession, "OWA.UserOptions", UserConfigurationTypes.Dictionary, false);
			if (mailboxConfiguration != null)
			{
				using (mailboxConfiguration)
				{
					IDictionary dictionary = null;
					try
					{
						dictionary = mailboxConfiguration.GetDictionary();
					}
					catch (CorruptDataException)
					{
						return false;
					}
					catch (InvalidOperationException)
					{
						return false;
					}
					int? num = dictionary["NewEnabledPonts"] as int?;
					if (num == null)
					{
						return false;
					}
					return (num.Value & 16) > 0;
				}
				return false;
			}
			return false;
		}

		public static bool IsPureAscii(string targetStr)
		{
			bool result = true;
			if (!string.IsNullOrEmpty(targetStr))
			{
				foreach (char c in targetStr)
				{
					if (c > '\u007f')
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public static void EnsurePureAscii(MultiValuedProperty<string> property, ManageInboxRule.ThrowTerminatingErrorDelegate writeErrorDelegate)
		{
			if (property.Count > 0)
			{
				foreach (string text in property)
				{
					if (!InboxRuleDataProvider.IsPureAscii(text))
					{
						writeErrorDelegate(new LocalizedException(RulesTasksStrings.ErrorInvalidCharException), ErrorCategory.InvalidArgument, text);
					}
				}
			}
		}

		public static void ValidateInboxRuleProperties(InboxRule inboxrule, ManageInboxRule.ThrowTerminatingErrorDelegate writeErrorDelegate)
		{
			if (inboxrule.FromAddressContainsWords != null && inboxrule.FromAddressContainsWords.Count > 0)
			{
				InboxRuleDataProvider.EnsurePureAscii(inboxrule.FromAddressContainsWords, writeErrorDelegate);
			}
			if (inboxrule.RecipientAddressContainsWords != null && inboxrule.RecipientAddressContainsWords.Count > 0)
			{
				InboxRuleDataProvider.EnsurePureAscii(inboxrule.RecipientAddressContainsWords, writeErrorDelegate);
			}
			if (inboxrule.ExceptIfFromAddressContainsWords != null && inboxrule.ExceptIfFromAddressContainsWords.Count > 0)
			{
				InboxRuleDataProvider.EnsurePureAscii(inboxrule.ExceptIfFromAddressContainsWords, writeErrorDelegate);
			}
			if (inboxrule.ExceptIfRecipientAddressContainsWords != null && inboxrule.ExceptIfRecipientAddressContainsWords.Count > 0)
			{
				InboxRuleDataProvider.EnsurePureAscii(inboxrule.ExceptIfRecipientAddressContainsWords, writeErrorDelegate);
			}
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			InboxRuleId inboxRuleId = rootId as InboxRuleId;
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && inboxRuleId == null)
			{
				throw new NotSupportedException("rootId");
			}
			Rules ruleCollection = null;
			if (this.IncludeHidden)
			{
				ruleCollection = base.MailboxSession.AllInboxRules;
			}
			else
			{
				ruleCollection = base.MailboxSession.InboxRules;
			}
			if (inboxRuleId == null || (inboxRuleId.Name == null && inboxRuleId.RuleId == null))
			{
				foreach (Rule rule in ruleCollection)
				{
					this.InjectRuleNameIfNeeded(rule);
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject(rule));
				}
			}
			else if (inboxRuleId.RuleId != null)
			{
				Rule rule2 = null;
				try
				{
					rule2 = ruleCollection.FindByRuleId(inboxRuleId.RuleId);
				}
				catch (ObjectNotFoundException)
				{
				}
				if (rule2 != null)
				{
					this.InjectRuleNameIfNeeded(rule2);
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject(rule2));
				}
				else if (inboxRuleId.StoreObjectId != null)
				{
					string ruleId = inboxRuleId.StoreObjectId.Value.ToString(CultureInfo.InvariantCulture);
					foreach (Rule candidate in ruleCollection)
					{
						if (string.Equals(candidate.Name, ruleId, StringComparison.OrdinalIgnoreCase))
						{
							yield return (T)((object)this.ConvertStoreObjectToPresentationObject(candidate));
						}
					}
				}
			}
			else
			{
				foreach (Rule rule3 in ruleCollection)
				{
					if (string.Equals(rule3.Name, inboxRuleId.Name, StringComparison.OrdinalIgnoreCase))
					{
						yield return (T)((object)this.ConvertStoreObjectToPresentationObject(rule3));
					}
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			InboxRule inboxRule = instance as InboxRule;
			if (inboxRule == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			Rules inboxRules = base.MailboxSession.InboxRules;
			bool flag = false;
			int num = -1;
			switch (inboxRule.ObjectState)
			{
			case ObjectState.New:
			{
				Rule rule = Rule.Create(inboxRules);
				this.CopyPresentationObjectToStoreObject(inboxRule, rule);
				int count = inboxRules.Count;
				if (inboxRule.propertyBag.IsChanged(InboxRuleSchema.Priority) && count > 0)
				{
					num = this.InsertRuleIntoCollection(inboxRules, rule);
				}
				else
				{
					inboxRules.Insert(0, rule);
				}
				flag = true;
				break;
			}
			case ObjectState.Changed:
			{
				if (inboxRule.RuleId == null)
				{
					throw new InvalidOperationException("Inbox rule to be modified has no RuleId");
				}
				int count2 = inboxRules.Count;
				Rule rule2 = inboxRules.FindByRuleId(inboxRule.RuleId);
				rule2.IsParameterInError = false;
				if (inboxRule.SupportedByTask)
				{
					if (inboxRule.propertyBag.IsChanged(InboxRuleSchema.Priority) && count2 > 1)
					{
						inboxRules.Remove(rule2);
						this.CopyPresentationObjectToStoreObject(inboxRule, rule2);
						num = this.InsertRuleIntoCollection(inboxRules, rule2);
					}
					else
					{
						this.CopyPresentationObjectToStoreObject(inboxRule, rule2);
					}
				}
				else
				{
					if (rule2.IsNotSupported || !InboxRuleDataProvider.OnlyEnableStateChanged(inboxRule))
					{
						throw new InboxRuleOperationException(Strings.ErrorInboxRuleNotSupported);
					}
					rule2.IsEnabled = inboxRule.Enabled;
				}
				flag = true;
				break;
			}
			case ObjectState.Deleted:
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			}
			if (!this.IsAlwaysDeleteOutlookRulesBlob() && this.ConfirmDeleteOutlookBlob != null && inboxRules.LegacyOutlookRulesCacheExists && !this.ConfirmDeleteOutlookBlob())
			{
				return;
			}
			if (flag)
			{
				inboxRules.Save();
			}
			if (inboxRule.RuleId == null)
			{
				inboxRules = base.MailboxSession.InboxRules;
				inboxRule.RuleId = inboxRules[(num != -1) ? num : 0].Id;
			}
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			InboxRule inboxRule = instance as InboxRule;
			if (inboxRule == null)
			{
				throw new NotSupportedException("Delete: " + instance.GetType().FullName);
			}
			Rules allInboxRules = base.MailboxSession.AllInboxRules;
			Rule item = allInboxRules.FindByRuleId(inboxRule.RuleId);
			allInboxRules.Remove(item);
			allInboxRules.Save(this.includeHidden);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InboxRuleDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		private static Guid[] GetSubscriptionGuids(IList<AggregationSubscriptionIdentity> subscriptionIds)
		{
			if (subscriptionIds == null || subscriptionIds.Count == 0)
			{
				return new Guid[0];
			}
			Guid[] array = new Guid[subscriptionIds.Count];
			for (int i = 0; i < subscriptionIds.Count; i++)
			{
				array[i] = subscriptionIds[i].SubscriptionId;
			}
			return array;
		}

		private static InboxRuleDataProvider.ActionMappingEntry GetActionMappingEntry(ActionType actionType)
		{
			foreach (InboxRuleDataProvider.ActionMappingEntry actionMappingEntry in InboxRuleDataProvider.ActionMappings)
			{
				if (actionMappingEntry.ActionType == actionType)
				{
					return actionMappingEntry;
				}
			}
			return null;
		}

		private static InboxRuleDataProvider.ConditionMappingEntry GetConditionMappingEntry(ConditionType conditionType, InboxRuleDataProvider.PredicateType predicateType)
		{
			foreach (InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry in InboxRuleDataProvider.ConditionMappings)
			{
				if (conditionMappingEntry.ConditionType == conditionType && conditionMappingEntry.PredicateType == predicateType)
				{
					return conditionMappingEntry;
				}
			}
			return null;
		}

		private static ByteQuantifiedSize? GetSize(int? nullable)
		{
			if (nullable == null)
			{
				return null;
			}
			ulong num = (ulong)((long)nullable.Value);
			num *= 1024UL;
			ByteQuantifiedSize value = new ByteQuantifiedSize(num);
			return new ByteQuantifiedSize?(value);
		}

		private static MailboxFolder ResolveMailboxFolder(InboxRuleDataProvider provider, StoreObjectId folderStoreObjectId)
		{
			Microsoft.Exchange.Data.Storage.Management.MailboxFolderId identity;
			if (folderStoreObjectId != null)
			{
				identity = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(provider.ADMailboxOwner.Id, folderStoreObjectId, null);
			}
			else
			{
				identity = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(provider.ADMailboxOwner.Id, provider.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), null);
			}
			MailboxFolder result;
			using (MailboxFolderDataProvider mailboxFolderDataProvider = new MailboxFolderDataProvider(provider.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings(), provider.ADMailboxOwner, "InboxRuleDataProvider: ResolveMailboxFolder"))
			{
				try
				{
					result = (MailboxFolder)mailboxFolderDataProvider.Read<MailboxFolder>(identity);
				}
				catch (ObjectNotFoundException)
				{
					result = null;
				}
			}
			return result;
		}

		private static QueryFilter CreateMessageClassificationIdFilter(IList<string> guids)
		{
			List<ComparisonFilter> list = new List<ComparisonFilter>(guids.Count);
			foreach (string g in guids)
			{
				Guid guid;
				if (GuidHelper.TryParseGuid(g, out guid))
				{
					ComparisonFilter item = new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, guid);
					list.Add(item);
				}
			}
			return new OrFilter(list.ToArray());
		}

		private static bool OnlyEnableStateChanged(InboxRule inboxRule)
		{
			foreach (KeyValuePair<ProviderPropertyDefinition, object> keyValuePair in inboxRule.propertyBag)
			{
				if (inboxRule.propertyBag.IsChanged(keyValuePair.Key) && keyValuePair.Key != InboxRuleSchema.Enabled && keyValuePair.Key != inboxRule.propertyBag.ObjectStatePropertyDefinition)
				{
					return false;
				}
			}
			return true;
		}

		private static MultiValuedProperty<string> BuildMultiValuedPropertyWithoutDuplicates(IEnumerable<string> inputList)
		{
			HashSet<string> hashSet = new HashSet<string>(inputList, StringComparer.OrdinalIgnoreCase);
			string[] array = new string[hashSet.Count];
			hashSet.CopyTo(array);
			return new MultiValuedProperty<string>(array);
		}

		private static void ConvertStringListToLowerInvariant(IList<string> inputList)
		{
			if (inputList == null || inputList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < inputList.Count; i++)
			{
				inputList[i] = inputList[i].ToLowerInvariant();
			}
		}

		private static int? GetSize(ByteQuantifiedSize? nullable)
		{
			if (nullable == null)
			{
				return null;
			}
			ulong num = nullable.Value.ToBytes();
			if (num == 0UL)
			{
				return null;
			}
			num /= 1024UL;
			if (num > 2147483647UL)
			{
				return new int?(int.MaxValue);
			}
			return new int?((int)num);
		}

		private static void RemoveMatching<T>(ICollection<T> collection, Func<T, bool> predicate)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			List<T> list = null;
			foreach (T t in collection)
			{
				if (predicate(t))
				{
					if (list == null)
					{
						list = new List<T>();
					}
					list.Add(t);
				}
			}
			if (list != null)
			{
				foreach (T item in list)
				{
					collection.Remove(item);
				}
			}
		}

		private static string[] BuildRecipientStringList(ADRecipientOrAddress[] addresses)
		{
			List<string> list = new List<string>();
			foreach (ADRecipientOrAddress adrecipientOrAddress in addresses)
			{
				list.Add(adrecipientOrAddress.DisplayName);
			}
			return list.ToArray();
		}

		private static string[] BuildE164NumberStringList(MultiValuedProperty<E164Number> numbers)
		{
			List<string> list = new List<string>(numbers.Count);
			foreach (E164Number e164Number in numbers)
			{
				list.Add(e164Number.ToString());
			}
			return list.ToArray();
		}

		private static string GetImportanceDescription(Microsoft.Exchange.Data.Storage.Importance importance)
		{
			string text = null;
			switch (importance)
			{
			case Microsoft.Exchange.Data.Storage.Importance.Low:
				text = ServerStrings.InboxRuleImportanceLow;
				break;
			case Microsoft.Exchange.Data.Storage.Importance.Normal:
				text = ServerStrings.InboxRuleImportanceNormal;
				break;
			case Microsoft.Exchange.Data.Storage.Importance.High:
				text = ServerStrings.InboxRuleImportanceHigh;
				break;
			}
			if (text != null)
			{
				return RulesTasksStrings.InboxRuleDescriptionWithImportance(text);
			}
			return string.Empty;
		}

		private static string GetSensitivityDescription(Sensitivity sensitivity)
		{
			string text = null;
			switch (sensitivity)
			{
			case Sensitivity.Normal:
				text = ServerStrings.InboxRuleSensitivityNormal;
				break;
			case Sensitivity.Personal:
				text = ServerStrings.InboxRuleSensitivityPersonal;
				break;
			case Sensitivity.Private:
				text = ServerStrings.InboxRuleSensitivityPrivate;
				break;
			case Sensitivity.CompanyConfidential:
				text = ServerStrings.InboxRuleSensitivityCompanyConfidential;
				break;
			}
			if (text != null)
			{
				return RulesTasksStrings.InboxRuleDescriptionWithSensitivity(text);
			}
			return string.Empty;
		}

		private static string GetRequestedActionDescription(string action)
		{
			int requestedActionLocalizedStringEnumIndex = FlaggedForActionCondition.GetRequestedActionLocalizedStringEnumIndex(action);
			if (requestedActionLocalizedStringEnumIndex > 0)
			{
				switch (requestedActionLocalizedStringEnumIndex)
				{
				case 1:
					return ClientStrings.RequestedActionCall;
				case 2:
					return ClientStrings.RequestedActionDoNotForward;
				case 3:
					return ClientStrings.RequestedActionFollowUp;
				case 5:
					return ClientStrings.RequestedActionForward;
				case 7:
					return ClientStrings.RequestedActionRead;
				case 8:
					return ClientStrings.RequestedActionReply;
				case 9:
					return ClientStrings.RequestedActionReplyToAll;
				case 10:
					return ClientStrings.RequestedActionReview;
				}
			}
			return null;
		}

		private static string GetFlaggedForActionDescription(string action)
		{
			if (string.IsNullOrEmpty(action))
			{
				return string.Empty;
			}
			if (string.Equals(action, LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, RequestedAction.Any), StringComparison.OrdinalIgnoreCase))
			{
				return RulesTasksStrings.InboxRuleDescriptionFlaggedForAnyAction;
			}
			if (string.Equals(action, LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, RequestedAction.ForYourInformation), StringComparison.OrdinalIgnoreCase))
			{
				return RulesTasksStrings.InboxRuleDescriptionFlaggedForFYI;
			}
			if (string.Equals(action, LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, RequestedAction.NoResponseNecessary), StringComparison.OrdinalIgnoreCase))
			{
				return RulesTasksStrings.InboxRuleDescriptionFlaggedForNoResponse;
			}
			string requestedActionDescription = InboxRuleDataProvider.GetRequestedActionDescription(action);
			if (requestedActionDescription != null)
			{
				return RulesTasksStrings.InboxRuleDescriptionFlaggedForAction(requestedActionDescription);
			}
			return RulesTasksStrings.InboxRuleDescriptionFlaggedForAction(action);
		}

		private static InboxRuleMessageType? GetMessageTypeFromCondition(Microsoft.Exchange.Data.Storage.Condition condition)
		{
			if (condition is AutomaticForwardCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.AutomaticForward);
			}
			if (condition is MarkedAsOofCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.AutomaticReply);
			}
			if (condition is MeetingMessageCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.Calendaring);
			}
			if (condition is EncryptedCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.Encrypted);
			}
			if (condition is NdrCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.NonDeliveryReport);
			}
			if (condition is PermissionControlledCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.PermissionControlled);
			}
			if (condition is ReadReceiptCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.ReadReceipt);
			}
			if (condition is SignedCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.Signed);
			}
			if (condition is VoicemailCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.Voicemail);
			}
			if (condition is MeetingResponseCondition)
			{
				return new InboxRuleMessageType?(InboxRuleMessageType.CalendaringResponse);
			}
			FormsCondition formsCondition = condition as FormsCondition;
			if (formsCondition != null && formsCondition.Text.Length == 1)
			{
				string text = formsCondition.Text[0];
				string a;
				if ((a = text) != null && a == "IPM.Note.Microsoft.Approval.Request")
				{
					return new InboxRuleMessageType?(InboxRuleMessageType.ApprovalRequest);
				}
			}
			return null;
		}

		private static Microsoft.Exchange.Data.Storage.Condition GetConditionFromMessageType(Rule rule, InboxRuleMessageType messageType)
		{
			switch (messageType)
			{
			case InboxRuleMessageType.AutomaticReply:
				return MarkedAsOofCondition.Create(rule);
			case InboxRuleMessageType.AutomaticForward:
				return AutomaticForwardCondition.Create(rule);
			case InboxRuleMessageType.Encrypted:
				return EncryptedCondition.Create(rule);
			case InboxRuleMessageType.Calendaring:
				return MeetingMessageCondition.Create(rule);
			case InboxRuleMessageType.CalendaringResponse:
				return MeetingResponseCondition.Create(rule);
			case InboxRuleMessageType.PermissionControlled:
				return PermissionControlledCondition.Create(rule);
			case InboxRuleMessageType.Voicemail:
				return VoicemailCondition.Create(rule);
			case InboxRuleMessageType.Signed:
				return SignedCondition.Create(rule);
			case InboxRuleMessageType.ApprovalRequest:
				return ApprovalRequestCondition.Create(rule);
			case InboxRuleMessageType.ReadReceipt:
				return ReadReceiptCondition.Create(rule);
			case InboxRuleMessageType.NonDeliveryReport:
				return NdrCondition.Create(rule);
			default:
				return null;
			}
		}

		private static string GetMessageTypeDescription(InboxRuleMessageType messageType)
		{
			string text = null;
			switch (messageType)
			{
			case InboxRuleMessageType.AutomaticReply:
				text = ServerStrings.InboxRuleMessageTypeAutomaticReply;
				break;
			case InboxRuleMessageType.AutomaticForward:
				text = ServerStrings.InboxRuleMessageTypeAutomaticForward;
				break;
			case InboxRuleMessageType.Encrypted:
				text = ServerStrings.InboxRuleMessageTypeEncrypted;
				break;
			case InboxRuleMessageType.Calendaring:
				text = ServerStrings.InboxRuleMessageTypeCalendaring;
				break;
			case InboxRuleMessageType.CalendaringResponse:
				text = ServerStrings.InboxRuleMessageTypeCalendaringResponse;
				break;
			case InboxRuleMessageType.PermissionControlled:
				text = ServerStrings.InboxRuleMessageTypePermissionControlled;
				break;
			case InboxRuleMessageType.Voicemail:
				text = ServerStrings.InboxRuleMessageTypeVoicemail;
				break;
			case InboxRuleMessageType.Signed:
				text = ServerStrings.InboxRuleMessageTypeSigned;
				break;
			case InboxRuleMessageType.ApprovalRequest:
				text = ServerStrings.InboxRuleMessageTypeApprovalRequest;
				break;
			case InboxRuleMessageType.ReadReceipt:
				text = ServerStrings.InboxRuleMessageTypeReadReceipt;
				break;
			case InboxRuleMessageType.NonDeliveryReport:
				text = ServerStrings.InboxRuleMessageTypeNonDeliveryReport;
				break;
			}
			if (text == null)
			{
				return null;
			}
			return RulesTasksStrings.InboxRuleDescriptionMessageType(text);
		}

		private bool ResolveClassifications(IList<string> guids, out List<MessageClassification> classifications)
		{
			if (guids == null || guids.Count == 0)
			{
				classifications = new List<MessageClassification>();
				return true;
			}
			IConfigurationSession configurationSession;
			if (SharedConfiguration.IsDehydratedConfiguration(this.ADMailboxOwner.OrganizationId))
			{
				configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(this.ADMailboxOwner.OrganizationId);
			}
			else
			{
				configurationSession = base.GetSystemConfigurationSession(this.ADMailboxOwner.OrganizationId);
			}
			QueryFilter filter = InboxRuleDataProvider.CreateMessageClassificationIdFilter(guids);
			ADPagedReader<MessageClassification> adpagedReader = configurationSession.FindPaged<MessageClassification>(configurationSession.SessionSettings.CurrentOrganizationId.ConfigurationUnit, QueryScope.SubTree, filter, null, 0);
			classifications = new List<MessageClassification>(guids.Count);
			foreach (MessageClassification item in adpagedReader)
			{
				classifications.Add(item);
			}
			return classifications.Count == guids.Count;
		}

		private int InsertRuleIntoCollection(Rules ruleCollection, Rule rule)
		{
			int num = 0;
			while (num < ruleCollection.Count && rule.Sequence > ruleCollection[num].Sequence)
			{
				num++;
			}
			ruleCollection.Insert(num, rule);
			return num;
		}

		private void InjectRuleNameIfNeeded(Rule rule)
		{
			if (this.IncludeHidden && string.IsNullOrEmpty(rule.Name))
			{
				rule.Name = "No rule name found";
				if (rule.Id != null)
				{
					if (rule.Provider == DelegateUserCollection.DelegateRuleProvider)
					{
						rule.Name = string.Format("{0} {1}", "Delegate Rule", rule.Id.StoreRuleId.ToString());
						return;
					}
					rule.Name = rule.Id.StoreRuleId.ToString();
				}
			}
		}

		private InboxRule ConvertStoreObjectToPresentationObject(Rule rule)
		{
			InboxRule inboxRule = new InboxRule();
			inboxRule.Provider = this;
			inboxRule.DescriptionTimeZone = this.descriptionTimeZone;
			inboxRule.DescriptionTimeFormat = this.descriptionTimeFormat;
			inboxRule.MailboxOwnerId = base.MailboxSession.MailboxOwner.ObjectId;
			inboxRule.RuleId = rule.Id;
			inboxRule.Name = rule.Name;
			inboxRule.Priority = rule.Sequence - 9;
			inboxRule.Enabled = rule.IsEnabled;
			if (rule.IsParameterInError)
			{
				inboxRule.SetPropertyInError(InboxRuleSchema.StoreObjectInError);
			}
			inboxRule.SupportedByTask = !rule.IsNotSupported;
			foreach (Microsoft.Exchange.Data.Storage.Condition condition in rule.Conditions)
			{
				InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry = InboxRuleDataProvider.GetConditionMappingEntry(condition.ConditionType, InboxRuleDataProvider.PredicateType.Condition);
				if (conditionMappingEntry != null)
				{
					conditionMappingEntry.RuleConditionToPresentationDelegate(condition, inboxRule, this);
				}
				else
				{
					inboxRule.SupportedByTask = false;
				}
			}
			foreach (Microsoft.Exchange.Data.Storage.Condition condition2 in rule.Exceptions)
			{
				InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry2 = InboxRuleDataProvider.GetConditionMappingEntry(condition2.ConditionType, InboxRuleDataProvider.PredicateType.Exception);
				if (conditionMappingEntry2 != null)
				{
					conditionMappingEntry2.RuleConditionToPresentationDelegate(condition2, inboxRule, this);
				}
				else
				{
					inboxRule.SupportedByTask = false;
				}
			}
			foreach (ActionBase actionBase in rule.Actions)
			{
				InboxRuleDataProvider.ActionMappingEntry actionMappingEntry = InboxRuleDataProvider.GetActionMappingEntry(actionBase.ActionType);
				if (actionMappingEntry != null)
				{
					actionMappingEntry.RuleActionToPresentationDelegate(actionBase, inboxRule, this);
				}
				else
				{
					inboxRule.SupportedByTask = false;
				}
			}
			return inboxRule;
		}

		private void CopyPresentationObjectToStoreObject(InboxRule inboxRule, Rule rule)
		{
			this.CopyPropertiesToStoreObject(inboxRule, rule);
			this.CopyConditionsToStoreObject(inboxRule, rule);
			this.CopyActionsToStoreObject(inboxRule, rule);
			this.UpdateRuleProvider(rule);
		}

		private void CopyPropertiesToStoreObject(InboxRule inboxRule, Rule rule)
		{
			if (inboxRule.RuleId != null)
			{
				InboxRuleTaskHelper.SetRuleId(rule, inboxRule.RuleId);
			}
			if (inboxRule.propertyBag.IsChanged(InboxRuleSchema.Enabled))
			{
				rule.IsEnabled = inboxRule.Enabled;
			}
			if (inboxRule.propertyBag.IsChanged(InboxRuleSchema.Name))
			{
				rule.Name = inboxRule.Name;
			}
			if (inboxRule.propertyBag.IsChanged(InboxRuleSchema.Priority))
			{
				int num = inboxRule.Priority + 9;
				if (rule.Sequence < num)
				{
					InboxRuleTaskHelper.SetRuleSequence(rule, num + 1);
					return;
				}
				InboxRuleTaskHelper.SetRuleSequence(rule, num);
			}
		}

		private void CopyConditionsToStoreObject(InboxRule inboxRule, Rule rule)
		{
			InboxRuleDataProvider.ConditionMappingEntry[] conditionMappings = InboxRuleDataProvider.ConditionMappings;
			for (int i = 0; i < conditionMappings.Length; i++)
			{
				InboxRuleDataProvider.ConditionMappingEntry entry = conditionMappings[i];
				if (inboxRule.propertyBag.IsChanged(entry.PropertyDefinition))
				{
					if (entry.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
					{
						InboxRuleDataProvider.RemoveMatching<Microsoft.Exchange.Data.Storage.Condition>(rule.Conditions, (Microsoft.Exchange.Data.Storage.Condition condition) => condition.ConditionType == entry.ConditionType);
					}
					else if (entry.PredicateType == InboxRuleDataProvider.PredicateType.Exception)
					{
						InboxRuleDataProvider.RemoveMatching<Microsoft.Exchange.Data.Storage.Condition>(rule.Exceptions, (Microsoft.Exchange.Data.Storage.Condition condition) => condition.ConditionType == entry.ConditionType);
					}
					entry.AddPresentationPropertyToRuleObject(inboxRule, rule, this);
				}
			}
		}

		private void CopyActionsToStoreObject(InboxRule inboxRule, Rule rule)
		{
			InboxRuleDataProvider.ActionMappingEntry[] actionMappings = InboxRuleDataProvider.ActionMappings;
			for (int i = 0; i < actionMappings.Length; i++)
			{
				InboxRuleDataProvider.ActionMappingEntry entry = actionMappings[i];
				if (inboxRule.propertyBag.IsChanged(entry.PropertyDefinition))
				{
					InboxRuleDataProvider.RemoveMatching<ActionBase>(rule.Actions, (ActionBase action) => action.ActionType == entry.ActionType);
					entry.AddPresentationPropertyToRuleObject(inboxRule, rule, this);
				}
			}
			if (rule.Actions.Count == 0)
			{
				throw new InboxRuleOperationException(RulesTasksStrings.ErrorInboxRuleMustHaveActions);
			}
		}

		private void UpdateRuleProvider(Rule rule)
		{
			RuleProviderId ruleProviderId = RuleProviderId.OL98Plus;
			foreach (Microsoft.Exchange.Data.Storage.Condition condition in rule.Conditions)
			{
				InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry = InboxRuleDataProvider.GetConditionMappingEntry(condition.ConditionType, InboxRuleDataProvider.PredicateType.Condition);
				if (conditionMappingEntry != null && conditionMappingEntry.RuleProviderId > ruleProviderId)
				{
					ruleProviderId = conditionMappingEntry.RuleProviderId;
				}
			}
			foreach (Microsoft.Exchange.Data.Storage.Condition condition2 in rule.Exceptions)
			{
				InboxRuleDataProvider.ConditionMappingEntry conditionMappingEntry2 = InboxRuleDataProvider.GetConditionMappingEntry(condition2.ConditionType, InboxRuleDataProvider.PredicateType.Exception);
				if (conditionMappingEntry2 != null && conditionMappingEntry2.RuleProviderId > ruleProviderId)
				{
					ruleProviderId = conditionMappingEntry2.RuleProviderId;
				}
			}
			foreach (ActionBase actionBase in rule.Actions)
			{
				InboxRuleDataProvider.ActionMappingEntry actionMappingEntry = InboxRuleDataProvider.GetActionMappingEntry(actionBase.ActionType);
				if (actionMappingEntry != null && actionMappingEntry.RuleProviderId > ruleProviderId)
				{
					ruleProviderId = actionMappingEntry.RuleProviderId;
				}
			}
			rule.Provider = InboxRuleProviderMapping.GetRuleProviderString(ruleProviderId);
		}

		internal static PimSubscriptionProxy[] GetAllSubscriptions(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			AggregationSubscriptionDataProvider aggregationSubscriptionDataProvider = new AggregationSubscriptionDataProvider(AggregationTaskType.Get, user.Session, user);
			return (PimSubscriptionProxy[])aggregationSubscriptionDataProvider.Find<PimSubscriptionProxy>(null, null, false, null);
		}

		private bool TryGetSubscriptionIdentities(Guid[] subscriptionGuids, out AggregationSubscriptionIdentity[] subscriptions)
		{
			bool result = true;
			List<AggregationSubscriptionIdentity> list = new List<AggregationSubscriptionIdentity>((subscriptionGuids != null) ? subscriptionGuids.Length : 0);
			if (subscriptionGuids != null && subscriptionGuids.Length > 0)
			{
				PimSubscriptionProxy[] allSubscriptions = InboxRuleDataProvider.GetAllSubscriptions(this.ADMailboxOwner);
				HashSet<Guid> hashSet = new HashSet<Guid>(subscriptionGuids);
				if (allSubscriptions != null && allSubscriptions.Length > 0)
				{
					int num = 0;
					while (num < allSubscriptions.Length && hashSet.Count > 0)
					{
						PimSubscriptionProxy pimSubscriptionProxy = allSubscriptions[num];
						if (hashSet.Contains(pimSubscriptionProxy.Subscription.SubscriptionGuid))
						{
							list.Add(pimSubscriptionProxy.Subscription.SubscriptionIdentity);
							hashSet.Remove(pimSubscriptionProxy.Subscription.SubscriptionGuid);
						}
						num++;
					}
				}
				if (hashSet.Count > 0)
				{
					result = false;
				}
			}
			subscriptions = list.ToArray();
			return result;
		}

		internal static bool TryGetSubscriptionEmailAddresses(ADUser mailboxOwner, IList<AggregationSubscriptionIdentity> subscriptionIds, out IList<string> emailAddresses)
		{
			bool result = true;
			emailAddresses = new List<string>((subscriptionIds != null) ? subscriptionIds.Count : 0);
			if (subscriptionIds != null && subscriptionIds.Count > 0)
			{
				HashSet<Guid> hashSet = new HashSet<Guid>(from subscription in subscriptionIds
				select subscription.SubscriptionId);
				IList<PimSubscriptionProxy> allSubscriptions = InboxRuleDataProvider.GetAllSubscriptions(mailboxOwner);
				int num = 0;
				while (num < allSubscriptions.Count && hashSet.Count > 0)
				{
					PimSubscriptionProxy pimSubscriptionProxy = allSubscriptions[num];
					if (hashSet.Contains(pimSubscriptionProxy.Subscription.SubscriptionGuid))
					{
						emailAddresses.Add(pimSubscriptionProxy.EmailAddress.ToString());
						hashSet.Remove(pimSubscriptionProxy.Subscription.SubscriptionGuid);
					}
					num++;
				}
				if (hashSet.Count > 0)
				{
					result = false;
				}
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static InboxRuleDataProvider()
		{
			InboxRuleDataProvider.ActionMappingEntry[] array = new InboxRuleDataProvider.ActionMappingEntry[11];
			array[0] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.ApplyCategory, RuleProviderId.Exchange14, ActionType.AssignCategoriesAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				AssignCategoriesAction assignCategoriesAction = (AssignCategoriesAction)actionBase;
				inboxRule.ApplyCategory = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(assignCategoriesAction.Categories);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ApplyCategory))
				{
					rule.Actions.Add(AssignCategoriesAction.Create(inboxRule.ApplyCategory.ToArray(), rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ApplyCategory))
				{
					string category = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ApplyCategory.ToArray(), RulesTasksStrings.InboxRuleDescriptionAnd, 200);
					return RulesTasksStrings.InboxRuleDescriptionApplyCategory(category);
				}
				return string.Empty;
			});
			array[1] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.CopyToFolder, RuleProviderId.OL98Plus, ActionType.CopyToFolderAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				CopyToFolderAction copyToFolderAction = (CopyToFolderAction)actionBase;
				inboxRule.CopyToFolder = InboxRuleDataProvider.ResolveMailboxFolder(inboxRuleDataProvider, copyToFolderAction.Id);
				if (inboxRule.CopyToFolder == null)
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.CopyToFolder);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.CopyToFolder != null)
				{
					rule.Actions.Add(CopyToFolderAction.Create(inboxRule.CopyToFolder.InternalFolderIdentity.ObjectId, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.CopyToFolder != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionCopyToFolder(inboxRule.CopyToFolder.Name);
				}
				if (inboxRule.IsPropertyInError(InboxRuleSchema.CopyToFolder))
				{
					return RulesTasksStrings.InboxRuleDescriptionCopyToFolder(RulesTasksStrings.InboxRuleDescriptionFolderNotFound);
				}
				return string.Empty;
			});
			array[2] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.DeleteMessage, RuleProviderId.OL98Plus, ActionType.DeleteAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.DeleteMessage = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.DeleteMessage)
				{
					rule.Actions.Add(DeleteAction.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.DeleteMessage)
				{
					return RulesTasksStrings.InboxRuleDescriptionDeleteMessage;
				}
				return string.Empty;
			});
			array[3] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.ForwardAsAttachmentTo, RuleProviderId.OL98Plus, ActionType.ForwardAsAttachmentToRecipientsAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ForwardAsAttachmentToRecipientsAction forwardAsAttachmentToRecipientsAction = (ForwardAsAttachmentToRecipientsAction)actionBase;
				IList<Participant> participants = forwardAsAttachmentToRecipientsAction.Participants;
				inboxRule.ForwardAsAttachmentTo = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.ForwardAsAttachmentTo[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ForwardAsAttachmentTo != null && inboxRule.ForwardAsAttachmentTo.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.ForwardAsAttachmentTo.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.ForwardAsAttachmentTo)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Actions.Add(ForwardAsAttachmentToRecipientsAction.Create(list, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ForwardAsAttachmentTo != null && inboxRule.ForwardAsAttachmentTo.Length != 0)
				{
					string recipients = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.ForwardAsAttachmentTo), RulesTasksStrings.InboxRuleDescriptionAnd, 200);
					return RulesTasksStrings.InboxRuleDescriptionForwardAsAttachmentTo(recipients);
				}
				return string.Empty;
			});
			array[4] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.ForwardTo, RuleProviderId.OL98Plus, ActionType.ForwardToRecipientsAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ForwardToRecipientsAction forwardToRecipientsAction = (ForwardToRecipientsAction)actionBase;
				IList<Participant> participants = forwardToRecipientsAction.Participants;
				inboxRule.ForwardTo = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.ForwardTo[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ForwardTo != null && inboxRule.ForwardTo.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.ForwardTo.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.ForwardTo)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Actions.Add(ForwardToRecipientsAction.Create(list, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ForwardTo != null && inboxRule.ForwardTo.Length != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.ForwardTo), RulesTasksStrings.InboxRuleDescriptionAnd, 200);
					return RulesTasksStrings.InboxRuleDescriptionForwardTo(address);
				}
				return string.Empty;
			});
			array[5] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.MarkAsRead, RuleProviderId.Exchange14, ActionType.MarkAsReadAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.MarkAsRead = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MarkAsRead)
				{
					rule.Actions.Add(MarkAsReadAction.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MarkAsRead)
				{
					return RulesTasksStrings.InboxRuleDescriptionMarkAsRead;
				}
				return string.Empty;
			});
			array[6] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.MarkImportance, RuleProviderId.Exchange14, ActionType.MarkImportanceAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MarkImportanceAction markImportanceAction = actionBase as MarkImportanceAction;
				Microsoft.Exchange.Data.Storage.Importance importance = markImportanceAction.Importance;
				inboxRule.MarkImportance = new Microsoft.Exchange.Data.Storage.Importance?(importance);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MarkImportance != null)
				{
					Microsoft.Exchange.Data.Storage.Importance value = inboxRule.MarkImportance.Value;
					rule.Actions.Add(MarkImportanceAction.Create(value, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MarkImportance != null)
				{
					string importance;
					switch (inboxRule.MarkImportance.Value)
					{
					case Microsoft.Exchange.Data.Storage.Importance.Low:
						importance = ServerStrings.InboxRuleImportanceLow;
						break;
					case Microsoft.Exchange.Data.Storage.Importance.Normal:
						importance = ServerStrings.InboxRuleImportanceNormal;
						break;
					case Microsoft.Exchange.Data.Storage.Importance.High:
						importance = ServerStrings.InboxRuleImportanceHigh;
						break;
					default:
						return string.Empty;
					}
					return RulesTasksStrings.InboxRuleDescriptionMarkImportance(importance);
				}
				return string.Empty;
			});
			array[7] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.MoveToFolder, RuleProviderId.OL98Plus, ActionType.MoveToFolderAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MoveToFolderAction moveToFolderAction = (MoveToFolderAction)actionBase;
				inboxRule.MoveToFolder = InboxRuleDataProvider.ResolveMailboxFolder(inboxRuleDataProvider, moveToFolderAction.Id);
				if (inboxRule.MoveToFolder == null)
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.MoveToFolder);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MoveToFolder != null)
				{
					rule.Actions.Add(MoveToFolderAction.Create(inboxRule.MoveToFolder.InternalFolderIdentity.ObjectId, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MoveToFolder != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionMoveToFolder(inboxRule.MoveToFolder.Name);
				}
				if (inboxRule.IsPropertyInError(InboxRuleSchema.MoveToFolder))
				{
					return RulesTasksStrings.InboxRuleDescriptionMoveToFolder(RulesTasksStrings.InboxRuleDescriptionFolderNotFound);
				}
				return string.Empty;
			});
			array[8] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.RedirectTo, RuleProviderId.OL98Plus, ActionType.RedirectToRecipientsAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				RedirectToRecipientsAction redirectToRecipientsAction = (RedirectToRecipientsAction)actionBase;
				IList<Participant> participants = redirectToRecipientsAction.Participants;
				inboxRule.RedirectTo = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.RedirectTo[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.RedirectTo != null && inboxRule.RedirectTo.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.RedirectTo.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.RedirectTo)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Actions.Add(RedirectToRecipientsAction.Create(list, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.RedirectTo != null && inboxRule.RedirectTo.Length != 0)
				{
					string recipients = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.RedirectTo), RulesTasksStrings.InboxRuleDescriptionAnd, 200);
					return RulesTasksStrings.InboxRuleDescriptionRedirectTo(recipients);
				}
				return string.Empty;
			});
			array[9] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.SendTextMessageNotificationTo, RuleProviderId.Exchange14, ActionType.SendSmsAlertToRecipientsAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				SendSmsAlertToRecipientsAction sendSmsAlertToRecipientsAction = (SendSmsAlertToRecipientsAction)actionBase;
				IList<Participant> participants = sendSmsAlertToRecipientsAction.Participants;
				if (participants.Count > 0)
				{
					inboxRule.SendTextMessageNotificationTo = new MultiValuedProperty<E164Number>();
					for (int i = 0; i < participants.Count; i++)
					{
						E164Number item;
						if (E164Number.TryParse(participants[i].EmailAddress, out item) && !inboxRule.SendTextMessageNotificationTo.Contains(item))
						{
							inboxRule.SendTextMessageNotificationTo.Add(item);
						}
					}
					if (inboxRuleDataProvider.ADMailboxOwner.IsPersonToPersonTextMessagingEnabled || !inboxRuleDataProvider.ADMailboxOwner.IsMachineToPersonTextMessagingEnabled)
					{
						inboxRule.SetPropertyInError(InboxRuleSchema.MachineToPersonTextMessagingDisabled);
					}
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.SendTextMessageNotificationTo != null && inboxRule.SendTextMessageNotificationTo.Count != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.SendTextMessageNotificationTo.Count);
					foreach (E164Number e164Number in inboxRule.SendTextMessageNotificationTo)
					{
						list.Add(new Participant(e164Number.ToString(), e164Number.ToString(), "MOBILE"));
					}
					rule.Actions.Add(SendSmsAlertToRecipientsAction.Create(list, rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.SendTextMessageNotificationTo != null && inboxRule.SendTextMessageNotificationTo.Count != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildE164NumberStringList(inboxRule.SendTextMessageNotificationTo), RulesTasksStrings.InboxRuleDescriptionAnd, 200);
					return RulesTasksStrings.InboxRuleDescriptionSendTextMessageNotificationTo(address);
				}
				return string.Empty;
			});
			array[10] = new InboxRuleDataProvider.ActionMappingEntry(InboxRuleSchema.StopProcessingRules, RuleProviderId.OL98Plus, ActionType.StopProcessingAction, delegate(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.StopProcessingRules = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.StopProcessingRules)
				{
					rule.Actions.Add(StopProcessingAction.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.StopProcessingRules)
				{
					return RulesTasksStrings.InboxRuleDescriptionStopProcessingRules;
				}
				return string.Empty;
			});
			InboxRuleDataProvider.ActionMappings = array;
			InboxRuleDataProvider.ConditionMappingEntry[] array2 = new InboxRuleDataProvider.ConditionMappingEntry[68];
			array2[0] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.BodyContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsBodyStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsBodyStringCondition containsBodyStringCondition = (ContainsBodyStringCondition)condition;
				inboxRule.BodyContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsBodyStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.BodyContainsWords))
				{
					rule.Conditions.Add(ContainsBodyStringCondition.Create(rule, inboxRule.BodyContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.BodyContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.BodyContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionBodyContainsWords(words);
				}
				return string.Empty;
			});
			array2[1] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfBodyContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsBodyStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsBodyStringCondition containsBodyStringCondition = (ContainsBodyStringCondition)condition;
				inboxRule.ExceptIfBodyContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsBodyStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfBodyContainsWords))
				{
					rule.Exceptions.Add(ContainsBodyStringCondition.Create(rule, inboxRule.ExceptIfBodyContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfBodyContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfBodyContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionBodyContainsWords(words);
				}
				return string.Empty;
			});
			array2[2] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.FlaggedForAction, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.FlaggedForActionCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FlaggedForActionCondition flaggedForActionCondition = (FlaggedForActionCondition)baseCondition;
				inboxRule.FlaggedForAction = flaggedForActionCondition.Action;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!string.IsNullOrEmpty(inboxRule.FlaggedForAction))
				{
					FlaggedForActionCondition item = FlaggedForActionCondition.Create(rule, inboxRule.FlaggedForAction);
					rule.Conditions.Add(item);
				}
			}, (InboxRule inboxRule) => InboxRuleDataProvider.GetFlaggedForActionDescription(inboxRule.FlaggedForAction));
			array2[3] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfFlaggedForAction, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.FlaggedForActionCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FlaggedForActionCondition flaggedForActionCondition = (FlaggedForActionCondition)baseCondition;
				inboxRule.ExceptIfFlaggedForAction = flaggedForActionCondition.Action;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!string.IsNullOrEmpty(inboxRule.ExceptIfFlaggedForAction))
				{
					FlaggedForActionCondition item = FlaggedForActionCondition.Create(rule, inboxRule.ExceptIfFlaggedForAction);
					rule.Exceptions.Add(item);
				}
			}, (InboxRule inboxRule) => InboxRuleDataProvider.GetFlaggedForActionDescription(inboxRule.ExceptIfFlaggedForAction));
			array2[4] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.From, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.FromRecipientsCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FromRecipientsCondition fromRecipientsCondition = (FromRecipientsCondition)condition;
				IList<Participant> participants = fromRecipientsCondition.Participants;
				inboxRule.From = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.From[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.From != null && inboxRule.From.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.From.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.From)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Conditions.Add(FromRecipientsCondition.Create(rule, list));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.From != null && inboxRule.From.Length != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.From), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFrom(address);
				}
				return string.Empty;
			});
			array2[5] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfFrom, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.FromRecipientsCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FromRecipientsCondition fromRecipientsCondition = (FromRecipientsCondition)condition;
				IList<Participant> participants = fromRecipientsCondition.Participants;
				inboxRule.ExceptIfFrom = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.ExceptIfFrom[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfFrom != null && inboxRule.ExceptIfFrom.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.ExceptIfFrom.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.ExceptIfFrom)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Exceptions.Add(FromRecipientsCondition.Create(rule, list));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfFrom != null && inboxRule.ExceptIfFrom.Length != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.ExceptIfFrom), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFrom(address);
				}
				return string.Empty;
			});
			array2[6] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.FromAddressContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsSenderStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSenderStringCondition containsSenderStringCondition = (ContainsSenderStringCondition)condition;
				inboxRule.FromAddressContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSenderStringCondition.Text);
				InboxRuleDataProvider.ConvertStringListToLowerInvariant(inboxRule.FromAddressContainsWords);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.FromAddressContainsWords))
				{
					rule.Conditions.Add(ContainsSenderStringCondition.Create(rule, inboxRule.FromAddressContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.FromAddressContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.FromAddressContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFromAddressContainsWords(words);
				}
				return string.Empty;
			});
			array2[7] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfFromAddressContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsSenderStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSenderStringCondition containsSenderStringCondition = (ContainsSenderStringCondition)condition;
				inboxRule.ExceptIfFromAddressContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSenderStringCondition.Text);
				InboxRuleDataProvider.ConvertStringListToLowerInvariant(inboxRule.ExceptIfFromAddressContainsWords);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfFromAddressContainsWords))
				{
					rule.Exceptions.Add(ContainsSenderStringCondition.Create(rule, inboxRule.ExceptIfFromAddressContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfFromAddressContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfFromAddressContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFromAddressContainsWords(words);
				}
				return string.Empty;
			});
			array2[8] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.HeaderContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsHeaderStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsHeaderStringCondition containsHeaderStringCondition = (ContainsHeaderStringCondition)condition;
				inboxRule.HeaderContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsHeaderStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.HeaderContainsWords))
				{
					rule.Conditions.Add(ContainsHeaderStringCondition.Create(rule, inboxRule.HeaderContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.HeaderContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.HeaderContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionHeaderContainsWords(words);
				}
				return string.Empty;
			});
			array2[9] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfHeaderContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsHeaderStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsHeaderStringCondition containsHeaderStringCondition = (ContainsHeaderStringCondition)condition;
				inboxRule.ExceptIfHeaderContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsHeaderStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfHeaderContainsWords))
				{
					rule.Exceptions.Add(ContainsHeaderStringCondition.Create(rule, inboxRule.ExceptIfHeaderContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfHeaderContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfHeaderContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionHeaderContainsWords(words);
				}
				return string.Empty;
			});
			array2[10] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.FromSubscription, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Condition, ConditionType.FromSubscriptionCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FromSubscriptionCondition fromSubscriptionCondition = (FromSubscriptionCondition)condition;
				AggregationSubscriptionIdentity[] fromSubscription;
				if (!inboxRuleDataProvider.TryGetSubscriptionIdentities(fromSubscriptionCondition.Guids, out fromSubscription))
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.FromSubscription);
				}
				inboxRule.FromSubscription = fromSubscription;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.FromSubscription != null && inboxRule.FromSubscription.Length > 0)
				{
					Guid[] subscriptionGuids = InboxRuleDataProvider.GetSubscriptionGuids(inboxRule.FromSubscription);
					rule.Conditions.Add(FromSubscriptionCondition.Create(rule, subscriptionGuids));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.FromSubscription != null)
				{
					IList<string> subscriptionEmailAddresses = inboxRule.GetSubscriptionEmailAddresses(inboxRule.FromSubscription);
					if (inboxRule.IsPropertyInError(InboxRuleSchema.FromSubscription))
					{
						subscriptionEmailAddresses.Add(RulesTasksStrings.InboxRuleDescriptionSubscriptionNotFound);
					}
					string subscriptionEmailAddresses2 = RuleDescription.BuildDescriptionStringFromStringArray(subscriptionEmailAddresses, RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFromSubscription(subscriptionEmailAddresses2);
				}
				return string.Empty;
			});
			array2[11] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfFromSubscription, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Exception, ConditionType.FromSubscriptionCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				FromSubscriptionCondition fromSubscriptionCondition = (FromSubscriptionCondition)condition;
				AggregationSubscriptionIdentity[] exceptIfFromSubscription;
				if (!inboxRuleDataProvider.TryGetSubscriptionIdentities(fromSubscriptionCondition.Guids, out exceptIfFromSubscription))
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.ExceptIfFromSubscription);
				}
				inboxRule.ExceptIfFromSubscription = exceptIfFromSubscription;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfFromSubscription != null && inboxRule.ExceptIfFromSubscription.Length > 0)
				{
					Guid[] subscriptionGuids = InboxRuleDataProvider.GetSubscriptionGuids(inboxRule.ExceptIfFromSubscription);
					rule.Exceptions.Add(FromSubscriptionCondition.Create(rule, subscriptionGuids));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfFromSubscription != null)
				{
					IList<string> subscriptionEmailAddresses = inboxRule.GetSubscriptionEmailAddresses(inboxRule.ExceptIfFromSubscription);
					if (inboxRule.IsPropertyInError(InboxRuleSchema.ExceptIfFromSubscription))
					{
						subscriptionEmailAddresses.Add(RulesTasksStrings.InboxRuleDescriptionSubscriptionNotFound);
					}
					string subscriptionEmailAddresses2 = RuleDescription.BuildDescriptionStringFromStringArray(subscriptionEmailAddresses, RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionFromSubscription(subscriptionEmailAddresses2);
				}
				return string.Empty;
			});
			array2[12] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.HasAttachment, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.HasAttachmentCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				HasAttachmentCondition hasAttachmentCondition = (HasAttachmentCondition)condition;
				inboxRule.HasAttachment = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.HasAttachment)
				{
					rule.Conditions.Add(HasAttachmentCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.HasAttachment)
				{
					return RulesTasksStrings.InboxRuleDescriptionHasAttachment;
				}
				return string.Empty;
			});
			array2[13] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfHasAttachment, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.HasAttachmentCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				HasAttachmentCondition hasAttachmentCondition = (HasAttachmentCondition)condition;
				inboxRule.ExceptIfHasAttachment = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfHasAttachment)
				{
					rule.Exceptions.Add(HasAttachmentCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfHasAttachment)
				{
					return RulesTasksStrings.InboxRuleDescriptionHasAttachment;
				}
				return string.Empty;
			});
			array2[14] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.HasClassification, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Condition, ConditionType.MessageClassificationCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MessageClassificationCondition messageClassificationCondition = (MessageClassificationCondition)condition;
				List<MessageClassification> list;
				if (!inboxRuleDataProvider.ResolveClassifications(messageClassificationCondition.Text, out list))
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.HasClassification);
				}
				inboxRule.HasClassification = list.ToArray();
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.HasClassification != null && inboxRule.HasClassification.Length > 0)
				{
					List<string> list = new List<string>(from classification in inboxRule.HasClassification
					select classification.ClassificationID.ToString());
					rule.Conditions.Add(MessageClassificationCondition.Create(rule, list.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				IList<string> list = null;
				if (inboxRule.HasClassification != null && inboxRule.HasClassification.Length > 0)
				{
					list = new List<string>(from classification in inboxRule.HasClassification
					select classification.DisplayName);
				}
				if (inboxRule.IsPropertyInError(InboxRuleSchema.HasClassification))
				{
					if (list == null)
					{
						list = new string[]
						{
							RulesTasksStrings.InboxRuleDescriptionMessageClassificationNotFound
						};
					}
					else
					{
						list.Add(RulesTasksStrings.InboxRuleDescriptionMessageClassificationNotFound);
					}
				}
				if (list != null && list.Count > 0)
				{
					string classifications = RuleDescription.BuildDescriptionStringFromStringArray(list, RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionHasClassification(classifications);
				}
				return string.Empty;
			});
			array2[15] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfHasClassification, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Exception, ConditionType.MessageClassificationCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MessageClassificationCondition messageClassificationCondition = (MessageClassificationCondition)condition;
				List<MessageClassification> list;
				if (!inboxRuleDataProvider.ResolveClassifications(messageClassificationCondition.Text, out list))
				{
					inboxRule.SetPropertyInError(InboxRuleSchema.ExceptIfHasClassification);
				}
				inboxRule.ExceptIfHasClassification = list.ToArray();
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfHasClassification != null && inboxRule.ExceptIfHasClassification.Length > 0)
				{
					List<string> list = new List<string>(from classification in inboxRule.ExceptIfHasClassification
					select classification.ClassificationID.ToString());
					rule.Exceptions.Add(MessageClassificationCondition.Create(rule, list.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				IList<string> list = null;
				if (inboxRule.ExceptIfHasClassification != null && inboxRule.ExceptIfHasClassification.Length > 0)
				{
					list = new List<string>(from classification in inboxRule.ExceptIfHasClassification
					select classification.DisplayName);
				}
				if (inboxRule.IsPropertyInError(InboxRuleSchema.ExceptIfHasClassification))
				{
					if (list == null)
					{
						list = new string[]
						{
							RulesTasksStrings.InboxRuleDescriptionMessageClassificationNotFound
						};
					}
					else
					{
						list.Add(RulesTasksStrings.InboxRuleDescriptionMessageClassificationNotFound);
					}
				}
				if (list != null && list.Count > 0)
				{
					string classifications = RuleDescription.BuildDescriptionStringFromStringArray(list, RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionHasClassification(classifications);
				}
				return string.Empty;
			});
			array2[16] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.MarkedAsOofCondition, InboxRuleMessageType.AutomaticReply, RuleProviderId.OL98Plus);
			array2[17] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.MarkedAsOofCondition, InboxRuleMessageType.AutomaticReply, RuleProviderId.OL98Plus);
			array2[18] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.AutomaticForwardCondition, InboxRuleMessageType.AutomaticForward);
			array2[19] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.AutomaticForwardCondition, InboxRuleMessageType.AutomaticForward);
			array2[20] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.EncryptedCondition, InboxRuleMessageType.Encrypted);
			array2[21] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.EncryptedCondition, InboxRuleMessageType.Encrypted);
			array2[22] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.MeetingMessageCondition, InboxRuleMessageType.Calendaring);
			array2[23] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.MeetingMessageCondition, InboxRuleMessageType.Calendaring);
			array2[24] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.MeetingResponseCondition, InboxRuleMessageType.CalendaringResponse);
			array2[25] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.MeetingResponseCondition, InboxRuleMessageType.CalendaringResponse);
			array2[26] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.PermissionControlledCondition, InboxRuleMessageType.PermissionControlled);
			array2[27] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.PermissionControlledCondition, InboxRuleMessageType.PermissionControlled);
			array2[28] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.VoicemailCondition, InboxRuleMessageType.Voicemail);
			array2[29] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.VoicemailCondition, InboxRuleMessageType.Voicemail);
			array2[30] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.SignedCondition, InboxRuleMessageType.Signed);
			array2[31] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.SignedCondition, InboxRuleMessageType.Signed);
			array2[32] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.ApprovalRequestCondition, InboxRuleMessageType.ApprovalRequest);
			array2[33] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.ApprovalRequestCondition, InboxRuleMessageType.ApprovalRequest);
			array2[34] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.ReadReceiptCondition, InboxRuleMessageType.ReadReceipt);
			array2[35] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.ReadReceiptCondition, InboxRuleMessageType.ReadReceipt);
			array2[36] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Condition, ConditionType.NdrCondition, InboxRuleMessageType.NonDeliveryReport);
			array2[37] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleDataProvider.PredicateType.Exception, ConditionType.NdrCondition, InboxRuleMessageType.NonDeliveryReport);
			array2[38] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.MyNameInCcBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.SentCcMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.MyNameInCcBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MyNameInCcBox)
				{
					rule.Conditions.Add(SentCcMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MyNameInCcBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInCcBox;
				}
				return string.Empty;
			});
			array2[39] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfMyNameInCcBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.SentCcMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.ExceptIfMyNameInCcBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfMyNameInCcBox)
				{
					rule.Exceptions.Add(SentCcMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfMyNameInCcBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInCcBox;
				}
				return string.Empty;
			});
			array2[40] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.MyNameInToBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.SentToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.MyNameInToBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MyNameInToBox)
				{
					rule.Conditions.Add(SentToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MyNameInToBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInToBox;
				}
				return string.Empty;
			});
			array2[41] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfMyNameInToBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.SentToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.ExceptIfMyNameInToBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfMyNameInToBox)
				{
					rule.Exceptions.Add(SentToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfMyNameInToBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInToBox;
				}
				return string.Empty;
			});
			array2[42] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.MyNameInToOrCcBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.SentToOrCcMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.MyNameInToOrCcBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MyNameInToOrCcBox)
				{
					rule.Conditions.Add(SentToOrCcMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MyNameInToOrCcBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInToOrCcBox;
				}
				return string.Empty;
			});
			array2[43] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfMyNameInToOrCcBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.SentToOrCcMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.ExceptIfMyNameInToOrCcBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfMyNameInToOrCcBox)
				{
					rule.Exceptions.Add(SentToOrCcMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfMyNameInToOrCcBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameInToOrCcBox;
				}
				return string.Empty;
			});
			array2[44] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.MyNameNotInToBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.NotSentToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.MyNameNotInToBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.MyNameNotInToBox)
				{
					rule.Conditions.Add(NotSentToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.MyNameNotInToBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameNotInToBox;
				}
				return string.Empty;
			});
			array2[45] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfMyNameNotInToBox, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.NotSentToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				inboxRule.ExceptIfMyNameNotInToBox = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfMyNameNotInToBox)
				{
					rule.Exceptions.Add(NotSentToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfMyNameNotInToBox)
				{
					return RulesTasksStrings.InboxRuleDescriptionMyNameNotInToBox;
				}
				return string.Empty;
			});
			array2[46] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ReceivedAfterDate, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.WithinDateRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinDateRangeCondition withinDateRangeCondition = baseCondition as WithinDateRangeCondition;
				inboxRule.ReceivedAfterDate = withinDateRangeCondition.RangeLow;
				inboxRule.ReceivedBeforeDate = withinDateRangeCondition.RangeHigh;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ReceivedBeforeDate != null || inboxRule.ReceivedAfterDate != null)
				{
					WithinDateRangeCondition item = WithinDateRangeCondition.Create(rule, inboxRule.ReceivedAfterDate, inboxRule.ReceivedBeforeDate);
					rule.Conditions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ReceivedAfterDate == null)
				{
					return string.Empty;
				}
				CultureInfo culture = inboxRule.Culture;
				if (inboxRule.ReceivedBeforeDate != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionReceivedBetweenDates(inboxRule.GetDateString(inboxRule.ReceivedAfterDate.Value), inboxRule.GetDateString(inboxRule.ReceivedBeforeDate.Value));
				}
				return RulesTasksStrings.InboxRuleDescriptionReceivedAfterDate(inboxRule.GetDateString(inboxRule.ReceivedAfterDate.Value));
			});
			array2[47] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfReceivedAfterDate, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.WithinDateRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinDateRangeCondition withinDateRangeCondition = baseCondition as WithinDateRangeCondition;
				inboxRule.ExceptIfReceivedAfterDate = withinDateRangeCondition.RangeLow;
				inboxRule.ExceptIfReceivedBeforeDate = withinDateRangeCondition.RangeHigh;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfReceivedBeforeDate != null || inboxRule.ExceptIfReceivedAfterDate != null)
				{
					WithinDateRangeCondition item = WithinDateRangeCondition.Create(rule, inboxRule.ExceptIfReceivedAfterDate, inboxRule.ExceptIfReceivedBeforeDate);
					rule.Exceptions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfReceivedAfterDate == null)
				{
					return string.Empty;
				}
				CultureInfo culture = inboxRule.Culture;
				if (inboxRule.ExceptIfReceivedBeforeDate != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionReceivedBetweenDates(inboxRule.GetDateString(inboxRule.ExceptIfReceivedAfterDate.Value), inboxRule.GetDateString(inboxRule.ExceptIfReceivedBeforeDate.Value));
				}
				return RulesTasksStrings.InboxRuleDescriptionReceivedAfterDate(inboxRule.GetDateString(inboxRule.ExceptIfReceivedAfterDate.Value));
			});
			array2[48] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ReceivedBeforeDate, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.WithinDateRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinDateRangeCondition withinDateRangeCondition = baseCondition as WithinDateRangeCondition;
				inboxRule.ReceivedAfterDate = withinDateRangeCondition.RangeLow;
				inboxRule.ReceivedBeforeDate = withinDateRangeCondition.RangeHigh;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ReceivedBeforeDate != null || inboxRule.ReceivedAfterDate != null)
				{
					WithinDateRangeCondition item = WithinDateRangeCondition.Create(rule, inboxRule.ReceivedAfterDate, inboxRule.ReceivedBeforeDate);
					rule.Conditions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ReceivedBeforeDate != null && inboxRule.ReceivedAfterDate == null)
				{
					CultureInfo culture = inboxRule.Culture;
					return RulesTasksStrings.InboxRuleDescriptionReceivedBeforeDate(inboxRule.GetDateString(inboxRule.ReceivedBeforeDate.Value));
				}
				return string.Empty;
			});
			array2[49] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfReceivedBeforeDate, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.WithinDateRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinDateRangeCondition withinDateRangeCondition = baseCondition as WithinDateRangeCondition;
				inboxRule.ExceptIfReceivedAfterDate = withinDateRangeCondition.RangeLow;
				inboxRule.ExceptIfReceivedBeforeDate = withinDateRangeCondition.RangeHigh;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfReceivedBeforeDate != null || inboxRule.ExceptIfReceivedAfterDate != null)
				{
					WithinDateRangeCondition item = WithinDateRangeCondition.Create(rule, inboxRule.ExceptIfReceivedAfterDate, inboxRule.ExceptIfReceivedBeforeDate);
					rule.Exceptions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfReceivedBeforeDate != null && inboxRule.ExceptIfReceivedAfterDate == null)
				{
					CultureInfo culture = inboxRule.Culture;
					return RulesTasksStrings.InboxRuleDescriptionReceivedBeforeDate(inboxRule.GetDateString(inboxRule.ExceptIfReceivedBeforeDate.Value));
				}
				return string.Empty;
			});
			array2[50] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.RecipientAddressContainsWords, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsRecipientStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsRecipientStringCondition containsRecipientStringCondition = (ContainsRecipientStringCondition)condition;
				inboxRule.RecipientAddressContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsRecipientStringCondition.Text);
				InboxRuleDataProvider.ConvertStringListToLowerInvariant(inboxRule.RecipientAddressContainsWords);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.RecipientAddressContainsWords))
				{
					rule.Conditions.Add(ContainsRecipientStringCondition.Create(rule, inboxRule.RecipientAddressContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.RecipientAddressContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.RecipientAddressContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionRecipientAddressContainsWords(words);
				}
				return string.Empty;
			});
			array2[51] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfRecipientAddressContainsWords, RuleProviderId.Exchange14, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsRecipientStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsRecipientStringCondition containsRecipientStringCondition = (ContainsRecipientStringCondition)condition;
				inboxRule.ExceptIfRecipientAddressContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsRecipientStringCondition.Text);
				InboxRuleDataProvider.ConvertStringListToLowerInvariant(inboxRule.ExceptIfRecipientAddressContainsWords);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfRecipientAddressContainsWords))
				{
					rule.Exceptions.Add(ContainsRecipientStringCondition.Create(rule, inboxRule.ExceptIfRecipientAddressContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfRecipientAddressContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfRecipientAddressContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionRecipientAddressContainsWords(words);
				}
				return string.Empty;
			});
			array2[52] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.SentOnlyToMe, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.SentOnlyToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				SentOnlyToMeCondition sentOnlyToMeCondition = (SentOnlyToMeCondition)condition;
				inboxRule.SentOnlyToMe = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.SentOnlyToMe)
				{
					rule.Conditions.Add(SentOnlyToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.SentOnlyToMe)
				{
					return RulesTasksStrings.InboxRuleDescriptionSentOnlyToMe;
				}
				return string.Empty;
			});
			array2[53] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfSentOnlyToMe, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.SentOnlyToMeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				SentOnlyToMeCondition sentOnlyToMeCondition = (SentOnlyToMeCondition)condition;
				inboxRule.ExceptIfSentOnlyToMe = true;
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfSentOnlyToMe)
				{
					rule.Exceptions.Add(SentOnlyToMeCondition.Create(rule));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfSentOnlyToMe)
				{
					return RulesTasksStrings.InboxRuleDescriptionSentOnlyToMe;
				}
				return string.Empty;
			});
			array2[54] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.SentTo, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.SentToRecipientsCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				SentToRecipientsCondition sentToRecipientsCondition = (SentToRecipientsCondition)condition;
				IList<Participant> participants = sentToRecipientsCondition.Participants;
				inboxRule.SentTo = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.SentTo[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.SentTo != null && inboxRule.SentTo.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.SentTo.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.SentTo)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Conditions.Add(SentToRecipientsCondition.Create(rule, list));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.SentTo != null && inboxRule.SentTo.Length != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.SentTo), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSentTo(address);
				}
				return string.Empty;
			});
			array2[55] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfSentTo, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.SentToRecipientsCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				SentToRecipientsCondition sentToRecipientsCondition = (SentToRecipientsCondition)condition;
				IList<Participant> participants = sentToRecipientsCondition.Participants;
				inboxRule.ExceptIfSentTo = new ADRecipientOrAddress[participants.Count];
				for (int i = 0; i < participants.Count; i++)
				{
					inboxRule.ExceptIfSentTo[i] = new ADRecipientOrAddress(participants[i]);
				}
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.ExceptIfSentTo != null && inboxRule.ExceptIfSentTo.Length != 0)
				{
					List<Participant> list = new List<Participant>(inboxRule.ExceptIfSentTo.Length);
					foreach (ADRecipientOrAddress adrecipientOrAddress in inboxRule.ExceptIfSentTo)
					{
						list.Add(adrecipientOrAddress.Participant);
					}
					rule.Exceptions.Add(SentToRecipientsCondition.Create(rule, list));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfSentTo != null && inboxRule.ExceptIfSentTo.Length != 0)
				{
					string address = RuleDescription.BuildDescriptionStringFromStringArray(InboxRuleDataProvider.BuildRecipientStringList(inboxRule.ExceptIfSentTo), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSentTo(address);
				}
				return string.Empty;
			});
			array2[56] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.SubjectContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsSubjectStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSubjectStringCondition containsSubjectStringCondition = (ContainsSubjectStringCondition)condition;
				inboxRule.SubjectContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSubjectStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.SubjectContainsWords))
				{
					rule.Conditions.Add(ContainsSubjectStringCondition.Create(rule, inboxRule.SubjectContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.SubjectContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.SubjectContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSubjectContainsWords(words);
				}
				return string.Empty;
			});
			array2[57] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfSubjectContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsSubjectStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSubjectStringCondition containsSubjectStringCondition = (ContainsSubjectStringCondition)condition;
				inboxRule.ExceptIfSubjectContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSubjectStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfSubjectContainsWords))
				{
					rule.Exceptions.Add(ContainsSubjectStringCondition.Create(rule, inboxRule.ExceptIfSubjectContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfSubjectContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfSubjectContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSubjectContainsWords(words);
				}
				return string.Empty;
			});
			array2[58] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.SubjectOrBodyContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.ContainsSubjectOrBodyStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSubjectOrBodyStringCondition containsSubjectOrBodyStringCondition = (ContainsSubjectOrBodyStringCondition)condition;
				inboxRule.SubjectOrBodyContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSubjectOrBodyStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.SubjectOrBodyContainsWords))
				{
					rule.Conditions.Add(ContainsSubjectOrBodyStringCondition.Create(rule, inboxRule.SubjectOrBodyContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.SubjectOrBodyContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.SubjectOrBodyContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSubjectOrBodyContainsWords(words);
				}
				return string.Empty;
			});
			array2[59] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfSubjectOrBodyContainsWords, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.ContainsSubjectOrBodyStringCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				ContainsSubjectOrBodyStringCondition containsSubjectOrBodyStringCondition = (ContainsSubjectOrBodyStringCondition)condition;
				inboxRule.ExceptIfSubjectOrBodyContainsWords = InboxRuleDataProvider.BuildMultiValuedPropertyWithoutDuplicates(containsSubjectOrBodyStringCondition.Text);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfSubjectOrBodyContainsWords))
				{
					rule.Exceptions.Add(ContainsSubjectOrBodyStringCondition.Create(rule, inboxRule.ExceptIfSubjectOrBodyContainsWords.ToArray()));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (!MultiValuedPropertyBase.IsNullOrEmpty(inboxRule.ExceptIfSubjectOrBodyContainsWords))
				{
					string words = RuleDescription.BuildDescriptionStringFromStringArray(inboxRule.ExceptIfSubjectOrBodyContainsWords.ToArray(), RulesTasksStrings.InboxRuleDescriptionOr, 200);
					return RulesTasksStrings.InboxRuleDescriptionSubjectOrBodyContainsWords(words);
				}
				return string.Empty;
			});
			array2[60] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.WithImportance, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.MarkedAsImportanceCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MarkedAsImportanceCondition markedAsImportanceCondition = (MarkedAsImportanceCondition)condition;
				inboxRule.WithImportance = new Microsoft.Exchange.Data.Storage.Importance?(markedAsImportanceCondition.Importance);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.WithImportance) && inboxRule.WithImportance != null)
				{
					rule.Conditions.Add(MarkedAsImportanceCondition.Create(rule, inboxRule.WithImportance.Value));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.WithImportance) && inboxRule.WithImportance != null)
				{
					return InboxRuleDataProvider.GetImportanceDescription(inboxRule.WithImportance.Value);
				}
				return string.Empty;
			});
			array2[61] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfWithImportance, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.MarkedAsImportanceCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MarkedAsImportanceCondition markedAsImportanceCondition = (MarkedAsImportanceCondition)condition;
				inboxRule.ExceptIfWithImportance = new Microsoft.Exchange.Data.Storage.Importance?(markedAsImportanceCondition.Importance);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.ExceptIfWithImportance) && inboxRule.ExceptIfWithImportance != null)
				{
					rule.Exceptions.Add(MarkedAsImportanceCondition.Create(rule, inboxRule.ExceptIfWithImportance.Value));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.ExceptIfWithImportance) && inboxRule.ExceptIfWithImportance != null)
				{
					return InboxRuleDataProvider.GetImportanceDescription(inboxRule.ExceptIfWithImportance.Value);
				}
				return string.Empty;
			});
			array2[62] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.WithinSizeRangeMaximum, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.WithinSizeRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinSizeRangeCondition withinSizeRangeCondition = baseCondition as WithinSizeRangeCondition;
				inboxRule.WithinSizeRangeMinimum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeLow);
				inboxRule.WithinSizeRangeMaximum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeHigh);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if ((inboxRule.WithinSizeRangeMinimum != null && inboxRule.WithinSizeRangeMinimum.Value.ToBytes() > 0UL) || (inboxRule.WithinSizeRangeMaximum != null && inboxRule.WithinSizeRangeMaximum.Value.ToBytes() > 0UL))
				{
					WithinSizeRangeCondition item = WithinSizeRangeCondition.Create(rule, InboxRuleDataProvider.GetSize(inboxRule.WithinSizeRangeMinimum), InboxRuleDataProvider.GetSize(inboxRule.WithinSizeRangeMaximum));
					rule.Conditions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.WithinSizeRangeMaximum == null)
				{
					return string.Empty;
				}
				if (inboxRule.WithinSizeRangeMinimum != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionWithSizeBetween(inboxRule.WithinSizeRangeMinimum.ToString(), inboxRule.WithinSizeRangeMaximum.ToString());
				}
				return RulesTasksStrings.InboxRuleDescriptionWithSizeLessThan(inboxRule.WithinSizeRangeMaximum.ToString());
			});
			array2[63] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfWithinSizeRangeMaximum, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.WithinSizeRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinSizeRangeCondition withinSizeRangeCondition = baseCondition as WithinSizeRangeCondition;
				inboxRule.ExceptIfWithinSizeRangeMinimum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeLow);
				inboxRule.ExceptIfWithinSizeRangeMaximum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeHigh);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if ((inboxRule.ExceptIfWithinSizeRangeMinimum != null && inboxRule.ExceptIfWithinSizeRangeMinimum.Value.ToBytes() > 0UL) || (inboxRule.ExceptIfWithinSizeRangeMaximum != null && inboxRule.ExceptIfWithinSizeRangeMaximum.Value.ToBytes() > 0UL))
				{
					WithinSizeRangeCondition item = WithinSizeRangeCondition.Create(rule, InboxRuleDataProvider.GetSize(inboxRule.ExceptIfWithinSizeRangeMinimum), InboxRuleDataProvider.GetSize(inboxRule.ExceptIfWithinSizeRangeMaximum));
					rule.Exceptions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfWithinSizeRangeMaximum == null)
				{
					return string.Empty;
				}
				if (inboxRule.ExceptIfWithinSizeRangeMinimum != null)
				{
					return RulesTasksStrings.InboxRuleDescriptionWithSizeBetween(inboxRule.ExceptIfWithinSizeRangeMinimum.ToString(), inboxRule.ExceptIfWithinSizeRangeMaximum.ToString());
				}
				return RulesTasksStrings.InboxRuleDescriptionWithSizeLessThan(inboxRule.ExceptIfWithinSizeRangeMaximum.ToString());
			});
			array2[64] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.WithinSizeRangeMinimum, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.WithinSizeRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinSizeRangeCondition withinSizeRangeCondition = baseCondition as WithinSizeRangeCondition;
				inboxRule.WithinSizeRangeMinimum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeLow);
				inboxRule.WithinSizeRangeMaximum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeHigh);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if ((inboxRule.WithinSizeRangeMinimum != null && inboxRule.WithinSizeRangeMinimum.Value.ToBytes() > 0UL) || (inboxRule.WithinSizeRangeMaximum != null && inboxRule.WithinSizeRangeMaximum.Value.ToBytes() > 0UL))
				{
					WithinSizeRangeCondition item = WithinSizeRangeCondition.Create(rule, InboxRuleDataProvider.GetSize(inboxRule.WithinSizeRangeMinimum), InboxRuleDataProvider.GetSize(inboxRule.WithinSizeRangeMaximum));
					rule.Conditions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.WithinSizeRangeMinimum != null && inboxRule.WithinSizeRangeMaximum == null)
				{
					return RulesTasksStrings.InboxRuleDescriptionWithSizeGreaterThan(inboxRule.WithinSizeRangeMinimum.ToString());
				}
				return string.Empty;
			});
			array2[65] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfWithinSizeRangeMinimum, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.WithinSizeRangeCondition, delegate(Microsoft.Exchange.Data.Storage.Condition baseCondition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				WithinSizeRangeCondition withinSizeRangeCondition = baseCondition as WithinSizeRangeCondition;
				inboxRule.ExceptIfWithinSizeRangeMinimum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeLow);
				inboxRule.ExceptIfWithinSizeRangeMaximum = InboxRuleDataProvider.GetSize(withinSizeRangeCondition.RangeHigh);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if ((inboxRule.ExceptIfWithinSizeRangeMinimum != null && inboxRule.ExceptIfWithinSizeRangeMinimum.Value.ToBytes() > 0UL) || (inboxRule.ExceptIfWithinSizeRangeMaximum != null && inboxRule.ExceptIfWithinSizeRangeMaximum.Value.ToBytes() > 0UL))
				{
					WithinSizeRangeCondition item = WithinSizeRangeCondition.Create(rule, InboxRuleDataProvider.GetSize(inboxRule.ExceptIfWithinSizeRangeMinimum), InboxRuleDataProvider.GetSize(inboxRule.ExceptIfWithinSizeRangeMaximum));
					rule.Exceptions.Add(item);
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.ExceptIfWithinSizeRangeMinimum != null && inboxRule.ExceptIfWithinSizeRangeMaximum == null)
				{
					return RulesTasksStrings.InboxRuleDescriptionWithSizeGreaterThan(inboxRule.ExceptIfWithinSizeRangeMinimum.ToString());
				}
				return string.Empty;
			});
			array2[66] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.WithSensitivity, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Condition, ConditionType.MarkedAsSensitivityCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MarkedAsSensitivityCondition markedAsSensitivityCondition = (MarkedAsSensitivityCondition)condition;
				inboxRule.WithSensitivity = new Sensitivity?(markedAsSensitivityCondition.Sensitivity);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.WithSensitivity) && inboxRule.WithSensitivity != null)
				{
					rule.Conditions.Add(MarkedAsSensitivityCondition.Create(rule, inboxRule.WithSensitivity.Value));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.WithSensitivity) && inboxRule.WithSensitivity != null)
				{
					return InboxRuleDataProvider.GetSensitivityDescription(inboxRule.WithSensitivity.Value);
				}
				return string.Empty;
			});
			array2[67] = new InboxRuleDataProvider.ConditionMappingEntry(InboxRuleSchema.ExceptIfWithSensitivity, RuleProviderId.OL98Plus, InboxRuleDataProvider.PredicateType.Exception, ConditionType.MarkedAsSensitivityCondition, delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				MarkedAsSensitivityCondition markedAsSensitivityCondition = (MarkedAsSensitivityCondition)condition;
				inboxRule.ExceptIfWithSensitivity = new Sensitivity?(markedAsSensitivityCondition.Sensitivity);
			}, delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.ExceptIfWithSensitivity) && inboxRule.ExceptIfWithSensitivity != null)
				{
					rule.Exceptions.Add(MarkedAsSensitivityCondition.Create(rule, inboxRule.ExceptIfWithSensitivity.Value));
				}
			}, delegate(InboxRule inboxRule)
			{
				if (inboxRule.propertyBag.Contains(InboxRuleSchema.ExceptIfWithSensitivity) && inboxRule.ExceptIfWithSensitivity != null)
				{
					return InboxRuleDataProvider.GetSensitivityDescription(inboxRule.ExceptIfWithSensitivity.Value);
				}
				return string.Empty;
			});
			InboxRuleDataProvider.ConditionMappings = array2;
		}

		internal const int RuleSequenceNumberOffset = 9;

		internal const int DeleteOutlookDisabledRules = 16;

		internal const string NewEnabledPontsPropertyName = "NewEnabledPonts";

		internal const string DelegatePrefix = "Delegate Rule";

		internal const int FlaggedActionMaxLength = 100;

		private ADUser adMailboxOwner;

		private ExTimeZoneValue descriptionTimeZone;

		private string descriptionTimeFormat = "ddd, dd MMM yyyy HH':'mm' 'tt";

		private bool includeHidden;

		internal static readonly InboxRuleDataProvider.ActionMappingEntry[] ActionMappings;

		internal static readonly InboxRuleDataProvider.ConditionMappingEntry[] ConditionMappings;

		internal delegate void AddPresentationPropertyToRuleObject(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider);

		internal delegate string GetDescriptionString(InboxRule inboxRule);

		internal delegate void AddRuleActionToPresentationObject(ActionBase actionBase, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider);

		internal class ActionMappingEntry
		{
			public ActionMappingEntry(ProviderPropertyDefinition propertyDefinition, RuleProviderId ruleProviderId, ActionType actionType, InboxRuleDataProvider.AddRuleActionToPresentationObject ruleActionToPresentationDelegate, InboxRuleDataProvider.AddPresentationPropertyToRuleObject addPresentationPropertyToRuleRobject, InboxRuleDataProvider.GetDescriptionString descriptionStringDelegate)
			{
				this.PropertyDefinition = propertyDefinition;
				this.RuleProviderId = ruleProviderId;
				this.ActionType = actionType;
				this.RuleActionToPresentationDelegate = ruleActionToPresentationDelegate;
				this.AddPresentationPropertyToRuleObject = addPresentationPropertyToRuleRobject;
				this.DescriptionStringDelegate = descriptionStringDelegate;
			}

			public override string ToString()
			{
				return string.Format("{0}, {1}", this.ActionType, this.PropertyDefinition.Name);
			}

			public readonly ProviderPropertyDefinition PropertyDefinition;

			public readonly RuleProviderId RuleProviderId;

			public readonly ActionType ActionType;

			public readonly InboxRuleDataProvider.AddRuleActionToPresentationObject RuleActionToPresentationDelegate;

			public readonly InboxRuleDataProvider.AddPresentationPropertyToRuleObject AddPresentationPropertyToRuleObject;

			public readonly InboxRuleDataProvider.GetDescriptionString DescriptionStringDelegate;
		}

		internal delegate void AddRuleConditionToPresentationObject(Microsoft.Exchange.Data.Storage.Condition Condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider);

		internal enum PredicateType
		{
			Undefined,
			Condition,
			Exception
		}

		internal class ConditionMappingEntry
		{
			public ConditionMappingEntry(ProviderPropertyDefinition propertyDefinition, RuleProviderId ruleProviderId, InboxRuleDataProvider.PredicateType predicateType, ConditionType conditionType, InboxRuleDataProvider.AddRuleConditionToPresentationObject ruleConditionToPresentationDelegate, InboxRuleDataProvider.AddPresentationPropertyToRuleObject presentationConditionToRuleDelegate, InboxRuleDataProvider.GetDescriptionString descriptionStringDelegate)
			{
				this.PropertyDefinition = propertyDefinition;
				this.RuleProviderId = ruleProviderId;
				this.PredicateType = predicateType;
				this.ConditionType = conditionType;
				this.RuleConditionToPresentationDelegate = ruleConditionToPresentationDelegate;
				this.AddPresentationPropertyToRuleObject = presentationConditionToRuleDelegate;
				this.DescriptionStringDelegate = descriptionStringDelegate;
			}

			public ConditionMappingEntry(InboxRuleDataProvider.PredicateType predicateType, ConditionType conditionType, InboxRuleMessageType messageType) : this(predicateType, conditionType, messageType, RuleProviderId.Exchange14)
			{
			}

			public ConditionMappingEntry(InboxRuleDataProvider.PredicateType predicateType, ConditionType conditionType, InboxRuleMessageType messageType, RuleProviderId ruleProviderId)
			{
				this.RuleProviderId = ruleProviderId;
				this.PredicateType = predicateType;
				this.ConditionType = conditionType;
				this.messageType = new InboxRuleMessageType?(messageType);
				if (this.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
				{
					this.PropertyDefinition = InboxRuleSchema.MessageTypeMatches;
				}
				else
				{
					this.PropertyDefinition = InboxRuleSchema.ExceptIfMessageTypeMatches;
				}
				this.RuleConditionToPresentationDelegate = delegate(Microsoft.Exchange.Data.Storage.Condition condition, InboxRule inboxRule, InboxRuleDataProvider inboxRuleDataProvider)
				{
					InboxRuleMessageType? messageTypeFromCondition = InboxRuleDataProvider.GetMessageTypeFromCondition(condition);
					if (messageTypeFromCondition == null)
					{
						return;
					}
					if (messageTypeFromCondition != this.messageType)
					{
						return;
					}
					if (this.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
					{
						inboxRule.MessageTypeMatches = new InboxRuleMessageType?(this.messageType.Value);
						return;
					}
					inboxRule.ExceptIfMessageTypeMatches = new InboxRuleMessageType?(this.messageType.Value);
				};
				this.AddPresentationPropertyToRuleObject = delegate(InboxRule inboxRule, Rule rule, InboxRuleDataProvider inboxRuleDataProvider)
				{
					Microsoft.Exchange.Data.Storage.Condition conditionFromMessageType = InboxRuleDataProvider.GetConditionFromMessageType(rule, this.messageType.Value);
					if (this.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
					{
						if (inboxRule.MessageTypeMatches == this.messageType.Value)
						{
							rule.Conditions.Add(conditionFromMessageType);
							return;
						}
					}
					else if (inboxRule.ExceptIfMessageTypeMatches == this.messageType.Value)
					{
						rule.Exceptions.Add(conditionFromMessageType);
					}
				};
				this.DescriptionStringDelegate = delegate(InboxRule inboxRule)
				{
					if (this.PredicateType == InboxRuleDataProvider.PredicateType.Condition)
					{
						if (inboxRule.MessageTypeMatches != this.messageType)
						{
							return string.Empty;
						}
						return InboxRuleDataProvider.GetMessageTypeDescription(inboxRule.MessageTypeMatches.Value);
					}
					else
					{
						if (inboxRule.ExceptIfMessageTypeMatches != this.messageType)
						{
							return string.Empty;
						}
						return InboxRuleDataProvider.GetMessageTypeDescription(inboxRule.ExceptIfMessageTypeMatches.Value);
					}
				};
			}

			public override string ToString()
			{
				return string.Format("{0}, {1}, {2}, {3}", new object[]
				{
					this.PredicateType,
					this.ConditionType,
					this.PropertyDefinition.Name,
					(this.messageType != null) ? this.messageType.ToString() : string.Empty
				});
			}

			public readonly ProviderPropertyDefinition PropertyDefinition;

			public readonly RuleProviderId RuleProviderId;

			public readonly InboxRuleDataProvider.PredicateType PredicateType;

			public readonly ConditionType ConditionType;

			public readonly InboxRuleDataProvider.AddRuleConditionToPresentationObject RuleConditionToPresentationDelegate;

			public readonly InboxRuleDataProvider.AddPresentationPropertyToRuleObject AddPresentationPropertyToRuleObject;

			public readonly InboxRuleDataProvider.GetDescriptionString DescriptionStringDelegate;

			private InboxRuleMessageType? messageType;
		}
	}
}
