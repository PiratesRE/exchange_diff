using System;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal abstract class MailboxSignatureParserBase : IMailboxSignatureParser, IMailboxSignatureSectionProcessor
	{
		public bool Process(MailboxSignatureSectionMetadata sectionMetadata, byte[] buffer, ref int offset)
		{
			MailboxSignatureSectionType type = sectionMetadata.Type;
			if (type <= MailboxSignatureSectionType.TenantHint)
			{
				switch (type)
				{
				case MailboxSignatureSectionType.BasicInformation:
					return this.ParseMailboxBasicInformation(sectionMetadata, buffer, ref offset);
				case MailboxSignatureSectionType.MappingMetadata:
					return this.ParseMailboxMappingMetadata(sectionMetadata, buffer, ref offset);
				case MailboxSignatureSectionType.BasicInformation | MailboxSignatureSectionType.MappingMetadata:
					break;
				case MailboxSignatureSectionType.NamedPropertyMapping:
					return this.ParseMailboxNamedPropertyMapping(sectionMetadata, buffer, ref offset);
				default:
					if (type == MailboxSignatureSectionType.ReplidGuidMapping)
					{
						return this.ParseMailboxReplidGuidMapping(sectionMetadata, buffer, ref offset);
					}
					if (type == MailboxSignatureSectionType.TenantHint)
					{
						return this.ParseMailboxTenantHint(sectionMetadata, buffer, ref offset);
					}
					break;
				}
			}
			else if (type <= MailboxSignatureSectionType.MailboxTypeVersion)
			{
				if (type == MailboxSignatureSectionType.MailboxShape)
				{
					return this.ParseMailboxShape(sectionMetadata, buffer, ref offset);
				}
				if (type == MailboxSignatureSectionType.MailboxTypeVersion)
				{
					return this.ParseMailboxTypeVersion(sectionMetadata, buffer, ref offset);
				}
			}
			else
			{
				if (type == MailboxSignatureSectionType.PartitionInformation)
				{
					return this.ParsePartitionInformation(sectionMetadata, buffer, ref offset);
				}
				if (type == MailboxSignatureSectionType.UserInformation)
				{
					return this.ParseUserInformation(sectionMetadata, buffer, ref offset);
				}
			}
			return this.ParseMailboxUnknownSection(sectionMetadata, buffer, ref offset);
		}

		public abstract bool ParseMailboxBasicInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxMappingMetadata(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxNamedPropertyMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxReplidGuidMapping(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxTenantHint(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxShape(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseMailboxTypeVersion(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParsePartitionInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public abstract bool ParseUserInformation(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset);

		public bool ParseMailboxUnknownSection(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			MailboxSignatureParserBase.Skip(mailboxSignatureSectionMetadata, buffer, ref offset);
			return false;
		}

		protected static void Skip(MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata, byte[] buffer, ref int offset)
		{
			if (offset < 0 || offset > buffer.Length)
			{
				throw new InvalidParameterException((LID)33608U, "Buffer too small.");
			}
			int num = offset + mailboxSignatureSectionMetadata.Length;
			if (num < offset || num > buffer.Length)
			{
				throw new CorruptDataException((LID)49992U, "The new offset is invalid.");
			}
			offset = num;
		}
	}
}
