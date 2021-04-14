using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BasePullRequest : BaseRequest
	{
		[DataMember(IsRequired = true)]
		[XmlElement]
		public string SubscriptionId { get; set; }

		internal static SubscriptionId ParseSubscriptionId(string subscriptionId)
		{
			SubscriptionId result;
			try
			{
				result = Microsoft.Exchange.Services.Core.Types.SubscriptionId.Parse(subscriptionId);
			}
			catch (InvalidSubscriptionException)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>(0L, "[BasePullRequest::ParseSubscriptionId] Failed to parse subscription id string: '{0}'", subscriptionId ?? "<NULL>");
				result = null;
			}
			return result;
		}

		internal static WebServicesInfo[] PerformServiceDiscoveryForSubscriptionId(string subscriptionId, CallContext callContext, BaseRequest request)
		{
			WebServicesInfo proxyToSelfCASIfNeeded = request.GetProxyToSelfCASIfNeeded();
			if (proxyToSelfCASIfNeeded != null)
			{
				return new WebServicesInfo[]
				{
					proxyToSelfCASIfNeeded
				};
			}
			SubscriptionId subscriptionId2 = BasePullRequest.ParseSubscriptionId(subscriptionId);
			if (subscriptionId2 == null)
			{
				return null;
			}
			if (subscriptionId2.MailboxGuid != Guid.Empty)
			{
				MailboxIdServerInfo mailboxIdServerInfo = MailboxIdServerInfo.Create(new MailboxId(subscriptionId2.MailboxGuid));
				if (mailboxIdServerInfo != null && !string.IsNullOrEmpty(mailboxIdServerInfo.ServerFQDN) && !string.Equals(mailboxIdServerInfo.ServerFQDN, subscriptionId2.ServerFQDN, StringComparison.OrdinalIgnoreCase))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "SubscribedMailboxFailedOver", subscriptionId2.MailboxGuid);
					throw new SubscriptionNotFoundException();
				}
			}
			if (string.Compare(LocalServer.GetServer().Fqdn, subscriptionId2.ServerFQDN, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return null;
			}
			WebServicesInfo casserviceForServer = SingleProxyDeterministicCASBoxScoring.GetCASServiceForServer(subscriptionId2.ServerFQDN);
			if (casserviceForServer == null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string>((long)request.GetHashCode(), "[NotificationCommandBase::EvaluateSubscriptionProxy] Tried to get WebServicesInfo instance for FQDN {0}, but failed.", subscriptionId2.ServerFQDN);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "ServerInSubscriptionId", subscriptionId2.ServerFQDN);
				ProxyEventLogHelper.LogNoApplicableDestinationCAS(subscriptionId2.ServerFQDN);
				throw new SubscriptionNotFoundException();
			}
			return new WebServicesInfo[]
			{
				casserviceForServer
			};
		}

		internal override WebServicesInfo[] PerformServiceDiscovery(CallContext callContext)
		{
			return BasePullRequest.PerformServiceDiscoveryForSubscriptionId(this.SubscriptionId, callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
