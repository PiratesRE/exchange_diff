using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetStatusResult : RopResult
	{
		internal SuccessfulGetStatusResult(TableStatus tableStatus) : base(RopId.GetStatus, ErrorCode.None, null)
		{
			this.tableStatus = tableStatus;
		}

		internal SuccessfulGetStatusResult(Reader reader) : base(reader)
		{
			this.tableStatus = (TableStatus)reader.ReadByte();
		}

		internal static SuccessfulGetStatusResult Parse(Reader reader)
		{
			return new SuccessfulGetStatusResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.tableStatus);
		}

		private readonly TableStatus tableStatus;
	}
}
