using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class InlineAttachmentInfo
	{
		public InlineAttachmentInfo()
		{
		}

		public InlineAttachmentInfo(AttachmentId id, string contentId, int renderingPosition, bool isInline)
		{
			this.Id = id;
			this.ContentId = contentId;
			this.RenderingPosition = new int?(renderingPosition);
			this.IsInline = new bool?(isInline);
		}

		public AttachmentId Id;

		public string ContentId;

		public int? RenderingPosition;

		public bool? IsInline;
	}
}
