using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncDocumentSchema : UncItemSchema
	{
		public new static UncDocumentSchema Instance
		{
			get
			{
				if (UncDocumentSchema.instance == null)
				{
					UncDocumentSchema.instance = new UncDocumentSchema();
				}
				return UncDocumentSchema.instance;
			}
		}

		private static UncDocumentSchema instance = null;

		public static readonly DocumentLibraryPropertyDefinition FileType = DocumentSchema.FileType;

		public static readonly DocumentLibraryPropertyDefinition FileSize = DocumentSchema.FileSize;
	}
}
