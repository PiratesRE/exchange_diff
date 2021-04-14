using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ConversationIndex : IEquatable<ConversationIndex>
	{
		private ConversationIndex(byte[] bytes)
		{
			Util.ThrowOnNullArgument(bytes, "bytes");
			this.bytes = bytes;
			this.readonlyBytes = null;
			this.isParsed = false;
			this.guid = Guid.Empty;
			this.components = null;
			this.readonlyComponents = null;
			this.hasHash = false;
			this.hashCode = 0;
		}

		public static ConversationIndex CreateNew()
		{
			return ConversationIndex.Create(Guid.NewGuid());
		}

		public static ConversationIndex Create(IList<byte> bytes)
		{
			Util.ThrowOnNullArgument(bytes, "bytes");
			if (!ConversationIndex.IsValidConversationIndex(bytes))
			{
				throw new ArgumentException("bytes doesn't correspond to a valid conversation index");
			}
			return new ConversationIndex(ConversationIndex.CloneBytes(bytes));
		}

		public static ConversationId RetrieveConversationId(IList<byte> conversationIndexInBytes)
		{
			return ConversationId.Create(ConversationIndex.Create(conversationIndexInBytes));
		}

		public static bool TryCreate(IList<byte> bytes, out ConversationIndex conversationIndex)
		{
			conversationIndex = ConversationIndex.Empty;
			if (bytes == null || !ConversationIndex.IsValidConversationIndex(bytes))
			{
				return false;
			}
			conversationIndex = new ConversationIndex(ConversationIndex.CloneBytes(bytes));
			return true;
		}

		public static ConversationIndex Create(ConversationId conversationId)
		{
			Util.ThrowOnNullArgument(conversationId, "conversationId");
			return ConversationIndex.Create(new Guid(conversationId.GetBytes()));
		}

		public static ConversationIndex Create(Guid guid)
		{
			return ConversationIndex.Create(guid, ExDateTime.UtcNow);
		}

		public static ConversationIndex Create(Guid guid, ExDateTime header)
		{
			byte[] array = new byte[22];
			array[0] = 1;
			long num = ((DateTime)header.ToUtc()).ToFileTime();
			int num2 = 56;
			for (int i = 1; i < 6; i++)
			{
				array[i] = (byte)(num >> num2 & 255L);
				num2 -= 8;
			}
			byte[] array2 = guid.ToByteArray();
			for (int j = 0; j < array2.Length; j++)
			{
				array[6 + j] = array2[j];
			}
			return new ConversationIndex(array);
		}

		public static ConversationIndex CreateFromParent(Guid conversationIdGuid, ConversationIndex parentIndex)
		{
			return ConversationIndex.CreateFromParent(parentIndex.bytes, ExDateTime.UtcNow).UpdateGuid(conversationIdGuid);
		}

		public static ConversationIndex CreateFromParent(IList<byte> parentBytes)
		{
			return ConversationIndex.CreateFromParent(parentBytes, ExDateTime.UtcNow);
		}

		public static ConversationIndex CreateFromParent(IList<byte> parentBytes, ExDateTime messageTime)
		{
			if (parentBytes == null || !ConversationIndex.IsValidConversationIndex(parentBytes))
			{
				return ConversationIndex.CreateNew();
			}
			int count = parentBytes.Count;
			byte[] array = new byte[count + 5];
			parentBytes.CopyTo(array, 0);
			ulong num = (ulong)(messageTime.ToFileTime() & -65536L);
			ulong lastFileTime = ConversationIndex.GetLastFileTime(parentBytes);
			if (num > lastFileTime)
			{
				num -= lastFileTime;
			}
			else
			{
				num = lastFileTime - num;
			}
			uint num2;
			if ((num >> 32 & 16646144UL) == 0UL)
			{
				num2 = (uint)(num >> 32 & 131071UL);
				num2 <<= 14;
				num2 |= (uint)((num & (ulong)-262144) >> 18);
				array[count] = (byte)((num2 & 4278190080U) >> 24);
			}
			else
			{
				num2 = (uint)(num >> 32 & 4194303UL);
				num2 <<= 9;
				num2 |= (uint)((num & (ulong)-8388608) >> 23);
				array[count] = (byte)((num2 & 2130706432U) >> 24 | 128U);
			}
			array[count + 1] = (byte)((num2 & 16711680U) >> 16);
			array[count + 2] = (byte)((num2 & 65280U) >> 8);
			array[count + 3] = (byte)(num2 & 255U);
			array[count + 4] = (byte)(messageTime.UtcTicks & 255L);
			return new ConversationIndex(array);
		}

		public static bool operator ==(ConversationIndex index1, ConversationIndex index2)
		{
			return index1.Equals(index2);
		}

		public static bool operator !=(ConversationIndex index1, ConversationIndex index2)
		{
			return !index1.Equals(index2);
		}

		public static bool CheckStageValue(ConversationIndex.FixupStage fixupStage, ConversationIndex.FixupStage expectedStage)
		{
			if (expectedStage == ConversationIndex.FixupStage.Unknown)
			{
				return fixupStage == ConversationIndex.FixupStage.Unknown;
			}
			return (fixupStage & expectedStage) == expectedStage;
		}

		public static bool WasMessageEverProcessed(ICorePropertyBag propertyBag)
		{
			return propertyBag.GetValueAsNullable<bool>(ItemSchema.ConversationIndexTracking) != null;
		}

		private static byte[] CloneBytes(IList<byte> bytes)
		{
			byte[] array = new byte[bytes.Count];
			bytes.CopyTo(array, 0);
			return array;
		}

		private static byte[] ExtractBytes(byte[] bytes, int start, int count)
		{
			Util.ThrowOnNullArgument(bytes, "bytes");
			if (start < 0)
			{
				throw new ArgumentException("start must be > 0");
			}
			if (count <= 0)
			{
				throw new ArgumentException("count must be >= 0");
			}
			byte[] array = new byte[count];
			Array.Copy(bytes, start, array, 0, count);
			return array;
		}

		private static bool IsValidConversationIndex(IList<byte> conversationIndex)
		{
			return conversationIndex.Count >= 22 && (conversationIndex.Count - 22) % 5 == 0;
		}

		private static ulong GetLastFileTime(IList<byte> parentBytes)
		{
			ulong num = 0UL;
			for (int i = 1; i < 6; i++)
			{
				num |= (ulong)parentBytes[i];
				num <<= 8;
			}
			num <<= 8;
			for (int j = 22; j < parentBytes.Count; j += 5)
			{
				ulong num2;
				if ((parentBytes[j] & 128) == 128)
				{
					num2 = (ulong)((int)(parentBytes[j] & 127) << 15 | (int)parentBytes[j + 1] << 7 | parentBytes[j + 2] >> 1);
					num2 <<= 32;
					num2 |= (ulong)((int)parentBytes[j + 2] << 31 | (int)parentBytes[j + 3] << 23);
				}
				else
				{
					num2 = (ulong)((int)(parentBytes[j] & 127) << 10 | (int)parentBytes[j + 1] << 2 | parentBytes[j + 2] >> 6);
					num2 <<= 32;
					num2 |= (ulong)((int)parentBytes[j + 2] << 26 | (int)parentBytes[j + 3] << 18);
				}
				num += num2;
			}
			return num;
		}

		public static bool IsFixUpCreatingNewConversation(ConversationIndex.FixupStage fixupStage)
		{
			ConversationIndex.FixupStage fixupStage2 = ConversationIndex.FixupStage.H3 | ConversationIndex.FixupStage.H4 | ConversationIndex.FixupStage.H8 | ConversationIndex.FixupStage.H11 | ConversationIndex.FixupStage.H12 | ConversationIndex.FixupStage.SC;
			return ConversationIndex.CheckStageValue(fixupStage2, fixupStage);
		}

		public static bool IsFixupAddingOutOfOrderMessageToConversation(ConversationIndex.FixupStage fixupStage)
		{
			return ConversationIndex.CheckStageValue(fixupStage, ConversationIndex.FixupStage.H6);
		}

		public static bool CompareTopics(string incomingTopic, string foundTopic)
		{
			return (string.IsNullOrEmpty(foundTopic) && string.IsNullOrEmpty(incomingTopic)) || (foundTopic != null && incomingTopic != null && 0 == string.Compare(incomingTopic, foundTopic, StringComparison.OrdinalIgnoreCase));
		}

		public ConversationIndex UpdateGuid(Guid guid)
		{
			return this.UpdateGuid(guid.ToByteArray());
		}

		public ConversationIndex UpdateGuid(ConversationId conversationId)
		{
			return this.UpdateGuid(conversationId.GetBytes());
		}

		public ConversationIndex UpdateHeader(byte[] header)
		{
			Util.ThrowOnNullOrEmptyArgument(header, "header");
			if (header.Length != 5)
			{
				throw new ArgumentException("header must be 5 bytes long");
			}
			byte[] destinationArray = ConversationIndex.CloneBytes(this.bytes);
			Array.Copy(header, 0, destinationArray, 1, 5);
			return new ConversationIndex(destinationArray);
		}

		public IList<byte> Bytes
		{
			get
			{
				if (this.readonlyBytes == null)
				{
					this.readonlyBytes = new System.Collections.ObjectModel.ReadOnlyCollection<byte>(this.bytes);
				}
				return this.readonlyBytes;
			}
		}

		public byte[] ToByteArray()
		{
			return ConversationIndex.CloneBytes(this.bytes);
		}

		public IList<byte[]> Components
		{
			get
			{
				this.EnsureParsed();
				return this.readonlyComponents;
			}
		}

		public byte[] Header
		{
			get
			{
				return this.Components[0];
			}
		}

		public Guid Guid
		{
			get
			{
				this.EnsureParsed();
				return this.guid;
			}
		}

		public bool IsParentOf(ConversationIndex childIndex)
		{
			Util.ThrowOnNullArgument(childIndex, "childIndex");
			return childIndex.bytes.Length == this.bytes.Length + 5 && this.IsAncestorOf(childIndex);
		}

		public bool IsAncestorOf(ConversationIndex childIndex)
		{
			Util.ThrowOnNullArgument(childIndex, "childIndex");
			if (this.Guid.CompareTo(childIndex.Guid) != 0)
			{
				return false;
			}
			int num = this.bytes.Length;
			if (childIndex.bytes.Length < num + 5)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (this.bytes[i] != childIndex.bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		private ConversationIndex UpdateGuid(byte[] guidBytes)
		{
			byte[] destinationArray = ConversationIndex.CloneBytes(this.bytes);
			Array.Copy(guidBytes, 0, destinationArray, 6, 16);
			return new ConversationIndex(destinationArray);
		}

		private void EnsureParsed()
		{
			if (this.isParsed)
			{
				return;
			}
			int num = (this.bytes.Length - 16 - 1) / 5;
			this.components = new List<byte[]>(num);
			this.components.Add(ConversationIndex.ExtractBytes(this.bytes, 1, 5));
			this.guid = new Guid(ConversationIndex.ExtractBytes(this.bytes, 6, 16));
			for (int i = 1; i < num; i++)
			{
				this.components.Add(ConversationIndex.ExtractBytes(this.bytes, 22 + (i - 1) * 5, 5));
			}
			this.readonlyComponents = new System.Collections.ObjectModel.ReadOnlyCollection<byte[]>(this.components);
			this.isParsed = true;
		}

		public override bool Equals(object o)
		{
			if (!(o is ConversationIndex))
			{
				return false;
			}
			ConversationIndex o2 = (ConversationIndex)o;
			return this.Equals(o2);
		}

		public bool Equals(ConversationIndex o)
		{
			return ArrayComparer<byte>.Comparer.Equals(this.bytes, o.bytes);
		}

		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				int num = 0;
				int num2 = this.bytes.Length;
				for (int i = 0; i < num2; i++)
				{
					num ^= (int)this.bytes[i] << 8 * (i & 3);
				}
				this.hashCode = num;
				this.hasHash = true;
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return GlobalObjectId.ByteArrayToHexString(this.bytes);
		}

		public static ConversationIndex GenerateFromPhoneNumber(string number)
		{
			E164Number e164Number;
			if (!E164Number.TryParse(number, out e164Number))
			{
				return ConversationIndex.Empty;
			}
			int num = "472e2878-19b1-4ac1-a21a-".Length + 12;
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append("472e2878-19b1-4ac1-a21a-");
			if (e164Number.SignificantNumber.Length <= 12)
			{
				stringBuilder.Append(e164Number.SignificantNumber);
			}
			else
			{
				stringBuilder.Append(e164Number.SignificantNumber.Substring(e164Number.SignificantNumber.Length - 12));
			}
			while (stringBuilder.Length < num)
			{
				stringBuilder.Append('f');
			}
			return ConversationIndex.Create(new Guid(stringBuilder.ToString()));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ConversationIndex()
		{
			byte[] array = new byte[22];
			array[0] = 1;
			ConversationIndex.Empty = new ConversationIndex(array);
		}

		private const int GuidLength = 16;

		private const int ComponentLength = 5;

		private const int HeaderLength = 22;

		private const byte Reserved = 1;

		private const int CountedPhoneNumberLength = 12;

		private const string SmsConversationIndexGuidPrefix = "472e2878-19b1-4ac1-a21a-";

		public static readonly ConversationIndex Empty;

		private readonly byte[] bytes;

		private System.Collections.ObjectModel.ReadOnlyCollection<byte> readonlyBytes;

		private bool isParsed;

		private Guid guid;

		private List<byte[]> components;

		private System.Collections.ObjectModel.ReadOnlyCollection<byte[]> readonlyComponents;

		private bool hasHash;

		private int hashCode;

		[Flags]
		public enum FixupStage
		{
			Unknown = 0,
			H1 = 1,
			H2 = 2,
			H3 = 4,
			H4 = 8,
			H5 = 16,
			H6 = 32,
			H7 = 64,
			H8 = 128,
			H9 = 256,
			H10 = 512,
			H11 = 1024,
			H12 = 2048,
			H13 = 4096,
			H14 = 8192,
			Error = 262144,
			M1 = 524288,
			M2 = 1048576,
			M3 = 2097152,
			M4 = 4194304,
			L1 = 67108864,
			S1 = 134217728,
			S2 = 268435456,
			SC = 536870912,
			TC = 1073741824
		}
	}
}
