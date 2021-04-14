using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiUpdateStatRequest : MapiHttpRequest
	{
		public NspiUpdateStatRequest(NspiUpdateStatFlags flags, NspiState state, bool deltaRequested, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.deltaRequested = deltaRequested;
		}

		public NspiUpdateStatRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiUpdateStatFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.deltaRequested = reader.ReadBool();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiUpdateStatFlags Flags
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

		public bool DeltaRequested
		{
			get
			{
				return this.deltaRequested;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteBool(this.deltaRequested);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiUpdateStatFlags flags;

		private readonly NspiState state;

		private readonly bool deltaRequested;
	}
}
