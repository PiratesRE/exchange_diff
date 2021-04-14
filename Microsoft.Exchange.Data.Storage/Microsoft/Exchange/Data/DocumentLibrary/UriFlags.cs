using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[Flags]
	internal enum UriFlags
	{
		Sharepoint = 1,
		Unc = 2,
		DocumentLibrary = 4,
		List = 8,
		Document = 16,
		Folder = 32,
		Other = 64,
		SharepointList = 9,
		SharepointDocumentLibrary = 5,
		SharepointDocument = 17,
		SharepointFolder = 33,
		UncDocumentLibrary = 6,
		UncDocument = 18,
		UncFolder = 34
	}
}
