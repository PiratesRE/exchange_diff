using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailboxItem : DeliverableItem
	{
		public MailboxItem(MailRecipient recipient) : base(recipient)
		{
		}

		public override ADObjectId Database
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Database");
			}
		}

		public bool InStorageGroup
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Database").Depth >= 14;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return base.GetProperty<Guid>("Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid");
			}
		}

		public string ServerName
		{
			get
			{
				return base.GetProperty<string>("Microsoft.Exchange.Transport.DirectoryData.ServerName");
			}
		}
	}
}
