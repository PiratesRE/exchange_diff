using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.MessageSecurity;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInServerState
	{
		public SmtpInServerState()
		{
			this.CurrentTime = DateTime.UtcNow;
			this.ServiceState = ServiceState.Inactive;
			this.RejectionSmtpResponse = SmtpResponse.Empty;
			this.ThrottleDelay = TimeSpan.Zero;
			this.localIPAddresses = new Lazy<IReadOnlyList<IPAddress>>(new Func<IReadOnlyList<IPAddress>>(this.DetermineLocalIPAddresses));
		}

		public SmtpInServerState(ISmtpInServer smtpInServer)
		{
			ArgumentValidator.ThrowIfNull("smtpInServer", smtpInServer);
			this.CurrentTime = smtpInServer.CurrentTime;
			this.ServiceState = smtpInServer.TargetRunningState;
			this.ThrottleDelay = smtpInServer.ThrottleDelay;
			this.ThrottleDelayContext = smtpInServer.ThrottleDelayContext;
			this.RejectCommands = smtpInServer.RejectCommands;
			this.RejectMailFromInternet = smtpInServer.RejectMailFromInternet;
			this.RejectSubmits = smtpInServer.RejectSubmits;
			this.RejectionSmtpResponse = smtpInServer.RejectionSmtpResponse;
		}

		public void SetRunTimeDependencies(IAgentRuntime theAgentRuntime, ICategorizer theCategorizer, ICertificateCache theCertificateCache, ICertificateValidator theCertificateValidator, IIsMemberOfResolver<RoutingAddress> theIsMemberOfResolver, IMessageThrottlingManager theMessageThrottlingManager, IShadowRedundancyManager theShadowRedundancyManager, ISmtpInMailItemStorage theMailItemStorage, IQueueQuotaComponent theQueueQuotaComponent)
		{
			ArgumentValidator.ThrowIfNull("theAgentRuntime", theAgentRuntime);
			ArgumentValidator.ThrowIfNull("theCategorizer", theCategorizer);
			ArgumentValidator.ThrowIfNull("theCertificateCache", theCertificateCache);
			ArgumentValidator.ThrowIfNull("theCertificateValidator", theCertificateValidator);
			ArgumentValidator.ThrowIfNull("theIsMemberOfResolver", theIsMemberOfResolver);
			ArgumentValidator.ThrowIfNull("theMessageThrottlingManager", theMessageThrottlingManager);
			ArgumentValidator.ThrowIfNull("theMailItemStorage", theMailItemStorage);
			this.agentRuntime = theAgentRuntime;
			this.categorizer = theCategorizer;
			this.certificateCache = theCertificateCache;
			this.certificateValidator = theCertificateValidator;
			this.isMemberOfResolver = theIsMemberOfResolver;
			this.messageThrottlingManager = theMessageThrottlingManager;
			this.shadowRedundancyManager = theShadowRedundancyManager;
			this.mailItemStorage = theMailItemStorage;
			this.queueQuotaComponent = theQueueQuotaComponent;
			this.maxTlsConnectionsPerMinute = this.transportConfiguration.LocalServer.TransportServer.MaxReceiveTlsRatePerMinute;
		}

		public void SetLoadTimeDependencies(IProtocolLog protocolLogToUse, ITransportAppConfig transportAppConfigToUse, ITransportConfiguration transportConfigurationToUse)
		{
			ArgumentValidator.ThrowIfNull("protocolLogToUse", protocolLogToUse);
			ArgumentValidator.ThrowIfNull("transportAppConfigToUse", transportAppConfigToUse);
			ArgumentValidator.ThrowIfNull("transportConfigurationToUse", transportConfigurationToUse);
			this.protocolLog = protocolLogToUse;
			this.transportAppConfig = transportAppConfigToUse;
			this.transportConfiguration = transportConfigurationToUse;
		}

		public bool IsLocalAddress(IPAddress ipAddress)
		{
			return IPAddress.IsLoopback(ipAddress) || this.localIPAddresses.Value.Any((IPAddress localAddress) => localAddress.Equals(ipAddress));
		}

		public IAuthzAuthorization AuthzAuthorization
		{
			get
			{
				IAuthzAuthorization result;
				if ((result = this.authzAuthorization) == null)
				{
					result = (this.authzAuthorization = this.CreateAuthzAuthorization());
				}
				return result;
			}
		}

		public IAgentRuntime AgentRuntime
		{
			get
			{
				return this.agentRuntime;
			}
		}

		public ICategorizer Categorizer
		{
			get
			{
				return this.categorizer;
			}
		}

		public ICertificateCache CertificateCache
		{
			get
			{
				return this.certificateCache;
			}
		}

		public ICertificateValidator CertificateValidator
		{
			get
			{
				return this.certificateValidator;
			}
		}

		public IDirectTrust DirectTrust
		{
			get
			{
				IDirectTrust result;
				if ((result = this.directTrust) == null)
				{
					result = (this.directTrust = this.CreateDirectTrust());
				}
				return result;
			}
		}

		public IExEventLog EventLog
		{
			get
			{
				IExEventLog result;
				if ((result = this.eventLog) == null)
				{
					result = (this.eventLog = this.CreateEventLog());
				}
				return result;
			}
		}

		public IMessageThrottlingManager MessageThrottlingManager
		{
			get
			{
				return this.messageThrottlingManager;
			}
		}

		public IPConnectionTable InboundTlsIPConnectionTable
		{
			get
			{
				IPConnectionTable result;
				if ((result = this.connectionTable) == null)
				{
					result = (this.connectionTable = this.CreateConnectionTable());
				}
				return result;
			}
		}

		public IProtocolLog ProtocolLog
		{
			get
			{
				return this.protocolLog;
			}
		}

		public IQueueQuotaComponent QueueQuotaComponent
		{
			get
			{
				return this.queueQuotaComponent;
			}
		}

		public IEventNotificationItem EventNotificationItem
		{
			get
			{
				IEventNotificationItem result;
				if ((result = this.eventNotificationItem) == null)
				{
					result = (this.eventNotificationItem = this.CreateEventNotificationItem());
				}
				return result;
			}
		}

		public bool IsDataRedactionNecessary
		{
			get
			{
				return this.GetIsDataRedactionNecessary();
			}
		}

		public IIsMemberOfResolver<RoutingAddress> IsMemberOfResolver
		{
			get
			{
				return this.isMemberOfResolver;
			}
		}

		public ISmtpMessageContextBlob MessageContextBlob
		{
			get
			{
				ISmtpMessageContextBlob result;
				if ((result = this.messageContextBlob) == null)
				{
					result = (this.messageContextBlob = this.CreateMessageContextBlob());
				}
				return result;
			}
		}

		public ITracer Tracer
		{
			get
			{
				ITracer result;
				if ((result = this.tracer) == null)
				{
					result = (this.tracer = this.CreateTracer());
				}
				return result;
			}
		}

		public IShadowRedundancyManager ShadowRedundancyManager
		{
			get
			{
				return this.shadowRedundancyManager;
			}
		}

		public ISmtpReceiveConfiguration SmtpConfiguration
		{
			get
			{
				ISmtpReceiveConfiguration result;
				if ((result = this.smtpReceiveConfiguration) == null)
				{
					result = (this.smtpReceiveConfiguration = this.CreateSmtpConfiguration());
				}
				return result;
			}
		}

		public ISmtpInMailItemStorage MailItemStorage
		{
			get
			{
				return this.mailItemStorage;
			}
		}

		public ServiceState ServiceState { get; set; }

		public void SetRejectState(bool rejectCommands, bool rejectMailSubmission, bool rejectMailFromInternet, SmtpResponse rejectionResponse)
		{
			ArgumentValidator.ThrowIfInvalidValue<SmtpResponse>("rejectionResponse", rejectionResponse, (SmtpResponse response) => (!rejectCommands && !rejectMailSubmission && !rejectMailFromInternet) || !rejectionResponse.IsEmpty);
			this.RejectCommands = rejectCommands;
			this.RejectSubmits = rejectMailSubmission;
			this.RejectMailFromInternet = rejectMailFromInternet;
			this.RejectionSmtpResponse = rejectionResponse;
		}

		public bool RejectCommands { get; private set; }

		public bool RejectSubmits { get; private set; }

		public bool RejectMailFromInternet { get; private set; }

		public SmtpResponse RejectionSmtpResponse { get; private set; }

		public DateTime CurrentTime { get; set; }

		public void SetThrottleState(TimeSpan perMessageDelay, string diagnosticContext)
		{
			this.ThrottleDelay = perMessageDelay;
			this.ThrottleDelayContext = diagnosticContext;
		}

		public TimeSpan ThrottleDelay { get; private set; }

		public string ThrottleDelayContext { get; private set; }

		protected virtual IAuthzAuthorization CreateAuthzAuthorization()
		{
			return new AuthzAuthorizationWrapper();
		}

		protected virtual ICertificateCache CreateCertificateCache()
		{
			return Components.CertificateComponent.Cache;
		}

		protected virtual IDirectTrust CreateDirectTrust()
		{
			return new DirectTrustWrapper();
		}

		protected virtual IExEventLog CreateEventLog()
		{
			return new ExEventLogWrapper(new ExEventLog(ExTraceGlobals.SmtpReceiveTracer.Category, TransportEventLog.GetEventSource()));
		}

		protected virtual IPConnectionTable CreateConnectionTable()
		{
			return new IPConnectionTable(this.maxTlsConnectionsPerMinute);
		}

		protected virtual IEventNotificationItem CreateEventNotificationItem()
		{
			return new EventNotificationItemWrapper();
		}

		protected virtual bool GetIsDataRedactionNecessary()
		{
			return Util.IsDataRedactionNecessary();
		}

		protected virtual ISmtpMessageContextBlob CreateMessageContextBlob()
		{
			return new SmtpMessageContextBlobWrapper();
		}

		protected virtual ITracer CreateTracer()
		{
			return ExTraceGlobals.SmtpReceiveTracer;
		}

		protected virtual ISmtpReceiveConfiguration CreateSmtpConfiguration()
		{
			return SmtpReceiveConfiguration.Create(this.transportAppConfig, this.transportConfiguration);
		}

		protected virtual ISmtpInMailItemStorage CreateMailItemStorage()
		{
			return new SmtpInMailItemStorage();
		}

		private IReadOnlyList<IPAddress> DetermineLocalIPAddresses()
		{
			return Util.DetermineLocalIPAddresses(this.EventLog);
		}

		private readonly Lazy<IReadOnlyList<IPAddress>> localIPAddresses;

		private int maxTlsConnectionsPerMinute;

		private IAgentRuntime agentRuntime;

		private ICategorizer categorizer;

		private ICertificateCache certificateCache;

		private ICertificateValidator certificateValidator;

		private IDirectTrust directTrust;

		private IMessageThrottlingManager messageThrottlingManager;

		private IPConnectionTable connectionTable;

		private IExEventLog eventLog;

		private ITracer tracer;

		private IAuthzAuthorization authzAuthorization;

		private ISmtpMessageContextBlob messageContextBlob;

		private IIsMemberOfResolver<RoutingAddress> isMemberOfResolver;

		private IQueueQuotaComponent queueQuotaComponent;

		private IEventNotificationItem eventNotificationItem;

		private IShadowRedundancyManager shadowRedundancyManager;

		private ISmtpInMailItemStorage mailItemStorage;

		private ISmtpReceiveConfiguration smtpReceiveConfiguration;

		private IProtocolLog protocolLog;

		private ITransportAppConfig transportAppConfig;

		private ITransportConfiguration transportConfiguration;
	}
}
