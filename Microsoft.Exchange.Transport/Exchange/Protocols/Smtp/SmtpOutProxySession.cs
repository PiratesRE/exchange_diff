using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpOutProxySession : ISmtpSession, ISmtpOutSession
	{
		public SmtpOutProxySession(ISmtpInSession inSession, SmtpOutConnection smtpOutConnection, ulong sessionId, IPEndPoint target, ProtocolLog protocolLog, ProtocolLoggingLevel loggingLevel, CertificateCache certificateCache, CertificateValidator certificateValidator, ITransportConfiguration transportConfiguration, TransportAppConfig transportAppConfig, string connectionContextString)
		{
			this.InSession = inSession;
			this.smtpOutConnection = smtpOutConnection;
			this.Connector = smtpOutConnection.Connector;
			this.response = new List<string>(50);
			this.sessionProps = new SmtpSessionProps(sessionId);
			this.certificateValidator = certificateValidator;
			this.TlsConfiguration = smtpOutConnection.TlsConfig;
			this.transportConfiguration = transportConfiguration;
			this.transportAppConfig = transportAppConfig;
			this.currentState = SmtpOutSession.SessionState.ConnectResponse;
			this.sessionProps.AdvertisedEhloOptions = new EhloOptions();
			SmtpDomain fqdn = this.Connector.Fqdn;
			this.sessionProps.HelloDomain = ((fqdn != null && !string.IsNullOrEmpty(fqdn.Domain)) ? fqdn.Domain : ComputerInformation.DnsPhysicalFullyQualifiedDomainName);
			this.sessionProps.RemoteEndPoint = target;
			this.advertisedTlsCertificate = SmtpOutSession.LoadTlsCertificate(this.TlsConfiguration, certificateCache, this.transportAppConfig.SmtpSendConfiguration.OneLevelWildcardMatchForCertSelection, this.Connector.Name, this.GetHashCode());
			this.logSession = protocolLog.OpenSession(this.Connector.Name, this.SessionId, target, null, loggingLevel);
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "attempting to connect. " + connectionContextString);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<IPEndPoint>((long)this.GetHashCode(), "Attempting to connect to {0}", target);
			SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_SmtpSendNewProxySession, null, new object[]
			{
				this.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo),
				target
			});
			this.TlsConfiguration.LogTlsOverride(this.logSession);
		}

		public ulong SessionId
		{
			get
			{
				return this.sessionProps.SessionId;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.sessionProps.RemoteEndPoint;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.sessionProps.LocalEndPoint;
			}
		}

		public string ProxyTargetHostName
		{
			get
			{
				return this.smtpOutConnection.SmtpHostName;
			}
		}

		public string HelloDomain
		{
			get
			{
				return this.sessionProps.HelloDomain;
			}
			set
			{
				this.sessionProps.HelloDomain = value;
			}
		}

		public IEhloOptions AdvertisedEhloOptions
		{
			get
			{
				return this.sessionProps.AdvertisedEhloOptions;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
			set
			{
				this.isAuthenticated = value;
			}
		}

		public IProtocolLogSession LogSession
		{
			get
			{
				return this.logSession;
			}
		}

		public SecureState SecureState
		{
			get
			{
				return this.secureState;
			}
		}

		public SmtpOutSession.SessionState NextState
		{
			get
			{
				return this.nextState;
			}
			set
			{
				this.nextState = value;
			}
		}

		public virtual bool Disconnected
		{
			get
			{
				return this.disconnected;
			}
		}

		public long ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public NetworkConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public bool IsProxying
		{
			get
			{
				return this.isProxying;
			}
			set
			{
				this.isProxying = value;
			}
		}

		public bool IsClientProxy
		{
			get
			{
				return this.smtpOutConnection.ClientProxy;
			}
		}

		public SmtpResponse BlindProxySuccessfulInboundResponse
		{
			get
			{
				return this.blindProxySuccessfulInboundResponse;
			}
			set
			{
				this.blindProxySuccessfulInboundResponse = value;
			}
		}

		public AckDetails AckDetails
		{
			get
			{
				return null;
			}
		}

		internal TlsSendConfiguration TlsConfiguration { get; private set; }

		public byte[] GetTlsEapKey()
		{
			return this.connection.TlsEapKey;
		}

		public byte[] GetCertificatePublicKey()
		{
			return this.connection.RemoteCertificate.GetPublicKey();
		}

		public void AckConnection(AckStatus ackStatus, SmtpResponse smtpResponse, SessionSetupFailureReason failureReason)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<AckStatus, SmtpResponse>((long)this.GetHashCode(), "AckConnection called with status: {0}, Response: {1}", ackStatus, smtpResponse);
			this.smtpOutConnection.BytesSent = (ulong)this.connection.BytesSent;
			this.smtpOutConnection.AckConnection(ackStatus, smtpResponse, null, null, failureReason);
		}

		public void Disconnect()
		{
			this.Disconnect(false, false, SmtpResponse.Empty, SessionSetupFailureReason.None, false);
		}

		public void ShutdownConnection()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.ShutdownConnection);
			NetworkConnection networkConnection = this.connection;
			if (networkConnection != null)
			{
				networkConnection.Shutdown();
			}
		}

		public void StartUsingConnection()
		{
			this.logSession.LogConnect();
			SmtpProxyPerfCountersWrapper smtpProxyPerfCounters = this.InSession.SmtpProxyPerfCounters;
			if (smtpProxyPerfCounters != null)
			{
				this.InSession.SmtpProxyPerfCounters.IncrementOutboundConnectionsTotal();
				this.InSession.SmtpProxyPerfCounters.IncrementOutboundConnectionsCurrent();
			}
			this.StartReadLine();
		}

		public void StartTls(SecureState secureState)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Initiating TLS on the outboundConnection");
			this.secureState = (secureState | SecureState.NegotiationRequested);
		}

		public void DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		public NetworkConnection DissociateFromSetupHandler()
		{
			NetworkConnection result = this.connection;
			this.connection = null;
			return result;
		}

		public void SetNextStateToAuthLogin()
		{
			if (!string.IsNullOrEmpty(this.InSession.ProxyUserName) && this.InSession.ProxyPassword != null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "XPROXY command succeeded or was not required, will try to authenticate under AUTH LOGIN");
				this.NextState = SmtpOutSession.SessionState.Auth;
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Unable to AUTH LOGIN because username or password is null");
			throw new InvalidOperationException("Username and/or password cannot be null or empty while setting up proxy session");
		}

		public void FailoverConnection(SmtpResponse smtpResponse, SessionSetupFailureReason failoverReason)
		{
			this.FailoverConnection(smtpResponse, failoverReason, false);
		}

		public void ConnectionCompleted(NetworkConnection networkConnection)
		{
			this.connection = networkConnection;
			this.connectionId = networkConnection.ConnectionId;
			this.connection.MaxLineLength = 2000;
			this.logSession.LocalEndPoint = this.connection.LocalEndPoint;
			this.connection.Timeout = (int)this.smtpOutConnection.Connector.ConnectionInactivityTimeOut.TotalSeconds;
		}

		public string GetConnectionInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LastIndex : ");
			stringBuilder.Append(this.breadcrumbs.LastFilledIndex);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Session BreadCrumb : ");
			for (int i = 0; i < 64; i++)
			{
				stringBuilder.Append(Enum.Format(typeof(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs), this.breadcrumbs.BreadCrumb[i], "x"));
				stringBuilder.Append(" ");
			}
			stringBuilder.AppendLine();
			if (this.connection != null)
			{
				stringBuilder.Append(this.connection.GetBreadCrumbsInfo());
			}
			else
			{
				stringBuilder.AppendLine("connection = null");
			}
			return stringBuilder.ToString();
		}

		public void PrepareForNextMessageOnCachedSession()
		{
			throw new InvalidOperationException("A blind proxy session should never be cached");
		}

		public bool CheckRequireOorg()
		{
			if (!this.Connector.RequireOorg || this.AdvertisedEhloOptions.XOorg)
			{
				return true;
			}
			ExTraceGlobals.SmtpSendTracer.TraceError<IPEndPoint, string, string>((long)this.GetHashCode(), "Connection to remote endpoint '{0} ({1})' for send connector '{2}' will be dropped because the server did not advertise XOORG.", this.connection.RemoteEndPoint, this.smtpOutConnection.SmtpHostName, this.Connector.Name);
			SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SessionFailedBecauseXOorgNotOffered, this.Connector.Name + "-" + this.connection.RemoteEndPoint, new object[]
			{
				this.connection.RemoteEndPoint,
				this.smtpOutConnection.SmtpHostName,
				this.Connector.Name
			});
			string context = string.Format(CultureInfo.InvariantCulture, "Connection to remote endpoint '{0} ({1})' for send connector '{2}' will be dropped because the server did not advertise XOORG.", new object[]
			{
				this.connection.RemoteEndPoint,
				this.smtpOutConnection.SmtpHostName,
				this.Connector.Name
			});
			this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, context);
			this.FailoverConnection(SmtpResponse.RequireXOorgToSendMail, SessionSetupFailureReason.ProtocolError);
			this.NextState = SmtpOutSession.SessionState.Quit;
			return false;
		}

		public bool TryGetRemainingSmtpTargets(out IEnumerable<INextHopServer> remainingTargets)
		{
			return this.smtpOutConnection.TryGetRemainingSmtpTargets(out remainingTargets);
		}

		private static void WriteCompleteReadLine(IAsyncResult asyncResult)
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)asyncResult.AsyncState;
			object obj;
			smtpOutProxySession.connection.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutProxySession.HandleErrorSettingUpProxySession(obj, false);
				return;
			}
			smtpOutProxySession.sendBuffer.Reset();
			smtpOutProxySession.StartReadLine();
		}

		private static void ReadLineComplete(IAsyncResult asyncResult)
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)asyncResult.AsyncState;
			bool overflow = false;
			byte[] buffer;
			int offset;
			int size;
			object obj;
			smtpOutProxySession.connection.EndReadLine(asyncResult, out buffer, out offset, out size, out obj);
			if (obj != null)
			{
				if (!(obj is SocketError) || (SocketError)obj != SocketError.MessageSize)
				{
					smtpOutProxySession.HandleErrorSettingUpProxySession(obj, false);
					return;
				}
				overflow = true;
			}
			if (smtpOutProxySession.disconnected)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)smtpOutProxySession.GetHashCode(), "Command Received from NetworkConnection, but we are already disconnected");
				return;
			}
			smtpOutProxySession.StartProcessingResponse(buffer, offset, size, overflow);
		}

		private static void TlsNegotiationComplete(IAsyncResult asyncResult)
		{
			SmtpOutProxySession smtpOutProxySession = (SmtpOutProxySession)asyncResult.AsyncState;
			object obj;
			smtpOutProxySession.connection.EndNegotiateTlsAsClient(asyncResult, out obj);
			if (obj != null)
			{
				smtpOutProxySession.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "TLS negotiation failed with error {0}", new object[]
				{
					obj
				});
				bool retryWithoutStartTls = smtpOutProxySession.currentState == SmtpOutSession.SessionState.StartTLS && !smtpOutProxySession.TlsConfiguration.RequireTls;
				smtpOutProxySession.HandleErrorSettingUpProxySession(obj, retryWithoutStartTls);
				return;
			}
			ConnectionInfo tlsConnectionInfo = smtpOutProxySession.connection.TlsConnectionInfo;
			Util.LogTlsSuccessResult(smtpOutProxySession.logSession, tlsConnectionInfo, smtpOutProxySession.connection.RemoteCertificate);
			smtpOutProxySession.TlsNegotiationComplete();
		}

		private static string ConvertStringListToString(List<string> stringList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in stringList)
			{
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		private void FailoverConnection(SmtpResponse smtpResponse, SessionSetupFailureReason failoverReason, bool retryWithoutStartTls)
		{
			this.failoverInProgress = true;
			this.smtpOutConnection.FailoverConnection(smtpResponse, retryWithoutStartTls, failoverReason);
		}

		private void Disconnect(bool disposeCurrentCommand, bool failover, SmtpResponse failoverResponse, SessionSetupFailureReason failoverReason, bool retryWithoutStartTls)
		{
			if (failover && (failoverResponse.IsEmpty || failoverReason == SessionSetupFailureReason.None))
			{
				throw new ArgumentException("failoverResponse and failoverReason need to be specified when failing over");
			}
			ExTraceGlobals.SmtpSendTracer.TraceError<long, string>((long)this.GetHashCode(), "Disconnect initiated for connection {0} due to {1}", this.connectionId, failoverResponse.ToString());
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.Disconnect);
			if (disposeCurrentCommand)
			{
				this.DisposeCurrentCommand();
			}
			if (!this.disconnected)
			{
				this.disconnected = true;
				if (this.connection != null)
				{
					SmtpProxyPerfCountersWrapper smtpProxyPerfCounters = this.InSession.SmtpProxyPerfCounters;
					if (smtpProxyPerfCounters != null)
					{
						this.InSession.SmtpProxyPerfCounters.DecrementOutboundConnectionsCurrent();
					}
					this.connection.Dispose();
					this.LogSession.LogDisconnect(DisconnectReason.Local);
				}
				if (failover && !this.failoverInProgress)
				{
					this.FailoverConnection(failoverResponse, failoverReason, retryWithoutStartTls);
				}
				if (!this.failoverInProgress)
				{
					this.smtpOutConnection.RemoveConnection();
				}
				this.logSession = null;
			}
		}

		private void DisposeCurrentCommand()
		{
			if (this.currentCommand != null)
			{
				this.currentCommand.Dispose();
				this.currentCommand = null;
			}
		}

		private void HandleErrorSettingUpProxySession(object error, bool retryWithoutStartTls)
		{
			SessionSetupFailureReason failoverReason;
			string text;
			if (error is SocketError)
			{
				failoverReason = SessionSetupFailureReason.SocketError;
				text = "Socket error " + error.ToString();
			}
			else if (error is SecurityStatus)
			{
				failoverReason = SessionSetupFailureReason.ProtocolError;
				text = "Security status " + error.ToString();
			}
			else
			{
				failoverReason = SessionSetupFailureReason.ProtocolError;
				text = "Error " + ((error != null) ? error.ToString() : "<null>");
			}
			ExTraceGlobals.SmtpSendTracer.TraceError<long, string>((long)this.GetHashCode(), "SmtpOutProxySession(id={0}).HandleErrorSettingUpProxySession ({1})", this.connectionId, text);
			SmtpResponse failoverResponse = new SmtpResponse("451", "4.4.0", new string[]
			{
				text
			});
			bool failover = this.currentState != SmtpOutSession.SessionState.Quit;
			this.Disconnect(true, failover, failoverResponse, failoverReason, retryWithoutStartTls);
		}

		private void AppendToResponseBuffer(string responseline)
		{
			if (this.response.Count >= 50)
			{
				throw new FormatException("Excessive data, unable to parse : " + SmtpOutProxySession.ConvertStringListToString(this.response));
			}
			int length = responseline.Length;
			if (length > 0 && responseline[length - 1] == '\r')
			{
				responseline = responseline.Substring(0, length - 1);
			}
			this.response.Add(responseline);
		}

		private void MoveToNextState()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.MoveToNextState);
			this.DisposeCurrentCommand();
			if (!this.Disconnected)
			{
				this.currentState = this.NextState;
				this.currentCommand = this.CreateSmtpCommand(this.currentState);
				while (!this.SendCurrentCommand())
				{
					this.DisposeCurrentCommand();
					this.currentState = this.NextState;
					this.currentCommand = this.CreateSmtpCommand(this.currentState);
				}
			}
		}

		private SmtpCommand CreateSmtpCommand(SmtpOutSession.SessionState state)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpOutSession.SessionState>((long)this.GetHashCode(), "Creating Smtp Command: {0}", state);
			SmtpCommand smtpCommand;
			switch (state)
			{
			case SmtpOutSession.SessionState.Ehlo:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdEhlo);
				smtpCommand = new EHLOSmtpProxyCommand(this, this.transportConfiguration);
				break;
			case SmtpOutSession.SessionState.Helo:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdHelo);
				smtpCommand = new HELOSmtpProxyCommand(this);
				break;
			case SmtpOutSession.SessionState.Auth:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdAuth);
				smtpCommand = new AuthSmtpProxyCommand(this, this.transportConfiguration, false);
				break;
			case SmtpOutSession.SessionState.Exps:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdAuth);
				smtpCommand = new AuthSmtpProxyCommand(this, this.transportConfiguration, true);
				break;
			case SmtpOutSession.SessionState.StartTLS:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdStarttls);
				smtpCommand = new StarttlsSmtpProxyCommand(this, this.transportConfiguration, false);
				break;
			case SmtpOutSession.SessionState.AnonymousTLS:
				this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdStarttls);
				smtpCommand = new StarttlsSmtpProxyCommand(this, this.transportConfiguration, true);
				break;
			default:
				if (state != SmtpOutSession.SessionState.XProxy)
				{
					if (state != SmtpOutSession.SessionState.Quit)
					{
						throw new ArgumentException("Unknown command encountered in SmtpOutProxy: " + state, "state");
					}
					this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdQuit);
					smtpCommand = new QuitSmtpProxyCommand(this);
				}
				else
				{
					this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.CreateCmdXProxy);
					smtpCommand = new XProxySmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
				}
				break;
			}
			smtpCommand.ParsingStatus = ParsingStatus.Complete;
			smtpCommand.OutboundCreateCommand();
			return smtpCommand;
		}

		private bool SendCurrentCommand()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.SendCurrentCommand);
			if (this.currentState == SmtpOutSession.SessionState.StartTLS && (byte)(this.secureState & SecureState.NegotiationRequested) == 128)
			{
				throw new InvalidOperationException("Should not attempt to send command when TLS negotiation is on");
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpOutSession.SessionState>((long)this.GetHashCode(), "Invoking Command Handler for {0}", this.currentState);
			if (!this.InvokeCommandHandler())
			{
				return false;
			}
			this.SendBufferThenReadLine();
			return true;
		}

		private void SendBufferThenReadLine()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.SendBufferThenReadLine);
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Flushing SendBuffer");
			this.connection.BeginWrite(this.sendBuffer.GetBuffer(), 0, this.sendBuffer.Length, SmtpOutProxySession.writeCompleteReadLine, this);
		}

		private bool InvokeCommandHandler()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.InvokeCommandHandler);
			this.currentCommand.OutboundFormatCommand();
			if (this.currentCommand.ProtocolCommandString != null)
			{
				this.currentCommand.ProtocolCommand = ByteString.StringToBytesAndAppendCRLF(this.currentCommand.ProtocolCommandString, true);
				if (string.IsNullOrEmpty(this.currentCommand.RedactedProtocolCommandString))
				{
					this.logSession.LogSend(this.currentCommand.ProtocolCommand);
				}
				else
				{
					this.logSession.LogSend(ByteString.StringToBytes(this.currentCommand.RedactedProtocolCommandString, true));
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Enqueuing Command: {0} on the connection", this.currentCommand.ProtocolCommandString);
				this.sendBuffer.Append(this.currentCommand.ProtocolCommand);
				return true;
			}
			if (this.currentCommand.ProtocolCommand != null)
			{
				this.logSession.LogSend(SmtpOutSession.BinaryData);
				this.sendBuffer.Append(this.currentCommand.ProtocolCommand);
				return true;
			}
			return false;
		}

		private void ProcessResponse()
		{
			this.InvokeResponseHandler();
		}

		private void InvokeResponseHandler()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.InvokeResponseHandler);
			SmtpResponse smtpResponse;
			if (!SmtpResponse.TryParse(this.response, out smtpResponse))
			{
				throw new FormatException("Response text was incorrectly formed : " + SmtpOutProxySession.ConvertStringListToString(this.response));
			}
			this.response.Clear();
			if (this.currentCommand == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "Invoked Response Handler for ConnectResponse");
				this.ConnectResponseEvent(smtpResponse);
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Invoked Response Handler for {0}", this.currentCommand.ProtocolCommandKeyword);
			this.currentCommand.SmtpResponse = smtpResponse;
			this.HandlePostParseResponse();
		}

		private void HandlePostParseResponse()
		{
			if (string.Equals(this.currentCommand.SmtpResponse.StatusCode, "421", StringComparison.Ordinal) && !(this.currentCommand is QuitSmtpCommand))
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Attempting failover. 421 Status code on {0}. NextState: Quit", this.currentCommand.ProtocolCommandKeyword);
				this.FailoverConnection(this.currentCommand.SmtpResponse, SessionSetupFailureReason.Shutdown);
				this.NextState = SmtpOutSession.SessionState.Quit;
			}
			else
			{
				this.currentCommand.OutboundProcessResponse();
				if ((byte)(this.secureState & SecureState.NegotiationRequested) == 128)
				{
					if (!(this.currentCommand is StarttlsSmtpCommand))
					{
						throw new InvalidOperationException("Command being processed is not StartTls");
					}
					X509Certificate2 x509Certificate = null;
					if ((byte)(this.secureState & ~SecureState.NegotiationRequested) == 1)
					{
						x509Certificate = this.advertisedTlsCertificate;
					}
					this.logSession.LogCertificate("Sending certificate", x509Certificate);
					this.connection.BeginNegotiateTlsAsClient(x509Certificate, this.connection.RemoteEndPoint.Address.ToString(), SmtpOutProxySession.tlsNegotiationComplete, this);
					return;
				}
				else if (this.currentCommand.ParsingStatus == ParsingStatus.MoreDataRequired)
				{
					this.currentCommand.ProtocolCommand = null;
					this.currentCommand.ProtocolCommandString = null;
					this.currentCommand.ParsingStatus = ParsingStatus.Complete;
					this.InvokeCommandHandler();
					this.SendBufferThenReadLine();
					return;
				}
			}
			if (!this.IsProxying)
			{
				this.MoveToNextState();
				return;
			}
			if (this.IsClientProxy && !this.IsAuthenticated)
			{
				throw new InvalidOperationException("Cannot proxy client session without authenticating");
			}
			this.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy session successfully set up for {0}. Inbound session will now be blindly proxied", new object[]
			{
				this.IsClientProxy ? Util.Redact(this.InSession.ProxyUserName) : "outbound proxy"
			});
			SmtpResponse successfulResponse = this.BlindProxySuccessfulInboundResponse;
			this.DisposeCurrentCommand();
			this.smtpOutConnection.RemoveConnection();
			this.InSession.SmtpProxyPerfCounters.UpdateOnProxySuccess();
			this.InSession.HandleBlindProxySetupSuccess(successfulResponse, this.connection, this.SessionId, this.logSession, this.IsClientProxy);
			this.logSession = null;
		}

		private void ConnectResponseEvent(SmtpResponse smtpResponse)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<IPEndPoint>((long)this.GetHashCode(), "Connected to remote server: {0}", this.sessionProps.RemoteEndPoint);
			if (!this.disconnected)
			{
				if (smtpResponse.StatusCode[0] != '2')
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "Server is not accepting mail, connect response: {0}", smtpResponse);
					this.FailoverConnection(smtpResponse, SessionSetupFailureReason.ProtocolError);
					this.NextState = SmtpOutSession.SessionState.Quit;
					this.response.Clear();
					this.MoveToNextState();
					return;
				}
				if (this.Connector.ForceHELO)
				{
					if (this.IsClientProxy)
					{
						throw new InvalidOperationException("Client proxy connector should not have ForceHelo set");
					}
					this.NextState = SmtpOutSession.SessionState.Helo;
				}
				else
				{
					this.NextState = SmtpOutSession.SessionState.Ehlo;
				}
				this.response.Clear();
				this.MoveToNextState();
			}
		}

		private void StartProcessingResponse(byte[] buffer, int offset, int size, bool overflow)
		{
			BufferBuilder bufferBuilder = this.responseBuffer ?? new BufferBuilder(size);
			try
			{
				if (bufferBuilder.Length + size > 32768)
				{
					this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "line too long");
					string message = string.Format("Illegal response, length exceeds the maximum that can be handled by SmtpOut. Max = {0} chars", 32768);
					throw new FormatException(message);
				}
				bufferBuilder.Append(buffer, offset, size);
				if (overflow)
				{
					this.responseBuffer = bufferBuilder;
					this.StartReadLine();
				}
				else
				{
					this.responseBuffer = null;
					bufferBuilder.RemoveUnusedBufferSpace();
					if (!(this.currentCommand is AuthSmtpCommand))
					{
						this.logSession.LogReceive(bufferBuilder.GetBuffer());
					}
					string text = bufferBuilder.ToString();
					this.AppendToResponseBuffer(text);
					if (text.Length < 3)
					{
						throw new FormatException("Illegal response: " + text);
					}
					if (text.Length > 3 && text[3] == '-')
					{
						this.StartReadLine();
					}
					else
					{
						this.ProcessResponse();
					}
				}
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "The connection was dropped because a response was illegally formatted. The error is: {0}", ex.Message);
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "The connection was dropped because a response was illegally formatted. The error is: {0}", new object[]
				{
					ex.Message
				});
				this.HandleErrorSettingUpProxySession("Response was not in the expected format", false);
			}
		}

		private void StartReadLine()
		{
			this.connection.BeginReadLine(SmtpOutProxySession.readLineComplete, this);
		}

		private void TlsNegotiationComplete()
		{
			this.DropBreadcrumb(SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs.TlsNegotiationComplete);
			ExTraceGlobals.SmtpSendTracer.TraceDebug<long>((long)this.GetHashCode(), "TLS negotiation completed for connection {0}. Reissue Ehlo", this.connectionId);
			if (this.TlsConfiguration.RequireTls && this.connection.RemoteCertificate == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "No remote certificate present");
				this.HandleErrorSettingUpProxySession(SecurityStatus.IncompleteCredentials, false);
				return;
			}
			this.logSession.LogCertificateThumbprint("Received certificate", this.connection.RemoteCertificate);
			this.secureState &= ~SecureState.NegotiationRequested;
			if (this.TlsConfiguration.RequireTls && this.connection.TlsCipherKeySize < 128)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<int>((long)this.GetHashCode(), "Quit proxy session because Tls cipher strength is too weak at {0}", this.connection.TlsCipherKeySize);
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Tls cipher strength is too weak");
				this.FailoverConnection(SmtpResponse.AuthTempFailureTLSCipherTooWeak, SessionSetupFailureReason.ProtocolError);
				this.NextState = SmtpOutSession.SessionState.Quit;
				this.MoveToNextState();
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Remote has supplied certificate {0}", this.connection.RemoteCertificate.Subject);
			ChainValidityStatus chainValidityStatus = ChainValidityStatus.Valid;
			if (this.TlsConfiguration.RequireTls && (this.TlsConfiguration.TlsAuthLevel == RequiredTlsAuthLevel.CertificateValidation || this.TlsConfiguration.TlsAuthLevel == RequiredTlsAuthLevel.DomainValidation))
			{
				RequiredTlsAuthLevel valueOrDefault = this.TlsConfiguration.TlsAuthLevel.GetValueOrDefault();
				RequiredTlsAuthLevel? requiredTlsAuthLevel;
				if (requiredTlsAuthLevel != null)
				{
					switch (valueOrDefault)
					{
					case RequiredTlsAuthLevel.CertificateValidation:
						chainValidityStatus = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate, this.transportAppConfig.SmtpSendConfiguration.CacheOnlyUrlRetrievalForRemoteCertChain);
						break;
					case RequiredTlsAuthLevel.DomainValidation:
						if (!SmtpOutSession.MatchCertificateWithTlsDomain(this.TlsConfiguration.TlsDomains, this.connection.RemoteCertificate, this.logSession, this.certificateValidator))
						{
							chainValidityStatus = ChainValidityStatus.SubjectMismatch;
						}
						else
						{
							chainValidityStatus = this.certificateValidator.ChainValidateAsAnonymous(this.connection.RemoteCertificate, this.transportAppConfig.SmtpSendConfiguration.CacheOnlyUrlRetrievalForRemoteCertChain);
						}
						break;
					}
				}
			}
			if (chainValidityStatus != ChainValidityStatus.Valid)
			{
				string arg = chainValidityStatus.ToString();
				ExTraceGlobals.SmtpSendTracer.TraceError<string, string>((long)this.GetHashCode(), "Outbound TLS authentication failed with error {0} for proxy session due to certificate validation error.Target is {1}.", arg, this.connection.RemoteEndPoint.Address.ToString());
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, string.Format("outbound TLS authentication failed for the proxy session due to certificate validation error. ChainValidityStatus = {0}.", arg));
				if (!this.certificateValidator.ShouldTreatValidationResultAsSuccess(chainValidityStatus))
				{
					this.FailoverConnection(SmtpResponse.CertificateValidationFailure, SessionSetupFailureReason.ProtocolError);
					this.NextState = SmtpOutSession.SessionState.Quit;
					this.MoveToNextState();
					return;
				}
				SmtpOutConnection.Events.LogEvent(TransportEventLogConstants.Tuple_CertificateRevocationListCheckTrasientFailureTreatedAsSuccess, this.connection.RemoteCertificate.SerialNumber, new object[]
				{
					chainValidityStatus.ToString(),
					this.connection.RemoteCertificate.SerialNumber,
					this.connection.RemoteCertificate.Subject,
					this.connection.RemoteCertificate.Issuer,
					this.connection.RemoteCertificate.Thumbprint,
					"SmtpOutProxy"
				});
				this.logSession.LogCertificate(string.Format(CultureInfo.InvariantCulture, "Treating CRL chain validation failure {0} as success.", new object[]
				{
					chainValidityStatus
				}), this.connection.RemoteCertificate);
			}
			this.logSession.LogCertificate("Proxy target certificate", this.connection.RemoteCertificate);
			this.NextState = SmtpOutSession.SessionState.Ehlo;
			this.MoveToNextState();
		}

		private const int MaxResponseLength = 32768;

		private const int NumberOfBreadcrumbs = 64;

		public readonly ISmtpInSession InSession;

		public readonly SmtpSendConnectorConfig Connector;

		private static readonly SmtpResponse GenericProxyFailedResponse = new SmtpResponse("401", "4.5.4", new string[]
		{
			"Proxy failed"
		});

		private static readonly AsyncCallback writeCompleteReadLine = new AsyncCallback(SmtpOutProxySession.WriteCompleteReadLine);

		private static readonly AsyncCallback tlsNegotiationComplete = new AsyncCallback(SmtpOutProxySession.TlsNegotiationComplete);

		private static readonly AsyncCallback readLineComplete = new AsyncCallback(SmtpOutProxySession.ReadLineComplete);

		private readonly SmtpOutConnection smtpOutConnection;

		private NetworkConnection connection;

		private SmtpCommand currentCommand;

		private List<string> response;

		private SecureState secureState;

		private X509Certificate2 advertisedTlsCertificate;

		private bool isAuthenticated;

		private IProtocolLogSession logSession;

		private bool disconnected;

		private SmtpOutSession.SessionState currentState;

		private SmtpOutSession.SessionState nextState;

		private SmtpSessionProps sessionProps;

		private long connectionId;

		private BufferBuilder responseBuffer;

		private BufferBuilder sendBuffer = new BufferBuilder();

		private Breadcrumbs<SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs> breadcrumbs = new Breadcrumbs<SmtpOutProxySession.SmtpOutProxySessionBreadcrumbs>(64);

		private ITransportConfiguration transportConfiguration;

		private TransportAppConfig transportAppConfig;

		private CertificateValidator certificateValidator;

		private bool isProxying;

		private bool failoverInProgress;

		private SmtpResponse blindProxySuccessfulInboundResponse = SmtpOutProxySession.GenericProxyFailedResponse;

		public enum SmtpOutProxySessionBreadcrumbs
		{
			EMPTY,
			CreateCmdConnectResponse,
			CreateCmdEhlo,
			CreateCmdAuth,
			CreateCmdStarttls,
			CreateCmdXProxy,
			Disconnect,
			CreateCmdQuit,
			EnqueueResponseHandler,
			MoveToNextState,
			SendCurrentCommand,
			SendBufferThenReadLine,
			InvokeCommandHandler,
			InvokeResponseHandler,
			TlsNegotiationComplete,
			ShutdownConnection,
			CreateCmdHelo
		}
	}
}
