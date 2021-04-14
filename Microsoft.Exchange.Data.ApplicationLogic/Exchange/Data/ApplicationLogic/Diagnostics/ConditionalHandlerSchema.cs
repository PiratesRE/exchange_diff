using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class ConditionalHandlerSchema : ObjectSchema
	{
		internal static SimpleProviderPropertyDefinition BuildStringPropDef(string name)
		{
			return ConditionalHandlerSchema.BuildRefTypePropDef<string>(name);
		}

		internal static SimpleProviderPropertyDefinition BuildRefTypePropDef<T>(string name)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(T), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildValueTypePropDef<T>(string name, T defaultValue)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(T), PropertyDefinitionFlags.PersistDefaultValue, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildUnlimitedPropDef(string name)
		{
			return new SimpleProviderPropertyDefinition(name, ExchangeObjectVersion.Current, typeof(Unlimited<uint>), PropertyDefinitionFlags.None, Unlimited<uint>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		internal static SimpleProviderPropertyDefinition BuildValueTypePropDef<T>(string name)
		{
			return ConditionalHandlerSchema.BuildValueTypePropDef<T>(name, default(T));
		}

		public static readonly SimpleProviderPropertyDefinition SmtpAddress = ConditionalHandlerSchema.BuildStringPropDef("SmtpAddress");

		public static readonly SimpleProviderPropertyDefinition DisplayName = ConditionalHandlerSchema.BuildStringPropDef("DisplayName");

		public static readonly SimpleProviderPropertyDefinition TenantName = ConditionalHandlerSchema.BuildStringPropDef("TenantName");

		public static readonly SimpleProviderPropertyDefinition WindowsLiveId = ConditionalHandlerSchema.BuildStringPropDef("WindowsLiveId");

		public static readonly SimpleProviderPropertyDefinition MailboxServer = ConditionalHandlerSchema.BuildStringPropDef("MailboxServer");

		public static readonly SimpleProviderPropertyDefinition MailboxDatabase = ConditionalHandlerSchema.BuildStringPropDef("MailboxDatabase");

		public static readonly SimpleProviderPropertyDefinition MailboxServerVersion = ConditionalHandlerSchema.BuildStringPropDef("MailboxServerVersion");

		public static readonly SimpleProviderPropertyDefinition IsMonitoringUser = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("IsMonitoringUser");

		public static readonly SimpleProviderPropertyDefinition IsOverBudgetAtStart = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("IsOverBudgetAtStart");

		public static readonly SimpleProviderPropertyDefinition IsOverBudgetAtEnd = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("IsOverBudgetAtEnd");

		public static readonly SimpleProviderPropertyDefinition BudgetBalanceStart = ConditionalHandlerSchema.BuildValueTypePropDef<float>("BudgetBalanceStart");

		public static readonly SimpleProviderPropertyDefinition BudgetBalanceEnd = ConditionalHandlerSchema.BuildValueTypePropDef<float>("BudgetBalanceEnd");

		public static readonly SimpleProviderPropertyDefinition BudgetDelay = ConditionalHandlerSchema.BuildValueTypePropDef<float>("BudgetDelay");

		public static readonly SimpleProviderPropertyDefinition BudgetUsed = ConditionalHandlerSchema.BuildValueTypePropDef<float>("BudgetUsed");

		public static readonly SimpleProviderPropertyDefinition BudgetLockedOut = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("BudgetLockedOut");

		public static readonly SimpleProviderPropertyDefinition BudgetLockedUntil = ConditionalHandlerSchema.BuildValueTypePropDef<ExDateTime>("BudgetLockedUntil");

		public static readonly SimpleProviderPropertyDefinition ActivityId = ConditionalHandlerSchema.BuildStringPropDef("ActivityId");

		public static readonly SimpleProviderPropertyDefinition Cmd = ConditionalHandlerSchema.BuildStringPropDef("Cmd");

		public static readonly SimpleProviderPropertyDefinition ElapsedTime = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("ElapsedTime");

		public static readonly SimpleProviderPropertyDefinition Exception = ConditionalHandlerSchema.BuildStringPropDef("Exception");

		public static readonly SimpleProviderPropertyDefinition PreWlmElapsed = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("PreWlmDelay");

		public static readonly SimpleProviderPropertyDefinition WlmQueueElapsed = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("WlmQueueElapsed");

		public static readonly SimpleProviderPropertyDefinition PostWlmElapsed = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("PostWlmDelay");

		public static readonly SimpleProviderPropertyDefinition CommandElapsed = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("CommandElapsed");

		public static readonly SimpleProviderPropertyDefinition ThrottlingPolicyName = ConditionalHandlerSchema.BuildStringPropDef("ThrottlingPolicyName");

		public static readonly SimpleProviderPropertyDefinition MaxConcurrency = ConditionalHandlerSchema.BuildUnlimitedPropDef("MaxConcurrency");

		public static readonly SimpleProviderPropertyDefinition MaxBurst = ConditionalHandlerSchema.BuildUnlimitedPropDef("MaxBurst");

		public static readonly SimpleProviderPropertyDefinition RechargeRate = ConditionalHandlerSchema.BuildUnlimitedPropDef("RechargeRate");

		public static readonly SimpleProviderPropertyDefinition CutoffBalance = ConditionalHandlerSchema.BuildUnlimitedPropDef("CutoffBalance");

		public static readonly SimpleProviderPropertyDefinition ThrottlingPolicyScope = ConditionalHandlerSchema.BuildStringPropDef("ThrottlingPolicyScope");

		public static readonly SimpleProviderPropertyDefinition ConcurrencyStart = ConditionalHandlerSchema.BuildValueTypePropDef<int>("ConcurrencyStart");

		public static readonly SimpleProviderPropertyDefinition ConcurrencyEnd = ConditionalHandlerSchema.BuildValueTypePropDef<int>("ConcurrencyEnd");
	}
}
