using System;
using Microsoft.Exchange.Data.MailboxSignature;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal interface IMailboxSignatureParser
	{
		bool ParseMailboxBasicInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxMappingMetadata(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxNamedPropertyMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxReplidGuidMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxTenantHint(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxTypeVersion(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParsePartitionInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		bool ParseMailboxUnknownSection(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);
	}
}
