using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class ByteColumnValue : ColumnValueOfStruct<byte>
	{
		protected override int Size
		{
			[DebuggerStepThrough]
			get
			{
				return 1;
			}
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			byte valueOrDefault = base.Value.GetValueOrDefault();
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)(&valueOrDefault), 1, base.Value != null);
		}

		protected override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				base.Value = null;
				return;
			}
			base.CheckDataCount(count);
			base.Value = new byte?(value[startIndex]);
		}
	}
}
