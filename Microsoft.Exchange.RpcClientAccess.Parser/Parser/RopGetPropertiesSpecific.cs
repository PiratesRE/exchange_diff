using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetPropertiesSpecific : InputRop
	{
		internal PropertyTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
		}

		internal override RopId RopId
		{
			get
			{
				return RopId.GetPropertiesSpecific;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetPropertiesSpecific();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new GetPropertiesSpecificResultFactory(outputBuffer.Count, connection.String8Encoding);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.streamLimit = reader.ReadUInt16();
			this.flags = ((reader.ReadUInt16() == 0) ? GetPropertiesFlags.None : ((GetPropertiesFlags)int.MinValue));
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			GetPropertiesSpecificResultFactory resultFactory = new GetPropertiesSpecificResultFactory(outputBuffer.Count, serverObject.String8Encoding);
			this.result = ropHandler.GetPropertiesSpecific(serverObject, this.streamLimit, this.flags, this.propertyTags, resultFactory);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.streamLimit = streamLimit;
			this.flags = flags;
			this.propertyTags = propertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.streamLimit);
			writer.WriteUInt16((this.flags == GetPropertiesFlags.None) ? 0 : 1);
			writer.WriteCountAndPropertyTagArray(this.PropertyTags, FieldLength.WordSize);
		}

		internal void SetParseOutputData(PropertyTag[] parserPropertyTags)
		{
			Util.ThrowOnNullArgument(parserPropertyTags, "parserPropertyTags");
			this.parserPropertyTags = parserPropertyTags;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			if (this.parserPropertyTags == null)
			{
				throw new InvalidOperationException("SetParseOutputData must be called before ParseOutput.");
			}
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => new SuccessfulGetPropertiesSpecificResult(readerParameter, this.parserPropertyTags, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" StreamLimit=").Append(this.streamLimit);
			stringBuilder.Append(" Tags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.GetPropertiesSpecific;

		private PropertyTag[] propertyTags;

		private ushort streamLimit;

		private GetPropertiesFlags flags;

		private PropertyTag[] parserPropertyTags;
	}
}
