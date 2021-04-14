using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSortTableResult : RopResult
	{
		internal SuccessfulSortTableResult(TableStatus tableStatus) : base(RopId.SortTable, ErrorCode.None, null)
		{
			this.tableStatus = tableStatus;
		}

		internal SuccessfulSortTableResult(Reader reader) : base(reader)
		{
			this.tableStatus = (TableStatus)reader.ReadByte();
		}

		internal TableStatus TableStatus
		{
			get
			{
				return this.tableStatus;
			}
		}

		public override string ToString()
		{
			return string.Format("SuccessfulSortTableResult: [Status: {0}]", this.tableStatus);
		}

		internal static SuccessfulSortTableResult Parse(Reader reader)
		{
			return new SuccessfulSortTableResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.tableStatus);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Status=").Append(this.tableStatus);
		}

		private readonly TableStatus tableStatus;
	}
}
