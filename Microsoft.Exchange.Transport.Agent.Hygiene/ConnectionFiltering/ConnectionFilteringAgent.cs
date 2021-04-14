using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ConnectionFiltering;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	internal class ConnectionFilteringAgent : SmtpReceiveAgent
	{
		internal ConnectionFilteringAgent(ConnectionFilterConfig config, SmtpServer server, BypassedRecipients blockListProviderBypassedRecipients)
		{
			this.server = server;
			base.OnConnect += this.OnConnectHandler;
			base.OnMailCommand += this.OnMailFromHandler;
			base.OnRcptCommand += this.OnRcptToHandler;
			base.OnEndOfHeaders += this.OnEndOfHeadersHandler;
			this.config = config;
			this.blockListProviderBypassedRecipients = blockListProviderBypassedRecipients;
		}

		public void OnConnectHandler(ConnectEventSource source, ConnectEventArgs args)
		{
			ConnectionFilteringAgent.AssertSourceAndArgsNotNull(source, args, "OnConnectHandler");
			ExTraceGlobals.OnConnectTracer.TraceDebug((long)this.GetHashCode(), "OnConnectHandler started");
			if (!args.SmtpSession.IsExternalConnection)
			{
				ExTraceGlobals.OnConnectTracer.TraceDebug((long)this.GetHashCode(), "Edge running behind MTA.  Exiting.");
				return;
			}
			bool flag = CommonUtils.IsEnabled(this.config.AllowListConfig, args.SmtpSession);
			bool flag2 = CommonUtils.IsEnabled(this.config.BlockListConfig, args.SmtpSession);
			if (!flag && !flag2)
			{
				ExTraceGlobals.OnConnectTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Block policy disabled.  Exiting.");
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(args.SmtpSession, ExTraceGlobals.OnConnectTracer, this))
			{
				ExTraceGlobals.OnConnectTracer.TraceDebug((long)this.GetHashCode(), "AntispamBypass permissions are set on the session. Exiting.");
				return;
			}
			IPAddress address = args.SmtpSession.RemoteEndPoint.Address;
			PermissionCheckResults permissionCheckResults = this.server.IPPermission.Check(address);
			ExTraceGlobals.OnConnectTracer.TraceDebug<IPAddress, PermissionCheckResults>((long)this.GetHashCode(), "IsOnAllowOrDenyList({0}) returned: {1}", address, permissionCheckResults);
			if (flag)
			{
				bool flag3 = (permissionCheckResults & PermissionCheckResults.Allow) != PermissionCheckResults.None;
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnAllowList"] = flag3;
				if (flag3)
				{
					ConnectionFilteringAgent.PerformanceCounters.ConnectionOnIPAllowList();
					return;
				}
			}
			if (flag2)
			{
				bool flag4 = (permissionCheckResults & PermissionCheckResults.Deny) != PermissionCheckResults.None;
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnDenyList"] = flag4;
				if (flag4)
				{
					ConnectionFilteringAgent.PerformanceCounters.ConnectionOnIPBlockList();
				}
				this.blockedByMachineGeneratedEntry = ((permissionCheckResults & PermissionCheckResults.MachineDeny) != PermissionCheckResults.None);
			}
		}

		public void OnMailFromHandler(ReceiveCommandEventSource source, MailCommandEventArgs args)
		{
			ConnectionFilteringAgent.AssertSourceAndArgsNotNull(source, args, "OnMailFromHandler");
			ExTraceGlobals.OnMailFromTracer.TraceDebug((long)this.GetHashCode(), "Starting OnMailFromHandler");
			if (!args.SmtpSession.IsExternalConnection)
			{
				ExTraceGlobals.OnMailFromTracer.TraceDebug((long)this.GetHashCode(), "Edge running behind MTA.  Exiting.");
				args.SmtpSession.Properties.Remove("Microsoft.Exchange.IsOnAllowList");
				args.SmtpSession.Properties.Remove("Microsoft.Exchange.IsOnBlockList");
				args.SmtpSession.Properties.Remove("Microsoft.Exchange.IsOnSafeList");
				args.SmtpSession.Properties.Remove("Microsoft.Exchange.IsOnDenyList");
				return;
			}
			if (!CommonUtils.IsEnabled(this.config.AllowListConfig, args.SmtpSession) && !CommonUtils.IsEnabled(this.config.BlockListConfig, args.SmtpSession))
			{
				ExTraceGlobals.OnMailFromTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Block policy disabled.  Exiting.");
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(args.SmtpSession, ExTraceGlobals.OnMailFromTracer, this))
			{
				ExTraceGlobals.OnMailFromTracer.TraceDebug((long)this.GetHashCode(), "AntispamBypass permissions are set on the session. Exiting.");
				return;
			}
			if (ConnectionFilteringAgent.IsPropertyValueTrue(args.SmtpSession.Properties, "Microsoft.Exchange.IsOnDenyList"))
			{
				ExTraceGlobals.OnMailFromTracer.TraceError<IPAddress>((long)this.GetHashCode(), "The IP {0} was found in the static block list. Rejecting mail from command", args.SmtpSession.RemoteEndPoint.Address);
				string message = this.blockedByMachineGeneratedEntry ? this.FormatMachineRejectionResponse(args.SmtpSession.RemoteEndPoint.Address) : this.FormatStaticRejectionResponse(args.SmtpSession.RemoteEndPoint.Address);
				SmtpResponse smtpResponse = ConnectionFilteringAgent.CreateSmtpRejectionResponse(message);
				LogEntry logEntry = this.blockedByMachineGeneratedEntry ? ConnectionFilteringAgent.RejectContext.MachineGeneratedLocalBlockListEntry : ConnectionFilteringAgent.RejectContext.AdministratorLocalBlockListEntry;
				AgentLog.Instance.LogRejectCommand(base.Name, base.EventTopic, args, smtpResponse, logEntry);
				source.RejectCommand(smtpResponse);
			}
		}

		public void OnRcptToHandler(ReceiveCommandEventSource source, RcptCommandEventArgs args)
		{
			ConnectionFilteringAgent.AssertSourceAndArgsNotNull(source, args, "OnRcptToHandler");
			ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "Starting OnRcptToHandler");
			if (!args.SmtpSession.IsExternalConnection)
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "Edge running behind MTA.  Exiting.");
				return;
			}
			bool flag = CommonUtils.IsEnabled(this.config.AllowListProviderConfig, args.SmtpSession);
			bool flag2 = CommonUtils.IsEnabled(this.config.BlockListProviderConfig, args.SmtpSession);
			if (!flag && !flag2)
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Block policy disabled.  Exiting.");
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(args.SmtpSession, ExTraceGlobals.OnRcptToTracer, this))
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "AntispamBypass permissions are set on the session. Exiting.");
				return;
			}
			if (this.blockListProviderBypassedRecipients.IsBypassed(args.RecipientAddress))
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Recipient {0} is bypassed for the block list providers.", args.RecipientAddress);
				flag2 = false;
			}
			if (ConnectionFilteringAgent.IsPropertyValueTrue(args.SmtpSession.Properties, "Microsoft.Exchange.IsOnAllowList"))
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "Connecting IP on allow list.  Skipping verification.");
				return;
			}
			IPAddress address = args.SmtpSession.RemoteEndPoint.Address;
			if (address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "ipv6 address found, skipping RBL checks.");
				return;
			}
			object obj;
			if (args.SmtpSession.Properties.TryGetValue("Microsoft.Exchange.IsOnSafeList", out obj))
			{
				if ((bool)obj)
				{
					return;
				}
				flag = false;
			}
			object obj2;
			if (!args.SmtpSession.Properties.TryGetValue("Microsoft.Exchange.IsOnBlockList", out obj2) || !flag2 || !(bool)obj2)
			{
				this.IssueDNSQuery(address, source, args, flag, flag2);
				return;
			}
			ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "Connecting IP address is on a blocklist.  Rejecting recipient");
			object obj3;
			if (!args.SmtpSession.Properties.TryGetValue("Microsoft.Exchange.IsOnBlockListErrorMessage", out obj3))
			{
				throw new InvalidOperationException("session property 'IsOnBlockListErrorMessage' cannot be null");
			}
			object providerName;
			if (!args.SmtpSession.Properties.TryGetValue("Microsoft..Exchange.IsOnBlockListProvider", out providerName))
			{
				throw new InvalidOperationException("session property 'IsOnBlockListProvider' cannot be null");
			}
			SmtpResponse smtpResponse = ConnectionFilteringAgent.CreateSmtpRejectionResponse((string)obj3);
			AgentLog.Instance.LogRejectCommand(base.Name, base.EventTopic, args, smtpResponse, ConnectionFilteringAgent.RejectContext.BlockListProviderEntry(providerName));
			source.RejectCommand(smtpResponse);
		}

		public void OnEndOfHeadersHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs args)
		{
			ConnectionFilteringAgent.AssertSourceAndArgsNotNull(source, args, "OnEndOfHeadersHandler");
			ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "Starting EOH handler");
			if (args.SmtpSession.IsExternalConnection)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "On Edge.  Exiting.");
				return;
			}
			bool flag;
			this.OnEndOfHeadersIPFilterHandler(source, args, out flag);
			if (flag)
			{
				this.OnEndOfHeadersRBLFilterHandler(source, args);
			}
		}

		public void OnEndOfHeadersIPFilterHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs args, out bool shouldContinue)
		{
			shouldContinue = true;
			ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "Starting IPFilter EOH handler");
			bool flag = CommonUtils.IsEnabled(this.config.AllowListConfig, args.SmtpSession);
			bool flag2 = CommonUtils.IsEnabled(this.config.BlockListConfig, args.SmtpSession);
			if (!flag && !flag2)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Block policy disabled.  Exiting.");
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(args.SmtpSession, ExTraceGlobals.OnEOHTracer, this))
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "AntispamBypass permissions are set on the session. Exiting.");
				return;
			}
			IPAddress lastExternalIPAddress = args.SmtpSession.LastExternalIPAddress;
			if (lastExternalIPAddress == null)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Deny handler: originating IP is null, skipping ip checks.");
				shouldContinue = false;
				return;
			}
			PermissionCheckResults permissionCheckResults = this.server.IPPermission.Check(lastExternalIPAddress);
			ExTraceGlobals.OnEOHTracer.TraceDebug<IPAddress, PermissionCheckResults>((long)this.GetHashCode(), "IsOnAllowOrDenyList({0}) returned: {1}", lastExternalIPAddress, permissionCheckResults);
			if (flag)
			{
				bool flag3 = (permissionCheckResults & PermissionCheckResults.Allow) != PermissionCheckResults.None;
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnAllowList"] = flag3;
				if (flag3)
				{
					ConnectionFilteringAgent.PerformanceCounters.OriginatingIPOnIPAllowList();
					shouldContinue = false;
					return;
				}
			}
			if (flag2)
			{
				bool flag4 = (permissionCheckResults & PermissionCheckResults.Deny) != PermissionCheckResults.None;
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnDenyList"] = flag4;
				if (flag4)
				{
					ConnectionFilteringAgent.PerformanceCounters.OriginatingIPOnIPBlockList();
					shouldContinue = false;
					bool flag5 = (permissionCheckResults & PermissionCheckResults.MachineDeny) != PermissionCheckResults.None;
					string message = flag5 ? this.FormatMachineRejectionResponse(lastExternalIPAddress) : this.FormatStaticRejectionResponse(lastExternalIPAddress);
					SmtpResponse smtpResponse = ConnectionFilteringAgent.CreateSmtpRejectionResponse(message);
					LogEntry logEntry = flag5 ? ConnectionFilteringAgent.RejectContext.MachineGeneratedLocalBlockListEntry : ConnectionFilteringAgent.RejectContext.AdministratorLocalBlockListEntry;
					AgentLog.Instance.LogRejectMessage(base.Name, base.EventTopic, args, args.SmtpSession, args.MailItem, smtpResponse, logEntry);
					source.RejectMessage(smtpResponse);
				}
			}
		}

		public void OnEndOfHeadersRBLFilterHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs args)
		{
			ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "Starting RBL filter handler.");
			bool flag = CommonUtils.IsEnabled(this.config.AllowListProviderConfig, args.SmtpSession);
			bool flag2 = CommonUtils.IsEnabled(this.config.BlockListProviderConfig, args.SmtpSession);
			if (!flag && !flag2)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "IP Allow/Block policy disabled.  Exiting.");
				return;
			}
			if (CommonUtils.HasAntispamBypassPermission(args.SmtpSession, ExTraceGlobals.OnEOHTracer, this))
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "AntispamBypass permissions are set on the session. Exiting.");
				return;
			}
			this.exceptionAddressCount = this.blockListProviderBypassedRecipients.NumBypassedRecipients(args.MailItem.Recipients);
			if (this.exceptionAddressCount == args.MailItem.Recipients.Count)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "All recipients are bypassed from the block list providers.");
				flag2 = false;
			}
			IPAddress lastExternalIPAddress = args.SmtpSession.LastExternalIPAddress;
			if (lastExternalIPAddress == null)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "RBL handler: originating ip is null, skipping check.");
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnSafeList"] = null;
				args.SmtpSession.Properties["Microsoft.Exchange.IsOnBlockList"] = null;
				return;
			}
			ExTraceGlobals.OnEOHTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "RBL handler: originating ip = {0}", lastExternalIPAddress);
			if (lastExternalIPAddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				ExTraceGlobals.OnEOHTracer.TraceDebug((long)this.GetHashCode(), "ipv6 address found, skipping RBL checks.");
				return;
			}
			this.crtBlockListIndex = 0;
			this.crtSafeListIndex = 0;
			this.IssueDNSQuery(lastExternalIPAddress, source, args, flag, flag2);
		}

		private static void AssertSourceAndArgsNotNull(ReceiveEventSource source, EventArgs args, string name)
		{
			if (source == null)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "The source object for {0} cannot be null", new object[]
				{
					name
				});
				throw new ArgumentNullException("source", message);
			}
			if (args == null)
			{
				string message2 = string.Format(CultureInfo.InvariantCulture, "The command arguments for {0} cannot be null", new object[]
				{
					name
				});
				throw new ArgumentNullException("args", message2);
			}
		}

		private static bool IsPropertyValueTrue(IDictionary<string, object> props, string key)
		{
			object obj;
			return props.TryGetValue(key, out obj) && obj is bool && (bool)obj;
		}

		private void IssueDNSQuery(IPAddress ip, object source, ReceiveEventArgs eventArgs, bool safeListEnabled, bool blockListEnabled)
		{
			if (!safeListEnabled && !blockListEnabled)
			{
				return;
			}
			this.asyncContext = null;
			if (TransportFacades.Dns.ServerList == null || TransportFacades.Dns.ServerList.Count == 0)
			{
				ConnectionFilteringAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_ConnectionFilteringDnsNotConfigured, null, null);
				ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "DNS server list is empty");
				return;
			}
			this.queryData = new DNSQueryData(ip, source, eventArgs, safeListEnabled, blockListEnabled);
			this.IssueNextQuery();
		}

		private void IssueNextQuery()
		{
			if (this.queryData.SafeListEnabled)
			{
				while (this.crtSafeListIndex < this.config.AllowListProviders.Length && this.crtSafeListIndex < 25)
				{
					IPListProvider iplistProvider = this.config.AllowListProviders[this.crtSafeListIndex++];
					if (iplistProvider.Enabled)
					{
						this.queryData.Provider = iplistProvider;
						string domainName = this.queryData.ReverseIP + "." + this.queryData.Provider.LookupDomain;
						if (this.asyncContext == null)
						{
							this.asyncContext = base.GetAgentAsyncContext();
						}
						TransportFacades.Dns.BeginResolveToAddresses(domainName, AddressFamily.InterNetwork, new AsyncCallback(this.SafeListDnsCallback), null);
						return;
					}
				}
				if (!this.propertyWasSet)
				{
					ExTraceGlobals.DNSTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "IP Address {0} not found on safe list. Setting IsOnSafeList = false", this.queryData.HostIP);
					this.queryData.EventArgs.SmtpSession.Properties["Microsoft.Exchange.IsOnSafeList"] = false;
					this.propertyWasSet = true;
				}
			}
			if (this.queryData.BlockListEnabled)
			{
				while (this.crtBlockListIndex < this.config.BlockListProviders.Length && this.crtSafeListIndex + this.crtBlockListIndex < 25)
				{
					IPListProvider iplistProvider2 = this.config.BlockListProviders[this.crtBlockListIndex++];
					if (iplistProvider2.Enabled)
					{
						this.queryData.Provider = iplistProvider2;
						string domainName2 = this.queryData.ReverseIP + "." + this.queryData.Provider.LookupDomain;
						if (this.asyncContext == null)
						{
							this.asyncContext = base.GetAgentAsyncContext();
						}
						TransportFacades.Dns.BeginResolveToAddresses(domainName2, AddressFamily.InterNetwork, new AsyncCallback(this.BlockListDnsCallback), null);
						return;
					}
				}
				ExTraceGlobals.DNSTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "IP Address {0} not found on block list. Setting IsOnBlockList = false", this.queryData.HostIP);
				this.queryData.EventArgs.SmtpSession.Properties["Microsoft.Exchange.IsOnBlockList"] = false;
			}
			this.CallMExAsyncCompletionIfNeeded();
		}

		public void SafeListDnsCallback(IAsyncResult ar)
		{
			this.asyncContext.Resume();
			IPAddress[] addresses;
			if (Dns.EndResolveToAddresses(ar, out addresses) == DnsStatus.Success && ConnectionFilteringAgent.MatchResult(addresses, this.queryData.Provider))
			{
				ExTraceGlobals.DNSTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "IP {0} was found on a safe list", this.queryData.HostIP);
				this.queryData.EventArgs.SmtpSession.Properties["Microsoft.Exchange.IsOnSafeList"] = true;
				if (this.queryData.CurrentEvent == SMTPEvent.RcptTo)
				{
					ConnectionFilteringAgent.PerformanceCounters.ConnectionOnIPallowListProvider();
				}
				else
				{
					ConnectionFilteringAgent.PerformanceCounters.OriginatingIPOnIPallowListProvider();
				}
				this.asyncContext.Complete();
				return;
			}
			this.IssueNextQuery();
		}

		public void BlockListDnsCallback(IAsyncResult ar)
		{
			this.asyncContext.Resume();
			IPAddress[] addresses;
			if (Dns.EndResolveToAddresses(ar, out addresses) == DnsStatus.Success && ConnectionFilteringAgent.MatchResult(addresses, this.queryData.Provider))
			{
				ExTraceGlobals.DNSTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "IP {0} was found on a block list", this.queryData.HostIP);
				string text = null;
				IPBlockListProvider ipblockListProvider = (IPBlockListProvider)this.queryData.Provider;
				if (ipblockListProvider.RejectionResponse != AsciiString.Empty)
				{
					try
					{
						text = string.Format(CultureInfo.CurrentCulture, ipblockListProvider.RejectionResponse, new object[]
						{
							this.queryData.HostIP,
							this.queryData.Provider.Name,
							this.queryData.Provider.LookupDomain
						});
						goto IL_DE;
					}
					catch (FormatException)
					{
						text = "Recipient not authorized, your IP has been found on a block list";
						goto IL_DE;
					}
				}
				text = "Recipient not authorized, your IP has been found on a block list";
				IL_DE:
				SmtpResponse smtpResponse = ConnectionFilteringAgent.CreateSmtpRejectionResponse(text);
				string name = this.queryData.Provider.Name;
				LogEntry logEntry = ConnectionFilteringAgent.RejectContext.BlockListProviderEntry(name);
				if (this.queryData.CurrentEvent == SMTPEvent.RcptTo)
				{
					ConnectionFilteringAgent.PerformanceCounters.ConnectionOnIPblockListProvider();
					SmtpSession smtpSession = ((RcptCommandEventArgs)this.queryData.EventArgs).SmtpSession;
					smtpSession.Properties["Microsoft.Exchange.IsOnBlockList"] = true;
					smtpSession.Properties["Microsoft.Exchange.IsOnBlockListErrorMessage"] = text;
					smtpSession.Properties["Microsoft..Exchange.IsOnBlockListProvider"] = name;
					AgentLog.Instance.LogRejectCommand(base.Name, base.EventTopic, (RcptCommandEventArgs)this.queryData.EventArgs, smtpResponse, logEntry);
					((ReceiveCommandEventSource)this.queryData.Source).RejectCommand(smtpResponse);
				}
				else
				{
					ConnectionFilteringAgent.PerformanceCounters.OriginatingIPOnIPblockListProvider();
					ReceiveMessageEventSource receiveMessageEventSource = (ReceiveMessageEventSource)this.queryData.Source;
					EndOfHeadersEventArgs endOfHeadersEventArgs = (EndOfHeadersEventArgs)this.queryData.EventArgs;
					endOfHeadersEventArgs.SmtpSession.Properties["Microsoft.Exchange.IsOnBlockList"] = true;
					if (this.exceptionAddressCount == 0)
					{
						AgentLog.Instance.LogRejectMessage(base.Name, base.EventTopic, endOfHeadersEventArgs, endOfHeadersEventArgs.SmtpSession, endOfHeadersEventArgs.MailItem, smtpResponse, logEntry);
						receiveMessageEventSource.RejectMessage(smtpResponse);
					}
					else
					{
						this.blockListProviderBypassedRecipients.RemoveNonBypassedRecipients(endOfHeadersEventArgs.MailItem, true, smtpResponse, base.Name, base.EventTopic, endOfHeadersEventArgs, endOfHeadersEventArgs.SmtpSession, logEntry);
					}
				}
				this.asyncContext.Complete();
				return;
			}
			this.IssueNextQuery();
		}

		private static bool MatchResult(IPAddress[] addresses, IPListProvider provider)
		{
			int i = 0;
			while (i < addresses.Length)
			{
				IPAddress ipaddress = addresses[i];
				if (!provider.AnyMatch)
				{
					if (provider.BitmaskMatch != null)
					{
						byte[] addressBytes = provider.BitmaskMatch.GetAddressBytes();
						byte[] addressBytes2 = ipaddress.GetAddressBytes();
						for (int j = 0; j < addressBytes.Length; j++)
						{
							if ((addressBytes[j] & addressBytes2[j]) != 0)
							{
								return true;
							}
						}
					}
					if (provider.IPAddressesMatch != null)
					{
						foreach (IPAddress obj in provider.IPAddressesMatch)
						{
							if (ipaddress.Equals(obj))
							{
								return true;
							}
						}
					}
					i++;
					continue;
				}
				return true;
			}
			return false;
		}

		private void CallMExAsyncCompletionIfNeeded()
		{
			if (this.asyncContext != null)
			{
				this.asyncContext.Complete();
				this.asyncContext = null;
			}
		}

		private static SmtpResponse CreateSmtpRejectionResponse(string message)
		{
			return new SmtpResponse("550", "5.7.1", new string[]
			{
				message
			});
		}

		private string FormatMachineRejectionResponse(IPAddress ipaddress)
		{
			return ConnectionFilteringAgent.FormatRejectionMessage(this.config.BlockListConfig.MachineEntryRejectionResponse, IPBlockListConfig.DefaultMachineRejectionResponse, ipaddress);
		}

		private string FormatStaticRejectionResponse(IPAddress ipaddress)
		{
			return ConnectionFilteringAgent.FormatRejectionMessage(this.config.BlockListConfig.StaticEntryRejectionResponse, IPBlockListConfig.DefaultStaticRejectionResponse, ipaddress);
		}

		private static string FormatRejectionMessage(string message, string defaultMessage, IPAddress ipaddress)
		{
			if (!string.IsNullOrEmpty(message))
			{
				try
				{
					return string.Format(CultureInfo.InvariantCulture, message, new object[]
					{
						ipaddress
					});
				}
				catch (FormatException)
				{
					return message;
				}
			}
			return string.Format(CultureInfo.InvariantCulture, defaultMessage, new object[]
			{
				ipaddress
			});
		}

		private const string DefaultErrorMessage = "Recipient not authorized, your IP has been found on a block list";

		private const int MaxQueriedProviders = 25;

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.IPAllowDenyTracer.Category, "MSExchange Antispam");

		private DNSQueryData queryData;

		private int crtSafeListIndex;

		private int crtBlockListIndex;

		private int exceptionAddressCount;

		private AgentAsyncContext asyncContext;

		private bool propertyWasSet;

		private ConnectionFilterConfig config;

		private BypassedRecipients blockListProviderBypassedRecipients;

		private SmtpServer server;

		private bool blockedByMachineGeneratedEntry;

		private static class TraceMessage
		{
			public const string BehindMTA = "Edge running behind MTA.  Exiting.";

			public const string AllowBlockDisabled = "IP Allow/Block policy disabled.  Exiting.";

			public const string SetProperty = "message property '{0}' set to {1}";

			public const string AntispamBypassSet = "AntispamBypass permissions are set on the session. Exiting.";
		}

		private static class RejectContext
		{
			public static LogEntry BlockListProviderEntry(object providerName)
			{
				return new LogEntry("BlockListProvider", providerName);
			}

			private const string LocalBlockList = "LocalBlockList";

			private const string BlockListProvider = "BlockListProvider";

			public static readonly LogEntry AdministratorLocalBlockListEntry = new LogEntry("LocalBlockList", "entry created by administrator");

			public static readonly LogEntry MachineGeneratedLocalBlockListEntry = new LogEntry("LocalBlockList", "machine-generated entry");
		}

		internal class PerformanceCounters
		{
			public static void ConnectionOnIPAllowList()
			{
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowList.Increment();
			}

			public static void ConnectionOnIPBlockList()
			{
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockList.Increment();
			}

			public static void ConnectionOnIPallowListProvider()
			{
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowProviders.Increment();
			}

			public static void ConnectionOnIPblockListProvider()
			{
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockProviders.Increment();
			}

			public static void OriginatingIPOnIPAllowList()
			{
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowList.Increment();
			}

			public static void OriginatingIPOnIPBlockList()
			{
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockList.Increment();
			}

			public static void OriginatingIPOnIPallowListProvider()
			{
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowProviders.Increment();
			}

			public static void OriginatingIPOnIPblockListProvider()
			{
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockProviders.Increment();
			}

			public static void RemoveCounters()
			{
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowList.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockList.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowProviders.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockProviders.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowList.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockList.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowProviders.RawValue = 0L;
				ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockProviders.RawValue = 0L;
			}
		}
	}
}
