using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSetMessageStatusResult : RopResult
	{
		internal SuccessfulSetMessageStatusResult(MessageStatusFlags oldStatus) : base(RopId.SetMessageStatus, ErrorCode.None, null)
		{
			this.oldStatus = oldStatus;
		}

		internal SuccessfulSetMessageStatusResult(Reader reader) : base(reader)
		{
			this.oldStatus = (MessageStatusFlags)reader.ReadUInt32();
		}

		internal MessageStatusFlags OldStatus
		{
			get
			{
				return this.oldStatus;
			}
		}

		internal static SuccessfulSetMessageStatusResult Parse(Reader reader)
		{
			return new SuccessfulSetMessageStatusResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.oldStatus);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Old Status=").Append(this.oldStatus);
		}

		private readonly MessageStatusFlags oldStatus;
	}
}
