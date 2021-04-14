using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WelcomeMessageBodyData
	{
		public WelcomeMessageBodyData(WelcomeToGroupMessageTemplate template, string joinHeaderMessage, bool isMailEnabledUser, bool isAddingUserDifferent, CultureInfo cultureInfo)
		{
			ArgumentValidator.ThrowIfNull("template", template);
			ArgumentValidator.ThrowIfNullOrEmpty("joiningHeaderMessage", joinHeaderMessage);
			ArgumentValidator.ThrowIfNull("cultureInfo", cultureInfo);
			this.groupDisplayName = template.EncodedGroupDisplayName;
			this.groupDescription = template.EncodedGroupDescription;
			this.joiningHeaderMessage = joinHeaderMessage;
			this.inboxUrl = template.GroupInboxUrl;
			this.calendarUrl = template.GroupCalendarUrl;
			this.sharePointUrl = template.GroupSharePointUrl;
			this.subscribeUrl = template.SubscribeUrl;
			this.unsubscribeUrl = template.UnsubscribeUrl;
			this.groupSmtpAddress = template.Group.PrimarySmtpAddress.ToString();
			this.showExchangeLinks = !isMailEnabledUser;
			this.isAddedByDifferentUser = isAddingUserDifferent;
			this.isGroupAutoSubscribed = template.GroupIsAutoSubscribe;
			this.isSharePointEnabled = !string.IsNullOrEmpty(template.GroupSharePointUrl);
			this.cultureInfo = cultureInfo;
			this.executingUserHasPhoto = (template.ExecutingUserPhoto != null);
			this.executingUserPhotoId = ((template.ExecutingUserPhoto != null) ? template.ExecutingUserPhoto.ImageId : WelcomeMessageBodyData.BlankGifImage.ImageId);
			this.groupHasPhoto = (template.GroupPhoto != null);
			this.groupPhotoId = ((template.GroupPhoto != null) ? template.GroupPhoto.ImageId : WelcomeMessageBodyData.BlankGifImage.ImageId);
			this.groupType = template.Group.ModernGroupType;
		}

		public string GroupTitleHeading
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailHeader(this.groupDisplayName).ToString(this.cultureInfo);
			}
		}

		public string GroupDescription
		{
			get
			{
				if (string.IsNullOrEmpty(this.groupDescription))
				{
					return ClientStrings.GroupMailboxWelcomeEmailDefaultDescription.ToString(this.cultureInfo);
				}
				return this.groupDescription;
			}
		}

		public string JoiningHeaderMessage
		{
			get
			{
				return this.joiningHeaderMessage;
			}
		}

		public string SubscribeActivityToInboxLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailSubscribeToInboxTitle.ToString(this.cultureInfo);
			}
		}

		public string SubscribeActivityToInboxDescriptionLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailSubscribeToInboxSubtitle.ToString(this.cultureInfo);
			}
		}

		public string StartConversationLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailStartConversationTitle.ToString(this.cultureInfo);
			}
		}

		public string StartConversationEmail
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailStartConversationSubtitle(this.GroupSmtpAddress).ToString(this.cultureInfo);
			}
		}

		public string GroupDocumentsMainTitleHeaderLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailShareFilesTitle.ToString(this.cultureInfo);
			}
		}

		public string GroupDocumentsMainDescriptionHeaderLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailShareFilesSubTitle.ToString(this.cultureInfo);
			}
		}

		public string CollaborateHeadingLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailO365FooterTitle.ToString(this.cultureInfo) + "&nbsp;";
			}
		}

		public string GroupConversationsLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailO365FooterBrowseConversations.ToString(this.cultureInfo) + "&nbsp;";
			}
		}

		public string GroupCalendarLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailO365FooterBrowseViewCalendar.ToString(this.cultureInfo) + "&nbsp;";
			}
		}

		public string GroupDocumentsLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailO365FooterBrowseShareFiles.ToString(this.cultureInfo);
			}
		}

		public string GroupTypeStringLabel
		{
			get
			{
				if (this.groupType == ModernGroupObjectType.Private)
				{
					return ClientStrings.GroupMailboxWelcomeEmailPrivateTypeText.ToString(this.cultureInfo);
				}
				return ClientStrings.GroupMailboxWelcomeEmailPublicTypeText.ToString(this.cultureInfo);
			}
		}

		public string SubscribeFooterPrefixLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailFooterSubscribeDescriptionText.ToString(this.cultureInfo);
			}
		}

		public string SubscribeFooterLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailFooterSubscribeLinkText.ToString(this.cultureInfo);
			}
		}

		public string UnsubscribeFooterPrefixLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailFooterUnsubscribeDescirptionText.ToString(this.cultureInfo);
			}
		}

		public string UnsubscribeFooterLabel
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeEmailFooterUnsubscribeLinkText.ToString(this.cultureInfo);
			}
		}

		public string InboxUrl
		{
			get
			{
				return this.inboxUrl;
			}
		}

		public string CalendarUrl
		{
			get
			{
				return this.calendarUrl;
			}
		}

		public string SharePointUrl
		{
			get
			{
				return this.sharePointUrl;
			}
		}

		public string SubscribeUrl
		{
			get
			{
				return this.subscribeUrl;
			}
		}

		public string UnsubscribeUrl
		{
			get
			{
				return this.unsubscribeUrl;
			}
		}

		public string GroupSmtpAddress
		{
			get
			{
				return this.groupSmtpAddress;
			}
		}

		public string ExecutingUserPhotoId
		{
			get
			{
				return "cid:" + this.executingUserPhotoId;
			}
		}

		public string GroupPhotoId
		{
			get
			{
				return "cid:" + this.groupPhotoId;
			}
		}

		public string ConverasationsImageId
		{
			get
			{
				return "cid:" + WelcomeMessageBodyData.WelcomeConversationsIcon.ImageId;
			}
		}

		public string FilesImageId
		{
			get
			{
				return "cid:" + WelcomeMessageBodyData.WelcomeDocumentIcon.ImageId;
			}
		}

		public string O365ImageId
		{
			get
			{
				return "cid:" + WelcomeMessageBodyData.WelcomeO365Icon.ImageId;
			}
		}

		public string ArrowImageId
		{
			get
			{
				return "cid:" + WelcomeMessageBodyData.WelcomeArrowIcon.ImageId;
			}
		}

		public string FlippedArrowImageId
		{
			get
			{
				return "cid:" + WelcomeMessageBodyData.WelcomeArrowFlippedIcon.ImageId;
			}
		}

		public bool IsRightToLeftLanguage
		{
			get
			{
				return this.cultureInfo.TextInfo.IsRightToLeft;
			}
		}

		public bool IsLeftToRightLanguage
		{
			get
			{
				return !this.cultureInfo.TextInfo.IsRightToLeft;
			}
		}

		public string NormalTextFlowDirectionCss
		{
			get
			{
				if (!this.IsRightToLeftLanguage)
				{
					return "ltr";
				}
				return "rtl";
			}
		}

		public string ReverseTextFlowDirectionCss
		{
			get
			{
				if (!this.IsRightToLeftLanguage)
				{
					return "rtl";
				}
				return "ltr";
			}
		}

		public bool ShowSubscribeBody
		{
			get
			{
				return !this.isGroupAutoSubscribed;
			}
		}

		public bool ShowExchangeLinks
		{
			get
			{
				return this.showExchangeLinks;
			}
		}

		public bool IsAddedByDifferentUser
		{
			get
			{
				return this.isAddedByDifferentUser;
			}
		}

		public bool ExecutingUserHasPhoto
		{
			get
			{
				return this.executingUserHasPhoto;
			}
		}

		public bool GroupHasPhoto
		{
			get
			{
				return this.groupHasPhoto;
			}
		}

		public bool IsGroupAutoSubscribed
		{
			get
			{
				return this.isGroupAutoSubscribed;
			}
		}

		public bool IsSharePointEnabled
		{
			get
			{
				return this.isSharePointEnabled;
			}
		}

		public bool ShowPhotosHeader
		{
			get
			{
				return this.ExecutingUserHasPhoto || this.GroupHasPhoto;
			}
		}

		public ModernGroupObjectType GroupType
		{
			get
			{
				return this.groupType;
			}
		}

		private const string AttachmentIdPrefix = "cid:";

		public static readonly string UserPhotoImageId = "user_photo";

		public static readonly string GroupPhotoImageId = "group_photo";

		public static readonly ImageAttachment WelcomeConversationsIcon = new ImageAttachment("welcome_conversations_icon.png", "welcome_conversations_icon", "image/png", null);

		public static readonly ImageAttachment WelcomeDocumentIcon = new ImageAttachment("welcome_files_icon.png", "welcome_files_icon", "image/png", null);

		public static readonly ImageAttachment WelcomeO365Icon = new ImageAttachment("welcome_O365_icon.png", "welcome_O365_icon", "image/png", null);

		public static readonly ImageAttachment WelcomeArrowIcon = new ImageAttachment("welcome_arrow.png", "welcome_arrow", "image/png", null);

		public static readonly ImageAttachment WelcomeArrowFlippedIcon = new ImageAttachment("welcome_arrow_flipped.png", "welcome_arrow_flipped", "image/png", null);

		public static readonly ImageAttachment BlankGifImage = new ImageAttachment("blank.gif", "blank", "image/gif", null);

		private readonly string groupDisplayName;

		private readonly string groupDescription;

		private readonly string joiningHeaderMessage;

		private readonly string inboxUrl;

		private readonly string calendarUrl;

		private readonly string sharePointUrl;

		private readonly string subscribeUrl;

		private readonly string unsubscribeUrl;

		private readonly string groupSmtpAddress;

		private readonly string executingUserPhotoId;

		private readonly string groupPhotoId;

		private readonly bool showExchangeLinks;

		private readonly bool isAddedByDifferentUser;

		private readonly bool executingUserHasPhoto;

		private readonly bool groupHasPhoto;

		private readonly bool isGroupAutoSubscribed;

		private readonly bool isSharePointEnabled;

		private ModernGroupObjectType groupType;

		private CultureInfo cultureInfo;
	}
}
