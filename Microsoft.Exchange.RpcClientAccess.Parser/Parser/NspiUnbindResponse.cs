using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiUnbindResponse : MapiHttpOperationResponse
	{
		public NspiUnbindResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
		}

		public NspiUnbindResponse(Reader reader) : base(reader)
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
