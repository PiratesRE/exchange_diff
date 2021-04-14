using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiModLinkAttResponse : MapiHttpOperationResponse
	{
		public NspiModLinkAttResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
		}

		public NspiModLinkAttResponse(Reader reader) : base(reader)
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
