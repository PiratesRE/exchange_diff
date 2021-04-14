using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSetColumnsResult : RopResult
	{
		internal TableStatus TableStatus
		{
			get
			{
				return this.tableStatus;
			}
		}

		internal SuccessfulSetColumnsResult(TableStatus tableStatus) : base(RopId.SetColumns, ErrorCode.None, null)
		{
			this.tableStatus = tableStatus;
		}

		internal SuccessfulSetColumnsResult(Reader reader) : base(reader)
		{
			this.tableStatus = (TableStatus)reader.ReadByte();
		}

		internal static SuccessfulSetColumnsResult Parse(Reader reader)
		{
			return new SuccessfulSetColumnsResult(reader);
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
