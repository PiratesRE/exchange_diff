using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataBlob : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}:{2}", this.DataTypeId, this.BlobId, this.ChunkId));
			}
		}

		public int DataTypeId
		{
			get
			{
				return (int)this[DataBlobSchema.DataTypeIdProperty];
			}
			set
			{
				this[DataBlobSchema.DataTypeIdProperty] = value;
			}
		}

		public Guid BlobId
		{
			get
			{
				return (Guid)this[DataBlobSchema.BlobIdProperty];
			}
			set
			{
				this[DataBlobSchema.BlobIdProperty] = value;
			}
		}

		public Version Version
		{
			get
			{
				if (this.version == null)
				{
					this.SetBuildVersion();
				}
				return this.version;
			}
		}

		public int ChunkId
		{
			get
			{
				return (int)this[DataBlobSchema.ChunkIdProperty];
			}
			set
			{
				this[DataBlobSchema.ChunkIdProperty] = value;
			}
		}

		public bool IsLastChunk
		{
			get
			{
				return (bool)this[DataBlobSchema.IsLastChunkProperty];
			}
			set
			{
				this[DataBlobSchema.IsLastChunkProperty] = value;
			}
		}

		public long Checksum
		{
			get
			{
				return (long)this[DataBlobSchema.ChecksumProperty];
			}
			set
			{
				this[DataBlobSchema.ChecksumProperty] = value;
			}
		}

		public byte[] DataChunk
		{
			get
			{
				return (byte[])this[DataBlobSchema.DataChunkProperty];
			}
			set
			{
				this[DataBlobSchema.DataChunkProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(DataBlobSchema);
		}

		private void SetBuildVersion()
		{
			int major = (int)this[DataBlobSchema.MajorVersionProperty];
			int minor = (int)this[DataBlobSchema.MinorVersionProperty];
			int build = (int)this[DataBlobSchema.BuildNumberProperty];
			int revision = (int)this[DataBlobSchema.RevisionNumberProperty];
			this.version = BuildVersion.GetBuildVersionObject(major, minor, build, revision);
		}

		private Version version;
	}
}
