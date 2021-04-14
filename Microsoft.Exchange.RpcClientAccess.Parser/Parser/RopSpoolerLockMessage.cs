using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSpoolerLockMessage : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SpoolerLockMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSpoolerLockMessage();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId messageId, LockState lockState)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageId = messageId;
			this.lockState = lockState;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.messageId.Serialize(writer);
			writer.WriteByte((byte)this.lockState);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSpoolerLockMessage.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageId = StoreId.Parse(reader);
			this.lockState = (LockState)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SpoolerLockMessage(serverObject, this.messageId, this.lockState, RopSpoolerLockMessage.resultFactory);
		}

		private const RopId RopType = RopId.SpoolerLockMessage;

		private static SpoolerLockMessageResultFactory resultFactory = new SpoolerLockMessageResultFactory();

		private StoreId messageId;

		private LockState lockState;
	}
}
