using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopDeleteAttachment : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.DeleteAttachment;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopDeleteAttachment();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, uint attachmentNumber)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.attachmentNumber = attachmentNumber;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt32(this.attachmentNumber);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopDeleteAttachment.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.attachmentNumber = reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.DeleteAttachment(serverObject, this.attachmentNumber, RopDeleteAttachment.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Number=").Append(this.attachmentNumber);
		}

		private const RopId RopType = RopId.DeleteAttachment;

		private static DeleteAttachmentResultFactory resultFactory = new DeleteAttachmentResultFactory();

		private uint attachmentNumber;
	}
}
