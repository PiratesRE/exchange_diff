using System;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal interface IMailboxSignatureSectionProcessor
	{
		bool Process(MailboxSignatureSectionMetadata sectionMetadata, byte[] buffer, ref int offset);
	}
}
