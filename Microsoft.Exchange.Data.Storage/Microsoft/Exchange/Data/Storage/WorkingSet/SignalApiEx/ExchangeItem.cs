using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeItem : Item
	{
		public ExchangeItem(string identifier, string sourceSystem, bool forceDownload, Item item) : base(identifier, sourceSystem, forceDownload)
		{
			if (identifier == null)
			{
				throw new ArgumentException("Identifier can't be null");
			}
			this.Item = item;
		}

		internal ExchangeItem(BinaryReader reader, IUnpacker unpacker) : base(reader)
		{
			string text = reader.ReadString();
			if (!text.Equals("n"))
			{
				this.AttachContentId = new Guid(text);
				unpacker.SetContent(this, this.AttachContentId);
			}
		}

		public Item Item { get; set; }

		internal Guid AttachContentId { get; private set; }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (obj != null)
			{
				ExchangeItem exchangeItem = obj as ExchangeItem;
				if (exchangeItem == null)
				{
					return false;
				}
				if (this.Item != null)
				{
					return this.Item.Equals(exchangeItem.Item);
				}
				if (exchangeItem.Item != null)
				{
					return false;
				}
			}
			return true;
		}

		internal override bool WriteObject(BinaryWriter writer)
		{
			base.WriteObject(writer);
			if (this.HasAttachment())
			{
				this.AttachContentId = Guid.NewGuid();
				writer.Write(this.AttachContentId.ToString());
				return true;
			}
			writer.Write("n");
			return false;
		}

		internal override Item.ItemTypeCode GetItemTypeCode()
		{
			return 2;
		}

		internal override bool HasAttachment()
		{
			return this.Item != null;
		}
	}
}
