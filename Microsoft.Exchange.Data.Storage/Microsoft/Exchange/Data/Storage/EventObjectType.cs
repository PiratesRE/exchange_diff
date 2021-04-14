using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EventObjectType
	{
		None = 0,
		Item = 1,
		Folder = 2
	}
}
