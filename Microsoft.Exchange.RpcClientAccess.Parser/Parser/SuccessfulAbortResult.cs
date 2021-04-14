using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulAbortResult : RopResult
	{
		internal SuccessfulAbortResult(TableStatus status) : base(RopId.Abort, ErrorCode.None, null)
		{
			this.status = status;
		}

		internal SuccessfulAbortResult(Reader reader) : base(reader)
		{
			this.status = (TableStatus)reader.ReadByte();
		}

		internal TableStatus Status
		{
			get
			{
				return this.status;
			}
		}

		internal static SuccessfulAbortResult Parse(Reader reader)
		{
			return new SuccessfulAbortResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.status);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" TableStatus=").Append(this.status);
		}

		private readonly TableStatus status;
	}
}
