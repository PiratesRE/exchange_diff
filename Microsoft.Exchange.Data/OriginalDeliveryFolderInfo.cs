using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal class OriginalDeliveryFolderInfo
	{
		public OriginalDeliveryFolderInfo(OriginalDeliveryFolderInfo.DeliveryFolderType folderType, int folderHash)
		{
			this.folderType = folderType;
			this.folderHash = folderHash;
		}

		public int FolderHash
		{
			get
			{
				return this.folderHash;
			}
		}

		public OriginalDeliveryFolderInfo.DeliveryFolderType FolderType
		{
			get
			{
				return this.folderType;
			}
		}

		public bool IsDeliveryFolderInbox
		{
			get
			{
				return this.folderType == OriginalDeliveryFolderInfo.DeliveryFolderType.Inbox;
			}
		}

		public bool IsDeliveryFolderClutter
		{
			get
			{
				return this.folderType == OriginalDeliveryFolderInfo.DeliveryFolderType.Clutter;
			}
		}

		public bool IsDeliveryFolderDeletedItems
		{
			get
			{
				return this.folderType == OriginalDeliveryFolderInfo.DeliveryFolderType.DeletedItems;
			}
		}

		public byte[] Serialize()
		{
			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(1);
				binaryWriter.Write(0);
				binaryWriter.Write((byte)this.folderType);
				binaryWriter.Write(this.folderHash);
				binaryWriter.Flush();
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static OriginalDeliveryFolderInfo Deserialize(byte[] folderInfoBytes)
		{
			ArgumentValidator.ThrowIfNull("folderInfoBytes", folderInfoBytes);
			OriginalDeliveryFolderInfo result = null;
			if (folderInfoBytes.Length == 9)
			{
				using (MemoryStream memoryStream = new MemoryStream(folderInfoBytes, false))
				{
					BinaryReader binaryReader = new BinaryReader(memoryStream);
					short num = binaryReader.ReadInt16();
					if (num > 1)
					{
						return null;
					}
					binaryReader.ReadInt16();
					byte b = binaryReader.ReadByte();
					int num2 = binaryReader.ReadInt32();
					result = new OriginalDeliveryFolderInfo((OriginalDeliveryFolderInfo.DeliveryFolderType)b, num2);
				}
				return result;
			}
			return result;
		}

		public static bool IsDeliveryFolderInfoBytesDenoteInbox(byte[] deliveryFolderInfoBytes)
		{
			bool result = false;
			if (deliveryFolderInfoBytes != null)
			{
				OriginalDeliveryFolderInfo originalDeliveryFolderInfo = OriginalDeliveryFolderInfo.Deserialize(deliveryFolderInfoBytes);
				if (originalDeliveryFolderInfo != null && originalDeliveryFolderInfo.IsDeliveryFolderInbox)
				{
					result = true;
				}
			}
			return result;
		}

		public override string ToString()
		{
			switch (this.folderType)
			{
			case OriginalDeliveryFolderInfo.DeliveryFolderType.Inbox:
				return "Inbox";
			case OriginalDeliveryFolderInfo.DeliveryFolderType.DeletedItems:
				return "DeletedItems";
			case OriginalDeliveryFolderInfo.DeliveryFolderType.Clutter:
				return "Clutter";
			}
			return this.FolderHash.ToString();
		}

		public const string InboxString = "Inbox";

		public const string ClutterString = "Clutter";

		public const string DeletedItemsString = "DeletedItems";

		private const short SerializationVersion = 1;

		private const short RequiredNumberOfBytes = 9;

		private readonly OriginalDeliveryFolderInfo.DeliveryFolderType folderType;

		private readonly int folderHash;

		internal enum DeliveryFolderType : byte
		{
			Inbox = 1,
			DeletedItems,
			Other,
			Clutter
		}
	}
}
