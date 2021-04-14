using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataBlobCommonSchema
	{
		public static HygienePropertyDefinition DataTypeIdProperty = new HygienePropertyDefinition("DataTypeId", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition BlobIdProperty = new HygienePropertyDefinition("BlobId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition MajorVersionProperty = new HygienePropertyDefinition("MajorVersion", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition MinorVersionProperty = new HygienePropertyDefinition("MinorVersion", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition BuildNumberProperty = new HygienePropertyDefinition("BuildNumber", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition RevisionNumberProperty = new HygienePropertyDefinition("RevisionNumber", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition BlobSizeBytesProperty = new HygienePropertyDefinition("BlobSizeBytes", typeof(long), -1L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition IsCompleteBlobProperty = new HygienePropertyDefinition("IsCompleteBlob", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChunkIdProperty = new HygienePropertyDefinition("ChunkId", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsLastChunkProperty = new HygienePropertyDefinition("IsLastchunk", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChecksumProperty = new HygienePropertyDefinition("Checksum", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DataChunkProperty = new HygienePropertyDefinition("DataChunk", typeof(byte[]));

		public static readonly HygienePropertyDefinition BlobVersionTypeProperty = new HygienePropertyDefinition("BlobVersionType", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PrimaryOnlyProperty = new HygienePropertyDefinition("PrimaryOnly", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MajorVersionOnlyProperty = new HygienePropertyDefinition("MajorVersionOnly", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LatestVersionOnlyProperty = new HygienePropertyDefinition("LatestVersionOnly", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SinceVersionOnlyProperty = new HygienePropertyDefinition("SinceVersionOnly", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
