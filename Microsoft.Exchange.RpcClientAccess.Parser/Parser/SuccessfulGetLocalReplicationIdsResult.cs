using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetLocalReplicationIdsResult : RopResult
	{
		internal SuccessfulGetLocalReplicationIdsResult(StoreLongTermId localReplicationId) : base(RopId.GetLocalReplicationIds, ErrorCode.None, null)
		{
			this.localReplicationId = localReplicationId;
		}

		internal SuccessfulGetLocalReplicationIdsResult(Reader reader) : base(reader)
		{
			this.localReplicationId = StoreLongTermId.Parse(reader, false);
		}

		public StoreLongTermId LocalReplicationId
		{
			get
			{
				return this.localReplicationId;
			}
		}

		internal static SuccessfulGetLocalReplicationIdsResult Parse(Reader reader)
		{
			return new SuccessfulGetLocalReplicationIdsResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.localReplicationId.Serialize(writer, false);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ReplID=[").Append(this.localReplicationId).Append("]");
		}

		private readonly StoreLongTermId localReplicationId;
	}
}
