using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopExpandRow : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ExpandRow;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopExpandRow();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, short maxRows, StoreId categoryId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.maxRows = maxRows;
			this.categoryId = categoryId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteInt16(this.maxRows);
			this.categoryId.Serialize(writer);
		}

		internal void SetParseOutputData(PropertyTag[] columns)
		{
			Util.ThrowOnNullArgument(columns, "columns");
			this.columns = columns;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			if (this.columns == null)
			{
				throw new InvalidOperationException("SetParseOutputData must be called before ParseOutput.");
			}
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => new SuccessfulExpandRowResult(readerParameter, this.columns, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new ExpandRowResultFactory(outputBuffer.Count, connection.String8Encoding);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.maxRows = reader.ReadInt16();
			this.categoryId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			ExpandRowResultFactory resultFactory = new ExpandRowResultFactory(outputBuffer.Count, serverObject.String8Encoding);
			this.result = ropHandler.ExpandRow(serverObject, this.maxRows, this.categoryId, resultFactory);
		}

		private const RopId RopType = RopId.ExpandRow;

		private short maxRows;

		private StoreId categoryId;

		private PropertyTag[] columns;
	}
}
