using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	internal sealed class FastTransferDistributedFolderInfo : IEquatable<FastTransferDistributedFolderInfo>
	{
		public FastTransferDistributedFolderInfo(uint flags, uint depth, StoreLongTermId folderLongTermId, uint localSiteDatabaseCount, string[] databases)
		{
			Util.ThrowOnNullArgument(databases, "databases");
			this.flags = flags;
			this.depth = depth;
			this.folderLongTermId = folderLongTermId;
			this.localSiteDatabaseCount = localSiteDatabaseCount;
			this.databases = databases;
		}

		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public uint Depth
		{
			get
			{
				return this.depth;
			}
		}

		public StoreLongTermId FolderLongTermId
		{
			get
			{
				return this.folderLongTermId;
			}
		}

		public uint LocalSiteDatabaseCount
		{
			get
			{
				return this.localSiteDatabaseCount;
			}
		}

		public string[] Databases
		{
			get
			{
				return this.databases ?? Array<string>.Empty;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is FastTransferDistributedFolderInfo && this.Equals((FastTransferDistributedFolderInfo)obj);
		}

		public override int GetHashCode()
		{
			return this.folderLongTermId.GetHashCode() ^ this.localSiteDatabaseCount.GetHashCode() ^ ArrayComparer<string>.Comparer.GetHashCode(this.Databases);
		}

		public override string ToString()
		{
			return string.Format("FolderLongTermId: {0}, DatabaseCount: {1}, LocalSiteDatabaseCount: {2}, ReplicaDatabases: {{{3}}}", new object[]
			{
				this.FolderLongTermId.ToString(),
				this.Databases.Length,
				this.localSiteDatabaseCount,
				string.Join(",", this.Databases)
			});
		}

		public bool Equals(FastTransferDistributedFolderInfo other)
		{
			return this.folderLongTermId.Equals(other.FolderLongTermId) && this.LocalSiteDatabaseCount == other.LocalSiteDatabaseCount && ArrayComparer<string>.Comparer.Equals(this.Databases, other.Databases);
		}

		internal static FastTransferDistributedFolderInfo Parse(Reader reader)
		{
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			StoreLongTermId storeLongTermId = StoreLongTermId.Parse(reader);
			uint num3 = reader.ReadUInt32();
			uint num4 = reader.ReadUInt32();
			string[] array = new string[num3];
			uint num5 = 0U;
			while ((ulong)num5 < (ulong)((long)array.Length))
			{
				array[(int)((UIntPtr)num5)] = reader.ReadAsciiString(StringFlags.IncludeNull);
				num5 += 1U;
			}
			return new FastTransferDistributedFolderInfo(num, num2, storeLongTermId, num4, array);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt32(this.flags);
			writer.WriteUInt32(this.depth);
			this.folderLongTermId.Serialize(writer);
			writer.WriteUInt32((uint)this.Databases.Length);
			writer.WriteUInt32(this.localSiteDatabaseCount);
			foreach (string value in this.Databases)
			{
				writer.WriteAsciiString(value, StringFlags.IncludeNull);
			}
		}

		private readonly uint flags;

		private readonly uint depth;

		private readonly StoreLongTermId folderLongTermId;

		private readonly uint localSiteDatabaseCount;

		private readonly string[] databases;
	}
}
