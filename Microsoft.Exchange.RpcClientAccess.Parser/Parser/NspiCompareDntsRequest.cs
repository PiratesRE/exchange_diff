using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiCompareDntsRequest : MapiHttpRequest
	{
		public NspiCompareDntsRequest(NspiCompareDNTsFlags flags, NspiState state, int ephemeralId1, int ephemeralId2, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.ephemeralId1 = ephemeralId1;
			this.ephemeralId2 = ephemeralId2;
		}

		public NspiCompareDntsRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiCompareDNTsFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.ephemeralId1 = reader.ReadInt32();
			this.ephemeralId2 = reader.ReadInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiCompareDNTsFlags Flags
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

		public int EphemeralId1
		{
			get
			{
				return this.ephemeralId1;
			}
		}

		public int EphemeralId2
		{
			get
			{
				return this.ephemeralId2;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteInt32(this.ephemeralId1);
			writer.WriteInt32(this.ephemeralId2);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiCompareDNTsFlags flags;

		private readonly NspiState state;

		private readonly int ephemeralId1;

		private readonly int ephemeralId2;
	}
}
