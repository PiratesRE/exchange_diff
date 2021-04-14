using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SignalEx : Signal
	{
		internal SignalEx(BinaryReader reader, IUnpacker unpacker) : base(reader, unpacker)
		{
			this.disposed = false;
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (base.Actor != null)
					{
						ExchangeItem exchangeItem = base.Actor as ExchangeItem;
						if (exchangeItem != null && exchangeItem.Item != null)
						{
							exchangeItem.Item.Dispose();
						}
						base.Actor = null;
					}
					if (base.Action != null)
					{
						ExchangeItem exchangeItem2 = base.Action.Item as ExchangeItem;
						if (exchangeItem2 != null && exchangeItem2.Item != null)
						{
							exchangeItem2.Item.Dispose();
						}
						base.Action = null;
					}
					base.ItemToDelete = null;
				}
				this.disposed = true;
			}
		}

		private bool disposed;
	}
}
