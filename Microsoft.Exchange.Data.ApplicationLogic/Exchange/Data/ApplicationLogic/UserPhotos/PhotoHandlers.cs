using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[Flags]
	internal enum PhotoHandlers
	{
		None = 0,
		FileSystem = 1,
		Mailbox = 2,
		ActiveDirectory = 4,
		Caching = 8,
		Http = 16,
		Private = 32,
		RemoteForest = 64
	}
}
