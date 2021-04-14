using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PostSavePropertyCollection : Dictionary<StoreObjectId, List<IPostSavePropertyCommand>>
	{
		public void Add(StoreObjectId itemId, IPostSavePropertyCommand propertyCommand)
		{
			List<IPostSavePropertyCommand> list;
			if (!base.ContainsKey(itemId))
			{
				list = new List<IPostSavePropertyCommand>();
				base[itemId] = list;
			}
			else
			{
				list = base[itemId];
			}
			list.Add(propertyCommand);
		}
	}
}
