using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCreateAttachment : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CreateAttachment;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCreateAttachment();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulCreateAttachmentResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopCreateAttachment.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.CreateAttachment(serverObject, RopCreateAttachment.resultFactory);
		}

		private const RopId RopType = RopId.CreateAttachment;

		private static CreateAttachmentResultFactory resultFactory = new CreateAttachmentResultFactory();
	}
}
