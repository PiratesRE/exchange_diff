using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StreamAttachmentBaseSchema : AttachmentSchema
	{
		public new static StreamAttachmentBaseSchema Instance
		{
			get
			{
				if (StreamAttachmentBaseSchema.instance == null)
				{
					StreamAttachmentBaseSchema.instance = new StreamAttachmentBaseSchema();
				}
				return StreamAttachmentBaseSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreAttachment coreAttachment)
		{
			base.CoreObjectUpdate(coreAttachment);
			StreamAttachmentBase.CoreObjectUpdateStreamAttachmentName(coreAttachment);
			try
			{
				StreamAttachmentBase.CoreObjectUpdateImageThumbnail(coreAttachment);
			}
			catch (FileNotFoundException)
			{
			}
		}

		private static StreamAttachmentBaseSchema instance;
	}
}
