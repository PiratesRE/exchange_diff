using System;
using System.Net.Sockets;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInComponent : IStartableTransportComponent, ITransportComponent
	{
		bool SelfListening { set; }

		ServiceState TargetRunningState { get; }

		void UpdateTime(DateTime time);

		void FlushProtocolLog();

		void RejectCommands();

		void RejectSubmits();

		bool HandleConnection(Socket connection);

		void SetLoadTimeDependencies(TransportAppConfig appConfig, ITransportConfiguration transportConfig);

		void SetRunTimeDependencies(IAgentRuntime agentRuntime, IMailRouter mailRouter, IProxyHubSelector proxyHubSelector, IEnhancedDns enhancedDns, ICategorizer categorizer, ICertificateCache certificateCache, ICertificateValidator certificateValidator, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessagingDatabase messagingDatabase, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, SmtpOutConnectionHandler smtpOutConnectionHandler, IQueueQuotaComponent queueQuotaComponent);

		void Pause(bool rejectSubmits, SmtpResponse reasonForPause);

		void SetThrottleDelay(TimeSpan throttleDelay, string throttleDelayContext);
	}
}
