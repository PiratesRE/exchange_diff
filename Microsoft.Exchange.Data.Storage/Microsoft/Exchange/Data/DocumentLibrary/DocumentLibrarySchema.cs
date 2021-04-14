using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentLibrarySchema
	{
		public static DocumentLibraryPropertyDefinition Id = DocumentLibraryItemSchema.Id;

		public static DocumentLibraryPropertyDefinition Uri = DocumentLibraryItemSchema.Uri;

		public static DocumentLibraryPropertyDefinition Title = new DocumentLibraryPropertyDefinition("Title", typeof(string), null, DocumentLibraryPropertyId.Title);

		public static DocumentLibraryPropertyDefinition Description = new DocumentLibraryPropertyDefinition("Description", typeof(string), null, DocumentLibraryPropertyId.Description);
	}
}
