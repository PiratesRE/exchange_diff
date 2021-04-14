using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFlushRecipients : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FlushRecipients;
			}
		}

		internal PropertyTag[] ExtraPropertyTags
		{
			get
			{
				return this.extraPropertyTags;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFlushRecipients();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.extraPropertyTags = extraPropertyTags;
			this.recipientRows = recipientRows;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountAndPropertyTagArray(this.extraPropertyTags, FieldLength.WordSize);
			writer.WriteUInt16((ushort)this.recipientRows.Length);
			foreach (RecipientRow recipientRow in this.recipientRows)
			{
				recipientRow.Serialize(writer, this.extraPropertyTags, RecipientSerializationFlags.RecipientRowId, string8Encoding);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFlushRecipients.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.extraPropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
			ushort num = reader.ReadUInt16();
			this.recipientRows = new RecipientRow[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				this.recipientRows[i] = new RecipientRow(reader, this.extraPropertyTags, RecipientSerializationFlags.RecipientRowId);
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			for (int i = 0; i < this.recipientRows.Length; i++)
			{
				this.recipientRows[i].ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FlushRecipients(serverObject, this.extraPropertyTags, this.recipientRows, RopFlushRecipients.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ExtraTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.extraPropertyTags);
			stringBuilder.Append("]");
			stringBuilder.Append(" Recipients=[");
			Util.AppendToString<RecipientRow>(stringBuilder, this.recipientRows);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.FlushRecipients;

		private static FlushRecipientsResultFactory resultFactory = new FlushRecipientsResultFactory();

		private PropertyTag[] extraPropertyTags = Array<PropertyTag>.Empty;

		private RecipientRow[] recipientRows;
	}
}
