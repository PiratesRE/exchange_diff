using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	[CLSCompliant(false)]
	public class UInt16ColumnValue : ColumnValueOfStruct<ushort>
	{
		protected override int Size
		{
			[DebuggerStepThrough]
			get
			{
				return 2;
			}
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			ushort valueOrDefault = base.Value.GetValueOrDefault();
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)(&valueOrDefault), 2, base.Value != null);
		}

		protected override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				base.Value = null;
				return;
			}
			base.CheckDataCount(count);
			base.Value = new ushort?(BitConverter.ToUInt16(value, startIndex));
		}
	}
}
