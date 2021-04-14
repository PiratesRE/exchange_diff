using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointDocumentSchema : SharepointDocumentLibraryItemSchema
	{
		public new static SharepointDocumentSchema Instance
		{
			get
			{
				if (SharepointDocumentSchema.instance == null)
				{
					SharepointDocumentSchema.instance = new SharepointDocumentSchema();
				}
				return SharepointDocumentSchema.instance;
			}
		}

		private static SharepointDocumentSchema instance = null;

		public static readonly DocumentLibraryPropertyDefinition VersionControl = new DocumentLibraryPropertyDefinition("Version control", typeof(VersionControl), null, DocumentLibraryPropertyId.None);

		public static readonly DocumentLibraryPropertyDefinition CheckedOutUserId = new SharepointPropertyDefinition("Checked out user", typeof(string), DocumentLibraryPropertyId.None, "CheckedOutTitle", SharepointFieldType.Lookup, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SecondJoinedValue), null);

		internal static readonly DocumentLibraryPropertyDefinition VersionId = new SharepointPropertyDefinition("Document version number", typeof(int), DocumentLibraryPropertyId.None, "owshiddenversion", SharepointFieldType.Integer, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToInt), null);

		public static readonly DocumentLibraryPropertyDefinition FileSize = new SharepointPropertyDefinition("File Size", typeof(long), DocumentLibraryPropertyId.FileSize, "File_x0020_Size", SharepointFieldType.Lookup, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointJoinedToLong), null);

		public static readonly DocumentLibraryPropertyDefinition FileType = new SharepointPropertyDefinition("FileType", typeof(string), DocumentLibraryPropertyId.FileType, "File_x0020_Type", SharepointFieldType.Text, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.ExtensionToContentType), "application/octet-stream");
	}
}
