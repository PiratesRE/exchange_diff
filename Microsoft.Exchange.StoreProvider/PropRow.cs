using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropRow
	{
		public PropRow() : this(10)
		{
		}

		public PropRow(int count)
		{
			this.properties = new List<PropValue>(count);
		}

		public PropRow(IList<PropValue> properties)
		{
			this.properties = properties;
		}

		public PropRow(SafeHandle row) : this(row, false)
		{
		}

		public PropRow(SafeHandle row, bool retainAnsiStrings)
		{
			this.properties = SRow.Unmarshal(row.DangerousGetHandle(), retainAnsiStrings);
		}

		public PropRow(IntPtr row)
		{
			this.properties = SRow.Unmarshal(row);
		}

		public IList<PropValue> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public SafeHandle MarshalledPropertiesHandle
		{
			get
			{
				return this.marshalledPropertiesHandle;
			}
			set
			{
				this.marshalledPropertiesHandle = value;
			}
		}

		public void Add(PropValue pv)
		{
			this.properties.Add(pv);
		}

		public int GetBytesToMarshal()
		{
			return SRow.SizeOf;
		}

		public unsafe void MarshalToNative(SafeHandle row)
		{
			SRow* row2 = (SRow*)row.DangerousGetHandle().ToPointer();
			this.MarshalToNative(row2);
		}

		internal unsafe void MarshalToNative(SRow* row)
		{
			row->cValues = this.properties.Count;
			row->lpProps = this.marshalledPropertiesHandle.DangerousGetHandle();
		}

		private const int DefaultListSize = 10;

		private readonly IList<PropValue> properties;

		private SafeHandle marshalledPropertiesHandle;
	}
}
