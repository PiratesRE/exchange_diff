using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class HardDeleteMessagesResult : RopResult
	{
		internal HardDeleteMessagesResult(ErrorCode errorCode, bool isPartiallyCompleted) : base(RopId.HardDeleteMessages, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
		}

		internal HardDeleteMessagesResult(Reader reader) : base(reader)
		{
			this.isPartiallyCompleted = reader.ReadBool();
		}

		internal bool IsPartiallyCompleted
		{
			get
			{
				return this.isPartiallyCompleted;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.isPartiallyCompleted);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Partial Completion=").Append(this.isPartiallyCompleted);
		}

		private readonly bool isPartiallyCompleted;
	}
}
