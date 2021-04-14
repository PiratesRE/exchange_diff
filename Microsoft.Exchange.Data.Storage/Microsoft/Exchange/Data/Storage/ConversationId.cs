using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ConversationId : StoreId, IEquatable<ConversationId>, IComparable
	{
		private ConversationId(byte[] bytes)
		{
			Util.ThrowOnNullArgument(bytes, "bytes");
			if (bytes.Length != 16)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			this.bytes = bytes;
			this.hashCode = 0;
		}

		public static ConversationId Create(string base64String)
		{
			return new ConversationId(StoreId.Base64ToByteArray(base64String));
		}

		public static ConversationId Create(byte[] bytes)
		{
			return new ConversationId(ConversationId.CloneBytes(bytes));
		}

		public static ConversationId Create(Guid guid)
		{
			return new ConversationId(guid.ToByteArray());
		}

		public static ConversationId Create(ConversationIndex index)
		{
			return new ConversationId(index.Guid.ToByteArray());
		}

		public override string ToBase64String()
		{
			return Convert.ToBase64String(this.bytes);
		}

		public override byte[] GetBytes()
		{
			return ConversationId.CloneBytes(this.bytes);
		}

		public override bool Equals(object o)
		{
			ConversationId o2 = o as ConversationId;
			return this.Equals(o2);
		}

		public override bool Equals(StoreId o)
		{
			ConversationId o2 = o as ConversationId;
			return this.Equals(o2);
		}

		public bool Equals(ConversationId o)
		{
			return o != null && ArrayComparer<byte>.Comparer.Equals(this.bytes, o.bytes);
		}

		public override int GetHashCode()
		{
			this.hashCode = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				this.hashCode ^= (int)this.bytes[i] << 8 * (i % 4);
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return GlobalObjectId.ByteArrayToHexString(this.bytes);
		}

		private static byte[] CloneBytes(IList<byte> bytes)
		{
			byte[] array = new byte[bytes.Count];
			bytes.CopyTo(array, 0);
			return array;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			ConversationId conversationId = obj as ConversationId;
			if (obj == null)
			{
				throw new ArgumentException();
			}
			return ArrayComparer<byte>.Comparer.Compare(this.bytes, conversationId.bytes);
		}

		private byte[] bytes;

		private int hashCode;
	}
}
