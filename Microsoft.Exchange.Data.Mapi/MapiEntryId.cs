using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Mapi.Common;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class MapiEntryId : IEquatable<MapiEntryId>, IComparable<MapiEntryId>, IComparable
	{
		public static MapiEntryId Parse(string input)
		{
			return new MapiEntryId(input);
		}

		public static explicit operator byte[](MapiEntryId source)
		{
			return (byte[])((null == source) ? null : source.binaryEntryId.Clone());
		}

		public static bool operator ==(MapiEntryId operand1, MapiEntryId operand2)
		{
			return object.Equals(operand1, operand2);
		}

		public static bool operator !=(MapiEntryId operand1, MapiEntryId operand2)
		{
			return !object.Equals(operand1, operand2);
		}

		public bool Equals(MapiEntryId other)
		{
			if (object.ReferenceEquals(null, other) || this.binaryEntryId.Length != other.binaryEntryId.Length)
			{
				return false;
			}
			if (this.binaryEntryId == other.binaryEntryId)
			{
				return true;
			}
			int num = 0;
			while (this.binaryEntryId.Length > num)
			{
				if (this.binaryEntryId[num] != other.binaryEntryId[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public int CompareTo(MapiEntryId other)
		{
			if (null == other)
			{
				return 1;
			}
			int num = 0;
			while (num < this.binaryEntryId.Length && num < other.binaryEntryId.Length)
			{
				if (this.binaryEntryId[num] != other.binaryEntryId[num])
				{
					return (int)(this.binaryEntryId[num] - other.binaryEntryId[num]);
				}
				num++;
			}
			return this.binaryEntryId.Length - other.binaryEntryId.Length;
		}

		int IComparable.CompareTo(object obj)
		{
			return this.CompareTo(obj as MapiEntryId);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MapiEntryId);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = new int?(this.binaryEntryId.Length);
				int num = 0;
				while (this.binaryEntryId.Length > num)
				{
					int num2;
					if ((num2 = (num & 3)) == 0)
					{
						this.hashCode = (this.hashCode << 13 | (int)((uint)this.hashCode.Value >> 19));
					}
					this.hashCode ^= (int)this.binaryEntryId[num] << (num2 << 3);
					num++;
				}
			}
			return this.hashCode.Value;
		}

		public override string ToString()
		{
			if (this.literalEntryId == null)
			{
				StringBuilder stringBuilder = new StringBuilder(this.binaryEntryId.Length << 1);
				int num = 0;
				while (this.binaryEntryId.Length > num)
				{
					stringBuilder.AppendFormat("{0:X2}", this.binaryEntryId[num]);
					num++;
				}
				this.literalEntryId = stringBuilder.ToString();
			}
			return this.literalEntryId;
		}

		public MapiEntryId(byte[] binaryEntryId)
		{
			if (binaryEntryId == null || binaryEntryId.Length == 0)
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			this.binaryEntryId = (byte[])binaryEntryId.Clone();
		}

		public MapiEntryId(string literalEntryId)
		{
			if (literalEntryId == null)
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			literalEntryId = literalEntryId.Trim();
			if (string.IsNullOrEmpty(literalEntryId))
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			if ((1 & literalEntryId.Length) != 0)
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			byte[] array = new byte[literalEntryId.Length >> 1];
			int num = 0;
			while (array.Length > num)
			{
				if (!byte.TryParse(literalEntryId.Substring(num << 1, 2), NumberStyles.HexNumber, null, out array[num]))
				{
					throw new FormatException(Strings.ExceptionFormatNotSupported);
				}
				num++;
			}
			this.literalEntryId = literalEntryId;
			this.binaryEntryId = array;
		}

		private readonly byte[] binaryEntryId;

		private string literalEntryId;

		private int? hashCode;
	}
}
