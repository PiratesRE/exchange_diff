using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetContentsTableExtendedResult : RopResult
	{
		internal SuccessfulGetContentsTableExtendedResult(IServerObject table, int rowCount) : base(RopId.GetContentsTableExtended, ErrorCode.None, table)
		{
			Util.ThrowOnNullArgument(table, "table");
			this.rowCount = rowCount;
		}

		internal SuccessfulGetContentsTableExtendedResult(Reader reader) : base(reader)
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

		internal static SuccessfulGetContentsTableExtendedResult Parse(Reader reader)
		{
			return new SuccessfulGetContentsTableExtendedResult(reader);
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
