using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PoolItem<TItem> where TItem : class
	{
		internal PoolItem(TItem item, uint id)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			this.item = item;
			this.id = id;
			this.creationTime = (this.lastUsedTime = ExDateTime.UtcNow);
		}

		internal TItem Item
		{
			get
			{
				return this.item;
			}
		}

		internal uint ID
		{
			get
			{
				return this.id;
			}
		}

		internal ExDateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		internal ExDateTime LastUsedTime
		{
			get
			{
				return this.lastUsedTime;
			}
			set
			{
				this.lastUsedTime = value;
			}
		}

		public override bool Equals(object obj)
		{
			PoolItem<TItem> poolItem = obj as PoolItem<TItem>;
			if (poolItem != null && poolItem.ID == this.ID)
			{
				TItem titem = poolItem.Item;
				if (titem.Equals(this.Item) && poolItem.LastUsedTime == this.LastUsedTime)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.id.GetHashCode();
			TItem titem = this.item;
			return hashCode | titem.GetHashCode() | this.lastUsedTime.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ID:{0};LastUsedTime:{1};Item:{2}", new object[]
			{
				this.id,
				this.lastUsedTime,
				this.item
			});
		}

		private readonly TItem item;

		private readonly uint id;

		private readonly ExDateTime creationTime;

		private ExDateTime lastUsedTime;
	}
}
