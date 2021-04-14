using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FileAttachment : Attachment
	{
		public string ContentId
		{
			get
			{
				return (string)base[FileAttachmentSchema.ContentId];
			}
			set
			{
				base[FileAttachmentSchema.ContentId] = value;
			}
		}

		public string ContentLocation
		{
			get
			{
				return (string)base[FileAttachmentSchema.ContentLocation];
			}
			set
			{
				base[FileAttachmentSchema.ContentLocation] = value;
			}
		}

		public bool IsContactPhoto
		{
			get
			{
				return (bool)base[FileAttachmentSchema.IsContactPhoto];
			}
			set
			{
				base[FileAttachmentSchema.IsContactPhoto] = value;
			}
		}

		public byte[] ContentBytes
		{
			get
			{
				return (byte[])base[FileAttachmentSchema.ContentBytes];
			}
			set
			{
				base[FileAttachmentSchema.ContentBytes] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return FileAttachmentSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(FileAttachment).Namespace, typeof(FileAttachment).Name, Attachment.EdmEntityType);
	}
}
