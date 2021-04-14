using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache
{
	internal class RecipientInfoCacheIntProperty : RecipientInfoCacheProperty, IIntegerProperty, IProperty
	{
		public RecipientInfoCacheIntProperty(RecipientInfoCacheEntryElements element)
		{
			if (element != RecipientInfoCacheEntryElements.WeightedRank)
			{
				throw new ArgumentException("The element " + element + " is not an int type!");
			}
			base.State = PropertyState.Modified;
			this.element = element;
		}

		public int IntegerData
		{
			get
			{
				int num = -1;
				RecipientInfoCacheEntryElements recipientInfoCacheEntryElements = this.element;
				if (recipientInfoCacheEntryElements == RecipientInfoCacheEntryElements.WeightedRank)
				{
					num = (int)((this.entry.DateTimeTicks & 18014398501093376L) >> 23);
				}
				if (num == -1)
				{
					throw new ArgumentException("The element " + this.element + " is not an int type!");
				}
				return num;
			}
		}

		public override void Bind(RecipientInfoCacheEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("Entry is null!");
			}
			this.entry = entry;
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			throw new NotImplementedException("Can't set any Int value! Element: " + this.element);
		}

		private const long BitMask = 18014398501093376L;

		private RecipientInfoCacheEntryElements element;

		private RecipientInfoCacheEntry entry;
	}
}
