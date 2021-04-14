using System;
using Microsoft.Exchange.Data.Serialization;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal class PartitionInformation
	{
		public PartitionInformation(Guid partitionGuid, PartitionInformation.ControlFlags flags)
		{
			this.partitionGuid = partitionGuid;
			this.flags = flags;
		}

		public Guid PartitionGuid
		{
			get
			{
				return this.partitionGuid;
			}
		}

		public PartitionInformation.ControlFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public static PartitionInformation Parse(MailboxSignatureSectionMetadata metadata, byte[] buffer, ref int offset)
		{
			if (metadata.Length != 20)
			{
				throw new ArgumentException("Invalide PartitionInformation section");
			}
			Guid guid = Serialization.DeserializeGuid(buffer, ref offset);
			PartitionInformation.ControlFlags controlFlags = (PartitionInformation.ControlFlags)Serialization.DeserializeUInt32(buffer, ref offset);
			return new PartitionInformation(guid, controlFlags);
		}

		public int Serialize(byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				Serialization.SerializeGuid(buffer, ref offset, this.partitionGuid);
				Serialization.SerializeUInt32(buffer, ref offset, (uint)this.flags);
			}
			return 20;
		}

		private const int SerializedSize = 20;

		public const short RequiredVersion = 1;

		private readonly Guid partitionGuid;

		private readonly PartitionInformation.ControlFlags flags;

		[Flags]
		public enum ControlFlags
		{
			None = 0,
			CreateNewPartition = 1
		}
	}
}
