using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ViewFilterActions
	{
		None = 0,
		BindToExisting = 1,
		FindExisting = 2,
		FilterFound = 4,
		CreateFilter = 8,
		DeleteInvalidSearchFolder = 16,
		SearchCriteriaApplied = 32,
		SearchCompleted = 64,
		SearchFolderPopulateFailed = 128,
		SubscribeForNotification = 256,
		PopulateSearchFolderTimedOut = 512,
		ObjectNotFoundException = 1024,
		CorruptDataException = 2048,
		ObjectNotInitializedException = 4096,
		QueryInProgressException = 8192,
		Exception = 16384,
		LinkToSourceFolderSucceeded = 32768
	}
}
