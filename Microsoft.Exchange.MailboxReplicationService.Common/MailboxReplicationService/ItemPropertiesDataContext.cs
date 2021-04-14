using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ItemPropertiesDataContext : DataContext
	{
		public ItemPropertiesDataContext(ItemPropertiesBase props)
		{
			this.props = props;
		}

		public override string ToString()
		{
			return string.Format("ItemProps: {0}", this.props);
		}

		private ItemPropertiesBase props;
	}
}
