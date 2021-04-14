using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiModPropsResponse : MapiHttpOperationResponse
	{
		public NspiModPropsResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
		}

		public NspiModPropsResponse(Reader reader) : base(reader)
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
