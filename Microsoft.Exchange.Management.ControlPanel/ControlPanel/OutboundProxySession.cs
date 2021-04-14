using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class OutboundProxySession : RbacSession
	{
		private OutboundProxySession(RbacContext context, IEnumerable<Uri> serviceUrls) : base(context, OutboundProxySession.sessionPerfCounters, OutboundProxySession.esoSessionPerfCounters)
		{
			this.ServiceUrls = serviceUrls.ToArray<Uri>();
			this.serviceUrlProxyQueue = new ProxyQueue<Uri>(this.ServiceUrls);
		}

		public IEnumerable<Uri> ServiceUrls { get; private set; }

		protected override void WriteInitializationLog()
		{
			string[] value = (from url in this.ServiceUrls
			select url.Host).ToArray<string>();
			string text = string.Join("', '", value);
			ExTraceGlobals.RBACTracer.TraceInformation<OutboundProxySession, string>(0, 0L, "{0} created to proxy calls to '{1}'.", this, text);
			EcpEventLogConstants.Tuple_OutboundProxySessionInitialize.LogEvent(new object[]
			{
				base.NameForEventLog,
				text
			});
		}

		public override void RequestReceived()
		{
			base.RequestReceived();
			HttpContext.Current.RemapHandler(new ProxyHandler(this));
		}

		private IEnumerable<ProxyConnection> GetProxyConnections()
		{
			foreach (Uri uri in this.serviceUrlProxyQueue)
			{
				ProxyConnection connection = this.GetProxyConnection(uri);
				if (connection.IsCompatible)
				{
					yield return connection;
				}
			}
			yield break;
		}

		private ProxyConnection GetProxyConnection(Uri serviceUrl)
		{
			return this.proxyConnections.AddIfNotExists(serviceUrl, delegate(Uri url)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation<Uri>(0, 0L, "Creating ProxyConnection for {0}", url);
				return new ProxyConnection(url);
			});
		}

		public IAsyncResult BeginSendOutboundProxyRequest(HttpContext context, AsyncCallback requestCompletedCallback, object requestCompletedData)
		{
			return new OutboundProxyRequest(this.GetProxyConnections(), context, requestCompletedCallback, requestCompletedData);
		}

		public void EndSendOutboundProxyRequest(IAsyncResult result)
		{
			OutboundProxyRequest outboundProxyRequest = (OutboundProxyRequest)result;
			outboundProxyRequest.AsyncWaitHandle.WaitOne();
			if (outboundProxyRequest.Exception != null || outboundProxyRequest.AllServersFailed)
			{
				throw new ProxyFailureException(outboundProxyRequest.Context.Request.Url, base.ExecutingUserId, outboundProxyRequest.Exception);
			}
		}

		private static PerfCounterGroup sessionsCounters = new PerfCounterGroup(EcpPerfCounters.OutboundProxySessions, EcpPerfCounters.OutboundProxySessionsPeak, EcpPerfCounters.OutboundProxySessionsTotal);

		private static PerfCounterGroup requestsCounters = new PerfCounterGroup(EcpPerfCounters.OutboundProxyRequests, EcpPerfCounters.OutboundProxyRequestsPeak, EcpPerfCounters.OutboundProxyRequestsTotal);

		private static PerfCounterGroup esoSessionsCounters = new PerfCounterGroup(EcpPerfCounters.EsoOutboundProxySessions, EcpPerfCounters.EsoOutboundProxySessionsPeak, EcpPerfCounters.EsoOutboundProxySessionsTotal);

		private static PerfCounterGroup esoRequestsCounters = new PerfCounterGroup(EcpPerfCounters.EsoOutboundProxyRequests, EcpPerfCounters.EsoOutboundProxyRequestsPeak, EcpPerfCounters.EsoOutboundProxyRequestsTotal);

		private static SessionPerformanceCounters sessionPerfCounters = new SessionPerformanceCounters(OutboundProxySession.sessionsCounters, OutboundProxySession.requestsCounters);

		private static EsoSessionPerformanceCounters esoSessionPerfCounters = new EsoSessionPerformanceCounters(OutboundProxySession.sessionsCounters, OutboundProxySession.requestsCounters, OutboundProxySession.esoSessionsCounters, OutboundProxySession.esoRequestsCounters);

		private ProxyQueue<Uri> serviceUrlProxyQueue;

		private SynchronizedDictionary<Uri, ProxyConnection> proxyConnections = new SynchronizedDictionary<Uri, ProxyConnection>();

		public new sealed class Factory : RbacSession.Factory
		{
			public Factory(RbacContext context) : base(context)
			{
				if (OutboundProxySession.Factory.ProxyToLocalHost)
				{
					ExTraceGlobals.ProxyTracer.TraceInformation(0, 0L, "ProxyToLocalHost is True, so we will always proxy back to ourselves.");
					if (OutboundProxySession.Factory.ProxyToLocalHostUris == null)
					{
						OutboundProxySession.Factory.ProxyToLocalHostUris = new Uri[]
						{
							new Uri(HttpContext.Current.Request.Url, HttpRuntime.AppDomainAppVirtualPath)
						};
					}
					this.ProxyTargets = OutboundProxySession.Factory.ProxyToLocalHostUris;
					return;
				}
				OutboundProxySession.Factory.<>c__DisplayClassf CS$<>8__locals1 = new OutboundProxySession.Factory.<>c__DisplayClassf();
				CS$<>8__locals1.allowProxyingWithoutSsl = Registry.AllowProxyingWithoutSsl;
				CS$<>8__locals1.mailboxMajorVersion = base.Context.MailboxServerVersion.Major;
				using (StringWriter decisionLogWriter = new StringWriter())
				{
					IList<EcpService> servicesInMailboxSite = base.Context.GetServicesInMailboxSite(ClientAccessType.Internal, delegate(EcpService service)
					{
						decisionLogWriter.WriteLine(service.Url);
						Version version = new ServerVersion(service.ServerVersionNumber);
						DecisionLogger decisionLogger = new DecisionLogger(decisionLogWriter)
						{
							{
								(service.AuthenticationMethod & AuthenticationMethod.WindowsIntegrated) == AuthenticationMethod.WindowsIntegrated,
								Strings.ProxyServiceConditionWindowsAuth
							},
							{
								version.Major == CS$<>8__locals1.mailboxMajorVersion,
								Strings.ProxyServiceConditionMajorVersion(version.Major, CS$<>8__locals1.mailboxMajorVersion)
							},
							{
								service.Url.IsAbsoluteUri && !service.Url.IsLoopback && (service.Url.Scheme == Uri.UriSchemeHttps || service.Url.Scheme == Uri.UriSchemeHttp),
								Strings.ProxyServiceConditionInvalidUri
							},
							{
								CS$<>8__locals1.allowProxyingWithoutSsl || service.Url.Scheme == "https",
								Strings.ProxyServiceConditionSchemeUri(Environment.MachineName, service.ServerFullyQualifiedDomainName)
							}
						};
						decisionLogWriter.WriteLine();
						return decisionLogger.Decision;
					});
					this.ProxyTargets = (from service in servicesInMailboxSite
					select service.Url).ToArray<Uri>();
					this.decisionLog = decisionLogWriter.GetStringBuilder().ToString();
				}
			}

			public IList<Uri> ProxyTargets { get; private set; }

			protected override bool CanCreateSession()
			{
				if (this.ProxyTargets.Count == 0)
				{
					ExTraceGlobals.ProxyTracer.TraceInformation<string, string>(0, 0L, "No internal CAS was found in the mailbox site {0} that could serve as proxy target. Services considered:\r\n{1}", "[mailbox site]", this.decisionLog);
					EcpEventLogConstants.Tuple_EcpProxyCantFindCasServer.LogPeriodicEvent("[mailbox site]", new object[]
					{
						base.Settings.UserName,
						"[current site]",
						"[mailbox site]",
						this.decisionLog
					});
					throw new ProxyCantFindCasServerException(base.Settings.UserName, "[current site]", "[mailbox site]", this.decisionLog);
				}
				return true;
			}

			protected override RbacSession CreateNewSession()
			{
				return new OutboundProxySession(base.Context, this.ProxyTargets);
			}

			public static bool ProxyToLocalHost = StringComparer.OrdinalIgnoreCase.Equals("true", ConfigurationManager.AppSettings["ProxyToLocalHost"]);

			public static Uri[] ProxyToLocalHostUris;

			private string decisionLog;
		}
	}
}
