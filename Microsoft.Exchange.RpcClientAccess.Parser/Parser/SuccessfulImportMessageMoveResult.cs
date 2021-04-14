using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulImportMessageMoveResult : RopResult
	{
		internal SuccessfulImportMessageMoveResult(StoreId messageId) : base(RopId.ImportMessageMove, ErrorCode.None, null)
		{
			this.messageId = messageId;
		}

		internal SuccessfulImportMessageMoveResult(Reader reader) : base(reader)
		{
			this.messageId = StoreId.Parse(reader);
		}

		public StoreId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal static SuccessfulImportMessageMoveResult Parse(Reader reader)
		{
			return new SuccessfulImportMessageMoveResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.messageId.Serialize(writer);
		}

		private readonly StoreId messageId;
	}
}
