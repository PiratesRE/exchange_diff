using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ProgressInformation
	{
		public ProgressInformation(ushort version, int normalMessageCount, int associatedMessageCount, ulong normalMessagesTotalSize, ulong associatedMessagesTotalSize)
		{
			this.version = version;
			this.associatedMessageCount = associatedMessageCount;
			this.normalMessageCount = normalMessageCount;
			this.normalMessagesTotalSize = normalMessagesTotalSize;
			this.associatedMessagesTotalSize = associatedMessagesTotalSize;
		}

		public static ProgressInformation Parse(Reader reader)
		{
			ushort num = reader.ReadUInt16();
			reader.ReadUInt16();
			int num2 = reader.ReadInt32();
			ulong num3 = reader.ReadUInt64();
			int num4 = reader.ReadInt32();
			reader.ReadUInt32();
			ulong num5 = reader.ReadUInt64();
			return new ProgressInformation(num, num4, num2, num5, num3);
		}

		public void Serialize(Writer writer)
		{
			writer.WriteUInt16(this.version);
			writer.WriteUInt16(0);
			writer.WriteInt32(this.associatedMessageCount);
			writer.WriteUInt64(this.associatedMessagesTotalSize);
			writer.WriteInt32(this.normalMessageCount);
			writer.WriteUInt32(0U);
			writer.WriteUInt64(this.normalMessagesTotalSize);
		}

		public override string ToString()
		{
			return string.Format("ProgressInformation Version = {0}. NormalMessageCount = {1}. AssociatedMessageCount = {2}. NormalMessagesTotalSize {3}. AssociatedMessagesTotalSize = {4}.", new object[]
			{
				this.version,
				this.normalMessageCount,
				this.associatedMessageCount,
				this.normalMessagesTotalSize,
				this.associatedMessagesTotalSize
			});
		}

		public const int SerializedSize = 32;

		private ushort version;

		private int associatedMessageCount;

		private int normalMessageCount;

		private ulong normalMessagesTotalSize;

		private ulong associatedMessagesTotalSize;
	}
}
