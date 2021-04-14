using System;
using System.Net.Sockets;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInComponent : ISmtpInComponent, IStartableTransportComponent, ITransportComponent, IDiagnosable
	{
		public SmtpInComponent(bool useModernSmtpStack = false)
		{
			this.log = new ProtocolLog("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version, "SMTP Receive Protocol Log", "RECV", "SmtpReceiveProtocolLogs");
			if (useModernSmtpStack)
			{
				this.server = new ModernSmtpInServer();
			}
			else
			{
				this.server = new SmtpInServer();
			}
			this.loadTimeDependenciesSet = false;
			this.runTimeDependenciesSet = false;
		}

		public bool SelfListening
		{
			set
			{
				this.selfListening = value;
			}
		}

		public string CurrentState
		{
			get
			{
				return this.server.CurrentState;
			}
		}

		public ServiceState TargetRunningState
		{
			get
			{
				return this.server.TargetRunningState;
			}
		}

		public void UpdateTime(DateTime time)
		{
			this.server.CurrentTime = time;
		}

		public void SetRunTimeDependencies(IAgentRuntime agentRuntime, IMailRouter mailRouter, IProxyHubSelector proxyHubSelector, IEnhancedDns enhancedDns, ICategorizer categorizer, ICertificateCache certificateCache, ICertificateValidator certificateValidator, IIsMemberOfResolver<RoutingAddress> memberOfResolver, IMessagingDatabase messagingDatabase, IMessageThrottlingManager messageThrottlingManager, IShadowRedundancyManager shadowRedundancyManager, SmtpOutConnectionHandler smtpOutConnectionHandler, IQueueQuotaComponent queueQuotaComponent)
		{
			if (messagingDatabase == null)
			{
				throw new ArgumentNullException("messagingDatabase");
			}
			this.messagingDatabase = messagingDatabase;
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.server.SetRunTimeDependencies(agentRuntime, mailRouter, proxyHubSelector, enhancedDns, categorizer, certificateCache, certificateValidator, memberOfResolver, messageThrottlingManager, shadowRedundancyManager, new SmtpInMailItemStorage(), smtpOutConnectionHandler, queueQuotaComponent);
			this.runTimeDependenciesSet = true;
		}

		public void SetLoadTimeDependencies(TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration)
		{
			this.networkConnectionReceiveBufferSize = transportAppConfig.SmtpReceiveConfiguration.NetworkConnectionReceiveBufferSize;
			this.server.SetLoadTimeDependencies(this.log, transportAppConfig, transportConfiguration);
			this.loadTimeDependenciesSet = true;
		}

		public void Load()
		{
			if (!this.loadTimeDependenciesSet)
			{
				throw new InvalidOperationException("load-time dependencies should be set before calling Load()");
			}
			this.server.Load();
		}

		public void Unload()
		{
			this.server.Unload();
			this.log.Close();
		}

		public string OnUnhandledException(Exception e)
		{
			this.NonGracefullyCloseTcpListener();
			this.Pause();
			this.FlushProtocolLog();
			return null;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			if (!this.runTimeDependenciesSet)
			{
				throw new InvalidOperationException("run-time dependencies should be set before calling Start()");
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "Components Start");
			this.server.TargetRunningState = targetRunningState;
			if (initiallyPaused || !this.ShouldExecute())
			{
				this.Pause(true, SmtpResponse.ServiceUnavailable);
			}
			if (this.selfListening)
			{
				this.server.Initialize(new TcpListener.HandleFailure(SmtpInComponent.OnTcpListenerFailure), new TcpListener.HandleConnection(this.HandleConnection));
			}
			else
			{
				this.server.Initialize(null, null);
			}
			this.server.CurrentTime = DateTime.UtcNow;
		}

		public void FlushProtocolLog()
		{
			if (this.log != null)
			{
				this.log.Flush();
			}
		}

		public virtual void SetThrottleDelay(TimeSpan throttleDelay, string throttleDelayContext)
		{
			if (this.server.ThrottleDelay != throttleDelay || !string.Equals(this.server.ThrottleDelayContext, throttleDelayContext, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug<TimeSpan, string>((long)this.GetHashCode(), "Changing throttling delay to '{0}' with context '{1}'.", throttleDelay, throttleDelayContext ?? string.Empty);
				this.server.SetThrottleState(throttleDelay, throttleDelayContext);
			}
		}

		public void Stop()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "SmtpIn Component Stop");
			if (this.messagingDatabase.DataSource != null)
			{
				this.messagingDatabase.DataSource.TryForceFlush();
			}
			if (this.server != null)
			{
				this.server.Shutdown();
			}
		}

		public void Pause()
		{
			this.Pause(true, SmtpResponse.ServiceUnavailable);
		}

		public virtual void Pause(bool rejectSubmits, SmtpResponse reasonForPause)
		{
			this.server.SetRejectState(rejectSubmits && (this.shadowRedundancyManager == null || !this.shadowRedundancyManager.Configuration.Enabled), rejectSubmits, true, reasonForPause);
		}

		public virtual void Continue()
		{
			if (this.ShouldExecute())
			{
				this.server.SetRejectState(false, false, false, SmtpResponse.Empty);
			}
		}

		public void RejectCommands()
		{
			this.server.SetRejectState(true, this.server.RejectSubmits, this.server.RejectMailFromInternet, SmtpResponse.ConnectionDropped);
		}

		public void RejectSubmits()
		{
			this.server.SetRejectState(this.server.RejectCommands, true, this.server.RejectMailFromInternet, SmtpResponse.ConnectionDropped);
		}

		public bool HandleConnection(Socket socket)
		{
			if (Components.ShuttingDown || this.server == null)
			{
				socket.Close();
				ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "Drop new connection since SmtpInServer isn't created yet or is shutting down");
				return false;
			}
			bool flag = false;
			INetworkConnection networkConnection = null;
			bool result;
			try
			{
				networkConnection = this.server.CreateNetworkConnection(socket, this.networkConnectionReceiveBufferSize);
				flag = this.server.HandleConnection(networkConnection);
				result = flag;
			}
			finally
			{
				if (!flag && networkConnection != null)
				{
					networkConnection.Dispose();
				}
			}
			return result;
		}

		private static void OnTcpListenerFailure(bool addressAlreadyInUseFailure)
		{
			string reason = Strings.TcpListenerError;
			bool retryAlways = true;
			Components.StopService(reason, false, retryAlways, addressAlreadyInUseFailure);
		}

		private bool ShouldExecute()
		{
			return this.server.TargetRunningState == ServiceState.Active || this.server.TargetRunningState == ServiceState.Inactive;
		}

		private void NonGracefullyCloseTcpListener()
		{
			this.server.NonGracefullyCloseTcpListener();
		}

		public string GetDiagnosticComponentName()
		{
			return "SmtpIn";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			this.server.AddDiagnosticInfo(parameters, xelement);
			return xelement;
		}

		private readonly ISmtpInServer server;

		private bool selfListening;

		private readonly ProtocolLog log;

		private IMessagingDatabase messagingDatabase;

		private IShadowRedundancyManager shadowRedundancyManager;

		private bool loadTimeDependenciesSet;

		private bool runTimeDependenciesSet;

		private int networkConnectionReceiveBufferSize = 4096;
	}
}
