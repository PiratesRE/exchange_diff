using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class PersonId : StoreId, IEquatable<PersonId>
	{
		private PersonId(byte[] bytes)
		{
			Util.ThrowOnNullArgument(bytes, "bytes");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(bytes.Length, 16, "bytes");
			Util.ThrowOnArgumentInvalidOnGreaterThan(bytes.Length, 16, "bytes");
			this.bytes = bytes;
			this.hashCode = PersonId.ComputeHashCode(bytes);
		}

		public static PersonId CreateNew()
		{
			return new PersonId(Guid.NewGuid().ToByteArray());
		}

		public static PersonId Create(string base64String)
		{
			return new PersonId(StoreId.Base64ToByteArray(base64String));
		}

		public static PersonId Create(byte[] bytes)
		{
			return new PersonId(PersonId.CloneBytes(bytes));
		}

		public static long TraceId(PersonId personId)
		{
			return (long)((personId == null) ? 0 : personId.GetHashCode());
		}

		public override string ToBase64String()
		{
			return Convert.ToBase64String(this.bytes);
		}

		public override byte[] GetBytes()
		{
			return PersonId.CloneBytes(this.bytes);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as PersonId);
		}

		public override bool Equals(StoreId other)
		{
			return this.Equals(other as PersonId);
		}

		public bool Equals(PersonId other)
		{
			return other != null && (object.ReferenceEquals(this, other) || ArrayComparer<byte>.Comparer.Equals(this.bytes, other.bytes));
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.ToBase64String();
		}

		private static int ComputeHashCode(byte[] bytes)
		{
			int num = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				num ^= (int)bytes[i] << 8 * (i % 4);
			}
			return num;
		}

		private static byte[] CloneBytes(byte[] bytes)
		{
			byte[] array = new byte[bytes.Length];
			bytes.CopyTo(array, 0);
			return array;
		}

		private const int BytesInPersonId = 16;

		private readonly byte[] bytes;

		private readonly int hashCode;
	}
}
