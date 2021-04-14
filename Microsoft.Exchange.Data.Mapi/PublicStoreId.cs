using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class PublicStoreId : MessageStoreId
	{
		public PublicStoreId()
		{
		}

		public PublicStoreId(byte[] bytes) : base(bytes)
		{
		}
	}
}
