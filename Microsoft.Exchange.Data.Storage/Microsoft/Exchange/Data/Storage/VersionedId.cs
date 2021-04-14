using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class VersionedId : StoreId, IEquatable<VersionedId>, IComparable<VersionedId>, IComparable
	{
		public VersionedId(StoreObjectId itemId, byte[] changeKey)
		{
			if (changeKey.Length > 255)
			{
				throw new CorruptDataException(ServerStrings.ExChangeKeyTooLong);
			}
			if (itemId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			this.ItemId = itemId;
			this.ChangeKey = (byte[])changeKey.Clone();
		}

		public VersionedId(byte[] byteArray)
		{
			int num = 0;
			bool flag = false;
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray", ServerStrings.ExInvalidIdFormat);
			}
			if (byteArray.Length <= 1)
			{
				flag = true;
			}
			else
			{
				num = (int)byteArray[0];
				if (byteArray.Length <= num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.ItemId = StoreObjectId.Parse(byteArray, num + 1);
				this.ChangeKey = new byte[num];
				for (int i = 0; i < num; i++)
				{
					this.ChangeKey[i] = byteArray[1 + i];
				}
				return;
			}
			throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
		}

		public VersionedId(string base64String) : this(StoreId.Base64ToByteArray(base64String))
		{
		}

		internal VersionedId(byte[] storeObjectIdByteArray, byte[] changeKeyByteArray)
		{
			if (changeKeyByteArray.Length > 255)
			{
				throw new CorruptDataException(ServerStrings.ExChangeKeyTooLong);
			}
			this.ItemId = StoreObjectId.Parse(storeObjectIdByteArray, 0);
			this.ChangeKey = (byte[])changeKeyByteArray.Clone();
		}

		internal VersionedId(string storeObjectIdString, string changeKeyString) : this(StoreId.Base64ToByteArray(storeObjectIdString), StoreId.Base64ToByteArray(changeKeyString))
		{
		}

		public StoreObjectId ObjectId
		{
			get
			{
				return this.ItemId;
			}
		}

		public static VersionedId Deserialize(string base64Id)
		{
			if (base64Id == null)
			{
				throw new ArgumentNullException("base64Id");
			}
			return new VersionedId(base64Id);
		}

		public static VersionedId Deserialize(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			return new VersionedId(byteArray);
		}

		public static VersionedId Deserialize(string base64UniqueId, string base64ChangeKey)
		{
			if (base64UniqueId == null || base64ChangeKey == null)
			{
				throw new ArgumentNullException();
			}
			return new VersionedId(base64UniqueId, base64ChangeKey);
		}

		public static VersionedId Deserialize(byte[] byteArrayUniqueId, byte[] byteArrayChangeKey)
		{
			if (byteArrayUniqueId == null || byteArrayChangeKey == null)
			{
				throw new ArgumentNullException();
			}
			return new VersionedId(byteArrayUniqueId, byteArrayChangeKey);
		}

		public static VersionedId Deserialize(byte[] providerSpecificId, byte[] byteArrayChangeKey, StoreObjectType objectType)
		{
			if (providerSpecificId == null || byteArrayChangeKey == null)
			{
				throw new ArgumentNullException();
			}
			StoreObjectId itemId = StoreObjectId.FromProviderSpecificId(providerSpecificId, objectType);
			return new VersionedId(itemId, byteArrayChangeKey);
		}

		public static VersionedId FromStoreObjectId(StoreObjectId storeObjectId, byte[] changeKey)
		{
			Util.ThrowOnNullArgument(storeObjectId, "storeObjectId");
			Util.ThrowOnNullArgument(changeKey, "changeKey");
			return new VersionedId(storeObjectId, changeKey);
		}

		public override bool Equals(object id)
		{
			VersionedId other = id as VersionedId;
			return this.Equals(other);
		}

		public override bool Equals(StoreId id)
		{
			VersionedId other = id as VersionedId;
			return this.Equals(other);
		}

		public bool Equals(VersionedId other)
		{
			return other != null && this.ObjectId.Equals(other.ObjectId) && ArrayComparer<byte>.Comparer.Equals(this.ChangeKey, other.ChangeKey);
		}

		public int CompareTo(object o)
		{
			if (o == null)
			{
				return 1;
			}
			VersionedId versionedId = o as VersionedId;
			if (versionedId == null)
			{
				throw new ArgumentException();
			}
			return this.CompareTo(versionedId);
		}

		public int CompareTo(VersionedId v)
		{
			if (v == null)
			{
				return 1;
			}
			Type type = this.ItemId.GetType();
			Type type2 = v.ItemId.GetType();
			if (!type.Equals(type2))
			{
				return type.FullName.CompareTo(type2.FullName);
			}
			int num = this.ItemId.CompareTo(v.ItemId);
			if (num == 0)
			{
				return ArrayComparer<byte>.Comparer.Compare(this.ChangeKey, v.ChangeKey);
			}
			return num;
		}

		public override string ToBase64String()
		{
			return Convert.ToBase64String(this.GetBytes());
		}

		public override byte[] GetBytes()
		{
			byte[] bytes = this.ItemId.GetBytes();
			byte[] array = new byte[1 + this.ChangeKey.Length + bytes.Length];
			array[0] = (byte)this.ChangeKey.Length;
			this.ChangeKey.CopyTo(array, 1);
			bytes.CopyTo(array, 1 + this.ChangeKey.Length);
			return array;
		}

		public string ChangeKeyAsBase64String()
		{
			return Convert.ToBase64String(this.ChangeKey);
		}

		public byte[] ChangeKeyAsByteArray()
		{
			return (byte[])this.ChangeKey.Clone();
		}

		public override string ToString()
		{
			return this.ToBase64String();
		}

		public override int GetHashCode()
		{
			int num = this.ItemId.GetHashCode();
			for (int i = 0; i < this.ChangeKey.Length; i++)
			{
				num ^= (int)this.ChangeKey[i] << 8 * (i % 4);
			}
			return num;
		}

		protected readonly byte[] ChangeKey;

		protected readonly StoreObjectId ItemId;
	}
}
