using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class DeliverableItem : RestrictedItem
	{
		public DeliverableItem(MailRecipient recipient) : base(recipient)
		{
		}

		public abstract ADObjectId Database { get; }

		public virtual string HomeMdbDN
		{
			get
			{
				ADObjectId database = this.Database;
				if (database == null)
				{
					return null;
				}
				return database.DistinguishedName;
			}
		}

		public string HomeMdbName
		{
			get
			{
				ADObjectId database = this.Database;
				if (database != null)
				{
					return database.Name;
				}
				return string.Empty;
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return base.GetProperty<string>("Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN");
			}
		}
	}
}
