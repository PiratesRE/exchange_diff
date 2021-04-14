using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDocumentLibrary : IReadOnlyPropertyBag
	{
		ObjectId Id { get; }

		string Title { get; }

		string Description { get; }

		Uri Uri { get; }

		List<KeyValuePair<string, Uri>> GetHierarchy();

		IDocumentLibraryItem Read(ObjectId objectId, params PropertyDefinition[] propsToReturn);

		ITableView GetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, params PropertyDefinition[] propsToReturn);
	}
}
