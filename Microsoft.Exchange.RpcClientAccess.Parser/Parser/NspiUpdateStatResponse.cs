using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiUpdateStatResponse : MapiHttpOperationResponse
	{
		public NspiUpdateStatResponse(uint returnCode, NspiState state, int? delta, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.state = state;
			this.delta = delta;
		}

		public NspiUpdateStatResponse(Reader reader) : base(reader)
		{
			this.state = reader.ReadNspiState();
			this.delta = reader.ReadNullableInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public int? Delta
		{
			get
			{
				return this.delta;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteNspiState(this.state);
			writer.WriteNullableInt32(this.delta);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiState state;

		private readonly int? delta;
	}
}
