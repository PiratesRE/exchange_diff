using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSaveChangesMessage : RopSaveChanges
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SaveChangesMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSaveChangesMessage();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSaveChangesMessageResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new SaveChangesMessageResultFactory(this.realHandleTableIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			SaveChangesMessageResultFactory resultFactory = new SaveChangesMessageResultFactory(this.realHandleTableIndex);
			this.result = ropHandler.SaveChangesMessage(serverObject, this.saveChangesMode, resultFactory);
		}

		private const RopId RopType = RopId.SaveChangesMessage;
	}
}
