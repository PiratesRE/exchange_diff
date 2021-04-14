using System;

namespace System
{
	[Serializable]
	internal enum ConfigEvents
	{
		StartDocument,
		StartDTD,
		EndDTD,
		StartDTDSubset,
		EndDTDSubset,
		EndProlog,
		StartEntity,
		EndEntity,
		EndDocument,
		DataAvailable,
		LastEvent = 9
	}
}
