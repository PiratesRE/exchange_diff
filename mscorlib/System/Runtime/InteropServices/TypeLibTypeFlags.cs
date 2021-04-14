using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum TypeLibTypeFlags
	{
		FAppObject = 1,
		FCanCreate = 2,
		FLicensed = 4,
		FPreDeclId = 8,
		FHidden = 16,
		FControl = 32,
		FDual = 64,
		FNonExtensible = 128,
		FOleAutomation = 256,
		FRestricted = 512,
		FAggregatable = 1024,
		FReplaceable = 2048,
		FDispatchable = 4096,
		FReverseBind = 8192
	}
}
