using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetOwningServersResult : RopResult
	{
		internal SuccessfulGetOwningServersResult(ReplicaServerInfo replicaInfo) : base(RopId.GetOwningServers, ErrorCode.None, null)
		{
			this.replicaInfo = replicaInfo;
		}

		internal SuccessfulGetOwningServersResult(Reader reader) : base(reader)
		{
			this.replicaInfo = ReplicaServerInfo.Parse(reader);
		}

		internal ReplicaServerInfo ReplicaInfo
		{
			get
			{
				return this.replicaInfo;
			}
		}

		internal static SuccessfulGetOwningServersResult Parse(Reader reader)
		{
			return new SuccessfulGetOwningServersResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.replicaInfo.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" OwningServers=").Append(this.replicaInfo.ToString());
		}

		private readonly ReplicaServerInfo replicaInfo;
	}
}
