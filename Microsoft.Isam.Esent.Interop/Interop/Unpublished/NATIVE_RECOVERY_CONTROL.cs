using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_RECOVERY_CONTROL
	{
		public uint cbStruct;

		public JET_err errDefault;

		public IntPtr instance;

		public JET_SNT sntUnion;
	}
}
