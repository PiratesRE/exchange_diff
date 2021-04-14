using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemPartIrmInfo
	{
		private ItemPartIrmInfo(bool isRestricted, ContentRight usageRights, string templateName, string templateDescription, string conversationOwner, RightsManagedMessageDecryptionStatus decryptionStatus, ExDateTime userLicenseExpiryTime, bool requiresRepublishingWhenRecipientsChange, bool canRepublish)
		{
			this.isRestricted = isRestricted;
			this.usageRights = usageRights;
			this.templateName = templateName;
			this.templateDescription = templateDescription;
			this.conversationOwner = conversationOwner;
			this.decryptionStatus = decryptionStatus;
			this.userLicenseExpiryTime = userLicenseExpiryTime;
			this.requiresRepublishingWhenRecipientsChange = requiresRepublishingWhenRecipientsChange;
			this.canRepublish = canRepublish;
		}

		public bool IsRestricted
		{
			get
			{
				return this.isRestricted;
			}
		}

		public ContentRight UsageRights
		{
			get
			{
				return this.usageRights;
			}
		}

		public string TemplateName
		{
			get
			{
				return this.templateName;
			}
		}

		public string TemplateDescription
		{
			get
			{
				return this.templateDescription;
			}
		}

		public string ConversationOwner
		{
			get
			{
				return this.conversationOwner;
			}
		}

		public RightsManagedMessageDecryptionStatus DecryptionStatus
		{
			get
			{
				return this.decryptionStatus;
			}
		}

		public ExDateTime UserLicenseExpiryTime
		{
			get
			{
				return this.userLicenseExpiryTime;
			}
		}

		public bool RequiresRepublishingWhenRecipientsChange
		{
			get
			{
				return this.requiresRepublishingWhenRecipientsChange;
			}
		}

		public bool CanRepublish
		{
			get
			{
				return this.canRepublish;
			}
		}

		public BodyFormat BodyFormat { get; set; }

		internal static ItemPartIrmInfo Create(ContentRight usageRights, string templateName, string templateDescription, string conversationOwner, ExDateTime userLicenseExpiryTime, bool requiresRepublishingWhenRecipientsChange, bool canRepublish)
		{
			return new ItemPartIrmInfo(true, usageRights, templateName, templateDescription, conversationOwner, RightsManagedMessageDecryptionStatus.Success, userLicenseExpiryTime, requiresRepublishingWhenRecipientsChange, canRepublish);
		}

		internal static ItemPartIrmInfo CreateForDecryptionFailure(Exception exception)
		{
			return new ItemPartIrmInfo(true, ContentRight.None, string.Empty, string.Empty, string.Empty, RightsManagedMessageDecryptionStatus.CreateFromException(exception), ExDateTime.MinValue, true, false);
		}

		internal static ItemPartIrmInfo CreateForIrmDisabled()
		{
			return new ItemPartIrmInfo(true, ContentRight.None, string.Empty, string.Empty, string.Empty, RightsManagedMessageDecryptionStatus.FeatureDisabled, ExDateTime.MinValue, true, false);
		}

		internal static ItemPartIrmInfo CreateForUnsupportedScenario()
		{
			return new ItemPartIrmInfo(true, ContentRight.None, string.Empty, string.Empty, string.Empty, RightsManagedMessageDecryptionStatus.NotSupported, ExDateTime.MinValue, true, false);
		}

		internal static readonly ItemPartIrmInfo NotRestricted = new ItemPartIrmInfo(false, ContentRight.Owner, string.Empty, string.Empty, string.Empty, RightsManagedMessageDecryptionStatus.Success, ExDateTime.MaxValue, false, true);

		private readonly ExDateTime userLicenseExpiryTime;

		private readonly bool requiresRepublishingWhenRecipientsChange;

		private readonly bool canRepublish;

		private bool isRestricted;

		private ContentRight usageRights;

		private string templateName;

		private string templateDescription;

		private string conversationOwner;

		private RightsManagedMessageDecryptionStatus decryptionStatus;
	}
}
