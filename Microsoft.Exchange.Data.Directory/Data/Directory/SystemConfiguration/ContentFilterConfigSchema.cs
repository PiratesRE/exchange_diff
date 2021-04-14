using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ContentFilterConfigSchema : MessageHygieneAgentConfigSchema
	{
		public static readonly ADPropertyDefinition OutlookEmailPostmarkValidationEnabled = new ADPropertyDefinition("OutlookEmailPostmarkValidationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(64, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(64, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition RejectionResponse = new ADPropertyDefinition("RejectionResponse", ExchangeObjectVersion.Exchange2007, typeof(AsciiString), "msExchMessageHygieneRejectionMessage", ADPropertyDefinitionFlags.None, ContentFilterConfigSchema.Defaults.RejectionResponse, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 240)
		}, null, null);

		public static readonly ADPropertyDefinition BypassedRecipients = SharedPropertyDefinitions.BypassedRecipients;

		public static readonly ADPropertyDefinition EncodedPhrases = new ADPropertyDefinition("CustomWeightEntry", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMessageHygieneCustomWeightEntry", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QuarantineMailbox = new ADPropertyDefinition("QuarantineMailbox", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress?), "msExchMessageHygieneQuarantineMailbox", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLQuarantineThreshold = new ADPropertyDefinition("SCLQuarantineThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageHygieneSCLQuarantineThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 9, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLQuarantineEnabled = new ADPropertyDefinition("SCLQuarantineEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(512, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(512, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition SCLDeleteThreshold = new ADPropertyDefinition("SCLDeleteThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageHygieneSCLDeleteThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 9, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLDeleteEnabled = new ADPropertyDefinition("SCLDeleteEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(128, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(128, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition SCLRejectThreshold = new ADPropertyDefinition("SCLRejectThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageHygieneSCLRejectThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 7, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLRejectEnabled = new ADPropertyDefinition("SCLRejectEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MessageHygieneAgentConfigSchema.AgentsFlags
		}, null, ADObject.FlagGetterDelegate(256, MessageHygieneAgentConfigSchema.AgentsFlags), ADObject.FlagSetterDelegate(256, MessageHygieneAgentConfigSchema.AgentsFlags), null, null);

		public static readonly ADPropertyDefinition BypassedSenders = new ADPropertyDefinition("BypassedSenders", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress), "msExchMessageHygieneBypassedSenders", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition BypassedSenderDomains = new ADPropertyDefinition("BypassedSenderDomains", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomainWithSubdomains), "msExchMessageHygieneBypassedSenderDomains", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static class Defaults
		{
			public const int SCLDeleteThreshold = 9;

			public const bool SCLDeleteEnabled = false;

			public const int SCLRejectThreshold = 7;

			public const bool SCLRejectEnabled = true;

			public const int SCLQuarantineThreshold = 9;

			public const bool SCLQuarantineEnabled = false;

			public static readonly AsciiString RejectionResponse = new AsciiString("Message rejected as spam by Content Filtering.");
		}
	}
}
