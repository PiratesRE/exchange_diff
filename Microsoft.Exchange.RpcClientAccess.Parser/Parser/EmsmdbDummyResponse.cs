using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbDummyResponse : MapiHttpOperationResponse
	{
		public EmsmdbDummyResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
		}

		public EmsmdbDummyResponse(Reader reader) : base(reader)
		{
		}
	}
}
