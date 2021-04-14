using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSaveChangesAttachment : RopSaveChanges
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SaveChangesAttachment;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSaveChangesAttachment();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSaveChangesAttachment.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SaveChangesAttachment(serverObject, this.saveChangesMode, RopSaveChangesAttachment.resultFactory);
		}

		private const RopId RopType = RopId.SaveChangesAttachment;

		private static SaveChangesAttachmentResultFactory resultFactory = new SaveChangesAttachmentResultFactory();
	}
}
