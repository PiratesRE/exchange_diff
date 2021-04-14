using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class CompositeProperty
	{
		internal CompositeProperty()
		{
		}

		internal CompositeProperty(int integer, DateTime date)
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

		internal static CompositeProperty Parse(byte[] propertyBytes)
		{
			return CompositeProperty.Parse(propertyBytes, false);
		}

		internal static CompositeProperty Parse(byte[] propertyBytes, bool useFileTime)
		{
			if (propertyBytes.Length != 12)
			{
				throw new ArgumentException("The length of the composite property must be 12. It is: " + propertyBytes.Length);
			}
			CompositeProperty compositeProperty = new CompositeProperty();
			compositeProperty.integer = BitConverter.ToInt32(propertyBytes, 0);
			if (useFileTime)
			{
				long num = BitConverter.ToInt64(propertyBytes, 4);
				if (num == 0L)
				{
					compositeProperty.date = new DateTime?(DateTime.MinValue);
				}
				else
				{
					compositeProperty.date = new DateTime?(DateTime.FromFileTimeUtc(num));
				}
			}
			else
			{
				compositeProperty.date = new DateTime?(DateTime.FromBinary(BitConverter.ToInt64(propertyBytes, 4)));
			}
			return compositeProperty;
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
				if (this.date.Value >= CompositeProperty.minFileTime)
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
