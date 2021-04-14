using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class JournalRules : RuleDataService, IJournalRules, IDataSourceService<JournalRuleFilter, JournalRuleRow, JournalRule, SetJournalRule, NewJournalRule>, IDataSourceService<JournalRuleFilter, JournalRuleRow, JournalRule, SetJournalRule, NewJournalRule, BaseWebServiceParameters>, IEditListService<JournalRuleFilter, JournalRuleRow, JournalRule, NewJournalRule, BaseWebServiceParameters>, IGetListService<JournalRuleFilter, JournalRuleRow>, INewObjectService<JournalRuleRow, NewJournalRule>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<JournalRule, SetJournalRule, JournalRuleRow>, IGetObjectService<JournalRule>, IGetObjectForListService<JournalRuleRow>
	{
		public JournalRules() : base("JournalRule")
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-JournalRule@R:Organization")]
		public PowerShellResults<JournalRuleRow> GetList(JournalRuleFilter filter, SortOptions sort)
		{
			return base.GetList<JournalRuleRow, JournalRuleFilter>("Get-JournalRule", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-JournalRule?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-JournalRule", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-JournalRule?Identity@R:Organization")]
		public PowerShellResults<JournalRule> GetObject(Identity identity)
		{
			return base.GetObject<JournalRule>("Get-JournalRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-JournalRule?Identity@R:Organization")]
		public PowerShellResults<JournalRuleRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<JournalRuleRow>("Get-JournalRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-JournalRule@W:Organization")]
		public PowerShellResults<JournalRuleRow> NewObject(NewJournalRule properties)
		{
			properties.FaultIfNull();
			properties.Enabled = new bool?(true);
			properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			PowerShellResults<JournalRuleRow> powerShellResults = base.NewObject<JournalRuleRow, NewJournalRule>("New-JournalRule", properties);
			powerShellResults.Output = null;
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-JournalRule?Identity@R:Organization+Set-JournalRule?Identity@W:Organization")]
		public PowerShellResults<JournalRuleRow> SetObject(Identity identity, SetJournalRule properties)
		{
			properties.FaultIfNull();
			if (properties.Name != null)
			{
				properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			}
			return base.SetObject<JournalRule, SetJournalRule, JournalRuleRow>("Set-JournalRule", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-JournalRule?Identity@W:Organization")]
		public PowerShellResults<JournalRuleRow> DisableRule(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<JournalRuleRow>(new PSCommand().AddCommand("Disable-JournalRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-JournalRule?Identity@W:Organization")]
		public PowerShellResults<JournalRuleRow> EnableRule(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<JournalRuleRow>(new PSCommand().AddCommand("Enable-JournalRule"), identities, parameters);
		}

		public override int RuleNameMaxLength
		{
			get
			{
				return JournalRules.ruleNameMaxLength;
			}
		}

		public override RulePhrase[] SupportedConditions
		{
			get
			{
				return JournalRules.supportedConditions;
			}
		}

		public override RulePhrase[] SupportedActions
		{
			get
			{
				return JournalRules.supportedActions;
			}
		}

		public override RulePhrase[] SupportedExceptions
		{
			get
			{
				return new RulePhrase[0];
			}
		}

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		internal const string NewJournalRule = "New-JournalRule";

		internal const string GetJournalRule = "Get-JournalRule";

		internal const string SetJournalRule = "Set-JournalRule";

		internal const string RemoveJournalRule = "Remove-JournalRule";

		internal const string DisableJournalRule = "Disable-JournalRule";

		internal const string EnableJournalRule = "Enable-JournalRule";

		internal const string JournalEmailAddressParameterName = "JournalEmailAddress";

		internal const string RecipientParameterName = "Recipient";

		internal const string ScopeParameterName = "Scope";

		internal const string NameParameterName = "Name";

		internal const string EnabledParameterName = "Enabled";

		internal const string JournalRuleScopeName = "JournalRuleScope";

		internal const string GetListRole = "Get-JournalRule@R:Organization";

		internal const string RemoveObjectsRole = "Remove-JournalRule?Identity@W:Organization";

		internal const string GetObjectRole = "Get-JournalRule?Identity@R:Organization";

		internal const string NewObjectRole = "New-JournalRule@W:Organization";

		internal const string SetObjectRole = "Get-JournalRule?Identity@R:Organization+Set-JournalRule?Identity@W:Organization";

		internal const string DisableRuleRole = "Disable-JournalRule?Identity@W:Organization";

		internal const string EnableRuleRole = "Enable-JournalRule?Identity@W:Organization";

		private static int ruleNameMaxLength = Util.GetMaxLengthFromDefinition(ADObjectSchema.Name);

		private static RulePhrase[] supportedConditions = new RulePhrase[]
		{
			new RuleCondition("Recipient", Strings.JournalRecipientParameterLabel, new FormletParameter[]
			{
				new PeopleParameter("Recipient", PickerType.PickSelectMailbox)
			}, null, Strings.JournalRuleAutoNameFormatString, Strings.TransportRuleSenderGroupText, Strings.TransportRuleFromFlyOutText, LocalizedString.Empty, true)
		};

		private static RulePhrase[] supportedActions = new RulePhrase[]
		{
			new RulePhrase("JournalRuleScope", Strings.JournalRuleAllMessagesLabel, new FormletParameter[]
			{
				new JournalRuleScopeParameter("Global")
			}, null, true),
			new RulePhrase("JournalRuleScope", Strings.JournalRuleInternalMessagesLabel, new FormletParameter[]
			{
				new JournalRuleScopeParameter("Internal")
			}, null, true),
			new RulePhrase("JournalRuleScope", Strings.JournalRuleExternalMessagesLabel, new FormletParameter[]
			{
				new JournalRuleScopeParameter("External")
			}, null, true)
		};
	}
}
