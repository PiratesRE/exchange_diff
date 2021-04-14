using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiBindRequest : MapiHttpRequest
	{
		public NspiBindRequest(NspiBindFlags flags, NspiState state, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
		}

		public NspiBindRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiBindFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiBindFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiBindFlags flags;

		private readonly NspiState state;
	}
}
