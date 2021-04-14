using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	internal enum ListBaseType
	{
		GenericList,
		DocumentLibrary,
		Unused,
		DiscussionBoard,
		Survey,
		Issue,
		Any = -1
	}
}
