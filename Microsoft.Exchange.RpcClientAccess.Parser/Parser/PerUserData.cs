using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerUserData
	{
		public PerUserData(StoreLongTermId longTermId, Guid replicaGuid)
		{
			this.longTermId = longTermId;
			this.replicaGuid = replicaGuid;
		}

		internal PerUserData(Reader reader)
		{
			this.longTermId = StoreLongTermId.Parse(reader);
			this.replicaGuid = reader.ReadGuid();
		}

		internal void Serialize(Writer writer)
		{
			this.longTermId.Serialize(writer, true);
			writer.WriteGuid(this.replicaGuid);
		}

		internal StoreLongTermId LongTermId
		{
			get
			{
				return this.longTermId;
			}
		}

		internal Guid ReplicaGuid
		{
			get
			{
				return this.replicaGuid;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append("[LongTermId=").Append(this.longTermId);
			stringBuilder.Append(" ReplicaGuid=").Append(this.replicaGuid).Append("]");
		}

		private readonly StoreLongTermId longTermId;

		private readonly Guid replicaGuid;
	}
}
