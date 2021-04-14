using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum TypeLibExporterFlags
	{
		None = 0,
		OnlyReferenceRegistered = 1,
		CallerResolvedReferences = 2,
		OldNames = 4,
		ExportAs32Bit = 16,
		ExportAs64Bit = 32
	}
}
