using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentComponent : IAgentRuntime, ITransportComponent, IDiagnosable
	{
		public AcceptedDomainCollection FirstOrgAcceptedDomains
		{
			get
			{
				return Components.Configuration.FirstOrgAcceptedDomainTable;
			}
		}

		public RemoteDomainCollection RemoteDomains
		{
			get
			{
				return Components.Configuration.RemoteDomainTable;
			}
		}

		public void Load()
		{
			string configFile = null;
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub || Components.Configuration.ProcessTransportRole == ProcessTransportRole.Edge)
			{
				configFile = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config");
			}
			else if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.FrontEnd)
			{
				configFile = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\fetagents.config");
			}
			try
			{
				this.mexRuntime.Initialize(configFile, "Microsoft.Exchange.Data.Transport.Smtp.SmtpReceiveAgent", Components.Configuration.ProcessTransportRole, ConfigurationContext.Setup.InstallPath, new FactoryInitializer(ProcessAccessManager.RegisterAgentFactory));
				AgentLatencyTracker.RegisterMExRuntime(LatencyAgentGroup.SmtpReceive, this.mexRuntime);
			}
			catch (ExchangeConfigurationException ex)
			{
				Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_CannotStartAgents, null, new object[]
				{
					ex.LocalizedString,
					ex
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, "Microsoft Exchange couldn't start transport agents.", ResultSeverityLevel.Error, false);
				throw new TransportComponentLoadFailedException(Strings.AgentComponentFailed, ex);
			}
		}

		public void Unload()
		{
			try
			{
				this.mexRuntime.Shutdown();
			}
			catch (InvalidOperationException)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug(0L, "MExEvents.Shutdown threw InvalidOperationException: ongoing sessions.");
			}
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public ISmtpAgentSession NewSmtpAgentSession(ISmtpInSession smtpInSession, INetworkConnection networkConnection, bool isExternalConnection)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			ArgumentValidator.ThrowIfNull("networkConnection", networkConnection);
			SmtpReceiveServer smtpReceiveServer = SmtpReceiveServer.FromSmtpInSession(smtpInSession, Components.AgentComponent.FirstOrgAcceptedDomains, Components.AgentComponent.RemoteDomains, Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion);
			return new SmtpAgentSession(this.mexRuntime, smtpReceiveServer, smtpInSession, new SmtpSessionImpl(smtpInSession, networkConnection, isExternalConnection));
		}

		public ISmtpAgentSession NewSmtpAgentSession(SmtpInSessionState sessionState, IIsMemberOfResolver<RoutingAddress> isMemberOfResolver, AcceptedDomainCollection firstOrgAcceptedDomains, RemoteDomainCollection remoteDomains, ServerVersion adminDisplayVersion, out IMExSession mexSession)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("isMemberOfResolver", isMemberOfResolver);
			ArgumentValidator.ThrowIfNull("firstOrgAcceptedDomains", firstOrgAcceptedDomains);
			ArgumentValidator.ThrowIfNull("remoteDomains", remoteDomains);
			ArgumentValidator.ThrowIfNull("adminDisplayVersion", adminDisplayVersion);
			SmtpReceiveServer smtpReceiveServer = SmtpReceiveServer.FromSmtpInSessionState(sessionState, firstOrgAcceptedDomains, remoteDomains, adminDisplayVersion, isMemberOfResolver);
			return new SmtpAgentSession(this.mexRuntime, smtpReceiveServer, sessionState, ref mexSession);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "SmtpReceiveAgents";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			if (this.mexRuntime != null)
			{
				xelement.Add(this.mexRuntime.GetDiagnosticInfo(parameters, "SmtpReceiveAgent"));
			}
			return xelement;
		}

		private readonly MExRuntime mexRuntime = new MExRuntime();
	}
}
