using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncItemSchema : Schema
	{
		public static readonly DocumentLibraryPropertyDefinition Uri = DocumentLibraryItemSchema.Uri;

		public static readonly DocumentLibraryPropertyDefinition CreationDate = DocumentLibraryItemSchema.CreationTime;

		public static readonly DocumentLibraryPropertyDefinition DisplayName = DocumentLibraryItemSchema.DisplayName;

		public static readonly DocumentLibraryPropertyDefinition LastModifiedDate = DocumentLibraryItemSchema.LastModifiedDate;

		public static readonly DocumentLibraryPropertyDefinition IsFolder = DocumentLibraryItemSchema.IsFolder;

		public static readonly DocumentLibraryPropertyDefinition IsHidden = DocumentLibraryItemSchema.IsHidden;

		public static readonly DocumentLibraryPropertyDefinition Id = DocumentLibraryItemSchema.Id;
	}
}
