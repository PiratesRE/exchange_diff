using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentSchema : DocumentLibraryItemSchema
	{
		public new static DocumentSchema Instance
		{
			get
			{
				if (DocumentSchema.instance == null)
				{
					DocumentSchema.instance = new DocumentSchema();
				}
				return DocumentSchema.instance;
			}
		}

		private static DocumentSchema instance = null;

		public static readonly DocumentLibraryPropertyDefinition FileSize = new DocumentLibraryPropertyDefinition("Size", typeof(int), null, DocumentLibraryPropertyId.FileSize);

		public static readonly DocumentLibraryPropertyDefinition FileType = new DocumentLibraryPropertyDefinition("ContentType", typeof(string), string.Empty, DocumentLibraryPropertyId.FileType);
	}
}
