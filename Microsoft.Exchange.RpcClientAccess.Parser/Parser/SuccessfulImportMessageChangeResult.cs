using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulImportMessageChangeResult : SuccessfulImportMessageChangeResultBase
	{
		internal SuccessfulImportMessageChangeResult(IServerObject serverObject, StoreId messageId) : base(RopId.ImportMessageChange, serverObject, messageId)
		{
		}

		internal SuccessfulImportMessageChangeResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulImportMessageChangeResult Parse(Reader reader)
		{
			return new SuccessfulImportMessageChangeResult(reader);
		}
	}
}
