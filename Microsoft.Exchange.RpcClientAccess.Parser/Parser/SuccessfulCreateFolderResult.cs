using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCreateFolderResult : RopResult
	{
		internal SuccessfulCreateFolderResult(IServerObject serverObject, StoreId folderId, bool existed, bool hasRules, ReplicaServerInfo? replicaInfo) : base(RopId.CreateFolder, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.FolderId = folderId;
			this.Existed = existed;
			this.HasRules = hasRules;
			this.replicaInfo = replicaInfo;
		}

		internal SuccessfulCreateFolderResult(Reader reader) : base(reader)
		{
			this.FolderId = StoreId.Parse(reader);
			this.Existed = reader.ReadBool();
			if (this.Existed)
			{
				this.HasRules = reader.ReadBool();
				bool flag = reader.ReadBool();
				if (flag)
				{
					this.replicaInfo = new ReplicaServerInfo?(ReplicaServerInfo.Parse(reader));
				}
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

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulCreateFolderResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.FolderId.Serialize(writer);
			writer.WriteBool(this.Existed, 1);
			if (this.Existed)
			{
				writer.WriteBool(this.HasRules, 1);
				writer.WriteBool(this.IsGhosted, 1);
				if (this.IsGhosted)
				{
					this.replicaInfo.Value.Serialize(writer);
				}
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ID=").Append(this.FolderId.ToString());
			stringBuilder.Append(" Existed=").Append(this.Existed);
			stringBuilder.Append(" Has Rules=").Append(this.HasRules);
			stringBuilder.Append(" Ghosted=").Append(this.IsGhosted);
			if (this.IsGhosted)
			{
				stringBuilder.Append(" ReplicaInfo=").Append(this.replicaInfo.Value.ToString());
			}
		}

		internal readonly StoreId FolderId;

		internal readonly bool Existed;

		internal readonly bool HasRules;

		private readonly ReplicaServerInfo? replicaInfo;
	}
}
