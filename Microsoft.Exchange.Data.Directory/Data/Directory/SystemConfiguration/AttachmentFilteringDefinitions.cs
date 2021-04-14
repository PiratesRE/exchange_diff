using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AttachmentFilteringDefinitions
	{
		public const string DefaultRejectResponse = "Message rejected due to unacceptable attachments";

		public static readonly PropertyDefinitionConstraint[] RejectResponseConstraints = new PropertyDefinitionConstraint[]
		{
			new SmtpResponseConstraint(),
			new StringLengthConstraint(1, 240),
			new AsciiCharactersOnlyConstraint()
		};
	}
}
