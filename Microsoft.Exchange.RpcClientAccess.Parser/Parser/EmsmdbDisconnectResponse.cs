using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbDisconnectResponse : MapiHttpOperationResponse
	{
		public EmsmdbDisconnectResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
		}

		public EmsmdbDisconnectResponse(Reader reader) : base(reader)
		{
			base.ParseAuxiliaryBuffer(reader);
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			base.SerializeAuxiliaryBuffer(writer);
		}
	}
}
