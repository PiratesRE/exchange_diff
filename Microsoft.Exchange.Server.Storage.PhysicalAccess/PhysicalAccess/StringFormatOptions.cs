using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	[Flags]
	public enum StringFormatOptions
	{
		None = 0,
		IncludeDetails = 1,
		IncludeNestedObjectsId = 2,
		SkipParametersData = 4,
		MultiLine = 8
	}
}
