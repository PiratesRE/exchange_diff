using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class StringColumnValue : ColumnValue
	{
		public override object ValueAsObject
		{
			[DebuggerStepThrough]
			get
			{
				return this.Value;
			}
		}

		public string Value { get; set; }

		public override int Length
		{
			get
			{
				if (this.Value == null)
				{
					return 0;
				}
				return this.Value.Length * 2;
			}
		}

		protected override int Size
		{
			[DebuggerStepThrough]
			get
			{
				return 0;
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			if (this.Value != null)
			{
				fixed (void* value = this.Value)
				{
					return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, value, checked(this.Value.Length * 2), true);
				}
			}
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, null, 0, false);
		}

		protected override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				this.Value = null;
				return;
			}
			this.Value = StringCache.GetString(value, startIndex, count);
		}
	}
}
