using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ENUMCOLUMN
	{
		public uint cEnumColumnValue
		{
			get
			{
				return this.cbData;
			}
			set
			{
				this.cbData = value;
			}
		}

		public unsafe NATIVE_ENUMCOLUMNVALUE* rgEnumColumnValue
		{
			get
			{
				return (NATIVE_ENUMCOLUMNVALUE*)((void*)this.pvData);
			}
			set
			{
				this.pvData = new IntPtr((void*)value);
			}
		}

		public uint columnid;

		public int err;

		public uint cbData;

		public IntPtr pvData;
	}
}
