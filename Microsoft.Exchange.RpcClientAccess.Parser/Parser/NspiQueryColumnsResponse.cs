using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiQueryColumnsResponse : MapiHttpOperationResponse
	{
		public NspiQueryColumnsResponse(uint returnCode, PropertyTag[] columns, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.columns = columns;
		}

		public NspiQueryColumnsResponse(Reader reader) : base(reader)
		{
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly PropertyTag[] columns;
	}
}
