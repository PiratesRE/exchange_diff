using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetPerUserLongTermIdsResult : RopResult
	{
		internal SuccessfulGetPerUserLongTermIdsResult(StoreLongTermId[] longTermIds) : base(RopId.GetPerUserLongTermIds, ErrorCode.None, null)
		{
			if (longTermIds == null)
			{
				throw new ArgumentNullException("longTermIds");
			}
			this.longTermIds = longTermIds;
		}

		internal SuccessfulGetPerUserLongTermIdsResult(Reader reader) : base(reader)
		{
			this.longTermIds = reader.ReadSizeAndStoreLongTermIdArray();
		}

		internal StoreLongTermId[] LongTermIds
		{
			get
			{
				return this.longTermIds;
			}
		}

		internal static SuccessfulGetPerUserLongTermIdsResult Parse(Reader reader)
		{
			return new SuccessfulGetPerUserLongTermIdsResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedStoreLongTermIds(this.LongTermIds);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LongTermIds=[");
			Util.AppendToString<StoreLongTermId>(stringBuilder, this.longTermIds);
			stringBuilder.Append("]");
		}

		private readonly StoreLongTermId[] longTermIds;
	}
}
