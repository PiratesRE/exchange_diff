using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmptyFolderResult : RopResult
	{
		internal EmptyFolderResult(ErrorCode errorCode, bool isPartiallyCompleted) : base(RopId.EmptyFolder, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
		}

		internal EmptyFolderResult(Reader reader) : base(reader)
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
			stringBuilder.Append(" Partial=").Append(this.isPartiallyCompleted);
		}

		private readonly bool isPartiallyCompleted;
	}
}
