using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class InboxRules : RuleDataService, IInboxRules, IDataSourceService<InboxRuleFilter, RuleRow, InboxRule, SetInboxRule, NewInboxRule, RemoveInboxRule>, IEditListService<InboxRuleFilter, RuleRow, InboxRule, NewInboxRule, RemoveInboxRule>, IGetListService<InboxRuleFilter, RuleRow>, INewObjectService<RuleRow, NewInboxRule>, IRemoveObjectsService<RemoveInboxRule>, IEditObjectForListService<InboxRule, SetInboxRule, RuleRow>, IGetObjectService<InboxRule>, IGetObjectForListService<RuleRow>
	{
		public InboxRules() : base("InboxRule")
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule@R:Self")]
		public PowerShellResults<RuleRow> GetList(InboxRuleFilter filter, SortOptions sort)
		{
			return base.GetList<RuleRow, InboxRuleFilter>("Get-InboxRule", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-InboxRule?Identity@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, RemoveInboxRule parameters)
		{
			parameters = (parameters ?? new RemoveInboxRule());
			return base.Invoke(new PSCommand().AddCommand("Remove-InboxRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule?Identity@R:Self")]
		public PowerShellResults<InboxRule> GetObject(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-InboxRule");
			pscommand.AddParameter("DescriptionTimeFormat", EcpDateTimeHelper.GetWeekdayDateFormat(true));
			pscommand.AddParameter("DescriptionTimeZone", RbacPrincipal.Current.UserTimeZone);
			return base.GetObject<InboxRule>(pscommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule?Identity@R:Self")]
		public PowerShellResults<RuleRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<RuleRow>("Get-InboxRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-InboxRule?StopProcessingRules@W:Self")]
		public PowerShellResults<InboxRule> GetMailMessage(NewInboxRule properties)
		{
			return base.NewObject<InboxRule, NewInboxRule>("New-InboxRule", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-InboxRule?StopProcessingRules@W:Self")]
		public PowerShellResults<RuleRow> NewObject(NewInboxRule properties)
		{
			properties.FaultIfNull();
			properties = (NewInboxRule)InboxRules.SanitizeIdentityParameter(properties);
			properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			PowerShellResults<RuleRow> powerShellResults = base.NewObject<RuleRow, NewInboxRule>("New-InboxRule", properties);
			powerShellResults.Output = null;
			return powerShellResults;
		}

		private static InboxRuleParameters SanitizeIdentityParameter(InboxRuleParameters properties)
		{
			PeopleIdentity[] array = new PeopleIdentity[0];
			Action<PeopleIdentity> action = delegate(PeopleIdentity peopleIdentity)
			{
				peopleIdentity.IgnoreDisplayNameInIdentity = true;
			};
			Array.ForEach<PeopleIdentity>(properties.From ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.SentTo ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.ForwardTo ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.RedirectTo ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.ForwardAsAttachmentTo ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.ExceptIfFrom ?? array, action);
			Array.ForEach<PeopleIdentity>(properties.ExceptIfSentTo ?? array, action);
			return properties;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule?Identity@R:Self+Set-InboxRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> SetObject(Identity identity, SetInboxRule properties)
		{
			properties.FaultIfNull();
			properties = (SetInboxRule)InboxRules.SanitizeIdentityParameter(properties);
			if (properties.Name != null)
			{
				properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			}
			return base.SetObject<InboxRule, SetInboxRule, RuleRow>("Set-InboxRule", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-InboxRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> DisableRule(Identity[] identities, DisableInboxRule parameters)
		{
			parameters = (parameters ?? new DisableInboxRule());
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Disable-InboxRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-InboxRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> EnableRule(Identity[] identities, EnableInboxRule parameters)
		{
			parameters = (parameters ?? new EnableInboxRule());
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Enable-InboxRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule?Identity@R:Self+Set-InboxRule?Identity&Priority@W:Self")]
		public PowerShellResults IncreasePriority(Identity[] identities, ChangeInboxRule parameters)
		{
			parameters = (parameters ?? new ChangeInboxRule());
			return base.ChangePriority<InboxRule>(identities, -1, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-InboxRule?Identity@R:Self+Set-InboxRule?Identity&Priority@W:Self")]
		public PowerShellResults DecreasePriority(Identity[] identities, ChangeInboxRule parameters)
		{
			parameters = (parameters ?? new ChangeInboxRule());
			return base.ChangePriority<InboxRule>(identities, 1, parameters);
		}

		public override int RuleNameMaxLength
		{
			get
			{
				return InboxRules.ruleNameMaxLength;
			}
		}

		public override RulePhrase[] SupportedConditions
		{
			get
			{
				return InboxRules.supportedConditions;
			}
		}

		public override RulePhrase[] SupportedActions
		{
			get
			{
				return InboxRules.supportedActions;
			}
		}

		public override RulePhrase[] SupportedExceptions
		{
			get
			{
				return InboxRules.supportedExceptions;
			}
		}

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string NewInboxRule = "New-InboxRule";

		internal const string GetInboxRule = "Get-InboxRule";

		internal const string SetInboxRule = "Set-InboxRule";

		internal const string RemoveInboxRule = "Remove-InboxRule";

		internal const string DisableInboxRule = "Disable-InboxRule";

		internal const string EnableInboxRule = "Enable-InboxRule";

		private const string PeoplePickerRole = "Get-Recipient";

		internal const string AlwaysDeleteOutlookRulesBlob = "AlwaysDeleteOutlookRulesBlob";

		internal const string GetListRole = "Get-InboxRule@R:Self";

		internal const string RemoveObjectsRole = "Remove-InboxRule?Identity@W:Self";

		internal const string GetObjectRole = "Get-InboxRule?Identity@R:Self";

		internal const string NewObjectRole = "New-InboxRule?StopProcessingRules@W:Self";

		internal const string SetObjectRole = "Get-InboxRule?Identity@R:Self+Set-InboxRule?Identity@W:Self";

		internal const string DisableRuleRole = "Disable-InboxRule?Identity@W:Self";

		internal const string EnableRuleRole = "Enable-InboxRule?Identity@W:Self";

		internal const string ChangePriorityRole = "Get-InboxRule?Identity@R:Self+Set-InboxRule?Identity&Priority@W:Self";

		private static int ruleNameMaxLength = Util.GetMaxLengthFromDefinition(InboxRuleSchema.Name);

		private static RulePhrase[] supportedConditions = new RulePhrase[]
		{
			new RuleCondition("From", OwaOptionStrings.InboxRuleFromConditionText, new FormletParameter[]
			{
				new PeopleParameter("From", PickerType.PickFrom)
			}, "Get-Recipient", OwaOptionStrings.FromConditionFormat, OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleFromConditionFlyOutText, OwaOptionStrings.InboxRuleFromConditionPreCannedText, true),
			new RuleCondition("SentTo", OwaOptionStrings.InboxRuleSentToConditionText, new FormletParameter[]
			{
				new PeopleParameter("SentTo", PickerType.PickTo)
			}, "Get-Recipient", OwaOptionStrings.SentToConditionFormat, OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleSentToConditionFlyOutText, OwaOptionStrings.InboxRuleSentToConditionPreCannedText, true),
			new RuleCondition("FromSubscription", OwaOptionStrings.InboxRuleFromSubscriptionConditionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("FromSubscription", OwaOptionStrings.SubscriptionDialogTitle, OwaOptionStrings.SubscriptionDialogLabel, "RulesEditor/SubscriptionItems.svc", OwaOptionStrings.NoSubscriptionAvailable, null)
			}, "MultiTenant+Get-Subscription@R:Self", OwaOptionStrings.FromSubscriptionConditionFormat, OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleFromSubscriptionConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SubjectContains", OwaOptionStrings.InboxRuleSubjectContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.SubjectContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleSubjectContainsConditionFlyOutText, OwaOptionStrings.InboxRuleSubjectContainsConditionPreCannedText, true),
			new RuleCondition("SubjectOrBodyContains", OwaOptionStrings.InboxRuleSubjectOrBodyContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectOrBodyContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.SubjectOrBodyContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleSubjectOrBodyContainsConditionFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("FromAddressContains", OwaOptionStrings.InboxRuleFromAddressContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("FromAddressContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.FromAddressContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleFromAddressContainsConditionFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("MyNameInToOrCcBox", OwaOptionStrings.InboxRuleMyNameInToCcBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("MyNameInToOrCcBox")
			}, null, OwaOptionStrings.InboxRuleMyNameInToCcBoxConditionText, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInToCcBoxConditionFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("SentOnlyToMe", OwaOptionStrings.InboxRuleSentOnlyToMeConditionText, new FormletParameter[]
			{
				new BooleanParameter("SentOnlyToMe")
			}, null, OwaOptionStrings.SentOnlyToMeConditionFormat, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleSentOnlyToMeConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MyNameInToBox", OwaOptionStrings.InboxRuleMyNameInToBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("MyNameInToBox")
			}, null, OwaOptionStrings.InboxRuleMyNameInToBoxConditionText, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInToBoxConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MyNameInCcBox", OwaOptionStrings.InboxRuleMyNameInCcBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("MyNameInCcBox")
			}, null, OwaOptionStrings.InboxRuleMyNameInCcBoxConditionText, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInCcBoxConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MyNameNotInToBox", OwaOptionStrings.InboxRuleMyNameNotInToBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("MyNameNotInToBox")
			}, null, OwaOptionStrings.InboxRuleMyNameNotInToBoxConditionText, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameNotInToBoxConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("BodyContains", OwaOptionStrings.InboxRuleBodyContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("BodyContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.BodyContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleBodyContainsConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientAddressContains", OwaOptionStrings.InboxRuleRecipientAddressContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("RecipientAddressContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.RecipientAddressContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleRecipientAddressContainsConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HeaderContains", OwaOptionStrings.InboxRuleHeaderContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("HeaderContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.HeaderContainsConditionFormat, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleHeaderContainsConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("WithImportance", OwaOptionStrings.InboxRuleWithImportanceConditionText, new FormletParameter[]
			{
				new EnumParameter("WithImportance", OwaOptionStrings.ImportanceDialogTitle, OwaOptionStrings.ImportanceDialogLabel, typeof(Importance), null)
			}, null, OwaOptionStrings.WithImportanceConditionFormat, OwaOptionStrings.InboxRuleMarkedWithGroupText, OwaOptionStrings.InboxRuleWithImportanceConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("WithSensitivity", OwaOptionStrings.InboxRuleWithSensitivityConditionText, new FormletParameter[]
			{
				new EnumParameter("WithSensitivity", OwaOptionStrings.SensitivityDialogTitle, OwaOptionStrings.SensitivityDialogLabel, typeof(Sensitivity), null)
			}, null, OwaOptionStrings.WithSensitivityConditionFormat, OwaOptionStrings.InboxRuleMarkedWithGroupText, OwaOptionStrings.InboxRuleWithSensitivityConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HasAttachment", OwaOptionStrings.InboxRuleHasAttachmentConditionText, new FormletParameter[]
			{
				new BooleanParameter("HasAttachment")
			}, null, OwaOptionStrings.HasAttachmentConditionFormat, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleHasAttachmentConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MessageTypeMatches", OwaOptionStrings.InboxRuleMessageTypeMatchesConditionText, new FormletParameter[]
			{
				new EnumParameter("MessageTypeMatches", OwaOptionStrings.MessageTypeDialogTitle, OwaOptionStrings.MessageTypeDialogLabel, typeof(InboxRuleMessageType), null)
			}, null, OwaOptionStrings.MessageTypeMatchesConditionFormat, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleMessageTypeMatchesConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HasClassification", OwaOptionStrings.InboxRuleHasClassificationConditionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("HasClassification", OwaOptionStrings.ClassificationDialogTitle, OwaOptionStrings.ClassificationDialogLabel, "RulesEditor/MessageClassifications.svc", OwaOptionStrings.NoMessageClassificationAvailable, null)
			}, "Get-MessageClassification@R:Self", OwaOptionStrings.HasClassificationConditionFormat, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleHasClassificationConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("FlaggedForAction", OwaOptionStrings.InboxRuleFlaggedForActionConditionText, new FormletParameter[]
			{
				new EnumParameter("FlaggedForAction", OwaOptionStrings.FlagStatusDialogTitle, OwaOptionStrings.FlagStatusDialogLabel, typeof(RequestedAction), null, true)
			}, null, OwaOptionStrings.FlaggedForActionConditionFormat, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleFlaggedForActionConditionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("WithinSizeRange", OwaOptionStrings.InboxRuleWithinSizeRangeConditionText, new FormletParameter[]
			{
				new NumberRangeParameter("WithinSizeRange", new string[]
				{
					"WithinSizeRangeMaximum",
					"WithinSizeRangeMinimum"
				}, OwaOptionStrings.WithinSizeRangeDialogTitle, OwaOptionStrings.AtMostOnlyDisplayTemplate, OwaOptionStrings.AtLeastAtMostDisplayTemplate)
			}, null, OwaOptionStrings.WithinSizeRangeConditionFormat, false),
			new RuleCondition("WithinDateRange", OwaOptionStrings.InboxRuleWithinDateRangeConditionText, new FormletParameter[]
			{
				new DateRangeParameter("WithinDateRange", new string[]
				{
					"ReceivedBeforeDate",
					"ReceivedAfterDate"
				}, OwaOptionStrings.WithinDateRangeDialogTitle, OwaOptionStrings.BeforeDateDisplayTemplate, OwaOptionStrings.AfterDateDisplayTemplate)
			}, null, OwaOptionStrings.WithinDateRangeConditionFormat, false)
		};

		private static RulePhrase[] supportedActions = new RulePhrase[]
		{
			new RulePhrase("MoveToFolder", OwaOptionStrings.InboxRuleMoveToFolderActionText, new FormletParameter[]
			{
				new FolderParameter("MoveToFolder", OwaOptionStrings.MailboxFolderDialogTitle, OwaOptionStrings.MailboxFolderDialogLabel)
			}, "Get-MailboxFolder?Recurse&MailFolderOnly&ResultSize@R:Self", OwaOptionStrings.InboxRuleMoveCopyDeleteGroupText, OwaOptionStrings.InboxRuleMoveToFolderActionFlyOutText, true, true),
			new RulePhrase("ApplyCategory", OwaOptionStrings.InboxRuleApplyCategoryActionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ApplyCategory", OwaOptionStrings.CategoryDialogTitle, OwaOptionStrings.CategoryDialogLabel, "RulesEditor/MessageCategories.svc", OwaOptionStrings.NoMessageCategoryAvailable, null)
			}, null, OwaOptionStrings.InboxRuleMarkMessageGroupText, OwaOptionStrings.InboxRuleApplyCategoryActionFlyOutText, true, true),
			new RulePhrase("RedirectTo", OwaOptionStrings.InboxRuleRedirectToActionText, new FormletParameter[]
			{
				new PeopleParameter("RedirectTo", PickerType.PickTo)
				{
					UseAndDelimiter = true
				}
			}, "Get-Recipient", OwaOptionStrings.InboxRuleForwardRedirectGroupText, OwaOptionStrings.InboxRuleRedirectToActionFlyOutText, true, true),
			new RulePhrase("DeleteMessage", OwaOptionStrings.InboxRuleDeleteMessageActionText, new FormletParameter[]
			{
				new BooleanParameter("DeleteMessage")
			}, null, OwaOptionStrings.InboxRuleMoveCopyDeleteGroupText, OwaOptionStrings.InboxRuleDeleteMessageActionFlyOutText, true, true),
			new RulePhrase("SendTextMessageNotificationTo", OwaOptionStrings.InboxRuleSendTextMessageNotificationToActionText, new FormletParameter[]
			{
				new NotificationPhoneNumberParameter("SendTextMessageNotificationTo")
			}, "MachineToPersonTextingOnly", OwaOptionStrings.InboxRuleForwardRedirectGroupText, OwaOptionStrings.InboxRuleSendTextMessageNotificationToActionFlyOutText, true, false),
			new RulePhrase("CopyToFolder", OwaOptionStrings.InboxRuleCopyToFolderActionText, new FormletParameter[]
			{
				new FolderParameter("CopyToFolder", OwaOptionStrings.MailboxFolderDialogTitle, OwaOptionStrings.MailboxFolderDialogLabel)
			}, "Get-MailboxFolder?Recurse&MailFolderOnly&ResultSize@R:Self", OwaOptionStrings.InboxRuleMoveCopyDeleteGroupText, OwaOptionStrings.InboxRuleCopyToFolderActionFlyOutText, false, true),
			new RulePhrase("MarkAsRead", OwaOptionStrings.InboxRuleMarkAsReadActionText, new FormletParameter[]
			{
				new BooleanParameter("MarkAsRead")
			}, null, OwaOptionStrings.InboxRuleMarkMessageGroupText, OwaOptionStrings.InboxRuleMarkAsReadActionFlyOutText, false, true),
			new RulePhrase("ForwardTo", OwaOptionStrings.InboxRuleForwardToActionText, new FormletParameter[]
			{
				new PeopleParameter("ForwardTo", PickerType.PickTo)
				{
					UseAndDelimiter = true
				}
			}, "Get-Recipient", OwaOptionStrings.InboxRuleForwardRedirectGroupText, OwaOptionStrings.InboxRuleForwardToActionFlyOutText, false, true),
			new RulePhrase("ForwardAsAttachmentTo", OwaOptionStrings.InboxRuleForwardAsAttachmentToActionText, new FormletParameter[]
			{
				new PeopleParameter("ForwardAsAttachmentTo", PickerType.PickTo)
				{
					UseAndDelimiter = true
				}
			}, "Get-Recipient", OwaOptionStrings.InboxRuleForwardRedirectGroupText, OwaOptionStrings.InboxRuleForwardAsAttachmentToActionFlyOutText, false, true),
			new RulePhrase("MarkImportance", OwaOptionStrings.InboxRuleMarkImportanceActionText, new FormletParameter[]
			{
				new EnumParameter("MarkImportance", OwaOptionStrings.ImportanceDialogTitle, OwaOptionStrings.ImportanceDialogLabel, typeof(Importance), null)
			}, null, OwaOptionStrings.InboxRuleMarkMessageGroupText, OwaOptionStrings.InboxRuleMarkImportanceActionFlyOutText, false, true)
		};

		private static RulePhrase[] supportedExceptions = new RulePhrase[]
		{
			new RulePhrase("ExceptIfFrom", OwaOptionStrings.InboxRuleFromConditionText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfFrom", PickerType.PickFrom)
			}, "Get-Recipient", OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleFromConditionFlyOutText, false),
			new RulePhrase("ExceptIfSentTo", OwaOptionStrings.InboxRuleSentToConditionText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfSentTo", PickerType.PickTo)
			}, "Get-Recipient", OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleSentToConditionFlyOutText, false),
			new RulePhrase("ExceptIfFromSubscription", OwaOptionStrings.InboxRuleFromSubscriptionConditionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ExceptIfFromSubscription", OwaOptionStrings.SubscriptionDialogTitle, OwaOptionStrings.SubscriptionDialogLabel, "RulesEditor/SubscriptionItems.svc", OwaOptionStrings.NoSubscriptionAvailable, null)
			}, "MultiTenant+Get-Subscription@R:Self", OwaOptionStrings.InboxRuleSentOrReceivedGroupText, OwaOptionStrings.InboxRuleFromSubscriptionConditionFlyOutText, false),
			new RulePhrase("ExceptIfSubjectContains", OwaOptionStrings.InboxRuleSubjectContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleSubjectContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfSubjectOrBodyContains", OwaOptionStrings.InboxRuleSubjectOrBodyContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectOrBodyContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleSubjectOrBodyContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfFromAddressContains", OwaOptionStrings.InboxRuleFromAddressContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfFromAddressContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleFromAddressContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfMyNameInToOrCcBox", OwaOptionStrings.InboxRuleMyNameInToCcBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfMyNameInToOrCcBox")
			}, null, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInToCcBoxConditionFlyOutText, false),
			new RulePhrase("ExceptIfSentOnlyToMe", OwaOptionStrings.InboxRuleSentOnlyToMeConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfSentOnlyToMe")
			}, null, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleSentOnlyToMeConditionFlyOutText, false),
			new RulePhrase("ExceptIfMyNameInToBox", OwaOptionStrings.InboxRuleMyNameInToBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfMyNameInToBox")
			}, null, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInToBoxConditionFlyOutText, false),
			new RulePhrase("ExceptIfMyNameInCcBox", OwaOptionStrings.InboxRuleMyNameInCcBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfMyNameInCcBox")
			}, null, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameInCcBoxConditionFlyOutText, false),
			new RulePhrase("ExceptIfMyNameNotInToBox", OwaOptionStrings.InboxRuleMyNameNotInToBoxConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfMyNameNotInToBox")
			}, null, OwaOptionStrings.InboxRuleMyNameIsGroupText, OwaOptionStrings.InboxRuleMyNameNotInToBoxConditionFlyOutText, false),
			new RulePhrase("ExceptIfBodyContains", OwaOptionStrings.InboxRuleBodyContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfBodyContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleBodyContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfRecipientAddressContains", OwaOptionStrings.InboxRuleRecipientAddressContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfRecipientAddressContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleRecipientAddressContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfHeaderContains", OwaOptionStrings.InboxRuleHeaderContainsConditionText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfHeaderContainsWords", OwaOptionStrings.StringArrayDialogTitle, OwaOptionStrings.StringArrayDialogLabel)
			}, null, OwaOptionStrings.InboxRuleIncludeTheseWordsGroupText, OwaOptionStrings.InboxRuleHeaderContainsConditionFlyOutText, false),
			new RulePhrase("ExceptIfWithImportance", OwaOptionStrings.InboxRuleWithImportanceConditionText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfWithImportance", OwaOptionStrings.ImportanceDialogTitle, OwaOptionStrings.ImportanceDialogLabel, typeof(Importance), null)
			}, null, OwaOptionStrings.InboxRuleMarkedWithGroupText, OwaOptionStrings.InboxRuleWithImportanceConditionFlyOutText, false),
			new RulePhrase("ExceptIfWithSensitivity", OwaOptionStrings.InboxRuleWithSensitivityConditionText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfWithSensitivity", OwaOptionStrings.SensitivityDialogTitle, OwaOptionStrings.SensitivityDialogLabel, typeof(Sensitivity), null)
			}, null, OwaOptionStrings.InboxRuleMarkedWithGroupText, OwaOptionStrings.InboxRuleWithSensitivityConditionFlyOutText, false),
			new RulePhrase("ExceptIfHasAttachment", OwaOptionStrings.InboxRuleHasAttachmentConditionText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfHasAttachment")
			}, null, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleHasAttachmentConditionFlyOutText, false),
			new RulePhrase("ExceptIfMessageTypeMatches", OwaOptionStrings.InboxRuleMessageTypeMatchesConditionText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfMessageTypeMatches", OwaOptionStrings.MessageTypeDialogTitle, OwaOptionStrings.MessageTypeDialogLabel, typeof(InboxRuleMessageType), null)
			}, null, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleMessageTypeMatchesConditionFlyOutText, false),
			new RulePhrase("ExceptIfHasClassification", OwaOptionStrings.InboxRuleHasClassificationConditionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ExceptIfHasClassification", OwaOptionStrings.ClassificationDialogTitle, OwaOptionStrings.ClassificationDialogLabel, "RulesEditor/MessageClassifications.svc", OwaOptionStrings.NoMessageClassificationAvailable, null)
			}, "Get-MessageClassification@R:Self", OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleHasClassificationConditionFlyOutText, false),
			new RulePhrase("ExceptIfFlaggedForAction", OwaOptionStrings.InboxRuleFlaggedForActionConditionText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfFlaggedForAction", OwaOptionStrings.FlagStatusDialogTitle, OwaOptionStrings.FlagStatusDialogLabel, typeof(RequestedAction), null, true)
			}, null, OwaOptionStrings.InboxRuleItIsGroupText, OwaOptionStrings.InboxRuleFlaggedForActionConditionFlyOutText, false),
			new RulePhrase("ExceptIfWithinSizeRange", OwaOptionStrings.InboxRuleWithinSizeRangeConditionText, new FormletParameter[]
			{
				new NumberRangeParameter("ExceptIfWithinSizeRange", new string[]
				{
					"WithinSizeRangeMaximum",
					"WithinSizeRangeMinimum"
				}, OwaOptionStrings.WithinSizeRangeDialogTitle, OwaOptionStrings.AtMostOnlyDisplayTemplate, OwaOptionStrings.AtLeastAtMostDisplayTemplate)
			}, null, false),
			new RulePhrase("ExceptIfWithinDateRange", OwaOptionStrings.InboxRuleWithinDateRangeConditionText, new FormletParameter[]
			{
				new DateRangeParameter("ExceptIfWithinDateRange", new string[]
				{
					"ReceivedBeforeDate",
					"ReceivedAfterDate"
				}, OwaOptionStrings.WithinDateRangeDialogTitle, OwaOptionStrings.BeforeDateDisplayTemplate, OwaOptionStrings.AfterDateDisplayTemplate)
			}, null, false)
		};
	}
}
