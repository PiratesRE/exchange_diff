using System;
using System.Net.Sockets;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInServer
	{
		string Name { get; }

		Version Version { get; }

		ServiceState TargetRunningState { get; set; }

		TransportConfigContainer TransportSettings { get; }

		ITransportConfiguration Configuration { get; }

		Server ServerConfiguration { get; }

		bool IsBridgehead { get; }

		ICertificateValidator CertificateValidator { get; }

		void SetRejectState(bool rejectCommands, bool rejectMailSubmission, bool rejectMailFromInternet, SmtpResponse rejectionResponse);

		bool RejectCommands { get; }

		bool RejectSubmits { get; }

		bool RejectMailFromInternet { get; }

		SmtpResponse RejectionSmtpResponse { get; }

		DateTime CurrentTime { get; set; }

		IShadowRedundancyManager ShadowRedundancyManager { get; }

		ICategorizer Categorizer { get; }

		IInboundProxyDestinationTracker InboundProxyDestinationTracker { get; }

		IInboundProxyDestinationTracker InboundProxyAccountForestTracker { get; }

		ICertificateCache CertificateCache { get; }

		SmtpProxyPerfCountersWrapper ClientProxyPerfCounters { get; }

		SmtpProxyPerfCountersWrapper OutboundProxyPerfCounters { get; }

		OutboundProxyBySourceTracker OutboundProxyBySourceTracker { get; }

		SmtpOutConnectionHandler SmtpOutConnectionHandler { get; }

		ISmtpInMailItemStorage MailItemStorage { get; }

		void SetThrottleState(TimeSpan perMessageDelay, string diagnosticContext);

		TimeSpan ThrottleDelay { get; }

		string ThrottleDelayContext { get; }

		IProxyHubSelector ProxyHubSelector { get; }

		ISmtpReceiveConfiguration ReceiveConfiguration { get; }

		IPConnectionTable InboundTlsIPConnectionTable { get; }

		bool Ipv6ReceiveConnectionThrottlingEnabled { get; }

		bool ReceiveTlsThrottlingEnabled { get; }

		IEventNotificationItem EventNotificationItem { get; }

		void RemoveConnection(long id);

		string CurrentState { get; }

		void SetRunTimeDependencies(IAgentRuntime agentRuntime, IMailRouter mailRouter, IProxyHubSelector proxyHubSelector, IEnhancedDns enhancedDns, ICategorizer categorizer, ICertificateCache certificateCache, ICertificateValidator certificateValidator, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, ISmtpInMailItemStorage mailItemStorage, SmtpOutConnectionHandler smtpOutConnectionHandler, IQueueQuotaComponent queueQuotaComponent);

		void SetLoadTimeDependencies(IProtocolLog protocolLog, ITransportAppConfig transportAppConfig, ITransportConfiguration configuration);

		void Load();

		void Unload();

		void Initialize(TcpListener.HandleFailure failureDelegate = null, TcpListener.HandleConnection connectionHandler = null);

		void Shutdown();

		void NonGracefullyCloseTcpListener();

		INetworkConnection CreateNetworkConnection(Socket socket, int receiveBufferSize);

		bool HandleConnection(INetworkConnection connection);

		void AddDiagnosticInfo(DiagnosableParameters parameters, XElement element);
	}
}
