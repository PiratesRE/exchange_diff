using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfRequestIdAuxiliaryBlock : AuxiliaryBlock
	{
		public PerfRequestIdAuxiliaryBlock(ushort blockSessionId, ushort blockRequestId) : base(1, AuxiliaryBlockTypes.PerfRequestId)
		{
			this.blockSessionId = blockSessionId;
			this.blockRequestId = blockRequestId;
		}

		internal PerfRequestIdAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.blockSessionId = reader.ReadUInt16();
			this.blockRequestId = reader.ReadUInt16();
		}

		public ushort BlockSessionId
		{
			get
			{
				return this.blockSessionId;
			}
		}

		public ushort BlockRequestId
		{
			get
			{
				return this.blockRequestId;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.blockSessionId);
			writer.WriteUInt16(this.blockRequestId);
		}

		private readonly ushort blockSessionId;

		private readonly ushort blockRequestId;
	}
}
