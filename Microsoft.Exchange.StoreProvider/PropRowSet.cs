using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropRowSet
	{
		public PropRowSet() : this(10)
		{
		}

		public PropRowSet(int count)
		{
			this.rows = new List<PropRow>(count);
		}

		public PropRowSet(SafeHandle rowset) : this(rowset, false)
		{
		}

		public PropRowSet(SafeHandle rowset, bool retainAnsiStrings)
		{
			PropValue[][] array = SRowSet.Unmarshal(rowset, retainAnsiStrings);
			this.rows = new List<PropRow>(array.Length);
			foreach (PropValue[] properties in array)
			{
				this.rows.Add(new PropRow(properties));
			}
		}

		public IList<PropRow> Rows
		{
			get
			{
				return this.rows;
			}
		}

		public void Add(PropRow row)
		{
			this.rows.Add(row);
		}

		public int GetBytesToMarshal()
		{
			return SRowSet.SizeOf + this.rows.Count * SRow.SizeOf;
		}

		public unsafe void MarshalToNative(SafeHandle rowset)
		{
			SRowSet* ptr = (SRowSet*)rowset.DangerousGetHandle().ToPointer();
			ptr->cRows = this.rows.Count;
			SRow* ptr2 = (SRow*)(ptr + SRowSet.DataOffset / sizeof(SRowSet));
			foreach (PropRow propRow in this.rows)
			{
				propRow.MarshalToNative(ptr2);
				ptr2++;
			}
		}

		private const int DefaultListSize = 10;

		private List<PropRow> rows;
	}
}
