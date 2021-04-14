using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetOwningServers : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetOwningServers;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetOwningServers();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId folderId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.folderId = folderId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.folderId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetOwningServersResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetOwningServers.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.folderId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetOwningServers(serverObject, this.folderId, RopGetOwningServers.resultFactory);
		}

		private const RopId RopType = RopId.GetOwningServers;

		private static GetOwningServersResultFactory resultFactory = new GetOwningServersResultFactory();

		private StoreId folderId;
	}
}
