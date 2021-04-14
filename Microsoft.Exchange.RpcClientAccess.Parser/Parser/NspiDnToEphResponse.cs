using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiDnToEphResponse : MapiHttpOperationResponse
	{
		public NspiDnToEphResponse(uint returnCode, int[] ephemeralIds, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.ephemeralIds = ephemeralIds;
		}

		public NspiDnToEphResponse(Reader reader) : base(reader)
		{
			this.ephemeralIds = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
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
			writer.WriteNullableSizeAndIntegerArray(this.ephemeralIds, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly int[] ephemeralIds;
	}
}
