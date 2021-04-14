using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal class MailboxSignatureSectionsContainer
	{
		public MailboxSignatureSectionType SectionTypes
		{
			get
			{
				MailboxSignatureSectionType mailboxSignatureSectionType = MailboxSignatureSectionType.None;
				foreach (MailboxSignatureSectionsContainer.SignatureSection signatureSection in this.sections)
				{
					mailboxSignatureSectionType |= signatureSection.Metadata.Type;
				}
				return mailboxSignatureSectionType;
			}
		}

		public static MailboxSignatureSectionsContainer Parse(byte[] buffer, IMailboxSignatureSectionProcessor sectionProcessor)
		{
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer = new MailboxSignatureSectionsContainer();
			mailboxSignatureSectionsContainer.InternalParse(buffer, sectionProcessor);
			return mailboxSignatureSectionsContainer;
		}

		public static MailboxSignatureSectionsContainer Create(MailboxSignatureSectionType sectionsToCreate, IMailboxSignatureSectionCreator sectionCreator)
		{
			MailboxSignatureSectionsContainer mailboxSignatureSectionsContainer = new MailboxSignatureSectionsContainer();
			mailboxSignatureSectionsContainer.InternalCreate(sectionsToCreate, sectionCreator);
			return mailboxSignatureSectionsContainer;
		}

		public byte[] Serialize()
		{
			int num = 0;
			foreach (MailboxSignatureSectionsContainer.SignatureSection signatureSection in this.sections)
			{
				num += 12 + signatureSection.Data.Count;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			foreach (MailboxSignatureSectionsContainer.SignatureSection signatureSection2 in this.sections)
			{
				num2 += signatureSection2.Metadata.Serialize(array, num2);
				Array.Copy(signatureSection2.Data.Array, signatureSection2.Data.Offset, array, num2, signatureSection2.Data.Count);
				num2 += signatureSection2.Data.Count;
			}
			return array;
		}

		public MailboxSignatureSectionsContainer.SignatureSection GetSignatureSection(MailboxSignatureSectionType type)
		{
			int num = this.sections.FindIndex((MailboxSignatureSectionsContainer.SignatureSection e) => e.Metadata.Type == type);
			if (num != -1)
			{
				return this.sections.ElementAt(num);
			}
			return new MailboxSignatureSectionsContainer.SignatureSection(new MailboxSignatureSectionMetadata(MailboxSignatureSectionType.None, 0, 0, 0), default(ArraySegment<byte>));
		}

		public void UpdateSection(MailboxSignatureSectionMetadata metadata, byte[] data)
		{
			int num = this.sections.FindIndex((MailboxSignatureSectionsContainer.SignatureSection e) => e.Metadata.Type == metadata.Type);
			if (num != -1)
			{
				this.sections.RemoveAt(num);
			}
			this.sections.Add(new MailboxSignatureSectionsContainer.SignatureSection(metadata, new ArraySegment<byte>(data)));
		}

		private void InternalParse(byte[] buffer, IMailboxSignatureSectionProcessor sectionProcessor)
		{
			int i = 0;
			while (i < buffer.Length)
			{
				MailboxSignatureSectionMetadata mailboxSignatureSectionMetadata = MailboxSignatureSectionMetadata.Parse(buffer, ref i);
				if (mailboxSignatureSectionMetadata.Length > buffer.Length - i)
				{
					throw new ArgumentException("Metadata declares length past our buffer end.");
				}
				int num = i;
				bool flag = sectionProcessor.Process(mailboxSignatureSectionMetadata, buffer, ref i);
				if (i - num != mailboxSignatureSectionMetadata.Length)
				{
					throw new ArgumentException("Parsed more data than declared in metadata.");
				}
				if (flag)
				{
					if ((from section in this.sections
					select section.Metadata.Type).Contains(mailboxSignatureSectionMetadata.Type))
					{
						throw new ArgumentException("Same section appears more than once.");
					}
					this.sections.Add(new MailboxSignatureSectionsContainer.SignatureSection(mailboxSignatureSectionMetadata, new ArraySegment<byte>(buffer, num, mailboxSignatureSectionMetadata.Length)));
				}
			}
		}

		private void InternalCreate(MailboxSignatureSectionType sectionsToCreate, IMailboxSignatureSectionCreator sectionCreator)
		{
			for (MailboxSignatureSectionType mailboxSignatureSectionType = MailboxSignatureSectionType.BasicInformation; mailboxSignatureSectionType != MailboxSignatureSectionType.None; mailboxSignatureSectionType <<= 1)
			{
				MailboxSignatureSectionMetadata metadata;
				byte[] array;
				if (sectionsToCreate.HasFlag(mailboxSignatureSectionType) && sectionCreator.Create(mailboxSignatureSectionType, out metadata, out array))
				{
					this.sections.Add(new MailboxSignatureSectionsContainer.SignatureSection(metadata, new ArraySegment<byte>(array)));
				}
			}
		}

		public const uint UnifiedSignatureFormat = 102U;

		public const uint XSOSpecificMRSStoreProtocol = 103U;

		public const uint IncrementalMailboxSignatureMappingMetadataMRSStoreProtocol = 104U;

		private List<MailboxSignatureSectionsContainer.SignatureSection> sections = new List<MailboxSignatureSectionsContainer.SignatureSection>();

		internal struct SignatureSection
		{
			internal SignatureSection(MailboxSignatureSectionMetadata metadata, ArraySegment<byte> data)
			{
				this.metadata = metadata;
				this.data = data;
			}

			public MailboxSignatureSectionMetadata Metadata
			{
				get
				{
					return this.metadata;
				}
			}

			public ArraySegment<byte> Data
			{
				get
				{
					return this.data;
				}
			}

			private MailboxSignatureSectionMetadata metadata;

			private ArraySegment<byte> data;
		}
	}
}
