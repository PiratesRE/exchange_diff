using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class UMCallAnsweringRules : RuleDataService, IUMCallAnsweringRules, IDataSourceService<UMCallAnsweringRuleFilter, RuleRow, UMCallAnsweringRule, SetUMCallAnsweringRule, NewUMCallAnsweringRule, RemoveUMCallAnsweringRule>, IEditListService<UMCallAnsweringRuleFilter, RuleRow, UMCallAnsweringRule, NewUMCallAnsweringRule, RemoveUMCallAnsweringRule>, IGetListService<UMCallAnsweringRuleFilter, RuleRow>, INewObjectService<RuleRow, NewUMCallAnsweringRule>, IRemoveObjectsService<RemoveUMCallAnsweringRule>, IEditObjectForListService<UMCallAnsweringRule, SetUMCallAnsweringRule, RuleRow>, IGetObjectService<UMCallAnsweringRule>, IGetObjectForListService<RuleRow>
	{
		public UMCallAnsweringRules() : base("UMCallAnsweringRule")
		{
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule@R:Self")]
		public PowerShellResults<RuleRow> GetList(UMCallAnsweringRuleFilter filter, SortOptions sort)
		{
			return base.GetList<RuleRow, UMCallAnsweringRuleFilter>("Get-UMCallAnsweringRule", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-UMCallAnsweringRule?Identity@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, RemoveUMCallAnsweringRule parameters)
		{
			parameters = (parameters ?? new RemoveUMCallAnsweringRule());
			return base.Invoke(new PSCommand().AddCommand("Remove-UMCallAnsweringRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule?Identity@R:Self")]
		public PowerShellResults<UMCallAnsweringRule> GetObject(Identity identity)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-UMCallAnsweringRule");
			return base.GetObject<UMCallAnsweringRule>(psCommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule?Identity@R:Self")]
		public PowerShellResults<RuleRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<RuleRow>("Get-UMCallAnsweringRule", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-UMCallAnsweringRule@W:Self")]
		public PowerShellResults<RuleRow> NewObject(NewUMCallAnsweringRule properties)
		{
			properties.FaultIfNull();
			properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			PowerShellResults<RuleRow> powerShellResults = base.NewObject<RuleRow, NewUMCallAnsweringRule>("New-UMCallAnsweringRule", properties);
			powerShellResults.Output = null;
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule?Identity@R:Self+Set-UMCallAnsweringRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> SetObject(Identity identity, SetUMCallAnsweringRule properties)
		{
			properties.FaultIfNull();
			if (properties.Name != null)
			{
				properties.Name = base.GetUniqueRuleName(properties.Name, this.GetList(null, null).Output);
			}
			return base.SetObject<UMCallAnsweringRule, SetUMCallAnsweringRule, RuleRow>("Set-UMCallAnsweringRule", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-UMCallAnsweringRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> DisableRule(Identity[] identities, DisableUMCallAnsweringRule parameters)
		{
			parameters = (parameters ?? new DisableUMCallAnsweringRule());
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Disable-UMCallAnsweringRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-UMCallAnsweringRule?Identity@W:Self")]
		public PowerShellResults<RuleRow> EnableRule(Identity[] identities, EnableUMCallAnsweringRule parameters)
		{
			parameters = (parameters ?? new EnableUMCallAnsweringRule());
			return base.InvokeAndGetObject<RuleRow>(new PSCommand().AddCommand("Enable-UMCallAnsweringRule"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule?Identity@R:Self+Set-UMCallAnsweringRule?Identity&Priority@W:Self")]
		public PowerShellResults IncreasePriority(Identity[] identities, ChangeUMCallAnsweringRule parameters)
		{
			parameters = (parameters ?? new ChangeUMCallAnsweringRule());
			return base.ChangePriority<UMCallAnsweringRule>(identities, -1, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMCallAnsweringRule?Identity@R:Self+Set-UMCallAnsweringRule?Identity&Priority@W:Self")]
		public PowerShellResults DecreasePriority(Identity[] identities, ChangeUMCallAnsweringRule parameters)
		{
			parameters = (parameters ?? new ChangeUMCallAnsweringRule());
			return base.ChangePriority<UMCallAnsweringRule>(identities, 1, parameters);
		}

		public override int RuleNameMaxLength
		{
			get
			{
				return UMCallAnsweringRules.ruleNameMaxLength;
			}
		}

		public override RulePhrase[] SupportedConditions
		{
			get
			{
				return UMCallAnsweringRules.supportedConditions;
			}
		}

		public override RulePhrase[] SupportedActions
		{
			get
			{
				return UMCallAnsweringRules.supportedActions;
			}
		}

		public override RulePhrase[] SupportedExceptions
		{
			get
			{
				return UMCallAnsweringRules.supportedExceptions;
			}
		}

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string NewUMCallAnsweringRule = "New-UMCallAnsweringRule";

		internal const string GetUMCallAnsweringRule = "Get-UMCallAnsweringRule";

		internal const string SetUMCallAnsweringRule = "Set-UMCallAnsweringRule";

		internal const string RemoveUMCallAnsweringRule = "Remove-UMCallAnsweringRule";

		internal const string DisableUMCallAnsweringRule = "Disable-UMCallAnsweringRule";

		internal const string EnableUMCallAnsweringRule = "Enable-UMCallAnsweringRule";

		internal const string GetListRole = "Get-UMCallAnsweringRule@R:Self";

		internal const string RemoveObjectsRole = "Remove-UMCallAnsweringRule?Identity@W:Self";

		internal const string GetObjectRole = "Get-UMCallAnsweringRule?Identity@R:Self";

		internal const string NewObjectRole = "New-UMCallAnsweringRule@W:Self";

		internal const string SetObjectRole = "Get-UMCallAnsweringRule?Identity@R:Self+Set-UMCallAnsweringRule?Identity@W:Self";

		internal const string DisableRuleRole = "Disable-UMCallAnsweringRule?Identity@W:Self";

		internal const string EnableRuleRole = "Enable-UMCallAnsweringRule?Identity@W:Self";

		internal const string ChangePriorityRole = "Get-UMCallAnsweringRule?Identity@R:Self+Set-UMCallAnsweringRule?Identity&Priority@W:Self";

		private static int ruleNameMaxLength = Util.GetMaxLengthFromDefinition(UMCallAnsweringRuleSchema.Name);

		private static RulePhrase[] supportedConditions = new RulePhrase[]
		{
			new RuleCondition("CheckAutomaticReplies", Strings.CallAnsweringRuleCheckAutomaticRepliesText, new FormletParameter[]
			{
				new BooleanParameter("CheckAutomaticReplies")
			}, null, Strings.CheckAutomaticRepliesFormat, Strings.CallAnsweringRuleCheckAutomaticRepliesGroupText, Strings.CallAnsweringRuleCheckAutomaticRepliesFlyoutText, Strings.CallAnsweringRuleCheckAutomaticPreCannedText, true),
			new RuleCondition("ScheduleStatus", Strings.CallAnsweringRuleScheduleStatusText, new FormletParameter[]
			{
				new EnumParameter("ScheduleStatus", LocalizedString.Empty, LocalizedString.Empty, typeof(FreeBusyStatusEnum), null)
				{
					Values = new EnumValue[]
					{
						new EnumValue(Strings.ScheduleStatusFreeText, FreeBusyStatusEnum.Free.ToString()),
						new EnumValue(Strings.ScheduleStatusTentativeText, FreeBusyStatusEnum.Tentative.ToString()),
						new EnumValue(Strings.ScheduleStatusBusyText, FreeBusyStatusEnum.Busy.ToString()),
						new EnumValue(Strings.ScheduleStatusAwayText, FreeBusyStatusEnum.OutOfOffice.ToString())
					}
				}
			}, null, Strings.ScheduleStatusFormat, Strings.CallAnsweringRuleScheduleStatusGroupText, Strings.CallAnsweringRuleScheduleStatusFlyoutText, Strings.CallAnsweringRuleScheduleStatusPreCannedText, true),
			new RuleCondition("ExtensionsDialed", Strings.CallAnsweringRuleExtensionsDialedText, new FormletParameter[]
			{
				new ExtensionsDialedParameter("ExtensionsDialed")
			}, null, Strings.ExtensionsDialedFormat, Strings.CallAnsweringRuleExtensionsDialedGroupText, Strings.CallAnsweringRuleExtensionsDialedFlyoutText, Strings.CallAnsweringRuleExtensionsDialedPreCannedText, true),
			new RuleCondition("TimeOfDay", Strings.CallAnsweringRuleTimePeriodText, new FormletParameter[]
			{
				new TimePeriodParameter("TimeOfDay")
			}, null, Strings.TimeOfDayFormat, Strings.CallAnsweringRuleTimePeriodGroupText, Strings.CallAnsweringRuleTimePeriodFlyoutText, LocalizedString.Empty, true),
			new RuleCondition("CallerIds", Strings.CallAnsweringRuleCallerIdsText, new FormletParameter[]
			{
				new CallerIdsParameter("CallerIds")
			}, null, Strings.CallerIdsFormat, Strings.CallAnsweringRuleCallerIdsGroupText, Strings.CallAnsweringRuleCallerIdsFlyoutText, LocalizedString.Empty, true)
		};

		private static RulePhrase[] supportedActions = new RulePhrase[]
		{
			new RulePhrase("KeyMappings", Strings.CallAnsweringRuleKeyMappingsText, new FormletParameter[]
			{
				new KeyMappingsParameter("KeyMappings")
			}, null, Strings.CallAnsweringRuleKeyMappingsGroupText, Strings.CallAnsweringRuleKeyMappingsFlyoutText, true, true)
		};

		private static RulePhrase[] supportedExceptions = new RulePhrase[0];
	}
}
