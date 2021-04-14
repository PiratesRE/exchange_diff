using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointListSchema : SharepointObjectSchema
	{
		public new static SharepointListSchema Instance
		{
			get
			{
				if (SharepointListSchema.instance == null)
				{
					SharepointListSchema.instance = new SharepointListSchema();
				}
				return SharepointListSchema.instance;
			}
		}

		private static SharepointListSchema instance = null;

		public static readonly DocumentLibraryPropertyDefinition ID = new SharepointPropertyDefinition("ID", typeof(DocumentLibraryObjectId), DocumentLibraryPropertyId.Id, "ID", SharepointFieldType.Guid, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.NoOp), null);

		public static DocumentLibraryPropertyDefinition Name = new SharepointPropertyDefinition("Name", typeof(string), DocumentLibraryPropertyId.None, "Name", SharepointFieldType.Guid, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.NoOp), null);

		public static DocumentLibraryPropertyDefinition Title = new SharepointPropertyDefinition("Title", typeof(string), DocumentLibraryPropertyId.Title, "Title", SharepointFieldType.Text, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.NoOp), string.Empty);

		public static DocumentLibraryPropertyDefinition Description = new SharepointPropertyDefinition("Description", typeof(string), DocumentLibraryPropertyId.Description, "Description", SharepointFieldType.Text, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.NoOp), string.Empty);

		public static DocumentLibraryPropertyDefinition DefaultViewUri = new SharepointPropertyDefinition("DefaultViewUri", typeof(Uri), DocumentLibraryPropertyId.None, "DefaultViewUrl", SharepointFieldType.URL, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToRelateiveUri), null);

		public static DocumentLibraryPropertyDefinition ListType = new SharepointPropertyDefinition("ListType", typeof(int), DocumentLibraryPropertyId.None, "BaseType", SharepointFieldType.Integer, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToInt), null);

		public static DocumentLibraryPropertyDefinition DefaultTemplateUri = new SharepointPropertyDefinition("DefaultTemplateUri", typeof(Uri), DocumentLibraryPropertyId.None, "DefaultTemplateUrl", SharepointFieldType.URL, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToRelateiveUri), null);

		public static DocumentLibraryPropertyDefinition DocTemplateUri = new SharepointPropertyDefinition("DocTemplateUri", typeof(Uri), DocumentLibraryPropertyId.None, "DocTemplateUrl", SharepointFieldType.URL, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToRelateiveUri), null);

		public static DocumentLibraryPropertyDefinition ImageUri = new SharepointPropertyDefinition("ImageUri", typeof(Uri), DocumentLibraryPropertyId.None, "ImageUrl", SharepointFieldType.URL, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToRelateiveUri), null);

		public static DocumentLibraryPropertyDefinition PredefinedListType = new SharepointPropertyDefinition("Predefined ListType", typeof(int), DocumentLibraryPropertyId.None, "ServerTemplate", SharepointFieldType.Integer, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToInt), null);

		public static DocumentLibraryPropertyDefinition LastModifiedTime = new SharepointPropertyDefinition("Last modified time", typeof(ExDateTime), DocumentLibraryPropertyId.LastModifiedTime, "Modified", SharepointFieldType.DateTime, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepoinToListDateTime), null);

		public static DocumentLibraryPropertyDefinition CreationTime = new SharepointPropertyDefinition("Created Date", typeof(ExDateTime), DocumentLibraryPropertyId.CreationTime, "Created", SharepointFieldType.DateTime, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepoinToListDateTime), null);

		public static DocumentLibraryPropertyDefinition ItemCount = new SharepointPropertyDefinition("Item Count", typeof(int), DocumentLibraryPropertyId.None, "ItemCount", SharepointFieldType.DateTime, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToInt), null);

		internal static DocumentLibraryPropertyDefinition IsHidden = new SharepointPropertyDefinition("IsHidden", typeof(bool), DocumentLibraryPropertyId.IsHidden, "Hidden", SharepointFieldType.DateTime, new SharepointPropertyDefinition.MarshalTypeToSharepoint(SharepointHelpers.ObjectToString), new SharepointPropertyDefinition.MarshalTypeFromSharepoint(SharepointHelpers.SharepointToBool), null);

		public static DocumentLibraryPropertyDefinition Uri = DocumentLibraryItemSchema.Uri;
	}
}
