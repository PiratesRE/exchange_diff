using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetReceiveFolderTable : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetReceiveFolderTable;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetReceiveFolderTable();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulGetReceiveFolderTableResult.Parse(reader, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetReceiveFolderTable.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetReceiveFolderTable(serverObject, RopGetReceiveFolderTable.resultFactory);
		}

		private const RopId RopType = RopId.GetReceiveFolderTable;

		private static GetReceiveFolderTableResultFactory resultFactory = new GetReceiveFolderTableResultFactory();
	}
}
