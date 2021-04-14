using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDocumentLibraryItem : IReadOnlyPropertyBag
	{
		string DisplayName { get; }

		Uri Uri { get; }

		ObjectId Id { get; }

		bool IsFolder { get; }

		IDocumentLibraryFolder Parent { get; }

		IDocumentLibrary Library { get; }

		object TryGetProperty(PropertyDefinition propertyDefinition);

		List<KeyValuePair<string, Uri>> GetHierarchy();
	}
}
