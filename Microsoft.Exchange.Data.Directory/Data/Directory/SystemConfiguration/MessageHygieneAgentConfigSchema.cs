using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MessageHygieneAgentConfigSchema : ADConfigurationObjectSchema
	{
		private const int EnabledMask = 1;

		private const int ExternalMailEnabledMask = 2;

		private const int InternalMailEnabledMask = 4;

		internal const int OutlookEmailPostmarkValidationEnabledMask = 64;

		internal const int SCLDeleteEnabledMask = 128;

		internal const int SCLRejectEnabledMask = 256;

		internal const int SCLQuarantineEnabledMask = 512;

		internal const int BlockListEnabledMask = 1024;

		internal const int RecipientValidationEnabledMask = 2048;

		internal const int BlockBlankSendersMask = 4096;

		public static readonly ADPropertyDefinition AgentsFlags = new ADPropertyDefinition("AgentsFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchAgentsFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(1, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(1, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition ExternalMailEnabled = new ADPropertyDefinition("ExternalMailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(2, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(2, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition InternalMailEnabled = new ADPropertyDefinition("InternalMailEnabled ", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(4, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(4, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		private struct Defaults
		{
			public const int AgentFlags = 3;
		}
	}
}
