using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferDestinationCopyOperationConfigureResult : RopResult
	{
		internal SuccessfulFastTransferDestinationCopyOperationConfigureResult(IServerObject serverObject) : base(RopId.FastTransferDestinationCopyOperationConfigure, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulFastTransferDestinationCopyOperationConfigureResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferDestinationCopyOperationConfigureResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferDestinationCopyOperationConfigureResult(reader);
		}
	}
}
