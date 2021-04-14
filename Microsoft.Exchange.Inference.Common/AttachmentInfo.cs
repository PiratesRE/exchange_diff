using System;

namespace Microsoft.Exchange.Inference.Common
{
	internal class AttachmentInfo
	{
		public AttachmentInfo(AttachmentType attachmentType, string contentType, string fileExtension, bool isInline, long size)
		{
			this.AttachmentType = attachmentType;
			this.ContentType = contentType;
			this.FileExtension = fileExtension;
			this.IsInline = isInline;
			this.Size = size;
		}

		public AttachmentType AttachmentType { get; protected set; }

		public string ContentType { get; protected set; }

		public string FileExtension { get; protected set; }

		public bool IsInline { get; protected set; }

		public long Size { get; protected set; }
	}
}
