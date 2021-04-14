using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class BytesColumnValue : ColumnValue
	{
		public override object ValueAsObject
		{
			[DebuggerStepThrough]
			get
			{
				return this.Value;
			}
		}

		public byte[] Value { get; set; }

		public override int Length
		{
			get
			{
				if (this.Value == null)
				{
					return 0;
				}
				return this.Value.Length;
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
			if (this.Value == null)
			{
				return string.Empty;
			}
			return BitConverter.ToString(this.Value, 0, Math.Min(this.Value.Length, 16));
		}

		internal unsafe override int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
		{
			if (this.Value != null)
			{
				fixed (IntPtr* value = this.Value)
				{
					return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)value, this.Value.Length, true);
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
			this.Value = new byte[count];
			Array.Copy(value, startIndex, this.Value, 0, count);
		}
	}
}
