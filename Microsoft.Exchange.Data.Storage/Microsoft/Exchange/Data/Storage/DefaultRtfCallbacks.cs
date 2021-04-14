using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DefaultRtfCallbacks : RtfCallbackBase
	{
		internal DefaultRtfCallbacks(ICoreItem coreItem, bool itemReadOnly) : base(coreItem)
		{
			this.readOnly = itemReadOnly;
		}

		internal DefaultRtfCallbacks(CoreAttachmentCollection collection, Body itemBody, bool itemReadOnly) : base(collection, itemBody)
		{
			this.readOnly = itemReadOnly;
		}

		public override bool ProcessImage(string imageUrl, int approximateRenderingPosition)
		{
			AttachmentLink attachmentLink = base.FindAttachmentByBodyReference(imageUrl);
			if (attachmentLink == null)
			{
				return false;
			}
			attachmentLink.RenderingPosition = approximateRenderingPosition;
			attachmentLink.IsHidden = false;
			attachmentLink.MarkInline(true);
			return true;
		}

		public override bool SaveChanges()
		{
			return !this.readOnly && base.SaveChanges();
		}

		private bool readOnly;
	}
}
