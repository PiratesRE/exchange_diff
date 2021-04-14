using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopReloadCachedInformation : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ReloadCachedInformation;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopReloadCachedInformation();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, PropertyTag[] extraUnicodePropertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.extraUnicodePropertyTags = extraUnicodePropertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountAndPropertyTagArray(this.extraUnicodePropertyTags, FieldLength.WordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulReloadCachedInformationResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new ReloadCachedInformationResultFactory(outputBuffer.Count);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.extraUnicodePropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			ReloadCachedInformationResultFactory resultFactory = new ReloadCachedInformationResultFactory(outputBuffer.Count);
			this.result = ropHandler.ReloadCachedInformation(serverObject, this.extraUnicodePropertyTags, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ExtraTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.extraUnicodePropertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.ReloadCachedInformation;

		private PropertyTag[] extraUnicodePropertyTags;
	}
}
