using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SuccessfulImportMessageChangeResultBase : RopResult
	{
		internal SuccessfulImportMessageChangeResultBase(RopId ropId, IServerObject serverObject, StoreId messageId) : base(ropId, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.messageId = messageId;
		}

		internal SuccessfulImportMessageChangeResultBase(Reader reader) : base(reader)
		{
			this.messageId = StoreId.Parse(reader);
		}

		internal StoreId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.messageId.Serialize(writer);
		}

		private readonly StoreId messageId;
	}
}
