using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum SetColumnGrbit
	{
		None = 0,
		AppendLV = 1,
		OverwriteLV = 4,
		RevertToDefaultValue = 512,
		SeparateLV = 64,
		SizeLV = 8,
		UniqueMultiValues = 128,
		UniqueNormalizedMultiValues = 256,
		ZeroLength = 32,
		IntrinsicLV = 1024
	}
}
