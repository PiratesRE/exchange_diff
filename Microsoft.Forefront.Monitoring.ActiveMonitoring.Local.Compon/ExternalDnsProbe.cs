using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ExternalDnsProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			ExternalDnsProbe.RefreshExternalDnsServerList();
			string text = base.Definition.Attributes["Text"];
			CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(delegate()
			{
				this.resolveFinished.Set();
			});
			using (cancellationTokenRegistration)
			{
				IAsyncResult ar2 = ExternalDnsProbe.dnsResolver.BeginResolveToMailServers(text, DnsQueryOptions.BypassCache, delegate(IAsyncResult ar)
				{
					this.resolveFinished.Set();
				}, text);
				this.resolveFinished.WaitOne();
				if (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException(cancellationToken);
				}
				this.ResolveComplete(ar2);
			}
		}

		private static void RefreshExternalDnsServerList()
		{
			bool flag = false;
			DateTime dateTime = ExternalDnsProbe.lastRefreshTS;
			if ((DateTime.UtcNow - ExternalDnsProbe.lastRefreshTS).TotalSeconds >= 3600.0)
			{
				flag = true;
			}
			if (flag)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 98, "RefreshExternalDnsServerList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\ExternalDnsProbe.cs");
				Server server = topologyConfigurationSession.FindLocalServer();
				if (server == null)
				{
					throw new Exception("FindLocalServer() failed");
				}
				ADObjectId childId = server.Id.GetChildId("Transport Configuration");
				ADObjectId childId2 = childId.GetChildId("Frontend");
				FrontendTransportServer frontendTransportServer = topologyConfigurationSession.Read<FrontendTransportServer>(childId2);
				if (frontendTransportServer == null)
				{
					throw new Exception("this is not a Frontend server");
				}
				if (frontendTransportServer.ExternalDNSAdapterEnabled || MultiValuedPropertyBase.IsNullOrEmpty(frontendTransportServer.ExternalDNSServers))
				{
					ExternalDnsProbe.dnsResolver.AdapterServerList(frontendTransportServer.ExternalDNSAdapterGuid);
				}
				else if (frontendTransportServer.ExternalDNSServers != null && frontendTransportServer.ExternalDNSServers.Count > 0)
				{
					IPAddress[] array = new IPAddress[frontendTransportServer.ExternalDNSServers.Count];
					frontendTransportServer.ExternalDNSServers.CopyTo(array, 0);
					ExternalDnsProbe.dnsResolver.InitializeServerList(array);
				}
				ExternalDnsProbe.lastRefreshTS = DateTime.UtcNow;
			}
		}

		private void ResolveComplete(IAsyncResult ar)
		{
			TargetHost[] array;
			DnsStatus dnsStatus = Dns.EndResolveToMailServers(ar, out array);
			if (dnsStatus != DnsStatus.Success)
			{
				string arg = (string)ar.AsyncState;
				throw new Exception(string.Format("External DNS MX query for {0} failed with status = {1}", arg, dnsStatus));
			}
		}

		private static Dns dnsResolver = new Dns();

		private static DateTime lastRefreshTS;

		private ManualResetEvent resolveFinished = new ManualResetEvent(false);
	}
}
