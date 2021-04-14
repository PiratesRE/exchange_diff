using System;

namespace Microsoft.Exchange.TextProcessing
{
	public enum BoundaryType : byte
	{
		None,
		Normal,
		NormalLeftOnly,
		NormalRightOnly,
		Url,
		FullUrl
	}
}
