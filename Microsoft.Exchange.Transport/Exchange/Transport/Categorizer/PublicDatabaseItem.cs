using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class PublicDatabaseItem : DeliverableItem
	{
		public PublicDatabaseItem(MailRecipient recipient) : base(recipient)
		{
		}

		public string DistinguishedName
		{
			get
			{
				return base.GetProperty<string>("Microsoft.Exchange.Transport.DirectoryData.DistinguishedName");
			}
		}

		public override ADObjectId Database
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Id");
			}
		}

		public override string HomeMdbDN
		{
			get
			{
				return this.DistinguishedName;
			}
		}
	}
}
