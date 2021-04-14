using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataBlobVersionSchema
	{
		public static HygienePropertyDefinition DataTypeIdProperty = DataBlobCommonSchema.DataTypeIdProperty;

		public static HygienePropertyDefinition BlobIdProperty = DataBlobCommonSchema.BlobIdProperty;

		public static HygienePropertyDefinition MajorVersionProperty = DataBlobCommonSchema.MajorVersionProperty;

		public static HygienePropertyDefinition MinorVersionProperty = DataBlobCommonSchema.MinorVersionProperty;

		public static HygienePropertyDefinition BuildNumberProperty = DataBlobCommonSchema.BuildNumberProperty;

		public static HygienePropertyDefinition RevisionNumberProperty = DataBlobCommonSchema.RevisionNumberProperty;

		public static HygienePropertyDefinition BlobSizeBytesProperty = DataBlobCommonSchema.BlobSizeBytesProperty;

		public static HygienePropertyDefinition IsCompleteBlobProperty = DataBlobCommonSchema.IsCompleteBlobProperty;
	}
}
