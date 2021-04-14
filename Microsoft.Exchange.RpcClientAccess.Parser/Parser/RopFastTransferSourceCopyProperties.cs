using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceCopyProperties : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceCopyProperties;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceCopyProperties();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.level = level;
			this.flags = flags;
			this.sendOptions = sendOptions;
			this.propertyTags = propertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.level);
			writer.WriteByte((byte)this.flags);
			writer.WriteByte((byte)this.sendOptions);
			writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferSourceCopyPropertiesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferSourceCopyProperties.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.level = reader.ReadByte();
			this.flags = (FastTransferCopyPropertiesFlag)reader.ReadByte();
			this.sendOptions = (FastTransferSendOption)reader.ReadByte();
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferSourceCopyProperties(serverObject, this.level, this.flags, this.sendOptions, this.propertyTags, RopFastTransferSourceCopyProperties.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" level=").Append(this.level.ToString());
			stringBuilder.Append(" flags=").Append(this.flags.ToString());
			stringBuilder.Append(" sendOptions=").Append(this.sendOptions.ToString());
			stringBuilder.Append(" propertyTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.FastTransferSourceCopyProperties;

		private static FastTransferSourceCopyPropertiesResultFactory resultFactory = new FastTransferSourceCopyPropertiesResultFactory();

		private byte level;

		private FastTransferCopyPropertiesFlag flags;

		private FastTransferSendOption sendOptions;

		private PropertyTag[] propertyTags;
	}
}
