using System;
using Microsoft.Exchange.Data;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataColumn<T> : DataColumn where T : struct, IEquatable<T>
	{
		internal DataColumn(JET_coltyp type, bool fixedSize) : base(type, fixedSize)
		{
		}

		internal DataColumn(JET_coltyp type, bool fixedSize, int size) : base(type, fixedSize, size)
		{
		}

		private static DataColumn<T>.Formatter<T> Default
		{
			get
			{
				DataColumn<T>.Formatter<T> formatter = DataColumn<T>.formatter;
				if (formatter != null)
				{
					return formatter;
				}
				return DataColumn<T>.CreateFormatter();
			}
		}

		public T ReadFromCursor(DataTableCursor cursor)
		{
			if (typeof(T) == typeof(string))
			{
				string text = base.StringFromCursor(cursor);
				return (T)((object)text);
			}
			if (typeof(T) == typeof(bool))
			{
				bool flag = base.BoolFromCursor(cursor) ?? false;
				return (T)((object)flag);
			}
			if (typeof(T) == typeof(long))
			{
				long num = base.Int64FromCursor(cursor) ?? 0L;
				return (T)((object)num);
			}
			byte[] data = base.BytesFromCursor(cursor, false, 1);
			return DataColumn<T>.Default.FromBytes(data);
		}

		public void WriteToCursor(DataTableCursor cursor, T value)
		{
			try
			{
				Api.SetColumn(cursor.Session, cursor.TableId, base.ColumnId, this.BytesFromValue(value));
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
		}

		internal override ColumnCache NewCacheCell()
		{
			return new ColumnCacheValueType<T>();
		}

		internal override void ColumnValueToCache(ColumnValue data, ColumnCache cache)
		{
			BytesColumnValue bytesColumnValue = data as BytesColumnValue;
			if (bytesColumnValue == null)
			{
				ColumnValueOfStruct<T> columnValueOfStruct = (ColumnValueOfStruct<T>)data;
				((ColumnCache<T>)cache).Value = (columnValueOfStruct.Value ?? default(T));
				return;
			}
			if (bytesColumnValue.Value == null)
			{
				((ColumnCache<T>)cache).Value = default(T);
				return;
			}
			if (typeof(T) == typeof(IPvxAddress))
			{
				((ColumnCache<IPvxAddress>)cache).Value = DataColumn<IPvxAddress>.Default.FromBytes(bytesColumnValue.Value);
				return;
			}
			((ColumnCache<byte[]>)cache).Value = bytesColumnValue.Value;
		}

		internal override byte[] BytesFromCache(ColumnCache cache)
		{
			return this.BytesFromValue(((ColumnCache<T>)cache).Value);
		}

		internal byte[] BytesFromValue(T value)
		{
			return DataColumn<T>.Default.ToBytes(value);
		}

		private static DataColumn<T>.Formatter<T> CreateFormatter()
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				DataColumn<T>.FormatterInt32 formatterInt = new DataColumn<T>.FormatterInt32();
				DataColumn<T>.formatter = (formatterInt as DataColumn<T>.Formatter<T>);
			}
			else if (typeFromHandle == typeof(long))
			{
				DataColumn<T>.FormatterInt64 formatterInt2 = new DataColumn<T>.FormatterInt64();
				DataColumn<T>.formatter = (formatterInt2 as DataColumn<T>.Formatter<T>);
			}
			else if (typeFromHandle == typeof(byte))
			{
				DataColumn<T>.FormatterByte formatterByte = new DataColumn<T>.FormatterByte();
				DataColumn<T>.formatter = (formatterByte as DataColumn<T>.Formatter<T>);
			}
			else if (typeFromHandle == typeof(bool))
			{
				DataColumn<T>.FormatterBool formatterBool = new DataColumn<T>.FormatterBool();
				DataColumn<T>.formatter = (formatterBool as DataColumn<T>.Formatter<T>);
			}
			else if (typeFromHandle == typeof(Guid))
			{
				DataColumn<T>.FormatterGuid formatterGuid = new DataColumn<T>.FormatterGuid();
				DataColumn<T>.formatter = (formatterGuid as DataColumn<T>.Formatter<T>);
			}
			else if (typeFromHandle == typeof(DateTime))
			{
				DataColumn<T>.FormatterDateTime formatterDateTime = new DataColumn<T>.FormatterDateTime();
				DataColumn<T>.formatter = (formatterDateTime as DataColumn<T>.Formatter<T>);
			}
			else
			{
				if (!(typeFromHandle == typeof(IPvxAddress)))
				{
					throw new InvalidCastException();
				}
				DataColumn<T>.FormatterIPAddressBytes formatterIPAddressBytes = new DataColumn<T>.FormatterIPAddressBytes();
				DataColumn<T>.formatter = (formatterIPAddressBytes as DataColumn<T>.Formatter<T>);
			}
			return DataColumn<T>.formatter;
		}

		private static DataColumn<T>.Formatter<T> formatter;

		private abstract class Formatter<Td>
		{
			public abstract Td FromBytes(byte[] data);

			public abstract byte[] ToBytes(Td value);
		}

		private class FormatterInt32 : DataColumn<T>.Formatter<int>
		{
			public override int FromBytes(byte[] data)
			{
				if (data != null)
				{
					return BitConverter.ToInt32(data, 0);
				}
				return 0;
			}

			public override byte[] ToBytes(int value)
			{
				return BitConverter.GetBytes(value);
			}
		}

		private class FormatterInt64 : DataColumn<T>.Formatter<long>
		{
			public override long FromBytes(byte[] data)
			{
				if (data != null)
				{
					return BitConverter.ToInt64(data, 0);
				}
				return 0L;
			}

			public override byte[] ToBytes(long value)
			{
				return BitConverter.GetBytes(value);
			}
		}

		private class FormatterByte : DataColumn<T>.Formatter<byte>
		{
			public override byte FromBytes(byte[] data)
			{
				if (data != null)
				{
					return data[0];
				}
				return 0;
			}

			public override byte[] ToBytes(byte value)
			{
				return new byte[]
				{
					value
				};
			}
		}

		private class FormatterBool : DataColumn<T>.Formatter<bool>
		{
			public override bool FromBytes(byte[] data)
			{
				return data != null && BitConverter.ToBoolean(data, 0);
			}

			public override byte[] ToBytes(bool value)
			{
				return BitConverter.GetBytes(value);
			}
		}

		private class FormatterGuid : DataColumn<T>.Formatter<Guid>
		{
			public override Guid FromBytes(byte[] data)
			{
				if (data != null)
				{
					return new Guid(data);
				}
				return Guid.Empty;
			}

			public override byte[] ToBytes(Guid value)
			{
				return value.ToByteArray();
			}
		}

		private class FormatterDateTime : DataColumn<T>.Formatter<DateTime>
		{
			public override DateTime FromBytes(byte[] data)
			{
				if (data != null)
				{
					return DateTime.FromOADate(BitConverter.ToDouble(data, 0));
				}
				return default(DateTime);
			}

			public override byte[] ToBytes(DateTime value)
			{
				return BitConverter.GetBytes(value.ToOADate());
			}
		}

		private class FormatterIPAddressBytes : DataColumn<T>.Formatter<IPvxAddress>
		{
			public override IPvxAddress FromBytes(byte[] data)
			{
				if (data != null)
				{
					return new IPvxAddress(data);
				}
				return default(IPvxAddress);
			}

			public override byte[] ToBytes(IPvxAddress value)
			{
				return value.GetBytes();
			}
		}
	}
}
