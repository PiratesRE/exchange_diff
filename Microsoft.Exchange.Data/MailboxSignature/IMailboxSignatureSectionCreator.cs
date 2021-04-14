using System;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal interface IMailboxSignatureSectionCreator
	{
		bool Create(MailboxSignatureSectionType sectionType, out MailboxSignatureSectionMetadata sectionMetadata, out byte[] sectionData);
	}
}
