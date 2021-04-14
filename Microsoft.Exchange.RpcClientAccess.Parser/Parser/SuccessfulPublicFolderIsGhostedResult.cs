using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulPublicFolderIsGhostedResult : RopResult
	{
		internal SuccessfulPublicFolderIsGhostedResult(ReplicaServerInfo? replicaInfo) : base(RopId.PublicFolderIsGhosted, ErrorCode.None, null)
		{
			this.replicaInfo = replicaInfo;
		}

		internal SuccessfulPublicFolderIsGhostedResult(Reader reader) : base(reader)
		{
			bool flag = reader.ReadBool();
			if (flag)
			{
				this.replicaInfo = new ReplicaServerInfo?(ReplicaServerInfo.Parse(reader));
			}
		}

		internal bool IsGhosted
		{
			get
			{
				return this.replicaInfo != null;
			}
		}

		internal ReplicaServerInfo? ReplicaInfo
		{
			get
			{
				return this.replicaInfo;
			}
		}

		internal static SuccessfulPublicFolderIsGhostedResult Parse(Reader reader)
		{
			return new SuccessfulPublicFolderIsGhostedResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.IsGhosted, 1);
			if (this.IsGhosted)
			{
				this.replicaInfo.Value.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" IsGhosted=").Append(this.IsGhosted);
			if (this.IsGhosted)
			{
				stringBuilder.Append(" ReplicaInfo=").Append(this.replicaInfo.Value.ToString());
			}
		}

		private readonly ReplicaServerInfo? replicaInfo;
	}
}
