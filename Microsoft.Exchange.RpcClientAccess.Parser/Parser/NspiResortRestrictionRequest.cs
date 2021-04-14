using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiResortRestrictionRequest : MapiHttpRequest
	{
		public NspiResortRestrictionRequest(NspiResortRestrictionFlags flags, NspiState state, int[] ephemeralIds, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.ephemeralIds = ephemeralIds;
		}

		public NspiResortRestrictionRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiResortRestrictionFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.ephemeralIds = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiResortRestrictionFlags Flags
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

		public int[] EphemeralIds
		{
			get
			{
				return this.ephemeralIds;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableSizeAndIntegerArray(this.ephemeralIds, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiResortRestrictionFlags flags;

		private readonly NspiState state;

		private readonly int[] ephemeralIds;
	}
}
