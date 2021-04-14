using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulRestrictResult : RopResult
	{
		internal SuccessfulRestrictResult(TableStatus tableStatus) : base(RopId.Restrict, ErrorCode.None, null)
		{
			this.tableStatus = tableStatus;
		}

		internal SuccessfulRestrictResult(Reader reader) : base(reader)
		{
			this.tableStatus = (TableStatus)reader.ReadByte();
		}

		internal static SuccessfulRestrictResult Parse(Reader reader)
		{
			return new SuccessfulRestrictResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.tableStatus);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" TableStatus=").Append(this.tableStatus);
		}

		private readonly TableStatus tableStatus;
	}
}
