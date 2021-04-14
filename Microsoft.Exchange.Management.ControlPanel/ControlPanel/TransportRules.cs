using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class TransportRules : RuleDataService, ITransportRules, IDataSourceService<TransportRuleFilter, RuleRow, TransportRule, SetTransportRule, NewTransportRule>, IDataSourceService<TransportRuleFilter, RuleRow, TransportRule, SetTransportRule, NewTransportRule, BaseWebServiceParameters>, IEditListService<TransportRuleFilter, RuleRow, TransportRule, NewTransportRule, BaseWebServiceParameters>, IGetListService<TransportRuleFilter, RuleRow>, INewObjectService<RuleRow, NewTransportRule>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<TransportRule, SetTransportRule, RuleRow>, IGetObjectService<TransportRule>, IGetObjectForListService<RuleRow>
	{
		public TransportRules() : base("TransportRule")
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?ResultSize&Filter@R:Organization")]
		public PowerShellResults<RuleRow> GetList(TransportRuleFilter filter, SortOptions sort)
		{
			return base.GetList<RuleRow, TransportRuleFilter>("Get-TransportRule", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-TransportRule?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-TransportRule", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?Identity@R:Organization")]
		public PowerShellResults<TransportRule> GetObject(Identity identity)
		{
			return base.GetObject<TransportRule>("Get-TransportRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?Identity@R:Organization")]
		public PowerShellResults<RuleRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<RuleRow>("Get-TransportRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-TransportRule@W:Organization")]
		public PowerShellResults<RuleRow> NewObject(NewTransportRule properties)
		{
			properties.FaultIfNull();
			properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			return base.NewObject<RuleRow, NewTransportRule>("New-TransportRule", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule@R:Organization")]
		public PowerShellResults<TransportRule> GetObjectForNew(Identity identity)
		{
			if (identity != null)
			{
				PowerShellResults<TransportRule> @object = base.GetObject<TransportRule>("Get-TransportRule", identity);
				@object.Output[0].UpdateName(Strings.CopyOf(@object.Output[0].Name));
				return @object;
			}
			return null;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?Identity@R:Organization+Set-TransportRule?Identity@W:Organization")]
		public PowerShellResults<RuleRow> SetObject(Identity identity, SetTransportRule properties)
		{
			properties.FaultIfNull();
			if (properties.Name != null)
			{
				properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			}
			return base.SetObject<TransportRule, SetTransportRule, RuleRow>("Set-TransportRule", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-TransportRule?Identity@W:Organization")]
		public PowerShellResults<RuleRow> DisableRule(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Disable-TransportRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-TransportRule?Identity@W:Organization")]
		public PowerShellResults<RuleRow> EnableRule(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Enable-TransportRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?Identity@R:Organization+Set-TransportRule?Identity&Priority@W:Organization")]
		public PowerShellResults IncreasePriority(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.ChangePriority<TransportRule>(identities, -1, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TransportRule?Identity@R:Organization+Set-TransportRule?Identity&Priority@W:Organization")]
		public PowerShellResults DecreasePriority(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.ChangePriority<TransportRule>(identities, 1, parameters);
		}

		public override int RuleNameMaxLength
		{
			get
			{
				return TransportRules.ruleNameMaxLength;
			}
		}

		public override RulePhrase[] SupportedConditions
		{
			get
			{
				return this.supportedConditions;
			}
		}

		public override RulePhrase[] SupportedActions
		{
			get
			{
				return this.supportedActions;
			}
		}

		private static RulePhrase GetConnectorRulePhrase()
		{
			if (RbacPrincipal.Current.IsInRole("Enterprise"))
			{
				return new RulePhrase("Connector", Strings.TransportRuleRouteMessageSendConnectorActionText, new FormletParameter[]
				{
					new EnhancedEnumParameter("RouteMessageOutboundConnector", Strings.SendConnectorDialogTitle, Strings.SendConnectorDialogLabel, "DDI/DDIService.svc?schema=" + "ConnectorSendETR", Strings.NoOutboundConnectorsAvailable, null)
				}, null, Strings.TransportRuleRedirectTheMessageGroupText, Strings.TransportRuleSendConnectorFlyOutText, false);
			}
			return new RulePhrase("Connector", Strings.TransportRuleRouteMessageOutboundConnectorActionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("RouteMessageOutboundConnector", Strings.OutboundConnectorDialogTitle, Strings.OutboundConnectorDialogLabel, "DDI/DDIService.svc?schema=" + "ConnectorOutboundETR", Strings.NoOutboundConnectorsAvailable, null)
			}, null, Strings.TransportRuleRedirectTheMessageGroupText, Strings.TransportRuleOutboundConnectorFlyOutText, false);
		}

		public override RulePhrase[] SupportedExceptions
		{
			get
			{
				return this.supportedExceptions;
			}
		}

		internal const string NOT_FFO_ROLE = "!EOPStandard";

		internal const string AND_NOT_FFO_ROLE = "+!EOPStandard";

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		internal const string NewTransportRule = "New-TransportRule";

		internal const string GetTransportRule = "Get-TransportRule";

		internal const string SetTransportRule = "Set-TransportRule";

		internal const string RemoveTransportRule = "Remove-TransportRule";

		internal const string DisableTransportRule = "Disable-TransportRule";

		internal const string EnableTransportRule = "Enable-TransportRule";

		internal const string GetListRole = "Get-TransportRule?ResultSize&Filter@R:Organization";

		internal const string RemoveObjectsRole = "Remove-TransportRule?Identity@W:Organization";

		internal const string GetObjectRole = "Get-TransportRule?Identity@R:Organization";

		internal const string NewObjectRole = "New-TransportRule@W:Organization";

		internal const string GetObjectForNewRole = "Get-TransportRule@R:Organization";

		internal const string SetObjectRole = "Get-TransportRule?Identity@R:Organization+Set-TransportRule?Identity@W:Organization";

		internal const string DisableRuleRole = "Disable-TransportRule?Identity@W:Organization";

		internal const string EnableRuleRole = "Enable-TransportRule?Identity@W:Organization";

		internal const string ChangePriorityRole = "Get-TransportRule?Identity@R:Organization+Set-TransportRule?Identity&Priority@W:Organization";

		private static int ruleNameMaxLength = Util.GetMaxLengthFromDefinition(ADObjectSchema.Name);

		private RulePhrase[] supportedConditions = new RulePhrase[]
		{
			new RuleCondition("From", Strings.TransportRuleFromPredicateText, new FormletParameter[]
			{
				new PeopleParameter("From", PickerType.PickFrom)
			}, null, Strings.FromConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("SentTo", Strings.TransportRuleSentToPredicateText, new FormletParameter[]
			{
				new PeopleParameter("SentTo", PickerType.PickTo)
			}, null, Strings.SentToConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("FromScope", Strings.TransportRuleFromScopePredicateText, new FormletParameter[]
			{
				new EnumParameter("FromScope", Strings.FromScopeDialogTitle, Strings.FromScopeDialogLabel, typeof(FromUserScope), null)
			}, null, Strings.FromScopeConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromScopeFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("SentToScope", Strings.TransportRuleSentToScopePredicateText, new FormletParameter[]
			{
				new EnumParameter("SentToScope", Strings.ToScopeDialogTitle, Strings.ToScopeDialogLabel, typeof(ToUserScope), null)
			}, null, Strings.SentToScopeConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToScopeFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("FromMemberOf", Strings.TransportRuleFromMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("FromMemberOf", PickerType.PickFrom)
			}, null, Strings.FromMemberOfConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromMemberOfFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("SentToMemberOf", Strings.TransportRuleSentToMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("SentToMemberOf", PickerType.PickTo)
			}, null, Strings.SentToMemberOfConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToMemberOfFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("SubjectOrBodyContains", Strings.TransportRuleSubjectOrBodyContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectOrBodyContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.SubjectOrBodyContainsConditionFormat, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectOrBodyContainsFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("FromAddressContains", Strings.TransportRuleFromAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("FromAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.FromAddressContainsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromAddressContainsFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("RecipientAddressContains", Strings.TransportRuleRecipientAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("RecipientAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.RecipientAddressContainsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientAddressContainsFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("AttachmentContainsWords", Strings.TransportRuleAttachmentContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AttachmentContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.AttachmentContainsWordsConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentContainsWordsFlyOutText, LocalizedString.Empty, true),
			new RuleCondition("AttachmentMatchesPatterns", Strings.TransportRuleAttachmentMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AttachmentMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.AttachmentMatchesPatternsConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentIsUnsupported", Strings.TransportRuleAttachmentIsUnsupportedPredicateText, new FormletParameter[]
			{
				new BooleanParameter("AttachmentIsUnsupported")
			}, null, Strings.AttachmentIsUnsupportedConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentIsUnsupportedFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SubjectOrBodyMatchesPatterns", Strings.TransportRuleSubjectOrBodyMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectOrBodyMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.SubjectOrBodyMatchesPatternsConditionFormat, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectOrBodyMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("FromAddressMatchesPatterns", Strings.TransportRuleFromAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("FromAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.FromAddressMatchesPatternsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromAddressMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientAddressMatchesPatterns", Strings.TransportRuleRecipientAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("RecipientAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.RecipientAddressMatchesPatternsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientAddressMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfRecipientAddressContainsWords", Strings.TransportRuleAnyRecipientAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AnyOfRecipientAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.RecipientAddressContainsConditionFormat, Strings.TransportRuleAnyRecipientGroupText, Strings.TransportRuleFromAddressContainsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfRecipientAddressMatchesPatterns", Strings.TransportRuleAnyRecipientAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AnyOfRecipientAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.RecipientAddressMatchesPatternsConditionFormat, Strings.TransportRuleAnyRecipientGroupText, Strings.TransportRuleRecipientAddressMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentNameMatchesPatterns", Strings.TransportRuleAttachmentNameMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AttachmentNameMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.AttachmentNameMatchesPatternsConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentNameMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MessageContainsDataClassifications", Strings.TransportRuleContainsSensitiveInformationPredicateText, new FormletParameter[]
			{
				new DLPParameter("MessageContainsDataClassifications", Strings.DLPickerDialogTitle, Strings.DLPickerDialogLabel, Strings.TransportRuleContainsSensitiveInformationParameterNoSelectionText)
			}, "!EOPStandard", Strings.ContainsSensitiveInformationConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleContainsSensitiveInformationFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderManagementRelationship", Strings.TransportRuleSenderManagementRelationshipPredicateText, new FormletParameter[]
			{
				new EnumParameter("SenderManagementRelationship", Strings.SenderManagementRelationshipDialogTitle, Strings.SenderManagementRelationshipDialogLabel, typeof(ManagementRelationship), null)
			}, null, Strings.SenderManagementRelationshipConditionFormat, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleSenderManagementRelationshipFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MessageTypeMatches", Strings.TransportRuleMessageTypeMatchesPredicateText, new FormletParameter[]
			{
				new EnumParameter("MessageTypeMatches", Strings.MessageTypeMatchesDialogTitle, Strings.MessageTypeMatchesDialogLabel, typeof(MessageType), null)
			}, null, Strings.MessageTypeMatchesConditionFormat, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleMessageTypeMatchesFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HasClassification", Strings.TransportRuleHasClassificationPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("HasClassification", Strings.ClassificationDialogTitle, Strings.ClassificationDialogLabel, "RulesEditor/MessageClassifications.svc", Strings.NoMessageClassificationAvailable, null)
			}, "Get-MessageClassification@R:Self+!EOPStandard", Strings.TransportRuleHasClassificationConditionFormat, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleHasClassificationFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HasNoClassification", Strings.TransportRuleHasNoClassificationPredicateText, new FormletParameter[]
			{
				new BooleanParameter("HasNoClassification")
			}, "!EOPStandard", Strings.HasNoClassificationConditionFormat, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleHasNoClassificationFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SubjectContainsWords", Strings.TransportRuleSubjectContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.SubjectContainsWordsConditionFormat, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SubjectMatchesPatterns", Strings.TransportRuleSubjectMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SubjectMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.SubjectMatchesPatternsConditionFormat, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfToHeader", Strings.TransportRuleAnyOfToHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfToHeader", PickerType.PickTo)
			}, null, Strings.AnyOfToHeaderConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToHeaderFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfToHeaderMemberOf", Strings.TransportRuleAnyOfToHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfToHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.AnyOfToHeaderMemberOfConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToHeaderMemberOfFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfCcHeader", Strings.TransportRuleAnyOfCcHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfCcHeader", PickerType.PickTo)
			}, null, Strings.AnyOfCcHeaderConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfCcHeaderFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfCcHeaderMemberOf", Strings.TransportRuleAnyOfCcHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfCcHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.AnyOfCcHeaderMemberOfConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfCcHeaderMemberOfFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfToCcHeader", Strings.TransportRuleAnyOfToCcHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfToCcHeader", PickerType.PickTo)
			}, null, Strings.AnyOfToCcHeaderConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToCcHeaderFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AnyOfToCcHeaderMemberOf", Strings.TransportRuleAnyOfToCcHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("AnyOfToCcHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.AnyOfToCcHeaderMemberOfConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToCcHeaderMemberOfFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SCLOver", Strings.TransportRuleSCLOverPredicateText, new FormletParameter[]
			{
				new NumberEnumParameter("SCLOver", Strings.SCLOverDialogTitle, Strings.SCLOverDialogLabel, 9, -1, 5, new KeyValuePair<int, LocalizedString>[]
				{
					new KeyValuePair<int, LocalizedString>(-1, Strings.TransportRuleSCLBypass)
				})
			}, null, Strings.SCLOverConditionFormat, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleSCLOverFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("WithImportance", Strings.TransportRuleWithImportancePredicateText, new FormletParameter[]
			{
				new EnumParameter("WithImportance", Strings.WithImportanceDialogTitle, Strings.WithImportanceDialogLabel, typeof(Importance), null)
			}, null, Strings.WithImportanceConditionFormat, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleWithImportanceFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderInRecipientList", Strings.TransportRuleSenderInRecipientListPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("SenderInRecipientList", Strings.SenderInRecipientListDialogTitle, Strings.SenderInRecipientListDialogLabel, "RulesEditor/TransportConfigs.svc", Strings.NoSupervisionListAvailable, null)
			}, "Get-TransportConfig@C:OrganizationConfig+!EOPStandard", Strings.SenderInRecipientListConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderInRecipientListFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientInSenderList", Strings.TransportRuleRecipientInSenderListPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("RecipientInSenderList", Strings.RecipientInSenderListDialogTitle, Strings.RecipientInSenderListDialogLabel, "RulesEditor/TransportConfigs.svc", Strings.NoSupervisionListAvailable, null)
			}, "Get-TransportConfig@C:OrganizationConfig+!EOPStandard", Strings.RecipientInSenderListConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientInSenderListFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HeaderContains", Strings.TransportRuleHeaderContainsPredicateText, new FormletParameter[]
			{
				new StringParameter("HeaderContainsMessageHeader", Strings.HeaderContainsDialogTitle, Strings.HeaderContainsDialogLabel, typeof(HeaderName), false),
				new StringArrayParameter("HeaderContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.HeaderContainsWordsConditionFormat, Strings.TransportRuleMessageHeaderGroupText, Strings.TransportRuleHeaderContainsFlyOutText, LocalizedString.Empty, Strings.TransportRuleHeaderContainsExplanationText, false),
			new RuleCondition("BetweenMemberOf", Strings.TransportRuleBetweenMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("BetweenMemberOf1", PickerType.PickTo),
				new PeopleParameter("BetweenMemberOf2", PickerType.PickTo)
			}, null, Strings.BetweenMemberOfConditionFormat, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleBetweenMemberOfFlyOutText, LocalizedString.Empty, Strings.TransportRuleBetweenMemberOfExplanationText, false),
			new RuleCondition("HeaderMatches", Strings.TransportRuleHeaderMatchesPredicateText, new FormletParameter[]
			{
				new StringParameter("HeaderMatchesMessageHeader", Strings.HeaderMatchesDialogTitle, Strings.HeaderMatchesDialogLabel, typeof(HeaderName), false),
				new StringArrayParameter("HeaderMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.HeaderMatchesConditionFormat, Strings.TransportRuleMessageHeaderGroupText, Strings.TransportRuleHeaderMatchesFlyOutText, LocalizedString.Empty, Strings.TransportRuleHeaderMatchesExplanationText, false),
			new RuleCondition("ManagerForEvaluatedUser", Strings.TransportRuleManagerForEvaluatedUserPredicateText, new FormletParameter[]
			{
				new EnumParameter("ManagerForEvaluatedUser", Strings.ManagerForEvaluatedUserDialogTitle, Strings.ManagerForEvaluatedUserDialogLabel, typeof(EvaluatedUser), null),
				new PeopleParameter("ManagerAddresses", PickerType.PickTo)
			}, null, Strings.ManagerForEvaluatedUserConditionFormat, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleManagerForEvaluatedUserFlyOutText, LocalizedString.Empty, Strings.TransportRuleManagerForEvaluatedUserExplanationText, false),
			new RuleCondition("ADComparisonAttribute", Strings.TransportRuleADComparisonAttributePredicateText, new FormletParameter[]
			{
				new EnumParameter("ADComparisonAttribute", Strings.ADComparisonAttributeDialogTitle, Strings.ADComparisonAttributeDialogLabel, typeof(ADAttribute), null),
				new EnumParameter("ADComparisonOperator", Strings.ADComparisonOperatorDialogTitle, Strings.ADComparisonOperatorDialogLabel, typeof(Evaluation), null)
			}, null, Strings.ADComparisonAttributeConditionFormat, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleADComparisonAttributeFlyOutText, LocalizedString.Empty, Strings.TransportRuleADComparisonAttributeExplanationText, false),
			new RuleCondition("AttachmentExtensionMatchesWords", Strings.TransportRuleAttachmentExtensionMatchesWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("AttachmentExtensionMatchesWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.AttachmentExtenstionMatchesWordsConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentExtensionMatchesWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentSizeOver", Strings.TransportRuleAttachmentSizeOverPredicateText, new FormletParameter[]
			{
				new ByteQuantifiedSizeParameter("AttachmentSizeOver", Strings.AttachmentSizeOverDialogTitle, Strings.AttachmentSizeOverDialogLabel, 0L, 18014398509481982L, 0L)
			}, null, Strings.AttachmentSizeOverConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentSizeOverFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentProcessingLimitExceeded", Strings.TransportRuleAttachmentProcessingLimitsExceededPredicateText, new FormletParameter[]
			{
				new BooleanParameter("AttachmentProcessingLimitExceeded")
			}, null, Strings.AttachmentProcessingLimitsExceededConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentProcessingLimitExceededFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderADAttributeContainsWords", Strings.TransportRuleSenderADAttributeContainsWordsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("SenderADAttributeContainsWords", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel)
			}, null, Strings.SenderADAttributeContainsWordsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderADAttributeContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderADAttributeMatchesPatterns", Strings.TransportRuleSenderADAttributeMatchesPatternsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("SenderADAttributeMatchesPatterns", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel, Strings.TransportRuleSelectPropertiesTextPatternsNoSelectionText)
			}, null, Strings.SenderADAttributeMatchesPatternsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderADAttributeMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientADAttributeContainsWords", Strings.TransportRuleRecipientADAttributeContainsWordsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("RecipientADAttributeContainsWords", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel)
			}, null, Strings.RecipientADAttributeContainsWordsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientADAttributeContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientADAttributeMatchesPatterns", Strings.TransportRuleRecipientADAttributeMatchesPatternsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("RecipientADAttributeMatchesPatterns", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel, Strings.TransportRuleSelectPropertiesTextPatternsNoSelectionText)
			}, null, Strings.RecipientADAttributeMatchesPatternsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientADAttributeMatchesPatternsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("MessageSizeOver", Strings.TransportRuleMessageSizeOverPredicateText, new FormletParameter[]
			{
				new ByteQuantifiedSizeParameter("MessageSizeOver", Strings.MessageSizeOverDialogTitle, Strings.MessageSizeOverDialogLabel, 0L, 18014398509481982L, 512L)
			}, null, Strings.MessageSizeOverConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleMessageSizeOverFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("HasSenderOverride", Strings.TransportRuleHasSenderOverridePredicateText, new FormletParameter[]
			{
				new BooleanParameter("HasSenderOverride")
			}, "!EOPStandard", Strings.SenderHasOverridenConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderHasOverriddenTheActionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentHasExecutableContent", Strings.TransportRuleAttachmentHasExecutableContentPredicateText, new FormletParameter[]
			{
				new BooleanParameter("AttachmentHasExecutableContent")
			}, null, Strings.AttachmentHasExecutableContentConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentHasExecutableContentFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderIpRange", Strings.TransportRulesSenderIPRangeIsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SenderIpRanges", Strings.SenderIPDialogTitle, Strings.SenderIPDialogLabel, 255, Strings.TransportRuleSenderIpAddressNoSelectionText, Strings.TransportRuleSenderIPWatermark, "return IP4AddressValidator.Test($_)", Strings.TransportRuleSenderIPError)
			}, null, Strings.SenderIPRangeConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderIPAddressFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("SenderDomainIs", Strings.TransportRuleSenderDomainContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("SenderDomainIs", Strings.DomainDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.SenderDomainIsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderDomainContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("RecipientDomainIs", Strings.TransportRuleRecipientDomainContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("RecipientDomainIs", Strings.DomainDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.RecipientDomainIsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientDomainContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ContentCharacterSetContainsWords", Strings.TransportRuleContentCharacterPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ContentCharacterSetContainsWords", Strings.CharacterSetDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.MessageContainsCharacterSetConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleContentCharacterSetContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("AttachmentIsPasswordProtected", Strings.TransportRuleAttachmentIsPasswordProtectedPredicateText, new FormletParameter[]
			{
				new BooleanParameter("AttachmentIsPasswordProtected")
			}, null, Strings.AttachmentIsPasswordProtectedConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentIsPasswordProtectedFlyOutText, LocalizedString.Empty, false)
		};

		private RulePhrase[] supportedActions = new RulePhrase[]
		{
			new RulePhrase("ModerateMessageByUser", Strings.TransportRuleModerateMessageByUserActionText, new FormletParameter[]
			{
				new PeopleParameter("ModerateMessageByUser")
				{
					UseAndDelimiter = true
				}
			}, "!EOPStandard", Strings.TransportRuleForwardForApprovalGroupText, Strings.TransportRuleModerateMessageByUserFlyOutText, true),
			new RulePhrase("RedirectMessage", Strings.TransportRuleRedirectMessageActionText, new FormletParameter[]
			{
				new PeopleParameter("RedirectMessageTo")
				{
					UseAndDelimiter = true
				}
			}, null, Strings.TransportRuleRedirectTheMessageGroupText, Strings.TransportRuleRedirectMessageFlyOutText, true),
			new RulePhrase("RejectMessage", Strings.TransportRuleRejectMessageActionText, new FormletParameter[]
			{
				new StringParameter("RejectMessageReasonText", Strings.RejectReasonDialogTitle, Strings.RejectReasonDialogLabel, typeof(Word), false)
			}, null, Strings.TransportRuleBlockMessageGroupText, Strings.TransportRuleRejectMessageFlyOutText, true),
			new RulePhrase("RejectMessageEnhancedStatusCode", Strings.TransportRuleRejectMessageEnhancedStatusCodeText, new FormletParameter[]
			{
				new StringParameter("RejectMessageEnhancedStatusCode", Strings.RejectStatusCodeDialogTitle, Strings.RejectStatusCodeDialogLabel, typeof(RejectEnhancedStatus), false)
			}, null, Strings.TransportRuleBlockMessageGroupText, Strings.TransportRuleRejectMessageEnhancedStatusCodeFlyOutText, false),
			new RulePhrase("DeleteMessage", Strings.TransportRuleDeleteMessageActionText, new FormletParameter[]
			{
				new BooleanParameter("DeleteMessage")
			}, null, Strings.TransportRuleBlockMessageGroupText, Strings.TransportRuleDeleteMessageFlyOutText, true),
			new RulePhrase("BlindCopyTo", Strings.TransportRuleBlindCopyToActionText, new FormletParameter[]
			{
				new PeopleParameter("BlindCopyTo")
				{
					UseAndDelimiter = true
				}
			}, null, Strings.TransportRuleAddRecipientsGroupText, Strings.TransportRuleBlindCopyToFlyOutText, true),
			new RulePhrase("AppendHtmlDisclaimer", Strings.TransportRuleApplyHtmlDisclaimerActionText, new FormletParameter[]
			{
				new HiddenParameter("ApplyHtmlDisclaimerLocation", "Append"),
				new StringParameter("ApplyHtmlDisclaimerText", Strings.DisclaimerTextDialogTitle, Strings.DisclaimerTextDialogLabel, typeof(DisclaimerText), true),
				new EnumParameter("ApplyHtmlDisclaimerFallbackAction", Strings.DisclaimerFallbackDialogTitle, Strings.DisclaimerFallbackDialogLabel, typeof(DisclaimerFallbackAction), null)
			}, null, Strings.TransportRuleApplyDisclaimerGroupText, Strings.TransportRuleApplyHtmlDisclaimerFlyOutText, Strings.TransportRuleApplyHtmlDisclaimerExplanationText, true),
			new RulePhrase("PrependHtmlDisclaimer", Strings.TransportRuleApplyPrependHtmlDisclaimerActionText, new FormletParameter[]
			{
				new HiddenParameter("ApplyHtmlDisclaimerLocation", "Prepend"),
				new StringParameter("ApplyHtmlDisclaimerText", Strings.DisclaimerTextDialogTitle, Strings.DisclaimerTextDialogLabel, typeof(DisclaimerText), true),
				new EnumParameter("ApplyHtmlDisclaimerFallbackAction", Strings.DisclaimerFallbackDialogTitle, Strings.DisclaimerFallbackDialogTitle, typeof(DisclaimerFallbackAction), null)
			}, null, Strings.TransportRuleApplyDisclaimerGroupText, Strings.TransportRuleApplyPrependHtmlDisclaimerFlyOutText, Strings.TransportRuleApplyHtmlDisclaimerExplanationText, false),
			new RulePhrase("RemoveHeader", Strings.TransportRuleRemoveHeaderActionText, new FormletParameter[]
			{
				new StringParameter("RemoveHeader", Strings.MessageHeaderDisplayName, Strings.MessageHeaderDescription, typeof(HeaderName), false)
			}, null, Strings.TransportRuleModifyMessagePropertiesGroupText, Strings.TransportRuleRemoveHeaderFlyOutText, false),
			new RulePhrase("AddToRecipient", Strings.TransportRuleAddToRecipientActionText, new FormletParameter[]
			{
				new PeopleParameter("AddToRecipients")
				{
					UseAndDelimiter = true
				}
			}, null, Strings.TransportRuleAddRecipientsGroupText, Strings.TransportRuleAddToRecipientFlyOutText, false),
			new RulePhrase("CopyTo", Strings.TransportRuleCopyToActionText, new FormletParameter[]
			{
				new PeopleParameter("CopyTo")
				{
					UseAndDelimiter = true
				}
			}, null, Strings.TransportRuleAddRecipientsGroupText, Strings.TransportRuleCopyToFlyOutText, false),
			new RulePhrase("ModerateMessageByManager", Strings.TransportRuleModerateMessageByManagerActionText, new FormletParameter[]
			{
				new BooleanParameter("ModerateMessageByManager")
			}, "!EOPStandard", Strings.TransportRuleForwardForApprovalGroupText, Strings.TransportRuleModerateMessageByManagerFlyOutText, false),
			new RulePhrase("AddManagerAsRecipientType", Strings.TransportRuleAddManagerAsRecipientTypeActionText, new FormletParameter[]
			{
				new EnumParameter("AddManagerAsRecipientType", Strings.AddManagerAsRecipientTypeDialogTitle, Strings.AddManagerAsRecipientTypeDialogLabel, typeof(AddedRecipientType), null)
			}, null, Strings.TransportRuleAddRecipientsGroupText, Strings.TransportRuleAddManagerAsRecipientTypeFlyOutText, false),
			new RulePhrase("ApplyRightsProtectionTemplate", Strings.TransportRuleApplyRightsProtectionTemplateActionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ApplyRightsProtectionTemplate", Strings.ApplyRightsProtectionTemplateDialogTitle, Strings.ApplyRightsProtectionTemplateDialogLabel, "RulesEditor/RMSTemplates.svc", Strings.NoRMSTemplatesAvailable, null)
			}, null, Strings.TransportRuleModifyTheMessageSecurityGroupText, Strings.TransportRuleApplyRightsProtectionFlyOutText, false),
			new RulePhrase("RouteMessageOutboundRequireTls", Strings.TransportRuleRouteMessageOutboundRequireTlsActionText, new FormletParameter[]
			{
				new BooleanParameter("RouteMessageOutboundRequireTls")
			}, null, Strings.TransportRuleModifyTheMessageSecurityGroupText, Strings.TransportRuleRequireTLSEncryptionFlyOutText, false),
			new RulePhrase("ApplyOME", Strings.TransportRuleEncryptMessageWithEOEActionText, new FormletParameter[]
			{
				new BooleanParameter("ApplyOME")
			}, null, Strings.TransportRuleModifyTheMessageSecurityGroupText, Strings.TransportRuleEncryptMessageFlyOutText, false),
			new RulePhrase("RemoveOME", Strings.TransportRuleRemoveEOEEncryptMessageActionText, new FormletParameter[]
			{
				new BooleanParameter("RemoveOME")
			}, null, Strings.TransportRuleModifyTheMessageSecurityGroupText, Strings.TransportRuleDecryptMessageFlyOutText, false),
			new RulePhrase("PrependSubject", Strings.TransportRulePrependSubjectActionText, new FormletParameter[]
			{
				new StringParameter("PrependSubject", Strings.PrefixDisplayName, Strings.PrefixDescription, typeof(SubjectPrefix), false)
			}, null, false),
			new RulePhrase("SetHeaderName", Strings.TransportRuleSetHeaderNameActionText, new FormletParameter[]
			{
				new StringParameter("SetHeaderName", Strings.MessageHeaderDisplayName, Strings.MessageHeaderDescription, typeof(HeaderName), false),
				new StringParameter("SetHeaderValue", Strings.HeaderValueDisplayName, Strings.HeaderValueDescription, typeof(HeaderValue), false)
			}, null, Strings.TransportRuleModifyMessagePropertiesGroupText, Strings.TransportRuleSetHeaderNameFlyOutText, Strings.TransportRuleSetHeaderNameExplanationText, false),
			new RulePhrase("ApplyClassification", Strings.TransportRuleApplyClassificationActionText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ApplyClassification", Strings.ClassificationDialogTitle, Strings.ClassificationDialogLabel, "RulesEditor/MessageClassifications.svc", Strings.NoMessageClassificationAvailable, null)
			}, "Get-MessageClassification@R:Self+!EOPStandard", Strings.TransportRuleModifyMessagePropertiesGroupText, Strings.TransportRuleApplyClassificationFlyOutText, false),
			new RulePhrase("SetSCL", Strings.TransportRuleSetSCLActionText, new FormletParameter[]
			{
				new NumberEnumParameter("SetSCL", Strings.SetSCLDialogTitle, Strings.SetSCLDialogLabel, 9, -1, 5, new KeyValuePair<int, LocalizedString>[]
				{
					new KeyValuePair<int, LocalizedString>(-1, Strings.TransportRuleSCLBypass)
				})
			}, null, Strings.TransportRuleModifyMessagePropertiesGroupText, Strings.TransportRuleSetSCLFlyOutText, false),
			new RulePhrase("NotifySender", Strings.TransportRuleSenderNotifyActionText, new FormletParameter[]
			{
				new SenderNotifyParameter("SenderNotifySettings", Strings.SenderNotifyDialogTitle, Strings.SenderNotifyDialogLabel, Strings.TransportRuleSenderNotifyNoSelectionText, new string[]
				{
					"NotifySender",
					"RejectMessageReasonText"
				})
			}, "!EOPStandard", false),
			new RulePhrase("GenerateIncidentReport", Strings.TransportRuleGenerateIncidentReportActionText, new FormletParameter[]
			{
				new ObjectParameter("GenerateIncidentReport", LocalizedString.Empty, LocalizedString.Empty, typeof(Identity), "~/pickers/MailboxPicker.aspx", "Identity"),
				new IncidentReportContentParameter("IncidentReportContent", Strings.IncidentReportSelectContent, Strings.IncidentReportSelectContent, Strings.IncidentReportSelectContent)
			}, null, LocalizedString.Empty, LocalizedString.Empty, Strings.TransportRuleGenerateIncidentReportExplanationText, false),
			new RulePhrase("GenerateNotification", Strings.TransportRuleGenerateNotificationActionText, new FormletParameter[]
			{
				new StringParameter("GenerateNotification", Strings.NotificationTextDialogTitle, Strings.NotificationTextDialogLabel, typeof(DisclaimerText), true)
			}, null, false),
			new RulePhrase("Quarantine", Strings.TransportRuleQuarantineActionText, new FormletParameter[]
			{
				new BooleanParameter("Quarantine")
			}, "LiveID", Strings.TransportRuleRedirectTheMessageGroupText, Strings.TransportRuleQuarantineFlyOutText, false),
			TransportRules.GetConnectorRulePhrase()
		};

		private RulePhrase[] supportedExceptions = new RulePhrase[]
		{
			new RulePhrase("ExceptIfFrom", Strings.TransportRuleFromPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfFrom", PickerType.PickFrom)
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromFlyOutText, false),
			new RulePhrase("ExceptIfSentTo", Strings.TransportRuleSentToPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfSentTo", PickerType.PickTo)
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToFlyOutText, false),
			new RulePhrase("ExceptIfFromScope", Strings.TransportRuleFromScopePredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfFromScope", Strings.FromScopeDialogTitle, Strings.FromScopeDialogLabel, typeof(FromUserScope), null)
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromScopeFlyOutText, false),
			new RulePhrase("ExceptIfSentToScope", Strings.TransportRuleSentToScopePredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfSentToScope", Strings.ToScopeDialogTitle, Strings.ToScopeDialogLabel, typeof(ToUserScope), null)
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToScopeFlyOutText, false),
			new RulePhrase("ExceptIfFromMemberOf", Strings.TransportRuleFromMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfFromMemberOf", PickerType.PickFrom)
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromMemberOfFlyOutText, false),
			new RulePhrase("ExceptIfSentToMemberOf", Strings.TransportRuleSentToMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfSentToMemberOf", PickerType.PickTo)
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleSentToMemberOfFlyOutText, false),
			new RulePhrase("ExceptIfSubjectOrBodyContains", Strings.TransportRuleSubjectOrBodyContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectOrBodyContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectOrBodyContainsFlyOutText, false),
			new RulePhrase("ExceptIfFromAddressContains", Strings.TransportRuleFromAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfFromAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromAddressContainsFlyOutText, false),
			new RulePhrase("ExceptIfRecipientAddressContains", Strings.TransportRuleRecipientAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfRecipientAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientAddressContainsFlyOutText, false),
			new RulePhrase("ExceptIfAttachmentContainsWords", Strings.TransportRuleAttachmentContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAttachmentContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentContainsWordsFlyOutText, false),
			new RulePhrase("ExceptIfAttachmentMatchesPatterns", Strings.TransportRuleAttachmentMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAttachmentMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfAttachmentIsUnsupported", Strings.TransportRuleAttachmentIsUnsupportedPredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfAttachmentIsUnsupported")
			}, null, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentIsUnsupportedFlyOutText, false),
			new RulePhrase("ExceptIfSubjectOrBodyMatchesPatterns", Strings.TransportRuleSubjectOrBodyMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectOrBodyMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectOrBodyMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfFromAddressMatchesPatterns", Strings.TransportRuleFromAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfFromAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromAddressMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfRecipientAddressMatchesPatterns", Strings.TransportRuleRecipientAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfRecipientAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientAddressMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfAttachmentNameMatchesPatterns", Strings.TransportRuleAttachmentNameMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAttachmentNameMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentNameMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfRecipientAddressContainsWords", Strings.TransportRuleAnyRecipientAddressContainsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAnyOfRecipientAddressContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleAnyRecipientGroupText, Strings.TransportRuleFromAddressContainsFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfRecipientAddressMatchesPatterns", Strings.TransportRuleAnyRecipientAddressMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAnyOfRecipientAddressMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleAnyRecipientGroupText, Strings.TransportRuleRecipientAddressMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfMessageContainsDataClassifications", Strings.TransportRuleContainsSensitiveInformationPredicateText, new FormletParameter[]
			{
				new DLPParameter("ExceptIfMessageContainsDataClassifications", Strings.DLPickerDialogTitle, Strings.DLPickerDialogLabel, Strings.TransportRuleContainsSensitiveInformationParameterNoSelectionText)
			}, "!EOPStandard", Strings.TransportRuleMessageGroupText, Strings.TransportRuleContainsSensitiveInformationFlyOutText, false),
			new RulePhrase("ExceptIfSenderManagementRelationship", Strings.TransportRuleSenderManagementRelationshipPredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfSenderManagementRelationship", Strings.SenderManagementRelationshipDialogTitle, Strings.SenderManagementRelationshipDialogLabel, typeof(ManagementRelationship), null)
			}, null, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleSenderManagementRelationshipFlyOutText, false),
			new RulePhrase("ExceptIfMessageTypeMatches", Strings.TransportRuleMessageTypeMatchesPredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfMessageTypeMatches", Strings.MessageTypeMatchesDialogTitle, Strings.MessageTypeMatchesDialogLabel, typeof(MessageType), null)
			}, null, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleMessageTypeMatchesFlyOutText, false),
			new RulePhrase("ExceptIfHasClassification", Strings.TransportRuleHasClassificationPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ExceptIfHasClassification", Strings.ClassificationDialogTitle, Strings.ClassificationDialogLabel, "RulesEditor/MessageClassifications.svc", Strings.NoMessageClassificationAvailable, null)
			}, "Get-MessageClassification@R:Self+!EOPStandard", Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleHasClassificationFlyOutText, false),
			new RulePhrase("ExceptIfHasNoClassification", Strings.TransportRuleHasNoClassificationPredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfHasNoClassification")
			}, "!EOPStandard", Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleHasNoClassificationFlyOutText, false),
			new RulePhrase("ExceptIfSubjectContainsWords", Strings.TransportRuleSubjectContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectContainsWordsFlyOutText, false),
			new RulePhrase("ExceptIfSubjectMatchesPatterns", Strings.TransportRuleSubjectMatchesPatternsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSubjectMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleSubjectBodyGroupText, Strings.TransportRuleSubjectMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfToHeader", Strings.TransportRuleAnyOfToHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfToHeader", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToHeaderFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfToHeaderMemberOf", Strings.TransportRuleAnyOfToHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfToHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToHeaderMemberOfFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfCcHeader", Strings.TransportRuleAnyOfCcHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfCcHeader", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfCcHeaderFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfCcHeaderMemberOf", Strings.TransportRuleAnyOfCcHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfCcHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfCcHeaderMemberOfFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfToCcHeader", Strings.TransportRuleAnyOfToCcHeaderPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfToCcHeader", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToCcHeaderFlyOutText, false),
			new RulePhrase("ExceptIfAnyOfToCcHeaderMemberOf", Strings.TransportRuleAnyOfToCcHeaderMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfAnyOfToCcHeaderMemberOf", PickerType.PickTo)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleAnyOfToCcHeaderMemberOfFlyOutText, false),
			new RulePhrase("ExceptIfSCLOver", Strings.TransportRuleSCLOverPredicateText, new FormletParameter[]
			{
				new NumberEnumParameter("ExceptIfSCLOver", Strings.SCLOverDialogTitle, Strings.SCLOverDialogLabel, 9, -1, 5, new KeyValuePair<int, LocalizedString>[]
				{
					new KeyValuePair<int, LocalizedString>(-1, Strings.TransportRuleSCLBypass)
				})
			}, null, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleSCLOverFlyOutText, false),
			new RulePhrase("ExceptIfWithImportance", Strings.TransportRuleWithImportancePredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfWithImportance", Strings.WithImportanceDialogTitle, Strings.WithImportanceDialogLabel, typeof(Importance), null)
			}, null, Strings.TransportRuleMessagePropertiesGroupText, Strings.TransportRuleWithImportanceFlyOutText, false),
			new RulePhrase("ExceptIfSenderInRecipientList", Strings.TransportRuleSenderInRecipientListPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ExceptIfSenderInRecipientList", Strings.SenderInRecipientListDialogTitle, Strings.SenderInRecipientListDialogLabel, "RulesEditor/TransportConfigs.svc", Strings.NoSupervisionListAvailable, null)
			}, "Get-TransportConfig@C:OrganizationConfig+!EOPStandard", Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderInRecipientListFlyOutText, false),
			new RulePhrase("ExceptIfRecipientInSenderList", Strings.TransportRuleRecipientInSenderListPredicateText, new FormletParameter[]
			{
				new EnhancedEnumParameter("ExceptIfRecipientInSenderList", Strings.RecipientInSenderListDialogTitle, Strings.RecipientInSenderListDialogLabel, "RulesEditor/TransportConfigs.svc", Strings.NoSupervisionListAvailable, null)
			}, "Get-TransportConfig@C:OrganizationConfig+!EOPStandard", Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientInSenderListFlyOutText, false),
			new RulePhrase("ExceptIfHeaderContains", Strings.TransportRuleHeaderContainsPredicateText, new FormletParameter[]
			{
				new StringParameter("ExceptIfHeaderContainsMessageHeader", Strings.HeaderContainsDialogTitle, Strings.HeaderContainsDialogLabel, typeof(HeaderName), false),
				new StringArrayParameter("ExceptIfHeaderContainsWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, typeof(Word[]))
			}, null, Strings.TransportRuleMessageHeaderGroupText, Strings.TransportRuleHeaderContainsFlyOutText, Strings.TransportRuleHeaderContainsExplanationText, false),
			new RulePhrase("ExceptIfBetweenMemberOf", Strings.TransportRuleBetweenMemberOfPredicateText, new FormletParameter[]
			{
				new PeopleParameter("ExceptIfBetweenMemberOf1", PickerType.PickTo),
				new PeopleParameter("ExceptIfBetweenMemberOf2", PickerType.PickTo)
			}, null, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleBetweenMemberOfFlyOutText, Strings.TransportRuleBetweenMemberOfExplanationText, false),
			new RulePhrase("ExceptIfHeaderMatches", Strings.TransportRuleHeaderMatchesPredicateText, new FormletParameter[]
			{
				new StringParameter("ExceptIfHeaderMatchesMessageHeader", Strings.HeaderMatchesDialogTitle, Strings.HeaderMatchesDialogLabel, typeof(HeaderName), false),
				new StringArrayParameter("ExceptIfHeaderMatchesPatterns", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.TransportRuleMessageHeaderGroupText, Strings.TransportRuleHeaderMatchesFlyOutText, Strings.TransportRuleHeaderMatchesExplanationText, false),
			new RulePhrase("ExceptIfManagerForEvaluatedUser", Strings.TransportRuleManagerForEvaluatedUserPredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfManagerForEvaluatedUser", Strings.ManagerForEvaluatedUserDialogTitle, Strings.ManagerForEvaluatedUserDialogLabel, typeof(EvaluatedUser), null),
				new PeopleParameter("ExceptIfManagerAddresses", PickerType.PickTo)
			}, null, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleManagerForEvaluatedUserFlyOutText, Strings.TransportRuleManagerForEvaluatedUserExplanationText, false),
			new RulePhrase("ExceptIfADComparisonAttribute", Strings.TransportRuleADComparisonAttributePredicateText, new FormletParameter[]
			{
				new EnumParameter("ExceptIfADComparisonAttribute", Strings.ADComparisonAttributeDialogTitle, Strings.ADComparisonAttributeDialogLabel, typeof(ADAttribute), null),
				new EnumParameter("ExceptIfADComparisonOperator", Strings.ADComparisonOperatorDialogTitle, Strings.ADComparisonOperatorDialogLabel, typeof(Evaluation), null)
			}, null, Strings.TransportRuleSenderRecipientGroupText, Strings.TransportRuleADComparisonAttributeFlyOutText, Strings.TransportRuleADComparisonAttributeExplanationText, false),
			new RuleCondition("ExceptIfAttachmentExtensionMatchesWords", Strings.TransportRuleAttachmentExtensionMatchesWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfAttachmentExtensionMatchesWords", Strings.StringArrayDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleEnterTextPatternsNoSelectionText, typeof(Word[]))
			}, null, Strings.AttachmentExtenstionMatchesWordsConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentExtensionMatchesWordsFlyOutText, LocalizedString.Empty, false),
			new RulePhrase("ExceptIfAttachmentSizeOver", Strings.TransportRuleAttachmentSizeOverPredicateText, new FormletParameter[]
			{
				new ByteQuantifiedSizeParameter("ExceptIfAttachmentSizeOver", Strings.AttachmentSizeOverDialogTitle, Strings.AttachmentSizeOverDialogLabel, 0L, 18014398509481982L, 0L)
			}, null, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentSizeOverFlyOutText, false),
			new RuleCondition("ExceptIfAttachmentProcessingLimitExceeded", Strings.TransportRuleAttachmentProcessingLimitsExceededPredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfAttachmentProcessingLimitExceeded")
			}, null, Strings.AttachmentProcessingLimitsExceededConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentProcessingLimitExceededFlyOutText, LocalizedString.Empty, false),
			new RulePhrase("ExceptIfSenderADAttributeContainsWords", Strings.TransportRuleSenderADAttributeContainsWordsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("ExceptIfSenderADAttributeContainsWords", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel)
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderADAttributeContainsWordsFlyOutText, false),
			new RulePhrase("ExceptIfSenderADAttributeMatchesPatterns", Strings.TransportRuleSenderADAttributeMatchesPatternsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("ExceptIfSenderADAttributeMatchesPatterns", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel, Strings.TransportRuleSelectPropertiesTextPatternsNoSelectionText)
			}, null, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderADAttributeMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfRecipientADAttributeContainsWords", Strings.TransportRuleRecipientADAttributeContainsWordsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("ExceptIfRecipientADAttributeContainsWords", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel)
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientADAttributeContainsWordsFlyOutText, false),
			new RulePhrase("ExceptIfRecipientADAttributeMatchesPatterns", Strings.TransportRuleRecipientADAttributeMatchesPatternsPredicateText, new FormletParameter[]
			{
				new ADAttributeParameter("ExceptIfRecipientADAttributeMatchesPatterns", Strings.ADAttributeDialogTitle, Strings.ADAttributeDialogLabel, Strings.TransportRuleSelectPropertiesTextPatternsNoSelectionText)
			}, null, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientADAttributeMatchesPatternsFlyOutText, false),
			new RulePhrase("ExceptIfMessageSizeOver", Strings.TransportRuleMessageSizeOverPredicateText, new FormletParameter[]
			{
				new ByteQuantifiedSizeParameter("ExceptIfMessageSizeOver", Strings.MessageSizeOverDialogTitle, Strings.MessageSizeOverDialogLabel, 0L, 18014398509481982L, 512L)
			}, null, Strings.TransportRuleMessageGroupText, Strings.TransportRuleMessageSizeOverFlyOutText, LocalizedString.Empty, false),
			new RulePhrase("ExceptIfHasSenderOverride", Strings.TransportRuleHasSenderOverridePredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfHasSenderOverride")
			}, "!EOPStandard", Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderHasOverriddenTheActionFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfAttachmentHasExecutableContent", Strings.TransportRuleAttachmentHasExecutableContentPredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfAttachmentHasExecutableContent")
			}, null, Strings.AttachmentHasExecutableContentConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentHasExecutableContentFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfSenderIpRange", Strings.TransportRulesSenderIPRangeIsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSenderIpRanges", Strings.SenderIPDialogTitle, Strings.SenderIPDialogLabel, 255, Strings.TransportRuleSenderIpAddressNoSelectionText, Strings.TransportRuleSenderIPWatermark, "return IP4AddressValidator.Test($_)", Strings.TransportRuleSenderIPError)
			}, null, Strings.SenderIPRangeConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderIPAddressFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfSenderDomainIs", Strings.TransportRuleSenderDomainContainsWordsPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfSenderDomainIs", Strings.DomainDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.SenderDomainIsConditionFormat, Strings.TransportRuleSenderGroupText, Strings.TransportRuleSenderDomainContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfRecipientDomainIs", Strings.TransportRuleRecipientDomainContainsWordsFlyOutText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfRecipientDomainIs", Strings.DomainDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.RecipientDomainIsConditionFormat, Strings.TransportRuleRecipientGroupText, Strings.TransportRuleRecipientDomainContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfContentCharacterSetContainsWords", Strings.TransportRuleContentCharacterPredicateText, new FormletParameter[]
			{
				new StringArrayParameter("ExceptIfContentCharacterSetContainsWords", Strings.CharacterSetDialogTitle, Strings.StringArrayDialogLabel)
			}, null, Strings.MessageContainsCharacterSetConditionFormat, Strings.TransportRuleMessageGroupText, Strings.TransportRuleContentCharacterSetContainsWordsFlyOutText, LocalizedString.Empty, false),
			new RuleCondition("ExceptIfAttachmentIsPasswordProtected", Strings.TransportRuleAttachmentIsPasswordProtectedPredicateText, new FormletParameter[]
			{
				new BooleanParameter("ExceptIfAttachmentIsPasswordProtected")
			}, null, Strings.AttachmentIsPasswordProtectedConditionFormat, Strings.TransportRuleAttachmentGroupText, Strings.TransportRuleAttachmentIsPasswordProtectedFlyOutText, LocalizedString.Empty, false)
		};
	}
}
