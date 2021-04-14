using System;
using System.Globalization;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessageSecurity.MessageClassifications
{
	internal class SystemClassificationSummary : ClassificationSummary
	{
		private SystemClassificationSummary(string name, Guid classificationID, LocalizedString displayName, LocalizedString senderDescription, LocalizedString recipientDescription, ClassificationDisplayPrecedenceLevel displayPrecedence, bool permissionMenuVisible, bool retainClassificationEnabled) : base(name, classificationID, null, displayName, senderDescription, recipientDescription, displayPrecedence, permissionMenuVisible, retainClassificationEnabled)
		{
			this.localizedDisplayName = displayName;
			this.localizedSenderDescription = senderDescription;
			this.localizedRecipientDescription = recipientDescription;
		}

		public static ClassificationSummary GetClassificationSummary(MessageClassification classification, CultureInfo locale)
		{
			foreach (SystemClassificationSummary systemClassificationSummary in SystemClassificationSummary.DefaultClassifications)
			{
				if (classification.ClassificationID == systemClassificationSummary.ClassificationID)
				{
					return new ClassificationSummary(classification.Name, classification.ClassificationID, locale.ToString(), systemClassificationSummary.localizedDisplayName.ToString(locale), systemClassificationSummary.localizedSenderDescription.ToString(locale), systemClassificationSummary.localizedRecipientDescription.ToString(locale), classification.DisplayPrecedence, classification.PermissionMenuVisible, classification.RetainClassificationEnabled);
				}
			}
			return null;
		}

		public const string PartnerMessageClassificationGuid = "030e9e2f-134b-4020-861c-5bfc616f113d";

		public const string OrarMessageClassificationGuid = "3f4cc40b-2a9f-4be5-8a55-0e3fdacddd43";

		public static readonly SystemClassificationSummary[] DefaultClassifications = new SystemClassificationSummary[]
		{
			new SystemClassificationSummary("ExPartnerMail", new Guid("030e9e2f-134b-4020-861c-5bfc616f113d"), SystemMessages.ExPartnerMailDisplayName, LocalizedString.Empty, LocalizedString.Empty, ClassificationDisplayPrecedenceLevel.Low, false, false),
			new SystemClassificationSummary("ExOrarMail", new Guid("3f4cc40b-2a9f-4be5-8a55-0e3fdacddd43"), SystemMessages.ExOrarMailDisplayName, SystemMessages.ExOrarMailSenderDescription, SystemMessages.ExOrarMailRecipientDescription, ClassificationDisplayPrecedenceLevel.Medium, false, false),
			new SystemClassificationSummary("ExAttachmentRemoved", new Guid("a4bb0cb2-4395-4d18-9799-1f904b20fe92"), SystemMessages.ExAttachmentRemovedDisplayName, SystemMessages.ExAttachmentRemovedSenderDescription, SystemMessages.ExAttachmentRemovedRecipientDescription, ClassificationDisplayPrecedenceLevel.Low, false, false)
		};

		private LocalizedString localizedDisplayName = LocalizedString.Empty;

		private LocalizedString localizedSenderDescription = LocalizedString.Empty;

		private LocalizedString localizedRecipientDescription = LocalizedString.Empty;
	}
}
