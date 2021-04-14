using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetMessageStatusResult : RopResult
	{
		internal SuccessfulGetMessageStatusResult(MessageStatusFlags messageStatus) : base(RopId.SetMessageStatus, ErrorCode.None, null)
		{
			this.messageStatus = messageStatus;
		}

		internal SuccessfulGetMessageStatusResult(Reader reader) : base(reader)
		{
			this.messageStatus = (MessageStatusFlags)reader.ReadUInt32();
		}

		internal MessageStatusFlags MessageStatus
		{
			get
			{
				return this.messageStatus;
			}
		}

		internal static SuccessfulGetMessageStatusResult Parse(Reader reader)
		{
			return new SuccessfulGetMessageStatusResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.messageStatus);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Status=").Append(this.messageStatus);
		}

		private readonly MessageStatusFlags messageStatus;
	}
}
