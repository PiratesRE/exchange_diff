using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetPropListResponse : MapiHttpOperationResponse
	{
		public NspiGetPropListResponse(uint returnCode, PropertyTag[] propertyTags, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.propertyTags = propertyTags;
		}

		public NspiGetPropListResponse(Reader reader) : base(reader)
		{
			this.propertyTags = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public PropertyTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteNullableCountAndPropertyTagArray(this.propertyTags, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly PropertyTag[] propertyTags;
	}
}
