using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulImportMessageChangePartialResult : SuccessfulImportMessageChangeResultBase
	{
		internal SuccessfulImportMessageChangePartialResult(IServerObject serverObject, StoreId messageId) : base(RopId.ImportMessageChangePartial, serverObject, messageId)
		{
		}

		internal SuccessfulImportMessageChangePartialResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulImportMessageChangePartialResult Parse(Reader reader)
		{
			return new SuccessfulImportMessageChangePartialResult(reader);
		}
	}
}
