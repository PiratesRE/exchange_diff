using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReferenceAttachmentSchema : AttachmentSchema
	{
		public new static ReferenceAttachmentSchema Instance
		{
			get
			{
				if (ReferenceAttachmentSchema.instance == null)
				{
					ReferenceAttachmentSchema.instance = new ReferenceAttachmentSchema();
				}
				return ReferenceAttachmentSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreAttachment coreAttachment)
		{
			base.CoreObjectUpdate(coreAttachment);
			ReferenceAttachment.CoreObjectUpdateReferenceAttachmentName(coreAttachment);
		}

		private static ReferenceAttachmentSchema instance;
	}
}
