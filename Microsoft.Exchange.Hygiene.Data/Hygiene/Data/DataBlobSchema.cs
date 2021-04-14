using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataBlobSchema
	{
		public static HygienePropertyDefinition DataTypeIdProperty = DataBlobCommonSchema.DataTypeIdProperty;

		public static HygienePropertyDefinition BlobIdProperty = DataBlobCommonSchema.BlobIdProperty;

		public static HygienePropertyDefinition MajorVersionProperty = DataBlobCommonSchema.MajorVersionProperty;

		public static HygienePropertyDefinition MinorVersionProperty = DataBlobCommonSchema.MinorVersionProperty;

		public static HygienePropertyDefinition BuildNumberProperty = DataBlobCommonSchema.BuildNumberProperty;

		public static HygienePropertyDefinition RevisionNumberProperty = DataBlobCommonSchema.RevisionNumberProperty;

		public static readonly HygienePropertyDefinition ChunkIdProperty = DataBlobCommonSchema.ChunkIdProperty;

		public static readonly HygienePropertyDefinition IsLastChunkProperty = DataBlobCommonSchema.IsLastChunkProperty;

		public static readonly HygienePropertyDefinition ChecksumProperty = DataBlobCommonSchema.ChecksumProperty;

		public static readonly HygienePropertyDefinition DataChunkProperty = DataBlobCommonSchema.DataChunkProperty;
	}
}
