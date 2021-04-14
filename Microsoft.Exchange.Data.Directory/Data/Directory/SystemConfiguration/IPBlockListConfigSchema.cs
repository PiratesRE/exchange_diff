using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class IPBlockListConfigSchema : MessageHygieneAgentConfigSchema
	{
		internal static readonly AsciiString DefaultMachineRejectionResponse = new AsciiString("External client with IP address {0} does not have permissions to submit to this server. Visit http://support.microsoft.com/kb/928123 for more information.");

		internal static readonly AsciiString DefaultStaticRejectionResponse = new AsciiString("External client with IP address {0} does not have permissions to submit to this server.");

		public static readonly ADPropertyDefinition MachineEntryRejectionResponse = new ADPropertyDefinition("MachineEntryRejectionResponse", ExchangeObjectVersion.Exchange2007, typeof(AsciiString), "msExchMessageHygieneMachineGeneratedRejectionResponse", ADPropertyDefinitionFlags.None, IPBlockListConfigSchema.DefaultMachineRejectionResponse, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 240)
		}, null, null);

		public static readonly ADPropertyDefinition StaticEntryRejectionResponse = new ADPropertyDefinition("StaticEntryRejectionResponse", ExchangeObjectVersion.Exchange2007, typeof(AsciiString), "msExchMessageHygieneStaticEntryRejectionResponse", ADPropertyDefinitionFlags.None, IPBlockListConfigSchema.DefaultStaticRejectionResponse, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 240)
		}, null, null);
	}
}
