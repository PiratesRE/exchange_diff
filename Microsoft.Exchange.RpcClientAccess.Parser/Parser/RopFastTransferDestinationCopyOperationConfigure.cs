using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferDestinationCopyOperationConfigure : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferDestinationCopyOperationConfigure;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferDestinationCopyOperationConfigure();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.copyOperation = copyOperation;
			this.flags = flags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.copyOperation);
			writer.WriteByte((byte)this.flags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferDestinationCopyOperationConfigureResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferDestinationCopyOperationConfigure.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.copyOperation = (FastTransferCopyOperation)reader.ReadByte();
			this.flags = (FastTransferCopyPropertiesFlag)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferDestinationCopyOperationConfigure(serverObject, this.copyOperation, this.flags, RopFastTransferDestinationCopyOperationConfigure.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" FastTransferCopyOperation=").Append(this.copyOperation);
			stringBuilder.Append(" FastTransferCopyPropertiesFlag=").Append(this.flags);
		}

		private const RopId RopType = RopId.FastTransferDestinationCopyOperationConfigure;

		private static FastTransferDestinationCopyOperationConfigureResultFactory resultFactory = new FastTransferDestinationCopyOperationConfigureResultFactory();

		private FastTransferCopyOperation copyOperation;

		private FastTransferCopyPropertiesFlag flags;
	}
}
