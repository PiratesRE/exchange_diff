using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal sealed class Agent : SmtpReceiveAgent
	{
		internal Agent(SmtpServer server)
		{
			Agent.ReadConfigFile();
			base.OnEndOfData += this.FilterMessage;
		}

		private static bool TryReadBoolean(string key, bool defaultValue)
		{
			bool result = defaultValue;
			try
			{
				string text = ConfigurationManager.AppSettings[key];
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string, string>(0L, "{0} key value configured in transport config is {1}", key, text);
				if (!bool.TryParse(text, out result))
				{
					result = defaultValue;
				}
			}
			catch (ConfigurationErrorsException arg)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceError<string, ConfigurationErrorsException>(0L, "Not able to load the {0} value from the transport config file. Exception raised {1}", key, arg);
				result = defaultValue;
			}
			return result;
		}

		private static void ReadConfigFile()
		{
			Agent.allowInvalidAttachment = Agent.TryReadBoolean("AllowInvalidAttachment", Agent.allowInvalidAttachment);
			Agent.allowInvalidAttachment = Agent.TryReadBoolean("AllowInvalidZipInAttachmentFilterAgent", Agent.allowInvalidAttachment);
			Agent.ignoreEmbeddedMessageNames = Agent.TryReadBoolean("IgnoreEmbeddedMessageNamesInAttachmentFilterAgent", Agent.ignoreEmbeddedMessageNames);
			Agent.skipDigitalSignedMessageFromAttachmentFilter = Agent.TryReadBoolean("SkipDigitalSignedMessageFromAttachmentFilterAgent", Agent.skipDigitalSignedMessageFromAttachmentFilter);
		}

		internal static ExEventLog Logger
		{
			get
			{
				return Agent.logger;
			}
		}

		public void FilterMessage(ReceiveMessageEventSource source, EndOfDataEventArgs args)
		{
			MailItem mailItem = args.MailItem;
			this.currentConfig = Configuration.Current;
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			string text = transportMailItem.ReceiveConnectorName;
			if (text != null)
			{
				int num = text.IndexOf(':');
				if (num != -1)
				{
					text = text.Substring(num + 1);
					if (text != null && !this.currentConfig.IsEnabled(text))
					{
						ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Skipping attachment filtering for Message-ID: {0}. ExceptionConnector : {1} ", mailItem.Message.MessageId, text);
						return;
					}
				}
			}
			if (ExTraceGlobals.AttachmentFilteringTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Attachment filtering processing message with Message-ID: {0}", mailItem.Message.MessageId);
			}
			Agent.CheckStatus checkStatus = Agent.CheckStatus.Ok;
			try
			{
				checkStatus = this.CheckMessage(mailItem.Message, 0, false);
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceError<string>((long)this.GetHashCode(), "Message exceeded complexity, rejecting. Exception: {0}", ex.ToString());
				checkStatus = Agent.CheckStatus.MustReject;
			}
			if (checkStatus == Agent.CheckStatus.Ok && !this.attachmentsWereRemoved)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "No action taken on message by attachment filtering agent.");
				return;
			}
			PerfCounters.MsgsFiltered.Increment();
			FilterActions enumValue = this.currentConfig.FilterAction;
			if (checkStatus == Agent.CheckStatus.MustReject)
			{
				enumValue = FilterActions.Reject;
			}
			else if (this.attachmentsWereRemoved)
			{
				enumValue = FilterActions.Strip;
			}
			ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Message failed attachment filtering agent. The action being applied is '{0}'.", Enum<FilterActions>.ToString((int)enumValue));
			switch (enumValue)
			{
			case FilterActions.Reject:
				source.RejectMessage(this.currentConfig.RejectResponse);
				return;
			case FilterActions.Strip:
				this.SetAttachmentsRemovedOutlookProperty(mailItem.Message);
				return;
			case FilterActions.SilentDelete:
				source.RejectMessage(this.currentConfig.GetSilentDeleteResponse(mailItem.Message.MessageId));
				return;
			default:
				return;
			}
		}

		private Agent.CheckStatus CheckMessage(EmailMessage message, int recursionDepth, bool parentSigned)
		{
			if (++recursionDepth > 10)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<int>((long)this.GetHashCode(), "Maximum attachment nesting depth ({0}) exceeded, rejecting message", recursionDepth);
				return Agent.CheckStatus.MustReject;
			}
			AttachmentCollection attachments = message.Attachments;
			int num = 0;
			Attachment[] array = new Attachment[attachments.Count];
			bool flag = parentSigned || message.MessageSecurityType == MessageSecurityType.ClearSigned;
			foreach (Attachment attachment in attachments)
			{
				if (!flag || !Agent.skipDigitalSignedMessageFromAttachmentFilter)
				{
					attachment.FileName = attachment.FileName;
				}
				Agent.CheckStatus checkStatus = this.CheckAttachment(attachment, recursionDepth, flag);
				if (checkStatus != Agent.CheckStatus.Ok)
				{
					if (this.currentConfig.FilterAction == FilterActions.Reject || this.currentConfig.FilterAction == FilterActions.SilentDelete || checkStatus == Agent.CheckStatus.MustReject)
					{
						return checkStatus;
					}
					array[num++] = attachment;
				}
			}
			for (int i = 0; i < num; i++)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Replacing attachment: {0}", array[i].FileName);
				array[i].ContentType = "text/plain";
				array[i].FileName = this.ReplacementAttachmentName(array[i].FileName);
				Stream contentWriteStream = array[i].GetContentWriteStream();
				Encoding encoding = this.currentConfig.IsAdminMessageUsAscii ? Encoding.ASCII : Encoding.Unicode;
				using (StreamWriter streamWriter = new StreamWriter(contentWriteStream, encoding))
				{
					streamWriter.Write(this.currentConfig.AdminMessage);
					streamWriter.Flush();
				}
				this.attachmentsWereRemoved = true;
			}
			return Agent.CheckStatus.Ok;
		}

		private Agent.CheckStatus CheckAttachment(Attachment attachment, int recursionDepth, bool parentSigned)
		{
			AttachmentInfo attachmentInfo = AttachmentInfo.BuildInfo(attachment);
			if (attachmentInfo == null)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Blocking because the message is corrupt and cannot be parsed or verified that it does not violate any safety constraints");
				return Agent.CheckStatus.MustReject;
			}
			if (this.currentConfig.IsBannedType(attachmentInfo.ContentTypes))
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Blocking because of content-type restriction");
				return Agent.CheckStatus.Bad;
			}
			if ((!Agent.ignoreEmbeddedMessageNames || null == attachment.EmbeddedMessage) && this.currentConfig.IsBannedName(attachmentInfo.Name))
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Blocking because of attachment name restriction");
				return Agent.CheckStatus.Bad;
			}
			Agent.CheckStatus checkStatus = Agent.CheckStatus.Ok;
			try
			{
				checkStatus = this.CheckContainer(attachmentInfo);
			}
			catch (ExchangeDataException ex)
			{
				if (this.currentConfig.FilterAction != FilterActions.Strip)
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Blocking because there was a problem parsing the container. Message will be rejected. Exception: {0}", ex.ToString());
					return Agent.CheckStatus.MustReject;
				}
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Blocking because there was a problem parsing the container. Attachment will be stripped. Exception: {0}", ex.ToString());
				return Agent.CheckStatus.Bad;
			}
			if (checkStatus != Agent.CheckStatus.Ok)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "Attachment was a container which had banned files within it, status: {0}", Enum<Agent.CheckStatus>.ToString((int)checkStatus));
				return checkStatus;
			}
			if (attachment.EmbeddedMessage != null)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Attachment is embedded message, recursively checking");
				return this.CheckMessage(attachment.EmbeddedMessage, recursionDepth, parentSigned);
			}
			ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)this.GetHashCode(), "Attachment is OK");
			return Agent.CheckStatus.Ok;
		}

		private Agent.CheckStatus CheckContainer(AttachmentInfo attachmentInfo)
		{
			IEnumerable<string> enumerable = null;
			if (!ContainerReaderFactory.Create(attachmentInfo, out enumerable))
			{
				return Agent.CheckStatus.MustReject;
			}
			if (enumerable != null)
			{
				try
				{
					foreach (string text in enumerable)
					{
						if (this.currentConfig.IsBannedName(text))
						{
							ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)this.GetHashCode(), "The message contains an illegal file {0} within a container file.", text);
							return Agent.CheckStatus.Bad;
						}
					}
				}
				catch (ExchangeDataException arg)
				{
					if (!Agent.allowInvalidAttachment)
					{
						throw;
					}
					ExTraceGlobals.AttachmentFilteringTracer.TraceError<string, ExchangeDataException>((long)this.GetHashCode(), "The message contains an invalid attachment {0}. Exception Raised: {1}", attachmentInfo.Name, arg);
				}
				return Agent.CheckStatus.Ok;
			}
			return Agent.CheckStatus.Ok;
		}

		private void SetAttachmentsRemovedOutlookProperty(EmailMessage emailMessage)
		{
			TextHeader newChild = new TextHeader("X-MS-Exchange-Organization-Classification", "a4bb0cb2-4395-4d18-9799-1f904b20fe92");
			emailMessage.RootPart.Headers.AppendChild(newChild);
		}

		private string ReplacementAttachmentName(string originalAttachmentName)
		{
			if (originalAttachmentName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
			{
				return originalAttachmentName;
			}
			return originalAttachmentName + ".txt";
		}

		private const string AllowInvalidAttachmentKey = "AllowInvalidAttachment";

		private const string AllowInvalidZipKey = "AllowInvalidZipInAttachmentFilterAgent";

		private const string IgnoreEmbeddedMessageNamesKey = "IgnoreEmbeddedMessageNamesInAttachmentFilterAgent";

		private const string SkipDigitalSignedMessageFromAttachmentFilterKey = "SkipDigitalSignedMessageFromAttachmentFilterAgent";

		private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

		private static bool allowInvalidAttachment = false;

		private static bool ignoreEmbeddedMessageNames = false;

		private static bool skipDigitalSignedMessageFromAttachmentFilter = false;

		private Configuration currentConfig;

		private bool attachmentsWereRemoved;

		private enum CheckStatus
		{
			Ok,
			Bad,
			MustReject
		}
	}
}
