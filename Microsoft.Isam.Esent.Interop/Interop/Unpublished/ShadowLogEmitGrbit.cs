using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum ShadowLogEmitGrbit
	{
		None = 0,
		FirstCall = 1,
		LastCall = 2,
		Cancel = 4,
		DataBuffers = 8,
		LogComplete = 16
	}
}
