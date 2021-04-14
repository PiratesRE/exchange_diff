using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiResortRestrictionResponse : MapiHttpOperationResponse
	{
		public NspiResortRestrictionResponse(uint returnCode, NspiState state, int[] ephemeralIds, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.state = state;
			this.ephemeralIds = ephemeralIds;
		}

		public NspiResortRestrictionResponse(Reader reader) : base(reader)
		{
			this.state = reader.ReadNspiState();
			this.ephemeralIds = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
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
			base.Serialize(writer);
			writer.WriteNspiState(this.state);
			writer.WriteNullableSizeAndIntegerArray(this.ephemeralIds, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiState state;

		private readonly int[] ephemeralIds;
	}
}
