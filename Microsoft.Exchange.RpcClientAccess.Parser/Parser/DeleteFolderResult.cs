using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class DeleteFolderResult : RopResult
	{
		internal DeleteFolderResult(ErrorCode errorCode, bool isPartiallyCompleted) : base(RopId.DeleteFolder, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
		}

		internal DeleteFolderResult(Reader reader) : base(reader)
		{
			this.isPartiallyCompleted = reader.ReadBool();
		}

		internal bool PartiallyCompleted
		{
			get
			{
				return this.isPartiallyCompleted;
			}
		}

		internal static RopResult Parse(Reader reader)
		{
			return new DeleteFolderResult(reader);
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
