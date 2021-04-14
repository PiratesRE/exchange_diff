using System;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataColumnString : DataColumn
	{
		internal DataColumnString(JET_coltyp type, bool fixedSize) : base(type, fixedSize)
		{
		}

		internal DataColumnString(JET_coltyp type, bool fixedSize, int size) : base(type, fixedSize, size)
		{
		}

		internal override ColumnCache NewCacheCell()
		{
			return new ColumnCacheString();
		}

		internal override void ColumnValueToCache(ColumnValue data, ColumnCache cache)
		{
			StringColumnValue stringColumnValue = (StringColumnValue)data;
			((ColumnCache<string>)cache).Value = stringColumnValue.ToString();
		}

		internal override byte[] BytesFromCache(ColumnCache cache)
		{
			string value = ((ColumnCache<string>)cache).Value;
			if (string.IsNullOrEmpty(value))
			{
				return DataColumnString.EmptyArray;
			}
			return Encoding.Unicode.GetBytes(value);
		}

		private static readonly byte[] EmptyArray = new byte[0];
	}
}
