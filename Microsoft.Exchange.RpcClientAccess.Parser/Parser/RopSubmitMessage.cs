using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSubmitMessage : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SubmitMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSubmitMessage();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, SubmitMessageFlags submitFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.submitFlags = submitFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.submitFlags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSubmitMessage.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.submitFlags = (SubmitMessageFlags)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SubmitMessage(serverObject, this.submitFlags, RopSubmitMessage.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.submitFlags);
		}

		private const RopId RopType = RopId.SubmitMessage;

		private static SubmitMessageResultFactory resultFactory = new SubmitMessageResultFactory();

		private SubmitMessageFlags submitFlags;
	}
}
