using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class TextMessageDeliveryContext
	{
		public bool PartnerDelivery
		{
			get
			{
				if (this.AgentWrapper != null)
				{
					return this.AgentWrapper.PartnerDelivery;
				}
				return this.partnerDelivery;
			}
			internal set
			{
				if (this.AgentWrapper != null)
				{
					this.AgentWrapper.PartnerDelivery = value;
				}
				this.partnerDelivery = value;
			}
		}

		public TransportAgentWrapper AgentWrapper { get; internal set; }

		public QueueDataAvailableEventHandler<TextMessageDeliveryContext> CleanerEventHandler { get; internal set; }

		public Uri EcpLinkUrl { get; internal set; }

		public string EcpTextMessagingSlab
		{
			get
			{
				if (this.ecpTextMessagingSlab == null)
				{
					this.ecpTextMessagingSlab = ((null == this.EcpLinkUrl) ? string.Empty : AntiXssEncoder.HtmlEncode(this.EcpLinkUrl.ToString() + "/sms/textmessaging.slab", false));
				}
				return this.ecpTextMessagingSlab;
			}
		}

		public string EcpEditNotificatonWizard
		{
			get
			{
				if (this.ecpEditNotificatonWizard == null)
				{
					this.ecpEditNotificatonWizard = ((null == this.EcpLinkUrl) ? string.Empty : AntiXssEncoder.HtmlEncode(this.EcpLinkUrl.ToString() + "/sms/EditNotification.aspx", false));
				}
				return this.ecpEditNotificatonWizard;
			}
		}

		public string EcpInboxRuleSlab
		{
			get
			{
				if (this.ecpInboxRuleSlab == null)
				{
					this.ecpInboxRuleSlab = ((null == this.EcpLinkUrl) ? string.Empty : AntiXssEncoder.HtmlEncode(this.EcpLinkUrl.ToString() + "/RulesEditor/InboxRules.slab", false));
				}
				return this.ecpInboxRuleSlab;
			}
		}

		public string MapiMessageClass { get; internal set; }

		public bool IsUndercurrentMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.MapiMessageClass, "IPM.Note.Mobile.SMS.Undercurrent");
			}
		}

		public bool IsAlertMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.MapiMessageClass, "IPM.Note.Mobile.SMS.Alert");
			}
		}

		public bool IsRuleAlertMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.MapiMessageClass, "IPM.Note.Mobile.SMS.Alert", false);
			}
		}

		public bool IsVoicemailMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.MapiMessageClass, "IPM.Note.Mobile.SMS.Alert.Voicemail");
			}
		}

		public bool IsAlertInfoMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.MapiMessageClass, "IPM.Note.Mobile.SMS.Alert.Info");
			}
		}

		public MessageItem Message { get; internal set; }

		public IList<TextSendingPackage> TextSendingPackages { get; internal set; }

		public ExchangePrincipal Principal
		{
			get
			{
				return this.principal;
			}
		}

		public void SetPrincipalFromProxyAddress(string proxyAddress)
		{
			this.principal = ExchangePrincipal.FromProxyAddress(this.AgentWrapper.ADSessionSettings, proxyAddress);
		}

		public IDictionary<MobileRecipient, EnvelopeRecipient> Recipients { get; internal set; }

		public TextMessagingSettingsVersion1Point0 Settings { get; internal set; }

		public IMobileService MobileService { get; internal set; }

		public CalendarNotificationType CalNotifTypeHint { get; internal set; }

		public bool IsFromOutlook { get; internal set; }

		public CultureInfo NotificationPreferredCulture { get; internal set; }

		private ExchangePrincipal principal;

		private string ecpTextMessagingSlab;

		private string ecpEditNotificatonWizard;

		private string ecpInboxRuleSlab;

		private bool partnerDelivery;
	}
}
