using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceCopyTo : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceCopyTo;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceCopyTo();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.level = level;
			this.flags = flags;
			this.sendOptions = sendOptions;
			this.excludedPropertyTags = excludedPropertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.level);
			writer.WriteUInt32((uint)this.flags);
			writer.WriteByte((byte)this.sendOptions);
			writer.WriteCountAndPropertyTagArray(this.excludedPropertyTags, FieldLength.WordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferSourceCopyToResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferSourceCopyTo.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.level = reader.ReadByte();
			this.flags = (FastTransferCopyFlag)reader.ReadUInt32();
			this.sendOptions = (FastTransferSendOption)reader.ReadByte();
			this.excludedPropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferSourceCopyTo(serverObject, this.level, this.flags, this.sendOptions, this.excludedPropertyTags, RopFastTransferSourceCopyTo.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Level=").Append(this.level);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" SendOptions=").Append(this.sendOptions);
			stringBuilder.Append(" ExcludeTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.excludedPropertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.FastTransferSourceCopyTo;

		private static FastTransferSourceCopyToResultFactory resultFactory = new FastTransferSourceCopyToResultFactory();

		private byte level;

		private FastTransferCopyFlag flags;

		private FastTransferSendOption sendOptions;

		private PropertyTag[] excludedPropertyTags;
	}
}
