using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulLongTermIdFromIdResult : RopResult
	{
		internal SuccessfulLongTermIdFromIdResult(StoreLongTermId longTermId) : base(RopId.LongTermIdFromId, ErrorCode.None, null)
		{
			this.longTermId = longTermId;
		}

		internal SuccessfulLongTermIdFromIdResult(Reader reader) : base(reader)
		{
			this.longTermId = StoreLongTermId.Parse(reader);
		}

		public StoreLongTermId LongTermId
		{
			get
			{
				return this.longTermId;
			}
		}

		internal static SuccessfulLongTermIdFromIdResult Parse(Reader reader)
		{
			return new SuccessfulLongTermIdFromIdResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.longTermId.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
		}

		private readonly StoreLongTermId longTermId;
	}
}
