using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AttachmentLevelLookup
	{
		public static AttachmentPolicy.Level GetLevelForAttachment(string fileExtension, string mimeType, UserContext userContext)
		{
			if (fileExtension == null)
			{
				throw new ArgumentNullException("fileExtension");
			}
			AttachmentPolicy attachmentPolicy;
			if (userContext != null)
			{
				attachmentPolicy = userContext.AttachmentPolicy;
			}
			else
			{
				attachmentPolicy = OwaConfigurationManager.Configuration.AttachmentPolicy;
			}
			if (mimeType == null || !attachmentPolicy.DirectFileAccessEnabled)
			{
				return AttachmentPolicy.Level.Block;
			}
			AttachmentPolicy.Level level = attachmentPolicy.GetLevel(fileExtension, AttachmentPolicy.TypeSignifier.File);
			if (level == AttachmentPolicy.Level.Allow)
			{
				return level;
			}
			AttachmentPolicy.Level level2 = attachmentPolicy.GetLevel(mimeType, AttachmentPolicy.TypeSignifier.Mime);
			if (level2 == AttachmentPolicy.Level.Allow)
			{
				return level2;
			}
			if (level == AttachmentPolicy.Level.Unknown && level2 == AttachmentPolicy.Level.Unknown)
			{
				return attachmentPolicy.TreatUnknownTypeAs;
			}
			if (level < level2)
			{
				return level;
			}
			return level2;
		}

		private static AttachmentPolicy.Level GetAttachmentLevel(AttachmentType attachmentType, string fileExtension, string mimeType, UserContext userContext)
		{
			if (attachmentType == AttachmentType.Ole || attachmentType == AttachmentType.EmbeddedMessage)
			{
				return AttachmentPolicy.Level.Allow;
			}
			return AttachmentLevelLookup.GetLevelForAttachment(fileExtension, mimeType, userContext);
		}

		public static AttachmentPolicy.Level GetAttachmentLevel(Attachment attachment, UserContext userContext)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment");
			}
			return AttachmentLevelLookup.GetAttachmentLevel(attachment.AttachmentType, attachment.FileExtension, attachment.ContentType ?? attachment.CalculatedContentType, userContext);
		}

		public static AttachmentPolicy.Level GetAttachmentLevel(AttachmentLink attachmentLink, UserContext userContext)
		{
			if (attachmentLink == null)
			{
				throw new ArgumentNullException("attachmentLink");
			}
			return AttachmentLevelLookup.GetAttachmentLevel(attachmentLink.AttachmentType, attachmentLink.FileExtension, attachmentLink.ContentType, userContext);
		}

		public static AttachmentPolicy.Level GetAttachmentLevel(AttachmentInfo attachmentInfo, UserContext userContext)
		{
			if (attachmentInfo == null)
			{
				throw new ArgumentNullException("attachmentInfo");
			}
			return AttachmentLevelLookup.GetAttachmentLevel(attachmentInfo.AttachmentType, attachmentInfo.FileExtension, attachmentInfo.ContentType, userContext);
		}
	}
}
