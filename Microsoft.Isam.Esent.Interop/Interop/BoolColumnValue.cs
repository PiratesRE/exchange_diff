using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	public class BoolColumnValue : ColumnValueOfStruct<bool>
	{
		public override object ValueAsObject
		{
			get
			{
				if (base.Value == null)
				{
					return null;
				}
				if (!base.Value.Value)
				{
					return BoolColumnValue.BoxedFalse;
				}
				return BoolColumnValue.BoxedTrue;
			}
		}

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
			byte b = base.Value.GetValueOrDefault() ? byte.MaxValue : 0;
			return base.SetColumns(sesid, tableid, columnValues, nativeColumns, i, (void*)(&b), 1, base.Value != null);
		}

		protected override void GetValueFromBytes(byte[] value, int startIndex, int count, int err)
		{
			if (1004 == err)
			{
				base.Value = null;
				return;
			}
			base.CheckDataCount(count);
			base.Value = new bool?(BitConverter.ToBoolean(value, startIndex));
		}

		private static readonly object BoxedTrue = true;

		private static readonly object BoxedFalse = false;
	}
}
