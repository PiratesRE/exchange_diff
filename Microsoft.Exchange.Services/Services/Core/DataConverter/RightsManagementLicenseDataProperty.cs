using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class RightsManagementLicenseDataProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public RightsManagementLicenseDataProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static RightsManagementLicenseDataProperty CreateCommand(CommandContext commandContext)
		{
			return new RightsManagementLicenseDataProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("RightsManagedLicenseProperty.ToXml should not be called.");
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ItemResponseShape itemResponseShape = commandSettings.ResponseShape as ItemResponseShape;
			if (!itemResponseShape.ClientSupportsIrm)
			{
				return;
			}
			RightsManagementLicenseDataType value;
			if (this.LoadRightsManagementLicenseData(commandSettings.StoreObject as Item, commandSettings.IdAndSession.Session, out value))
			{
				ServiceObject serviceObject = commandSettings.ServiceObject;
				serviceObject.PropertyBag[propertyInformation] = value;
			}
		}

		private static BodyType ConvertBodyFormatToBodyType(Microsoft.Exchange.Data.Storage.BodyFormat format)
		{
			switch (format)
			{
			case Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml:
			case Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf:
				return BodyType.HTML;
			}
			return BodyType.Text;
		}

		private bool LoadRightsManagementLicenseData(Item item, StoreSession session, out RightsManagementLicenseDataType rightsManagementLicenseData)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null)
			{
				rightsManagementLicenseData = null;
				return false;
			}
			if (!IrmUtils.DoesSessionSupportIrm(session))
			{
				rightsManagementLicenseData = RightsManagementLicenseDataType.CreateNoRightsTemplate();
				rightsManagementLicenseData.RightsManagedMessageDecryptionStatus = (int)RightsManagedMessageDecryptionStatus.FeatureDisabled.FailureCode;
				return true;
			}
			if (rightsManagedMessageItem.DecryptionStatus.Failed)
			{
				rightsManagementLicenseData = RightsManagementLicenseDataType.CreateNoRightsTemplate();
				rightsManagementLicenseData.RightsManagedMessageDecryptionStatus = (int)rightsManagedMessageItem.DecryptionStatus.FailureCode;
				return true;
			}
			if (!rightsManagedMessageItem.IsDecoded)
			{
				rightsManagementLicenseData = null;
				return false;
			}
			rightsManagementLicenseData = new RightsManagementLicenseDataType(rightsManagedMessageItem.UsageRights);
			if (IrmUtils.IsProtectedVoicemailItem(rightsManagedMessageItem))
			{
				rightsManagementLicenseData.ExportAllowed = false;
			}
			else
			{
				rightsManagementLicenseData.ExportAllowed = rightsManagedMessageItem.UsageRights.IsUsageRightGranted(ContentRight.Export);
			}
			if (rightsManagedMessageItem.UsageRights.IsUsageRightGranted(ContentRight.Forward) && (!rightsManagedMessageItem.Restriction.RequiresRepublishingWhenRecipientsChange || rightsManagedMessageItem.CanRepublish))
			{
				rightsManagementLicenseData.ModifyRecipientsAllowed = true;
			}
			else
			{
				rightsManagementLicenseData.ModifyRecipientsAllowed = false;
			}
			rightsManagementLicenseData.ContentOwner = rightsManagedMessageItem.ConversationOwner.EmailAddress;
			rightsManagementLicenseData.ContentExpiryDate = ExDateTimeConverter.ToUtcXsdDateTime(rightsManagedMessageItem.UserLicenseExpiryTime);
			rightsManagementLicenseData.BodyType = RightsManagementLicenseDataProperty.ConvertBodyFormatToBodyType(rightsManagedMessageItem.ProtectedBody.Format);
			RmsTemplate restriction = rightsManagedMessageItem.Restriction;
			if (restriction != null)
			{
				RightsManagementLicenseDataType rightsManagementLicenseDataType = rightsManagementLicenseData;
				Guid id = restriction.Id;
				rightsManagementLicenseDataType.RmsTemplateId = restriction.Id.ToString();
				CultureInfo locale = CultureInfo.InvariantCulture;
				MailboxSession mailboxSession = session as MailboxSession;
				if (mailboxSession != null && mailboxSession.Capabilities.CanHaveCulture)
				{
					locale = mailboxSession.PreferedCulture;
				}
				rightsManagementLicenseData.TemplateName = restriction.GetName(locale);
				rightsManagementLicenseData.TemplateDescription = restriction.GetDescription(locale);
			}
			return true;
		}

		internal static bool LoadRightsManagementLicenseData(ItemPart itemPart, out RightsManagementLicenseDataType rightsManagementLicenseData)
		{
			ItemPartIrmInfo irmInfo = itemPart.IrmInfo;
			if (irmInfo == null || !irmInfo.IsRestricted)
			{
				rightsManagementLicenseData = null;
				return false;
			}
			if (irmInfo.DecryptionStatus.Failed)
			{
				rightsManagementLicenseData = RightsManagementLicenseDataType.CreateNoRightsTemplate();
				rightsManagementLicenseData.RightsManagedMessageDecryptionStatus = (int)irmInfo.DecryptionStatus.FailureCode;
				return true;
			}
			rightsManagementLicenseData = new RightsManagementLicenseDataType(irmInfo.UsageRights);
			if (IrmUtils.IsProtectedVoicemailItem(itemPart))
			{
				rightsManagementLicenseData.ExportAllowed = false;
			}
			else
			{
				rightsManagementLicenseData.ExportAllowed = irmInfo.UsageRights.IsUsageRightGranted(ContentRight.Export);
			}
			if (irmInfo.UsageRights.IsUsageRightGranted(ContentRight.Forward) && (!irmInfo.RequiresRepublishingWhenRecipientsChange || irmInfo.CanRepublish))
			{
				rightsManagementLicenseData.ModifyRecipientsAllowed = true;
			}
			else
			{
				rightsManagementLicenseData.ModifyRecipientsAllowed = false;
			}
			rightsManagementLicenseData.ContentOwner = itemPart.IrmInfo.ConversationOwner;
			rightsManagementLicenseData.ContentExpiryDate = ExDateTimeConverter.ToUtcXsdDateTime(irmInfo.UserLicenseExpiryTime);
			rightsManagementLicenseData.TemplateDescription = irmInfo.TemplateDescription;
			rightsManagementLicenseData.TemplateName = irmInfo.TemplateName;
			rightsManagementLicenseData.BodyType = RightsManagementLicenseDataProperty.ConvertBodyFormatToBodyType(irmInfo.BodyFormat);
			return true;
		}
	}
}
