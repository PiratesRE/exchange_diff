using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct ReplicaServerInfo : IEquatable<ReplicaServerInfo>
	{
		public ReplicaServerInfo(ushort cheapReplicaCount, string[] replicas)
		{
			Util.ThrowOnNullArgument(replicas, "replicas");
			this.cheapReplicaCount = cheapReplicaCount;
			this.replicas = replicas;
		}

		public ReplicaServerInfo(string mailboxLegacyDistinguishedName)
		{
			Util.ThrowOnNullOrEmptyArgument(mailboxLegacyDistinguishedName, "mailboxLegacyDistinguishedName");
			this.cheapReplicaCount = 1;
			this.replicas = new string[]
			{
				mailboxLegacyDistinguishedName
			};
		}

		public ushort CheapReplicaCount
		{
			get
			{
				return this.cheapReplicaCount;
			}
		}

		public string[] Replicas
		{
			get
			{
				return this.replicas ?? Array<string>.Empty;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ReplicaServerInfo && this.Equals((ReplicaServerInfo)obj);
		}

		public override int GetHashCode()
		{
			return this.cheapReplicaCount.GetHashCode() ^ ArrayComparer<string>.Comparer.GetHashCode(this.replicas);
		}

		public override string ToString()
		{
			return string.Format("ServerCount: {0}, CheapReplicaCount: {1}, Replicas: {{{2}}}", this.Replicas.Length, this.CheapReplicaCount, string.Join(",", this.Replicas));
		}

		public bool Equals(ReplicaServerInfo other)
		{
			return this.CheapReplicaCount == other.CheapReplicaCount && ArrayComparer<string>.Comparer.Equals(this.Replicas, other.Replicas);
		}

		internal static ReplicaServerInfo Parse(Reader reader)
		{
			ushort num = reader.ReadUInt16();
			ushort num2 = reader.ReadUInt16();
			string[] array = new string[(int)num];
			uint num3 = 0U;
			while ((ulong)num3 < (ulong)((long)array.Length))
			{
				array[(int)((UIntPtr)num3)] = reader.ReadAsciiString(StringFlags.IncludeNull);
				num3 += 1U;
			}
			return new ReplicaServerInfo(num2, array);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt16((ushort)this.replicas.Length);
			writer.WriteUInt16(this.cheapReplicaCount);
			foreach (string value in this.replicas)
			{
				writer.WriteAsciiString(value, StringFlags.IncludeNull);
			}
		}

		private readonly ushort cheapReplicaCount;

		private readonly string[] replicas;
	}
}
