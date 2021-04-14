using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal static class CommonUtils
	{
		public static bool IsEnabled(MessageHygieneAgentConfig agentConfig, SmtpSession session)
		{
			return agentConfig.Enabled && ((agentConfig.ExternalMailEnabled && (session.AuthenticationSource == AuthenticationSource.Anonymous || session.AuthenticationSource == AuthenticationSource.Partner)) || (agentConfig.InternalMailEnabled && session.AuthenticationSource == AuthenticationSource.Organization));
		}

		public static bool IsExternalRecipient(EnvelopeRecipient recipient, Microsoft.Exchange.Diagnostics.Trace tracer, int tracerHashCode)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			object obj;
			bool flag;
			if (recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.RecipientType", out obj) && obj is Microsoft.Exchange.Data.Directory.Recipient.RecipientType)
			{
				switch ((Microsoft.Exchange.Data.Directory.Recipient.RecipientType)obj)
				{
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.User:
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox:
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser:
				{
					object arg;
					if (CommonUtils.HasExternalEmailAddress(recipient, out arg))
					{
						tracer.TraceDebug<RoutingAddress, object, object>((long)tracerHashCode, "Recipient {0} is external because its type is {1} and it has an external address {2}", recipient.Address, obj, arg);
						return true;
					}
					tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} has type {1} and no external address, so it is not external", recipient.Address, obj);
					return false;
				}
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Contact:
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact:
					tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} is external because its type is {1}", recipient.Address, obj);
					return true;
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup:
				{
					object arg;
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Transport.TargetAddressDistributionGroupAsExternal.Enabled && CommonUtils.HasExternalEmailAddress(recipient, out arg))
					{
						tracer.TraceDebug<RoutingAddress, object, object>((long)tracerHashCode, "Recipient {0} is external because type is {1}, has an external address {2}, and VariantConfiguration TargetAddressDistributionGroupAsExternal true", recipient.Address, obj, arg);
						return true;
					}
					tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} with type {1} has no external address or VariantConfiguration TargetAddressDistributionGroupAsExternal false, so is not external", recipient.Address, obj);
					return false;
				}
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder:
				{
					object obj2;
					if (!recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.IsRemoteRecipient", out obj2) || !(obj2 is bool))
					{
						tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} of type {1} doesn't have IsRemoteRecipient property, so not treating this as external", recipient.Address, obj);
						return false;
					}
					flag = (bool)obj2;
					if (flag)
					{
						tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} of type {1} is an external recipient", recipient.Address, obj);
						return flag;
					}
					tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} of type {1} is not an external recipient", recipient.Address, obj);
					return flag;
				}
				}
				tracer.TraceDebug<RoutingAddress, object>((long)tracerHashCode, "Recipient {0} is not external because its type is {1}", recipient.Address, obj);
				flag = false;
			}
			else
			{
				tracer.TraceDebug<RoutingAddress>((long)tracerHashCode, "Recipient {0} is external because it is a one-off address", recipient.Address);
				flag = true;
			}
			return flag;
		}

		public static bool HasAntispamBypassPermission(SmtpSession session, Microsoft.Exchange.Diagnostics.Trace tracer, Agent agent)
		{
			if (session != null && session.AntispamBypass)
			{
				if (tracer != null)
				{
					tracer.TraceDebug((long)((agent != null) ? agent.GetHashCode() : 0), "Session has AntispamBypass permission.");
				}
				return true;
			}
			return false;
		}

		public static void RegisterConfigurationChangeHandlers(string agentName, ADOperation registrationMethod, Microsoft.Exchange.Diagnostics.Trace tracer, object source)
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(registrationMethod);
			if (!adoperationResult.Succeeded)
			{
				if (tracer != null)
				{
					tracer.TraceDebug((long)((source != null) ? source.GetHashCode() : 0), "Failed to register for AD notifications.");
				}
				throw new ExchangeConfigurationException(Strings.FailedToRegisterForConfigChangeNotification(agentName), adoperationResult.Exception);
			}
		}

		public static void FailedToReadConfiguration(string agentName, bool onStartup, Exception exception, Microsoft.Exchange.Diagnostics.Trace tracer, ExEventLog logger, object source)
		{
			if (tracer != null)
			{
				tracer.TraceDebug<string, string>((long)((source != null) ? source.GetHashCode() : 0), "{0} configuration could not be read on {1}.", agentName, onStartup ? "start-up" : "update");
			}
			if (onStartup)
			{
				throw new ExchangeConfigurationException(Strings.FailedToReadConfiguration(agentName), exception);
			}
			if (logger != null)
			{
				logger.LogEvent(AgentsEventLogConstants.Tuple_FailedToReadConfiguration, null, new object[]
				{
					agentName
				});
			}
		}

		public static bool TryAddressBookFind(AddressBook addressBook, EnvelopeRecipientCollection recipients, out ReadOnlyCollection<AddressBookEntry> addressBookEntries, out AddressBookFindStatus status)
		{
			if (addressBook == null)
			{
				addressBookEntries = null;
				status = AddressBookFindStatus.Success;
				return true;
			}
			ADOperationResult adOperationResult;
			if (!ADNotificationAdapter.TryReadConfiguration<ReadOnlyCollection<AddressBookEntry>>(() => addressBook.Find(recipients), out addressBookEntries, out adOperationResult))
			{
				addressBookEntries = null;
				status = CommonUtils.ConvertToAddressBookFindStatus(adOperationResult);
				return status == AddressBookFindStatus.Success;
			}
			if (addressBookEntries != null && addressBookEntries.Count != recipients.Count)
			{
				throw new InvalidOperationException("Recipient API returned a corrupted collection.");
			}
			status = AddressBookFindStatus.Success;
			return true;
		}

		public static bool TryAddressBookFind(AddressBook addressBook, EnvelopeRecipientCollection recipients, out ReadOnlyCollection<AddressBookEntry> addressBookEntries)
		{
			AddressBookFindStatus addressBookFindStatus;
			return CommonUtils.TryAddressBookFind(addressBook, recipients, out addressBookEntries, out addressBookFindStatus);
		}

		public static bool TryAddressBookFind(AddressBook addressBook, RoutingAddress smtpAddress, out AddressBookEntry addressBookEntry, out AddressBookFindStatus status)
		{
			if (addressBook == null)
			{
				addressBookEntry = null;
				status = AddressBookFindStatus.Success;
				return true;
			}
			ADOperationResult adOperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<AddressBookEntry>(() => addressBook.Find(smtpAddress), out addressBookEntry, out adOperationResult))
			{
				status = AddressBookFindStatus.Success;
				return true;
			}
			addressBookEntry = null;
			status = CommonUtils.ConvertToAddressBookFindStatus(adOperationResult);
			return status == AddressBookFindStatus.Success;
		}

		public static bool TryAddressBookFind(AddressBook addressBook, RoutingAddress smtpAddress, out AddressBookEntry addressBookEntry)
		{
			AddressBookFindStatus addressBookFindStatus;
			return CommonUtils.TryAddressBookFind(addressBook, smtpAddress, out addressBookEntry, out addressBookFindStatus);
		}

		public static bool TryGetValidScl(HeaderList headers, out int scl)
		{
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}
			scl = 0;
			try
			{
				Header header = headers.FindFirst("X-MS-Exchange-Organization-SCL");
				if (header != null)
				{
					if (CommonUtils.TryParseScl(header.Value, out scl))
					{
						return true;
					}
					headers.RemoveChild(header);
				}
			}
			catch (ExchangeDataException)
			{
				return false;
			}
			return false;
		}

		public static bool TryParseScl(string s, out int scl)
		{
			if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out scl) && scl >= -1 && scl <= 9)
			{
				return true;
			}
			scl = 0;
			return false;
		}

		public static bool IsSafeRecipient(AddressBookEntry p1Recipient, IEnumerable<RoutingAddress> p2Recipients)
		{
			int num = 0;
			if (p1Recipient == null)
			{
				throw new ArgumentNullException("p1Recipient");
			}
			if (p2Recipients == null)
			{
				throw new ArgumentNullException("p2Recipients");
			}
			foreach (RoutingAddress recipientAddress in p2Recipients)
			{
				if (num >= 5)
				{
					break;
				}
				if (p1Recipient.IsSafeRecipient(recipientAddress))
				{
					return true;
				}
				num++;
			}
			return false;
		}

		public static void SetPartnerDomainRoutingOverride(ResolvedMessageEventSource source, EnvelopeRecipient recipient, RoutingDomain partnerRoutingDomain, RoutingDomain partnerConnectorDomain, Microsoft.Exchange.Diagnostics.Trace tracer, int tracerHashCode)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			RoutingOverride routingOverride = source.GetRoutingOverride(recipient);
			RoutingOverride routingOverride2 = new RoutingOverride(partnerConnectorDomain, new RoutingHost(partnerRoutingDomain.Domain), DeliveryQueueDomain.UseAlternateDeliveryRoutingHosts);
			source.SetRoutingOverride(recipient, routingOverride2);
			tracer.TraceDebug<object, RoutingOverride>((long)tracerHashCode, "Old Override = {0},New Override = {1}", routingOverride ?? "none", routingOverride2);
		}

		public static void SetPerimeterRoutingOverride(ResolvedMessageEventSource source, EnvelopeRecipient recipient, Microsoft.Exchange.Diagnostics.Trace tracer, int tracerHashCode)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			RoutingOverride routingOverride = source.GetRoutingOverride(recipient);
			RoutingDomain routingDomain = new RoutingDomain(recipient.Address.DomainPart);
			RoutingOverride routingOverride2 = new RoutingOverride(routingDomain, DeliveryQueueDomain.UseOverrideDomain);
			source.SetRoutingOverride(recipient, routingOverride2);
			tracer.TraceDebug<object, RoutingOverride>((long)tracerHashCode, "Old Override = {0},New Override = {1}", routingOverride ?? "none", routingOverride2);
		}

		public static void SetEhfRoutingOverride(ResolvedMessageEventSource source, EnvelopeRecipient recipient, Microsoft.Exchange.Diagnostics.Trace tracer, int tracerHashCode)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			string domainPart = recipient.Address.DomainPart;
			string type = "ehf";
			RoutingDomain routingDomain = new RoutingDomain(domainPart, type);
			RoutingOverride routingOverride = new RoutingOverride(routingDomain, DeliveryQueueDomain.UseOverrideDomain);
			source.SetRoutingOverride(recipient, routingOverride);
			tracer.TraceDebug<RoutingOverride>((long)tracerHashCode, "New Override Routing Domain = {0}", routingOverride);
		}

		public static bool TryGetAcceptedDomain(OrganizationId organizationId, RoutingAddress mailAddress, out Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain)
		{
			acceptedDomain = null;
			if (string.IsNullOrEmpty(mailAddress.DomainPart))
			{
				return false;
			}
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(organizationId, out perTenantAcceptedDomainTable))
			{
				acceptedDomain = perTenantAcceptedDomainTable.AcceptedDomainTable.Find(mailAddress.DomainPart);
				if (acceptedDomain != null)
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryGetRecipientAcceptedDomain(MailItem mailItem, out Microsoft.Exchange.Data.Transport.AcceptedDomain recipientAcceptedDomain)
		{
			recipientAcceptedDomain = null;
			TransportMailItemWrapper transportMailItemWrapper = (TransportMailItemWrapper)mailItem;
			TransportMailItem transportMailItem = transportMailItemWrapper.TransportMailItem;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(transportMailItem.OrganizationId, out perTenantAcceptedDomainTable))
			{
				if (transportMailItem.Directionality == MailDirectionality.Incoming && transportMailItem.Recipients.Count > 0)
				{
					recipientAcceptedDomain = perTenantAcceptedDomainTable.AcceptedDomainTable.Find(transportMailItem.Recipients[0].Email);
				}
				else
				{
					recipientAcceptedDomain = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomain;
				}
				if (recipientAcceptedDomain != null)
				{
					return true;
				}
			}
			return false;
		}

		public static void StampPutInQuarantineHeader(EmailMessage message, QuarantineFlavor flavor, int retentionDays, bool shouldEncrypt)
		{
			QuarantineConnector quarantineConnector = new QuarantineConnector(flavor, retentionDays, shouldEncrypt);
			quarantineConnector.StampMessage(message);
		}

		public static void PublishEventLogAndNotification(CommonUtils.EventLogHelper logHelper, string eventNotificationComponent, ExEventLog.EventTuple eventTuple, string eventMessage, params object[] args)
		{
			logHelper.EventLogger.LogEvent(eventTuple, null, args);
			CommonUtils.PublishEventNotification(logHelper, eventNotificationComponent, eventTuple, eventMessage, args);
		}

		public static void PublishPeriodicEventLogAndNotification(CommonUtils.EventLogHelper logHelper, string eventNotificationComponent, ExEventLog.EventTuple eventTuple, string periodicKey, string eventMessage, params object[] args)
		{
			bool flag = false;
			logHelper.EventLogger.LogEvent(eventTuple, periodicKey, out flag, args);
			if (!flag)
			{
				CommonUtils.PublishEventNotification(logHelper, eventNotificationComponent, eventTuple, eventMessage, args);
			}
		}

		private static void PublishEventNotification(CommonUtils.EventLogHelper logHelper, string eventNotificationComponent, ExEventLog.EventTuple eventTuple, string eventMessage, params object[] args)
		{
			ResultSeverityLevel severity = ResultSeverityLevel.Informational;
			switch (eventTuple.EntryType)
			{
			case EventLogEntryType.Error:
				severity = ResultSeverityLevel.Critical;
				break;
			case EventLogEntryType.Warning:
				severity = ResultSeverityLevel.Warning;
				break;
			case EventLogEntryType.Information:
				severity = ResultSeverityLevel.Informational;
				break;
			}
			try
			{
				EventNotificationItem.Publish(logHelper.ServiceComponent, eventNotificationComponent, null, string.Format(eventMessage, args), severity, false);
			}
			catch (FormatException ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string arg in args)
				{
					stringBuilder.Append(string.Format("{0},", arg));
				}
				logHelper.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_AlertStringFormatExceptionGenerated, null, new object[]
				{
					eventMessage,
					stringBuilder.ToString().TrimEnd(new char[]
					{
						','
					}),
					ex.ToString()
				});
			}
		}

		private static AddressBookFindStatus ConvertToAddressBookFindStatus(ADOperationResult adOperationResult)
		{
			if (adOperationResult == null)
			{
				throw new ArgumentNullException("adOperationResult");
			}
			switch (adOperationResult.ErrorCode)
			{
			case ADOperationErrorCode.Success:
				return AddressBookFindStatus.Success;
			case ADOperationErrorCode.RetryableError:
				return AddressBookFindStatus.TransientFailure;
			case ADOperationErrorCode.PermanentError:
				return AddressBookFindStatus.PermanentFailure;
			default:
				throw new InvalidOperationException("unknown ADOperationResult");
			}
		}

		private static bool HasExternalEmailAddress(EnvelopeRecipient recipient, out object externalAddressObj)
		{
			return recipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress", out externalAddressObj) && !string.IsNullOrEmpty(externalAddressObj as string);
		}

		internal struct EventLogHelper
		{
			internal EventLogHelper(string serviceComponent, ExEventLog eventLogger)
			{
				this.serviceComponent = serviceComponent;
				this.eventLogger = eventLogger;
			}

			internal string ServiceComponent
			{
				get
				{
					return this.serviceComponent;
				}
			}

			internal ExEventLog EventLogger
			{
				get
				{
					return this.eventLogger;
				}
			}

			private readonly string serviceComponent;

			private readonly ExEventLog eventLogger;
		}
	}
}
