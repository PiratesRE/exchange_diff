using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetPropsResponse : MapiHttpOperationResponse
	{
		public NspiGetPropsResponse(uint returnCode, uint codePage, PropertyValue[] propertyValues, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.codePage = codePage;
			this.propertyValues = propertyValues;
		}

		public NspiGetPropsResponse(Reader reader) : base(reader)
		{
			this.codePage = reader.ReadUInt32();
			Encoding asciiEncoding;
			if (!String8Encodings.TryGetEncoding((int)this.codePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			this.propertyValues = reader.ReadNullableCountAndPropertyValueList(asciiEncoding, WireFormatStyle.Nspi);
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public PropertyValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			Encoding asciiEncoding;
			if (!String8Encodings.TryGetEncoding((int)this.codePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			writer.WriteUInt32(this.codePage);
			writer.WriteNullableCountAndPropertyValueList(this.propertyValues, asciiEncoding, WireFormatStyle.Nspi);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint codePage;

		private readonly PropertyValue[] propertyValues;
	}
}
