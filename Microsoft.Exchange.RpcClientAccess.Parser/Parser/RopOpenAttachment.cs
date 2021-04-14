using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenAttachment : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenAttachment;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenAttachment();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, OpenMode openMode, uint attachmentNumber)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.openMode = openMode;
			this.attachmentNumber = attachmentNumber;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.openMode);
			writer.WriteUInt32(this.attachmentNumber);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulOpenAttachmentResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopOpenAttachment.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.openMode = (OpenMode)reader.ReadByte();
			this.attachmentNumber = reader.ReadUInt32();
			if (this.attachmentNumber > 1024U)
			{
				throw new BufferParseException(string.Format("Attachment number is greater then maximum attachment number allowed. Maximum: {0}. Actual: {1}", 1024U, this.attachmentNumber));
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.OpenAttachment(serverObject, this.openMode, this.attachmentNumber, RopOpenAttachment.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" OpenMode=").Append(this.openMode);
			stringBuilder.Append(" Number=").Append(this.attachmentNumber);
		}

		private const RopId RopType = RopId.OpenAttachment;

		private const uint MaxAttachmentNumber = 1024U;

		private static OpenAttachmentResultFactory resultFactory = new OpenAttachmentResultFactory();

		private OpenMode openMode;

		private uint attachmentNumber;
	}
}
