using System;
using System.Globalization;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public class JET_INDEX_RANGE
	{
		public JET_INDEX_COLUMN[] startColumns { get; set; }

		public JET_INDEX_COLUMN[] endColumns { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEX_RANGE", new object[0]);
		}

		internal NATIVE_INDEX_RANGE GetNativeIndexRange(ref GCHandleCollection handles)
		{
			NATIVE_INDEX_RANGE result = default(NATIVE_INDEX_RANGE);
			if (this.startColumns != null)
			{
				NATIVE_INDEX_COLUMN[] array = new NATIVE_INDEX_COLUMN[this.startColumns.Length];
				for (int i = 0; i < this.startColumns.Length; i++)
				{
					array[i] = this.startColumns[i].GetNativeIndexColumn(ref handles);
				}
				result.rgStartColumns = handles.Add(array);
				result.cStartColumns = (uint)this.startColumns.Length;
			}
			if (this.endColumns != null)
			{
				NATIVE_INDEX_COLUMN[] array = new NATIVE_INDEX_COLUMN[this.endColumns.Length];
				for (int j = 0; j < this.endColumns.Length; j++)
				{
					array[j] = this.endColumns[j].GetNativeIndexColumn(ref handles);
				}
				result.rgEndColumns = handles.Add(array);
				result.cEndColumns = (uint)this.endColumns.Length;
			}
			return result;
		}
	}
}
