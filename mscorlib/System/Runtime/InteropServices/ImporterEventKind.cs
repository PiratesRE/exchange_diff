using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum ImporterEventKind
	{
		NOTIF_TYPECONVERTED,
		NOTIF_CONVERTWARNING,
		ERROR_REFTOINVALIDTYPELIB
	}
}
