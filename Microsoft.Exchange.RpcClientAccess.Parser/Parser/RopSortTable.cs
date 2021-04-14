using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSortTable : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SortTable;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSortTable();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.categoryCount = categoryCount;
			this.expandedCount = expandedCount;
			this.sortOrders = sortOrders;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteUInt16((ushort)this.sortOrders.Length);
			writer.WriteUInt16(this.categoryCount);
			writer.WriteUInt16(this.expandedCount);
			for (int i = 0; i < this.sortOrders.Length; i++)
			{
				this.sortOrders[i].Serialize(writer);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSortTableResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSortTable.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (SortTableFlags)reader.ReadByte();
			ushort num = reader.ReadUInt16();
			this.categoryCount = reader.ReadUInt16();
			this.expandedCount = reader.ReadUInt16();
			reader.CheckBoundary((uint)num, 5U);
			this.sortOrders = new SortOrder[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				this.sortOrders[i] = SortOrder.Parse(reader);
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SortTable(serverObject, this.flags, this.categoryCount, this.expandedCount, this.sortOrders, RopSortTable.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Categories=").Append(this.categoryCount);
			stringBuilder.Append(" Expanded=").Append(this.expandedCount);
			if (this.sortOrders != null)
			{
				Util.AppendToString<SortOrder>(stringBuilder, this.sortOrders);
			}
		}

		private const RopId RopType = RopId.SortTable;

		private static SortTableResultFactory resultFactory = new SortTableResultFactory();

		private SortTableFlags flags;

		private ushort categoryCount;

		private ushort expandedCount;

		private SortOrder[] sortOrders;
	}
}
