using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MobileTransport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Delivery;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class TransportAgentWrapper
	{
		public TransportAgentWrapper(AgentAsyncContext asyncContext, SubmittedMessageEventSource src, QueuedMessageEventArgs e) : this(asyncContext)
		{
			if (src == null)
			{
				throw new ArgumentNullException("src");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			this.SubmittedMessageEventSource = src;
			this.QueuedMessageEventArgs = e;
		}

		public TransportAgentWrapper(AgentAsyncContext asyncContext, DeliverMailItemEventSource src, DeliverMailItemEventArgs e) : this(asyncContext)
		{
			if (src == null)
			{
				throw new ArgumentNullException("src");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			this.DeliverMailItemEventSource = src;
			this.DeliverMailItemEventArgs = e;
			this.IsInDeliveryAgent = true;
		}

		private TransportAgentWrapper(AgentAsyncContext asyncContext)
		{
			ExSmsCounters.PendingDelivery.Increment();
			this.AgentAsyncContext = asyncContext;
		}

		public bool IsInDeliveryAgent { get; private set; }

		public bool PartnerDelivery { get; internal set; }

		public ReadOnlyEnvelopeRecipientCollection ReadOnlyRecipients
		{
			get
			{
				if (this.readOnlyRecipients == null)
				{
					this.readOnlyRecipients = (this.IsInDeliveryAgent ? this.DeliverMailItemEventArgs.DeliverableMailItem.Recipients : this.QueuedMessageEventArgs.MailItem.Recipients);
				}
				return this.readOnlyRecipients;
			}
		}

		public EmailMessage EmailMessage
		{
			get
			{
				if (this.emailMessage == null)
				{
					this.emailMessage = (this.IsInDeliveryAgent ? this.DeliverMailItemEventArgs.DeliverableMailItem.Message : this.QueuedMessageEventArgs.MailItem.Message);
				}
				return this.emailMessage;
			}
		}

		public DsnParameters MailItemDsnParametersCopy
		{
			get
			{
				if (this.mailItemDsnParametersCopy == null)
				{
					this.mailItemDsnParametersCopy = new DsnParameters();
				}
				return this.mailItemDsnParametersCopy;
			}
		}

		internal ADSessionSettings ADSessionSettings
		{
			get
			{
				if (this.adSessionSettings == null)
				{
					OrganizationId organizationId = null;
					if (this.IsInDeliveryAgent)
					{
						RoutedMailItemWrapper routedMailItemWrapper = this.DeliverMailItemEventArgs.DeliverableMailItem as RoutedMailItemWrapper;
						if (routedMailItemWrapper != null && routedMailItemWrapper.RoutedMailItem != null)
						{
							organizationId = routedMailItemWrapper.RoutedMailItem.OrganizationId;
						}
					}
					else
					{
						TransportMailItemWrapper transportMailItemWrapper = this.QueuedMessageEventArgs.MailItem as TransportMailItemWrapper;
						if (transportMailItemWrapper != null && transportMailItemWrapper.TransportMailItem != null)
						{
							organizationId = transportMailItemWrapper.TransportMailItem.OrganizationId;
						}
					}
					this.adSessionSettings = ((null == organizationId) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId));
				}
				return this.adSessionSettings;
			}
		}

		private AgentAsyncContext AgentAsyncContext { get; set; }

		private DeliverMailItemEventSource DeliverMailItemEventSource { get; set; }

		private DeliverMailItemEventArgs DeliverMailItemEventArgs { get; set; }

		private SubmittedMessageEventSource SubmittedMessageEventSource { get; set; }

		private QueuedMessageEventArgs QueuedMessageEventArgs { get; set; }

		private TransportMailItem TransportMailItem
		{
			get
			{
				if (!this.IsInDeliveryAgent && this.transportMailItem == null)
				{
					this.transportMailItem = ((TransportMailItemWrapper)this.QueuedMessageEventArgs.MailItem).TransportMailItem;
				}
				return this.transportMailItem;
			}
		}

		private MimePart RootPart
		{
			get
			{
				if (this.rootPart == null)
				{
					if (this.IsInDeliveryAgent)
					{
						this.rootPart = this.DeliverMailItemEventArgs.DeliverableMailItem.Message.RootPart;
					}
					else if (this.QueuedMessageEventArgs.MailItem.MimeDocument == null)
					{
						this.rootPart = this.QueuedMessageEventArgs.MailItem.Message.RootPart;
					}
					else
					{
						try
						{
							this.rootPart = this.QueuedMessageEventArgs.MailItem.MimeDocument.RootPart;
						}
						catch (ObjectDisposedException arg)
						{
							this.rootPart = this.QueuedMessageEventArgs.MailItem.Message.RootPart;
							ExTraceGlobals.XsoTracer.TraceError<ObjectDisposedException>((long)this.GetHashCode(), "MimeDocument has been disposed. Raises Exception: {0}", arg);
						}
					}
				}
				return this.rootPart;
			}
		}

		private EnvelopeRecipientCollection Recipients
		{
			get
			{
				return this.ReadOnlyRecipients as EnvelopeRecipientCollection;
			}
		}

		public static DsnParameters CloneDsnParameters(DsnParameters src)
		{
			if (src == null)
			{
				return null;
			}
			DsnParameters dsnParameters = new DsnParameters();
			foreach (KeyValuePair<string, object> keyValuePair in src)
			{
				dsnParameters[keyValuePair.Key] = keyValuePair.Value;
			}
			return dsnParameters;
		}

		public static void AddDsnParameters(MailRecipient recipient, DsnParameters dsnParam)
		{
			if (dsnParam == null)
			{
				return;
			}
			foreach (KeyValuePair<string, object> keyValuePair in dsnParam)
			{
				recipient.AddDsnParameters(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static MailRecipient CastEnvelopeRecipientToMailRecipient(EnvelopeRecipient envelopeRecipient)
		{
			return ((MailRecipientWrapper)envelopeRecipient).MailRecipient;
		}

		public bool TrySetRecipientDsnTypeRequested(EnvelopeRecipient recipient, DsnTypeRequested requested)
		{
			if (this.IsInDeliveryAgent)
			{
				return false;
			}
			recipient.RequestedReports = requested;
			return true;
		}

		public void AckRecipient(EnvelopeRecipient recipient, AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			if (!this.IsInDeliveryAgent)
			{
				TransportAgentWrapper.CastEnvelopeRecipientToMailRecipient(recipient).Ack(ackStatus, smtpResponse);
				return;
			}
			switch (ackStatus)
			{
			case AckStatus.Success:
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
				this.DeliverMailItemEventSource.AckRecipientSuccess(recipient, smtpResponse);
				return;
			case AckStatus.Retry:
				this.DeliverMailItemEventSource.AckRecipientDefer(recipient, smtpResponse);
				return;
			case AckStatus.Fail:
				this.DeliverMailItemEventSource.AckRecipientFail(recipient, smtpResponse);
				return;
			default:
				this.DeliverMailItemEventSource.AckRecipientFail(recipient, smtpResponse);
				return;
			}
		}

		public void LogTrackingForTransportMailItem()
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			if (!this.PartnerDelivery)
			{
				MessageTrackingLog.TrackRelayedAndFailed(MessageTrackingSource.AGENT, this.TransportMailItem, this.TransportMailItem.Recipients, null);
			}
		}

		public void RemoveRecipients(IList<EnvelopeRecipient> recipients)
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			foreach (EnvelopeRecipient recipient in recipients)
			{
				this.Recipients.Remove(recipient);
			}
		}

		public void AddDsnParameters(string key, object value)
		{
			this.MailItemDsnParametersCopy[key] = value;
			if (this.IsInDeliveryAgent)
			{
				this.DeliverMailItemEventSource.AddDsnParameters(key, value);
				return;
			}
			this.TransportMailItem.AddDsnParameters(key, value);
		}

		public void AddDsnParameters(EnvelopeRecipient recipient, string key, object value)
		{
			if (this.IsInDeliveryAgent)
			{
				this.DeliverMailItemEventSource.AddDsnParameters(recipient, key, value);
				return;
			}
			TransportAgentWrapper.CastEnvelopeRecipientToMailRecipient(recipient).AddDsnParameters(key, value);
		}

		public bool DoDsnParametersExist(string key)
		{
			if (this.IsInDeliveryAgent)
			{
				object obj = null;
				return this.DeliverMailItemEventSource.TryGetDsnParameters(key, out obj);
			}
			return this.TransportMailItem.DsnParameters != null && this.TransportMailItem.DsnParameters.ContainsKey(key);
		}

		public bool TryGetDsnParameters(string key, out object value)
		{
			value = null;
			if (this.IsInDeliveryAgent)
			{
				return this.DeliverMailItemEventSource.TryGetDsnParameters(key, out value);
			}
			return this.TransportMailItem.DsnParameters != null && this.TransportMailItem.DsnParameters.TryGetValue(key, out value);
		}

		public bool DoDsnParametersExist(EnvelopeRecipient recipient, string key)
		{
			if (this.IsInDeliveryAgent)
			{
				object obj = null;
				return this.DeliverMailItemEventSource.TryGetDsnParameters(recipient, key, out obj);
			}
			MailRecipient mailRecipient = TransportAgentWrapper.CastEnvelopeRecipientToMailRecipient(recipient);
			return mailRecipient.DsnParameters != null && mailRecipient.DsnParameters.ContainsKey(key);
		}

		public bool TryGetDsnParameters(EnvelopeRecipient recipient, string key, out object value)
		{
			value = null;
			if (this.IsInDeliveryAgent)
			{
				return this.DeliverMailItemEventSource.TryGetDsnParameters(recipient, key, out value);
			}
			MailRecipient mailRecipient = TransportAgentWrapper.CastEnvelopeRecipientToMailRecipient(recipient);
			return mailRecipient.DsnParameters != null && mailRecipient.DsnParameters.TryGetValue(key, out value);
		}

		public void CompleteAsyncEvent()
		{
			ExSmsCounters.PendingDelivery.Decrement();
			this.AgentAsyncContext.Complete();
		}

		public void Resume()
		{
			this.AgentAsyncContext.Resume();
		}

		public void SetCachedTextMessagingSettings(TextMessagingSettingsBase settings)
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			StringBuilder stringBuilder = new StringBuilder(settings.ToBase64String());
			int num = 0;
			int num2 = 1024 - "X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-".Length - 1;
			if (0 >= num2)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("EffectiveMaxTextValueBytesPerValueLimits", num2.ToString()));
			}
			int num3 = 0;
			int num4 = Math.Min(num2, stringBuilder.Length - num3);
			while (stringBuilder.Length > num3)
			{
				this.SetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-" + num++.ToString(), stringBuilder.ToString(num3, num4));
				num3 += num4;
				num4 = Math.Min(num2, stringBuilder.Length - num3);
			}
			if (0 < num)
			{
				this.SetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Count-Of-Settings-Segments", num.ToString());
			}
		}

		public TextMessagingSettingsBase GetCachedTextMessagingSettings()
		{
			string s = null;
			if (!this.TryGetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Count-Of-Settings-Segments", out s))
			{
				return null;
			}
			int i = 0;
			if (!int.TryParse(s, out i))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(i * 1024);
			int num = 0;
			while (i > num)
			{
				string value = null;
				if (!this.TryGetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-" + num.ToString(), out value))
				{
					return null;
				}
				if (string.IsNullOrEmpty(value))
				{
					return null;
				}
				stringBuilder.Append(value);
				num++;
			}
			TextMessagingSettingsBase result;
			try
			{
				result = (TextMessagingSettingsBase)VersionedXmlBase.ParseFromBase64(stringBuilder.ToString());
			}
			catch (XmlException ex)
			{
				throw new MobileDriverStateException(Strings.ErrorCannotParseSettings(ex.ToString()));
			}
			catch (InvalidOperationException ex2)
			{
				throw new MobileDriverStateException(Strings.ErrorCannotParseSettings(ex2.ToString()));
			}
			return result;
		}

		public bool TryGetTextHeader(string name, out string text)
		{
			text = null;
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (this.RootPart == null)
			{
				return false;
			}
			Header header = this.RootPart.Headers.FindFirst(name);
			if (header == null)
			{
				return false;
			}
			text = header.Value;
			return true;
		}

		public void SetTextHeader(string name, string text)
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			if (this.RootPart == null)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("RootPart", Strings.ConstNull));
			}
			Header header = this.RootPart.Headers.FindFirst(name);
			if (header == null)
			{
				header = new TextHeader(name, text);
				this.RootPart.Headers.AppendChild(header);
				return;
			}
			header.Value = text;
		}

		public string GetMapiMessageClass()
		{
			string result = null;
			if (!this.TryGetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Mapi-Class", out result))
			{
				result = this.EmailMessage.MapiMessageClass;
			}
			return result;
		}

		public void SetOnceMapiMessageClassToMimeHeader()
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			string text = null;
			if (!this.TryGetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Mapi-Class", out text))
			{
				string mapiMessageClass = this.EmailMessage.MapiMessageClass;
				this.SetTextHeader("X-MS-Exchange-Organization-Text-Messaging-Mapi-Class", mapiMessageClass);
			}
		}

		public void ForkSubmission(IList<EnvelopeRecipient> recipients)
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			this.SubmittedMessageEventSource.Fork(recipients);
		}

		public void SetAgentAsyncContext(AgentAsyncContext asyncContext)
		{
			if (this.AgentAsyncContext != null)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("AgentAsyncContext", this.AgentAsyncContext.ToString()));
			}
			this.AgentAsyncContext = asyncContext;
		}

		public void SetDsnFormat(DsnFormat dsnFormat)
		{
			if (this.IsInDeliveryAgent)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("IsInDeliveryAgent", true.ToString()));
			}
			this.TransportMailItem.DsnFormat = dsnFormat;
		}

		private const int OneK = 1024;

		private MimePart rootPart;

		private EmailMessage emailMessage;

		private ReadOnlyEnvelopeRecipientCollection readOnlyRecipients;

		private TransportMailItem transportMailItem;

		private DsnParameters mailItemDsnParametersCopy;

		private ADSessionSettings adSessionSettings;
	}
}
