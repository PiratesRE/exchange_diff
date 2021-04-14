using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetSpecialTableResponse : MapiHttpOperationResponse
	{
		public NspiGetSpecialTableResponse(uint returnCode, uint codePage, uint? version, PropertyValue[][] propertyValues, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.codePage = codePage;
			this.version = version;
			this.propertyValues = propertyValues;
		}

		public NspiGetSpecialTableResponse(Reader reader) : base(reader)
		{
			this.codePage = reader.ReadUInt32();
			this.version = reader.ReadNullableUInt32();
			Encoding asciiEncoding;
			if (!String8Encodings.TryGetEncoding((int)this.codePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			this.propertyValues = reader.ReadNullableCountAndPropertyValueListList(asciiEncoding, WireFormatStyle.Nspi);
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public uint? Version
		{
			get
			{
				return this.version;
			}
		}

		public PropertyValue[][] PropertyValues
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
			writer.WriteNullableUInt32(this.version);
			writer.WriteNullableCountAndPropertyValueListList(this.propertyValues, asciiEncoding, WireFormatStyle.Nspi);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint codePage;

		private readonly uint? version;

		private readonly PropertyValue[][] propertyValues;
	}
}
