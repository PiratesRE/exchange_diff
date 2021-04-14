using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.PersonalAutoAttendant;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class UnifiedMessagingOptions : OwaPage, IRegistryOnlyForm
	{
		protected Toolbar PersonalAutoAttendantToolBar
		{
			get
			{
				return this.personalAutoAttendantToolbar;
			}
		}

		protected string TelephoneFolderId
		{
			get
			{
				return this.telephoneFolderId.ToBase64String();
			}
		}

		protected string TelephoneFolderName
		{
			get
			{
				return this.telephoneFolderName;
			}
			set
			{
				this.telephoneFolderName = Utilities.HtmlEncode(value);
			}
		}

		protected string TelephoneNumber
		{
			get
			{
				return this.dialOnPhoneNumber;
			}
		}

		protected bool IsOutOfOfficeGreeting
		{
			get
			{
				return this.isOutOfOffice;
			}
		}

		protected bool IsFIFOOrder
		{
			get
			{
				return this.isFIFOOrder;
			}
		}

		protected UMSMSNotificationOptions SMSNotificationValue
		{
			get
			{
				return this.smsNotificationValue;
			}
		}

		protected bool IsPinlessVoicemail
		{
			get
			{
				return this.isPinlessVoicemail;
			}
		}

		protected bool IsDialPlanEnterprise
		{
			get
			{
				return this.isDialPlanEnterprise;
			}
		}

		protected bool IsVoicemailPreviewReceive
		{
			get
			{
				return this.isVoicemailPreviewReceive;
			}
		}

		protected bool IsVoicemailPreviewSend
		{
			get
			{
				return this.isVoicemailPreviewSend;
			}
		}

		protected bool IsMissedCallNotification
		{
			get
			{
				return this.isMissedCallNotification;
			}
		}

		protected bool IsEnabledForVoicemailPreview
		{
			get
			{
				return this.enabledForVoicemailPreview;
			}
		}

		protected bool IsEnabledForPinlessVoicemail
		{
			get
			{
				return this.enabledForPinlessVoicemail;
			}
		}

		protected bool IsEnabledForSMSNotification
		{
			get
			{
				return this.enabledForSMSNotification;
			}
		}

		protected bool IsEnabledForMissedCallNotification
		{
			get
			{
				return this.enabledForMissedCallNotification;
			}
		}

		protected bool IsEnabledForPersonalAutoAttendant
		{
			get
			{
				return this.enabledForPersonalAutoAttendant;
			}
		}

		protected bool IsEnabledForOutDialing
		{
			get
			{
				return this.enabledForOutdialing;
			}
		}

		protected bool IsPlayOnPhoneEnabled
		{
			get
			{
				return this.isPlayOnPhoneEnabled;
			}
		}

		protected Infobar PaaInfobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.infobar = new Infobar();
			using (UMClientCommon umclientCommon = new UMClientCommon(base.UserContext.ExchangePrincipal))
			{
				if (!umclientCommon.IsUMEnabled())
				{
					throw new OwaInvalidRequestException("User is not UM enabled");
				}
				this.isPlayOnPhoneEnabled = umclientCommon.IsPlayOnPhoneEnabled();
				this.enabledForPinlessVoicemail = umclientCommon.UMPolicy.AllowPinlessVoiceMailAccess;
				this.enabledForSMSNotification = umclientCommon.IsSmsNotificationsEnabled();
				this.enabledForVoicemailPreview = umclientCommon.UMPolicy.AllowVoiceMailPreview;
				this.enabledForMissedCallNotification = umclientCommon.UMPolicy.AllowMissedCallNotifications;
				this.isDialPlanEnterprise = (umclientCommon.DialPlan.SubscriberType == UMSubscriberType.Enterprise);
				UMPropertiesEx umproperties = umclientCommon.GetUMProperties();
				this.isOutOfOffice = umproperties.OofStatus;
				this.isFIFOOrder = umproperties.ReadUnreadVoicemailInFIFOOrder;
				this.isMissedCallNotification = umproperties.MissedCallNotificationEnabled;
				this.dialOnPhoneNumber = umproperties.PlayOnPhoneDialString;
				this.telephoneAccessNumbers = umproperties.TelephoneAccessNumbers;
				this.isPinlessVoicemail = umproperties.PinlessAccessToVoicemail;
				this.smsNotificationValue = umproperties.SMSNotificationOption;
				this.isVoicemailPreviewReceive = umproperties.ReceivedVoiceMailPreviewEnabled;
				this.isVoicemailPreviewSend = umproperties.SentVoiceMailPreviewEnabled;
				try
				{
					this.telephoneFolderId = StoreObjectId.FromProviderSpecificId(Convert.FromBase64String(umproperties.TelephoneAccessFolderEmail));
				}
				catch (CorruptDataException arg)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string, CorruptDataException>(0L, "Invalid format for Folder ID string: {0}. Exception: {1}", umproperties.TelephoneAccessFolderEmail, arg);
				}
				catch (FormatException arg2)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string, FormatException>(0L, "Invalid format for Folder ID string: {0}. Exception: {1}", umproperties.TelephoneAccessFolderEmail, arg2);
				}
			}
			if (this.telephoneFolderId == null)
			{
				this.telephoneFolderId = base.UserContext.InboxFolderId;
			}
			try
			{
				using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, this.telephoneFolderId, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				}))
				{
					this.TelephoneFolderName = folder.DisplayName;
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<StoreObjectId>(0L, "The folder doesn't exist. Folder Id: {0}", this.telephoneFolderId);
				this.telephoneFolderId = base.UserContext.InboxFolderId;
				using (Folder folder2 = Folder.Bind(base.UserContext.MailboxSession, this.telephoneFolderId, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				}))
				{
					this.TelephoneFolderName = folder2.DisplayName;
				}
			}
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				ipaastore.GetUserPermissions(out this.enabledForPersonalAutoAttendant, out this.enabledForOutdialing);
				if (this.enabledForPersonalAutoAttendant)
				{
					this.personalAutoAttendantToolbar = new PersonalAutoAttendantListToolbar();
				}
			}
		}

		protected void RenderPersonalAutoAttendantList()
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.UserContext.ExchangePrincipal))
			{
				IList<PersonalAutoAttendant> personalAutoAttendants = null;
				ipaastore.TryGetAutoAttendants(PAAValidationMode.StopOnFirstError, out personalAutoAttendants);
				PersonalAutoAttendantListView personalAutoAttendantListView = new PersonalAutoAttendantListView(base.UserContext, personalAutoAttendants);
				personalAutoAttendantListView.Render(base.Response.Output);
			}
		}

		protected void RenderTelephoneHeader()
		{
			if (string.IsNullOrEmpty(this.telephoneAccessNumbers))
			{
				base.Response.Output.Write(LocalizedStrings.GetHtmlEncoded(1121744548));
				return;
			}
			base.Response.Output.Write(LocalizedStrings.GetHtmlEncoded(1879630091), "<B>" + Utilities.HtmlEncode(this.telephoneAccessNumbers) + "</B>");
		}

		protected bool IsSMSConfigured()
		{
			if (this.isLazySMSConfigured == null)
			{
				this.isLazySMSConfigured = new bool?(UnifiedMessagingOptions.IsSMSConfigured(base.UserContext));
			}
			return this.isLazySMSConfigured.Value;
		}

		internal static bool IsSMSConfigured(UserContext context)
		{
			bool result;
			using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(context.MailboxSession))
			{
				ADObjectId objectId = context.MailboxSession.MailboxOwner.ObjectId;
				TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(objectId);
				result = (textMessagingAccount != null && textMessagingAccount.NotificationPhoneNumber != null && textMessagingAccount.NotificationPhoneNumberVerified);
			}
			return result;
		}

		private bool isOutOfOffice;

		private bool isFIFOOrder;

		private string telephoneAccessNumbers;

		private string dialOnPhoneNumber;

		private StoreObjectId telephoneFolderId;

		private string telephoneFolderName;

		private bool isMissedCallNotification;

		private bool enabledForPersonalAutoAttendant;

		private bool enabledForOutdialing;

		private bool enabledForMissedCallNotification;

		private bool enabledForVoicemailPreview;

		private bool enabledForPinlessVoicemail;

		private bool enabledForSMSNotification;

		private bool isPlayOnPhoneEnabled;

		private bool isPinlessVoicemail;

		private bool isDialPlanEnterprise;

		private bool isVoicemailPreviewReceive;

		private bool isVoicemailPreviewSend;

		private Toolbar personalAutoAttendantToolbar;

		private UMSMSNotificationOptions smsNotificationValue;

		private bool? isLazySMSConfigured;

		private Infobar infobar;
	}
}
