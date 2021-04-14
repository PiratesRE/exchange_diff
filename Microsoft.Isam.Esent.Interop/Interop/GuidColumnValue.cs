using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class GuidColumnValue : ColumnValueOfStruct<Guid>
	{
		protected override int Size
		{
			[DebuggerStepThrough]
			get
			{
				return 16;
			}
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			Guid valueOrDefault = base.Value.GetValueOrDefault();
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)(&valueOrDefault), this.Size, base.Value != null);
		}

		protected unsafe override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				base.Value = null;
				return;
			}
			base.CheckDataCount(count);
			Guid value2;
			void* ptr = (void*)(&value2);
			byte* ptr2 = (byte*)ptr;
			for (int i = 0; i < this.Size; i++)
			{
				ptr2[i] = value[startIndex + i];
			}
			base.Value = new Guid?(value2);
		}
	}
}
