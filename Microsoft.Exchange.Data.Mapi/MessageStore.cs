using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class MessageStore : MapiObject
	{
		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageStore>(this);
		}

		internal override MapiProp RawMapiEntry
		{
			get
			{
				throw new NotImplementedException("The property RawMapiEntry is not implemented.");
			}
		}

		internal override void Read(bool keepUnmanagedResources)
		{
			throw new NotImplementedException("The method Read is not implemented.");
		}

		internal override void Save(bool keepUnmanagedResources)
		{
			throw new NotImplementedException("The method Save is not implemented.");
		}

		public MessageStore()
		{
		}

		internal MessageStore(MessageStoreId mapiObjectId, MapiSession mapiSession) : base(mapiObjectId, mapiSession)
		{
		}
	}
}
