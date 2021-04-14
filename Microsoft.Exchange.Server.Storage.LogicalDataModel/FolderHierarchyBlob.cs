using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public struct FolderHierarchyBlob
	{
		public FolderHierarchyBlob(int mailboxPartitionNumber, int mailboxNumber, byte[] parentFolderId, byte[] folderId, string displayName, int depth, int sortPosition)
		{
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.mailboxNumber = mailboxNumber;
			this.parentFolderId = parentFolderId;
			this.folderId = folderId;
			this.displayName = displayName;
			this.depth = depth;
			this.sortPosition = sortPosition;
		}

		public int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public byte[] ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
		}

		public int SortPosition
		{
			get
			{
				return this.sortPosition;
			}
		}

		internal static byte[] Serialize(IList<FolderHierarchyBlob> arrayOfItems)
		{
			int count = arrayOfItems.Count;
			int num = 0;
			num += SerializedValue.SerializeInt32(1, null, 0);
			num += SerializedValue.SerializeInt32(count, null, 0);
			for (int i = 0; i < count; i++)
			{
				num += SerializedValue.SerializeInt32(arrayOfItems[i].mailboxPartitionNumber, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].mailboxNumber, null, 0);
				num += SerializedValue.SerializeBinary(arrayOfItems[i].parentFolderId, null, 0);
				num += SerializedValue.SerializeBinary(arrayOfItems[i].folderId, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].displayName, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].depth, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].sortPosition, null, 0);
			}
			byte[] array = new byte[num];
			int num2 = 0;
			num2 += SerializedValue.SerializeInt32(1, array, num2);
			num2 += SerializedValue.SerializeInt32(count, array, num2);
			for (int j = 0; j < count; j++)
			{
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].mailboxPartitionNumber, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].mailboxNumber, array, num2);
				num2 += SerializedValue.SerializeBinary(arrayOfItems[j].parentFolderId, array, num2);
				num2 += SerializedValue.SerializeBinary(arrayOfItems[j].folderId, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].displayName, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].depth, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].sortPosition, array, num2);
			}
			return array;
		}

		internal static FolderHierarchyBlob[] Deserialize(byte[] theBlob)
		{
			int num = 0;
			int num2 = SerializedValue.ParseInt32(theBlob, ref num);
			if (num2 != 1)
			{
				throw new InvalidSerializedFormatException("Wrong version.");
			}
			int num3 = SerializedValue.ParseInt32(theBlob, ref num);
			if ((theBlob.Length - num) / 7 < num3)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			if (num3 < 0)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			FolderHierarchyBlob[] array = new FolderHierarchyBlob[num3];
			for (int i = 0; i < num3; i++)
			{
				int num4 = SerializedValue.ParseInt32(theBlob, ref num);
				int num5 = SerializedValue.ParseInt32(theBlob, ref num);
				byte[] array2 = SerializedValue.ParseBinary(theBlob, ref num);
				byte[] array3 = SerializedValue.ParseBinary(theBlob, ref num);
				string text = SerializedValue.ParseString(theBlob, ref num);
				int num6 = SerializedValue.ParseInt32(theBlob, ref num);
				int num7 = SerializedValue.ParseInt32(theBlob, ref num);
				array[i] = new FolderHierarchyBlob(num4, num5, array2, array3, text, num6, num7);
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("MailboxPartitionNumber:[");
			stringBuilder.AppendAsString(this.MailboxPartitionNumber);
			stringBuilder.Append("] MailboxNumber:[");
			stringBuilder.AppendAsString(this.MailboxNumber);
			stringBuilder.Append("] ParentFolderId:[");
			stringBuilder.AppendAsString(this.ParentFolderId);
			stringBuilder.Append("] FolderId:[");
			stringBuilder.AppendAsString(this.FolderId);
			stringBuilder.Append("] DisplayName:[");
			stringBuilder.AppendAsString(this.DisplayName);
			stringBuilder.Append("] Depth:[");
			stringBuilder.AppendAsString(this.Depth);
			stringBuilder.Append("] SortPosition:[");
			stringBuilder.AppendAsString(this.SortPosition);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string FolderHierarchyBlobAsString(byte[] theBlob)
		{
			FolderHierarchyBlob[] array = FolderHierarchyBlob.Deserialize(theBlob);
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("FolderHierarchyBlob:[");
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("[");
				stringBuilder.Append(array[i].ToString());
				stringBuilder.Append("]");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public const int BlobVersion = 1;

		private int mailboxPartitionNumber;

		private int mailboxNumber;

		private byte[] parentFolderId;

		private byte[] folderId;

		private string displayName;

		private int depth;

		private int sortPosition;
	}
}
