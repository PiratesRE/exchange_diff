using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetContentsTableResult : RopResult
	{
		internal SuccessfulGetContentsTableResult(IServerObject table, int rowCount) : base(RopId.GetContentsTable, ErrorCode.None, table)
		{
			Util.ThrowOnNullArgument(table, "table");
			this.rowCount = rowCount;
		}

		internal SuccessfulGetContentsTableResult(Reader reader) : base(reader)
		{
			this.rowCount = reader.ReadInt32();
		}

		internal int RowCount
		{
			get
			{
				return this.rowCount;
			}
		}

		internal static SuccessfulGetContentsTableResult Parse(Reader reader)
		{
			return new SuccessfulGetContentsTableResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.rowCount);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" RowCount=").Append(this.rowCount);
		}

		private readonly int rowCount;
	}
}
