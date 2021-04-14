using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MailboxSignatureConverter
	{
		public static byte[] ConvertTenantHint(byte[] originalSignature, MailboxSignatureFlags originalSignatureFlags, TenantPartitionHint tenantHint)
		{
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer;
			if (originalSignatureFlags == MailboxSignatureFlags.GetLegacy)
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Create(MailboxSignatureSectionType.BasicInformation, new MailboxSignatureConverter.MailboxBasicInformationSectionCreator(originalSignature));
			}
			else
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Parse(originalSignature, NullMailboxSignatureSectionProcessor.Instance);
			}
			if (tenantHint == null)
			{
				tenantHint = TenantPartitionHint.FromOrganizationId(OrganizationId.ForestWideOrgId);
			}
			byte[] persistablePartitionHint = tenantHint.GetPersistablePartitionHint();
			int num = TenantHintHelper.SerializeTenantHint(persistablePartitionHint, null, 0);
			byte[] array = new byte[num];
			TenantHintHelper.SerializeTenantHint(persistablePartitionHint, array, 0);
			mailboxSignatureSectionsContainer.UpdateSection(new MailboxSignatureSectionMetadata(MailboxSignatureSectionType.TenantHint, 1, 1, persistablePartitionHint.Length), array);
			return mailboxSignatureSectionsContainer.Serialize();
		}

		public static byte[] ConvertPartitionInformation(byte[] originalSignature, MailboxSignatureFlags originalSignatureFlags, PartitionInformation partitionInformation)
		{
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer;
			if (originalSignatureFlags == MailboxSignatureFlags.GetLegacy)
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Create(MailboxSignatureSectionType.BasicInformation, new MailboxSignatureConverter.MailboxBasicInformationSectionCreator(originalSignature));
			}
			else
			{
				mailboxSignatureSectionsContainer = MailboxSignatureSectionsContainer.Parse(originalSignature, NullMailboxSignatureSectionProcessor.Instance);
			}
			int num = partitionInformation.Serialize(null, 0);
			byte[] array = new byte[num];
			partitionInformation.Serialize(array, 0);
			mailboxSignatureSectionsContainer.UpdateSection(new MailboxSignatureSectionMetadata(MailboxSignatureSectionType.PartitionInformation, 1, 1, array.Length), array);
			return mailboxSignatureSectionsContainer.Serialize();
		}

		public static byte[] ExtractMailboxBasicInfo(byte[] signatureBlob)
		{
			MailboxSignatureConverter.MailboxBasicInformationExtractor mailboxBasicInformationExtractor = new MailboxSignatureConverter.MailboxBasicInformationExtractor();
			MailboxSignatureSectionsContainer.Parse(signatureBlob, mailboxBasicInformationExtractor);
			return mailboxBasicInformationExtractor.MailboxBasicInformation;
		}

		public static byte[] CreatePublicFolderMailboxBasicInformation()
		{
			int num = 32 + MailboxSignatureConverter.defaultPublicFolderGlobCounts.Length;
			byte[] array = new byte[num];
			Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, array, 0, 16);
			Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, array, 16, 16);
			Buffer.BlockCopy(MailboxSignatureConverter.defaultPublicFolderGlobCounts, 0, array, 32, MailboxSignatureConverter.defaultPublicFolderGlobCounts.Length);
			return array;
		}

		private static byte[] defaultPublicFolderGlobCounts = new byte[]
		{
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			4,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			3,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			7,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			5,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			6,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			1
		};

		private class MailboxBasicInformationSectionCreator : IMailboxSignatureSectionCreator
		{
			public MailboxBasicInformationSectionCreator(byte[] mailboxBasicInformation)
			{
				this.mailboxBasicInformation = mailboxBasicInformation;
			}

			bool IMailboxSignatureSectionCreator.Create(MailboxSignatureSectionType sectionType, out MailboxSignatureSectionMetadata sectionMetadata, out byte[] sectionData)
			{
				sectionMetadata = new MailboxSignatureSectionMetadata(MailboxSignatureSectionType.BasicInformation, 1, 1, this.mailboxBasicInformation.Length);
				sectionData = this.mailboxBasicInformation;
				return true;
			}

			private readonly byte[] mailboxBasicInformation;
		}

		private class MailboxBasicInformationExtractor : IMailboxSignatureSectionProcessor
		{
			public byte[] MailboxBasicInformation { get; private set; }

			bool IMailboxSignatureSectionProcessor.Process(MailboxSignatureSectionMetadata sectionMetadata, byte[] buffer, ref int offset)
			{
				if (sectionMetadata.Type == MailboxSignatureSectionType.BasicInformation)
				{
					this.MailboxBasicInformation = new byte[sectionMetadata.Length];
					Array.Copy(buffer, offset, this.MailboxBasicInformation, 0, sectionMetadata.Length);
				}
				offset += sectionMetadata.Length;
				return true;
			}
		}
	}
}
