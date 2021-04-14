using System;
using System.IO;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class MsgStoragePropertyPrefix
	{
		internal MsgStoragePropertyPrefix(MsgSubStorageType substorageType)
		{
			this.substorageType = substorageType;
		}

		internal int AttachmentCount
		{
			get
			{
				return this.attachmentCount;
			}
			set
			{
				this.attachmentCount = value;
			}
		}

		internal int RecipientCount
		{
			get
			{
				return this.recipientCount;
			}
			set
			{
				this.recipientCount = value;
			}
		}

		internal int Size
		{
			get
			{
				return MsgStoragePropertyPrefix.SubstorageStreamPrefixSizes[(int)this.substorageType];
			}
		}

		internal int Read(byte[] streamData)
		{
			if (this.substorageType == MsgSubStorageType.TopLevelMessage || this.substorageType == MsgSubStorageType.AttachedMessage)
			{
				this.AttachmentCount = BitConverter.ToInt32(streamData, 20);
				this.RecipientCount = BitConverter.ToInt32(streamData, 16);
			}
			return this.Size;
		}

		internal int Write(BinaryWriter writer)
		{
			writer.Write(MsgStoragePropertyPrefix.Padding, 0, 8);
			if (this.substorageType == MsgSubStorageType.Attachment || this.substorageType == MsgSubStorageType.Recipient)
			{
				return this.Size;
			}
			writer.Write(this.RecipientCount);
			writer.Write(this.AttachmentCount);
			writer.Write(this.RecipientCount);
			writer.Write(this.AttachmentCount);
			if (this.substorageType == MsgSubStorageType.AttachedMessage)
			{
				return this.Size;
			}
			writer.Write(MsgStoragePropertyPrefix.Padding, 0, 8);
			return this.Size;
		}

		private const int RecipientCountOffset = 16;

		private const int AttachmentCountOffset = 20;

		internal static readonly int[] SubstorageStreamPrefixSizes = new int[]
		{
			32,
			24,
			8,
			8
		};

		internal static readonly byte[] Padding = new byte[8];

		private int attachmentCount;

		private int recipientCount;

		private MsgSubStorageType substorageType;
	}
}
