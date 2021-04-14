using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies.Journaling;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Internal;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal class UnwrapJournalAgent : RoutingAgent
	{
		public UnwrapJournalAgent(bool isEnabled, JournalPerfCountersWrapper perfCountersWrapper)
		{
			this.isEHAJournalingEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.EHAJournaling.Enabled;
			this.isEnabled = isEnabled;
			base.OnSubmittedMessage += this.SubmitMessage;
			this.perfCountersWrapper = perfCountersWrapper;
		}

		private static void CommitTrackAndEnqueue(TransportMailItem journalMailItem, TransportMailItem transportMailItem, string agentName)
		{
			transportMailItem.CommitLazy();
			SubmitHelper.TrackAndEnqueue(journalMailItem, transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion, agentName);
		}

		private static TransportMailItem CreateNewTransportMailItem(IReadOnlyMailItem originalMailItem, EmailMessage emailMessage, OrganizationId organizationId, string agentName, string perfAttribution, RoutingAddress p2FromAddress)
		{
			TransportMailItem transportMailItem = TransportMailItem.NewSideEffectMailItem(originalMailItem, organizationId, LatencyComponent.Agent, MailDirectionality.Incoming, default(Guid));
			if (transportMailItem != null)
			{
				transportMailItem.PerfCounterAttribution = perfAttribution;
				transportMailItem.MimeDocument = emailMessage.MimeDocument;
				transportMailItem.ReceiveConnectorName = "Agent:" + agentName;
				transportMailItem.From = p2FromAddress;
				SubmitHelper.PatchHeaders(transportMailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion);
				TransportFacades.EnsureSecurityAttributes(transportMailItem);
				SubmitHelper.StampOriginalMessageSize(transportMailItem);
			}
			return transportMailItem;
		}

		private static void SetPrioritization(ITransportMailItemFacade tmi, bool isSourceEha)
		{
			if (isSourceEha)
			{
				tmi.PrioritizationReason = "eha legacy archive journaling";
			}
			else
			{
				tmi.PrioritizationReason = "Live archive journaling";
			}
			tmi.Priority = DeliveryPriority.None;
		}

		private static void SetAutoResponseSuppress(TransportMailItem mailItem, AutoResponseSuppress suppress)
		{
			if (mailItem != null && mailItem.RootPart != null && mailItem.RootPart.Headers != null)
			{
				Header[] array = mailItem.RootPart.Headers.FindAll("X-Auto-Response-Suppress");
				if (array.Length == 0)
				{
					Header newChild = new AsciiTextHeader("X-Auto-Response-Suppress", ResolverMessage.FormatAutoResponseSuppressHeaderValue(suppress));
					mailItem.RootPart.Headers.AppendChild(newChild);
					return;
				}
				array[0].Value = ResolverMessage.FormatAutoResponseSuppressHeaderValue(suppress);
				for (int i = 1; i < array.Length; i++)
				{
					mailItem.RootPart.Headers.RemoveChild(array[i]);
				}
			}
		}

		private static void AddRecipientsToTransportMailItem(List<RoutingAddress> targetRecipients, TransportMailItem unwrappedMailItem)
		{
			if (unwrappedMailItem.Recipients != null)
			{
				unwrappedMailItem.Recipients.Clear();
				foreach (RoutingAddress routingAddress in targetRecipients)
				{
					unwrappedMailItem.Recipients.Add(routingAddress.ToString());
				}
			}
		}

		private static List<AddressInfo> ConvertAndGetAddressInfoList(List<RoutingAddress> list)
		{
			List<AddressInfo> list2 = new List<AddressInfo>(list.Count);
			foreach (RoutingAddress address in list)
			{
				AddressInfo item = new AddressInfo(address);
				list2.Add(item);
			}
			return list2;
		}

		private static void ApplyHeaderFirewall(HeaderList headers)
		{
			HeaderFirewall.Filter(headers, ~RestrictedHeaderSet.MTA);
		}

		private static void NdrAllRecipients(MailItem mailItem, SmtpResponse smtpResponse)
		{
			int count = mailItem.Recipients.Count;
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			for (int i = count - 1; i >= 0; i--)
			{
				recipients.Remove(recipients[i], DsnType.Failure, smtpResponse);
			}
		}

		private static UnjournalRecipientType GetRecipientType(MailItem mailItem, string email)
		{
			IADRecipientCache iadrecipientCache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			if (ProxyAddressBase.IsAddressStringValid(email) && RoutingAddress.IsValidAddress(email))
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(email, true);
				Result<ADRawEntry> result = default(Result<ADRawEntry>);
				result = iadrecipientCache.FindAndCacheRecipient(proxyAddress);
				if (result.Data == null || result.Error == ProviderError.NotFound)
				{
					return UnjournalRecipientType.External;
				}
				object obj;
				if (result.Data.TryGetValueWithoutDefault(ADRecipientSchema.RecipientType, out obj))
				{
					if (obj != null && obj is Microsoft.Exchange.Data.Directory.Recipient.RecipientType)
					{
						Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)obj;
						if (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox)
						{
							return UnjournalRecipientType.Mailbox;
						}
						if (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup)
						{
							return UnjournalRecipientType.DistributionGroup;
						}
					}
					return UnjournalRecipientType.ResolvedOther;
				}
			}
			return UnjournalRecipientType.External;
		}

		private static RoutingAddress GetJournalArchiveAddress(MailItem mailItem, string email)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			string text = string.Empty;
			IADRecipientCache iadrecipientCache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			if (ProxyAddressBase.IsAddressStringValid(email) && RoutingAddress.IsValidAddress(email))
			{
				SmtpProxyAddress proxyAddress = new SmtpProxyAddress(email, true);
				Result<ADRawEntry> result = default(Result<ADRawEntry>);
				result = iadrecipientCache.FindAndCacheRecipient(proxyAddress);
				if (result.Data == null || result.Error == ProviderError.NotFound)
				{
					text = string.Format("UnWrapJournalAgent: GetJournalArchiveAddress, message id = {0} , ADlookup failed or found zero results for email address of {1} ", arg, email);
					ExTraceGlobals.JournalingTracer.TraceDebug(0L, text);
					UnwrapJournalAgent.PublishMonitoringResults(mailItem, email, text);
					return RoutingAddress.Empty;
				}
				ADPropertyDefinition[] extraProperties = new ADPropertyDefinition[]
				{
					MiniRecipientSchema.JournalArchiveAddress
				};
				object obj;
				if (iadrecipientCache.ReloadRecipient(proxyAddress, extraProperties).Data.TryGetValueWithoutDefault(MiniRecipientSchema.JournalArchiveAddress, out obj))
				{
					if (obj != null && obj is SmtpAddress)
					{
						SmtpAddress smtpAddress = (SmtpAddress)obj;
						if (smtpAddress != SmtpAddress.Empty && smtpAddress.IsValidAddress && smtpAddress != SmtpAddress.NullReversePath)
						{
							return new RoutingAddress(smtpAddress.ToString());
						}
						text = string.Format("For message id = {0} , Loaded journalarchiveaddress property of {1} from transport cache after AD lookup completed, value returned is empty/invalid/nullreverseaddress: {2}", arg, email, smtpAddress);
						ExTraceGlobals.JournalingTracer.TraceDebug(0L, text);
					}
					else
					{
						text = string.Format("For message id = {0} , Loaded journalarchiveaddress property of {1} from transport cache after AD lookup completed, but value returned is null or datatype mismatches : {2}", arg, email, obj);
						ExTraceGlobals.JournalingTracer.TraceDebug(0L, text);
					}
				}
				else
				{
					text = string.Format("For message id = {0} , Failed to load journalarchiveaddress property of {1} from transport cache after AD lookup completed", arg, email);
					ExTraceGlobals.JournalingTracer.TraceDebug(0L, text);
				}
			}
			UnwrapJournalAgent.PublishMonitoringResults(mailItem, email, text);
			return RoutingAddress.Empty;
		}

		private static void AddToListWithDuplicateCheck(List<RoutingAddress> destinationList, RoutingAddress address)
		{
			if (!destinationList.Contains(address))
			{
				destinationList.Add(address);
			}
		}

		private static List<AddressInfo> GetJournalArchiveAddresses(EnvelopeJournalReport journalReport, MailItem mailItem, int hashCode)
		{
			List<RoutingAddress> list = new List<RoutingAddress>();
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)hashCode, "UnJournalAgent: GetJournalArchiveAddresses, message id = {0} ", arg);
			foreach (AddressInfo addressInfo in journalReport.Recipients)
			{
				UnwrapJournalAgent.CheckJournalArchiveAddressAndInsert((string)addressInfo.Address, mailItem, list);
			}
			RoutingAddress journalArchiveAddress = UnwrapJournalAgent.GetJournalArchiveAddress(mailItem, (string)journalReport.EnvelopeSender.Address);
			if (journalArchiveAddress != RoutingAddress.Empty && journalArchiveAddress != RoutingAddress.NullReversePath)
			{
				journalReport.SenderJournalArchiveAddress = new AddressInfo(journalArchiveAddress);
			}
			else
			{
				journalReport.SenderJournalArchiveAddress = new AddressInfo(RoutingAddress.NullReversePath);
			}
			return UnwrapJournalAgent.ConvertAndGetAddressInfoList(list);
		}

		private static void CheckJournalArchiveAddressAndInsert(string recipientAddress, MailItem mailItem, List<RoutingAddress> journalArchiveAddresses)
		{
			RoutingAddress journalArchiveAddress = UnwrapJournalAgent.GetJournalArchiveAddress(mailItem, recipientAddress);
			if (journalArchiveAddress != RoutingAddress.Empty && journalArchiveAddress != RoutingAddress.NullReversePath)
			{
				UnwrapJournalAgent.AddToListWithDuplicateCheck(journalArchiveAddresses, journalArchiveAddress);
			}
		}

		private static string GetPrimarySmtpAddress(MailItem mailItem, RoutingAddress email)
		{
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = (ADRecipientCache<TransportMiniRecipient>)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			if (email.IsValid)
			{
				ProxyAddress proxyAddress = new SmtpProxyAddress(email.ToString(), true);
				Result<TransportMiniRecipient> result = adrecipientCache.FindAndCacheRecipient(proxyAddress);
				if (result.Data != null && result.Error != ProviderError.NotFound)
				{
					string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(result.Data);
					if (!string.IsNullOrEmpty(primarySmtpAddress))
					{
						return primarySmtpAddress;
					}
				}
			}
			return null;
		}

		private static List<RoutingAddress> FindUnprovisionedRecipients(EnvelopeJournalReport journalReport, MailItem mailItem)
		{
			List<RoutingAddress> list = new List<RoutingAddress>();
			if (journalReport == null || journalReport.ExternalOrUnprovisionedRecipients == null)
			{
				throw new ArgumentNullException("Must specify the list of unprovisioned and external recipients ");
			}
			if (journalReport.ExternalOrUnprovisionedRecipients.Count > 0)
			{
				foreach (RoutingAddress routingAddress in journalReport.ExternalOrUnprovisionedRecipients)
				{
					if (UnwrapJournalAgent.IsUserInOrg(mailItem, routingAddress))
					{
						list.Add(routingAddress);
					}
				}
			}
			if (!journalReport.IsSenderInternal && journalReport.Sender.RecipientType != UnjournalRecipientType.DistributionGroup && UnwrapJournalAgent.IsUserInOrg(mailItem, journalReport.Sender.Address))
			{
				list.Add(journalReport.Sender.Address);
			}
			return list;
		}

		private static bool IsUserInOrg(MailItem mailItem, RoutingAddress targetAddress)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			OrganizationId orgId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			PerTenantAcceptedDomainTable acceptedDomainTable = Components.Configuration.GetAcceptedDomainTable(orgId);
			bool result = false;
			if (targetAddress != RoutingAddress.NullReversePath)
			{
				result = (acceptedDomainTable.AcceptedDomainTable.GetDomainEntry(SmtpDomain.Parse(targetAddress.DomainPart)) != null);
			}
			ExTraceGlobals.JournalingTracer.TraceDebug<string, string, string>(0L, "UnJournalAgent: MessageId: {0}, TargetAddress {1}. IsUserInOrg returns {2}. Target org compared with current org.", arg, targetAddress.ToString(), result.ToString());
			return result;
		}

		private static StringBuilder GetEHAInformation(MailItem mailItem)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(",SND=<");
			stringBuilder.Append(mailItem.FromAddress);
			stringBuilder.Append(">");
			string value = string.Empty;
			if (1 == mailItem.Recipients.Count)
			{
				value = mailItem.Recipients[0].Address.ToString();
			}
			else
			{
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					MailRecipientWrapper mailRecipientWrapper = (MailRecipientWrapper)envelopeRecipient;
					if (UnwrapJournalAgent.IsUserInOrg(mailItem, mailRecipientWrapper.Address))
					{
						value = mailRecipientWrapper.Address.ToString();
						break;
					}
				}
			}
			stringBuilder.Append(",RCP=<");
			stringBuilder.Append(value);
			stringBuilder.Append(">");
			HeaderList headerList = null;
			if (mailItem != null && mailItem.Message != null && mailItem.Message.RootPart != null)
			{
				headerList = mailItem.Message.RootPart.Headers;
			}
			stringBuilder.Append(",RBS=<");
			stringBuilder.Append(EnvelopeJournalUtility.ReadHeaderValueWithDefault<int>(headerList, "X-MS-EHA-ConfirmBatchSize", 1000, new EnvelopeJournalUtility.Parser<int>(int.TryParse)));
			stringBuilder.Append(">");
			stringBuilder.Append(",RTO=<");
			stringBuilder.Append(EnvelopeJournalUtility.ReadHeaderValueWithDefault<int>(headerList, "X-MS-EHA-ConfirmTimeout", 3600, new EnvelopeJournalUtility.Parser<int>(int.TryParse)));
			stringBuilder.Append(">");
			stringBuilder.Append(",RXD=<");
			stringBuilder.Append(EnvelopeJournalUtility.ReadHeaderValueWithDefault<DateTime>(headerList, "X-MS-EHA-MessageExpiryDate", MessageConstants.EHAJournalHeaderDefaults.DefaultRetainUntilDate, new EnvelopeJournalUtility.Parser<DateTime>(DateTime.TryParse)));
			stringBuilder.Append(">");
			stringBuilder.Append(",RID=<");
			stringBuilder.Append(UnwrapJournalAgent.GetEhaMessageIdHeader(mailItem));
			stringBuilder.Append(">");
			stringBuilder.Append(",ExtOrgId=<");
			stringBuilder.Append(UnwrapJournalAgent.GetExternalOrganizationId(mailItem));
			stringBuilder.Append(">");
			return stringBuilder;
		}

		private static string GetExternalOrganizationId(MailItem mailItem)
		{
			IReadOnlyMailItem readOnlyMailItem = (IReadOnlyMailItem)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			return readOnlyMailItem.ExternalOrganizationId.ToString();
		}

		private static string GetEhaMessageIdHeader(MailItem mailItem)
		{
			string text = string.Empty;
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			if (mailItem.Message != null && mailItem.Message.RootPart != null)
			{
				HeaderList headers = mailItem.Message.RootPart.Headers;
				Header header = headers.FindFirst("X-MS-EHAMessageID");
				if (header != null)
				{
					text = ((header.Value == null) ? string.Empty : header.Value);
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>(0L, "UnJournalAgent: MessageId: {0} This journal message originated from EHA migration. It has EHA message id header of {1} on it.", arg, text);
				}
				else
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "UnJournalAgent: MessageId: {0} This journal message originated from live journal EHA traffic. EHA message id header not found.", arg);
				}
			}
			return text;
		}

		private static void PublishMonitoringResults(MailItem mailItem, string email, string errorMessage)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			OrganizationId organizationId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
			string str = string.Empty;
			if (organizationId != null && organizationId.OrganizationalUnit != null)
			{
				str = organizationId.OrganizationalUnit.Name;
			}
			Guid guid = Guid.Empty;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			if (transportMailItem2 != null && !transportMailItem2.IsRowDeleted)
			{
				guid = transportMailItem2.ExternalOrganizationId;
			}
			new EventNotificationItem(ExchangeComponent.JournalArchive.Name, "JournalArchiveComponent", string.Empty, ResultSeverityLevel.Error)
			{
				StateAttribute1 = guid.ToString() + " " + str,
				StateAttribute2 = (string.IsNullOrEmpty(errorMessage) ? string.Empty : errorMessage)
			}.Publish(false);
		}

		private void SetPrioritization(ITransportMailItemFacade tmi, MailItem mailItem)
		{
			if (this.IsMessageSourceEhaMigration(mailItem))
			{
				UnwrapJournalAgent.SetPrioritization(tmi, true);
				return;
			}
			UnwrapJournalAgent.SetPrioritization(tmi, false);
		}

		private void SubmitMessage(SubmittedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (source == null || args == null)
			{
				throw new ArgumentNullException("internal transport error");
			}
			if (!this.isEnabled)
			{
				return;
			}
			if (!this.isEHAJournalingEnabled)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "UnJournalAgent: This is not DC, agent is not enabled.");
				return;
			}
			this.timer.Reset();
			this.timer.Start();
			MailItem mailItem = args.MailItem;
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string text = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} SubmitMessage Invoked.", text);
			using (JournalLogContext journalLogContext = new JournalLogContext("UJA", "OnSubmittedMessage", mailItem.Message.MessageId, mailItem))
			{
				OrganizationId organizationId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
				this.configuration = ArchiveJournalTenantConfiguration.GetTenantConfig(organizationId);
				if (this.configuration == null)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Unable to retrieve archive journaling configuration.", text);
					journalLogContext.LogAsSkipped("Cfg", new object[0]);
				}
				else
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Retrieved archive journaling configuration.", text);
					ProcessingStatus processingStatus;
					if (this.configuration.LegacyArchiveJournalingEnabled || this.configuration.LegacyArchiveLiveJournalingEnabled || this.configuration.JournalArchivingEnabled)
					{
						ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} archive journaling enabled.", text);
						if (!this.IfJournalAlreadyProcessed(mailItem))
						{
							processingStatus = this.ProcessJournalMessage(source, mailItem, organizationId, journalLogContext);
							ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Message processed , status = {1}.", text, processingStatus.ToString());
						}
						else
						{
							processingStatus = ProcessingStatus.AlreadyProcessed;
							journalLogContext.LogAsSkipped("APed", new object[0]);
							ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Already processed message.", text);
						}
					}
					else
					{
						ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Legacy archive journaling not enabled.", text);
						journalLogContext.LogAsSkipped("LAJDis", new object[0]);
						processingStatus = ProcessingStatus.LegacyArchiveJournallingDisabled;
					}
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Final ProcessingStatus {1}", text, processingStatus.ToString());
					switch (processingStatus)
					{
					case ProcessingStatus.PermanentError:
						this.MarkProcessingDoneForJournalNdr(mailItem);
						this.UpdateProcessingTime();
						break;
					case ProcessingStatus.TransientError:
					case ProcessingStatus.LegacyArchiveJournallingDisabled:
						this.UpdateProcessingTime();
						break;
					case ProcessingStatus.UnwrapProcessSuccess:
					case ProcessingStatus.DropJournalReportWithoutNdr:
					case ProcessingStatus.NoUsersResolved:
						this.DropJournalEnvelop(source, text);
						this.UpdateProcessingTime();
						break;
					case ProcessingStatus.NdrProcessSuccess:
						this.MarkProcessingDoneForJournalNdr(mailItem);
						this.UpdateProcessingTime();
						break;
					case ProcessingStatus.NonJournalMsgFromLegacyArchiveCustomer:
					case ProcessingStatus.AlreadyProcessed:
						this.UpdateProcessingTime();
						break;
					case ProcessingStatus.NdrJournalReport:
						UnwrapJournalAgent.NdrAllRecipients(mailItem, SmtpResponse.InvalidContent);
						this.UpdateProcessingTime();
						break;
					}
					this.UpdatePerformanceCounters(processingStatus);
				}
			}
		}

		private void DropJournalEnvelop(SubmittedMessageEventSource source, string messageId)
		{
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Delete the journal message.", messageId);
			source.Delete();
		}

		private void LogEHAResults(StringBuilder logResponse, ProcessingStatus processingStatus, int unprovisionedCount, int distributionGroupCount, bool permanentError, bool noUsersResolved)
		{
			if (ProcessingStatus.UnwrapProcessSuccess == processingStatus || ProcessingStatus.NdrProcessSuccess == processingStatus || ProcessingStatus.NoUsersResolved == processingStatus || ProcessingStatus.DropJournalReportWithoutNdr == processingStatus || ProcessingStatus.PermanentError == processingStatus)
			{
				logResponse.Append(",status=<");
				logResponse.Append(processingStatus.ToString());
				logResponse.Append(">");
				if (permanentError)
				{
					logResponse.Append(",<permanenterror>");
				}
				else if (noUsersResolved)
				{
					logResponse.Append(",<nousersresolved>");
				}
				else if (unprovisionedCount > 0 && distributionGroupCount > 0)
				{
					logResponse.Append(",<unprovisionedanddistributionlistusers>");
				}
				else if (unprovisionedCount > 0)
				{
					logResponse.Append(",<unprovisionedusers>");
				}
				else if (distributionGroupCount > 0)
				{
					logResponse.Append(",<distributiongroup>");
				}
				lock (UnwrapJournalAgent.syncObject)
				{
					if (UnwrapJournalAgent.logger == null)
					{
						UnwrapJournalAgent.logger = new UnwrapJournalAgent.EHAUnwrapJournalAgentLog();
					}
				}
				UnwrapJournalAgent.logger.Append(logResponse);
				logResponse.Length = 0;
			}
		}

		private bool IsMessageSourceEhaMigration(MailItem mailItem)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			if (mailItem.Message == null || mailItem.Message.RootPart == null)
			{
				return false;
			}
			HeaderList headers = mailItem.Message.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-EHA-MessageExpiryDate");
			if (header != null)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This journal message originated from EHA migration. It has EHA header {1} on it.", arg, "X-MS-EHA-MessageExpiryDate".ToString());
				return true;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This journal message originated from live journal EHA traffic. EHA header {1} not found.", arg, "X-MS-EHA-MessageExpiryDate".ToString());
			return false;
		}

		private void StampReceiveAndExpiryTimes(TransportMailItem mailItem, string receiveTime, string expiryTime)
		{
			if (!string.IsNullOrEmpty(receiveTime))
			{
				this.AddHeaderToMessage(mailItem, "X-MS-Exchange-Organization-Unjournal-OriginalReceiveDate", receiveTime);
			}
			if (!string.IsNullOrEmpty(expiryTime))
			{
				this.AddHeaderToMessage(mailItem, "X-MS-Exchange-Organization-Unjournal-OriginalExpiryDate", expiryTime);
			}
		}

		private string GetExpiryTime(TransportMailItem mailItem)
		{
			string arg = (mailItem == null) ? string.Empty : mailItem.MsgId.ToString();
			if (mailItem.Message != null && mailItem.Message.RootPart != null)
			{
				HeaderList headers = mailItem.Message.RootPart.Headers;
				Header header = headers.FindFirst("X-MS-EHA-MessageExpiryDate");
				ExDateTime exDateTime;
				if (header != null && !string.IsNullOrEmpty(header.Value) && ExDateTime.TryParse(header.Value, out exDateTime))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Expiry date found {1}.", arg, exDateTime.ToString());
					return exDateTime.ToString();
				}
			}
			return null;
		}

		private string GetOriginalReceiveTime(TransportMailItem mailItem)
		{
			string arg = (mailItem == null) ? string.Empty : mailItem.MsgId.ToString();
			if (mailItem.Message != null && mailItem.Message.RootPart != null)
			{
				HeaderList headers = mailItem.Message.RootPart.Headers;
				Header header = headers.FindFirst("X-MS-EHA-MessageDate");
				ExDateTime exDateTime;
				if (header != null && !string.IsNullOrEmpty(header.Value) && ExDateTime.TryParse(header.Value, out exDateTime))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Original Recieved date found {1}.", arg, exDateTime.ToString());
					return exDateTime.ToString();
				}
			}
			return null;
		}

		private void MarkProcessingDone(TransportMailItem transportItem)
		{
			string arg = (transportItem == null) ? string.Empty : transportItem.MsgId.ToString();
			this.perfCountersWrapper.Increment(PerfCounters.MessagesUnjournaled, 1L);
			this.AddHeaderToMessage(transportItem, "X-MS-Exchange-Organization-Unjournal-Processed", string.Empty);
			ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} MarkProcessingDone with {1}.", arg, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmitted");
		}

		private void MarkProcessingDoneForJournalNdr(MailItem mailItem)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			this.SetPrioritization(transportMailItem, mailItem);
			mailItem.Properties["Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmittedForJournalNdr"] = true;
			this.AddHeaderToMessage(transportMailItem2, "X-MS-Exchange-Organization-Unjournal-ProcessedNdr", string.Empty);
			ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} MarkProcessingDoneForJournalNdr with {1}.", arg, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmittedForJournalNdr");
		}

		private void UpdateProcessingTime()
		{
			this.timer.Stop();
			long elapsedMilliseconds = this.timer.ElapsedMilliseconds;
			ExTraceGlobals.JournalingTracer.TraceDebug<long>((long)this.GetHashCode(), "UnJournalAgent{0} ms to journal message", elapsedMilliseconds);
			PerfCounters.ProcessingTime.IncrementBy(elapsedMilliseconds);
		}

		private bool IfJournalAlreadyProcessed(MailItem mailItem)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			if (this.HeaderExists(mailItem, "X-MS-Exchange-Organization-Unjournal-Processed"))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Message was already processed OnSubmitted, skipping: {1}", arg, mailItem.Message.MessageId);
				return true;
			}
			return false;
		}

		private bool IfJournalAlreadyProcessedForNDR(MailItem mailItem)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			if (this.HeaderExists(mailItem, "X-MS-Exchange-Organization-Unjournal-ProcessedNdr"))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Message was already processed OnSubmitted, skipping: {1}", arg, mailItem.Message.MessageId);
				return true;
			}
			return false;
		}

		private bool HeaderExists(MailItem mailItem, string headerName)
		{
			return mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(headerName) != null;
		}

		private bool IfInternalJournalReport(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			if (transportMailItem2 == null)
			{
				return false;
			}
			if (this.HeaderExists(mailItem, "X-MS-InternalJournal"))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<long, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is an internal journal report generated by the journal agent in DC for an EHA customer, this should not be unjournaled: {1}", transportMailItem2.MsgId, mailItem.Message.MessageId);
				return true;
			}
			return false;
		}

		private ProcessingStatus ProcessJournalMessage(SubmittedMessageEventSource source, MailItem mailItem, OrganizationId orgId, JournalLogContext logContext)
		{
			ProcessingStatus processingStatus = ProcessingStatus.NotDone;
			List<EnvelopeRecipient> recipientList = mailItem.Recipients.ToList<EnvelopeRecipient>();
			RoutingAddress fromAddress = mailItem.FromAddress;
			UnwrapJournalAgent.OriginalMailItemInfo originalMailItem = new UnwrapJournalAgent.OriginalMailItemInfo(recipientList, fromAddress);
			Exception ex = null;
			Exception ex2 = null;
			FailureMessageType failureMessageType = FailureMessageType.Unknown;
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string text = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			StringBuilder stringBuilder = null;
			int unprovisionedCount = 0;
			int distributionGroupCount = 0;
			try
			{
				EnvelopeJournalVersion envelopeJournalVersion = EnvelopeJournalUtility.CheckEnvelopeJournalVersion(mailItem.Message);
				if (envelopeJournalVersion == EnvelopeJournalVersion.None)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is not a journal report.", text);
					processingStatus = ProcessingStatus.NonJournalMsgFromLegacyArchiveCustomer;
					logContext.LogAsSkipped("NJMFLAC", new object[0]);
				}
				else if (this.isEHAJournalingEnabled && this.IfInternalJournalReport(mailItem))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is an internal journal report, hence we dont unwrap it.", text);
					processingStatus = ProcessingStatus.AlreadyProcessed;
					logContext.LogAsSkipped("APed", new object[]
					{
						"InJR"
					});
				}
				else if (this.IfJournalAlreadyProcessedForNDR(mailItem))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is already processed for {1} , hence we will not process it again.", text, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmittedForJournalNdr".ToString());
					processingStatus = ProcessingStatus.AlreadyProcessed;
					logContext.LogAsSkipped("APed", new object[]
					{
						"NDR"
					});
				}
				else
				{
					this.perfCountersWrapper.Increment(PerfCounters.MessagesProcessed, 1L);
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is journal report which we should unwrap.", text);
					if (this.IsMessageSourceEhaMigration(mailItem))
					{
						stringBuilder = UnwrapJournalAgent.GetEHAInformation(mailItem);
						this.SetEhaMigrationMailbox(mailItem);
					}
					EnvelopeJournalReport envelopeJournalReport = EnvelopeJournalUtility.ExtractEnvelopeJournalMessage(mailItem.Message, envelopeJournalVersion);
					if (envelopeJournalReport.Defective)
					{
						ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} This is a defective journal report.", text);
						this.perfCountersWrapper.Increment(PerfCounters.DefectiveJournals, 1L);
						logContext.AddParameter("Def", new object[]
						{
							envelopeJournalReport.Defective
						});
					}
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Updating recipients of the journal report.", text);
					List<RoutingAddress> list = this.ProcessRecipients(mailItem, envelopeJournalReport);
					this.SetPrioritization(transportMailItem, mailItem);
					if (list.Count > 0)
					{
						this.ProcessEmbeddedMessageInJournal(source, mailItem, envelopeJournalReport, list, logContext, out unprovisionedCount, out distributionGroupCount, out processingStatus);
						string key = "UnRec";
						object[] array = new object[1];
						array[0] = from x in list
						select x.ToString();
						logContext.AddParameter(key, array);
						ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} The journal report was unwrapped successfully.", text);
					}
					else
					{
						this.ProcessCauseOfFailureAndSendJournalReportAsIs(source, mailItem, envelopeJournalReport, logContext, out unprovisionedCount, out distributionGroupCount, out processingStatus);
						ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} There are no mail recipients, the journal report was sent to journal ndr mailbox.", text);
					}
					logContext.AddParameter("PSt", new object[]
					{
						processingStatus
					});
				}
			}
			catch (InvalidEnvelopeJournalMessageException ex3)
			{
				ex2 = ex3;
				failureMessageType = FailureMessageType.UnexpectedJournalMessageFormatMsg;
			}
			catch (ADTransientException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			catch (DataValidationException ex6)
			{
				ex2 = ex6;
				failureMessageType = FailureMessageType.PermanentErrorOther;
			}
			catch (ExchangeDataException ex7)
			{
				ex2 = ex7;
				failureMessageType = FailureMessageType.PermanentErrorOther;
			}
			if (ex != null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Putting this message into retry, as there was an error during unjournaling: {0}.", ex.ToString());
				UnwrapJournalAgent.OriginalMailItemInfo.ResetSenderRecipientInMailItem(mailItem, originalMailItem);
				source.Defer(UnwrapJournalGlobals.RetryIntervalOnError);
				processingStatus = ProcessingStatus.TransientError;
				UnwrapJournalGlobals.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_UnJournalingTransientError, null, new object[]
				{
					text,
					orgId,
					ex2
				});
				logContext.LogAsRetriableError(new object[]
				{
					processingStatus,
					ex
				});
			}
			if (ex2 != null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Journal report could not be unjournaled. Got an exception during processing: {0}", ex2.ToString());
				if (this.configuration.DropJournalsWithPermanentErrors)
				{
					if (this.IsMessageSourceEhaMigration(mailItem))
					{
						processingStatus = ProcessingStatus.DropJournalReportWithoutNdr;
					}
					else
					{
						processingStatus = ProcessingStatus.NdrJournalReport;
					}
				}
				else
				{
					processingStatus = ProcessingStatus.PermanentError;
					this.SendReportAsIsForPermanentError(mailItem, failureMessageType);
				}
				logContext.LogAsFatalError(new object[]
				{
					processingStatus,
					failureMessageType,
					ex2
				});
				UnwrapJournalGlobals.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_UnJournalingPermanentError, null, new object[]
				{
					text,
					orgId,
					ex2
				});
			}
			if (stringBuilder != null)
			{
				this.LogEHAResults(stringBuilder, processingStatus, unprovisionedCount, distributionGroupCount, ex2 != null, processingStatus == ProcessingStatus.NoUsersResolved);
			}
			return processingStatus;
		}

		private void SetEhaMigrationMailbox(MailItem mailItem)
		{
			RoutingAddress routingAddress = RoutingAddress.NullReversePath;
			if (1 == mailItem.Recipients.Count)
			{
				routingAddress = mailItem.Recipients[0].Address;
			}
			else
			{
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					MailRecipientWrapper mailRecipientWrapper = (MailRecipientWrapper)envelopeRecipient;
					if (mailRecipientWrapper.Address.LocalPart.ToLower().Contains("ehamigrationmailbox"))
					{
						routingAddress = mailRecipientWrapper.Address;
						break;
					}
				}
			}
			if (routingAddress.IsValid && routingAddress != RoutingAddress.NullReversePath)
			{
				this.configuration.EhaMigrationMailboxAddress = routingAddress;
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Eha migration mailbox address found : {0}", routingAddress.ToString());
				return;
			}
			this.configuration.EhaMigrationMailboxAddress = this.configuration.MSExchangeRecipient;
			ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Eha migration mailbox address NOT found, reseting migration mailbox to MSExchangeRecipientaddress : {0}", this.configuration.MSExchangeRecipient.ToString());
		}

		private void SendReportAsIsForPermanentError(MailItem mailItem, FailureMessageType messageType)
		{
			this.SetJournalNdrAddress(mailItem);
			this.PrepareDeliveryMessageForNdrReport(mailItem, messageType, new List<RoutingAddress>(), new List<RoutingAddress>());
		}

		private void ProcessEmbeddedMessageInJournal(SubmittedMessageEventSource source, MailItem mailItem, EnvelopeJournalReport journalReport, List<RoutingAddress> targetRecipients, JournalLogContext logContext, out int unprovisionedCount, out int distributionGroupsCount, out ProcessingStatus processingStatus)
		{
			processingStatus = ProcessingStatus.NotDone;
			unprovisionedCount = 0;
			distributionGroupsCount = 0;
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string text = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			bool flag = this.IsMessageSourceEhaMigration(mailItem);
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Processing embedded message in the journal report.", text);
			if (flag)
			{
				this.ProcessEhaMessageIDHeader(journalReport, transportMailItem2);
			}
			if (journalReport.Defective)
			{
				if (this.configuration.DropJournalsWithPermanentErrors)
				{
					processingStatus = ProcessingStatus.NdrJournalReport;
					logContext.AddParameter("Def", new object[]
					{
						"DropJR"
					});
				}
				else
				{
					this.SetJournalNdrAddress(mailItem);
					List<RoutingAddress> list = new List<RoutingAddress>();
					list = (from x in journalReport.Recipients
					select x.Address).ToList<RoutingAddress>();
					if (journalReport.Sender != null)
					{
						list.Add(journalReport.Sender.Address);
					}
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Preparing deliver message for defective journal to ndr report.", text);
					this.PrepareDeliveryMessageForNdrReport(mailItem, FailureMessageType.DefectiveJournalWithRecipientsMsg, list, new List<RoutingAddress>());
					ExTraceGlobals.JournalingTracer.TraceDebug<string, int>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Forking a copy to ndr mailbox. Original Recipients Count = {1}.", text, targetRecipients.Count);
					processingStatus = ProcessingStatus.NdrProcessSuccess;
					logContext.AddParameter("Ndr", new object[]
					{
						FailureMessageType.DefectiveJournalWithRecipientsMsg
					});
				}
			}
			else
			{
				List<RoutingAddress> list2;
				List<RoutingAddress> list3;
				this.GetUnProvisionedUsersAndDLsForDelivery(journalReport, mailItem, out list2, out list3);
				if (list2.Count > 0 || list3.Count > 0)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Found unprovisioned users count {1} or DLs. Count = {2}.", text, list2.Count, list3.Count);
					unprovisionedCount = list2.Count;
					distributionGroupsCount = list3.Count;
					this.SetJournalNdrAddress(mailItem);
					ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Preparing deliver message for ndr report.", text);
					this.PrepareDeliveryMessageForNdrReport(mailItem, FailureMessageType.UnProvisionedRecipientsMsg, list2, list3);
					ExTraceGlobals.JournalingTracer.TraceDebug<string, int>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Forking a copy to ndr mailbox. Original Recipients Count = {1}.", text, targetRecipients.Count);
					processingStatus = ProcessingStatus.NdrProcessSuccess;
					string key = "Ndr";
					object[] array = new object[5];
					array[0] = FailureMessageType.UnProvisionedRecipientsMsg;
					array[1] = unprovisionedCount;
					array[2] = from x in list2
					select x.ToString();
					array[3] = distributionGroupsCount;
					array[4] = from x in list3
					select x.ToString();
					logContext.AddParameter(key, array);
				}
			}
			if (this.CreateEmbeddedMessageAndResubmit(targetRecipients, transportMailItem2, journalReport, flag, text))
			{
				if (processingStatus == ProcessingStatus.NotDone)
				{
					processingStatus = ProcessingStatus.UnwrapProcessSuccess;
				}
				this.TrackAgentInfo(source, targetRecipients);
				return;
			}
			throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessageAttachment(text));
		}

		private void GetUnProvisionedUsersAndDLsForDelivery(EnvelopeJournalReport journalReport, MailItem mailItem, out List<RoutingAddress> unprovisionedUsersList, out List<RoutingAddress> distributionGroups)
		{
			unprovisionedUsersList = new List<RoutingAddress>();
			distributionGroups = new List<RoutingAddress>();
			if (!this.configuration.DropUnprovisionedUsersMessages)
			{
				List<RoutingAddress> list = UnwrapJournalAgent.FindUnprovisionedRecipients(journalReport, mailItem);
				if (list != null && list.Count > 0)
				{
					unprovisionedUsersList.AddRange(list);
				}
			}
			if (this.configuration.RedirectDistributionListMessages)
			{
				if (journalReport.DistributionLists != null && journalReport.DistributionLists.Count > 0)
				{
					distributionGroups.AddRange(journalReport.DistributionLists);
				}
				if (journalReport.Sender.RecipientType == UnjournalRecipientType.DistributionGroup)
				{
					distributionGroups.Add(journalReport.Sender.Address);
				}
			}
		}

		private bool CreateEmbeddedMessageAndResubmit(List<RoutingAddress> targetRecipients, TransportMailItem journalMailItem, EnvelopeJournalReport journalReport, bool isSourceEha, string messageId)
		{
			bool result = false;
			string ehaExpiryTime = null;
			string ehaReceiveTime = null;
			if (isSourceEha)
			{
				ehaExpiryTime = this.GetExpiryTime(journalMailItem);
				ehaReceiveTime = this.GetOriginalReceiveTime(journalMailItem);
			}
			EmailMessage emailMessage = this.ConvertAttachementToEmailMessage(journalReport.EmbeddedMessageAttachment);
			if (emailMessage != null && emailMessage.MimeDocument != null)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Extracted embedded message.", messageId);
				emailMessage.Normalize(NormalizeOptions.NormalizeMessageId | NormalizeOptions.MergeAddressHeaders | NormalizeOptions.RemoveDuplicateHeaders, false);
				UnwrapJournalAgent.ApplyHeaderFirewall(emailMessage.MimeDocument.RootPart.Headers);
				RoutingAddress from;
				if (isSourceEha)
				{
					from = this.configuration.EhaMigrationMailboxAddress;
				}
				else
				{
					from = journalMailItem.From;
				}
				TransportMailItem transportMailItem = UnwrapJournalAgent.CreateNewTransportMailItem(journalMailItem, emailMessage, journalMailItem.OrganizationId, "Unwrap Journal Agent", "submit", journalReport.EnvelopeSender.Address);
				this.UpdateTransportMailItemWithCustomPropertiesHeaders(from, targetRecipients, transportMailItem, journalMailItem, journalReport, ehaReceiveTime, ehaExpiryTime, isSourceEha, messageId);
				UnwrapJournalAgent.CommitTrackAndEnqueue(journalMailItem, transportMailItem, "Unwrap Journal Agent");
				ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Unwrapped mailitem submitted in the queue successfully.", messageId);
				result = true;
				PerfCounters.TotalMessagesUnjournaledSize.IncrementBy(transportMailItem.MimeSize);
				this.perfCountersWrapper.Increment(PerfCounters.UsersUnjournaled, (long)targetRecipients.Count);
			}
			else
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Unwrapped mailitem or embedded email message is null.", messageId);
			}
			return result;
		}

		private void UpdateTransportMailItemWithCustomPropertiesHeaders(RoutingAddress from, List<RoutingAddress> recipients, TransportMailItem newTransportMailItem, TransportMailItem parentMailItem, EnvelopeJournalReport journalReport, string ehaReceiveTime, string ehaExpiryTime, bool isSourceEha, string messageId)
		{
			newTransportMailItem.From = from;
			UnwrapJournalAgent.AddRecipientsToTransportMailItem(recipients, newTransportMailItem);
			UnwrapJournalAgent.SetPrioritization(newTransportMailItem, isSourceEha);
			this.ApplyUnjournalHeaders(newTransportMailItem, journalReport, isSourceEha, ehaReceiveTime, ehaExpiryTime);
			this.MarkProcessingDone(newTransportMailItem);
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Unwrapped mailitem applied header firewall and applied unjournal headers.", messageId);
		}

		private void ApplyUnjournalHeaders(TransportMailItem unwrappedMailItem, EnvelopeJournalReport journalReport, bool isSourceEha, string ehaReceiveTime, string ehaExpiryTime)
		{
			this.AddExchangeOrganizationBccHeader(journalReport, unwrappedMailItem);
			this.ProcessSender(journalReport, unwrappedMailItem);
			this.ProcessIsSenderARecipient(journalReport, unwrappedMailItem);
			this.RemoveDispositionNotificationHeaderAndExpiryDates(unwrappedMailItem);
			this.CheckAndAddFromHeaderIfMissing(journalReport, unwrappedMailItem);
			this.AddSuppressAutoResponseHeader(unwrappedMailItem);
			if (isSourceEha)
			{
				this.StampReceiveAndExpiryTimes(unwrappedMailItem, ehaReceiveTime, ehaExpiryTime);
				this.ProcessEhaMessageIDHeader(journalReport, unwrappedMailItem);
			}
		}

		private void AddSuppressAutoResponseHeader(TransportMailItem mailItem)
		{
			AutoResponseSuppress suppress = AutoResponseSuppress.DR | AutoResponseSuppress.RN | AutoResponseSuppress.NRN | AutoResponseSuppress.OOF | AutoResponseSuppress.AutoReply;
			UnwrapJournalAgent.SetAutoResponseSuppress(mailItem, suppress);
		}

		private void ProcessSender(EnvelopeJournalReport journalReport, TransportMailItem mailItem)
		{
			string headerValue;
			if (string.IsNullOrEmpty(journalReport.Sender.PrimarySmtpAddress))
			{
				headerValue = journalReport.Sender.Address.ToString();
			}
			else
			{
				headerValue = journalReport.Sender.PrimarySmtpAddress;
			}
			this.AddHeaderToMessage(mailItem, "X-MS-Exchange-Organization-Unjournal-SenderAddress", headerValue);
		}

		private void CheckAndAddFromHeaderIfMissing(EnvelopeJournalReport journalReport, TransportMailItem transportItem)
		{
			if (transportItem.MimeDocument.RootPart.Headers.FindFirst(HeaderId.From) == null)
			{
				string email = string.Empty;
				string displayName = string.Empty;
				if (string.IsNullOrEmpty(journalReport.EnvelopeSender.PrimarySmtpAddress))
				{
					email = journalReport.EnvelopeSender.Address.ToString();
				}
				else
				{
					email = journalReport.EnvelopeSender.PrimarySmtpAddress;
				}
				displayName = ((journalReport.EnvelopeSender.FriendlyName == null) ? string.Empty : journalReport.EnvelopeSender.FriendlyName);
				HeaderList headers = transportItem.MimeDocument.RootPart.Headers;
				AddressHeader addressHeader = new AddressHeader(HeaderId.From.ToString());
				MimeRecipient newChild = new MimeRecipient(displayName, email);
				addressHeader.AppendChild(newChild);
				headers.AppendChild(addressHeader);
			}
		}

		private void ProcessEhaMessageIDHeader(EnvelopeJournalReport journalReport, TransportMailItem mailItem)
		{
			this.AddHeaderToMessage(mailItem, "X-MS-EHAMessageID", journalReport.MessageId);
		}

		private void ProcessIsSenderARecipient(EnvelopeJournalReport journalReport, TransportMailItem transportItem)
		{
			string arg = (transportItem == null) ? string.Empty : transportItem.MsgId.ToString();
			string strB;
			if (string.IsNullOrEmpty(journalReport.Sender.PrimarySmtpAddress) || string.Compare(RoutingAddress.NullReversePath.ToString(), journalReport.Sender.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
			{
				strB = journalReport.Sender.Address.ToString();
			}
			else
			{
				strB = journalReport.Sender.PrimarySmtpAddress;
			}
			foreach (AddressInfo addressInfo in journalReport.Recipients)
			{
				if (!string.IsNullOrEmpty(addressInfo.PrimarySmtpAddress) && string.Compare(RoutingAddress.NullReversePath.ToString(), addressInfo.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(addressInfo.PrimarySmtpAddress, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} Sender is also a recipient {1}.", arg, addressInfo.ToString());
					this.AddHeaderToMessage(transportItem, "X-MS-Exchange-Organization-Unjournal-SenderIsRecipient", string.Empty);
				}
			}
		}

		private void RemoveDispositionNotificationHeaderAndExpiryDates(TransportMailItem transportItem)
		{
			if (transportItem != null)
			{
				transportItem.MsgId.ToString();
			}
			else
			{
				string empty = string.Empty;
			}
			if (transportItem.MimeDocument.RootPart != null && transportItem.Message.MimeDocument.RootPart.Headers != null)
			{
				transportItem.Message.MimeDocument.RootPart.Headers.RemoveAll(HeaderId.DispositionNotificationTo);
				transportItem.Message.MimeDocument.RootPart.Headers.RemoveAll(HeaderId.ReturnReceiptTo);
				transportItem.Message.MimeDocument.RootPart.Headers.RemoveAll(HeaderId.ExpiryDate);
				transportItem.Message.MimeDocument.RootPart.Headers.RemoveAll(HeaderId.Expires);
			}
		}

		private void AddHeaderToMessage(TransportMailItem transportItem, string headerName, string headerValue)
		{
			Header header = Header.Create(headerName);
			header.Value = headerValue;
			HeaderList headers = transportItem.MimeDocument.RootPart.Headers;
			headers.AppendChild(header);
		}

		private void AddExchangeOrganizationBccHeader(EnvelopeJournalReport journalReport, TransportMailItem transportItem)
		{
			string arg = (transportItem == null) ? string.Empty : transportItem.MsgId.ToString();
			AddressHeader addressHeader = new AddressHeader("X-MS-Exchange-Organization-BCC");
			foreach (AddressInfo addressInfo in journalReport.Recipients)
			{
				if (addressInfo.IncludedInBcc)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} BCC recipients found, header will be included {1}.", arg, addressInfo.ToString());
					MimeRecipient newChild = new MimeRecipient(addressInfo.FriendlyName, addressInfo.Address.ToString());
					addressHeader.AppendChild(newChild);
				}
			}
			HeaderList headers = transportItem.MimeDocument.RootPart.Headers;
			headers.AppendChild(addressHeader);
		}

		private void SetJournalNdrAddress(MailItem mailItem)
		{
			mailItem.Recipients.Clear();
			if (this.IsMessageSourceEhaMigration(mailItem))
			{
				mailItem.Recipients.Add(this.configuration.EhaMigrationMailboxAddress);
				return;
			}
			mailItem.Recipients.Add(this.configuration.JournalReportNdrTo);
		}

		private void ProcessCauseOfFailureAndSendJournalReportAsIs(SubmittedMessageEventSource source, MailItem mailItem, EnvelopeJournalReport journalReport, JournalLogContext logContext, out int unprovisionedCount, out int distributionGroupsCount, out ProcessingStatus status)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem mailItem2 = transportMailItem as TransportMailItem;
			bool flag = this.IsMessageSourceEhaMigration(mailItem);
			unprovisionedCount = 0;
			distributionGroupsCount = 0;
			status = ProcessingStatus.NotDone;
			if (journalReport != null)
			{
				if (journalReport.Defective)
				{
					if (this.configuration.DropJournalsWithPermanentErrors)
					{
						status = ProcessingStatus.NdrJournalReport;
						logContext.AddParameter("Def", new object[]
						{
							"DropJR"
						});
						return;
					}
					this.SetJournalNdrAddress(mailItem);
					List<RoutingAddress> list = new List<RoutingAddress>();
					list = (from x in journalReport.Recipients
					select x.Address).ToList<RoutingAddress>();
					if (journalReport.EnvelopeSender != null)
					{
						list.Add(journalReport.EnvelopeSender.Address);
					}
					this.PrepareDeliveryMessageForNdrReport(mailItem, FailureMessageType.DefectiveJournalNoRecipientsMsg, list, new List<RoutingAddress>());
					status = ProcessingStatus.NdrProcessSuccess;
					logContext.AddParameter("Ndr", new object[]
					{
						FailureMessageType.DefectiveJournalNoRecipientsMsg
					});
					return;
				}
				else
				{
					List<RoutingAddress> list2;
					List<RoutingAddress> list3;
					this.GetUnProvisionedUsersAndDLsForDelivery(journalReport, mailItem, out list2, out list3);
					if (list2.Count > 0 || list3.Count > 0)
					{
						this.SetJournalNdrAddress(mailItem);
						unprovisionedCount = list2.Count;
						distributionGroupsCount = list3.Count;
						FailureMessageType failureType = FailureMessageType.UnProvisionedRecipientsMsg;
						status = ProcessingStatus.NdrProcessSuccess;
						this.PrepareDeliveryMessageForNdrReport(mailItem, failureType, list2, list3);
						if (flag)
						{
							this.ProcessEhaMessageIDHeader(journalReport, mailItem2);
						}
						string key = "Ndr";
						object[] array = new object[5];
						array[0] = FailureMessageType.UnProvisionedRecipientsMsg;
						array[1] = unprovisionedCount;
						array[2] = from x in list2
						select x.ToString();
						array[3] = distributionGroupsCount;
						array[4] = from x in list3
						select x.ToString();
						logContext.AddParameter(key, array);
						return;
					}
					status = ProcessingStatus.NoUsersResolved;
				}
			}
		}

		private List<RoutingAddress> ProcessRecipients(MailItem mailItem, EnvelopeJournalReport journalReport)
		{
			List<RoutingAddress> list = new List<RoutingAddress>();
			List<AddressInfo> recipientAddressesToProcess = new List<AddressInfo>();
			if (this.configuration.JournalArchivingEnabled && !this.IsMessageSourceEhaMigration(mailItem))
			{
				recipientAddressesToProcess = UnwrapJournalAgent.GetJournalArchiveAddresses(journalReport, mailItem, this.GetHashCode());
			}
			else
			{
				recipientAddressesToProcess = journalReport.Recipients;
			}
			return this.CategorizeJournalRecipients(journalReport, mailItem, recipientAddressesToProcess);
		}

		private List<RoutingAddress> CategorizeJournalRecipients(EnvelopeJournalReport journalReport, MailItem mailItem, List<AddressInfo> recipientAddressesToProcess)
		{
			List<RoutingAddress> list = new List<RoutingAddress>();
			List<RoutingAddress> list2 = new List<RoutingAddress>();
			List<RoutingAddress> list3 = new List<RoutingAddress>();
			List<RoutingAddress> list4 = new List<RoutingAddress>();
			foreach (AddressInfo addressInfo in recipientAddressesToProcess)
			{
				UnjournalRecipientType recipientType = UnwrapJournalAgent.GetRecipientType(mailItem, (string)addressInfo.Address);
				addressInfo.RecipientType = recipientType;
				switch (recipientType)
				{
				case UnjournalRecipientType.Mailbox:
					addressInfo.PrimarySmtpAddress = UnwrapJournalAgent.GetPrimarySmtpAddress(mailItem, addressInfo.Address);
					UnwrapJournalAgent.AddToListWithDuplicateCheck(list2, addressInfo.Address);
					break;
				case UnjournalRecipientType.DistributionGroup:
					UnwrapJournalAgent.AddToListWithDuplicateCheck(list4, addressInfo.Address);
					break;
				case UnjournalRecipientType.ResolvedOther:
				case UnjournalRecipientType.External:
					UnwrapJournalAgent.AddToListWithDuplicateCheck(list3, addressInfo.Address);
					break;
				}
			}
			if (!this.configuration.RedirectDistributionListMessages && list4.Count > 0)
			{
				list.AddRange(list4);
			}
			if (list2.Count > 0)
			{
				list.AddRange(list2);
			}
			journalReport.ExternalOrUnprovisionedRecipients = list3;
			journalReport.DistributionLists = list4;
			this.ProcessJournalReportSender(journalReport, mailItem, list);
			this.ProcessJournalReportRecipients(journalReport, mailItem);
			return list;
		}

		private void ProcessJournalReportRecipients(EnvelopeJournalReport journalReport, MailItem mailItem)
		{
			if (this.configuration.JournalArchivingEnabled && !this.IsMessageSourceEhaMigration(mailItem))
			{
				foreach (AddressInfo addressInfo in journalReport.Recipients)
				{
					RoutingAddress journalArchiveAddress = UnwrapJournalAgent.GetJournalArchiveAddress(mailItem, (string)addressInfo.Address);
					if (journalArchiveAddress != RoutingAddress.Empty && journalArchiveAddress != RoutingAddress.NullReversePath)
					{
						addressInfo.PrimarySmtpAddress = journalArchiveAddress.ToString();
					}
				}
			}
		}

		private void ProcessJournalReportSender(EnvelopeJournalReport journalReport, MailItem mailItem, List<RoutingAddress> targetRecipients)
		{
			string primarySmtpAddress = UnwrapJournalAgent.GetPrimarySmtpAddress(mailItem, journalReport.EnvelopeSender.Address);
			journalReport.EnvelopeSender.PrimarySmtpAddress = primarySmtpAddress;
			UnjournalRecipientType unjournalRecipientType;
			string text;
			if (this.configuration.JournalArchivingEnabled && !this.IsMessageSourceEhaMigration(mailItem))
			{
				if (journalReport.SenderJournalArchiveAddressIsValid)
				{
					unjournalRecipientType = UnwrapJournalAgent.GetRecipientType(mailItem, journalReport.Sender.Address.ToString());
					text = UnwrapJournalAgent.GetPrimarySmtpAddress(mailItem, journalReport.Sender.Address);
				}
				else
				{
					unjournalRecipientType = UnjournalRecipientType.Empty;
					text = null;
				}
			}
			else
			{
				unjournalRecipientType = UnwrapJournalAgent.GetRecipientType(mailItem, journalReport.Sender.Address.ToString());
				text = primarySmtpAddress;
			}
			journalReport.Sender.RecipientType = unjournalRecipientType;
			journalReport.IsSenderInternal = (unjournalRecipientType == UnjournalRecipientType.Mailbox);
			journalReport.Sender.PrimarySmtpAddress = ((text == null) ? string.Empty : text);
			if (journalReport.IsSenderInternal && !mailItem.Recipients.Contains(journalReport.Sender.Address))
			{
				targetRecipients.Add(journalReport.Sender.Address);
			}
		}

		private EmailMessage ConvertAttachementToEmailMessage(Attachment messageatt)
		{
			Stream contentReadStream = messageatt.GetContentReadStream();
			return EmailMessage.Create(contentReadStream);
		}

		private void UpdateJournalBodyWithNdrMessage(MailItem mailItem, LocalizedString locMessage)
		{
			Body body = mailItem.Message.Body;
			Encoding encoding = Charset.GetEncoding(body.CharsetName);
			TextConverter textConverter;
			switch (mailItem.Message.Body.BodyFormat)
			{
			default:
				textConverter = new TextToText
				{
					InputEncoding = encoding,
					HeaderFooterFormat = HeaderFooterFormat.Text,
					Footer = "\n" + locMessage.ToString()
				};
				break;
			case BodyFormat.Html:
				textConverter = new HtmlToHtml
				{
					InputEncoding = encoding,
					HeaderFooterFormat = HeaderFooterFormat.Html,
					Footer = "<br/>" + locMessage.ToString()
				};
				break;
			}
			using (Stream contentReadStream = body.GetContentReadStream())
			{
				using (Stream contentWriteStream = body.GetContentWriteStream())
				{
					textConverter.Convert(contentReadStream, contentWriteStream);
				}
			}
		}

		private void PrepareDeliveryMessageForNdrReport(MailItem mailItem, FailureMessageType failureType, List<RoutingAddress> recipients, List<RoutingAddress> distributionGroups)
		{
			ITransportMailItemFacade transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			TransportMailItem transportMailItem2 = transportMailItem as TransportMailItem;
			string arg = (transportMailItem2 == null) ? string.Empty : transportMailItem2.MsgId.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("UserListStart;");
			if (recipients != null && recipients.Count > 0)
			{
				foreach (RoutingAddress routingAddress in recipients)
				{
					stringBuilder.AppendLine(routingAddress.ToString() + "; <Unprovisioned>");
				}
			}
			if (distributionGroups != null && distributionGroups.Count > 0)
			{
				foreach (RoutingAddress routingAddress2 in distributionGroups)
				{
					stringBuilder.AppendLine(routingAddress2.ToString() + ";<DistributionGroup>");
				}
			}
			stringBuilder.AppendLine("UserListEnd;");
			LocalizedString locMessage = LocalizedString.Empty;
			switch (failureType)
			{
			case FailureMessageType.DefectiveJournalNoRecipientsMsg:
				locMessage = AgentStrings.DefectiveJournalNoRecipients(stringBuilder.ToString());
				break;
			case FailureMessageType.DefectiveJournalWithRecipientsMsg:
				locMessage = AgentStrings.DefectiveJournalWithRecipients(stringBuilder.ToString());
				break;
			case FailureMessageType.UnProvisionedRecipientsMsg:
				locMessage = AgentStrings.UnProvisionedRecipientsMsg(stringBuilder.ToString());
				break;
			case FailureMessageType.NoRecipientsResolvedMsg:
				locMessage = AgentStrings.NoRecipientsResolvedMsg;
				break;
			case FailureMessageType.UnexpectedJournalMessageFormatMsg:
				locMessage = AgentStrings.UnexpectedJournalMessageFormatMsg;
				break;
			case FailureMessageType.PermanentErrorOther:
				locMessage = AgentStrings.PermanentErrorOther;
				break;
			}
			if (!locMessage.IsEmpty)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "UnJournalAgent: MessageId: {0} About to update journal body with ndr message {1}.", arg, locMessage.ToString());
				this.UpdateJournalBodyWithNdrMessage(mailItem, locMessage);
			}
		}

		private void TrackAgentInfo(SubmittedMessageEventSource source, List<RoutingAddress> targetRecipients)
		{
			int num = 0;
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (targetRecipients != null && targetRecipients.Count > 0)
			{
				this.VerifyAndAddAgentInfoDataItem("destNum", targetRecipients.Count.ToString(), list, ref num);
				foreach (RoutingAddress address in targetRecipients)
				{
					string text = (string)address;
					if (!this.VerifyAndAddAgentInfoDataItem("dest", text.ToString(), list, ref num))
					{
						break;
					}
				}
			}
			if (list.Count > 0)
			{
				source.TrackAgentInfo("UJA", "UM", list);
			}
		}

		private bool VerifyAndAddAgentInfoDataItem(string key, string value, List<KeyValuePair<string, string>> data, ref int dataLength)
		{
			if (dataLength < 0 || dataLength >= UnwrapJournalAgent.maxMsgTrkAgenInfoLength)
			{
				return false;
			}
			KeyValuePair<string, string> item = new KeyValuePair<string, string>(key, value);
			int num = item.Key.Length + item.Value.Length;
			if (num <= 0 || num > UnwrapJournalAgent.maxMsgTrkAgenInfoLength - dataLength)
			{
				return false;
			}
			dataLength += num;
			data.Add(item);
			return true;
		}

		private void UpdatePerformanceCounters(ProcessingStatus processingStatus)
		{
			switch (processingStatus)
			{
			case ProcessingStatus.PermanentError:
				this.perfCountersWrapper.Increment(PerfCounters.PermanentError, 1L);
				return;
			case ProcessingStatus.TransientError:
				this.perfCountersWrapper.Increment(PerfCounters.TransientError, 1L);
				return;
			case ProcessingStatus.UnwrapProcessSuccess:
				break;
			case ProcessingStatus.NdrProcessSuccess:
				this.perfCountersWrapper.Increment(PerfCounters.NdrProcessSuccess, 1L);
				return;
			case ProcessingStatus.LegacyArchiveJournallingDisabled:
				this.perfCountersWrapper.Increment(PerfCounters.LegacyArchiveJournallingDisabled, 1L);
				return;
			case ProcessingStatus.NonJournalMsgFromLegacyArchiveCustomer:
				this.perfCountersWrapper.Increment(PerfCounters.NonJournalMsgFromLegacyArchiveCustomer, 1L);
				return;
			case ProcessingStatus.AlreadyProcessed:
				this.perfCountersWrapper.Increment(PerfCounters.AlreadyProcessed, 1L);
				return;
			case ProcessingStatus.DropJournalReportWithoutNdr:
				this.perfCountersWrapper.Increment(PerfCounters.DropJournalReportWithoutNdr, 1L);
				return;
			case ProcessingStatus.NoUsersResolved:
				this.perfCountersWrapper.Increment(PerfCounters.NoUsersResolved, 1L);
				return;
			case ProcessingStatus.NdrJournalReport:
				this.perfCountersWrapper.Increment(PerfCounters.NdrJournalReport, 1L);
				break;
			default:
				return;
			}
		}

		private static readonly object syncObject = new object();

		private static UnwrapJournalAgent.EHAUnwrapJournalAgentLog logger = null;

		private static AutoResponseSuppressFormatter autoResponseSuppressFormatter = new AutoResponseSuppressFormatter();

		private static int maxMsgTrkAgenInfoLength = Math.Min(512, Components.TransportAppConfig.Logging.MaxMsgTrkAgenInfoSize);

		private readonly bool isEHAJournalingEnabled;

		private readonly bool isEnabled;

		private readonly JournalPerfCountersWrapper perfCountersWrapper;

		private Stopwatch timer = new Stopwatch();

		private ArchiveJournalTenantConfiguration configuration;

		internal class OriginalMailItemInfo
		{
			internal OriginalMailItemInfo(List<EnvelopeRecipient> recipientList, RoutingAddress fromAddress)
			{
				this.senderAddress = RoutingAddress.Parse(fromAddress.ToString());
				this.recipientList = new List<RoutingAddress>();
				foreach (EnvelopeRecipient envelopeRecipient in recipientList)
				{
					RoutingAddress address = envelopeRecipient.Address;
					this.recipientList.Add(envelopeRecipient.Address);
				}
			}

			internal static void ResetSenderRecipientInMailItem(MailItem mailItem, UnwrapJournalAgent.OriginalMailItemInfo originalMailItem)
			{
				mailItem.Recipients.Clear();
				foreach (RoutingAddress address in originalMailItem.recipientList)
				{
					mailItem.Recipients.Add(address);
				}
				mailItem.FromAddress = originalMailItem.senderAddress;
			}

			private RoutingAddress senderAddress;

			private List<RoutingAddress> recipientList;
		}

		internal class EHAUnwrapJournalAgentLog : IDisposable
		{
			internal EHAUnwrapJournalAgentLog()
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "static EHAUnwrapJournalAgentLog created");
				string[] array = new string[2];
				for (int i = 0; i < 2; i++)
				{
					array[i] = ((UnwrapJournalAgent.EHAUnwrapJournalAgentLog.Field)i).ToString();
				}
				if (!Directory.Exists(UnwrapJournalAgent.EHAUnwrapJournalAgentLog.LogDirectory))
				{
					Directory.CreateDirectory(UnwrapJournalAgent.EHAUnwrapJournalAgentLog.LogDirectory);
				}
				this.logSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "EHA Migration Log", array);
				this.log = new Log("EHAMigrationLog", new LogHeaderFormatter(this.logSchema), "EHAMigrationLogs");
				this.log.Configure(UnwrapJournalAgent.EHAUnwrapJournalAgentLog.LogDirectory, UnwrapJournalAgent.EHAUnwrapJournalAgentLog.MaxAge, 262144000L, 10485760L);
			}

			public void Dispose()
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "EHAUnwrapJournalAgentLog Dispose() called");
				lock (this)
				{
					if (this.log != null)
					{
						this.log.Close();
						this.log = null;
					}
				}
			}

			internal StringBuilder Append(StringBuilder sb)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
				logRowFormatter[1] = sb.ToString();
				if (this.log != null)
				{
					this.log.Append(logRowFormatter, 0);
				}
				return sb;
			}

			private const string LogComponentName = "EHAMigrationLogs";

			private const string FileNamePrefix = "EHAMigrationLog";

			private const long MaxFileSize = 10485760L;

			private const long MaxDirSize = 262144000L;

			public static readonly string LogDirectory = Path.Combine(Configuration.TransportServer.InstallPath.PathName, "TransportRoles\\Logs\\EHAConfirmations\\");

			private static readonly TimeSpan MaxAge = TimeSpan.FromDays(30.0);

			private Log log;

			private LogSchema logSchema;

			private enum Field
			{
				Timestamp,
				LogString,
				NumFields
			}
		}
	}
}
