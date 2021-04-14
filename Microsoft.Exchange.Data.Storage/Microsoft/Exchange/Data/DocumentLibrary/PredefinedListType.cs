using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	internal enum PredefinedListType
	{
		GenericList = 100,
		DocumentLibrary,
		Survey,
		Links,
		Announcements,
		Contacts,
		Events,
		Tasks,
		DiscussionBoard,
		PictureLibrary,
		DataSources,
		WebTemplateCatalog,
		WebPartCatalog = 113,
		ListTemplateCatalog,
		XMLForm,
		CustomGrid = 120,
		IssueTracking = 1100,
		Any = -1
	}
}
