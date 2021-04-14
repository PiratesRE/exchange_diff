using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentLibraryItemSchema : Schema
	{
		public static readonly DocumentLibraryPropertyDefinition Uri = new DocumentLibraryPropertyDefinition("Uri", typeof(Uri), null, DocumentLibraryPropertyId.Uri);

		public static readonly DocumentLibraryPropertyDefinition CreationTime = new DocumentLibraryPropertyDefinition("CreationTime", typeof(ExDateTime), null, DocumentLibraryPropertyId.CreationTime);

		public static readonly DocumentLibraryPropertyDefinition DisplayName = new DocumentLibraryPropertyDefinition("Display Name", typeof(string), string.Empty, DocumentLibraryPropertyId.Title);

		public static readonly DocumentLibraryPropertyDefinition LastModifiedDate = new DocumentLibraryPropertyDefinition("LastModifiedDate", typeof(ExDateTime), null, DocumentLibraryPropertyId.LastModifiedTime);

		public static readonly DocumentLibraryPropertyDefinition IsFolder = new DocumentLibraryPropertyDefinition("IsFolder", typeof(bool), null, DocumentLibraryPropertyId.IsFolder);

		public static readonly DocumentLibraryPropertyDefinition IsHidden = new DocumentLibraryPropertyDefinition("IsHidden", typeof(bool), null, DocumentLibraryPropertyId.IsHidden);

		public static readonly DocumentLibraryPropertyDefinition Id = new DocumentLibraryPropertyDefinition("Id", typeof(DocumentLibraryObjectId), null, DocumentLibraryPropertyId.Id);
	}
}
