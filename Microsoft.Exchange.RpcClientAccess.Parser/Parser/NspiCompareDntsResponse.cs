using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiCompareDntsResponse : MapiHttpOperationResponse
	{
		public NspiCompareDntsResponse(uint returnCode, int result, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.result = result;
		}

		public NspiCompareDntsResponse(Reader reader) : base(reader)
		{
			this.result = reader.ReadInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public int Result
		{
			get
			{
				return this.result;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.result);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly int result;
	}
}
