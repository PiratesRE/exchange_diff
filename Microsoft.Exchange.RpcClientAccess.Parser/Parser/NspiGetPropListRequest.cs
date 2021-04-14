using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetPropListRequest : MapiHttpRequest
	{
		public NspiGetPropListRequest(NspiGetPropListFlags flags, int ephemeralId, uint codePage, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.ephemeralId = ephemeralId;
			this.codePage = codePage;
		}

		public NspiGetPropListRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiGetPropListFlags)reader.ReadUInt32();
			this.ephemeralId = reader.ReadInt32();
			this.codePage = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiGetPropListFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public int EphemeralId
		{
			get
			{
				return this.ephemeralId;
			}
		}

		public uint CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteInt32(this.ephemeralId);
			writer.WriteUInt32(this.codePage);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiGetPropListFlags flags;

		private readonly int ephemeralId;

		private readonly uint codePage;
	}
}
