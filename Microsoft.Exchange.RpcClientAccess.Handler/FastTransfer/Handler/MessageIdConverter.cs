using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class MessageIdConverter : IdConverter
	{
		internal MessageIdConverter() : base(PropertyTag.Mid, CoreItemSchema.Id)
		{
		}

		protected override long CreateClientId(StoreSession session, StoreId id)
		{
			return session.IdConverter.GetMidFromMessageId(StoreId.GetStoreObjectId(id));
		}
	}
}
