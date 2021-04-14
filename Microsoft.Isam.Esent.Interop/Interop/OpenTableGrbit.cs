using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum OpenTableGrbit
	{
		None = 0,
		DenyWrite = 1,
		DenyRead = 2,
		ReadOnly = 4,
		Updatable = 8,
		PermitDDL = 16,
		NoCache = 32,
		Preread = 64,
		Sequential = 32768,
		TableClass1 = 65536,
		TableClass2 = 131072,
		TableClass3 = 196608,
		TableClass4 = 262144,
		TableClass5 = 327680,
		TableClass6 = 393216,
		TableClass7 = 458752,
		TableClass8 = 524288,
		TableClass9 = 589824,
		TableClass10 = 655360,
		TableClass11 = 720896,
		TableClass12 = 786432,
		TableClass13 = 851968,
		TableClass14 = 917504,
		TableClass15 = 983040
	}
}
