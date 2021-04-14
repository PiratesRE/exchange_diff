using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetContentsTableExtended : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetContentsTableExtended;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetContentsTableExtended();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new GetContentsTableExtendedResultFactory(base.PeekReturnServerObjectHandleValue);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.extendedTableFlags = (ExtendedTableFlags)reader.ReadInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			GetContentsTableExtendedResultFactory resultFactory = new GetContentsTableExtendedResultFactory(base.PeekReturnServerObjectHandleValue);
			this.result = ropHandler.GetContentsTableExtended(serverObject, this.extendedTableFlags, resultFactory);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ExtendedTableFlags extendedTableFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.extendedTableFlags = extendedTableFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteInt32((int)this.extendedTableFlags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetContentsTableExtendedResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.extendedTableFlags);
		}

		private const RopId RopType = RopId.GetContentsTableExtended;

		private ExtendedTableFlags extendedTableFlags;
	}
}
