using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ItemIdProperty : ItemIdPropertyBase
	{
		private ItemIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ItemIdProperty CreateCommand(CommandContext commandContext)
		{
			return new ItemIdProperty(commandContext);
		}

		internal override ServiceObjectId CreateServiceObjectId(string id, string changeKey)
		{
			return new ItemId
			{
				Id = id,
				ChangeKey = changeKey
			};
		}

		internal override Array CreateServiceObjectidArray(List<ConcatenatedIdAndChangeKey> ids)
		{
			ItemId[] array = new ItemId[ids.Count];
			for (int i = 0; i < ids.Count; i++)
			{
				array[i] = new ItemId
				{
					Id = ids[i].Id,
					ChangeKey = ids[i].ChangeKey
				};
			}
			return array;
		}
	}
}
