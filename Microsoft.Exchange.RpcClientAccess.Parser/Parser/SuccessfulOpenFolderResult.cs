using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulOpenFolderResult : RopResult
	{
		internal SuccessfulOpenFolderResult(IServerObject folder, bool hasRules, ReplicaServerInfo? replicaInfo) : base(RopId.OpenFolder, ErrorCode.None, folder)
		{
			this.hasRules = hasRules;
			this.replicaInfo = replicaInfo;
		}

		internal SuccessfulOpenFolderResult(Reader reader) : base(reader)
		{
			this.hasRules = reader.ReadBool();
			bool flag = reader.ReadBool();
			if (flag)
			{
				this.replicaInfo = new ReplicaServerInfo?(ReplicaServerInfo.Parse(reader));
			}
		}

		internal bool HasRules
		{
			get
			{
				return this.hasRules;
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

		internal static SuccessfulOpenFolderResult Parse(Reader reader)
		{
			return new SuccessfulOpenFolderResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.hasRules, 1);
			writer.WriteBool(this.IsGhosted, 1);
			if (this.IsGhosted)
			{
				this.replicaInfo.Value.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" HasRules=").Append(this.HasRules);
			stringBuilder.Append(" IsGhosted=").Append(this.IsGhosted);
			if (this.IsGhosted)
			{
				stringBuilder.Append(" ReplicaInfo=").Append(this.replicaInfo.Value.ToString());
			}
		}

		private readonly bool hasRules;

		private readonly ReplicaServerInfo? replicaInfo;
	}
}
