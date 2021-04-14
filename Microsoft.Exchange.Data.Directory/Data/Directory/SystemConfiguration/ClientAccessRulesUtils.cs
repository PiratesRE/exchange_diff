using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class ClientAccessRulesUtils
	{
		internal static bool ShouldBlockConnection(OrganizationId organizationId, string username, ClientAccessProtocol protocol, IPEndPoint remoteEndpoint, ClientAccessAuthenticationMethod authenticationType, Action<ClientAccessRulesEvaluationContext> blockLoggerDelegate, Action<double> latencyLoggerDelegate)
		{
			return ClientAccessRulesUtils.ShouldBlockConnection(organizationId, username, protocol, remoteEndpoint, authenticationType, null, blockLoggerDelegate, latencyLoggerDelegate);
		}

		internal static bool ShouldBlockConnection(OrganizationId organizationId, string username, ClientAccessProtocol protocol, IPEndPoint remoteEndpoint, ClientAccessAuthenticationMethod authenticationType, IReadOnlyPropertyBag propertyBag, Action<ClientAccessRulesEvaluationContext> blockLoggerDelegate, Action<double> latencyLoggerDelegate)
		{
			DateTime utcNow = DateTime.UtcNow;
			bool shouldBlock = false;
			long ticks = utcNow.Ticks;
			if (organizationId == null)
			{
				ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(ticks, "[Client Access Rules] ShouldBlockConnection assuming OrganizationId.ForestWideOrgId for null OrganizationId");
				organizationId = OrganizationId.ForestWideOrgId;
			}
			if (remoteEndpoint != null)
			{
				ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(ticks, "[Client Access Rules] ShouldBlockConnection - Initializing context to run rules");
				ClientAccessRuleCollection collection = ClientAccessRulesCache.Instance.GetCollection(organizationId);
				ClientAccessRulesEvaluationContext context = new ClientAccessRulesEvaluationContext(collection, username, remoteEndpoint, protocol, authenticationType, propertyBag, ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>(), delegate(ClientAccessRulesEvaluationContext evaluationContext)
				{
					shouldBlock = true;
					blockLoggerDelegate(evaluationContext);
				}, null, ticks);
				collection.Run(context);
			}
			ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCalls.Increment();
			if (shouldBlock)
			{
				ClientAccessRulesPerformanceCounters.TotalConnectionsBlockedByClientAccessRules.Increment();
			}
			double totalMilliseconds = (DateTime.UtcNow - utcNow).TotalMilliseconds;
			latencyLoggerDelegate(totalMilliseconds);
			if (totalMilliseconds > 50.0)
			{
				ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCallsOver50ms.Increment();
			}
			if (totalMilliseconds > 10.0)
			{
				ClientAccessRulesPerformanceCounters.TotalClientAccessRulesEvaluationCallsOver10ms.Increment();
			}
			ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(ticks, string.Format("[Client Access Rules] ShouldBlockConnection - Evaluate - Org: {0} - Protocol: {1} - Username: {2} - IP: {3} - Port: {4} - Auth Type: {5} - Blocked: {6} - Latency: {7}", new object[]
			{
				organizationId.ToString(),
				protocol.ToString(),
				username.ToString(),
				remoteEndpoint.Address.ToString(),
				remoteEndpoint.Port.ToString(),
				authenticationType.ToString(),
				shouldBlock.ToString(),
				totalMilliseconds.ToString()
			}));
			return shouldBlock;
		}

		internal static IPEndPoint GetRemoteEndPointFromContext(HttpContext httpContext)
		{
			int remotePortFromContext = ClientAccessRulesUtils.GetRemotePortFromContext(httpContext);
			IPAddress remoteIPAddressFromContext = ClientAccessRulesUtils.GetRemoteIPAddressFromContext(httpContext);
			if (httpContext == null || remoteIPAddressFromContext == null || remotePortFromContext == 0)
			{
				return null;
			}
			return new IPEndPoint(remoteIPAddressFromContext, remotePortFromContext);
		}

		private static int GetRemotePortFromContext(HttpContext httpContext)
		{
			string text = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_PORT"];
			int result;
			if (text != null)
			{
				text = text.Split(new char[]
				{
					','
				}).Last<string>();
				if (int.TryParse(text, out result))
				{
					return result;
				}
			}
			if (int.TryParse(httpContext.Request.ServerVariables["REMOTE_PORT"], out result))
			{
				return result;
			}
			return 0;
		}

		private static IPAddress GetRemoteIPAddressFromContext(HttpContext httpContext)
		{
			if (httpContext == null || httpContext.Request == null || httpContext.Request.ServerVariables == null)
			{
				return null;
			}
			string text = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			IPAddress result;
			if (text != null)
			{
				text = text.Split(new char[]
				{
					','
				}).Last<string>();
				if (IPAddress.TryParse(text, out result))
				{
					return result;
				}
			}
			if (IPAddress.TryParse(httpContext.Request.UserHostAddress, out result))
			{
				return result;
			}
			return null;
		}

		internal static string GetUsernameFromContext(HttpContext httpContext)
		{
			if (httpContext.Request.Headers[WellKnownHeader.WLIDMemberName] != null)
			{
				string text = httpContext.Request.Headers[WellKnownHeader.WLIDMemberName].ToString();
				if (!string.IsNullOrEmpty(text))
				{
					SmtpAddress smtpAddress = SmtpAddress.Parse(text);
					return string.Format("{0}\\{1}", smtpAddress.Domain, smtpAddress.Local);
				}
			}
			if (httpContext.Items.Contains("AuthenticatedUser") && httpContext.Items["AuthenticatedUser"] != null)
			{
				return httpContext.Items["AuthenticatedUser"].ToString();
			}
			return string.Empty;
		}

		internal static string GetUsernameFromADRawEntry(ADRawEntry rawEntry)
		{
			SmtpAddress smtpAddress = SmtpAddress.Empty;
			if (rawEntry[ADRecipientSchema.WindowsLiveID] != null)
			{
				smtpAddress = (SmtpAddress)rawEntry[ADRecipientSchema.WindowsLiveID];
				if (smtpAddress.IsValidAddress)
				{
					return ClientAccessRulesUtils.GetUsernameFromWindowsLiveId(smtpAddress);
				}
			}
			return ClientAccessRulesUtils.GetUsernameFromIdInformation(smtpAddress, (SecurityIdentifier)rawEntry[ADRecipientSchema.MasterAccountSid], (SecurityIdentifier)rawEntry[ADMailboxRecipientSchema.Sid], rawEntry.Id);
		}

		internal static string GetUsernameFromADObjectId(ADObjectId adObjectId)
		{
			if (adObjectId == null || adObjectId.DomainId == null)
			{
				return string.Empty;
			}
			return string.Format("{0}\\{1}", adObjectId.DomainId.Name, adObjectId.Name);
		}

		internal static string GetUsernameFromWindowsLiveId(SmtpAddress smtpAddress)
		{
			return string.Format("{0}\\{1}", smtpAddress.Domain, smtpAddress.Local);
		}

		internal static string GetUsernameFromIdInformation(SmtpAddress liveId, SecurityIdentifier masterAccountSid, SecurityIdentifier sid, ADObjectId adObjectId)
		{
			if (liveId.IsValidAddress)
			{
				return ClientAccessRulesUtils.GetUsernameFromWindowsLiveId(liveId);
			}
			if (masterAccountSid != null)
			{
				return SidToAccountMap.Singleton.Get(masterAccountSid);
			}
			if (sid != null)
			{
				return SidToAccountMap.Singleton.Get(sid);
			}
			return ClientAccessRulesUtils.GetUsernameFromADObjectId(adObjectId);
		}

		internal static ClientAccessRule GetAllowLocalClientAccessRule()
		{
			return new ADClientAccessRule
			{
				Name = "[Allow Local Connections In-Memory Hardcoded Rule]",
				Priority = 1,
				Enabled = true,
				DatacenterAdminsOnly = true,
				Action = ClientAccessRulesAction.AllowAccess,
				AnyOfClientIPAddressesOrRanges = ClientAccessRulesUtils.GetAllLocalIPAddresses()
			}.GetClientAccessRule();
		}

		private static IPRange[] GetAllLocalIPAddresses()
		{
			List<IPRange> list = new List<IPRange>();
			try
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in from a in allNetworkInterfaces
				where a.OperationalStatus == OperationalStatus.Up
				select a)
				{
					IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
					UnicastIPAddressInformationCollection unicastAddresses = ipproperties.UnicastAddresses;
					foreach (IPAddressInformation ipaddressInformation in unicastAddresses.OrderBy((UnicastIPAddressInformation ua) => ua.Address.AddressFamily))
					{
						if (!ipaddressInformation.IsTransient)
						{
							if (ipaddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
							{
								list.Add(IPRange.Parse(ipaddressInformation.Address.ToString()));
							}
							else if (ipaddressInformation.Address.AddressFamily == AddressFamily.InterNetworkV6)
							{
								list.Add(IPRange.Parse(ipaddressInformation.Address.ToString()));
							}
						}
					}
				}
			}
			catch (NetworkInformationException ex)
			{
				ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(0L, string.Format("[Client Access Rules] GetAllLocalIPAddresses threw an unexpected exception ({0})", ex.ToString()));
			}
			return list.Distinct<IPRange>().ToArray<IPRange>();
		}

		private const string AuthenticatedUserItemName = "AuthenticatedUser";

		private const string UsernameFormatString = "{0}\\{1}";
	}
}
