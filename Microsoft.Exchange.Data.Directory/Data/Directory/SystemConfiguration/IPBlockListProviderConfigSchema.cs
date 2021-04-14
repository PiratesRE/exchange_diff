using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class IPBlockListProviderConfigSchema : MessageHygieneAgentConfigSchema
	{
		public static readonly PropertyDefinition BypassedRecipients = SharedPropertyDefinitions.BypassedRecipients;
	}
}
