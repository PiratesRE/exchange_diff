using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class InvalidItem : DirectoryItem
	{
		public InvalidItem(MailRecipient recipient) : base(recipient)
		{
		}

		public override void PreProcess(Expansion expansion)
		{
			base.FailRecipient(AckReason.InvalidDirectoryObject);
		}
	}
}
