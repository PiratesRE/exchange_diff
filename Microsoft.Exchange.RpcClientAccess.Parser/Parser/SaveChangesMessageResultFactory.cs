using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SaveChangesMessageResultFactory : StandardResultFactory
	{
		internal SaveChangesMessageResultFactory(byte realHandleTableIndex) : base(RopId.SaveChangesMessage)
		{
			this.realHandleTableIndex = realHandleTableIndex;
		}

		public RopResult CreateSuccessfulResult(StoreId messageId)
		{
			return new SuccessfulSaveChangesMessageResult(this.realHandleTableIndex, messageId);
		}

		private readonly byte realHandleTableIndex;
	}
}
