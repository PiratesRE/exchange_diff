using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IFilterGenerator
	{
		QueryFilter Execute(string searchString, bool isContentIndexingEnabled, Folder folder, SearchScope searchScope);
	}
}
