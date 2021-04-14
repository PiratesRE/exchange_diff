using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopReadRecipients : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ReadRecipients;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopReadRecipients();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.recipientRowId = recipientRowId;
			this.extraUnicodePropertyTags = extraUnicodePropertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt32(this.recipientRowId);
			writer.WriteCountAndPropertyTagArray(this.extraUnicodePropertyTags, FieldLength.WordSize);
		}

		internal void SetParseOutputData(PropertyTag[] extraPropertyTags)
		{
			Util.ThrowOnNullArgument(extraPropertyTags, "extraPropertyTags");
			this.extraPropertyTags = extraPropertyTags;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			if (this.extraPropertyTags == null)
			{
				throw new InvalidOperationException("SetParseOutputData must be called before ParseOutput.");
			}
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => new SuccessfulReadRecipientsResult(readerParameter, this.extraPropertyTags), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new ReadRecipientsResultFactory(outputBuffer.Count);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.recipientRowId = reader.ReadUInt32();
			this.extraUnicodePropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			ReadRecipientsResultFactory resultFactory = new ReadRecipientsResultFactory(outputBuffer.Count);
			this.result = ropHandler.ReadRecipients(serverObject, this.recipientRowId, this.extraUnicodePropertyTags, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" RowId=").Append(this.recipientRowId);
			stringBuilder.Append(" UnicodeTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.extraUnicodePropertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.ReadRecipients;

		private PropertyTag[] extraUnicodePropertyTags;

		private uint recipientRowId;

		private PropertyTag[] extraPropertyTags;
	}
}
