using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	[Flags]
	public enum Regions
	{
		None = 0,
		NA01 = 1,
		EMEA01 = 2,
		NASIP01 = 4,
		EMEASIP01 = 8,
		CN01 = 16,
		NASIP02 = 32,
		APAC01 = 64,
		Reserved5 = 128,
		Reserved6 = 256,
		Reserved7 = 512,
		Reserved8 = 1024,
		Reserved9 = 2048,
		Reserved10 = 4096,
		Reserved11 = 8192,
		Reserved12 = 16384,
		Reserved13 = 32768,
		Reserved14 = 65536,
		Reserved15 = 131072,
		Reserved16 = 262144,
		Reserved17 = 524288,
		Reserved18 = 1048576,
		Reserved19 = 2097152,
		Reserved20 = 4194304,
		Reserved21 = 8388608
	}
}
