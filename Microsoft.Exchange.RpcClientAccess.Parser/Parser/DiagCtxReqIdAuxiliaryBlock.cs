using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class DiagCtxReqIdAuxiliaryBlock : AuxiliaryBlock
	{
		public DiagCtxReqIdAuxiliaryBlock(int requestId) : base(1, AuxiliaryBlockTypes.DiagCtxReqId)
		{
			this.requestId = requestId;
		}

		internal DiagCtxReqIdAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.requestId = reader.ReadInt32();
		}

		public int RequestId
		{
			get
			{
				return this.requestId;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.requestId);
		}

		private readonly int requestId;
	}
}
