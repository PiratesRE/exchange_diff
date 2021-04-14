using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MapiEventFlags
	{
		None = 0,
		FolderAssociated = 1,
		Ancestor = 4,
		Children = 8,
		ContentOnly = 16,
		SoftDeleted = 32,
		Subfolder = 64,
		ModifiedByMove = 128,
		Source = 256,
		Destination = 512,
		ObjectClassTruncated = 1024,
		SearchFolder = 2048
	}
}
