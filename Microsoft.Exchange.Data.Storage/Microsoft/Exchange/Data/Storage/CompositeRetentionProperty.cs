using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CompositeRetentionProperty
	{
		internal CompositeRetentionProperty()
		{
		}

		internal CompositeRetentionProperty(int integer, DateTime date)
		{
			this.integer = integer;
			this.date = new DateTime?(date);
		}

		internal int Integer
		{
			get
			{
				return this.integer;
			}
			set
			{
				this.integer = value;
			}
		}

		internal DateTime? Date
		{
			get
			{
				return this.date;
			}
			set
			{
				this.date = value;
			}
		}

		internal static CompositeRetentionProperty Parse(byte[] propertyBytes)
		{
			return CompositeRetentionProperty.Parse(propertyBytes, false);
		}

		internal static CompositeRetentionProperty Parse(byte[] propertyBytes, bool useFileTime)
		{
			if (propertyBytes.Length != 12)
			{
				throw new ArgumentException("The length of the composite property must be 12. It is: " + propertyBytes.Length);
			}
			CompositeRetentionProperty compositeRetentionProperty = new CompositeRetentionProperty();
			compositeRetentionProperty.integer = BitConverter.ToInt32(propertyBytes, 0);
			if (useFileTime)
			{
				long num = BitConverter.ToInt64(propertyBytes, 4);
				if (num == 0L)
				{
					compositeRetentionProperty.date = new DateTime?(DateTime.MinValue);
				}
				else
				{
					compositeRetentionProperty.date = new DateTime?(DateTime.FromFileTimeUtc(num));
				}
			}
			else
			{
				compositeRetentionProperty.date = new DateTime?(DateTime.FromBinary(BitConverter.ToInt64(propertyBytes, 4)));
			}
			return compositeRetentionProperty;
		}

		internal byte[] GetBytes()
		{
			return this.GetBytes(false);
		}

		internal byte[] GetBytes(bool useUtc)
		{
			byte[] bytes = BitConverter.GetBytes(this.integer);
			byte[] bytes2;
			if (useUtc)
			{
				long value = 0L;
				if (this.date.Value >= CompositeRetentionProperty.minFileTime)
				{
					value = this.date.Value.ToFileTimeUtc();
				}
				bytes2 = BitConverter.GetBytes(value);
			}
			else
			{
				bytes2 = BitConverter.GetBytes(this.date.Value.ToBinary());
			}
			byte[] array = new byte[bytes.Length + bytes2.Length];
			Array.Copy(bytes, array, bytes.Length);
			Array.Copy(bytes2, 0, array, bytes.Length, bytes2.Length);
			return array;
		}

		private static DateTime minFileTime = new DateTime(1601, 1, 1);

		private int integer;

		private DateTime? date;
	}
}
