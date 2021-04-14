using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum JET_cbtyp
	{
		Null = 0,
		Finalize = 1,
		BeforeInsert = 2,
		AfterInsert = 4,
		BeforeReplace = 8,
		AfterReplace = 16,
		BeforeDelete = 32,
		AfterDelete = 64,
		UserDefinedDefaultValue = 128,
		OnlineDefragCompleted = 256,
		FreeCursorLS = 512,
		FreeTableLS = 1024
	}
}
