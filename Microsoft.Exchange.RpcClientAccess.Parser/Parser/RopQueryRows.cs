using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopQueryRows : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.QueryRows;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopQueryRows();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.useForwardDirection = useForwardDirection;
			this.rowCount = rowCount;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteBool(this.useForwardDirection, 1);
			writer.WriteUInt16(this.rowCount);
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
			RopResult ropResult = RopResult.Parse(reader, (Reader readerParameter) => new SuccessfulQueryRowsResult(readerParameter, this.columns, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
			if (this.result == null)
			{
				this.result = ropResult;
			}
			if (base.ChainedResults == null)
			{
				base.ChainedResults = new List<RopResult>(10);
			}
			base.ChainedResults.Add(ropResult);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new QueryRowsResultFactory(outputBuffer.Count, connection.String8Encoding);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (QueryRowsFlags)reader.ReadByte();
			this.useForwardDirection = reader.ReadBool();
			this.rowCount = reader.ReadUInt16();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override byte[] CreateFakeRopRequest(RopResult result, ServerObjectHandleTable serverObjectHandleTable)
		{
			SuccessfulQueryRowsResult successfulQueryRowsResult = result as SuccessfulQueryRowsResult;
			if ((byte)(this.flags & QueryRowsFlags.DoNotAdvance) == 0 && (byte)(this.flags & QueryRowsFlags.SendMax) != 0 && successfulQueryRowsResult.BookmarkOrigin != BookmarkOrigin.End && successfulQueryRowsResult.BookmarkOrigin != BookmarkOrigin.Beginning && successfulQueryRowsResult.Rows.Length > 0 && (successfulQueryRowsResult.Rows.Length < (int)this.rowCount || (byte)(this.flags & QueryRowsFlags.ChainAlways) != 0))
			{
				RopQueryRows ropQueryRows = (RopQueryRows)RopQueryRows.CreateRop();
				ropQueryRows.SetInput(base.LogonIndex, base.HandleTableIndex, this.flags, this.useForwardDirection, (ushort)((int)this.rowCount - successfulQueryRowsResult.Rows.Length));
				return RopDriver.CreateFakeRopRequest(ropQueryRows, serverObjectHandleTable);
			}
			return null;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			QueryRowsResultFactory resultFactory = new QueryRowsResultFactory(outputBuffer.Count, serverObject.String8Encoding);
			this.result = ropHandler.QueryRows(serverObject, this.flags, this.useForwardDirection, this.rowCount, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Forward=").Append(this.useForwardDirection);
			stringBuilder.Append(" Count=").Append(this.rowCount);
		}

		private const RopId RopType = RopId.QueryRows;

		private QueryRowsFlags flags;

		private bool useForwardDirection;

		private ushort rowCount;

		private PropertyTag[] columns;
	}
}
