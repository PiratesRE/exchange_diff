using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SetReadFlagsResult : RopResult
	{
		internal SetReadFlagsResult(ErrorCode errorCode, bool isPartiallyCompleted) : base(RopId.SetReadFlags, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
		}

		internal SetReadFlagsResult(Reader reader) : base(reader)
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

		private readonly bool isPartiallyCompleted;
	}
}
