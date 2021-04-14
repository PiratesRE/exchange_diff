using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentPolicyChecker : AttachmentHandler.IAttachmentPolicyChecker
	{
		private AttachmentPolicyChecker()
		{
		}

		private AttachmentPolicyChecker(AttachmentPolicy policy)
		{
			this.policy = policy;
		}

		public AttachmentPolicyLevel GetPolicy(IAttachment attachment, bool isPublicLogin)
		{
			AttachmentPolicyLevel attachmentPolicyLevel = AttachmentPolicyLevel.Unknown;
			AttachmentPolicyLevel attachmentPolicyLevel2 = AttachmentPolicyLevel.Unknown;
			string fileExtension = attachment.FileExtension;
			string text = attachment.ContentType ?? attachment.CalculatedContentType;
			bool directFileAccessEnabled = this.policy.GetDirectFileAccessEnabled(isPublicLogin);
			if (text == null || !directFileAccessEnabled)
			{
				return AttachmentPolicyLevel.Block;
			}
			if (!string.IsNullOrEmpty(fileExtension))
			{
				attachmentPolicyLevel2 = this.policy.GetLevel(fileExtension, AttachmentPolicy.TypeSignifier.File);
			}
			if (!string.IsNullOrEmpty(text))
			{
				attachmentPolicyLevel = this.policy.GetLevel(text, AttachmentPolicy.TypeSignifier.Mime);
			}
			if (attachmentPolicyLevel2 == AttachmentPolicyLevel.Allow || attachmentPolicyLevel == AttachmentPolicyLevel.Allow)
			{
				return AttachmentPolicyLevel.Allow;
			}
			if (attachmentPolicyLevel2 == AttachmentPolicyLevel.Block || attachmentPolicyLevel == AttachmentPolicyLevel.Block)
			{
				return AttachmentPolicyLevel.Block;
			}
			if (attachmentPolicyLevel2 == AttachmentPolicyLevel.ForceSave || attachmentPolicyLevel == AttachmentPolicyLevel.ForceSave)
			{
				return AttachmentPolicyLevel.ForceSave;
			}
			return this.policy.TreatUnknownTypeAs;
		}

		internal static AttachmentHandler.IAttachmentPolicyChecker CreateInstance(AttachmentPolicy policy)
		{
			return new AttachmentPolicyChecker(policy);
		}

		private AttachmentPolicy policy;
	}
}
