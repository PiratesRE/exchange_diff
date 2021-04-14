using System;
using System.Collections;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class ProxyTest
	{
		public override string ToString()
		{
			return this.proxyChainConfig[this.proxyChainConfig.Length - 2].Endpoint.ToString();
		}

		public ProxyTest(StsWorkItem.EndOPDetectionCallback endOPDetectionCallback, IEnumerator proxyEnumerator)
		{
			this.endOPDetectionCallback = endOPDetectionCallback;
			this.detectionResult = OPDetectionResult.Unknown;
			this.m_proxyEnumerator = proxyEnumerator;
		}

		public OPDetectionResult BeginOPDetection(ProxyEndPoint[] path, ProxyType lastType, NetworkCredential lastAuth, IPEndPoint hostEndpoint, IPAddress target)
		{
			this.sender = target;
			this.proxyChainConfig = new ProxyEndPoint[(path == null) ? 2 : (path.Length + 2)];
			if (path != null)
			{
				for (int i = 0; i < path.Length; i++)
				{
					this.proxyChainConfig[i] = path[i];
				}
			}
			this.proxyChainConfig[this.proxyChainConfig.Length - 2] = new ProxyEndPoint(target, 0, lastType, lastAuth);
			this.proxyChainConfig[this.proxyChainConfig.Length - 1] = new ProxyEndPoint(hostEndpoint, ProxyType.None, new NetworkCredential());
			return this.StartNextDetection();
		}

		public void DetectionChainResult(OPDetectionResult result, ProxyType type, int port)
		{
			switch (result)
			{
			case OPDetectionResult.IsOpenProxy:
				this.positiveProxyType = type;
				this.positivePort = port;
				this.detectionResult = result;
				break;
			case OPDetectionResult.NotOpenProxy:
				if (this.detectionResult != OPDetectionResult.IsOpenProxy)
				{
					this.detectionResult = result;
				}
				break;
			}
			this.StartNextDetection();
		}

		private void DetectOpenProxy(ushort port, ProxyType targetType)
		{
			this.proxyChainConfig[this.proxyChainConfig.Length - 2].Endpoint.Port = (int)port;
			this.proxyChainConfig[this.proxyChainConfig.Length - 1].Type = targetType;
			this.chain = new ProxyChain(this.proxyChainConfig, this, "220 ");
			this.chain.DetectOpenProxy(10000);
		}

		private OPDetectionResult StartNextDetection()
		{
			if (this.detectionResult != OPDetectionResult.IsOpenProxy && !ProtocolAnalysisBgAgent.ShutDown)
			{
				while (this.m_portEnumerator == null || !this.m_portEnumerator.MoveNext())
				{
					this.m_portEnumerator = null;
					if (!this.m_proxyEnumerator.MoveNext())
					{
						goto IL_A7;
					}
					this.proxyType = (ProxyType)this.m_proxyEnumerator.Current;
					this.portList = ProtocolAnalysisBgAgent.GetProxyPortList(this.proxyType);
					this.m_portEnumerator = this.portList.GetEnumerator();
					this.m_portEnumerator.Reset();
				}
				ushort port = (ushort)this.m_portEnumerator.Current;
				this.DetectOpenProxy(port, this.proxyType);
				return OPDetectionResult.Pending;
			}
			IL_A7:
			string message = (this.detectionResult == OPDetectionResult.IsOpenProxy) ? (this.positiveProxyType.ToString() + ":" + this.positivePort) : string.Empty;
			this.endOPDetectionCallback(this.detectionResult, this.positiveProxyType, message, this.sender);
			return this.detectionResult;
		}

		private OPDetectionResult detectionResult;

		private ProxyType positiveProxyType;

		private int positivePort;

		private ProxyEndPoint[] proxyChainConfig;

		private ProxyChain chain;

		private ushort[] portList;

		private IEnumerator m_portEnumerator;

		private IEnumerator m_proxyEnumerator;

		private ProxyType proxyType;

		private IPAddress sender;

		private StsWorkItem.EndOPDetectionCallback endOPDetectionCallback;
	}
}
