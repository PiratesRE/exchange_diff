using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[CollectionDataContract]
	public class UserSkippedItemList : List<UserSkippedItem>
	{
		public UserSkippedItemList()
		{
		}

		public UserSkippedItemList(ICollection<MigrationUserSkippedItem> skippedItems) : base((skippedItems == null) ? 0 : skippedItems.Count)
		{
			if (skippedItems != null)
			{
				foreach (MigrationUserSkippedItem skippedItem in skippedItems)
				{
					base.Add(new UserSkippedItem(skippedItem));
				}
			}
		}
	}
}
