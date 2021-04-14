using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbDisconnectRequest : MapiHttpRequest
	{
		public EmsmdbDisconnectRequest(ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
		}

		public EmsmdbDisconnectRequest(Reader reader) : base(reader)
		{
			base.ParseAuxiliaryBuffer(reader);
		}

		public override void Serialize(Writer writer)
		{
			base.SerializeAuxiliaryBuffer(writer);
		}
	}
}
