using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProtocolSession : BaseSession, IReadOnlyPropertyBag
	{
		public ProtocolSession(NetworkConnection connection, VirtualServer virtualServer) : base(connection, virtualServer.Server.PreAuthConnectionTimeout, virtualServer.Server.MaxCommandLength)
		{
			this.virtualServer = virtualServer;
			if (this.Server.LightLog != null)
			{
				this.lightLogSession = this.Server.LightLog.OpenSession((ulong)connection.ConnectionId, connection.RemoteEndPoint, connection.LocalEndPoint, ProtocolLoggingLevel.Verbose);
			}
			this.mailboxLogTimeout = ExDateTime.MinValue;
		}

		public WorkloadSettings WorkloadSettings { get; protected set; }

		public VirtualServer VirtualServer
		{
			get
			{
				return this.virtualServer;
			}
		}

		public ResponseFactory ResponseFactory
		{
			get
			{
				return this.responseFactory;
			}
			set
			{
				this.responseFactory = value;
			}
		}

		public ProtocolBaseServices Server
		{
			get
			{
				return this.virtualServer.Server;
			}
		}

		public ProxySession ProxySession
		{
			get
			{
				return this.proxySession;
			}
			set
			{
				this.proxySession = value;
			}
		}

		public bool IsTls
		{
			get
			{
				return this.isTls;
			}
		}

		public bool OkToIssueRead
		{
			get
			{
				return this.okToIssueRead;
			}
			set
			{
				this.okToIssueRead = value;
			}
		}

		public bool ProxyToLegacyServer { get; set; }

		public IStandardBudget Budget { get; set; }

		public ActivityScope ActivityScope { get; set; }

		public LightWeightLogSession LightLogSession
		{
			get
			{
				return this.lightLogSession;
			}
		}

		public LrsSession LrsSession { get; set; }

		protected internal IPAddress ProxyRemoteAddress { get; set; }

		protected internal bool MailboxLogEnabled
		{
			get
			{
				return this.mailboxLogTimeout > ExDateTime.UtcNow;
			}
		}

		public void StartSession(bool startSSL)
		{
			base.EnterCommandProcessing();
			try
			{
				if (this.LightLogSession != null)
				{
					this.LightLogSession.BeginCommand(ProtocolSession.OpenBuf);
				}
				this.nonAuthSessionDisconnectTime = ExDateTime.UtcNow.AddSeconds((double)this.Server.PreAuthConnectionTimeout);
				if (startSSL)
				{
					this.StartSsl();
				}
				else if (this.AddToUnauthenticatedConnectionsPerIp())
				{
					if (!base.SendToClient(new StringResponseItem(this.BannerString())))
					{
						return;
					}
					this.hasShownBanner = true;
					base.SendToClient(new EndResponseItem(new BaseSession.SendCompleteDelegate(this.EndCommandProcess)));
				}
				ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, "ProtocolSession has started.");
			}
			finally
			{
				base.LeaveCommandProcessing();
			}
		}

		public bool StartSsl()
		{
			bool result;
			try
			{
				base.NegotiatingTls = true;
				base.Connection.BeginNegotiateTlsAsServer(this.virtualServer.Certificate, new AsyncCallback(this.TlsNegotiationCompleteCallback), base.Connection);
				result = true;
			}
			catch (Win32Exception ex)
			{
				base.NegotiatingTls = false;
				if (ex.NativeErrorCode != -2146885628)
				{
					throw;
				}
				ProtocolBaseServices.SessionTracer.TraceError(base.SessionId, "Unable to find certificate.");
				ProtocolBaseServices.LogEvent(this.Server.SslCertificateNotFoundEventTuple, null, new string[0]);
				result = false;
			}
			return result;
		}

		public void BeginShutdown()
		{
			this.BeginShutdown(null);
		}

		public void BeginShutdown(string message)
		{
			this.BeginShutdown(message, null);
		}

		public void BeginShutdown(string message, BaseSession.ConnectionShutdownDelegate connectionShutdown)
		{
			base.ConnectionShutdown = connectionShutdown;
			if (!string.IsNullOrEmpty(message))
			{
				base.SendToClient(new StringResponseItem(message, new BaseSession.SendCompleteDelegate(this.EndShutdown)));
				return;
			}
			base.SendToClient(new BufferResponseItem(ProtocolSession.EmptyBuffer, 0, 0, new BaseSession.SendCompleteDelegate(this.EndShutdown)));
		}

		public abstract string BannerString();

		public override bool IsUserTraceEnabled()
		{
			ResponseFactory responseFactory = this.responseFactory;
			if (responseFactory != null)
			{
				ProtocolUser protocolUser = responseFactory.ProtocolUser;
				if (protocolUser != null)
				{
					string uniqueName = protocolUser.UniqueName;
					if (!string.IsNullOrEmpty(uniqueName))
					{
						return ExUserTracingAdaptor.Instance.IsTracingEnabledUser(uniqueName);
					}
				}
			}
			return false;
		}

		public override string GetUserNameForLogging()
		{
			ResponseFactory responseFactory = this.ResponseFactory;
			if (responseFactory != null)
			{
				string userName = responseFactory.UserName;
				if (!string.IsNullOrEmpty(userName))
				{
					return userName;
				}
			}
			return "(Unauthorized user)";
		}

		public override void EnterReadLoop(NetworkConnection networkConnection)
		{
			lock (this.LockObject)
			{
				if (this.responseFactory != null && !this.responseFactory.IsAuthenticated && !this.responseFactory.IsDisconnected)
				{
					int num = (int)(this.nonAuthSessionDisconnectTime - ExDateTime.UtcNow).TotalSeconds;
					if (num <= 0)
					{
						ProtocolBaseServices.SessionTracer.Information(base.SessionId, "ProtocolSession.EnterReadLoop. Preauthenticated timeout");
						if (this.LightLogSession != null)
						{
							this.LightLogSession.ErrorMessage = "PreAuthTimeout";
						}
						this.BeginShutdown();
						return;
					}
					base.Connection.Timeout = num;
				}
			}
			base.EnterReadLoop(networkConnection);
		}

		public override string ToString()
		{
			if (this.ProxyRemoteAddress != null)
			{
				return string.Format("{0} proxy from {1}", base.ToString(), this.ProxyRemoteAddress);
			}
			return base.ToString();
		}

		public override bool CheckNonCriticalException(Exception exception)
		{
			if (this.lightLogSession != null)
			{
				this.lightLogSession.ExceptionCaught = exception;
			}
			return base.CheckNonCriticalException(exception);
		}

		internal void SetMailboxLogTimeout(ExDateTime timeout)
		{
			this.mailboxLogTimeout = timeout;
			if (this.VerifyMailboxLogEnabled())
			{
				if (this.mailboxLogger != null)
				{
					throw new InvalidOperationException("this.mailboxLogger is not null.");
				}
				this.mailboxLogger = new MailboxLogger(this.ResponseFactory.Store, ProtocolBaseServices.ServiceName);
				if (this.mailboxLogger.LastError != null)
				{
					ProtocolBaseServices.LogEvent(this.Server.CreateMailboxLoggerFailedEventTuple, this.ResponseFactory.UserName, new string[]
					{
						this.ResponseFactory.UserName,
						this.mailboxLogger.LastError.ToString()
					});
				}
				this.ClearOldMailboxLogs();
				this.mailboxLogger.WriteLog(Encoding.ASCII.GetBytes(string.Format("<session id = \"{0}\" user = \"{1}\" from = \"{2}\" to = \"{3}\" time = \"{4}\" secure = \"{5}\">", new object[]
				{
					base.SessionId,
					this.ResponseFactory.UserName,
					base.RemoteEndPoint,
					base.LocalEndPoint,
					ExDateTime.Now,
					this.IsTls
				})));
			}
		}

		internal void LogReceive(byte[] buf, int offset, int size)
		{
			if (this.LightLogSession != null)
			{
				this.LightLogSession.RequestSize += (long)size;
			}
			bool flag = ProtocolBaseServices.SessionTracer.IsTraceEnabled(TraceType.InfoTrace);
			bool flag2 = this.VerifyMailboxLogEnabled();
			if (!flag && !flag2)
			{
				return;
			}
			if (flag)
			{
				string @string = Encoding.ASCII.GetString(buf, offset, size);
				ProtocolBaseServices.SessionTracer.Information<string>(base.SessionId, ">>> CommandReceived: {0}", @string);
			}
			if (flag2)
			{
				this.mailboxLogger.AppendLog(string.Format("<receive time = \"{0}\">", ExDateTime.Now));
				this.mailboxLogger.AppendLog(buf, offset, size);
				this.mailboxLogger.AppendLog("</receive>\r\n");
			}
		}

		internal void LogSend(byte[] buf, int offset, int size)
		{
			if (this.LightLogSession != null)
			{
				this.LightLogSession.ResponseSize += (long)size;
			}
			bool flag = ProtocolBaseServices.SessionTracer.IsTraceEnabled(TraceType.InfoTrace);
			bool flag2 = this.VerifyMailboxLogEnabled();
			if (!flag && !flag2)
			{
				return;
			}
			if (flag)
			{
				string arg = Encoding.ASCII.GetString(buf, offset, size).Trim();
				ProtocolBaseServices.SessionTracer.Information<string>(base.SessionId, "<<< Response sent: {0}", arg);
			}
			if (flag2)
			{
				this.mailboxLogger.AppendLog("<d>");
				this.mailboxLogger.AppendLog(buf, offset, size);
				this.mailboxLogger.AppendLog("</d>");
			}
		}

		internal void LogSend(string format, int size)
		{
			if (this.LightLogSession != null)
			{
				this.LightLogSession.ResponseSize += (long)size;
			}
			bool flag = ProtocolBaseServices.SessionTracer.IsTraceEnabled(TraceType.InfoTrace);
			bool flag2 = this.VerifyMailboxLogEnabled();
			if (!flag && !flag2)
			{
				return;
			}
			string text = string.Format(format, size);
			if (flag)
			{
				ProtocolBaseServices.SessionTracer.Information<string>(base.SessionId, "<<< Response sent: {0}", text.Trim());
			}
			if (flag2)
			{
				this.mailboxLogger.AppendLog("<d>");
				this.mailboxLogger.AppendLog(text);
				this.mailboxLogger.AppendLog("</d>");
			}
		}

		internal void LogInformation(string template, params object[] args)
		{
			ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, template, args);
			if (this.VerifyMailboxLogEnabled())
			{
				this.mailboxLogger.AppendLog(string.Format("<information time = \"{0}\">", ExDateTime.Now));
				this.mailboxLogger.AppendLog(string.Format(template, args));
				this.mailboxLogger.AppendLog("</information>\r\n");
			}
		}

		internal object GetThrottlingPolicyValue(Func<IThrottlingPolicy, object> func)
		{
			if (this.Budget == null)
			{
				return null;
			}
			return func(this.Budget.ThrottlingPolicy);
		}

		internal void EndShutdown()
		{
			lock (this.LockObject)
			{
				if (base.ProcessingCommandRefCounter > 0)
				{
					base.Disconnected = true;
				}
				else
				{
					base.Dispose();
				}
			}
		}

		internal void EnforceMicroDelayAndDisposeCostHandles(IStandardBudget perCallBudget)
		{
			try
			{
				if (perCallBudget != null)
				{
					bool flag = false;
					ResourceKey[] resourcesToAccess = null;
					lock (this.LockObject)
					{
						if (this.ResponseFactory != null)
						{
							flag = true;
							resourcesToAccess = this.ResponseFactory.ResourceKeys;
						}
					}
					try
					{
						if (flag)
						{
							ResourceLoadDelayInfo.EnforceDelay(perCallBudget, this.WorkloadSettings, resourcesToAccess, TimeSpan.MaxValue, null);
						}
					}
					finally
					{
						if (perCallBudget != null)
						{
							perCallBudget.Dispose();
						}
					}
				}
			}
			finally
			{
				ActivityContext.ClearThreadScope();
			}
		}

		protected internal bool AddToUnauthenticatedConnectionsPerIp()
		{
			if (ProtocolSession.unauthenticatedConnectionsPerIp.Add(base.RemoteEndPoint.Address) > this.Server.MaxConcurrentConnectionsFromSingleIp)
			{
				this.LogInformation("Number of unauthenticated connections from single IP {0} exceeded {1}, session disconnected.", new object[]
				{
					base.RemoteEndPoint.Address,
					this.Server.MaxConcurrentConnectionsFromSingleIp
				});
				ProtocolBaseServices.LogEvent(this.Server.MaxConnectionsFromSingleIpExceededEventTuple, base.RemoteEndPoint.Address.ToString(), new string[]
				{
					base.RemoteEndPoint.Address.ToString(),
					this.Server.MaxConcurrentConnectionsFromSingleIp.ToString()
				});
				this.virtualServer.Connections_Rejected.Increment();
				this.BeginShutdown(this.Server.MaxConnectionsError);
				return false;
			}
			this.VirtualServer.UnAuth_Connections.Increment();
			return true;
		}

		protected internal void RemoveFromUnauthenticatedConnectionsPerIp()
		{
			lock (this.LockObject)
			{
				if (!this.removedFromNonAuth)
				{
					ProtocolSession.unauthenticatedConnectionsPerIp.Remove(base.RemoteEndPoint.Address);
					this.VirtualServer.UnAuth_Connections.Decrement();
					this.removedFromNonAuth = true;
				}
			}
		}

		protected internal bool VerifyMailboxLogEnabled()
		{
			bool mailboxLogEnabled = this.MailboxLogEnabled;
			if (!mailboxLogEnabled && this.mailboxLogger != null)
			{
				this.mailboxLogger.AppendLog(string.Format("<stopped time = \"{0}\" /></session>\r\n", ExDateTime.Now));
				this.mailboxLogger.Flush();
				this.mailboxLogger.Dispose();
				this.mailboxLogger = null;
			}
			return mailboxLogEnabled;
		}

		protected override void ReadLineCompletePostProcessing()
		{
			lock (this.LockObject)
			{
				if (this.responseFactory != null && !this.responseFactory.IsAuthenticated && !this.responseFactory.IsDisconnected)
				{
					int num = (int)(this.nonAuthSessionDisconnectTime - ExDateTime.UtcNow).TotalSeconds;
					if (num <= 0)
					{
						ProtocolBaseServices.SessionTracer.Information(base.SessionId, "ProtocolSession.ReadLineCompletePostProcessing. Preauthenticated timeout");
						if (this.LightLogSession != null)
						{
							this.LightLogSession.ErrorMessage = "PreAuthTimeout";
						}
						this.BeginShutdown();
					}
					else
					{
						base.Connection.Timeout = num;
					}
				}
			}
		}

		protected void EndCommandProcess()
		{
			this.ResponseFactory.RecordCommandEnd();
			string commandName = string.Empty;
			if (this.LightLogSession != null && !this.ResponseFactory.IsInAuthenticationMode)
			{
				this.SetDiagnosticValue(PopImapConditionalHandlerSchema.LightLogContext, string.Concat(new object[]
				{
					this.LightLogSession.Result,
					this.LightLogSession.RowsProcessed,
					this.LightLogSession.Recent,
					this.LightLogSession.TotalSize,
					this.LightLogSession.SearchType,
					this.LightLogSession.ClientIp,
					this.LightLogSession.Message,
					this.LightLogSession.ErrorMessage,
					this.LightLogSession.LiveIdAuthResult,
					this.LightLogSession.FolderCount,
					this.LightLogSession.ItemsDeleted,
					(this.LightLogSession.ExceptionCaught == null) ? "<no exception>" : this.LightLogSession.ExceptionCaught.ToString(),
					this.LightLogSession.CafeActivityId,
					(this.LightLogSession.Budget == null) ? "<null budget>" : this.LightLogSession.Budget.ToString()
				}));
				if (this.LightLogSession.Command != null)
				{
					commandName = Encoding.Default.GetString(this.LightLogSession.Command);
				}
				this.LightLogSession.CompleteCommand();
			}
			if (this.MailboxLogEnabled && this.mailboxLogger != null)
			{
				this.mailboxLogger.AppendLog(string.Format("<commandFinished time = \"{0}\" />\r\n", ExDateTime.Now));
				this.mailboxLogger.Flush();
			}
			string text = this[PopImapConditionalHandlerSchema.RequestId] as string;
			Guid key;
			if (!string.IsNullOrEmpty(text) && Guid.TryParse(text, out key))
			{
				PopImapRequestData popImapRequestData = PopImapRequestCache.Instance.Get(key);
				popImapRequestData.Message = (this[PopImapConditionalHandlerSchema.Message] as string);
				popImapRequestData.LightLogContext = (this[PopImapConditionalHandlerSchema.LightLogContext] as string);
				popImapRequestData.Response = (this[PopImapConditionalHandlerSchema.Response] as string);
				popImapRequestData.RequestTime = ((this[ConditionalHandlerSchema.ElapsedTime] != null) ? TimeSpan.Parse(this[ConditionalHandlerSchema.ElapsedTime].ToString()).TotalMilliseconds : 0.0);
				popImapRequestData.ResponseType = (this[PopImapConditionalHandlerSchema.ResponseType] as string);
				if (popImapRequestData.CommandName == null)
				{
					popImapRequestData.CommandName = commandName;
				}
				if ((popImapRequestData.ErrorDetails == null || popImapRequestData.ErrorDetails.Count == 0) && this[ConditionalHandlerSchema.Exception] != null)
				{
					popImapRequestData.ErrorDetails = new List<ErrorDetail>();
					popImapRequestData.ErrorDetails.Add(new ErrorDetail
					{
						ErrorMessage = this[ConditionalHandlerSchema.Exception].ToString(),
						UserEmail = (this[ConditionalHandlerSchema.SmtpAddress] as string)
					});
				}
			}
			List<ConditionalResults> list = ConditionalRegistrationCache.Singleton.Evaluate(this);
			if (list != null && list.Count > 0)
			{
				foreach (ConditionalResults hit in list)
				{
					ConditionalRegistrationLog.Save(hit);
				}
			}
			base.BeginRead();
		}

		protected override int SendNextChunk(NetworkConnection nc)
		{
			if (this.VerifyMailboxLogEnabled() && this.mailboxLogger != null)
			{
				this.mailboxLogger.AppendLog(string.Format("<sendStart time = \"{0}\" />\r\n", ExDateTime.Now));
			}
			int num = base.SendNextChunk(nc);
			if (num > 0 && this.VerifyMailboxLogEnabled() && this.mailboxLogger != null)
			{
				this.mailboxLogger.AppendLog(string.Format("<sendEnd time = \"{0}\" bytes=\"{1}\"/>\r\n", ExDateTime.Now, num));
			}
			return num;
		}

		protected override void InternalDispose()
		{
			lock (this.LockObject)
			{
				try
				{
					try
					{
						if (this.responseFactory != null)
						{
							this.responseFactory.RecordCommandEnd();
						}
						if (!this.removedFromNonAuth)
						{
							this.RemoveFromUnauthenticatedConnectionsPerIp();
						}
					}
					finally
					{
						if (this.virtualServer != null)
						{
							this.virtualServer.RemoveSession(this);
							this.virtualServer = null;
						}
					}
					if (this.LightLogSession != null)
					{
						if (this.LightLogSession.Command == null)
						{
							this.LightLogSession.BeginCommand(ProtocolSession.CloseBuf);
						}
						this.LightLogSession.CompleteCommand();
						this.lightLogSession = null;
					}
					if (this.LrsSession != null)
					{
						this.LrsSession = null;
					}
					if (this.mailboxLogger != null)
					{
						this.mailboxLogger.AppendLog(string.Format("<disconnected time = \"{0}\"/></session>\r\n", ExDateTime.Now));
						this.mailboxLogger.Flush();
						this.mailboxLogger.Dispose();
						this.mailboxLogger = null;
					}
					if (this.proxySession != null)
					{
						((IDisposable)this.proxySession).Dispose();
						this.proxySession = null;
					}
				}
				finally
				{
					try
					{
						if (this.responseFactory != null)
						{
							this.responseFactory.Dispose();
							this.responseFactory = null;
						}
					}
					finally
					{
						base.InternalDispose();
					}
				}
			}
		}

		protected bool Is7BitString(byte[] buf, int offset, int size)
		{
			for (int i = 0; i < size; i++)
			{
				if (buf[offset + i] != 9 && buf[offset + i] != 10 && buf[offset + i] != 13 && (buf[offset + i] > 126 || buf[offset + i] < 32))
				{
					return false;
				}
			}
			return true;
		}

		protected void ClearOldMailboxLogs()
		{
			MailboxLogger mailboxLogger = this.mailboxLogger;
			if (!this.VerifyMailboxLogEnabled() || mailboxLogger == null || mailboxLogger.LastError != null || this.ResponseFactory == null || !this.ResponseFactory.IsStoreConnected)
			{
				return;
			}
			bool flag = false;
			ProtocolBaseServices.FaultInjectionTracer.TraceTest<bool>(2816879933U, ref flag);
			if (flag)
			{
				mailboxLogger.ClearOldLogs(5000, 10485760L);
				return;
			}
			string uniqueName = this.ResponseFactory.ProtocolUser.UniqueName;
			ExDateTime exDateTime = ProtocolSession.GetNextTimeToClearMailboxLogs(uniqueName);
			if (exDateTime < ExDateTime.UtcNow)
			{
				object perUserClearMailboxLogsLock = ProtocolSession.GetPerUserClearMailboxLogsLock(uniqueName);
				lock (perUserClearMailboxLogsLock)
				{
					exDateTime = ProtocolSession.GetNextTimeToClearMailboxLogs(uniqueName);
					UserConfiguration userConfiguration = null;
					try
					{
						IDictionary dictionary = null;
						if (exDateTime == ExDateTime.MinValue)
						{
							userConfiguration = this.GetUserConfiguration();
							dictionary = userConfiguration.GetDictionary();
							if (dictionary.Contains("nextTimeToClearMailboxLogs"))
							{
								exDateTime = (ExDateTime)dictionary["nextTimeToClearMailboxLogs"];
							}
							else
							{
								exDateTime = ExDateTime.MinValue;
							}
							ProtocolSession.SetNextTimeToClearMailboxLogs(uniqueName, exDateTime);
						}
						if (exDateTime < ExDateTime.UtcNow)
						{
							mailboxLogger.ClearOldLogs(5000, 10485760L);
							exDateTime = ExDateTime.UtcNow.AddHours(6.0);
							ProtocolSession.SetNextTimeToClearMailboxLogs(uniqueName, exDateTime);
							if (userConfiguration == null)
							{
								userConfiguration = this.GetUserConfiguration();
								dictionary = userConfiguration.GetDictionary();
							}
							dictionary["nextTimeToClearMailboxLogs"] = exDateTime;
							userConfiguration.Save();
						}
					}
					catch (StorageTransientException ex)
					{
						ProtocolBaseServices.SessionTracer.TraceError<string, string>(base.SessionId, "Error when trying to get, create, or save UserConfiguration object: {0}\nStack trace: {1}", ex.Message, ex.StackTrace);
					}
					catch (StoragePermanentException ex2)
					{
						ProtocolBaseServices.SessionTracer.TraceError<string, string>(base.SessionId, "Error when trying to get, create, or save UserConfiguration object: {0}\nStack trace: {1}", ex2.Message, ex2.StackTrace);
					}
					finally
					{
						if (userConfiguration != null)
						{
							userConfiguration.Dispose();
						}
					}
				}
			}
		}

		private static ExDateTime GetNextTimeToClearMailboxLogs(string uniqueName)
		{
			ExDateTime minValue;
			lock (ProtocolSession.nextTimeToClearMailboxLogs)
			{
				if (!ProtocolSession.nextTimeToClearMailboxLogs.TryGetValue(uniqueName, out minValue))
				{
					minValue = ExDateTime.MinValue;
					ProtocolSession.nextTimeToClearMailboxLogs[uniqueName] = minValue;
				}
			}
			return minValue;
		}

		private static void SetNextTimeToClearMailboxLogs(string uniqueName, ExDateTime time)
		{
			lock (ProtocolSession.nextTimeToClearMailboxLogs)
			{
				ProtocolSession.nextTimeToClearMailboxLogs[uniqueName] = time;
			}
		}

		private static object GetPerUserClearMailboxLogsLock(string uniqueName)
		{
			object obj2;
			lock (ProtocolSession.nextTimeToClearMailboxLogsLock)
			{
				if (!ProtocolSession.nextTimeToClearMailboxLogsLock.TryGetValue(uniqueName, out obj2))
				{
					obj2 = new object();
					ProtocolSession.nextTimeToClearMailboxLogsLock[uniqueName] = obj2;
				}
			}
			return obj2;
		}

		private UserConfiguration GetUserConfiguration()
		{
			UserConfiguration userConfiguration;
			try
			{
				userConfiguration = this.ResponseFactory.Store.UserConfigurationManager.GetMailboxConfiguration(this.GetUserConfigurationName(), UserConfigurationTypes.Dictionary);
			}
			catch (ObjectNotFoundException)
			{
				try
				{
					userConfiguration = this.ResponseFactory.Store.UserConfigurationManager.CreateMailboxConfiguration(this.GetUserConfigurationName(), UserConfigurationTypes.Dictionary);
					userConfiguration.Save();
				}
				catch (ObjectExistedException)
				{
					userConfiguration = this.ResponseFactory.Store.UserConfigurationManager.GetMailboxConfiguration(this.GetUserConfigurationName(), UserConfigurationTypes.Dictionary);
				}
			}
			return userConfiguration;
		}

		private void TlsNegotiationCompleteCallback(IAsyncResult iar)
		{
			base.EnterCommandProcessing();
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(base.SessionId, "User {0} entering ProtocolSession.TlsNegotiationCompleteCallback.", this.GetUserNameForLogging());
				ProtocolBaseServices.SessionTracer.TraceDebug(base.SessionId, "SSLHandshakeCompleted");
				NetworkConnection networkConnection = (NetworkConnection)iar.AsyncState;
				object obj;
				networkConnection.EndNegotiateTlsAsServer(iar, out obj);
				if (obj != null)
				{
					ProtocolBaseServices.SessionTracer.TraceError(base.SessionId, "TLS negotiation failed: {0}.", new object[]
					{
						obj
					});
					base.Dispose();
				}
				else
				{
					base.NegotiatingTls = false;
					this.isTls = base.Connection.IsTls;
					this.virtualServer.SSLConnections_Total.Increment();
					this.virtualServer.SSLConnections_Current.Increment();
					if (!this.hasShownBanner)
					{
						if (this.AddToUnauthenticatedConnectionsPerIp() && !base.SendToClient(new StringResponseItem(this.BannerString())))
						{
							return;
						}
					}
					else if (!base.SendToClient(new BufferResponseItem(ProtocolSession.EmptyBuffer, 0, 0)))
					{
						return;
					}
					base.SendToClient(new EndResponseItem(new BaseSession.SendCompleteDelegate(this.EndCommandProcess)));
				}
			}
			finally
			{
				base.LeaveCommandProcessing();
				ProtocolBaseServices.InMemoryTraceOperationCompleted(base.SessionId);
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				array[num++] = this[propertyDefinition];
			}
			return array;
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				object result = null;
				if (this.diagnosticsProperties.TryGetValue(propertyDefinition, out result))
				{
					return result;
				}
				if (propertyDefinition == ConditionalHandlerSchema.SmtpAddress)
				{
					if (this.ResponseFactory != null)
					{
						return this.ResponseFactory.PrimarySmtpAddress;
					}
					return null;
				}
				else if (propertyDefinition == ConditionalHandlerSchema.DisplayName)
				{
					if (this.ResponseFactory != null)
					{
						return this.ResponseFactory.UserName;
					}
					return null;
				}
				else if (propertyDefinition == ConditionalHandlerSchema.TenantName)
				{
					if (this.ResponseFactory != null && this.ResponseFactory.ProtocolUser != null)
					{
						return this.ResponseFactory.ProtocolUser.AcceptedDomain;
					}
					return null;
				}
				else if (propertyDefinition == ConditionalHandlerSchema.Cmd)
				{
					if (this.ResponseFactory != null)
					{
						return this.ResponseFactory.CommandName;
					}
					return null;
				}
				else if (propertyDefinition == PopImapConditionalHandlerSchema.Parameters)
				{
					if (this.ResponseFactory != null)
					{
						return this.ResponseFactory.Parameters;
					}
					return null;
				}
				else
				{
					if (propertyDefinition == ConditionalHandlerSchema.ThrottlingPolicyName)
					{
						return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.GetShortIdentityString());
					}
					if (propertyDefinition == ConditionalHandlerSchema.MaxConcurrency)
					{
						if (ProtocolBaseServices.ServiceName == "POP3")
						{
							return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.PopMaxConcurrency);
						}
						return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ImapMaxConcurrency);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.MaxBurst)
					{
						if (ProtocolBaseServices.ServiceName == "POP3")
						{
							return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.PopMaxBurst);
						}
						return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ImapMaxBurst);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.RechargeRate)
					{
						if (ProtocolBaseServices.ServiceName == "POP3")
						{
							return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.PopRechargeRate);
						}
						return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ImapRechargeRate);
					}
					else if (propertyDefinition == ConditionalHandlerSchema.CutoffBalance)
					{
						if (ProtocolBaseServices.ServiceName == "POP3")
						{
							return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.PopCutoffBalance);
						}
						return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ImapCutoffBalance);
					}
					else
					{
						if (propertyDefinition == ConditionalHandlerSchema.ThrottlingPolicyScope)
						{
							return this.GetThrottlingPolicyValue((IThrottlingPolicy policy) => policy.ThrottlingPolicyScope);
						}
						if (propertyDefinition == ConditionalHandlerSchema.BudgetLockedOut)
						{
							ITokenBucket budgetTokenBucket = this.GetBudgetTokenBucket();
							return budgetTokenBucket != null && budgetTokenBucket.Locked;
						}
						if (propertyDefinition == ConditionalHandlerSchema.BudgetLockedUntil)
						{
							ITokenBucket budgetTokenBucket2 = this.GetBudgetTokenBucket();
							return (budgetTokenBucket2 != null) ? budgetTokenBucket2.LockedUntilUtc : new DateTime?(DateTime.MinValue);
						}
						return null;
					}
				}
			}
		}

		public void SetDiagnosticValue(PropertyDefinition propDef, object value)
		{
			this.diagnosticsProperties[propDef] = value;
		}

		public void ClearDiagnosticValue(PropertyDefinition propDef)
		{
			object obj;
			this.diagnosticsProperties.TryRemove(propDef, out obj);
		}

		internal ITokenBucket GetBudgetTokenBucket()
		{
			StandardBudgetWrapper standardBudgetWrapper = this.Budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				return standardBudgetWrapper.GetInnerBudget().CasTokenBucket;
			}
			return null;
		}

		internal void SetBudgetDiagnosticValues(bool start)
		{
			ITokenBucket budgetTokenBucket = this.GetBudgetTokenBucket();
			if (budgetTokenBucket != null)
			{
				float balance = budgetTokenBucket.GetBalance();
				this.SetDiagnosticValue(start ? ConditionalHandlerSchema.BudgetBalanceStart : ConditionalHandlerSchema.BudgetBalanceEnd, balance);
				this.SetDiagnosticValue(start ? ConditionalHandlerSchema.IsOverBudgetAtStart : ConditionalHandlerSchema.IsOverBudgetAtEnd, balance < 0f);
			}
			StandardBudgetWrapper standardBudgetWrapper = this.Budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				this.SetDiagnosticValue(start ? ConditionalHandlerSchema.ConcurrencyStart : ConditionalHandlerSchema.ConcurrencyEnd, standardBudgetWrapper.GetInnerBudget().Connections);
			}
		}

		private const string NextTimeToClearMailboxLogsProperty = "nextTimeToClearMailboxLogs";

		private const int MaxNumberOfMailboxLogs = 5000;

		private const long MaxSizeOfMailboxLogs = 10485760L;

		protected static readonly byte[] EmptyBuffer = new byte[0];

		private static readonly byte[] OpenBuf = Encoding.ASCII.GetBytes("OpenSession");

		private static readonly byte[] CloseBuf = Encoding.ASCII.GetBytes("CloseSession");

		private static RefCountTable<IPAddress> unauthenticatedConnectionsPerIp = new RefCountTable<IPAddress>();

		private static Dictionary<string, ExDateTime> nextTimeToClearMailboxLogs = new Dictionary<string, ExDateTime>();

		private static Dictionary<string, object> nextTimeToClearMailboxLogsLock = new Dictionary<string, object>();

		private ConcurrentDictionary<PropertyDefinition, object> diagnosticsProperties = new ConcurrentDictionary<PropertyDefinition, object>();

		private ResponseFactory responseFactory;

		private ProxySession proxySession;

		private bool okToIssueRead = true;

		private bool isTls;

		private VirtualServer virtualServer;

		private bool hasShownBanner;

		private LightWeightLogSession lightLogSession;

		private ExDateTime mailboxLogTimeout;

		private MailboxLogger mailboxLogger;

		private bool removedFromNonAuth;

		private ExDateTime nonAuthSessionDisconnectTime;
	}
}
