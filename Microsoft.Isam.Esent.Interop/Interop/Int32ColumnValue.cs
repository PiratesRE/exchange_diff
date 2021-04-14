using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class Int32ColumnValue : ColumnValueOfStruct<int>
	{
		protected override int Size
		{
			[DebuggerStepThrough]
			get
			{
				return 4;
			}
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			int valueOrDefault = base.Value.GetValueOrDefault();
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)(&valueOrDefault), 4, base.Value != null);
		}

		protected override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				base.Value = null;
				return;
			}
			base.CheckDataCount(count);
			base.Value = new int?(BitConverter.ToInt32(value, startIndex));
		}
	}
}
