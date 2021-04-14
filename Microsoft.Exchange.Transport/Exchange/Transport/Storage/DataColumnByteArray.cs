using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataColumnByteArray : DataColumn
	{
		internal DataColumnByteArray(JET_coltyp type, bool fixedSize) : base(type, fixedSize)
		{
		}

		internal DataColumnByteArray(JET_coltyp type, bool fixedSize, int size) : base(type, fixedSize, size)
		{
		}

		internal override ColumnCache NewCacheCell()
		{
			return new ColumnCacheByteArray();
		}

		internal override void ColumnValueToCache(ColumnValue data, ColumnCache cache)
		{
			BytesColumnValue bytesColumnValue = (BytesColumnValue)data;
			((ColumnCache<byte[]>)cache).Value = bytesColumnValue.Value;
		}

		internal override byte[] BytesFromCache(ColumnCache cache)
		{
			return ((ColumnCache<byte[]>)cache).Value;
		}
	}
}
