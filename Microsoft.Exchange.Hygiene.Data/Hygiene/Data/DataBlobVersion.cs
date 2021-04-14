using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataBlobVersion : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}:{2}", this.DataTypeId, this.Version.ToString(), this.BlobId));
			}
		}

		public int DataTypeId
		{
			get
			{
				return (int)this[DataBlobVersionSchema.DataTypeIdProperty];
			}
			set
			{
				this[DataBlobVersionSchema.DataTypeIdProperty] = value;
			}
		}

		public Guid BlobId
		{
			get
			{
				return (Guid)this[DataBlobVersionSchema.BlobIdProperty];
			}
			set
			{
				this[DataBlobVersionSchema.BlobIdProperty] = value;
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
			set
			{
				this.version = value;
				if (value != null)
				{
					this[DataBlobVersionSchema.MajorVersionProperty] = value.Major;
					this[DataBlobVersionSchema.MinorVersionProperty] = value.Minor;
					this[DataBlobVersionSchema.BuildNumberProperty] = value.Build;
					this[DataBlobVersionSchema.RevisionNumberProperty] = value.Revision;
					return;
				}
				this[DataBlobVersionSchema.MajorVersionProperty] = -1;
				this[DataBlobVersionSchema.MinorVersionProperty] = -1;
				this[DataBlobVersionSchema.BuildNumberProperty] = -1;
				this[DataBlobVersionSchema.RevisionNumberProperty] = -1;
			}
		}

		public long BlobSizeBytes
		{
			get
			{
				return (long)this[DataBlobVersionSchema.BlobSizeBytesProperty];
			}
			set
			{
				this[DataBlobVersionSchema.BlobSizeBytesProperty] = value;
			}
		}

		public bool IsCompleteBlob
		{
			get
			{
				return (bool)this[DataBlobVersionSchema.IsCompleteBlobProperty];
			}
			set
			{
				this[DataBlobVersionSchema.IsCompleteBlobProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(DataBlobVersionSchema);
		}

		private void SetBuildVersion()
		{
			int major = (int)this[DataBlobVersionSchema.MajorVersionProperty];
			int minor = (int)this[DataBlobVersionSchema.MinorVersionProperty];
			int build = (int)this[DataBlobVersionSchema.BuildNumberProperty];
			int revision = (int)this[DataBlobVersionSchema.RevisionNumberProperty];
			this.version = BuildVersion.GetBuildVersionObject(major, minor, build, revision);
		}

		private Version version;
	}
}
