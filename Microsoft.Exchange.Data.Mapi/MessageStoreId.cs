using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class MessageStoreId : MapiObjectId
	{
		public MessageStoreId()
		{
		}

		public MessageStoreId(byte[] bytes) : base(bytes)
		{
		}

		public MessageStoreId(MapiEntryId entryId) : base(entryId)
		{
		}
	}
}
