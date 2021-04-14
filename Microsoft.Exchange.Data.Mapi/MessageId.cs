using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class MessageId : MapiObjectId
	{
		public MessageId()
		{
		}

		public MessageId(byte[] bytes) : base(bytes)
		{
		}

		internal MessageId(MapiEntryId entryId) : base(entryId)
		{
		}

		public override string ToString()
		{
			return base.MapiEntryId.ToString();
		}
	}
}
