using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum ExporterEventKind
	{
		NOTIF_TYPECONVERTED,
		NOTIF_CONVERTWARNING,
		ERROR_REFTOINVALIDASSEMBLY
	}
}
