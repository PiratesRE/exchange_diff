using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RightsManagement;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DsnGenerator : IDsnGeneratorComponent, ITransportComponent
	{
		public DsnGenerator()
		{
			this.eventLogger = new ExEventLog(ExTraceGlobals.DSNTracer.Category, TransportEventLog.GetEventSource());
		}

		public DsnMailOutHandlerDelegate DsnMailOutHandler
		{
			get
			{
				if (this.dsnMailOutHandler == null)
				{
					this.dsnMailOutHandler = new DsnMailOutHandlerDelegate(this.SendToCategorizer);
				}
				return this.dsnMailOutHandler;
			}
			set
			{
				this.dsnMailOutHandler = value;
			}
		}

		public DsnHumanReadableWriter DsnHumanReadableWriter
		{
			get
			{
				return this.internalHumanReadableWriter;
			}
		}

		public QuarantineConfig QuarantineConfig
		{
			get
			{
				return this.quarantineConfig;
			}
		}

		public static DsnRecipientInfo CreateDsnRecipientInfo(string displayName, string address, string addressType, SmtpResponse smtpResponse)
		{
			return new DsnRecipientInfo(displayName, address, addressType, smtpResponse.EnhancedStatusCode, smtpResponse.ToString(), null, null, smtpResponse.DsnExplanation, null, null);
		}

		public static RoutingAddress GetDefaultExternalPostmasterAddress(OrganizationId organizationId)
		{
			string text = null;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(organizationId, out perTenantAcceptedDomainTable))
			{
				text = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomainName;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Components.Configuration.LocalServer.TransportServer.GetDomainOrComputerName();
			}
			return new RoutingAddress("postmaster", text);
		}

		public void Load()
		{
			this.Configure();
			this.RegisterConfigurationChangeHandlers();
			DsnGeneratorPerfCounters.SetCategoryName(DsnGenerator.perfCounterCategoryMap[Components.Configuration.ProcessTransportRole]);
			DsnGenerator.internalPerfCounters = DsnGeneratorPerfCounters.GetInstance("Internal");
			DsnGenerator.externalPerfCounters = DsnGeneratorPerfCounters.GetInstance("External");
			this.configReloadTimer = new GuardedTimer(new TimerCallback(this.TimedConfigUpdate), null, Components.TransportAppConfig.ADPolling.DsnMessageCustomizationReloadInterval, Components.TransportAppConfig.ADPolling.DsnMessageCustomizationReloadInterval);
		}

		public void Unload()
		{
			this.configReloadTimer.Dispose(false);
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem)
		{
			this.GenerateDSNs(mailItem, mailItem.Recipients, null, DsnGenerator.CallerComponent.Other, null);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, DsnGenerator.CallerComponent callerComponent)
		{
			this.GenerateDSNs(mailItem, mailItem.Recipients, null, callerComponent, null);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, string remoteServer, DsnGenerator.CallerComponent callerComponent)
		{
			this.GenerateDSNs(mailItem, mailItem.Recipients, remoteServer, callerComponent, ((RoutedMailItem)mailItem).LastQueueLevelError);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList)
		{
			this.GenerateDSNs(mailItem, recipientList, null, DsnGenerator.CallerComponent.Other, null);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, string remoteServer)
		{
			this.GenerateDSNs(mailItem, recipientList, remoteServer, DsnGenerator.CallerComponent.Other, null);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, LastError lastQueueLevelError)
		{
			this.GenerateDSNs(mailItem, recipientList, null, DsnGenerator.CallerComponent.Other, lastQueueLevelError);
		}

		public void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, string remoteServer, DsnGenerator.CallerComponent callerComponent, LastError lastQueueLevelError)
		{
			if (!mailItem.IsActive)
			{
				return;
			}
			LatencyTracker.BeginTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
			if (callerComponent == DsnGenerator.CallerComponent.ResolverRouting)
			{
				DsnGenerator.DeliveryFailureCounterWrapper.Instance.CheckCategorizerRecipient(recipientList);
			}
			else if (callerComponent == DsnGenerator.CallerComponent.Delivery)
			{
				DsnGenerator.DeliveryFailureCounterWrapper.Instance.CheckDeliveryRecipient(recipientList);
			}
			PoisonContext context = PoisonMessage.Context;
			PoisonMessage.Context = new MessageContext(mailItem.RecordId, mailItem.InternetMessageId, MessageProcessingSource.DsnGenerator);
			DsnFlags neededDsns = DsnGenerator.GetNeededDsns(mailItem, recipientList);
			if (neededDsns == DsnFlags.None)
			{
				ExTraceGlobals.DSNTracer.TraceDebug(0L, "No recipients need a DSN. Nothing to do");
				PoisonMessage.Context = context;
				LatencyTracker.EndTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
				return;
			}
			if (this.InternalGenerateDSNs(mailItem, recipientList, neededDsns, remoteServer, lastQueueLevelError))
			{
				DsnGenerator.MarkUpHandledRecipients(recipientList, neededDsns);
			}
			PoisonMessage.Context = context;
		}

		public void GenerateNDRForInvalidAddresses(bool switchPoisonContext, IReadOnlyMailItem mailItem, List<DsnRecipientInfo> recipientList)
		{
			this.GenerateNDRForInvalidAddresses(switchPoisonContext, mailItem, recipientList, null);
		}

		public void GenerateNDRForInvalidAddresses(bool switchPoisonContext, IReadOnlyMailItem mailItem, List<DsnRecipientInfo> recipientList, DsnMailOutHandlerDelegate dsnMailOutHandler)
		{
			LatencyTracker.BeginTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
			this.DsnMailOutHandler = dsnMailOutHandler;
			PoisonContext context = null;
			if (switchPoisonContext)
			{
				context = PoisonMessage.Context;
				PoisonMessage.Context = new MessageContext(mailItem.RecordId, mailItem.InternetMessageId, MessageProcessingSource.DsnGenerator);
			}
			if (mailItem.From == RoutingAddress.NullReversePath)
			{
				ExTraceGlobals.DSNTracer.TraceError(0L, "mailItem has <> as P1 from. Consider it badmail");
				MessageTrackingLog.TrackBadmail(MessageTrackingSource.DSN, null, mailItem, "mailItem has <> as P1 from. Consider it badmail");
				LatencyTracker.EndTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
			}
			else
			{
				this.InternalGenerateDSNs(mailItem, recipientList, DsnFlags.Failure, null, null);
			}
			if (switchPoisonContext)
			{
				PoisonMessage.Context = context;
			}
		}

		public void DecorateStoreNdr(TransportMailItem transportMessage, RoutingAddress ndrRecipient)
		{
			Header header = Header.Create("X-MS-Exchange-Organization-Dsn-Version");
			header.Value = "12";
			transportMessage.RootPart.Headers.AppendChild(header);
			Header header2 = Header.Create("X-MS-Exchange-Organization-SCL");
			header2.Value = "-1";
			transportMessage.RootPart.Headers.AppendChild(header2);
			transportMessage.RootPart.Headers.RemoveAll(HeaderId.To);
			transportMessage.RootPart.Headers.RemoveAll(HeaderId.Sender);
			string smtpAddress = (string)DsnGenerator.GetInternalPostmasterAddress(transportMessage.OrganizationId, transportMessage.TransportSettings.ExternalPostmasterAddress);
			transportMessage.Message.From = new EmailRecipient(null, smtpAddress);
			transportMessage.Message.To.Add(new EmailRecipient(null, (string)ndrRecipient));
		}

		public void MonitorJobs()
		{
			if (DsnGenerator.internalPerfCounters != null && DsnGenerator.externalPerfCounters != null)
			{
				DsnGenerator.DeliveryFailureCounterWrapper.Instance.UpdateDsnSlidingCounters();
			}
		}

		public void DecorateMfn(TransportMailItem transportMessage, string displayName, string emailAddress)
		{
			transportMessage.Message.From = new EmailRecipient(displayName, emailAddress);
			string smtpAddress = (string)DsnGenerator.GetInternalPostmasterAddress(transportMessage.OrganizationId, transportMessage.TransportSettings.ExternalPostmasterAddress);
			transportMessage.Message.Sender = new EmailRecipient(null, smtpAddress);
		}

		public void CreateStoreQuotaMessageBody(TransportMailItem transportMailItem, QuotaType quotaType, string quotaMessageClass, int? localeId, int currentSize, int? maxSize, string folderName, bool isPrimaryMailbox, bool isPublicFolderMailbox)
		{
			transportMailItem.From = RoutingAddress.NullReversePath;
			QuotaInformation quotaInformation = this.internalHumanReadableWriter.GetQuotaInformation(quotaType, localeId, currentSize, maxSize, folderName, isPrimaryMailbox, isPublicFolderMailbox);
			EncodingOptions encodingOptions = new EncodingOptions(quotaInformation.MessageCharset.Name, quotaInformation.Culture.Name, Microsoft.Exchange.Data.Mime.EncodingFlags.None);
			using (Stream stream = transportMailItem.OpenMimeWriteStream())
			{
				using (MimeWriter mimeWriter = new MimeWriter(stream, true, encodingOptions))
				{
					ExTraceGlobals.DSNTracer.TraceDebug<int>(0L, "Created mimeWriter with codepage: {0}", quotaInformation.MessageCharset.CodePage);
					mimeWriter.StartPart();
					this.AddQuotaRFC822Headers(transportMailItem, mimeWriter, DsnGenerator.MapQuotaMessageClassToQuotaClass(quotaMessageClass), quotaInformation);
					mimeWriter.StartPart();
					mimeWriter.StartHeader(HeaderId.ContentType);
					mimeWriter.WriteHeaderValue("text/plain;");
					mimeWriter.WriteParameter("charset", quotaInformation.MessageCharset.Name);
					mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
					using (EncoderStream encoderStream = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
					{
						this.internalHumanReadableWriter.WriteTextQuotaBody(encoderStream, quotaInformation);
						encoderStream.Flush();
					}
					mimeWriter.EndPart();
					mimeWriter.StartPart();
					mimeWriter.StartHeader(HeaderId.ContentType);
					mimeWriter.WriteHeaderValue("text/html;");
					mimeWriter.WriteParameter("charset", quotaInformation.MessageCharset.Name);
					mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
					using (EncoderStream encoderStream2 = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
					{
						this.internalHumanReadableWriter.WriteHtmlQuotaBody(encoderStream2, quotaInformation);
						encoderStream2.Flush();
					}
					mimeWriter.EndPart();
					mimeWriter.EndPart();
				}
			}
		}

		internal static string GetNextThreadIndex(byte[] currentThreadIndex)
		{
			int num;
			byte[] base64DecodedBytes = DsnGenerator.GetBase64DecodedBytes(currentThreadIndex, out num);
			if (base64DecodedBytes[0] != 1 || num < 22 || (num - 22) % 5 != 0)
			{
				ExTraceGlobals.DSNTracer.TraceError<int, byte[]>(0L, "Invalid thread index, length is {0}, bytes:[{1}]", num, base64DecodedBytes);
				return null;
			}
			if (base64DecodedBytes.Length <= num + 5)
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Unexpected decoded length: ",
					num,
					" for target array length: ",
					base64DecodedBytes.Length
				}));
			}
			uint num2 = DsnGenerator.ComputeNextThreadIndexDelta(base64DecodedBytes, num);
			base64DecodedBytes[num++] = (byte)((num2 & 4278190080U) >> 24);
			base64DecodedBytes[num++] = (byte)((num2 & 16711680U) >> 16);
			base64DecodedBytes[num++] = (byte)((num2 & 65280U) >> 8);
			base64DecodedBytes[num++] = (byte)(num2 & 255U);
			base64DecodedBytes[num++] = (byte)(Environment.TickCount & 255);
			if ((num - 22) % 5 != 0)
			{
				throw new InvalidOperationException("Unexpected decoded length: " + num);
			}
			return DsnGenerator.Base64EncodeBytes(base64DecodedBytes, num);
		}

		private static string MapQuotaMessageClassToQuotaClass(string messageClass)
		{
			if (messageClass.Equals("IPM.Note.StorageQuotaWarning.SendReceive", StringComparison.OrdinalIgnoreCase))
			{
				return "SendReceive";
			}
			if (messageClass.Equals("IPM.Note.StorageQuotaWarning.Send", StringComparison.OrdinalIgnoreCase))
			{
				return "Send";
			}
			return "Warning";
		}

		private static RoutingAddress GetInternalPostmasterAddress(OrganizationId organizationId, SmtpAddress? configuredExternalPostmasterAddress)
		{
			MicrosoftExchangeRecipientPerTenantSettings microsoftExchangeRecipientPerTenantSettings;
			if (Components.Configuration.TryGetMicrosoftExchangeRecipient(organizationId, out microsoftExchangeRecipientPerTenantSettings) && !microsoftExchangeRecipientPerTenantSettings.UsingDefault && !microsoftExchangeRecipientPerTenantSettings.PrimarySmtpAddress.Equals(SmtpAddress.NullReversePath))
			{
				return (RoutingAddress)microsoftExchangeRecipientPerTenantSettings.PrimarySmtpAddress.ToString();
			}
			return DsnGenerator.GetExternalPostmasterAddress(organizationId, configuredExternalPostmasterAddress);
		}

		private static RoutingAddress GetExternalPostmasterAddress(OrganizationId organizationId, SmtpAddress? configuredPostmasterAddress)
		{
			if (configuredPostmasterAddress != null && configuredPostmasterAddress.Value.IsValidAddress && configuredPostmasterAddress.Value != SmtpAddress.NullReversePath)
			{
				return (RoutingAddress)configuredPostmasterAddress.Value.ToString();
			}
			return DsnGenerator.GetDefaultExternalPostmasterAddress(organizationId);
		}

		private static DsnGenerator.DsnConfig GetOrgWideDsnConfig(bool internalConfig, OrganizationId organizationId)
		{
			TransportConfigContainer transportSettings = Components.Configuration.TransportSettings.TransportSettings;
			if (internalConfig)
			{
				string reportingServer = DsnGenerator.GetReportingServer(transportSettings.InternalDsnReportingAuthority, organizationId, internalConfig);
				return new DsnGenerator.DsnConfig(internalConfig, transportSettings.InternalDelayDsnEnabled, transportSettings.InternalDsnMaxMessageAttachSize, reportingServer, transportSettings.InternalDsnSendHtml, transportSettings.InternalDsnDefaultLanguage, transportSettings.InternalDsnLanguageDetectionEnabled, DsnGenerator.GetInternalPostmasterAddress(organizationId, null), null, organizationId);
			}
			string reportingServer2 = DsnGenerator.GetReportingServer(transportSettings.ExternalDsnReportingAuthority, organizationId, internalConfig);
			return new DsnGenerator.DsnConfig(internalConfig, transportSettings.ExternalDelayDsnEnabled, transportSettings.ExternalDsnMaxMessageAttachSize, reportingServer2, transportSettings.ExternalDsnSendHtml, transportSettings.ExternalDsnDefaultLanguage, transportSettings.ExternalDsnLanguageDetectionEnabled, DsnGenerator.GetDefaultExternalPostmasterAddress(organizationId), null, organizationId);
		}

		private static string GetReportingServer(SmtpDomain reportingSmtpDomain, OrganizationId organizationId, bool internalConfig)
		{
			string text = null;
			if (!MultiTenantTransport.MultiTenancyEnabled && reportingSmtpDomain != null)
			{
				text = reportingSmtpDomain.ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				if (internalConfig && organizationId == OrganizationId.ForestWideOrgId)
				{
					text = Components.Configuration.LocalServer.TransportServer.Fqdn;
					text = ((!string.IsNullOrEmpty(text)) ? text : ComputerInformation.DnsFullyQualifiedDomainName);
				}
				else
				{
					text = (Components.TransportAppConfig.DeliveryFailureConfiguration.DSNServerConnectorFqdn ?? ComputerInformation.DnsFullyQualifiedDomainName);
				}
			}
			return text;
		}

		private static DsnFlags GetNeededDsns(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList)
		{
			DsnFlags dsnFlags = DsnFlags.None;
			if (mailItem.From == RoutingAddress.NullReversePath)
			{
				bool flag = false;
				ExTraceGlobals.DSNTracer.TraceDebug(0L, "mailItem is a DSN. Marking all recipients as complete");
				foreach (MailRecipient mailRecipient in recipientList)
				{
					if (DsnGenerator.IsDsnNeededForThisRecipient(mailRecipient, DsnFlags.AllFlags))
					{
						if (mailRecipient.Status == Status.Handled)
						{
							if (DsnFlags.Failure == mailRecipient.DsnNeeded)
							{
								flag = true;
							}
							mailRecipient.Status = Status.Complete;
						}
						mailRecipient.DsnCompleted |= mailRecipient.DsnNeeded;
						mailRecipient.DsnNeeded = DsnFlags.None;
					}
				}
				if (flag)
				{
					MessageTrackingLog.TrackBadmail(MessageTrackingSource.DSN, null, mailItem, "NDRing a mail recepient that requires a DSN");
				}
				return dsnFlags;
			}
			ExTraceGlobals.DSNTracer.TraceDebug(0L, "Recipients Status to Get Needed DSNs:");
			foreach (MailRecipient mailRecipient2 in recipientList)
			{
				if (mailRecipient2.IsActive)
				{
					dsnFlags |= mailRecipient2.DsnNeeded;
					DsnGenerator.TraceRecipientStatus(mailRecipient2);
				}
			}
			return dsnFlags;
		}

		private static void PatchQuarantineRecipientsIfNeeded(ref DsnFlags dsnFlags, DsnGenerator.DsnConfig dsnConfig, QuarantineConfig quarantineConfig, IEnumerable recipientList)
		{
			if ((dsnFlags & DsnFlags.Quarantine) != DsnFlags.None && (quarantineConfig == null || string.IsNullOrEmpty(quarantineConfig.Mailbox)))
			{
				ExTraceGlobals.DSNTracer.TraceError(0L, "There was an attempt to quarantine recipients while the quarantine configuration was corrupt, or AD unreachable, or when no quarantine mailbox was configured. The recipients will be NDR'ed instead of quarantined");
				foreach (object obj in recipientList)
				{
					MailRecipient mailRecipient = obj as MailRecipient;
					if (mailRecipient != null && mailRecipient.IsActive && mailRecipient.AckStatus == AckStatus.Quarantine)
					{
						ExTraceGlobals.DSNTracer.TraceError<string>(0L, "NDR'ing quarantined recipient: {0}", mailRecipient.Email.ToString());
						mailRecipient.Ack(AckStatus.Fail, AckReason.QuarantineDisabled);
					}
				}
				dsnFlags &= ~DsnFlags.Quarantine;
				dsnFlags |= DsnFlags.Failure;
			}
		}

		private static string DeencapsulateAddress(string address)
		{
			if (SmtpProxyAddress.IsEncapsulatedAddress(address))
			{
				try
				{
					SmtpProxyAddress smtpProxyAddress = new SmtpProxyAddress(address, false);
					ProxyAddress proxyAddress;
					if (smtpProxyAddress.TryDeencapsulate(out proxyAddress))
					{
						if (proxyAddress.Prefix != ProxyAddressPrefix.LegacyDN)
						{
							return proxyAddress.ProxyAddressString;
						}
						return null;
					}
				}
				catch (ArgumentOutOfRangeException arg)
				{
					ExTraceGlobals.DSNTracer.TraceDebug<string, ArgumentOutOfRangeException>(0L, "The encapsulated address could not be used to create SmtpProxyAddress: {0}, {1}", address, arg);
					return null;
				}
				return address;
			}
			return address;
		}

		private static void FillDisplayNameFromRecipientCollection(EmailRecipientCollection recipCollection, Dictionary<string, DsnRecipientInfo> displayNameLookUpList, OutboundCodePageDetector codepageDetector, ref int recipientsRemaining)
		{
			if (recipientsRemaining <= 0)
			{
				return;
			}
			for (int i = 0; i < recipCollection.Count; i++)
			{
				EmailRecipient emailRecipient = recipCollection[i];
				string smtpAddress = emailRecipient.SmtpAddress;
				DsnRecipientInfo dsnRecipientInfo;
				if (displayNameLookUpList.TryGetValue(smtpAddress, out dsnRecipientInfo))
				{
					string text = null;
					try
					{
						text = emailRecipient.DisplayName;
					}
					catch (ExchangeDataException arg)
					{
						ExTraceGlobals.DSNTracer.TraceDebug<ExchangeDataException>(0L, "Unsupported encoding for display name {0}", arg);
					}
					if (!string.IsNullOrEmpty(text))
					{
						dsnRecipientInfo.DisplayName = text;
						codepageDetector.AddText(text);
						recipientsRemaining--;
						ExTraceGlobals.DSNTracer.TraceDebug<string, string, int>(0L, "Find displayName [{0}] for <{1}> and {2} recipients remain", text, smtpAddress, recipientsRemaining);
						if (recipientsRemaining == 0)
						{
							return;
						}
					}
				}
			}
		}

		private static void IncrementPerfCounter(bool toInternal, DsnFlags dsnFlag, List<DsnRecipientInfo> dsnRecipientList)
		{
			DsnGeneratorPerfCountersInstance dsnGeneratorPerfCountersInstance = toInternal ? DsnGenerator.internalPerfCounters : DsnGenerator.externalPerfCounters;
			if (dsnGeneratorPerfCountersInstance == null)
			{
				return;
			}
			DsnGenerator.DeliveryFailureCounterWrapper.Instance.IncreaseDsnSlidingCounters(dsnGeneratorPerfCountersInstance, toInternal, dsnFlag, dsnRecipientList);
			switch (dsnFlag)
			{
			case DsnFlags.Delivery:
				dsnGeneratorPerfCountersInstance.DeliveredDsnTotal.Increment();
				return;
			case DsnFlags.Delay:
				dsnGeneratorPerfCountersInstance.DelayedDsnTotal.Increment();
				return;
			case DsnFlags.Delivery | DsnFlags.Delay:
				break;
			case DsnFlags.Failure:
				dsnGeneratorPerfCountersInstance.FailedDsnTotal.Increment();
				return;
			default:
				if (dsnFlag == DsnFlags.Relay)
				{
					dsnGeneratorPerfCountersInstance.RelayedDsnTotal.Increment();
					return;
				}
				if (dsnFlag != DsnFlags.Expansion)
				{
					return;
				}
				dsnGeneratorPerfCountersInstance.ExpandedDsnTotal.Increment();
				break;
			}
		}

		private static void TraceRecipientStatus(MailRecipient recipient)
		{
			ExTraceGlobals.DSNTracer.TraceDebug(0L, "\r\n Recipient: {0}\r\n Status {1}\r\n StatusText: {2}\r\n DsnRequested: {3}\r\n DsnNeeded: {4}\r\n DsnCompleted: {5}", new object[]
			{
				recipient.Email,
				recipient.Status,
				recipient.SmtpResponse,
				recipient.DsnRequested,
				recipient.DsnNeeded,
				recipient.DsnCompleted
			});
		}

		private static void AddInterceptorHeaders(MimeWriter mimeWriter, IReadOnlyMailItem originalMailItem)
		{
			try
			{
				HeaderList headers = originalMailItem.MimeDocument.RootPart.Headers;
				Header[] array = headers.FindAll("X-MS-Exchange-Organization-Matched-Interceptor-Rule");
				foreach (Header header in array)
				{
					mimeWriter.WriteHeader("X-MS-Exchange-Organization-Matched-Interceptor-Rule", header.Value);
				}
			}
			catch (ExchangeDataException arg)
			{
				ExTraceGlobals.DSNTracer.TraceError<ExchangeDataException>(0L, "Cannot write all headers for the DSN, got {0}", arg);
			}
		}

		private static void AddRFC822Headers(DsnGenerator.DsnConfig dsnConfig, IReadOnlyMailItem mailItem, DateTime dateTimeGenerated, DsnGenerator.DsnAction dsnAction, MimeWriter mimeWriter, CultureInfo cultureInfo, string messageId)
		{
			try
			{
				HeaderList headers = mailItem.RootPart.Headers;
				mimeWriter.WriteHeader(HeaderId.MimeVersion, "1.0");
				if (dsnConfig.Internal)
				{
					mimeWriter.StartHeader(HeaderId.From);
					mimeWriter.WriteRecipient(ADMicrosoftExchangeRecipient.DefaultDisplayName, dsnConfig.PostmasterAddress.ToString());
				}
				else
				{
					mimeWriter.WriteHeader(HeaderId.From, dsnConfig.PostmasterAddress.ToString());
				}
				mimeWriter.StartHeader(HeaderId.To);
				mimeWriter.WriteRecipient(null, mailItem.From.ToString());
				mimeWriter.StartHeader(HeaderId.Date);
				mimeWriter.WriteHeaderValue(dateTimeGenerated);
				mimeWriter.StartHeader(HeaderId.ContentType);
				mimeWriter.WriteHeaderValue("multipart/report");
				mimeWriter.WriteParameter("report-type", "delivery-status");
				mimeWriter.WriteParameter("boundary", Guid.NewGuid().ToString());
				mimeWriter.WriteHeader("X-MS-Exchange-Organization-OriginalArrivalTime", Util.FormatOrganizationalMessageArrivalTime(DateTime.UtcNow));
				mimeWriter.WriteHeader("X-MS-Exchange-Organization-SCL", "-1");
				if ((dsnAction.Flags & DsnFlags.Quarantine) != DsnFlags.None)
				{
					mimeWriter.WriteHeader("X-MS-Exchange-Organization-Quarantine", "true");
					if (mailItem.Scl != -2)
					{
						mimeWriter.WriteHeader("X-MS-Exchange-Organization-Original-SCL", mailItem.Scl.ToString(NumberFormatInfo.InvariantInfo));
					}
				}
				if ((dsnAction.Flags & DsnFlags.Failure) != DsnFlags.None)
				{
					Header header = headers.FindFirst("X-MS-Exchange-Organization-Journaled-To-Recipients");
					if (header != null)
					{
						header.WriteTo(mimeWriter);
					}
					mimeWriter.WriteHeader("X-MS-Exchange-Message-Is-Ndr", string.Empty);
				}
				string internetMessageId = mailItem.InternetMessageId;
				mimeWriter.WriteHeader("X-MS-Exchange-Organization-Dsn-Version", "12");
				mimeWriter.WriteHeader(HeaderId.ContentLanguage, cultureInfo.Name);
				if ((dsnAction.Flags & DsnFlags.Quarantine) != DsnFlags.None && !string.IsNullOrEmpty(internetMessageId))
				{
					mimeWriter.WriteHeader(HeaderId.MessageId, internetMessageId);
				}
				else
				{
					mimeWriter.WriteHeader(HeaderId.MessageId, messageId);
				}
				if (!string.IsNullOrEmpty(internetMessageId))
				{
					mimeWriter.WriteHeader(HeaderId.InReplyTo, internetMessageId);
				}
				DsnGenerator.WriteReferencesHeader(headers, internetMessageId, mimeWriter);
				Header header2 = headers.FindFirst("Thread-Topic");
				string value;
				if (header2 != null && header2.TryGetValue(out value))
				{
					mimeWriter.WriteHeader("Thread-Topic", value);
				}
				Header header3 = headers.FindFirst("Thread-Index");
				if (header3 != null)
				{
					string nextThreadIndex = DsnGenerator.GetNextThreadIndex(MimeInternalHelpers.GetHeaderRawValue(header3));
					if (!string.IsNullOrEmpty(nextThreadIndex))
					{
						mimeWriter.WriteHeader("Thread-Index", nextThreadIndex);
					}
				}
				string subject = mailItem.Subject;
				if (string.IsNullOrEmpty(subject))
				{
					mimeWriter.WriteHeader(HeaderId.Subject, dsnAction.Subject.ToString(cultureInfo));
				}
				else
				{
					mimeWriter.WriteHeader(HeaderId.Subject, dsnAction.Subject.ToString(cultureInfo) + subject);
				}
			}
			catch (ExchangeDataException arg)
			{
				ExTraceGlobals.DSNTracer.TraceError<ExchangeDataException>(0L, "Cannot write all headers for the DSN, got {0}", arg);
			}
		}

		private static void WriteReferencesHeader(HeaderList originalHeaders, string originalMessageId, MimeWriter mimeWriter)
		{
			string value = null;
			AsciiTextHeader asciiTextHeader = originalHeaders.FindFirst(HeaderId.References) as AsciiTextHeader;
			if (asciiTextHeader == null)
			{
				AsciiTextHeader asciiTextHeader2 = originalHeaders.FindFirst(HeaderId.InReplyTo) as AsciiTextHeader;
				if (asciiTextHeader2 != null && !string.IsNullOrEmpty(originalMessageId))
				{
					value = asciiTextHeader2.Value + " " + originalMessageId;
				}
				else if (asciiTextHeader2 != null)
				{
					value = asciiTextHeader2.Value;
				}
				else if (!string.IsNullOrEmpty(originalMessageId))
				{
					value = originalMessageId;
				}
			}
			else if (!string.IsNullOrEmpty(originalMessageId))
			{
				value = asciiTextHeader.Value + " " + originalMessageId;
			}
			else
			{
				value = asciiTextHeader.Value;
			}
			if (!string.IsNullOrEmpty(value))
			{
				mimeWriter.WriteHeader(HeaderId.References, value);
			}
		}

		private static byte[] GetBase64DecodedBytes(byte[] currentThreadIndex, out int decodedLength)
		{
			byte[] result;
			using (Base64Decoder base64Decoder = new Base64Decoder())
			{
				byte[] array = new byte[base64Decoder.GetMaxByteCount(currentThreadIndex.Length) + 5];
				int num = 0;
				int num2 = 0;
				bool flag;
				do
				{
					int num3;
					int num4;
					base64Decoder.Convert(currentThreadIndex, num, currentThreadIndex.Length - num, array, num2, array.Length - num2, true, out num3, out num4, out flag);
					num += num3;
					num2 += num4;
				}
				while (!flag);
				decodedLength = num2;
				result = array;
			}
			return result;
		}

		private static string Base64EncodeBytes(byte[] bytes, int length)
		{
			int num = 0;
			int num2 = 0;
			byte[] array;
			using (Base64Encoder base64Encoder = new Base64Encoder(0))
			{
				array = new byte[base64Encoder.GetMaxByteCount(length)];
				bool flag;
				do
				{
					int num3;
					int num4;
					base64Encoder.Convert(bytes, num, length - num, array, num2, array.Length - num2, true, out num3, out num4, out flag);
					num += num3;
					num2 += num4;
				}
				while (!flag);
			}
			return Encoding.ASCII.GetString(array, 0, num2);
		}

		private static uint ComputeNextThreadIndexDelta(byte[] threadIndex, int length)
		{
			long num = DateTime.UtcNow.ToFileTime();
			long num2 = DsnGenerator.ExtractLastFileTime(threadIndex, length);
			num &= 72057594037862400L;
			long num3;
			if (num > num2)
			{
				num3 = num - num2;
			}
			else
			{
				num3 = num2 - num;
			}
			uint num4;
			if (0L == (num3 & 71494644084506624L))
			{
				num4 = (uint)((num3 & 562949953159168L) >> 18);
			}
			else
			{
				num4 = (uint)((num3 & 18014398501093376L) >> 23);
				num4 |= 2147483648U;
			}
			return num4;
		}

		private static long ExtractLastFileTime(byte[] threadIndex, int totalLength)
		{
			long num = (long)((ulong)threadIndex[1] << 48 | (ulong)threadIndex[2] << 40 | (ulong)threadIndex[3] << 32 | (ulong)threadIndex[4] << 24 | (ulong)threadIndex[5] << 16);
			for (int i = 22; i < totalLength; i += 5)
			{
				if (128 == (threadIndex[i] & 128))
				{
					long num2 = (long)(threadIndex[i] & 127) << 47 | (long)((long)((ulong)threadIndex[i + 1]) << 39) | (long)((long)((ulong)threadIndex[i + 2]) << 31) | (long)((long)((ulong)threadIndex[i + 3]) << 23);
					num += num2;
				}
				else
				{
					long num3 = (long)(threadIndex[i] & 127) << 42 | (long)((long)((ulong)threadIndex[i + 1]) << 34) | (long)((long)((ulong)threadIndex[i + 2]) << 26) | (long)((long)((ulong)threadIndex[i + 3]) << 18);
					num += num3;
				}
			}
			return num;
		}

		private static void AddHumanReadableBodyPart(DsnGenerator.DsnConfig dsnConfig, Stream dsnBodyStream, Charset charset, MimeWriter mimeWriter)
		{
			mimeWriter.StartPart();
			if (dsnConfig.SendHtml)
			{
				mimeWriter.StartHeader(HeaderId.ContentType);
				mimeWriter.WriteHeaderValue("multipart/alternative");
				mimeWriter.WriteParameter("differences", "Content-Type");
				mimeWriter.WriteParameter("boundary", Guid.NewGuid().ToString());
				mimeWriter.StartPart();
			}
			DsnGenerator.WritePlainTextHumanReadableBodyPart(dsnBodyStream, charset, mimeWriter);
			if (dsnConfig.SendHtml)
			{
				mimeWriter.EndPart();
				mimeWriter.StartPart();
				DsnGenerator.WriteHmtlHumanReadableBodyPart(dsnBodyStream, charset, mimeWriter);
				mimeWriter.EndPart();
			}
			mimeWriter.EndPart();
		}

		private static void WritePlainTextHumanReadableBodyPart(Stream dsnBodyStream, Charset charset, MimeWriter mimeWriter)
		{
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("text/plain;");
			mimeWriter.WriteParameter("charset", charset.Name);
			mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
			dsnBodyStream.Position = 0L;
			using (EncoderStream encoderStream = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
			{
				Encoding encoding = charset.GetEncoding();
				new HtmlToText
				{
					InputEncoding = encoding,
					DetectEncodingFromByteOrderMark = false,
					DetectEncodingFromMetaTag = false,
					OutputEncoding = encoding
				}.Convert(dsnBodyStream, encoderStream);
			}
		}

		private static void WriteHmtlHumanReadableBodyPart(Stream dsnBodyStream, Charset charset, MimeWriter mimeWriter)
		{
			byte[] buffer = new byte[4096];
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("text/html;");
			mimeWriter.WriteParameter("charset", charset.Name);
			mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
			dsnBodyStream.Position = 0L;
			using (EncoderStream encoderStream = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
			{
				int count;
				while ((count = dsnBodyStream.Read(buffer, 0, 4096)) > 0)
				{
					encoderStream.Write(buffer, 0, count);
				}
				encoderStream.Flush();
			}
		}

		private static void WriteMachineReadableLine(Stream writeStream, byte[] fieldName, string fieldValue)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(fieldValue);
			DsnGenerator.WriteMachineReadableLine(writeStream, fieldName, bytes, 0, bytes.Length);
		}

		private static void WriteMachineReadableLine(Stream writeStream, byte[] fieldName, byte[] fieldValue, int startIndex, int count)
		{
			int i = count;
			int num = fieldName.Length;
			int num2 = 998 - num;
			writeStream.Write(fieldName, 0, num);
			if (i > num2)
			{
				writeStream.Write(fieldValue, startIndex, num2);
				writeStream.Write(DsnGenerator.CRLFSpaceAscii, 0, DsnGenerator.CRLFSpaceAscii.Length);
				startIndex += num2;
				for (i -= num2; i > 997; i -= 997)
				{
					writeStream.Write(fieldValue, startIndex, 997);
					writeStream.Write(DsnGenerator.CRLFSpaceAscii, 0, DsnGenerator.CRLFSpaceAscii.Length);
					startIndex += 997;
				}
			}
			writeStream.Write(fieldValue, startIndex, i);
			writeStream.Write(DsnGenerator.CRLFAscii, 0, DsnGenerator.CRLFAscii.Length);
		}

		private static void WriteStatusText(Stream writeStream, string statusText)
		{
			int num = 0;
			int length = statusText.Length;
			byte[] bytes = Encoding.ASCII.GetBytes(statusText);
			int num2;
			while (num < length && -1 != (num2 = statusText.IndexOf("\r\n", num, length - num, StringComparison.Ordinal)))
			{
				DsnGenerator.WriteMachineReadableLine(writeStream, (num == 0) ? DsnGenerator.DiagnosticCodeFieldAscii : DsnGenerator.SpaceAscii, bytes, num, num2 - num);
				num = num2 + 2;
			}
			if (num < length)
			{
				DsnGenerator.WriteMachineReadableLine(writeStream, (num == 0) ? DsnGenerator.DiagnosticCodeFieldAscii : DsnGenerator.SpaceAscii, bytes, num, length - num);
			}
		}

		private static void AddMachineReadableBodyPart(EncodingOptions encodingOptions, IReadOnlyMailItem mailItem, List<DsnRecipientInfo> recipientList, DsnGenerator.DsnConfig dsnConfig, DsnGenerator.DsnAction dsnAction, MimeWriter mimeWriter, string remoteMta, DateTime expireTime)
		{
			mimeWriter.StartPart();
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("message/delivery-status");
			using (Stream rawContentWriteStream = mimeWriter.GetRawContentWriteStream())
			{
				string envId = mailItem.EnvId;
				if (!string.IsNullOrEmpty(envId))
				{
					DsnGenerator.WriteMachineReadableLine(rawContentWriteStream, DsnGenerator.MessageEnvIdFieldAscii, envId);
				}
				DsnGenerator.WriteMachineReadableLine(rawContentWriteStream, DsnGenerator.MessageReportingMtaFieldAscii, dsnConfig.DsnReportingAuthority);
				if (!string.IsNullOrEmpty(mailItem.HeloDomain))
				{
					DsnGenerator.WriteMachineReadableLine(rawContentWriteStream, DsnGenerator.MessageFromMtaFieldAscii, mailItem.HeloDomain);
				}
				if (mailItem.DateReceived != DateTime.MinValue)
				{
					DateHeader dateHeader = new DateHeader("Date", mailItem.DateReceived);
					DsnGenerator.WriteMachineReadableLine(rawContentWriteStream, DsnGenerator.MessageArrvalDateFieldAscii, dateHeader.Value);
				}
				rawContentWriteStream.Write(DsnGenerator.CRLFAscii, 0, DsnGenerator.CRLFAscii.Length);
				DsnGenerator.WriteMachineReadableListOfRecips(encodingOptions, recipientList, dsnAction, rawContentWriteStream, remoteMta, expireTime);
				rawContentWriteStream.Flush();
			}
			mimeWriter.EndPart();
		}

		private static void AddOriginalMessageOrHeaders(DsnGenerator.DsnConfig dsnConfig, IReadOnlyMailItem mailItem, DsnGenerator.DsnAction dsnAction, MimeWriter mimeWriter, bool isInitMsg, bool isNDRDiagnosticInfoEnabled)
		{
			mimeWriter.StartPart();
			mimeWriter.StartHeader(HeaderId.ContentType);
			if ((dsnAction.Flags & DsnFlags.Quarantine) != DsnFlags.None)
			{
				mimeWriter.WriteHeaderValue("message/rfc822");
				DsnGenerator.AddOriginalMessageForQuarantineDsn(mailItem, mimeWriter);
			}
			else
			{
				if (DsnGenerator.IsFullBodyRequiredInDsn(mailItem, dsnConfig, dsnAction.Flags) && !isInitMsg)
				{
					mimeWriter.WriteHeaderValue("message/rfc822");
					bool flag = false;
					EmailMessage emailMessage = null;
					try
					{
						if (E4eHelper.IsPipelineDecrypted(mailItem))
						{
							emailMessage = DsnGenerator.GetOriginalMessageE4e(mailItem, out flag);
						}
						else
						{
							emailMessage = DsnGenerator.GetOriginalMessage(mailItem, out flag);
						}
						if (emailMessage != null)
						{
							using (Stream rawContentWriteStream = mimeWriter.GetRawContentWriteStream())
							{
								HeaderFirewall.OutputFilter filter = isNDRDiagnosticInfoEnabled ? new HeaderFirewall.OutputFilter(~RestrictedHeaderSet.MTA, true) : new HeaderFirewall.OutputFilter(RestrictedHeaderSet.All, true);
								emailMessage.MimeDocument.RootPart.WriteTo(rawContentWriteStream, null, filter);
								rawContentWriteStream.Flush();
								goto IL_C5;
							}
						}
						DsnGenerator.AddHeaderOnly(mailItem, mimeWriter, isNDRDiagnosticInfoEnabled);
						IL_C5:
						goto IL_DD;
					}
					finally
					{
						if (flag && emailMessage != null)
						{
							emailMessage.Dispose();
						}
					}
				}
				DsnGenerator.AddHeaderOnly(mailItem, mimeWriter, isNDRDiagnosticInfoEnabled);
			}
			IL_DD:
			mimeWriter.EndPart();
		}

		private static EmailMessage GetOriginalMessage(IReadOnlyMailItem mailItem, out bool needDispose)
		{
			string text = null;
			needDispose = false;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionPL", out text);
			if (string.IsNullOrEmpty(text))
			{
				return mailItem.Message;
			}
			string text2 = null;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionUL", out text2);
			if (string.IsNullOrEmpty(text2))
			{
				ExTraceGlobals.DSNTracer.TraceError<string>(0L, "UseLicense absent from {0}, strip it from message from NDR", mailItem.Message.MessageId);
				return null;
			}
			string text3 = null;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionLicenseUri", out text3);
			if (string.IsNullOrEmpty(text3))
			{
				ExTraceGlobals.DSNTracer.TraceError<string>(0L, "LicenseUri absent from {0}, strip it from message from NDR", mailItem.Message.MessageId);
				return null;
			}
			Uri licenseUri = null;
			if (!Uri.TryCreate(text3, UriKind.Absolute, out licenseUri))
			{
				ExTraceGlobals.DSNTracer.TraceError<string>(0L, "LicenseUri is corrupted for {0}, strip it from message from NDR", mailItem.Message.MessageId);
				return null;
			}
			EmailMessage result2;
			using (RmsEncryptor rmsEncryptor = new RmsEncryptor(mailItem.OrganizationId, mailItem, text, text2, licenseUri, null))
			{
				IAsyncResult result = rmsEncryptor.BeginEncrypt(null, null);
				AsyncOperationResult<EmailMessage> asyncOperationResult = rmsEncryptor.EndEncrypt(result);
				if (!asyncOperationResult.IsSucceeded)
				{
					ExTraceGlobals.DSNTracer.TraceError<string, Exception>(0L, "Re-encryption failed for DSN {0} because of {1}, strip it from message from NDR", mailItem.Message.MessageId, asyncOperationResult.Exception);
					result2 = null;
				}
				else
				{
					ExTraceGlobals.DSNTracer.TraceDebug<string>(0L, "Successfully Re-encrypted DSN {0}", mailItem.Message.MessageId);
					needDispose = true;
					result2 = asyncOperationResult.Data;
				}
			}
			return result2;
		}

		private static EmailMessage GetOriginalMessageE4e(IReadOnlyMailItem mailItem, out bool needDispose)
		{
			Exception ex;
			return E4eEncryptionHelper.GetOriginalMessage(mailItem, ExTraceGlobals.DSNTracer, 0L, out needDispose, out ex);
		}

		private static void AddHeaderOnly(IReadOnlyMailItem mailItem, MimeWriter mimeWriter, bool isNDRDiagnosticInfoEnabled)
		{
			mimeWriter.WriteHeaderValue("text/rfc822-headers");
			using (Stream rawContentWriteStream = mimeWriter.GetRawContentWriteStream())
			{
				HeaderFirewall.OutputFilter filter = isNDRDiagnosticInfoEnabled ? new HeaderFirewall.OutputFilter(~RestrictedHeaderSet.MTA) : new HeaderFirewall.OutputFilter(RestrictedHeaderSet.All);
				mailItem.RootPart.Headers.WriteTo(rawContentWriteStream, null, filter);
				rawContentWriteStream.Flush();
			}
		}

		private static void AddOriginalMessageForQuarantineDsn(IReadOnlyMailItem mailItem, MimeWriter mimeWriter)
		{
			bool flag = false;
			using (Stream rawContentWriteStream = mimeWriter.GetRawContentWriteStream())
			{
				EmailRecipient emailRecipient = mailItem.Message.From ?? mailItem.Message.Sender;
				if (emailRecipient != null)
				{
					AddressHeader addressHeader = new AddressHeader("X-MS-Exchange-Organization-Original-Sender");
					addressHeader.AppendChild(new MimeRecipient(null, emailRecipient.SmtpAddress));
					addressHeader.WriteTo(rawContentWriteStream);
					flag = true;
				}
				bool flag2 = false;
				EmailMessage emailMessage = null;
				try
				{
					if (E4eHelper.IsPipelineDecrypted(mailItem))
					{
						emailMessage = DsnGenerator.GetOriginalMessageE4e(mailItem, out flag2);
					}
					else
					{
						emailMessage = DsnGenerator.GetOriginalMessage(mailItem, out flag2);
					}
					if (emailMessage != null)
					{
						emailMessage.MimeDocument.RootPart.WriteTo(rawContentWriteStream);
						flag = true;
					}
				}
				finally
				{
					if (flag2 && emailMessage != null)
					{
						emailMessage.Dispose();
					}
				}
				if (flag)
				{
					rawContentWriteStream.Flush();
				}
			}
		}

		private static bool IsFullBodyRequiredInDsn(IReadOnlyMailItem mailItem, DsnGenerator.DsnConfig dsnConfig, DsnFlags dsnFlags)
		{
			bool flag = mailItem.MimeSize < (long)dsnConfig.DsnOriginalMessageMaxAttachSize.ToBytes();
			if ((dsnConfig.Internal && (dsnFlags & DsnFlags.Delivery) != DsnFlags.None && flag) || ((dsnFlags & DsnFlags.Failure) != DsnFlags.None && (mailItem.IsJournalReport() || flag)))
			{
				switch (mailItem.DsnFormat)
				{
				case DsnFormat.Default:
				case DsnFormat.Full:
					return true;
				}
			}
			return false;
		}

		private static void MarkUpHandledRecipients(IEnumerable<MailRecipient> recipientList, DsnFlags dsnFlags)
		{
			ExTraceGlobals.DSNTracer.TraceDebug(0L, "Recipients Status AFTER Markup");
			foreach (MailRecipient mailRecipient in recipientList)
			{
				if (DsnGenerator.IsDsnNeededForThisRecipient(mailRecipient, dsnFlags))
				{
					mailRecipient.DsnCompleted |= (mailRecipient.DsnNeeded & dsnFlags);
					mailRecipient.DsnNeeded &= ~dsnFlags;
					if (mailRecipient.Status == Status.Handled)
					{
						mailRecipient.Status = Status.Complete;
					}
				}
				DsnGenerator.TraceRecipientStatus(mailRecipient);
			}
		}

		private static bool IsInitiationMessage(EmailMessage message)
		{
			HeaderList headers = message.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Approval-Initiator");
			Header header2 = headers.FindFirst("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers");
			return header != null && header2 != null;
		}

		private static bool IsDsnNeededForThisRecipient(MailRecipient recip, DsnFlags dsnFlags)
		{
			return recip.IsActive && (recip.DsnNeeded & dsnFlags) != DsnFlags.None;
		}

		private static void WriteMachineReadableListOfRecips(EncodingOptions encodingOptions, List<DsnRecipientInfo> recipientList, DsnGenerator.DsnAction dsnAction, Stream bodyStream, string remoteMta, DateTime expireTime)
		{
			for (int i = 0; i < recipientList.Count; i++)
			{
				DsnRecipientInfo recipientInfo = recipientList[i];
				DsnGenerator.WriteMachineReadableRecipientBlock(encodingOptions, bodyStream, dsnAction, recipientInfo, remoteMta, expireTime);
			}
		}

		private static void WriteMachineReadableRecipientBlock(EncodingOptions encodingOptions, Stream bodyStream, DsnGenerator.DsnAction dsnAction, DsnRecipientInfo recipientInfo, string remoteMta, DateTime expireTime)
		{
			string orecip = recipientInfo.ORecip;
			string displayName = recipientInfo.DisplayName;
			string address = recipientInfo.Address;
			string enhancedStatusCode = recipientInfo.EnhancedStatusCode;
			string statusText = recipientInfo.StatusText;
			if (!string.IsNullOrEmpty(orecip))
			{
				DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.OriginalRecipientFieldAscii, orecip);
			}
			DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.FinalRecipientFieldAscii, address);
			DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.ActionFieldAscii, dsnAction.Action);
			DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.StatusFieldAscii, string.IsNullOrEmpty(enhancedStatusCode) ? dsnAction.Status : enhancedStatusCode);
			DsnGenerator.WriteStatusText(bodyStream, statusText);
			if (dsnAction.Flags == DsnFlags.Delay)
			{
				if (expireTime == DateTime.MaxValue)
				{
					throw new InvalidOperationException();
				}
				DateHeader dateHeader = new DateHeader("Date", expireTime);
				DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.WillRetryUntilAscii, dateHeader.Value);
			}
			if (!string.IsNullOrEmpty(remoteMta))
			{
				DsnGenerator.WriteMachineReadableLine(bodyStream, DsnGenerator.RemoteMtaAscii, remoteMta);
			}
			if (!string.IsNullOrEmpty(displayName))
			{
				TextHeader textHeader = new TextHeader("X-Display-Name", displayName);
				textHeader.WriteTo(bodyStream, encodingOptions);
			}
			bodyStream.Write(DsnGenerator.CRLFAscii, 0, DsnGenerator.CRLFAscii.Length);
			ExTraceGlobals.DSNTracer.TraceDebug<string, string>(0L, "Emitted machine recipient - statusCode: {0} diagnostic-code: {1}", enhancedStatusCode, statusText);
		}

		private static TransportMailItem CreateNewDSN(QuarantineConfig quarantineConfig, IReadOnlyMailItem mailItem, DsnGenerator.DsnAction dsnAction, OrganizationId organizationId)
		{
			TransportMailItem transportMailItem = TransportMailItem.NewSideEffectMailItem(mailItem, organizationId, LatencyComponent.DsnGenerator, MailDirectionality.Originating, mailItem.ExternalOrganizationId);
			LatencyTracker.EndTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
			LatencyTracker.BeginTrackLatency(LatencyComponent.DsnGenerator, transportMailItem.LatencyTracker);
			DsnGenerator.TryCacheDSNTransportSettings(transportMailItem);
			if (mailItem.IsShadowed())
			{
				ShadowRedundancyManager shadowRedundancyManager = Components.ShadowRedundancyComponent.ShadowRedundancyManager;
				shadowRedundancyManager.LinkSideEffectMailItem(mailItem, transportMailItem);
			}
			transportMailItem.From = RoutingAddress.NullReversePath;
			if ((dsnAction.Flags & DsnFlags.Quarantine) != DsnFlags.None)
			{
				transportMailItem.Recipients.Add(quarantineConfig.Mailbox);
			}
			else
			{
				transportMailItem.Recipients.Add((string)mailItem.From);
			}
			transportMailItem.ReceiveConnectorName = "DSN";
			transportMailItem.BodyType = BodyType.SevenBit;
			return transportMailItem;
		}

		private static void TryCacheDSNTransportSettings(TransportMailItem dsnMailItem)
		{
			try
			{
				dsnMailItem.CacheTransportSettings();
			}
			catch (TenantTransportSettingsNotFoundException arg)
			{
				ExTraceGlobals.DSNTracer.TraceError<OrganizationId, TenantTransportSettingsNotFoundException>(0L, "Rescoping DSN to first org due to exception while caching transport settings for scope {0}: {1}", dsnMailItem.ADRecipientCache.OrganizationId, arg);
				dsnMailItem.ADRecipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0);
				dsnMailItem.CacheTransportSettings();
			}
			catch (DataSourceOperationException arg2)
			{
				ExTraceGlobals.DSNTracer.TraceError<OrganizationId, DataSourceOperationException>(0L, "DsnGenerator hit exception while caching transport settings for scope {0}: {1}", dsnMailItem.ADRecipientCache.OrganizationId, arg2);
			}
			catch (TransientException arg3)
			{
				ExTraceGlobals.DSNTracer.TraceError<OrganizationId, TransientException>(0L, "DsnGenerator hit exception while caching transport settings for scope {0}: {1}", dsnMailItem.ADRecipientCache.OrganizationId, arg3);
			}
		}

		private void SendDSN(TransportMailItem dsnMailItem, DsnGenerator.DsnAction dsnAction, MsgTrackDSNInfo msgTrackDSNInfo)
		{
			MultilevelAuth.EnsureSecurityAttributes(dsnMailItem, SubmitAuthCategory.Internal, MultilevelAuthMechanism.SecureInternalSubmit, null);
			this.DsnMailOutHandler(dsnMailItem);
			MessageTrackingLog.TrackDSN(dsnMailItem, msgTrackDSNInfo);
		}

		private void SendToCategorizer(TransportMailItem dsnMailItem)
		{
			dsnMailItem.CommitLazy();
			LatencyTracker.EndTrackLatency(LatencyComponent.DsnGenerator, dsnMailItem.LatencyTracker);
			dsnMailItem.PerfCounterAttribution = "InQueue";
			Components.CategorizerComponent.EnqueueSubmittedMessage(dsnMailItem);
		}

		private void RegisterConfigurationChangeHandlers()
		{
			ADObjectId orgAdObjectId = null;
			IConfigurationSession adSession;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2849, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\DsnGenerator.cs");
				orgAdObjectId = adSession.GetOrgContainerId();
			});
			if (!adoperationResult.Succeeded)
			{
				throw new TransportComponentLoadFailedException(Strings.ReadOrgContainerFailed, adoperationResult.Exception);
			}
			Components.ConfigChanged += this.ConfigUpdate;
			Components.Configuration.TransportSettingsChanged += this.TransportSettingsConfigUpdate;
			ADNotificationAdapter.RegisterChangeNotification<SystemMessage>(SystemMessage.GetDsnCustomizationContainer(orgAdObjectId), delegate(ADNotificationEventArgs param0)
			{
				this.ReadSystemMessages();
			});
			ADObjectId quarantineConfigObjectId = null;
			adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				quarantineConfigObjectId = QuarantineConfig.GetConfigObjectId();
			}, 0);
			if (!adoperationResult.Succeeded)
			{
				throw new TransportComponentLoadFailedException(Strings.ReadOrgContainerFailed, adoperationResult.Exception);
			}
			ADNotificationAdapter.RegisterChangeNotification<ContentFilterConfig>(quarantineConfigObjectId, delegate(ADNotificationEventArgs param0)
			{
				this.ReadQuarantineConfig();
			});
		}

		private void ConfigUpdate(object source, EventArgs args)
		{
			this.Configure();
		}

		private void TimedConfigUpdate(object state)
		{
			this.ReadSystemMessages();
		}

		private void TransportSettingsConfigUpdate(TransportSettingsConfiguration transportSettingsConfig)
		{
			this.RefreshDsnCodesCopiedToAdmin(transportSettingsConfig.TransportSettings);
		}

		private void Configure()
		{
			this.ReadQuarantineConfig();
			this.ReadSystemMessages();
			this.RefreshDsnCodesCopiedToAdmin(Components.Configuration.TransportSettings.TransportSettings);
		}

		private void ReadQuarantineConfig()
		{
			QuarantineConfig quarantineConfig = new QuarantineConfig();
			if (quarantineConfig.Load())
			{
				this.quarantineConfig = quarantineConfig;
			}
		}

		private void ReadSystemMessages()
		{
			Dictionary<string, string> internalConfiguredStatusCodes = new Dictionary<string, string>();
			Dictionary<string, string> externalConfiguredStatusCodes = new Dictionary<string, string>();
			ADOperationResult adoperationResult;
			bool flag = ADNotificationAdapter.TryReadConfigurationPaged<SystemMessage>(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2962, "ReadSystemMessages", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\DsnGenerator.cs");
				ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
				return tenantOrTopologyConfigurationSession.FindPaged<SystemMessage>(SystemMessage.GetDsnCustomizationContainer(orgContainerId), QueryScope.SubTree, null, null, ADGenericPagedReader<SystemMessage>.DefaultPageSize);
			}, delegate(SystemMessage dsnMessage)
			{
				if (!(dsnMessage.DsnCode != null))
				{
					if (dsnMessage.QuotaMessageType != null)
					{
						string keyToAdd = dsnMessage.Language.ToString() + dsnMessage.QuotaMessageType.Value.ToString();
						TransportHelpers.AttemptAddToDictionary<string, string>(internalConfiguredStatusCodes, keyToAdd, dsnMessage.Text, null);
					}
					return;
				}
				string keyToAdd2 = dsnMessage.Language.ToString() + dsnMessage.DsnCode.Value;
				if (dsnMessage.Internal)
				{
					TransportHelpers.AttemptAddToDictionary<string, string>(internalConfiguredStatusCodes, keyToAdd2, dsnMessage.Text, null);
					return;
				}
				TransportHelpers.AttemptAddToDictionary<string, string>(externalConfiguredStatusCodes, keyToAdd2, dsnMessage.Text, null);
			}, out adoperationResult);
			if (flag || this.internalHumanReadableWriter == null)
			{
				this.internalHumanReadableWriter = new DsnHumanReadableWriter(internalConfiguredStatusCodes);
			}
			if (flag || this.externalHumanReadableWriter == null)
			{
				this.externalHumanReadableWriter = new DsnHumanReadableWriter(externalConfiguredStatusCodes);
			}
			if (!flag)
			{
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DsnUnableToReadSystemMessageConfig, null, new object[]
				{
					adoperationResult.Exception
				});
			}
		}

		private void RefreshDsnCodesCopiedToAdmin(TransportConfigContainer transportSettings)
		{
			MultiValuedProperty<EnhancedStatusCode> generateCopyOfDSNFor = transportSettings.GenerateCopyOfDSNFor;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>(generateCopyOfDSNFor.Count);
			foreach (EnhancedStatusCode enhancedStatusCode in generateCopyOfDSNFor)
			{
				dictionary[enhancedStatusCode.Value] = true;
			}
			this.dsnCodesCopiedToAdmin = dictionary;
		}

		private DsnGenerator.DsnConfig GetDsnConfig(IReadOnlyMailItem mailitem, bool internalConfig)
		{
			bool flag = false;
			try
			{
				if (!mailitem.IsPoison)
				{
					mailitem.CacheTransportSettings();
					flag = true;
				}
			}
			catch (DataSourceOperationException arg)
			{
				ExTraceGlobals.DSNTracer.TraceDebug<DataSourceOperationException>(0L, "CacheTransportSettings hit {0}", arg);
			}
			catch (TransientException arg2)
			{
				ExTraceGlobals.DSNTracer.TraceDebug<TransientException>(0L, "CacheTransportSettings hit {0}", arg2);
			}
			if (!flag)
			{
				return DsnGenerator.GetOrgWideDsnConfig(internalConfig, mailitem.OrganizationId ?? OrganizationId.ForestWideOrgId);
			}
			PerTenantTransportSettings transportSettings = mailitem.TransportSettings;
			bool flag2 = mailitem.OrganizationId == null || mailitem.OrganizationId == OrganizationId.ForestWideOrgId;
			ByteQuantifiedSize dsnOriginalMessageMaxAttachSize;
			if (internalConfig)
			{
				string reportingServer = DsnGenerator.GetReportingServer(transportSettings.InternalDsnReportingAuthority, mailitem.OrganizationId, internalConfig);
				RoutingAddress internalPostmasterAddress = DsnGenerator.GetInternalPostmasterAddress(mailitem.OrganizationId, transportSettings.ExternalPostmasterAddress);
				dsnOriginalMessageMaxAttachSize = ((transportSettings.InternalDsnMaxMessageAttachSize < Components.Configuration.TransportSettings.TransportSettings.InternalDsnMaxMessageAttachSize) ? transportSettings.InternalDsnMaxMessageAttachSize : Components.Configuration.TransportSettings.TransportSettings.InternalDsnMaxMessageAttachSize);
				return new DsnGenerator.DsnConfig(internalConfig, transportSettings.InternalDelayDsnEnabled, dsnOriginalMessageMaxAttachSize, reportingServer, transportSettings.InternalDsnSendHtml, transportSettings.InternalDsnDefaultLanguage, transportSettings.InternalDsnLanguageDetectionEnabled, internalPostmasterAddress, flag2 ? this.dsnCodesCopiedToAdmin : null, mailitem.OrganizationId);
			}
			string reportingServer2 = DsnGenerator.GetReportingServer(transportSettings.ExternalDsnReportingAuthority, mailitem.OrganizationId, internalConfig);
			RoutingAddress externalPostmasterAddress = DsnGenerator.GetExternalPostmasterAddress(mailitem.OrganizationId, transportSettings.ExternalPostmasterAddress);
			dsnOriginalMessageMaxAttachSize = ((transportSettings.ExternalDsnMaxMessageAttachSize < Components.Configuration.TransportSettings.TransportSettings.ExternalDsnMaxMessageAttachSize) ? transportSettings.ExternalDsnMaxMessageAttachSize : Components.Configuration.TransportSettings.TransportSettings.ExternalDsnMaxMessageAttachSize);
			return new DsnGenerator.DsnConfig(internalConfig, transportSettings.ExternalDelayDsnEnabled, dsnOriginalMessageMaxAttachSize, reportingServer2, transportSettings.ExternalDsnSendHtml, transportSettings.ExternalDsnDefaultLanguage, transportSettings.ExternalDsnLanguageDetectionEnabled, externalPostmasterAddress, flag2 ? this.dsnCodesCopiedToAdmin : null, mailitem.OrganizationId);
		}

		private bool InternalGenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable recipientList, DsnFlags dsnFlags, string remoteMta, LastError lastQueueLevelError)
		{
			DsnGenerator.DsnConfig dsnConfig;
			DsnHumanReadableWriter dsnHumanReadableWriter;
			this.GetDsnConfigAndHumanReadbleWriter(mailItem, out dsnConfig, out dsnHumanReadableWriter);
			DsnGenerator.PatchQuarantineRecipientsIfNeeded(ref dsnFlags, dsnConfig, this.quarantineConfig, recipientList);
			if (!dsnConfig.EnableDelayDsn)
			{
				dsnFlags &= ~DsnFlags.Delay;
			}
			if (dsnFlags == DsnFlags.None)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.OriginalMailDsn, mailItem.LatencyTracker);
				return true;
			}
			bool result;
			try
			{
				HeaderList headers = mailItem.RootPart.Headers;
				Header acceptLanguageHeader = headers.FindFirst("Accept-Language");
				Header contentLanguageHeader = headers.FindFirst(HeaderId.ContentLanguage);
				CultureInfo dsnCulture = dsnHumanReadableWriter.GetDsnCulture(acceptLanguageHeader, contentLanguageHeader, dsnConfig.LanguageDetectionEnabled, dsnConfig.DefaultCulture);
				foreach (DsnGenerator.DsnAction dsnAction in DsnGenerator.dsnActions)
				{
					if ((dsnAction.Flags & dsnFlags) != DsnFlags.None)
					{
						this.GenerateDsn(dsnConfig, dsnHumanReadableWriter, mailItem, recipientList, dsnAction, remoteMta, dsnCulture, lastQueueLevelError);
					}
				}
				result = true;
			}
			catch (IOException ex)
			{
				if (ExceptionHelper.IsHandleableTransientCtsException(ex))
				{
					ExTraceGlobals.DSNTracer.TraceDebug<IOException>(0L, "Transient CTS exception occurred: {0}", ex);
					result = false;
				}
				else
				{
					if (!ExceptionHelper.IsHandleablePermanentCtsException(ex))
					{
						throw;
					}
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DsnDiskFull, null, new object[]
					{
						mailItem.InternetMessageId,
						Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath
					});
					ExceptionHelper.StopServiceOnFatalError(Strings.DiskFull(Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath));
					result = false;
				}
			}
			return result;
		}

		private void GetDsnConfigAndHumanReadbleWriter(IReadOnlyMailItem mailItem, out DsnGenerator.DsnConfig dsnConfig, out DsnHumanReadableWriter dsnHumanWriter)
		{
			OrganizationId organizationId = mailItem.OrganizationId ?? OrganizationId.ForestWideOrgId;
			bool flag = TransportIsInternalResolver.IsInternal(organizationId, mailItem.From, false);
			dsnConfig = this.GetDsnConfig(mailItem, flag);
			dsnHumanWriter = (flag ? this.internalHumanReadableWriter : this.externalHumanReadableWriter);
		}

		private void GenerateDsn(DsnGenerator.DsnConfig dsnConfig, DsnHumanReadableWriter humanReadableWriter, IReadOnlyMailItem mailItem, IEnumerable recipientList, DsnGenerator.DsnAction dsnAction, string remoteMta, CultureInfo cultureInfo, LastError lastQueueLevelError)
		{
			if ((dsnAction.Flags & DsnFlags.Failure) != DsnFlags.None)
			{
				Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Message-Is-Ndr");
				if (header != null)
				{
					MessageTrackingLog.TrackBadmail(MessageTrackingSource.DSN, null, mailItem, "NDRing already generated NDR");
					return;
				}
			}
			TransportMailItem transportMailItem = null;
			DateTime dateTime = (dsnAction.Flags == DsnFlags.Delay) ? ((IQueueItem)mailItem).Expiry.ToLocalTime() : DateTime.MaxValue;
			string reportingServer = DsnGenerator.GetReportingServer(null, dsnConfig.OrganizationId, dsnConfig.Internal);
			string text = string.Concat(new string[]
			{
				"<",
				Guid.NewGuid().ToString(),
				"@",
				reportingServer,
				">"
			});
			string internetMessageId = mailItem.InternetMessageId;
			OutboundCodePageDetector codepageDetector = new OutboundCodePageDetector();
			bool isInitMsg = DsnGenerator.IsInitiationMessage(mailItem.Message);
			bool isNDRDiagnosticInfoEnabled = false;
			if (!mailItem.IsPoison)
			{
				RemoteDomainEntry domainContentConfig = null;
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					domainContentConfig = ContentConverter.GetDomainContentConfig(mailItem.From, mailItem.OrganizationId);
				}, 3);
				isNDRDiagnosticInfoEnabled = (domainContentConfig == null || domainContentConfig.NDRDiagnosticInfoEnabled);
			}
			bool flag;
			bool isExternal;
			List<DsnRecipientInfo> dsnRecipientInfoList = this.GetDsnRecipientInfoList(mailItem, recipientList, lastQueueLevelError, dsnAction.Flags, codepageDetector, humanReadableWriter, cultureInfo, text, dsnConfig, isInitMsg, out flag, out isExternal);
			DateTime dateTimeGenerated = DateTime.UtcNow.ToLocalTime();
			ExTraceGlobals.DSNTracer.TraceDebug<string>(0L, "Generating a {0} DSN", dsnAction.Action);
			transportMailItem = DsnGenerator.CreateNewDSN(this.quarantineConfig, mailItem, dsnAction, dsnConfig.OrganizationId);
			if (flag)
			{
				string smtpAddress = (string)DsnGenerator.GetInternalPostmasterAddress(dsnConfig.OrganizationId, new SmtpAddress?((SmtpAddress)((string)dsnConfig.PostmasterAddress)));
				transportMailItem.Recipients.Add(smtpAddress);
			}
			using (Stream stream = transportMailItem.OpenMimeWriteStream())
			{
				using (Stream stream2 = Streams.CreateTemporaryStorageStream())
				{
					HeaderList headerList = (HeaderList)mailItem.MimeDocument.RootPart.Headers.Clone();
					HeaderFirewall.Filter(headerList, ~RestrictedHeaderSet.MTA);
					Charset charset;
					humanReadableWriter.CreateDsnHumanReadableBody(stream2, cultureInfo, mailItem.Subject, dsnRecipientInfoList, codepageDetector, dsnAction.Flags, dsnConfig.DsnReportingAuthority, remoteMta, new DateTime?(dateTime), headerList, out charset, mailItem.DsnParameters, isExternal, isNDRDiagnosticInfoEnabled);
					EncodingOptions encodingOptions = new EncodingOptions(charset.Name, cultureInfo.Name, Microsoft.Exchange.Data.Mime.EncodingFlags.None);
					using (MimeWriter mimeWriter = new MimeWriter(stream, true, encodingOptions))
					{
						ExTraceGlobals.DSNTracer.TraceDebug<int>(0L, "Created mimeWriter with detected codepage: {0}", charset.CodePage);
						mimeWriter.StartPart();
						DsnGenerator.AddRFC822Headers(dsnConfig, mailItem, dateTimeGenerated, dsnAction, mimeWriter, cultureInfo, text);
						mimeWriter.WriteHeader("Auto-Submitted", "auto-replied");
						DsnGenerator.AddInterceptorHeaders(mimeWriter, mailItem);
						DsnGenerator.AddHumanReadableBodyPart(dsnConfig, stream2, charset, mimeWriter);
						stream2.Close();
						DsnGenerator.AddMachineReadableBodyPart(encodingOptions, mailItem, dsnRecipientInfoList, dsnConfig, dsnAction, mimeWriter, remoteMta, dateTime);
						try
						{
							DsnGenerator.AddOriginalMessageOrHeaders(dsnConfig, mailItem, dsnAction, mimeWriter, isInitMsg, isNDRDiagnosticInfoEnabled);
						}
						catch (ExchangeDataException arg)
						{
							ExTraceGlobals.DSNTracer.TraceError<ExchangeDataException>(0L, "Cannot attach original message or headers, dropping DSN. exception is {0}", arg);
							MessageTrackingLog.TrackBadmail(MessageTrackingSource.DSN, null, mailItem, "Cannot attach original message or headers, dropping DSN.");
							return;
						}
						mimeWriter.EndPart();
					}
				}
			}
			if (transportMailItem != null)
			{
				string originalDsnSender = string.Empty;
				bool flag2 = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-CatchAll-OriginalRecipients") != null;
				if (flag2 && mailItem.Recipients != null)
				{
					originalDsnSender = string.Join(", ", (from r in mailItem.Recipients
					select r.ToString()).ToArray<string>());
					if (mailItem.Recipients.Count > 1)
					{
						ExTraceGlobals.DSNTracer.TraceError<int>(0L, "There should be exactly one recipient when the mail is delivered to the catch-all recipient. Recipient count: {0}", mailItem.Recipients.Count);
					}
				}
				MsgTrackDSNInfo msgTrackDSNInfo = new MsgTrackDSNInfo(internetMessageId, dsnAction.Flags, originalDsnSender);
				if (transportMailItem.MimeDocument != null)
				{
					transportMailItem.Priority = mailItem.Priority;
					transportMailItem.RiskLevel = mailItem.RiskLevel;
					DecodingOptions headerDecodingOptions = transportMailItem.MimeDocument.HeaderDecodingOptions;
					MimeInternalHelpers.SetDecodingOptionsDecodingFlags(ref headerDecodingOptions, headerDecodingOptions.DecodingFlags | DecodingFlags.FallbackToRaw);
					MimeInternalHelpers.SetDocumentDecodingOptions(transportMailItem.MimeDocument, headerDecodingOptions);
				}
				transportMailItem.FallbackToRawOverride = true;
				this.SendDSN(transportMailItem, dsnAction, msgTrackDSNInfo);
				DsnGenerator.IncrementPerfCounter(dsnConfig.Internal, dsnAction.Flags, dsnRecipientInfoList);
				transportMailItem = null;
			}
		}

		private List<DsnRecipientInfo> GetDsnRecipientInfoList(IReadOnlyMailItem mailItem, IEnumerable recipientList, LastError lastQueueLevelError, DsnFlags dsnFlag, OutboundCodePageDetector codepageDetector, DsnHumanReadableWriter dsnHumanReadableWriter, CultureInfo cultureInfo, string dsnMessageId, DsnGenerator.DsnConfig dsnConfig, bool isInitMsg, out bool copyToPostmaster, out bool isExternalNDR)
		{
			List<DsnRecipientInfo> list = recipientList as List<DsnRecipientInfo>;
			copyToPostmaster = false;
			isExternalNDR = false;
			if (list != null)
			{
				ExTraceGlobals.DSNTracer.TraceDebug(0L, "Dsn called with list of DsnRecipientInfo");
				if (dsnFlag == DsnFlags.Failure)
				{
					foreach (DsnRecipientInfo dsnRecipientInfo in list)
					{
						string enhancedStatusCode = dsnRecipientInfo.EnhancedStatusCode;
						bool isCustomizedDsn;
						string ndrhumanReadableExplanation = dsnHumanReadableWriter.GetNDRHumanReadableExplanation(cultureInfo, enhancedStatusCode, isInitMsg, out isCustomizedDsn);
						ExTraceGlobals.DSNTracer.TraceDebug<string, string, CultureInfo>(0L, "Got explanation [{0}] for status {1} in {2}", ndrhumanReadableExplanation, string.IsNullOrEmpty(enhancedStatusCode) ? "<null>" : enhancedStatusCode, cultureInfo);
						if (isInitMsg)
						{
							dsnRecipientInfo.ModeratedRecipients = mailItem.Message.Cc;
							using (EmailRecipientCollection.Enumerator enumerator2 = dsnRecipientInfo.ModeratedRecipients.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									EmailRecipient emailRecipient = enumerator2.Current;
									if (!string.IsNullOrEmpty(emailRecipient.DisplayName))
									{
										codepageDetector.AddText(emailRecipient.DisplayName);
									}
								}
								goto IL_EE;
							}
							goto IL_E1;
						}
						goto IL_E1;
						IL_EE:
						dsnRecipientInfo.DsnHumanReadableExplanation = ndrhumanReadableExplanation;
						dsnRecipientInfo.IsCustomizedDsn = isCustomizedDsn;
						codepageDetector.AddText(ndrhumanReadableExplanation);
						if (this.TryAddEnhancedText(dsnRecipientInfo, cultureInfo))
						{
							codepageDetector.AddText(dsnRecipientInfo.NdrEnhancedText);
						}
						codepageDetector.AddText(dsnRecipientInfo.AddressType);
						codepageDetector.AddText(dsnRecipientInfo.Address);
						if (!copyToPostmaster && !string.IsNullOrEmpty(enhancedStatusCode) && dsnConfig.DsnCodesCopiedToAdmin != null && dsnConfig.DsnCodesCopiedToAdmin.ContainsKey(enhancedStatusCode))
						{
							copyToPostmaster = true;
							continue;
						}
						continue;
						IL_E1:
						codepageDetector.AddText(dsnRecipientInfo.DisplayName);
						goto IL_EE;
					}
				}
				list = this.ReplaceDsnRecipientForCatchAll(list, mailItem, dsnConfig.Internal);
				return list;
			}
			list = new List<DsnRecipientInfo>();
			ExTraceGlobals.DSNTracer.TraceDebug(0L, "DsnRecipientInfo created");
			Dictionary<string, DsnRecipientInfo> dictionary = new Dictionary<string, DsnRecipientInfo>(StringComparer.OrdinalIgnoreCase);
			IEnumerable<MailRecipient> enumerable = recipientList as IEnumerable<MailRecipient>;
			if (enumerable == null)
			{
				throw new ArgumentException("recipientList", "Is not a valid list type");
			}
			IdnMapping idnMapping = new IdnMapping();
			foreach (MailRecipient mailRecipient in enumerable)
			{
				bool flag = false;
				if (mailRecipient.NextHop.NextHopType.DeliveryType == DeliveryType.DnsConnectorDelivery)
				{
					isExternalNDR = true;
				}
				if (DsnGenerator.IsDsnNeededForThisRecipient(mailRecipient, dsnFlag))
				{
					SmtpResponse smtpResponse = mailRecipient.SmtpResponse;
					string enhancedStatusCode2 = smtpResponse.EnhancedStatusCode;
					string text = mailRecipient.Email.ToString();
					string text2 = null;
					if (dsnFlag == DsnFlags.Failure)
					{
						text2 = dsnHumanReadableWriter.GetNDRHumanReadableExplanation(cultureInfo, enhancedStatusCode2, isInitMsg, out flag);
						codepageDetector.AddText(text2);
					}
					ExTraceGlobals.DSNTracer.TraceDebug(0L, string.Format("Address: {0}, got explanation [{1}] for status {2} in {3}, isCustomizedDsn: {4}", new object[]
					{
						text,
						text2,
						string.IsNullOrEmpty(enhancedStatusCode2) ? "<null>" : enhancedStatusCode2,
						cultureInfo,
						flag
					}));
					if (!copyToPostmaster && !string.IsNullOrEmpty(enhancedStatusCode2) && dsnConfig.DsnCodesCopiedToAdmin != null && dsnConfig.DsnCodesCopiedToAdmin.ContainsKey(enhancedStatusCode2))
					{
						copyToPostmaster = true;
					}
					string text3 = null;
					string text4 = DsnGenerator.DeencapsulateAddress(text);
					if (mailRecipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.MapiDisplayName", out text3))
					{
						ExTraceGlobals.DSNTracer.TraceDebug<string, string>(0L, "Got display name [{0}] for address <{1}> from extended properties", text3, text);
					}
					RoutingAddress routingAddress = new RoutingAddress(text4);
					string text5 = null;
					if (routingAddress.IsValid)
					{
						text5 = mailRecipient.Email.DomainPart;
					}
					if (!string.IsNullOrEmpty(text5))
					{
						try
						{
							string unicode = idnMapping.GetUnicode(text5);
							codepageDetector.AddText(unicode);
							text4 = string.Concat(new string[]
							{
								mailRecipient.Email.LocalPart + "@" + unicode
							});
						}
						catch (ArgumentException arg)
						{
							ExTraceGlobals.DSNTracer.TraceDebug<string, ArgumentException>(0L, "Cannot decode domain '{0}' as IDN, exception: {1}", text5, arg);
						}
					}
					bool overwriteDefault;
					string[] dsnParamTexts = DsnHumanReadableWriter.GetDsnParamTexts(DsnParamText.PerRecipientDsnParamText, mailRecipient.DsnParameters, cultureInfo, codepageDetector, out overwriteDefault);
					string dsnSource = null;
					if (!mailRecipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.DsnGenerator.DsnSource", out dsnSource))
					{
						dsnSource = null;
					}
					DsnRecipientInfo dsnRecipientInfo2 = new DsnRecipientInfo(text3, text, null, smtpResponse.EnhancedStatusCode, smtpResponse.ToString(), mailRecipient.ORcpt, text2, smtpResponse.DsnExplanation, text4, dsnParamTexts, overwriteDefault, dsnSource, mailRecipient.RetryCount, (lastQueueLevelError != null && lastQueueLevelError.InnerError != null) ? lastQueueLevelError.InnerError.GetFormattedErrorDetails() : string.Empty, (lastQueueLevelError != null) ? lastQueueLevelError.GetFormattedErrorDetails() : string.Empty, (lastQueueLevelError != null) ? lastQueueLevelError.GetReceivingServerErrorDetails(null) : string.Empty);
					dsnRecipientInfo2.IsCustomizedDsn = flag;
					if (this.TryAddEnhancedText(dsnRecipientInfo2, cultureInfo))
					{
						codepageDetector.AddText(dsnRecipientInfo2.NdrEnhancedText);
					}
					if (isInitMsg)
					{
						dsnRecipientInfo2.ModeratedRecipients = mailItem.Message.Cc;
						using (EmailRecipientCollection.Enumerator enumerator4 = dsnRecipientInfo2.ModeratedRecipients.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								EmailRecipient emailRecipient2 = enumerator4.Current;
								if (!string.IsNullOrEmpty(emailRecipient2.DisplayName))
								{
									codepageDetector.AddText(emailRecipient2.DisplayName);
								}
							}
							goto IL_4F7;
						}
						goto IL_4E5;
					}
					goto IL_4E5;
					IL_4F7:
					list.Add(dsnRecipientInfo2);
					if (text3 == null)
					{
						dictionary[text] = dsnRecipientInfo2;
					}
					mailRecipient.DsnMessageId = dsnMessageId;
					continue;
					IL_4E5:
					if (!string.IsNullOrEmpty(text3))
					{
						codepageDetector.AddText(text3);
						goto IL_4F7;
					}
					goto IL_4F7;
				}
			}
			try
			{
				EmailRecipientCollection to = mailItem.Message.To;
				EmailRecipientCollection cc = mailItem.Message.Cc;
				int count = dictionary.Count;
				ExTraceGlobals.DSNTracer.TraceDebug<int>(0L, "Trying to find {0} addresses in P2 To and Cc", count);
				DsnGenerator.FillDisplayNameFromRecipientCollection(to, dictionary, codepageDetector, ref count);
				DsnGenerator.FillDisplayNameFromRecipientCollection(cc, dictionary, codepageDetector, ref count);
			}
			catch (ExchangeDataException arg2)
			{
				ExTraceGlobals.DSNTracer.TraceDebug<ExchangeDataException>(0L, "ExchangeDataException looking up display names. Will skip further lookups. {e}", arg2);
			}
			list = this.ReplaceDsnRecipientForCatchAll(list, mailItem, dsnConfig.Internal);
			return list;
		}

		private List<DsnRecipientInfo> ReplaceDsnRecipientForCatchAll(List<DsnRecipientInfo> dsnRecipientList, IReadOnlyMailItem mailItem, bool toInternal)
		{
			Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-CatchAll-OriginalRecipients");
			if (header == null)
			{
				return dsnRecipientList;
			}
			DsnGeneratorPerfCountersInstance dsnGeneratorPerfCountersInstance = toInternal ? DsnGenerator.internalPerfCounters : DsnGenerator.externalPerfCounters;
			if (dsnGeneratorPerfCountersInstance != null)
			{
				dsnGeneratorPerfCountersInstance.CatchAllRecipientFailedDsnTotal.Increment();
			}
			if (string.IsNullOrEmpty(header.Value))
			{
				ExTraceGlobals.DSNTracer.TraceError(0L, "The value of the catch-all original recipient header is null or empty");
				return dsnRecipientList;
			}
			if (dsnRecipientList.Count != 1)
			{
				ExTraceGlobals.DSNTracer.TraceError<int>(0L, "There should be exactly one DSN recipient in the case of catch-all. DSN recipient count: {0}", dsnRecipientList.Count);
				return dsnRecipientList;
			}
			string[] array = header.Value.Split(new char[]
			{
				';'
			});
			DsnRecipientInfo dsnRecipientInfo = dsnRecipientList[0];
			List<DsnRecipientInfo> list = new List<DsnRecipientInfo>();
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text) && RoutingAddress.IsValidAddress(text))
				{
					list.Add(dsnRecipientInfo.NewCloneWithDifferentRecipient(text));
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			return dsnRecipientList;
		}

		private void AddQuotaRFC822Headers(TransportMailItem quotaMailItem, MimeWriter mimeWriter, string quotaClass, QuotaInformation quotaInfo)
		{
			string value = (string)DsnGenerator.GetInternalPostmasterAddress(quotaMailItem.OrganizationId, quotaMailItem.TransportSettings.ExternalPostmasterAddress);
			mimeWriter.WriteHeader(HeaderId.MimeVersion, "1.0");
			mimeWriter.WriteHeader(HeaderId.From, value);
			mimeWriter.StartHeader(HeaderId.To);
			foreach (MailRecipient mailRecipient in quotaMailItem.Recipients)
			{
				mimeWriter.WriteRecipient(null, (string)mailRecipient.Email);
			}
			mimeWriter.StartHeader(HeaderId.Date);
			mimeWriter.WriteHeaderValue(DateTime.UtcNow.ToLocalTime());
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("multipart/alternative");
			mimeWriter.WriteParameter("differences", "Content-Type");
			mimeWriter.WriteParameter("boundary", Guid.NewGuid().ToString());
			mimeWriter.WriteHeader(HeaderId.ContentLanguage, quotaInfo.Culture.Name);
			mimeWriter.WriteHeader(HeaderId.MessageId, "<" + Guid.NewGuid().ToString() + ">");
			mimeWriter.WriteHeader(HeaderId.Importance, "high");
			mimeWriter.WriteHeader("X-MS-Exchange-Organization-SCL", "-1");
			mimeWriter.WriteHeader("X-MS-Exchange-Organization-StorageQuota", quotaClass);
			mimeWriter.WriteHeader(HeaderId.Subject, quotaInfo.Subject);
		}

		private bool TryAddEnhancedText(DsnRecipientInfo recipientInfo, CultureInfo cultureInfo)
		{
			SmtpResponse key;
			LocalizedString localizedString;
			if (!string.IsNullOrEmpty(recipientInfo.StatusText) && SmtpResponse.TryParse(recipientInfo.StatusText, out key) && AckReason.EnhancedTextGetter.TryGetValue(key, out localizedString))
			{
				recipientInfo.NdrEnhancedText = localizedString.ToString(cultureInfo);
				ExTraceGlobals.DSNTracer.TraceDebug<string, string>(0L, "RecipientInfo Address: '{0}', NdrEnhancedText set to: [{1}]", recipientInfo.Address, recipientInfo.NdrEnhancedText ?? "<null>");
				return true;
			}
			return false;
		}

		public const string MapiDisplayName = "Microsoft.Exchange.MapiDisplayName";

		public const string DsnSourceName = "Microsoft.Exchange.DsnGenerator.DsnSource";

		public const string XStorageQuotaHeader = "X-MS-Exchange-Organization-StorageQuota";

		public const string QuotaSendReceive = "SendReceive";

		public const string QuotaSend = "Send";

		public const string QuotaWarning = "Warning";

		public const string DsnVersion = "12";

		private const string CRLF = "\r\n";

		private const string DiagnosticCodeField = "Diagnostic-Code: smtp;";

		private const string InternalDsnPerfCounterInstanceName = "Internal";

		private const string ExternalDsnPerfCounterInstanceName = "External";

		private const int MaxMachineReadableLineLength = 998;

		private const int MinThreadIndexTotalSize = 22;

		private const int ThreadIndexDeltaSize = 5;

		private static readonly byte[] DiagnosticCodeFieldAscii = Encoding.ASCII.GetBytes("Diagnostic-Code: smtp;");

		private static readonly byte[] MessageEnvIdFieldAscii = Encoding.ASCII.GetBytes("Original-Envelope-Id: ");

		private static readonly byte[] MessageFromMtaFieldAscii = Encoding.ASCII.GetBytes("Received-From-MTA: dns;");

		private static readonly byte[] MessageReportingMtaFieldAscii = Encoding.ASCII.GetBytes("Reporting-MTA: dns;");

		private static readonly byte[] MessageArrvalDateFieldAscii = Encoding.ASCII.GetBytes("Arrival-Date: ");

		private static readonly byte[] OriginalRecipientFieldAscii = Encoding.ASCII.GetBytes("Original-Recipient: ");

		private static readonly byte[] FinalRecipientFieldAscii = Encoding.ASCII.GetBytes("Final-Recipient: rfc822;");

		private static readonly byte[] ActionFieldAscii = Encoding.ASCII.GetBytes("Action: ");

		private static readonly byte[] StatusFieldAscii = Encoding.ASCII.GetBytes("Status: ");

		private static readonly byte[] WillRetryUntilAscii = Encoding.ASCII.GetBytes("Will-Retry-Until: ");

		private static readonly byte[] RemoteMtaAscii = Encoding.ASCII.GetBytes("Remote-MTA: dns;");

		private static readonly byte[] CRLFAscii = new byte[]
		{
			13,
			10
		};

		private static readonly byte[] CRLFSpaceAscii = new byte[]
		{
			13,
			10,
			32
		};

		private static readonly byte[] SpaceAscii = new byte[]
		{
			32
		};

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport DSN"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport DSN"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery DSN"
			},
			{
				ProcessTransportRole.MailboxSubmission,
				"MSExchange Submission DSN"
			}
		};

		private static DsnGenerator.DsnAction[] dsnActions = new DsnGenerator.DsnAction[]
		{
			new DsnGenerator.DsnAction("failed", DsnFlags.Failure, "5.0.0"),
			new DsnGenerator.DsnAction("delayed", DsnFlags.Delay, "4.0.0"),
			new DsnGenerator.DsnAction("relayed", DsnFlags.Relay, "2.0.0"),
			new DsnGenerator.DsnAction("delivered", DsnFlags.Delivery, "2.0.0"),
			new DsnGenerator.DsnAction("expanded", DsnFlags.Expansion, "2.0.0"),
			new DsnGenerator.DsnAction("failed", DsnFlags.Quarantine, "5.0.0")
		};

		private static DsnGeneratorPerfCountersInstance externalPerfCounters;

		private static DsnGeneratorPerfCountersInstance internalPerfCounters;

		private static HashSet<string> filteredOutFailureDsns = new HashSet<string>(new string[]
		{
			AckReason.LocalRecipientAddressUnknown.ToString(),
			AckReason.RecipientPermissionRestricted.ToString()
		}, FailureDsnComparer.Instance);

		private static HashSet<string> filteredOutDsnSources = new HashSet<string>(new string[]
		{
			DsnSource.TransportRuleAgent
		}, StringComparer.OrdinalIgnoreCase);

		private ExEventLog eventLogger;

		private QuarantineConfig quarantineConfig;

		private DsnHumanReadableWriter internalHumanReadableWriter;

		private DsnHumanReadableWriter externalHumanReadableWriter;

		private Dictionary<string, bool> dsnCodesCopiedToAdmin;

		private DsnMailOutHandlerDelegate dsnMailOutHandler;

		private GuardedTimer configReloadTimer;

		public enum CallerComponent
		{
			Other,
			ResolverRouting,
			Delivery
		}

		internal struct DsnAction
		{
			public DsnAction(string action, DsnFlags flags, string status)
			{
				this.Action = action;
				this.Flags = flags;
				this.Status = status;
				this.Subject = DsnHumanReadableWriter.GetLocalizedSubjectPrefix(this.Flags);
			}

			public string Action;

			public DsnFlags Flags;

			public string Status;

			public LocalizedString Subject;
		}

		private class DsnConfig
		{
			public DsnConfig(bool internalConfig, bool enableDelayDsn, ByteQuantifiedSize dsnOriginalMessageMaxAttachSize, string dsnReportingAuthority, bool sendHtml, CultureInfo defaultCulture, bool languageDetectionEnabled, RoutingAddress postmasterAddress, Dictionary<string, bool> dsnCodesCopiedToAdmin, OrganizationId organizationId)
			{
				this.internalConfig = internalConfig;
				this.enableDelayDsn = enableDelayDsn;
				this.dsnOriginalMessageMaxAttachSize = dsnOriginalMessageMaxAttachSize;
				this.dsnReportingAuthority = dsnReportingAuthority;
				this.sendHtml = sendHtml;
				this.defaultCulture = defaultCulture;
				this.languageDetectionEnabled = languageDetectionEnabled;
				this.postmasterAddress = postmasterAddress;
				this.dsnCodesCopiedToAdmin = dsnCodesCopiedToAdmin;
				this.organizationId = (organizationId ?? OrganizationId.ForestWideOrgId);
			}

			public string DsnReportingAuthority
			{
				get
				{
					return this.dsnReportingAuthority;
				}
			}

			public bool EnableDelayDsn
			{
				get
				{
					return this.enableDelayDsn;
				}
			}

			public ByteQuantifiedSize DsnOriginalMessageMaxAttachSize
			{
				get
				{
					return this.dsnOriginalMessageMaxAttachSize;
				}
			}

			public bool Internal
			{
				get
				{
					return this.internalConfig;
				}
			}

			public bool SendHtml
			{
				get
				{
					return this.sendHtml;
				}
			}

			public CultureInfo DefaultCulture
			{
				get
				{
					return this.defaultCulture;
				}
			}

			public bool LanguageDetectionEnabled
			{
				get
				{
					return this.languageDetectionEnabled;
				}
			}

			public RoutingAddress PostmasterAddress
			{
				get
				{
					return this.postmasterAddress;
				}
			}

			public Dictionary<string, bool> DsnCodesCopiedToAdmin
			{
				get
				{
					return this.dsnCodesCopiedToAdmin;
				}
			}

			public OrganizationId OrganizationId
			{
				get
				{
					return this.organizationId;
				}
			}

			private bool enableDelayDsn;

			private ByteQuantifiedSize dsnOriginalMessageMaxAttachSize;

			private string dsnReportingAuthority;

			private bool internalConfig;

			private bool sendHtml;

			private CultureInfo defaultCulture;

			private bool languageDetectionEnabled;

			private RoutingAddress postmasterAddress;

			private Dictionary<string, bool> dsnCodesCopiedToAdmin;

			private OrganizationId organizationId;
		}

		private class DeliveryFailureCounterWrapper
		{
			private DeliveryFailureCounterWrapper()
			{
				this.percentRouting544 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentResolver514 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentResolver520 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentResolver524 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentResolver546 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliverySmtp560 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliverySmtpTotal = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliveryStoreDriver520 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliveryStoreDriver560 = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentRoutingTotal = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentResolverTotal = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliveryStoreDriverTotal = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentDeliveryAgent = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.percentForeignConnector = new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0));
				this.internalFailureDSNinLastHour = new SlidingPercentageCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
				this.alertableInternalFailureDsnInLastHour = new SlidingPercentageCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
				this.internalDelayedDSNinLastHour = new SlidingPercentageCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
				this.externalFailureDSNinLastHour = new SlidingPercentageCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
				this.externalDelayedDSNinLastHour = new SlidingPercentageCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			}

			public static DsnGenerator.DeliveryFailureCounterWrapper Instance
			{
				get
				{
					return DsnGenerator.DeliveryFailureCounterWrapper.instance;
				}
			}

			public void CheckCategorizerRecipient(IEnumerable<MailRecipient> recipientList)
			{
				foreach (MailRecipient mailRecipient in recipientList)
				{
					SmtpResponse smtpResponse = mailRecipient.SmtpResponse;
					if (smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
					{
						if (this.IsSmtpResponseCount(smtpResponse, "Resolver"))
						{
							string enhancedStatusCode;
							if ((enhancedStatusCode = smtpResponse.EnhancedStatusCode) != null)
							{
								if (!(enhancedStatusCode == "5.1.4"))
								{
									if (!(enhancedStatusCode == "5.2.0"))
									{
										if (!(enhancedStatusCode == "5.2.4"))
										{
											if (enhancedStatusCode == "5.4.6")
											{
												this.percentResolver546.AddNumerator(1L);
											}
										}
										else
										{
											this.percentResolver524.AddNumerator(1L);
										}
									}
									else
									{
										this.percentResolver520.AddNumerator(1L);
									}
								}
								else
								{
									this.percentResolver514.AddNumerator(1L);
								}
							}
							this.UpdateCountersResolverFailure();
							continue;
						}
						if (this.IsSmtpResponseCount(smtpResponse, "Routing") && !smtpResponse.StatusText[0].StartsWith("ROUTING.NoConnectorForAddressType", StringComparison.OrdinalIgnoreCase))
						{
							if (string.Equals(smtpResponse.EnhancedStatusCode, "5.4.4", StringComparison.Ordinal))
							{
								this.percentRouting544.AddNumerator(1L);
							}
							this.UpdateCountersRoutingFailure();
							continue;
						}
					}
					this.UpdateCountersOtherCategorizer();
				}
			}

			public void CheckDeliveryRecipient(IEnumerable<MailRecipient> recipientList)
			{
				foreach (MailRecipient mailRecipient in recipientList)
				{
					SmtpResponse smtpResponse = mailRecipient.SmtpResponse;
					if (smtpResponse.EnhancedStatusCode != null)
					{
						switch (mailRecipient.NextHop.NextHopType.DeliveryType)
						{
						case DeliveryType.MapiDelivery:
						case DeliveryType.SmtpDeliveryToMailbox:
							if (smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
							{
								if (string.Equals(smtpResponse.EnhancedStatusCode, "5.6.0", StringComparison.Ordinal))
								{
									this.percentDeliveryStoreDriver560.AddNumerator(1L);
								}
								else if (string.Equals(smtpResponse.EnhancedStatusCode, "5.2.0", StringComparison.Ordinal))
								{
									this.percentDeliveryStoreDriver520.AddNumerator(1L);
								}
								this.percentDeliveryStoreDriver520.AddDenominator(1L);
								this.percentDeliveryStoreDriver560.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentDeliveryStoreDriver520, DeliveryFailurePerfCounters.Delivery_StoreDriver_5_2_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleStoreDriver5_2_0);
								this.UpdatePerfCounter(this.percentDeliveryStoreDriver560, DeliveryFailurePerfCounters.Delivery_StoreDriver_5_6_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleStoreDriver5_6_0);
								this.percentDeliveryStoreDriverTotal.AddNumerator(1L);
								this.percentDeliveryStoreDriverTotal.AddDenominator(1L);
								DeliveryFailurePerfCounters.Delivery_StoreDriver_Total.RawValue = (long)((int)this.percentDeliveryStoreDriverTotal.Numerator);
							}
							else if (smtpResponse.SmtpResponseType == SmtpResponseType.Success)
							{
								this.percentDeliveryStoreDriver520.AddDenominator(1L);
								this.percentDeliveryStoreDriver560.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentDeliveryStoreDriver520, DeliveryFailurePerfCounters.Delivery_StoreDriver_5_2_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleStoreDriver5_2_0);
								this.UpdatePerfCounter(this.percentDeliveryStoreDriver560, DeliveryFailurePerfCounters.Delivery_StoreDriver_5_6_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleStoreDriver5_6_0);
								this.percentDeliveryStoreDriverTotal.AddDenominator(1L);
							}
							break;
						case DeliveryType.NonSmtpGatewayDelivery:
							if (smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
							{
								this.percentForeignConnector.AddNumerator(1L);
								this.percentForeignConnector.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentForeignConnector, DeliveryFailurePerfCounters.Delivery_ForeignConnector_Total, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleForeignConnector);
							}
							else if (smtpResponse.SmtpResponseType == SmtpResponseType.Success)
							{
								this.percentForeignConnector.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentForeignConnector, DeliveryFailurePerfCounters.Delivery_ForeignConnector_Total, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleForeignConnector);
							}
							break;
						case DeliveryType.SmartHostConnectorDelivery:
						case DeliveryType.SmtpRelayToRemoteAdSite:
						case DeliveryType.SmtpRelayToTiRg:
						case DeliveryType.SmtpRelayWithinAdSite:
						case DeliveryType.SmtpRelayWithinAdSiteToEdge:
						case DeliveryType.SmtpRelayToDag:
						case DeliveryType.SmtpRelayToMailboxDeliveryGroup:
						case DeliveryType.SmtpRelayToConnectorSourceServers:
						case DeliveryType.SmtpRelayToServers:
							if (smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
							{
								if (string.Equals(smtpResponse.EnhancedStatusCode, "5.6.0", StringComparison.Ordinal))
								{
									this.percentDeliverySmtp560.AddNumerator(1L);
									this.percentDeliverySmtp560.AddDenominator(1L);
									this.UpdatePerfCounter(this.percentDeliverySmtp560, DeliveryFailurePerfCounters.Delivery_SMTP_5_6_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleDeliverySMTP5_6_0);
								}
								this.percentDeliverySmtpTotal.AddNumerator(1L);
								this.percentDeliverySmtpTotal.AddDenominator(1L);
								DeliveryFailurePerfCounters.Delivery_SMTP_Total.RawValue = (long)((int)this.percentDeliverySmtpTotal.Numerator);
							}
							else if (smtpResponse.SmtpResponseType == SmtpResponseType.Success)
							{
								this.percentDeliverySmtp560.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentDeliverySmtp560, DeliveryFailurePerfCounters.Delivery_SMTP_5_6_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleDeliverySMTP5_6_0);
								this.percentDeliverySmtpTotal.AddDenominator(1L);
							}
							break;
						case DeliveryType.DeliveryAgent:
							if (smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
							{
								this.percentDeliveryAgent.AddNumerator(1L);
								this.percentDeliveryAgent.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentDeliveryAgent, DeliveryFailurePerfCounters.Delivery_DeliveryAgent_Total, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleDeliveryAgent);
							}
							else if (smtpResponse.SmtpResponseType == SmtpResponseType.Success)
							{
								this.percentDeliveryAgent.AddDenominator(1L);
								this.UpdatePerfCounter(this.percentDeliveryAgent, DeliveryFailurePerfCounters.Delivery_DeliveryAgent_Total, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleDeliveryAgent);
							}
							break;
						}
					}
				}
			}

			public void IncreaseDsnSlidingCounters(DsnGeneratorPerfCountersInstance counterInstance, bool internalDSN, DsnFlags dsnFlags, List<DsnRecipientInfo> dsnRecipientInfoList)
			{
				if (internalDSN)
				{
					if (dsnFlags == DsnFlags.Failure)
					{
						this.internalFailureDSNinLastHour.AddNumerator(1L);
						this.internalFailureDSNinLastHour.AddDenominator(1L);
						counterInstance.FailedDsnInLastHour.RawValue = this.internalFailureDSNinLastHour.Numerator;
						if (!DsnGenerator.DeliveryFailureCounterWrapper.ShouldFilterOutDsn(dsnRecipientInfoList))
						{
							this.alertableInternalFailureDsnInLastHour.AddNumerator(1L);
							this.alertableInternalFailureDsnInLastHour.AddDenominator(1L);
							counterInstance.AlertableFailedDsnInLastHour.RawValue = this.alertableInternalFailureDsnInLastHour.Numerator;
							return;
						}
					}
					else if (dsnFlags == DsnFlags.Delay)
					{
						this.internalDelayedDSNinLastHour.AddNumerator(1L);
						this.internalDelayedDSNinLastHour.AddDenominator(1L);
						counterInstance.DelayedDsnInLastHour.RawValue = this.internalDelayedDSNinLastHour.Numerator;
						return;
					}
				}
				else
				{
					if (dsnFlags == DsnFlags.Failure)
					{
						this.externalFailureDSNinLastHour.AddNumerator(1L);
						this.externalFailureDSNinLastHour.AddDenominator(1L);
						counterInstance.FailedDsnInLastHour.RawValue = this.externalFailureDSNinLastHour.Numerator;
						return;
					}
					if (dsnFlags == DsnFlags.Delay)
					{
						this.externalDelayedDSNinLastHour.AddNumerator(1L);
						this.externalDelayedDSNinLastHour.AddDenominator(1L);
						counterInstance.DelayedDsnInLastHour.RawValue = this.externalDelayedDSNinLastHour.Numerator;
					}
				}
			}

			public void UpdateDsnSlidingCounters()
			{
				this.internalFailureDSNinLastHour.GetSlidingPercentage();
				DsnGenerator.internalPerfCounters.FailedDsnInLastHour.RawValue = this.internalFailureDSNinLastHour.Numerator;
				this.internalDelayedDSNinLastHour.GetSlidingPercentage();
				DsnGenerator.internalPerfCounters.DelayedDsnInLastHour.RawValue = this.internalDelayedDSNinLastHour.Numerator;
				this.externalFailureDSNinLastHour.GetSlidingPercentage();
				DsnGenerator.externalPerfCounters.FailedDsnInLastHour.RawValue = this.externalFailureDSNinLastHour.Numerator;
				this.externalDelayedDSNinLastHour.GetSlidingPercentage();
				DsnGenerator.externalPerfCounters.DelayedDsnInLastHour.RawValue = this.externalDelayedDSNinLastHour.Numerator;
				this.alertableInternalFailureDsnInLastHour.GetSlidingPercentage();
				DsnGenerator.internalPerfCounters.AlertableFailedDsnInLastHour.RawValue = this.alertableInternalFailureDsnInLastHour.Numerator;
			}

			private static bool ShouldFilterOutDsn(List<DsnRecipientInfo> dsnRecipientInfoList)
			{
				if (dsnRecipientInfoList == null || dsnRecipientInfoList.Count == 0)
				{
					return false;
				}
				foreach (DsnRecipientInfo dsnRecipientInfo in dsnRecipientInfoList)
				{
					if (string.IsNullOrEmpty(dsnRecipientInfo.StatusText) || (!DsnGenerator.filteredOutFailureDsns.Contains(dsnRecipientInfo.StatusText) && !DsnGenerator.filteredOutDsnSources.Contains(dsnRecipientInfo.DsnSource)))
					{
						return false;
					}
				}
				return true;
			}

			private void UpdatePerfCounter(SlidingPercentageCounter slidingCounter, ExPerformanceCounter perfCounter, int minSample)
			{
				int num = (int)slidingCounter.GetSlidingPercentage();
				perfCounter.RawValue = (long)((slidingCounter.Denominator >= (long)minSample) ? num : 0);
			}

			private bool IsSmtpResponseCount(SmtpResponse smtpResponse, string textStart)
			{
				return smtpResponse.StatusText != null && smtpResponse.StatusText.Length > 0 && smtpResponse.StatusText[0].StartsWith(textStart, StringComparison.OrdinalIgnoreCase);
			}

			private void UpdateCountersResolverFailure()
			{
				this.percentResolver514.AddDenominator(1L);
				this.percentResolver520.AddDenominator(1L);
				this.percentResolver524.AddDenominator(1L);
				this.percentResolver546.AddDenominator(1L);
				this.percentResolverTotal.AddNumerator(1L);
				this.percentResolverTotal.AddDenominator(1L);
				this.UpdatePerfCounter(this.percentResolver514, DeliveryFailurePerfCounters.Resolver_5_1_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_1_4);
				this.UpdatePerfCounter(this.percentResolver520, DeliveryFailurePerfCounters.Resolver_5_2_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_0);
				this.UpdatePerfCounter(this.percentResolver524, DeliveryFailurePerfCounters.Resolver_5_2_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_4);
				this.UpdatePerfCounter(this.percentResolver546, DeliveryFailurePerfCounters.Resolver_5_4_6, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_4_6);
				DeliveryFailurePerfCounters.Resolver_Total.RawValue = (long)((int)this.percentResolverTotal.Numerator);
			}

			private void UpdateCountersRoutingFailure()
			{
				this.percentRouting544.AddDenominator(1L);
				this.percentRoutingTotal.AddNumerator(1L);
				this.percentRoutingTotal.AddDenominator(1L);
				this.percentResolver514.AddDenominator(1L);
				this.percentResolver520.AddDenominator(1L);
				this.percentResolver524.AddDenominator(1L);
				this.percentResolver546.AddDenominator(1L);
				this.percentResolverTotal.AddDenominator(1L);
				this.UpdatePerfCounter(this.percentRouting544, DeliveryFailurePerfCounters.Routing_5_4_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleRouting5_4_4);
				DeliveryFailurePerfCounters.Routing_Total.RawValue = this.percentRoutingTotal.Numerator;
				this.UpdatePerfCounter(this.percentResolver514, DeliveryFailurePerfCounters.Resolver_5_1_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_1_4);
				this.UpdatePerfCounter(this.percentResolver520, DeliveryFailurePerfCounters.Resolver_5_2_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_0);
				this.UpdatePerfCounter(this.percentResolver524, DeliveryFailurePerfCounters.Resolver_5_2_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_4);
				this.UpdatePerfCounter(this.percentResolver546, DeliveryFailurePerfCounters.Resolver_5_4_6, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_4_6);
				this.percentResolverTotal.AddDenominator(1L);
			}

			private void UpdateCountersOtherCategorizer()
			{
				this.percentRouting544.AddDenominator(1L);
				this.percentRoutingTotal.AddDenominator(1L);
				this.percentResolver514.AddDenominator(1L);
				this.percentResolver520.AddDenominator(1L);
				this.percentResolver524.AddDenominator(1L);
				this.percentResolver546.AddDenominator(1L);
				this.percentResolverTotal.AddDenominator(1L);
				this.UpdatePerfCounter(this.percentRouting544, DeliveryFailurePerfCounters.Routing_5_4_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleRouting5_4_4);
				this.UpdatePerfCounter(this.percentResolver514, DeliveryFailurePerfCounters.Resolver_5_1_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_1_4);
				this.UpdatePerfCounter(this.percentResolver520, DeliveryFailurePerfCounters.Resolver_5_2_0, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_0);
				this.UpdatePerfCounter(this.percentResolver524, DeliveryFailurePerfCounters.Resolver_5_2_4, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_2_4);
				this.UpdatePerfCounter(this.percentResolver546, DeliveryFailurePerfCounters.Resolver_5_4_6, Components.TransportAppConfig.DeliveryFailureConfiguration.DeliveryFailureMinSampleResolver5_4_6);
			}

			private static DsnGenerator.DeliveryFailureCounterWrapper instance = new DsnGenerator.DeliveryFailureCounterWrapper();

			private SlidingPercentageCounter percentRouting544;

			private SlidingPercentageCounter percentRoutingTotal;

			private SlidingPercentageCounter percentResolverTotal;

			private SlidingPercentageCounter percentResolver514;

			private SlidingPercentageCounter percentResolver520;

			private SlidingPercentageCounter percentResolver524;

			private SlidingPercentageCounter percentResolver546;

			private SlidingPercentageCounter percentDeliverySmtp560;

			private SlidingPercentageCounter percentDeliverySmtpTotal;

			private SlidingPercentageCounter percentDeliveryStoreDriver520;

			private SlidingPercentageCounter percentDeliveryStoreDriver560;

			private SlidingPercentageCounter percentDeliveryStoreDriverTotal;

			private SlidingPercentageCounter percentDeliveryAgent;

			private SlidingPercentageCounter percentForeignConnector;

			private SlidingPercentageCounter internalFailureDSNinLastHour;

			private SlidingPercentageCounter alertableInternalFailureDsnInLastHour;

			private SlidingPercentageCounter internalDelayedDSNinLastHour;

			private SlidingPercentageCounter externalFailureDSNinLastHour;

			private SlidingPercentageCounter externalDelayedDSNinLastHour;
		}
	}
}
