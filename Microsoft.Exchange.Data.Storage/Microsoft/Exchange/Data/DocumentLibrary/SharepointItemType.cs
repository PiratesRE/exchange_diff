using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[Flags]
	internal enum SharepointItemType
	{
		Web = 1,
		List = 2,
		Item = 4,
		DocumentLibrary = 10,
		Document = 20,
		Folder = 36
	}
}
