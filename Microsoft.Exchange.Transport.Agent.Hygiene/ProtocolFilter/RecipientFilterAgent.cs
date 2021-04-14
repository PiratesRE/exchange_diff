using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.ProtocolFilter;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	internal sealed class RecipientFilterAgent : SmtpReceiveAgent
	{
		public RecipientFilterAgent(RecipientFilterConfig recipientFilterConfig, Dictionary<RoutingAddress, bool> blockedRecipients, AddressBook addressBook, AcceptedDomainCollection acceptedDomains)
		{
			this.recipientFilterConfig = recipientFilterConfig;
			this.blockedRecipients = blockedRecipients;
			this.addressBook = addressBook;
			this.acceptedDomains = acceptedDomains;
			base.OnRcptCommand += this.RcptToHandler;
		}

		private static bool IsAuthenticated(RcptCommandEventArgs rcptEventArgs)
		{
			object obj;
			return rcptEventArgs.SmtpSession.Properties.TryGetValue("Microsoft.Exchange.IsAuthenticated", out obj) && obj is bool && (bool)obj;
		}

		private void RcptToHandler(ReceiveCommandEventSource receiveMessageEventSource, RcptCommandEventArgs rcptEventArgs)
		{
			if (this.IsPolicyDisabled(rcptEventArgs))
			{
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(rcptEventArgs.SmtpSession, ExTraceGlobals.RecipientFilterAgentTracer, this))
			{
				return;
			}
			LogEntry logEntry = null;
			SmtpResponse rcptNotFound = SmtpResponse.RcptNotFound;
			if ((this.IsRecipientLookupEnabled() && this.RecipientIsBlockedByRecipientLookup(rcptEventArgs, ref logEntry, ref rcptNotFound)) || (this.IsBlockListEnabled() && this.RecipientIsBlockedByBlockList(rcptEventArgs, ref logEntry)))
			{
				AgentLog.Instance.LogRejectCommand(base.Name, base.EventTopic, rcptEventArgs, rcptNotFound, logEntry);
				receiveMessageEventSource.RejectCommand(rcptNotFound);
			}
		}

		private bool IsPolicyDisabled(RcptCommandEventArgs rcptEventArgs)
		{
			if (!CommonUtils.IsEnabled(this.recipientFilterConfig, rcptEventArgs.SmtpSession))
			{
				ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug((long)this.GetHashCode(), "Recipient filter policy is disabled.");
				return true;
			}
			return false;
		}

		private bool IsRecipientLookupEnabled()
		{
			return this.addressBook != null && this.recipientFilterConfig.RecipientValidationEnabled;
		}

		private bool RecipientIsBlockedByRecipientLookup(RcptCommandEventArgs rcptEventArgs, ref LogEntry logEntry, ref SmtpResponse response)
		{
			bool flag = false;
			bool flag2 = false;
			Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain = this.acceptedDomains.Find(rcptEventArgs.RecipientAddress.DomainPart);
			if (acceptedDomain != null && acceptedDomain.UseAddressBook)
			{
				AddressBookEntry addressBookEntry;
				AddressBookFindStatus arg;
				if (CommonUtils.TryAddressBookFind(this.addressBook, rcptEventArgs.RecipientAddress, out addressBookEntry, out arg))
				{
					if (addressBookEntry == null)
					{
						ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Recipient {0} does not exist", rcptEventArgs.RecipientAddress);
						flag = true;
						logEntry = RecipientFilterAgent.RejectContext.RecipientDoesNotExist;
						response = SmtpResponse.RcptNotFound;
					}
					else if (addressBookEntry.RequiresAuthentication && !RecipientFilterAgent.IsAuthenticated(rcptEventArgs))
					{
						ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Recipient {0} requires authentication and session is not authenticated", rcptEventArgs.RecipientAddress);
						flag = true;
						logEntry = RecipientFilterAgent.RejectContext.RecipientIsRestricted;
						response = SmtpResponse.RcptNotFound;
					}
				}
				else
				{
					flag2 = true;
					switch (arg)
					{
					case AddressBookFindStatus.TransientFailure:
						ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Could not look up recipient {0}: transient failure.", rcptEventArgs.RecipientAddress);
						logEntry = RecipientFilterAgent.RejectContext.TransientFailure;
						response = SmtpResponse.DataTransactionFailed;
						break;
					case AddressBookFindStatus.PermanentFailure:
						ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Could not look up recipient {0}: permanent failure.", rcptEventArgs.RecipientAddress);
						logEntry = RecipientFilterAgent.RejectContext.PermanentFailure;
						response = SmtpResponse.RcptNotFound;
						break;
					default:
						ExTraceGlobals.RecipientFilterAgentTracer.TraceError<RoutingAddress, AddressBookFindStatus>((long)this.GetHashCode(), "Could not look up recipient {0}: unexpected status {1}. Returning temporary error response.", rcptEventArgs.RecipientAddress, arg);
						logEntry = RecipientFilterAgent.RejectContext.UnexpectedFailure;
						response = SmtpResponse.DataTransactionFailed;
						break;
					}
				}
			}
			if (flag)
			{
				Util.PerformanceCounters.RecipientFilter.RecipientRejectedByRecipientValidation();
			}
			return flag || flag2;
		}

		private bool IsBlockListEnabled()
		{
			return this.recipientFilterConfig.BlockListEnabled;
		}

		private bool RecipientIsBlockedByBlockList(RcptCommandEventArgs rcptEventArgs, ref LogEntry logEntry)
		{
			if (this.blockedRecipients.ContainsKey(rcptEventArgs.RecipientAddress))
			{
				ExTraceGlobals.RecipientFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Recipient {0} is on the block list", rcptEventArgs.RecipientAddress);
				Util.PerformanceCounters.RecipientFilter.RecipientRejectedByBlockList();
				logEntry = RecipientFilterAgent.RejectContext.RecipientOnBlockList;
				return true;
			}
			return false;
		}

		private RecipientFilterConfig recipientFilterConfig;

		private Dictionary<RoutingAddress, bool> blockedRecipients;

		private AddressBook addressBook;

		private AcceptedDomainCollection acceptedDomains;

		private static class RejectContext
		{
			public static readonly LogEntry RecipientDoesNotExist = new LogEntry("RecipientDoesNotExist", string.Empty);

			public static readonly LogEntry RecipientIsRestricted = new LogEntry("RecipientIsRestricted", string.Empty);

			public static readonly LogEntry RecipientOnBlockList = new LogEntry("RecipientOnBlockList", string.Empty);

			public static readonly LogEntry PermanentFailure = new LogEntry("PermanentFailure", string.Empty);

			public static readonly LogEntry TransientFailure = new LogEntry("TransientFailure", string.Empty);

			public static readonly LogEntry UnexpectedFailure = new LogEntry("UnexpectedFailure", string.Empty);
		}
	}
}
