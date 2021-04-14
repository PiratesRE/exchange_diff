using System;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal class NullMailboxSignatureSectionProcessor : IMailboxSignatureSectionProcessor
	{
		internal static IMailboxSignatureSectionProcessor Instance
		{
			get
			{
				return NullMailboxSignatureSectionProcessor.instance;
			}
		}

		public bool Process(MailboxSignatureSectionMetadata sectionMetadata, byte[] buffer, ref int offset)
		{
			offset += sectionMetadata.Length;
			return true;
		}

		private static IMailboxSignatureSectionProcessor instance = new NullMailboxSignatureSectionProcessor();
	}
}
