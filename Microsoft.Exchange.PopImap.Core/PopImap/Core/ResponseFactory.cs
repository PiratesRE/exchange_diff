using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ResponseFactory : IDisposeTrackable, IDisposable
	{
		protected ResponseFactory(ProtocolSession session)
		{
			this.session = session;
			this.disposeTracker = this.GetDisposeTracker();
			this.stopwatch = Stopwatch.StartNew();
			this.ActivityId = Guid.Empty;
			if (ResponseFactory.latencyDetectionContextFactory == null)
			{
				lock (ResponseFactory.lockObject)
				{
					if (ResponseFactory.latencyDetectionContextFactory == null)
					{
						ResponseFactory.latencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory(ProtocolBaseServices.ServiceName, ResponseFactory.DefaultMinPopImapThreshold, ResponseFactory.DefaultMinPopImapThreshold);
					}
				}
			}
		}

		public static ExTimeZone CurrentExTimeZone
		{
			get
			{
				if (ResponseFactory.currentExTimeZone == null)
				{
					ResponseFactory.currentExTimeZone = ExTimeZone.CurrentTimeZone;
				}
				return ResponseFactory.currentExTimeZone;
			}
		}

		public static char[] WordDelimiter
		{
			get
			{
				return ResponseFactory.wordDelimiter;
			}
		}

		public static RefCountTable<string> ConnectionsPerUser
		{
			get
			{
				return ResponseFactory.connectionsPerUser;
			}
		}

		public static bool CheckOnlyAuthenticationStatusEnabled { get; set; }

		public static bool EnforceLogsRetentionPolicyEnabled { get; set; }

		public static bool UsePrimarySmtpAddressEnabled { get; set; }

		public static bool IgnoreNonProvisionedServersEnabled { get; set; }

		public static bool AppendServerNameInBannerEnabled { get; set; }

		public static bool GlobalCriminalComplianceEnabled { get; set; }

		public static Func<bool> GetClientAccessRulesEnabled { get; set; }

		public static bool LrsLoggingEnabled { get; set; }

		public static bool KerberosAuthEnabled { get; set; }

		public static ClientAccessProtocol ClientAccessRulesProtocol { get; set; }

		public string CommandName { get; set; }

		public string Parameters { get; set; }

		public bool SkipAuthOnCafeEnabled { get; set; }

		public bool UseSamAccountNameAsUsername { get; protected set; }

		public string DefaultAcceptedDomainName
		{
			get
			{
				string text;
				if (ResponseFactory.defaultAcceptedDomainTable.TryGetValue(this.protocolUser.OrganizationId, out text))
				{
					return text;
				}
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.protocolUser.OrganizationId), 531, "DefaultAcceptedDomainName", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
				tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = this.Session.Budget;
				AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
				if (defaultAcceptedDomain == null)
				{
					ProtocolBaseServices.ServerTracer.TraceError(0L, "No default accepted domain found.");
					ProtocolBaseServices.LogEvent(this.NoDefaultAcceptedDomainFoundEventTuple, null, new string[0]);
					return null;
				}
				text = defaultAcceptedDomain.DomainName.ToString();
				ResponseFactory.defaultAcceptedDomainTable.Add(this.protocolUser.OrganizationId, text);
				return text;
			}
		}

		public uint InvalidCommands
		{
			get
			{
				return this.invalidCommands;
			}
			set
			{
				this.invalidCommands = value;
			}
		}

		public abstract bool IsAuthenticated { get; }

		public abstract bool IsDisconnected { get; }

		public abstract string TimeoutErrorString { get; }

		public abstract string FirstAuthenticateResponse { get; }

		public bool IsInAuthenticationMode
		{
			get
			{
				return this.serverContext != null;
			}
		}

		public bool ExactRFC822SizeEnabled
		{
			get
			{
				if (!this.ProtocolUser.UseProtocolDefaults)
				{
					return this.ProtocolUser.EnableExactRFC822Size;
				}
				return this.Session.Server.EnableExactRFC822Size;
			}
		}

		public bool SuppressReadReceipt
		{
			get
			{
				if (!this.ProtocolUser.UseProtocolDefaults)
				{
					return this.ProtocolUser.SuppressReadReceipt;
				}
				return this.Session.Server.SuppressReadReceipt;
			}
		}

		public bool ForceICalForCalendarRetrievalOption
		{
			get
			{
				return !this.ProtocolUser.UseProtocolDefaults && this.ProtocolUser.ForceICalForCalendarRetrievalOption;
			}
		}

		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
		}

		public AutoResetEvent ConnectionCreated
		{
			get
			{
				return this.connectionCreated;
			}
		}

		public EncryptionType? ProxyEncryptionType
		{
			get
			{
				return this.proxyEncryptionType;
			}
		}

		public ProtocolSession Session
		{
			get
			{
				return this.session;
			}
		}

		public ProtocolRequest IncompleteRequest
		{
			get
			{
				return this.incompleteRequest;
			}
			set
			{
				this.incompleteRequest = value;
			}
		}

		public bool ProxyMode
		{
			get
			{
				return this.session.ProxySession != null;
			}
		}

		public string Mailbox { get; set; }

		public Guid ActivityId { get; set; }

		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.userName = value.Replace('\\', '/');
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.User = this.userName;
						return;
					}
				}
				else
				{
					this.userName = null;
				}
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return this.protocolUser.PrimarySmtpAddress;
			}
		}

		public MailboxSession Store
		{
			get
			{
				if (this.store != null)
				{
					return this.store;
				}
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Session has been disconnected, store object in not available.");
				throw new ResponseFactory.SessionDisconnectedException();
			}
		}

		public bool IsStoreConnected
		{
			get
			{
				bool result;
				try
				{
					result = (this.store != null && this.store.IsConnected);
				}
				catch (ObjectDisposedException)
				{
					result = false;
				}
				return result;
			}
		}

		public bool NeedToReloadStoreStates { get; set; }

		public bool OkToResetRecipientCache
		{
			get
			{
				return this.okToResetRecipientCache;
			}
			set
			{
				this.okToResetRecipientCache = value;
			}
		}

		public UserConfigurationManager CustomStorageManager
		{
			get
			{
				if (this.customStorageManager == null)
				{
					lock (this)
					{
						if (this.customStorageManager == null)
						{
							this.customStorageManager = new UserConfigurationManager(this.Store);
						}
					}
				}
				return this.customStorageManager;
			}
		}

		public abstract string AuthenticationFailureString { get; }

		internal static int PodSiteStartRange { get; set; }

		internal static int PodSiteEndRange { get; set; }

		internal static string PodRedirectTemplate { get; set; }

		internal ProtocolUser ProtocolUser
		{
			get
			{
				return this.protocolUser;
			}
			set
			{
				this.protocolUser = value;
			}
		}

		internal ResourceKey[] ResourceKeys { get; private set; }

		protected abstract string ClientStringForMailboxSession { get; }

		protected abstract ExEventLog.EventTuple NoDefaultAcceptedDomainFoundEventTuple { get; }

		protected abstract BudgetType BudgetType { get; }

		protected uint LoginAttempts
		{
			get
			{
				return this.loginAttempts;
			}
			set
			{
				this.loginAttempts = value;
			}
		}

		protected abstract string AccountInvalidatedString { get; }

		protected bool ServerToServerAuthEnabled { get; set; }

		private protected bool ActualCafeAuthDone { protected get; private set; }

		public static bool IsXsoVersionChanged(int[] previousVersion)
		{
			return previousVersion == null || (previousVersion[0] < ResponseFactory.currentXsoVersion.FileMajorPart || (previousVersion[0] == ResponseFactory.currentXsoVersion.FileMajorPart && previousVersion[1] < ResponseFactory.currentXsoVersion.FileMinorPart) || (previousVersion[0] == ResponseFactory.currentXsoVersion.FileMajorPart && previousVersion[1] == ResponseFactory.currentXsoVersion.FileMinorPart && previousVersion[2] < ResponseFactory.currentXsoVersion.FileBuildPart) || (previousVersion[0] == ResponseFactory.currentXsoVersion.FileMajorPart && previousVersion[1] == ResponseFactory.currentXsoVersion.FileMinorPart && previousVersion[2] == ResponseFactory.currentXsoVersion.FileBuildPart && previousVersion[3] < ResponseFactory.currentXsoVersion.FilePrivatePart));
		}

		public static void RecordCurrentXsoVersion(Folder folder)
		{
			try
			{
				folder[FolderSchema.PopImapConversionVersion] = "15.00.1497.012";
				folder.Save();
			}
			catch (LocalizedException)
			{
			}
		}

		public static int[] GetPreviousXsoVersion(string fileVersion)
		{
			if (fileVersion == null)
			{
				return null;
			}
			string[] array = fileVersion.Split(new char[]
			{
				'.'
			});
			if (array.Length != 4)
			{
				return null;
			}
			int[] array2 = new int[4];
			for (int i = 0; i < array2.Length; i++)
			{
				if (!int.TryParse(array[i], out array2[i]))
				{
					return null;
				}
			}
			return array2;
		}

		public static AuthenticationMechanism GetAuthenticationMechanism(string authenticationMechanismString)
		{
			if (string.Compare(authenticationMechanismString, "ntlm", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (!ProtocolBaseServices.Service.GSSAPIAndNTLMAuthDisabled)
				{
					return AuthenticationMechanism.Ntlm;
				}
				return AuthenticationMechanism.None;
			}
			else if (string.Compare(authenticationMechanismString, "gssapi", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (!ProtocolBaseServices.Service.GSSAPIAndNTLMAuthDisabled)
				{
					return AuthenticationMechanism.Gssapi;
				}
				return AuthenticationMechanism.None;
			}
			else if (string.Compare(authenticationMechanismString, "kerberos", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (!ProtocolBaseServices.Service.GSSAPIAndNTLMAuthDisabled)
				{
					return AuthenticationMechanism.Kerberos;
				}
				return AuthenticationMechanism.None;
			}
			else
			{
				if (string.Compare(authenticationMechanismString, "plain", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return AuthenticationMechanism.Plain;
				}
				return AuthenticationMechanism.None;
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
				this.disposed = true;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ResponseFactory>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		public ProtocolResponse ProcessCommand(byte[] buf, int offset, int size)
		{
			ProtocolResponse protocolResponse = null;
			if (!this.IsSessionValid(out protocolResponse))
			{
				return protocolResponse;
			}
			ProtocolRequest request = this.GenerateRequest(buf, offset, size);
			this.MarkBudgetForRequest(request);
			protocolResponse = this.ProcessRequest(request);
			if (!this.IsAuthenticated && ++this.preAuthCommands >= 9)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Session disconnected after 9 pre-auth commands.");
				protocolResponse.IsDisconnectResponse = true;
			}
			if (protocolResponse != null && protocolResponse.IsCommandFailedResponse && ++this.failedCommands >= 10)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Session disconnected after 10 failed commands.");
				protocolResponse.IsDisconnectResponse = true;
			}
			return protocolResponse;
		}

		public void RecordCommandStart()
		{
			this.stopwatch.Start();
			if (this.ActivityId != Guid.Empty)
			{
				ActivityContextState activityContextState = new ActivityContextState(new Guid?(this.ActivityId), new ConcurrentDictionary<Enum, object>());
				this.Session.ActivityScope = ActivityContext.Resume(activityContextState, null);
			}
			else
			{
				this.Session.ActivityScope = ActivityContext.Start(null);
			}
			if (this.Session.ActivityScope != null)
			{
				this.Session.ActivityScope.Component = ProtocolBaseServices.ServiceName;
				this.Session.ActivityScope.Action = this.CommandName;
			}
			this.Session.SetBudgetDiagnosticValues(true);
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ActivityScope = this.Session.ActivityScope;
			}
		}

		public void RecordCommandEnd()
		{
			if (!this.stopwatch.IsRunning)
			{
				return;
			}
			this.stopwatch.Stop();
			int num = (int)this.stopwatch.ElapsedMilliseconds;
			if (num == 0 && this.stopwatch.ElapsedTicks > 0L)
			{
				num = 1;
			}
			this.stopwatch.Reset();
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ProcessingTime = num;
			}
			this.session.SetDiagnosticValue(ConditionalHandlerSchema.ElapsedTime, TimeSpan.FromMilliseconds((double)num));
			double num2;
			if (this.TryGetActivityContextStat(ActivityOperationType.UserDelay, out num2))
			{
				this.session.SetDiagnosticValue(ConditionalHandlerSchema.BudgetDelay, num2);
			}
			if (this.TryGetActivityContextStat(ActivityOperationType.BudgetUsed, out num2))
			{
				this.session.SetDiagnosticValue(ConditionalHandlerSchema.BudgetUsed, num2);
			}
			this.Session.SetBudgetDiagnosticValues(false);
			if (this.Session.ActivityScope != null)
			{
				this.Session.SetDiagnosticValue(PopImapConditionalHandlerSchema.RequestId, this.Session.ActivityScope.ActivityId.ToString());
			}
			ExPerformanceCounter averageCommandProcessingTime = this.session.VirtualServer.AverageCommandProcessingTime;
			bool flag = this.session.VerifyMailboxLogEnabled();
			if (this.session.ActivityScope != null)
			{
				List<KeyValuePair<string, object>> formattableStatistics = this.Session.ActivityScope.GetFormattableStatistics();
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				foreach (KeyValuePair<string, object> keyValuePair in formattableStatistics)
				{
					if (keyValuePair.Key.StartsWith("MailboxCall.AvgLatency"))
					{
						num3 = (float)keyValuePair.Value;
					}
					if (keyValuePair.Key.StartsWith("ADRead.AvgLatency"))
					{
						num4 = (float)keyValuePair.Value;
					}
					if (keyValuePair.Key.StartsWith("ADWrite.AvgLatency"))
					{
						num5 = (float)keyValuePair.Value;
					}
					if (keyValuePair.Key.StartsWith("ADSearch.AvgLatency"))
					{
						num6 = (float)keyValuePair.Value;
					}
				}
				if (num3 > 0f)
				{
					RatePerfCounters.IncrementLatencyPerfCounter(this.session.VirtualServer.RpcLatencyCounterIndex, (long)num3);
				}
				if (num4 + num5 + num6 > 0f)
				{
					RatePerfCounters.IncrementLatencyPerfCounter(this.session.VirtualServer.LdapLatencyCounterIndex, (long)(num4 + num5 + num6));
				}
				if (flag && this.session.ActivityScope != null)
				{
					this.session.LogInformation(LogRowFormatter.FormatCollection(this.Session.ActivityScope.GetFormattableStatistics()), null);
				}
				PopImapRequestData popImapRequestData = PopImapRequestCache.Instance.Get(this.Session.ActivityScope.ActivityId);
				popImapRequestData.CommandName = this.Session.ResponseFactory.CommandName;
				popImapRequestData.LdapLatency = (double)(num4 + num5 + num6);
				popImapRequestData.RpcLatency = (double)num3;
				popImapRequestData.ServerName = Environment.MachineName;
				popImapRequestData.UserEmail = this.Session.ResponseFactory.UserName;
				popImapRequestData.Parameters = this.Session.ResponseFactory.Parameters;
				if (this.Session.ActivityScope != null)
				{
					this.Session.ActivityScope.Component = null;
					this.Session.ActivityScope.Action = null;
				}
				this.Session.ActivityScope.End();
				this.Session.ActivityScope = null;
			}
			lock (ResponseFactory.commandProcessingTimeSamples)
			{
				int num7 = ResponseFactory.commandProcessingTimeSamples[ResponseFactory.insertionIndex];
				ResponseFactory.commandProcessingTimeSamples[ResponseFactory.insertionIndex] = num;
				ResponseFactory.insertionIndex = (ResponseFactory.insertionIndex + 1) % ResponseFactory.commandProcessingTimeSamples.Length;
				if (ResponseFactory.numSamples < ResponseFactory.commandProcessingTimeSamples.Length)
				{
					ResponseFactory.numSamples++;
				}
				ResponseFactory.latencySum += num;
				ResponseFactory.latencySum -= num7;
				averageCommandProcessingTime.RawValue = (long)(ResponseFactory.latencySum / ResponseFactory.numSamples);
			}
		}

		public abstract ProtocolRequest GenerateRequest(byte[] buf, int offset, int size);

		public ProtocolResponse ProcessRequest(ProtocolRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (this.Disposed)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Session disposed");
				return null;
			}
			ProtocolSession protocolSession = this.Session;
			if (protocolSession == null)
			{
				return null;
			}
			VirtualServer virtualServer = protocolSession.VirtualServer;
			if (virtualServer == null)
			{
				return null;
			}
			virtualServer.Requests_Total.Increment();
			if (request.PerfCounterTotal != null)
			{
				request.PerfCounterTotal.Increment();
			}
			if (!request.VerifyState())
			{
				if (request.PerfCounterFailures != null)
				{
					request.PerfCounterFailures.Increment();
				}
				virtualServer.Requests_Failure.Increment();
				protocolSession.SetDiagnosticValue(PopImapConditionalHandlerSchema.ResponseType, "Err");
				return this.ProcessInvalidState(request);
			}
			request.ParseArguments();
			if (request.ParseResult != ParseResult.notYetParsed && request.ParseResult != ParseResult.success)
			{
				protocolSession.SetDiagnosticValue(PopImapConditionalHandlerSchema.ResponseType, "Err");
				return this.ProcessParseError(request);
			}
			if (!request.IsComplete)
			{
				protocolSession.SetDiagnosticValue(PopImapConditionalHandlerSchema.ResponseType, "Err");
				return this.ProcessIncompleteRequest(request);
			}
			ProtocolBaseServices.Assert(request.ParseResult == ParseResult.success, "Unexpected parse result {0}.", new object[]
			{
				request.ParseResult
			});
			return this.ConnectToTheStoreAndProcessTheRequest(request);
		}

		public virtual void PreProcessRequest(ProtocolRequest request)
		{
			if (this.NeedToReloadStoreStates)
			{
				this.ReloadStoreStates();
				this.NeedToReloadStoreStates = false;
			}
		}

		public ProtocolResponse ConnectToTheStoreAndProcessTheRequest(ProtocolRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			ProtocolBaseServices.SessionTracer.TraceDebug<ProtocolRequest>(this.session.SessionId, "Calling Process: {0}", request);
			ProtocolResponse protocolResponse = null;
			ProtocolResponse result;
			try
			{
				if (this.store != null)
				{
					bool flag = request.NeedsStoreConnection || this.NeedToReloadStoreStates;
					try
					{
						if (flag)
						{
							Monitor.Enter(this.store);
						}
						if (flag)
						{
							this.ConnectToTheStore();
						}
						this.PreProcessRequest(request);
						protocolResponse = request.Process();
					}
					finally
					{
						if (flag)
						{
							this.DisconnectFromTheStore();
							if (Monitor.IsEntered(this.store))
							{
								Monitor.Exit(this.store);
							}
						}
					}
					result = protocolResponse;
				}
				else
				{
					this.session.SetDiagnosticValue(PopImapConditionalHandlerSchema.ResponseType, "OK");
					result = request.Process();
				}
			}
			catch (Exception ex)
			{
				this.session.SetDiagnosticValue(PopImapConditionalHandlerSchema.ResponseType, "Err");
				if (protocolResponse != null)
				{
					protocolResponse.Dispose();
				}
				IProxyLogin proxyLogin = request as IProxyLogin;
				if (proxyLogin != null)
				{
					proxyLogin.AuthenticationError = ex.GetType().ToString() + " " + ex.Message;
				}
				if (!this.session.CheckNonCriticalException(ex))
				{
					throw;
				}
				result = this.ProcessException(request, ex);
			}
			return result;
		}

		public abstract ProtocolResponse ProcessInvalidState(ProtocolRequest request);

		public abstract ProtocolResponse ProcessParseError(ProtocolRequest request);

		public abstract ProtocolResponse CommandIsNotAllASCII(byte[] buf, int offset, int size);

		public virtual ProtocolResponse ProcessIncompleteRequest(ProtocolRequest request)
		{
			ProtocolBaseServices.Assert(false, "ProcessIncompleteRequest is not implemented.", new object[0]);
			return null;
		}

		public abstract ProtocolResponse ProcessException(ProtocolRequest request, Exception exception);

		public abstract ProtocolResponse ProcessException(ProtocolRequest request, Exception exception, string responseString);

		public ProtocolResponse DoAuthenticate(ProtocolRequest request, AuthenticationMechanism authenticationMechanism)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			this.incompleteRequest = request;
			this.authenticationMechanism = authenticationMechanism;
			this.serverContext = this.CreateServerAuthenticationContext();
			SecurityStatus securityStatus;
			if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox && this.authenticationMechanism == AuthenticationMechanism.Kerberos)
			{
				securityStatus = SecurityStatus.OK;
			}
			else
			{
				securityStatus = this.serverContext.InitializeForInboundNegotiate(authenticationMechanism);
			}
			if (securityStatus != SecurityStatus.OK)
			{
				throw new LocalizedException(new LocalizedString("InitializeForInboundNegotiate failed with " + securityStatus));
			}
			this.Session.Connection.MaxLineLength = 4096;
			ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "StartAuthentication completed");
			return new ProtocolResponse(this.FirstAuthenticateResponse);
		}

		public abstract ProtocolResponse AuthenticationDone(ProtocolRequest authenticateRequest, ResponseFactory.AuthenticationResult authenticationResult);

		public ProtocolResponse ProcessAuthentication(byte[] buf, int offset, int size)
		{
			IProxyLogin proxyLogin = (IProxyLogin)this.IncompleteRequest;
			if (size > 0 && buf[offset] == 42)
			{
				this.SetSessionError("AuthCancelled");
				return this.AuthenticationDone(ResponseFactory.AuthenticationResult.cancel);
			}
			int inputLength = size;
			SecurityStatus securityStatus;
			if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox && this.authenticationMechanism == AuthenticationMechanism.Kerberos)
			{
				string text;
				if (!this.ExtractCafeHostname(buf, offset, size, out inputLength, out text))
				{
					return this.AuthenticationDone(ResponseFactory.AuthenticationResult.failure);
				}
				securityStatus = this.serverContext.InitializeForInboundExchangeAuth("SHA256", "IMAP/" + text, this.Session.VirtualServer.Certificate.GetPublicKey(), this.Session.Connection.TlsEapKey);
				if (securityStatus != SecurityStatus.OK)
				{
					this.Session.LogInformation("ExchangeAuth initialization failed for host {0} with status {1}", new object[]
					{
						text,
						securityStatus
					});
					return this.AuthenticationDone(ResponseFactory.AuthenticationResult.failure);
				}
			}
			else if (!this.ExtractClientIP(buf, offset, size, out inputLength))
			{
				return this.AuthenticationDone(ResponseFactory.AuthenticationResult.failure);
			}
			byte[] array;
			securityStatus = this.serverContext.NegotiateSecurityContext(buf, offset, inputLength, out array);
			if (!string.IsNullOrEmpty(this.serverContext.UserName))
			{
				this.UserName = this.serverContext.UserName;
			}
			SecurityStatus securityStatus2 = securityStatus;
			if (securityStatus2 <= SecurityStatus.LogonDenied)
			{
				if (securityStatus2 != SecurityStatus.InvalidToken && securityStatus2 != SecurityStatus.LogonDenied)
				{
				}
			}
			else if (securityStatus2 != SecurityStatus.IllegalMessage)
			{
				if (securityStatus2 != SecurityStatus.OK)
				{
					if (securityStatus2 == SecurityStatus.ContinueNeeded)
					{
						if (array == null)
						{
							throw new InvalidOperationException("No AUTH blob to send");
						}
						ProtocolResponse protocolResponse = new ProtocolResponse("+ ");
						protocolResponse.Append(Encoding.ASCII.GetString(array));
						return protocolResponse;
					}
				}
				else
				{
					if (array != null && array.Length > 0)
					{
						ProtocolResponse protocolResponse = new ProtocolResponse("+ ");
						protocolResponse.Append(Encoding.ASCII.GetString(array));
						if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox && this.authenticationMechanism == AuthenticationMechanism.Kerberos)
						{
							protocolResponse.Append("\r\n");
							protocolResponse.Append(this.AuthenticationDone(ResponseFactory.AuthenticationResult.authenticatedAsCafe).DataToSend);
							if (this.Session.LightLogSession != null)
							{
								this.Session.LightLogSession.LiveIdAuthResult = "AuthenticatedAsCafe";
							}
						}
						return protocolResponse;
					}
					if (this.Session.LightLogSession != null)
					{
						if ((this.authenticationMechanism == AuthenticationMechanism.Plain || this.authenticationMechanism == AuthenticationMechanism.Login) && proxyLogin != null && proxyLogin.LiveIdBasicAuth != null)
						{
							this.Session.LightLogSession.LiveIdAuthResult = proxyLogin.LiveIdBasicAuth.LastAuthResult.ToString();
						}
						else
						{
							this.Session.LightLogSession.LiveIdAuthResult = SecurityStatus.OK.ToString();
						}
					}
					return this.AuthenticationDone(ResponseFactory.AuthenticationResult.success);
				}
			}
			string text2 = securityStatus.ToString();
			string text3 = text2;
			string text4 = string.Empty;
			if (proxyLogin != null && proxyLogin.LiveIdBasicAuth != null)
			{
				text4 = proxyLogin.LiveIdBasicAuth.LastAuthResult.ToString();
				if (proxyLogin.LiveIdBasicAuth.LastAuthResult != LiveIdAuthResult.Success)
				{
					text3 = text3 + "-" + text4;
				}
				if (!string.IsNullOrEmpty(proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage))
				{
					text2 = text2 + "-" + proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage.Replace('"', '\'').Replace("\r\n", " ");
				}
			}
			this.Session.LogInformation("{0} authentication failed, {1}", new object[]
			{
				this.authenticationMechanism,
				text3
			});
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ErrorMessage = "AuthFailed:" + text3;
				this.Session.LightLogSession.LiveIdAuthResult = text4;
			}
			this.SetSessionError(string.Format("AuthFailed:{0},User: {1}", text2, this.serverContext.UserName ?? "not found"));
			return this.AuthenticationDone(ResponseFactory.AuthenticationResult.failure);
		}

		public void AddExceptionToCache(Exception exception)
		{
			if (this.Session.ActivityScope != null)
			{
				Guid activityId = this.Session.ActivityScope.ActivityId;
				PopImapRequestData popImapRequestData = PopImapRequestCache.Instance.Get(this.Session.ActivityScope.ActivityId);
				popImapRequestData.HasErrors = true;
				if (popImapRequestData.ErrorDetails == null)
				{
					popImapRequestData.ErrorDetails = new List<ErrorDetail>();
				}
				popImapRequestData.ErrorDetails.Add(new ErrorDetail
				{
					UserEmail = this.Session.ResponseFactory.UserName,
					StackTrace = exception.StackTrace,
					ErrorMessage = exception.Message,
					ErrorType = exception.GetType().ToString()
				});
			}
		}

		public void LogHandledException(Exception exception)
		{
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ExceptionCaught = exception;
			}
			this.SetSessionError(exception);
			if (exception is ObjectNotFoundException || exception is OverBudgetException)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<Type, string, Exception>(this.session.SessionId, "Exception {0} caught for user {1}: {2}", exception.GetType(), this.UserName, exception);
				return;
			}
			ProtocolBaseServices.SessionTracer.TraceError<string, Exception>(this.session.SessionId, "Exception caught for user {0}: {1}", this.UserName, exception);
			if (exception is ConnectionFailedTransientException || exception is ConnectionFailedPermanentException)
			{
				this.session.BeginShutdown(this.ProcessException(null, exception).DataToSend);
			}
		}

		public abstract void DoPostLoginTasks();

		public abstract bool DoProxyConnect(byte[] buf, int offset, int size, ProxySession proxySession);

		public OutboundConversionOptions GetOutboundConversionOptions()
		{
			if (this.options == null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.protocolUser.OrganizationId), 1858, "GetOutboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
				tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = this.Session.Budget;
				this.options = new OutboundConversionOptions(this.DefaultAcceptedDomainName);
				this.options.UserADSession = tenantOrRootOrgRecipientSession;
				this.options.LoadPerOrganizationCharsetDetectionOptions(this.protocolUser.OrganizationId);
				if (this.options.ByteEncoderTypeFor7BitCharsets.Equals(ByteEncoderTypeFor7BitCharsetsEnum.Undefined))
				{
					string value = ConfigurationManager.AppSettings["ByteEncoderTypeFor7BitCharsets"];
					ByteEncoderTypeFor7BitCharsets byteEncoderTypeFor7BitCharsets;
					if (!string.IsNullOrEmpty(value) && EnumValidator.TryParse<ByteEncoderTypeFor7BitCharsets>(value, EnumParseOptions.AllowNumericConstants | EnumParseOptions.IgnoreCase, out byteEncoderTypeFor7BitCharsets))
					{
						this.options.ByteEncoderTypeFor7BitCharsets = byteEncoderTypeFor7BitCharsets;
					}
				}
				MimeTextFormat mimeTextFormat = this.protocolUser.UseProtocolDefaults ? this.session.Server.MessagesRetrievalMimeTextFormat : this.protocolUser.MessagesRetrievalMimeTextFormat;
				this.options.IsSenderTrusted = false;
				this.options.DemoteBcc = true;
				this.options.InternetMessageFormat = InternetMessageFormat.Mime;
				switch (mimeTextFormat)
				{
				case MimeTextFormat.TextOnly:
					this.options.InternetTextFormat = InternetTextFormat.TextOnly;
					break;
				case MimeTextFormat.HtmlOnly:
					this.options.InternetTextFormat = InternetTextFormat.HtmlOnly;
					break;
				case MimeTextFormat.HtmlAndTextAlternative:
					this.options.InternetTextFormat = InternetTextFormat.HtmlAndTextAlternative;
					break;
				case MimeTextFormat.TextEnrichedOnly:
					this.options.InternetTextFormat = InternetTextFormat.TextEnrichedOnly;
					break;
				case MimeTextFormat.TextEnrichedAndTextAlternative:
					this.options.InternetTextFormat = InternetTextFormat.TextEnrichedAndTextAlternative;
					break;
				case MimeTextFormat.BestBodyFormat:
					this.options.InternetTextFormat = InternetTextFormat.BestBody;
					break;
				case MimeTextFormat.Tnef:
					this.options.InternetMessageFormat = InternetMessageFormat.Tnef;
					this.options.InternetTextFormat = InternetTextFormat.TextOnly;
					break;
				}
				if (!string.IsNullOrEmpty(this.owaServer))
				{
					try
					{
						this.options.OwaServer = this.owaServer;
					}
					catch (ArgumentException ex)
					{
						ProtocolBaseServices.SessionTracer.TraceError<string, string>(this.session.SessionId, "Unable to set OWA server url for user {0}.\r\n{1}", this.UserName, ex.Message);
						ProtocolBaseServices.LogEvent(this.protocolUser.OwaServerInvalidEventTuple, this.UserName, new string[]
						{
							this.UserName,
							ex.Message
						});
					}
				}
				this.options.ClearCategories = false;
				string value2 = ConfigurationManager.AppSettings["QuoteDisplayNameBeforeRfc2047Encoding"];
				bool quoteDisplayNameBeforeRfc2047Encoding = false;
				bool.TryParse(value2, out quoteDisplayNameBeforeRfc2047Encoding);
				this.options.QuoteDisplayNameBeforeRfc2047Encoding = quoteDisplayNameBeforeRfc2047Encoding;
				string text = ConfigurationManager.AppSettings["OverrideCalendarMessageCharset"];
				Charset charset;
				if (!string.IsNullOrEmpty(text) && Charset.TryGetCharset(text, out charset) && charset.IsAvailable)
				{
					this.options.DetectionOptions.PreferredCharset = charset;
				}
				if (this.recipientCacheResetTimer == null)
				{
					this.recipientCacheResetTimer = new Timer(new TimerCallback(this.ResetRecipientCache), this.options, ResponseFactory.DefaultRecipientCacheResetInterval, ResponseFactory.DefaultRecipientCacheResetInterval);
				}
			}
			return this.options;
		}

		public InboundConversionOptions GetInboundConversionOptions()
		{
			if (this.inboundOptions == null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.protocolUser.OrganizationId), 1976, "GetInboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
				tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = this.Session.Budget;
				this.inboundOptions = new InboundConversionOptions(this.DefaultAcceptedDomainName);
				this.inboundOptions.UserADSession = tenantOrRootOrgRecipientSession;
				this.inboundOptions.LoadPerOrganizationCharsetDetectionOptions(this.protocolUser.OrganizationId);
			}
			return this.inboundOptions;
		}

		public bool ConnectToTheStore()
		{
			bool flag = false;
			if (this.store != null)
			{
				flag = this.Store.ConnectWithStatus();
				if (flag)
				{
					this.NeedToReloadStoreStates = true;
				}
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Store.Connected");
			}
			return flag;
		}

		public void DisconnectFromTheStore()
		{
			if (this.IsStoreConnected)
			{
				this.Store.Disconnect();
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Store.Disconnected");
			}
		}

		internal bool DeleteMessages(StoreObjectId[] messages)
		{
			if (messages.Length == 0)
			{
				return true;
			}
			List<StoreObjectId> list;
			if (messages.Length <= 256)
			{
				list = new List<StoreObjectId>(messages.Length);
				for (int i = 0; i < messages.Length; i++)
				{
					if (messages[i] != null)
					{
						list.Add(messages[i]);
					}
				}
				if (list.Count > 0)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "Delete {0} items", list.Count);
					AggregateOperationResult aggregateOperationResult = this.store.Delete(DeleteItemFlags.SoftDelete, list.ToArray());
					return aggregateOperationResult.OperationResult == OperationResult.Succeeded;
				}
			}
			list = new List<StoreObjectId>(256);
			int num = 2;
			for (int j = 0; j < messages.Length; j++)
			{
				if (messages[j] != null)
				{
					list.Add(messages[j]);
				}
				if (list.Count >= 256 || j == messages.Length - 1)
				{
					this.BackOffFromStore(ref num);
					ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "Delete {0} items", list.Count);
					AggregateOperationResult aggregateOperationResult2 = this.store.Delete(DeleteItemFlags.SoftDelete, list.ToArray());
					if (aggregateOperationResult2.OperationResult != OperationResult.Succeeded)
					{
						return false;
					}
					list.Clear();
				}
			}
			return true;
		}

		internal void MarkAsRead(StoreObjectId[] messages)
		{
			if (messages.Length == 0)
			{
				return;
			}
			List<StoreObjectId> list;
			if (messages.Length <= 256)
			{
				list = new List<StoreObjectId>(messages.Length);
				for (int i = 0; i < messages.Length; i++)
				{
					if (messages[i] != null)
					{
						list.Add(messages[i]);
					}
				}
				ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "MarkAsRead {0} items", list.Count);
				this.store.MarkAsRead(this.SuppressReadReceipt, this.SuppressReadReceipt, list.ToArray());
				return;
			}
			list = new List<StoreObjectId>(256);
			int num = 2;
			for (int j = 0; j < messages.Length; j++)
			{
				if (messages[j] != null)
				{
					list.Add(messages[j]);
				}
				if (list.Count >= 256 || j == messages.Length - 1)
				{
					this.BackOffFromStore(ref num);
					ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "MarkAsRead {0} items", list.Count);
					this.store.MarkAsRead(this.SuppressReadReceipt, this.SuppressReadReceipt, list.ToArray());
					list.Clear();
				}
			}
		}

		internal AggregateOperationResult MoveItems(Folder folder, StoreObjectId destinationUid, bool returnNewIds, StoreObjectId[] messages)
		{
			return this.CopyorMoveItems(folder, destinationUid, returnNewIds, messages, false);
		}

		internal AggregateOperationResult CopyItems(Folder folder, StoreObjectId destinationUid, bool returnNewIds, StoreObjectId[] messages)
		{
			return this.CopyorMoveItems(folder, destinationUid, returnNewIds, messages, true);
		}

		internal OperationResult MoveItems(Folder folder, StoreObjectId destinationUid, StoreObjectId[] messages)
		{
			if (messages.Length == 0)
			{
				return OperationResult.Succeeded;
			}
			if (messages.Length <= 256)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "MoveItems {0} items", messages.Length);
				return folder.MoveItems(destinationUid, messages).OperationResult;
			}
			List<StoreObjectId> list = new List<StoreObjectId>(256);
			int num = 2;
			for (int i = 0; i < messages.Length; i++)
			{
				list.Add(messages[i]);
				if (list.Count >= 256 || i == messages.Length - 1)
				{
					this.BackOffFromStore(ref num);
					ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "MoveItems {0} items", list.Count);
					OperationResult operationResult = folder.MoveItems(destinationUid, list.ToArray()).OperationResult;
					if (operationResult != OperationResult.Succeeded)
					{
						return operationResult;
					}
					list.Clear();
				}
			}
			return OperationResult.Succeeded;
		}

		internal void BackOffFromStore(ref int backoffDelay)
		{
			if (this.store.IsInBackoffState)
			{
				Thread.Sleep(backoffDelay);
				if (backoffDelay < 1024)
				{
					backoffDelay <<= 2;
					return;
				}
			}
			else
			{
				backoffDelay = 2;
			}
		}

		internal IStandardBudget AcquirePerCommandBudget()
		{
			if (!this.IsAuthenticated || this.Session.Budget == null)
			{
				return null;
			}
			IStandardBudget budget = this.Session.Budget;
			bool flag = false;
			IStandardBudget standardBudget = StandardBudget.Acquire(budget.Owner);
			IStandardBudget result;
			try
			{
				standardBudget.CheckOverBudget();
				ResourceLoadDelayInfo.CheckResourceHealth(standardBudget, this.Session.WorkloadSettings, this.ResourceKeys);
				standardBudget.StartLocal("ResponseFactory.AcquirePerCommandBudget", default(TimeSpan));
				flag = true;
				result = standardBudget;
			}
			finally
			{
				if (!flag)
				{
					standardBudget.Dispose();
				}
			}
			return result;
		}

		protected static bool CanProxyTo(Service service, ExchangePrincipal exchangePrincipal)
		{
			ServerVersion serverVersion = new ServerVersion(service.ServerVersionNumber);
			ServerVersion serverVersion2 = new ServerVersion(exchangePrincipal.MailboxInfo.Location.ServerVersion);
			return (serverVersion.Major == Server.Exchange2011MajorVersion && serverVersion2.Major == Server.Exchange2011MajorVersion) || (serverVersion.Major == Server.Exchange2009MajorVersion && serverVersion2.Major == Server.Exchange2009MajorVersion) || (serverVersion.Major == Server.Exchange2007MajorVersion && serverVersion2.Major == Server.Exchange2007MajorVersion);
		}

		protected virtual void MarkBudgetForRequest(ProtocolRequest request)
		{
			if (this.Session.Budget != null && this.Session.Budget.LocalCostHandle != null)
			{
				this.Session.Budget.LocalCostHandle.MaxLiveTime = request.GetBudgetActionTimeout();
			}
		}

		protected abstract void ReloadStoreStates();

		protected abstract int GetE15MbxProxyPort(string e15MbxFqdn);

		protected abstract int GetE15MbxProxyPort(string e15MbxFqdn, bool isCrossForest, string userDomain);

		protected int GetE15MbxProxyPort<T>(string e15MbxFqdn, MruDictionaryCache<string, int> proxyPortCache, bool isCrossForest = false, string userDomain = "") where T : PopImapAdConfiguration, new()
		{
			int num = this.session.Server.ProxyPort;
			if (!proxyPortCache.TryGetValue(e15MbxFqdn, out num))
			{
				num = this.session.Server.ProxyPort;
				try
				{
					ITopologyConfigurationSession topologyConfigurationSession;
					if (isCrossForest && !string.IsNullOrEmpty(userDomain))
					{
						string resourceForestFqdnByAcceptedDomainName = ResponseFactory.GetResourceForestFqdnByAcceptedDomainName(userDomain);
						topologyConfigurationSession = ADSystemConfigurationSession.CreateRemoteForestSession(resourceForestFqdnByAcceptedDomainName, null);
					}
					else
					{
						topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2438, "GetE15MbxProxyPort", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
					}
					if (topologyConfigurationSession != null)
					{
						PopImapAdConfiguration popImapAdConfiguration = PopImapAdConfiguration.FindOne<T>(topologyConfigurationSession, e15MbxFqdn);
						if (popImapAdConfiguration != null)
						{
							num = popImapAdConfiguration.ProxyTargetPort;
						}
						if (num > 0 && num < 65535)
						{
							proxyPortCache[e15MbxFqdn] = num;
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.Session.CheckNonCriticalException(exception))
					{
						throw;
					}
				}
			}
			if (num <= 0 || num >= 65535)
			{
				ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "Proxy port could not be found.");
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.Message = "ProxyTargetPort from Config not found. Use Default port.";
				}
				num = (string.Equals(ProtocolBaseServices.ServiceName, "POP3", StringComparison.InvariantCultureIgnoreCase) ? 1995 : 1993);
			}
			ProtocolBaseServices.SessionTracer.TraceError<string, int>(this.session.SessionId, "Proxy port for {0} is {1}.", e15MbxFqdn, num);
			return num;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ReleaseResources();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.recipientCacheResetTimer != null)
				{
					this.recipientCacheResetTimer.Dispose();
					this.recipientCacheResetTimer = null;
				}
			}
		}

		protected abstract ProxySession NewProxySession(NetworkConnection connection);

		protected bool TryToConnect(SecureString password, out bool loginSucceeded)
		{
			loginSucceeded = false;
			if (string.IsNullOrEmpty(this.UserName))
			{
				ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "Empty user name.");
				return false;
			}
			if (this.clientSecurityContext != null)
			{
				((IDisposable)this.clientSecurityContext).Dispose();
				this.clientSecurityContext = null;
			}
			bool result;
			try
			{
				loginSucceeded = this.Logon(password, out this.clientSecurityContext);
				if (!loginSucceeded)
				{
					result = false;
				}
				else
				{
					bool flag = this.TryToConnect(this.clientSecurityContext);
					loginSucceeded = string.IsNullOrEmpty(((IProxyLogin)this.incompleteRequest).AuthenticationError);
					result = flag;
				}
			}
			finally
			{
				using (this.clientSecurityContext)
				{
				}
			}
			return result;
		}

		internal bool TryToConnect(string cat, string mailbox, string cafeActivityId)
		{
			if (this.clientSecurityContext != null)
			{
				((IDisposable)this.clientSecurityContext).Dispose();
				this.clientSecurityContext = null;
			}
			bool result;
			try
			{
				this.clientSecurityContext = this.GetClientSecurityContext(cat);
				if (!string.IsNullOrEmpty(mailbox) && !mailbox.Equals("\"\""))
				{
					this.Mailbox = mailbox;
				}
				if (this.ActivityId == Guid.Empty)
				{
					this.ActivityId = Guid.Parse(cafeActivityId);
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.CafeActivityId = cafeActivityId;
					}
				}
				result = this.TryToConnect(this.clientSecurityContext);
			}
			finally
			{
				this.Session.SetMaxCommandLength(this.Session.Server.MaxCommandLength);
				using (this.clientSecurityContext)
				{
				}
			}
			return result;
		}

		protected bool TryToConnect(ClientSecurityContext clientSecurityContext)
		{
			if (!this.CheckConnectionLimit())
			{
				return false;
			}
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			bool flag = false;
			bool result;
			try
			{
				if (proxyLogin.AdUser == null)
				{
					proxyLogin.AdUser = this.protocolUser.FindAdUser(proxyLogin.UserName, this.userSid, this.userPuid);
				}
				if (proxyLogin.AdUser == null || !this.ConfigureUser(proxyLogin.AdUser))
				{
					ProtocolBaseServices.SessionTracer.TraceError<string>(this.session.SessionId, "User {0} could not be found in Active Directory.", this.Session.GetUserNameForLogging());
					result = false;
				}
				else
				{
					SmtpAddress primarySmtpAddress = proxyLogin.AdUser.PrimarySmtpAddress;
					if (string.IsNullOrEmpty(proxyLogin.AdUser.PrimarySmtpAddress.Domain))
					{
						ProtocolBaseServices.SessionTracer.TraceError<string>(this.session.SessionId, "User {0} has no PrimarySmtpAddress in AD.", proxyLogin.UserName);
						this.SetSessionError("NoPrimarySmtpAddress");
						result = false;
					}
					else
					{
						this.userSid = proxyLogin.AdUser.Sid;
						this.AcquireSessionBudget();
						if (ResponseFactory.UsePrimarySmtpAddressEnabled)
						{
							this.UserName = proxyLogin.AdUser.PrimarySmtpAddress.ToString();
						}
						else
						{
							this.UserName = (this.UseSamAccountNameAsUsername ? proxyLogin.AdUser.SamAccountName : proxyLogin.AdUser.Alias);
						}
						if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox)
						{
							if (!this.OpenMailboxSession(clientSecurityContext, proxyLogin.AdUser))
							{
								return false;
							}
						}
						else
						{
							if (ProtocolBaseServices.ServerRoleService != ServerServiceRole.cafe)
							{
								this.SetSessionError("UnknownServerRole");
								return false;
							}
							if (!this.ProxyConnect(proxyLogin.AdUser))
							{
								return false;
							}
						}
						this.Session.RemoveFromUnauthenticatedConnectionsPerIp();
						flag = true;
						result = true;
					}
				}
			}
			catch (LocalizedException ex)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ExceptionCaught = ex;
				}
				this.SetSessionError(ex);
				result = false;
			}
			finally
			{
				if (!flag)
				{
					this.session.DisableUserTracing();
					this.ReleaseResources();
					this.protocolUser.Reset();
					if (string.IsNullOrEmpty(proxyLogin.AuthenticationError) && this.Session.LightLogSession != null)
					{
						proxyLogin.AuthenticationError = this.Session.LightLogSession.ErrorMessage;
					}
				}
			}
			return result;
		}

		protected abstract IEnumerable<EmailTransportService> GetProxyDestinations(ExchangePrincipal exchangePrincipal);

		protected void TraceProxyResponse(byte[] buf, int offset, int size)
		{
			if (!this.disposed && ProtocolBaseServices.SessionTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				string @string = Encoding.ASCII.GetString(buf, offset, size);
				this.Session.LogInformation("<<< ProxyResponse:{0}<<<", new object[]
				{
					@string
				});
			}
		}

		protected byte[] GetPlainAuthBlob()
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			if (proxyLogin == null || proxyLogin.Password == null)
			{
				return null;
			}
			byte[] result;
			using (AuthenticationContext authenticationContext = new AuthenticationContext())
			{
				SecurityStatus securityStatus = authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Plain, null, proxyLogin.UserName, null, proxyLogin.Password);
				if (securityStatus != SecurityStatus.OK)
				{
					throw new LocalizedException(new LocalizedString(string.Format("Unexpected response from InitializeForOutboundNegotiate: {0}", securityStatus)));
				}
				if (!string.IsNullOrEmpty(this.Mailbox))
				{
					authenticationContext.AuthorizationIdentity = Encoding.ASCII.GetBytes(this.Mailbox);
				}
				byte[] array;
				try
				{
					securityStatus = authenticationContext.NegotiateSecurityContext(null, out array);
				}
				catch (Exception exception)
				{
					if (!this.Session.CheckNonCriticalException(exception))
					{
						throw;
					}
					return null;
				}
				if (securityStatus != SecurityStatus.OK)
				{
					throw new LocalizedException(new LocalizedString(string.Format("Unexpected response from NegotiateSecurityContext: {0}", securityStatus)));
				}
				if (ProtocolBaseServices.GCCEnabledWithKeys || (ResponseFactory.GetClientAccessRulesEnabled() && this.clientAccessRulesSupportedByTargetServer))
				{
					string text = string.IsNullOrEmpty(proxyLogin.ClientIp) ? this.Session.Connection.RemoteEndPoint.Address.ToString() : proxyLogin.ClientIp;
					ProtocolBaseServices.FaultInjectionTracer.TraceTest<string>(2709925181U, ref text);
					if (!string.IsNullOrEmpty(text))
					{
						if (this.ActivityId == Guid.Empty && this.Session.ActivityScope != null)
						{
							this.ActivityId = this.Session.ActivityScope.ActivityId;
							if (this.Session.LightLogSession != null)
							{
								this.Session.LightLogSession.CafeActivityId = this.ActivityId.ToString();
							}
						}
						int num = array.Length;
						string text2 = this.ActivityId.ToString();
						string text3 = ProtocolBaseServices.GCCEnabledWithKeys ? GccUtils.GetAuthStringForThisServer() : string.Empty;
						string text4 = this.Session.Connection.RemoteEndPoint.Port.ToString();
						string s = (ResponseFactory.GetClientAccessRulesEnabled() && this.clientAccessRulesSupportedByTargetServer) ? string.Format("\0{0}\0{1}\0{2}\0{3}\0{4}\r\n", new object[]
						{
							text,
							text3,
							text2,
							text4,
							string.Empty
						}) : string.Format("\0{0}\0{1}\0{2}\r\n", text, text3, text2);
						byte[] bytes = Encoding.ASCII.GetBytes(s);
						Array.Resize<byte>(ref array, num + bytes.Length);
						Array.Copy(bytes, 0, array, num, bytes.Length);
						return array;
					}
				}
				Array.Resize<byte>(ref array, array.Length + 2);
				array[array.Length - 2] = 13;
				array[array.Length - 1] = 10;
				result = array;
			}
			return result;
		}

		protected string GetServerAuthXProxyCommand(string command)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			IProxyLogin proxyLogin = (IProxyLogin)this.incompleteRequest;
			stringBuilder.Append(command);
			stringBuilder.Append(" ");
			stringBuilder.Append(this.UserName);
			stringBuilder.Append(" ");
			stringBuilder.Append(this.catToken);
			stringBuilder.Append(" ");
			if (!string.IsNullOrEmpty(this.Mailbox))
			{
				stringBuilder.Append(this.Mailbox);
			}
			else
			{
				stringBuilder.Append("\"\"");
			}
			stringBuilder.Append(" ");
			if (this.ActivityId == Guid.Empty)
			{
				this.ActivityId = this.session.ActivityScope.ActivityId;
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.CafeActivityId = this.ActivityId.ToString();
				}
			}
			stringBuilder.Append(this.ActivityId);
			if (ProtocolBaseServices.GCCEnabledWithKeys)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(GccUtils.GetAuthStringForThisServer());
				stringBuilder.Append(" ");
				if (!string.IsNullOrEmpty(proxyLogin.ClientIp))
				{
					stringBuilder.Append(proxyLogin.ClientIp);
				}
				else if (this.Session.Connection.RemoteEndPoint.Address != null)
				{
					stringBuilder.Append(this.Session.Connection.RemoteEndPoint.Address.ToString());
				}
				stringBuilder.Append(" ");
				stringBuilder.Append(this.session.LocalEndPoint.Address);
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		protected void ProxyConnectionEncryptionComplete(IAsyncResult iar)
		{
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.session.SessionId, "User {0} entering ResponseFactory.ProxyConnectionEncryptionComplete.", this.Session.GetUserNameForLogging());
				ProxySession proxySession = (ProxySession)iar.AsyncState;
				NetworkConnection connection = proxySession.Connection;
				if (connection == null)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "ProxyConnectionEncryptionComplete(): proxySession.Connection is null.");
					proxySession.State = ProxySession.ProxyState.failed;
				}
				else
				{
					object obj;
					connection.EndNegotiateTlsAsClient(iar, out obj);
					if (obj != null)
					{
						this.Session.LogInformation("ProxyConnectionEncryptionComplete(): EndNegotiateTlsAsClient error result: {0}", new object[]
						{
							obj
						});
						proxySession.State = ProxySession.ProxyState.failed;
					}
					else if (ProtocolBaseServices.EnforceCertificateErrors && !connection.RemoteCertificate.Verify())
					{
						this.Session.LogInformation("ProxyConnectionEncryptionComplete(): Invalid Certificate", new object[0]);
						proxySession.State = ProxySession.ProxyState.failed;
					}
					else if (this.Session.Disposed)
					{
						ProtocolBaseServices.SessionTracer.TraceDebug(this.Session.SessionId, "Incoming session is disposed, nothing to do.");
						proxySession.State = ProxySession.ProxyState.failed;
					}
					else if (this.ProxyEncryptionType == EncryptionType.SSL)
					{
						proxySession.EnterReadLoop(connection);
					}
					else if (this.ProxyEncryptionType == EncryptionType.TLS)
					{
						proxySession.TransitProxyState(null, 0, 0);
					}
				}
			}
			finally
			{
				ProtocolBaseServices.InMemoryTraceOperationCompleted(this.session.SessionId);
			}
		}

		protected void AcquireSessionBudget()
		{
			if (this.Session.Budget != null)
			{
				return;
			}
			IStandardBudget standardBudget = null;
			lock (this.Session)
			{
				if (this.Session.Budget != null)
				{
					return;
				}
				this.Session.Budget = ((this.userSid != null) ? StandardBudget.Acquire(this.userSid, this.BudgetType, this.protocolUser.GetSessionSettings()) : StandardBudget.AcquireFallback(this.UserName, this.BudgetType));
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.Budget = this.Session.Budget;
				}
				standardBudget = this.Session.Budget;
			}
			standardBudget.CheckOverBudget();
			ResourceLoadDelayInfo.CheckResourceHealth(standardBudget, this.Session.WorkloadSettings, this.ResourceKeys);
			standardBudget.StartConnection("ResponseFactory.AcquireSessionBudget");
		}

		protected void SetSessionError(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (exception != null)
			{
				stringBuilder.Append(exception.GetType());
				stringBuilder.Append(':');
				stringBuilder.Append(exception.Message);
				exception = exception.InnerException;
				if (exception != null)
				{
					stringBuilder.Append(" --> ");
				}
			}
			this.SetSessionError(stringBuilder.ToString());
		}

		protected void SetSessionError(string error)
		{
			ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, error);
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.Message = error;
			}
			IProxyLogin proxyLogin = this.IncompleteRequest as IProxyLogin;
			if (proxyLogin != null)
			{
				proxyLogin.AuthenticationError = error;
			}
		}

		private static int GetMajorVersionFromVersionNumber(int versionNumber)
		{
			return versionNumber >> 22 & 63;
		}

		private static string GetResourceForestFqdnByAcceptedDomainName(string tenantAcceptedDomain)
		{
			return ADAccountPartitionLocator.GetResourceForestFqdnByAcceptedDomainName(tenantAcceptedDomain);
		}

		private AggregateOperationResult CopyorMoveItems(Folder folder, StoreObjectId destinationUid, bool returnNewIds, StoreObjectId[] messages, bool copy)
		{
			if (messages.Length == 0)
			{
				return new AggregateOperationResult(OperationResult.Succeeded, null);
			}
			string arg = copy ? "Copy" : "Move";
			List<StoreObjectId> list;
			if (messages.Length > 256)
			{
				list = new List<StoreObjectId>(256);
				AggregateOperationResult aggregateOperationResult = null;
				int num = 2;
				for (int i = 0; i < messages.Length; i++)
				{
					if (messages[i] != null)
					{
						list.Add(messages[i]);
					}
					if (list.Count >= 256 || i == messages.Length - 1)
					{
						this.BackOffFromStore(ref num);
						ProtocolBaseServices.SessionTracer.TraceDebug<string, int>(this.session.SessionId, "{0}Items {1} items", arg, list.Count);
						AggregateOperationResult aggregateOperationResult2;
						if (copy)
						{
							aggregateOperationResult2 = folder.CopyObjects(folder.Session, destinationUid, returnNewIds, list.ToArray());
						}
						else
						{
							aggregateOperationResult2 = folder.MoveObjects(folder.Session, destinationUid, returnNewIds, list.ToArray());
						}
						ProtocolBaseServices.SessionTracer.TraceDebug<string, OperationResult>(this.session.SessionId, "{0} returned {1}.", arg, aggregateOperationResult2.OperationResult);
						if (aggregateOperationResult == null)
						{
							aggregateOperationResult = aggregateOperationResult2;
						}
						else
						{
							aggregateOperationResult = AggregateOperationResult.Merge(aggregateOperationResult, aggregateOperationResult2);
						}
						list.Clear();
					}
				}
				return aggregateOperationResult;
			}
			list = new List<StoreObjectId>(messages.Length);
			for (int j = 0; j < messages.Length; j++)
			{
				if (messages[j] != null)
				{
					list.Add(messages[j]);
				}
			}
			ProtocolBaseServices.SessionTracer.TraceDebug<string, int>(this.session.SessionId, "{0}Items {1} items", arg, list.Count);
			if (copy)
			{
				return folder.CopyObjects(folder.Session, destinationUid, returnNewIds, list.ToArray());
			}
			return folder.MoveObjects(folder.Session, destinationUid, returnNewIds, list.ToArray());
		}

		private SecurityStatus SkipAuthenticationOnCafe(byte[] userBytes, byte[] passBytes, Guid requestId, out string commonAccessToken, out IAccountValidationContext accountValidationContext)
		{
			IProxyLogin proxyLogin = (IProxyLogin)this.IncompleteRequest;
			string @string = Encoding.ASCII.GetString(userBytes);
			char[] trimChars = new char[1];
			string text = @string.Trim(trimChars);
			proxyLogin.AdUser = this.ProtocolUser.FindAdUser(text, null, null);
			if (proxyLogin.AdUser != null)
			{
				commonAccessToken = "SkipAuthenticationOnCafeToken";
				accountValidationContext = null;
				return SecurityStatus.OK;
			}
			this.session.LightLogSession.ErrorMessage = null;
			return proxyLogin.LiveIdBasicAuth.GetCommonAccessToken(userBytes, passBytes, requestId, out commonAccessToken, out accountValidationContext);
		}

		private bool IsSessionValid(out ProtocolResponse response)
		{
			if (this.IsAuthenticated && this.accountValidationContext != null)
			{
				AccountState accountState = this.accountValidationContext.CheckAccount();
				if (accountState != AccountState.AccountEnabled)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Session disconnected because account has been invalidated: " + accountState.ToString());
					response = new ProtocolResponse(string.Format(this.AccountInvalidatedString, accountState));
					response.IsDisconnectResponse = true;
					return false;
				}
			}
			response = null;
			return true;
		}

		private IPEndPoint GetRemoteEndPoint()
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			IPAddress address;
			int port;
			if (IPAddress.TryParse(proxyLogin.ClientIp, out address) && int.TryParse(proxyLogin.ClientPort, out port))
			{
				return new IPEndPoint(address, port);
			}
			if (string.IsNullOrEmpty(proxyLogin.ClientIp) && string.IsNullOrEmpty(proxyLogin.ClientPort))
			{
				return this.Session.Connection.RemoteEndPoint;
			}
			this.Session.LogInformation(string.Format("Error parsing FE->BE IP:Port ({0}:{1})", (proxyLogin.ClientIp == null) ? "<null>" : proxyLogin.ClientIp, (proxyLogin.ClientPort == null) ? "<null>" : proxyLogin.ClientPort), new object[0]);
			return this.Session.Connection.RemoteEndPoint;
		}

		private bool BlockedByClientAccessRules(ADUser adUser)
		{
			if (ResponseFactory.GetClientAccessRulesEnabled() && ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox)
			{
				IProxyLogin proxyLoginRequest = this.incompleteRequest as IProxyLogin;
				return ClientAccessRulesUtils.ShouldBlockConnection(adUser.OrganizationId, ClientAccessRulesUtils.GetUsernameFromIdInformation(adUser.WindowsLiveID, adUser.MasterAccountSid, adUser.Sid, adUser.Id), ResponseFactory.ClientAccessRulesProtocol, this.GetRemoteEndPoint(), ClientAccessAuthenticationMethod.BasicAuthentication, adUser, delegate(ClientAccessRulesEvaluationContext context)
				{
					this.Session.LogInformation(string.Format("{0}={1}", ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name), new object[0]);
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.Message = string.Format("{0}={1}", ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
						this.Session.LightLogSession.ErrorMessage = ClientAccessRulesConstants.ClientAccessRulesLogonFailed;
					}
					proxyLoginRequest.AuthenticationError = ClientAccessRulesConstants.ClientAccessRulesLogonFailed;
				}, delegate(double latency)
				{
					if (latency > 50.0)
					{
						this.Session.LogInformation(string.Format("{0}={1}", ClientAccessRulesConstants.ClientAccessRulesLatency, latency), new object[0]);
						if (this.Session.LightLogSession != null)
						{
							this.Session.LightLogSession.Message = string.Format("{0}={1}", ClientAccessRulesConstants.ClientAccessRulesLatency, latency);
						}
					}
				});
			}
			return false;
		}

		private void ConfigureResourceKeys()
		{
			this.ResourceKeys = new ResourceKey[]
			{
				ProcessorResourceKey.Local,
				new MdbResourceHealthMonitorKey(this.Store.MailboxOwner.MailboxInfo.GetDatabaseGuid()),
				new MdbReplicationResourceHealthMonitorKey(this.Store.MailboxOwner.MailboxInfo.GetDatabaseGuid())
			};
		}

		private bool IsWellKnownAccount(AuthenticationContext serverContext)
		{
			if (serverContext.IsWellKnownAdministrator || serverContext.IsGuest || serverContext.IsAnonymous)
			{
				string name = serverContext.Identity.Name;
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.session.SessionId, "User {0} is not enabled for protocol access", name);
				return true;
			}
			return false;
		}

		private bool Logon(SecureString password, out ClientSecurityContext clientSecurityContext)
		{
			clientSecurityContext = null;
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			int num = this.UserName.IndexOf('/');
			int num2 = (num < this.UserName.Length - 1) ? this.UserName.IndexOf('/', num + 1) : -1;
			int num3 = this.UserName.IndexOf('@');
			if (num3 > 0)
			{
				if (num < num3 && num3 < num2)
				{
					this.SetSessionError("LogonFailed:UnsupportedLoginFormat1");
					return false;
				}
				if (num > num3)
				{
					num2 = num;
					num = 0;
				}
			}
			if (num2 == -1)
			{
				num2 = this.UserName.Length;
			}
			string domain;
			string text;
			if (num > 0)
			{
				domain = this.UserName.Substring(0, num);
				text = this.UserName.Substring(num + 1, num2 - num - 1);
			}
			else
			{
				domain = null;
				text = this.UserName.Substring(0, num2);
			}
			if (num2 < this.UserName.Length)
			{
				this.Mailbox = this.UserName.Substring(num2 + 1);
				if (string.IsNullOrEmpty(this.Mailbox))
				{
					this.SetSessionError("LogonFailed:UnsupportedLoginFormat2");
					return false;
				}
				proxyLogin.UserName = this.UserName.Remove(num2);
			}
			if (string.IsNullOrEmpty(text))
			{
				this.SetSessionError("LogonFailed:EmptyUserName");
				return false;
			}
			AuthenticationContext authenticationContext = null;
			AuthenticationContext authenticationContext2 = null;
			bool result;
			try
			{
				authenticationContext = new AuthenticationContext();
				authenticationContext2 = this.CreateServerAuthenticationContext();
				byte[] inputBuffer = null;
				SecurityStatus securityStatus = authenticationContext2.InitializeForInboundNegotiate(AuthenticationMechanism.Login);
				if (securityStatus != SecurityStatus.OK)
				{
					this.SetSessionError("LogonFailed:FailInitializeForInboundNegotiate");
					result = false;
				}
				else
				{
					SecurityStatus securityStatus2 = authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Login, null, text, domain, password);
					if (securityStatus2 != SecurityStatus.OK)
					{
						this.SetSessionError("LogonFailed:FailInitializeForOutboundNegotiate");
						result = false;
					}
					else
					{
						do
						{
							securityStatus2 = authenticationContext.NegotiateSecurityContext(inputBuffer, out inputBuffer);
							if (securityStatus2 != SecurityStatus.ContinueNeeded && securityStatus2 != SecurityStatus.OK)
							{
								break;
							}
							securityStatus = authenticationContext2.NegotiateSecurityContext(inputBuffer, out inputBuffer);
						}
						while ((securityStatus == SecurityStatus.ContinueNeeded || securityStatus == SecurityStatus.OK) && securityStatus2 == SecurityStatus.ContinueNeeded && securityStatus == SecurityStatus.ContinueNeeded);
						if (securityStatus2 == SecurityStatus.OK && securityStatus == SecurityStatus.OK && authenticationContext2.IsAuthenticated)
						{
							if (this.IsWellKnownAccount(authenticationContext2))
							{
								this.SetSessionError("LogonFailed:WellKnownAccount");
								result = false;
							}
							else
							{
								clientSecurityContext = this.GetClientSecurityContext(authenticationContext2);
								result = true;
							}
						}
						else
						{
							string text2 = securityStatus.ToString();
							string text3 = text2;
							string text4 = string.Empty;
							if (proxyLogin.LiveIdBasicAuth != null)
							{
								text4 = proxyLogin.LiveIdBasicAuth.LastAuthResult.ToString();
								if (proxyLogin.LiveIdBasicAuth.LastAuthResult != LiveIdAuthResult.Success)
								{
									text2 = text2 + "-" + text4;
								}
								if (!string.IsNullOrEmpty(proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage))
								{
									text3 = text3 + "-" + proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage.Replace('"', '\'').Replace("\r\n", " ");
								}
							}
							this.Session.LogInformation("User not found or invalid password. {0}", new object[]
							{
								text2
							});
							if (this.Session.LightLogSession != null)
							{
								this.Session.LightLogSession.ErrorMessage = "LogonFailed:" + text2;
								this.Session.LightLogSession.LiveIdAuthResult = text4.ToString();
							}
							this.SetSessionError("LogonFailed:" + text3);
							result = false;
						}
					}
				}
			}
			finally
			{
				if (authenticationContext != null)
				{
					authenticationContext.Dispose();
				}
				if (authenticationContext2 != null)
				{
					authenticationContext2.Dispose();
				}
			}
			return result;
		}

		private MiniRecipient FindPrincipalMailbox()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.protocolUser.OrganizationId), 3601, "FindPrincipalMailbox", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
			return this.FindPrincipalMailbox(tenantOrRootOrgRecipientSession);
		}

		private StorageMiniRecipient FindPrincipalMailbox(IRecipientSession session)
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			if (string.IsNullOrEmpty(this.Mailbox))
			{
				throw new ApplicationException("This call is only valid in delegate access scenario!");
			}
			QueryFilter filter;
			if (this.Mailbox.IndexOf('@') == -1)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.Alias, this.Mailbox);
			}
			else
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, this.Mailbox);
			}
			ADPagedReader<StorageMiniRecipient> source = session.FindPagedMiniRecipient<StorageMiniRecipient>(null, QueryScope.SubTree, filter, null, 2, null);
			StorageMiniRecipient[] array = source.ToArray<StorageMiniRecipient>();
			if (array == null || array.Length == 0)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ErrorMessage = "NoADRecipient";
				}
				proxyLogin.AuthenticationError = "NoADRecipient";
				this.Session.LogInformation("ADRecipient {0} not found.", new object[]
				{
					this.Mailbox
				});
				return null;
			}
			if (array.Length > 1)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ErrorMessage = "TooManyADRecipients";
				}
				proxyLogin.AuthenticationError = "TooManyADRecipients";
				this.Session.LogInformation("Found too many recipients.", new object[0]);
				return null;
			}
			return array[0];
		}

		private ExchangePrincipal FindExchangePrincipal()
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			ExchangePrincipal exchangePrincipal = null;
			try
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.protocolUser.OrganizationId), 3684, "FindExchangePrincipal", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs");
				if (string.IsNullOrEmpty(this.Mailbox))
				{
					try
					{
						exchangePrincipal = ExchangePrincipal.FromUserSid(tenantOrRootOrgRecipientSession, this.userSid);
						goto IL_12D;
					}
					catch (ObjectNotFoundException)
					{
						if (this.Session.LightLogSession != null)
						{
							this.Session.LightLogSession.ErrorMessage = "NoExchangePrincipal";
						}
						proxyLogin.AuthenticationError = "NoExchangePrincipal";
						this.Session.LogInformation("ExchangePrincipal for {0} not found.", new object[]
						{
							this.protocolUser.LegacyDistinguishedName
						});
						return null;
					}
				}
				StorageMiniRecipient storageMiniRecipient = this.FindPrincipalMailbox(tenantOrRootOrgRecipientSession);
				if (storageMiniRecipient == null)
				{
					return null;
				}
				try
				{
					exchangePrincipal = ExchangePrincipal.FromMiniRecipient(storageMiniRecipient);
				}
				catch (ObjectNotFoundException)
				{
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.ErrorMessage = "NoExchangePrincipal2";
					}
					proxyLogin.AuthenticationError = "NoExchangePrincipal2";
					this.Session.LogInformation("ExchangePrincipal for mailbox {0} not found.", new object[]
					{
						this.Mailbox
					});
					return null;
				}
				IL_12D:;
			}
			catch (SystemException exception)
			{
				this.LogHandledException(exception);
				return null;
			}
			if (exchangePrincipal == null)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ErrorMessage = "NoExchangePrincipal3";
				}
				proxyLogin.AuthenticationError = "NoExchangePrincipal3";
				this.Session.LogInformation("Unable to find ExchangePrincipal", new object[0]);
				return null;
			}
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.Message = string.Format("User:{0}", exchangePrincipal.ToString());
			}
			this.Session.LogInformation("Mailbox: User \"{0}\" Server name \"{1}\", {2}, legacyId \"{3}\"", new object[]
			{
				exchangePrincipal.MailboxInfo.DisplayName,
				exchangePrincipal.MailboxInfo.Location.ServerFqdn,
				new ServerVersion(exchangePrincipal.MailboxInfo.Location.ServerVersion),
				exchangePrincipal.LegacyDn
			});
			return exchangePrincipal;
		}

		private bool OpenMailboxSession(ClientSecurityContext clientSecurityContext, ADUser adUser)
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			if (clientSecurityContext == null)
			{
				ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "Empty clientSecurityContext.");
				return false;
			}
			ExchangePrincipal exchangePrincipal = this.FindExchangePrincipal();
			if (exchangePrincipal == null)
			{
				ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "ExchangePrincipal not found.");
				return false;
			}
			if (this.Session.LightLogSession != null && ProtocolBaseServices.IsMultiTenancyEnabled)
			{
				this.Session.LightLogSession.OrganizationId = proxyLogin.AdUser.OrganizationId.ToString();
			}
			this.FindOwaServer(exchangePrincipal);
			this.store = MailboxSession.OpenWithBestAccess(exchangePrincipal, adUser, clientSecurityContext, CultureInfo.InvariantCulture, this.ClientStringForMailboxSession);
			if (!this.store.CanActAsOwner)
			{
				this.SetSessionError("NoPermissions");
				this.store.Dispose();
				this.store = null;
				return false;
			}
			this.Session.SetMailboxLogTimeout(this.protocolUser.MailboxLogTimeout);
			this.store.SetClientIPEndpoints(this.Session.Connection.RemoteEndPoint.Address, this.Session.Connection.LocalEndPoint.Address);
			this.store.AccountingObject = this.Session.Budget;
			this.store.ExTimeZone = (TimeZoneHelper.GetUserTimeZone(this.store) ?? ResponseFactory.CurrentExTimeZone);
			this.Session.Connection.Timeout = this.Session.Server.ConnectionTimeout;
			this.ConfigureResourceKeys();
			if (this.ProtocolUser.LrsEnabled)
			{
				this.Session.LrsSession = ProtocolBaseServices.LrsLog.OpenSession(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.Session.Connection.RemoteEndPoint, this.Session.Connection.LocalEndPoint);
			}
			this.DoPostLoginTasks();
			this.DisconnectFromTheStore();
			return true;
		}

		private bool ProxyConnect(ADUser adUser)
		{
			if (this.ActualCafeAuthDone)
			{
				this.catToken = this.GetCommonAccessToken(adUser);
			}
			BackEndServer backEndServer;
			if (string.IsNullOrEmpty(this.Mailbox))
			{
				backEndServer = BackEndLocator.GetBackEndServer(adUser);
			}
			else
			{
				MiniRecipient miniRecipient = this.FindPrincipalMailbox();
				if (miniRecipient == null)
				{
					ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "Cross forest Cafe to Brick proxy principal mailbox could not be found.");
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.ErrorMessage = "NoCrossForestPrincipalMailbox";
					}
					return false;
				}
				backEndServer = BackEndLocator.GetBackEndServer(miniRecipient);
			}
			if (backEndServer != null)
			{
				this.session.SetDiagnosticValue(ConditionalHandlerSchema.MailboxServerVersion, backEndServer.Version);
				this.session.SetDiagnosticValue(ConditionalHandlerSchema.MailboxServer, backEndServer.Fqdn);
				int num = 0;
				if (backEndServer.Version >= Server.E15MinVersion)
				{
					num = 15;
				}
				else if (backEndServer.Version < Server.E15MinVersion && backEndServer.Version >= Server.E14MinVersion)
				{
					num = 14;
				}
				else if (backEndServer.Version < Server.E14MinVersion && backEndServer.Version >= Server.E2007MinVersion)
				{
					num = 12;
				}
				ProtocolBaseServices.SessionTracer.TraceDebug<string, int, int>(this.session.SessionId, "Proxy to external {0}, version {1}({2})", backEndServer.Fqdn, num, backEndServer.Version);
				switch (num)
				{
				case 12:
				case 14:
					return this.ProxyConnectToLegacyServer(num);
				case 15:
					return this.ProxyConnectTo15(backEndServer);
				}
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ErrorMessage = "UnsuportedBackendVersion" + backEndServer.Version;
				}
				return true;
			}
			if (this.Session.Server.IsPartnerHostedOnly && this.Session.Server.ExternalProxySettings != null)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Proxy to external proxy URL");
				this.Session.ProxyToLegacyServer = true;
				return this.ProxyConnect(this.Session.Server.ExternalProxySettings.Hostname.HostnameString, this.Session.Server.ExternalProxySettings.Port, this.Session.Server.ExternalProxySettings.EncryptionType);
			}
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ErrorMessage = "NoBackendFound";
			}
			return false;
		}

		private bool ProxyConnectTo15(BackEndServer backEndServer)
		{
			ProtocolBaseServices.SessionTracer.TraceDebug(this.session.SessionId, "Proxy to E15 MBX using BackEndLocator");
			string localForestFqdn = TopologyProvider.LocalForestFqdn;
			bool flag = !ProtocolBaseServices.IsMultiTenancyEnabled || backEndServer.Fqdn.EndsWith(localForestFqdn, StringComparison.OrdinalIgnoreCase);
			string fqdn = backEndServer.Fqdn;
			EncryptionType value = EncryptionType.SSL;
			int e15MbxProxyPort = this.GetE15MbxProxyPort(fqdn, !flag, this.ProtocolUser.AcceptedDomain);
			return this.ProxyConnect(fqdn, e15MbxProxyPort, new EncryptionType?(value));
		}

		private bool ProxyConnectToLegacyServer(int serverVersionMajor)
		{
			ProtocolBaseServices.SessionTracer.TraceDebug<int>(this.session.SessionId, "Proxy to E{0} CAS using Service Discovery", serverVersionMajor);
			Dictionary<int, int> dictionary = new Dictionary<int, int>
			{
				{
					0,
					1
				},
				{
					1,
					2
				},
				{
					-1,
					this.Session.IsTls ? 3 : 0
				}
			};
			int num = 4;
			string proxyHostname = null;
			int proxyPort = 0;
			EncryptionType? encryptionType = null;
			ExchangePrincipal exchangePrincipal = this.FindExchangePrincipal();
			if (exchangePrincipal == null)
			{
				ProtocolBaseServices.SessionTracer.TraceError(this.session.SessionId, "ExchangePrincipal not found.");
				return false;
			}
			foreach (EmailTransportService emailTransportService in this.GetProxyDestinations(exchangePrincipal))
			{
				if (emailTransportService.PopImapTransport)
				{
					if (serverVersionMajor == 14)
					{
						using (IEnumerator<ProtocolConnectionSettings> enumerator2 = emailTransportService.InternalConnectionSettings.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ProtocolConnectionSettings protocolConnectionSettings = enumerator2.Current;
								if (!ResponseFactory.IgnoreNonProvisionedServersEnabled || !emailTransportService.ServerFullyQualifiedDomainName.Equals(protocolConnectionSettings.Hostname.HostnameString, StringComparison.OrdinalIgnoreCase))
								{
									Dictionary<int, int> dictionary2 = dictionary;
									EncryptionType? encryptionType2 = protocolConnectionSettings.EncryptionType;
									if (dictionary2[((encryptionType2 != null) ? new int?((int)encryptionType2.GetValueOrDefault()) : null) ?? -1] < num)
									{
										proxyHostname = protocolConnectionSettings.Hostname.HostnameString;
										proxyPort = protocolConnectionSettings.Port;
										encryptionType = protocolConnectionSettings.EncryptionType;
										Dictionary<int, int> dictionary3 = dictionary;
										EncryptionType? encryptionType3 = protocolConnectionSettings.EncryptionType;
										num = dictionary3[((encryptionType3 != null) ? new int?((int)encryptionType3.GetValueOrDefault()) : null) ?? -1];
									}
								}
							}
							continue;
						}
					}
					if (serverVersionMajor != 12)
					{
						throw new InvalidOperationException("Unsupported server verson " + serverVersionMajor);
					}
					proxyHostname = emailTransportService.ServerFullyQualifiedDomainName;
					if (emailTransportService.SSLPort != -1 && dictionary[0] < num)
					{
						proxyPort = emailTransportService.SSLPort;
						encryptionType = new EncryptionType?(EncryptionType.SSL);
						num = dictionary[0];
					}
					if (emailTransportService.UnencryptedOrTLSPort != -1 && emailTransportService.LoginType > LoginOptions.PlainTextLogin && dictionary[1] < num)
					{
						proxyPort = emailTransportService.UnencryptedOrTLSPort;
						encryptionType = new EncryptionType?(EncryptionType.TLS);
						num = dictionary[1];
					}
					if (emailTransportService.UnencryptedOrTLSPort != -1 && emailTransportService.LoginType == LoginOptions.PlainTextLogin && dictionary[-1] < num)
					{
						proxyPort = emailTransportService.UnencryptedOrTLSPort;
						encryptionType = null;
						num = dictionary[-1];
					}
				}
			}
			return this.ProxyConnect(proxyHostname, proxyPort, encryptionType);
		}

		private bool ProxyConnect(string proxyHostname, int proxyPort, EncryptionType? proxyEncryptionType)
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			proxyLogin.AuthenticationError = string.Empty;
			proxyLogin.ProxyDestination = string.Concat(new object[]
			{
				proxyHostname,
				':',
				proxyPort,
				':',
				(proxyEncryptionType == null) ? "Plaintext" : proxyEncryptionType.ToString()
			});
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ProxyDestination = proxyLogin.ProxyDestination;
				this.Session.LightLogSession.Message = "Proxy:" + this.Session.LightLogSession.ProxyDestination;
			}
			try
			{
				this.Session.LogInformation("Start Proxy to {0}", new object[]
				{
					proxyLogin.ProxyDestination
				});
				if (string.IsNullOrEmpty(proxyHostname))
				{
					proxyLogin.AuthenticationError = "NoProxyServer";
					this.Session.LogInformation("Can't find a server to connect to for {0}", new object[]
					{
						this.protocolUser.UniqueName
					});
					return false;
				}
				if (proxyLogin.Password == null && (!this.ActualCafeAuthDone || this.catToken == null))
				{
					proxyLogin.AuthenticationError = "NoProxyPassword";
					this.Session.LogInformation("Can't make a proxy connection: Not enough info to auth on backend or skip auth.", new object[0]);
					return false;
				}
				this.connectionCreated = new AutoResetEvent(false);
				this.proxyEncryptionType = proxyEncryptionType;
				if (!this.StartProxyConnection(proxyHostname, proxyPort, AddressFamily.InterNetworkV6))
				{
					proxyLogin.AuthenticationError = "NoProxyConnection";
					this.Session.LogInformation("Can't start a proxy connection to {0}:{1}.", new object[]
					{
						proxyHostname,
						proxyPort
					});
					return false;
				}
				this.Session.Connection.Timeout = this.Session.Server.ConnectionTimeout;
				if (!this.connectionCreated.WaitOne(this.session.Server.PreAuthConnectionTimeout * 1000, true))
				{
					if (string.IsNullOrEmpty(proxyLogin.AuthenticationError))
					{
						IProxyLogin proxyLogin2 = proxyLogin;
						proxyLogin2.AuthenticationError += "ProxyTimeout";
					}
					this.Session.LogInformation("Timeout while connection to BE server.", new object[0]);
					return false;
				}
				if (this.session.ProxySession == null)
				{
					if (string.IsNullOrEmpty(proxyLogin.AuthenticationError))
					{
						proxyLogin.AuthenticationError = "ProxyFailed";
					}
					this.Session.LogInformation("Proxy connection failed", new object[0]);
					return false;
				}
				if (!this.session.ProxySession.IsConnected)
				{
					if (string.IsNullOrEmpty(proxyLogin.AuthenticationError))
					{
						proxyLogin.AuthenticationError = "ProxyNotAuthenticated";
					}
					this.session.ProxySession.Shutdown();
					this.session.ProxySession = null;
					this.Session.LogInformation("Unable to connect to BE server", new object[0]);
					return false;
				}
			}
			finally
			{
				if (this.Session.LightLogSession != null)
				{
					if (!string.IsNullOrEmpty(proxyLogin.AuthenticationError))
					{
						this.Session.LightLogSession.ErrorMessage = proxyLogin.AuthenticationError;
						this.Session.LightLogSession.ProxyDestination = null;
					}
					else
					{
						this.Session.LightLogSession.Message = "ProxySuccess";
					}
				}
			}
			return true;
		}

		private bool ConfigureUser(ADUser adUser)
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			ProtocolBaseServices.Assert(this.protocolUser != null, "Derived ResponseFactory.DoConnect should have created a derived ProtocolUser object", new object[0]);
			this.protocolUser.Configure(adUser);
			this.protocolUser.LogonName = this.UserName;
			this.session.EnableUserTracing();
			if (this.accountValidationContext != null)
			{
				this.accountValidationContext.SetOrgId(adUser.OrganizationId);
			}
			ProtocolBaseServices.SessionTracer.TraceDebug<ProtocolUser>(this.session.SessionId, "UserConfiguration {0}", this.protocolUser);
			if (!this.protocolUser.IsEnabled)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ErrorMessage = "UserDisabled";
				}
				proxyLogin.AuthenticationError = "UserDisabled";
				this.Session.LogInformation("User {0} is not enabled for protocol access.", new object[]
				{
					this.UserName
				});
				return false;
			}
			return !this.BlockedByClientAccessRules(adUser);
		}

		private string GetConnectionIdentity()
		{
			if (this.userSid != null)
			{
				return this.userSid.ToString();
			}
			if (!string.IsNullOrEmpty(this.userPuid))
			{
				return this.userPuid;
			}
			return this.UserName;
		}

		private bool CheckConnectionLimit()
		{
			IProxyLogin proxyLogin = this.incompleteRequest as IProxyLogin;
			string connectionIdentity = this.GetConnectionIdentity();
			this.protocolUser.ConnectionIdentity = connectionIdentity;
			int num = -1;
			if (!string.IsNullOrEmpty(connectionIdentity))
			{
				lock (ResponseFactory.connectionsPerUser)
				{
					if (ResponseFactory.connectionsPerUser.Add(connectionIdentity) > this.session.Server.MaxConnectionsPerUser)
					{
						num = this.Session.VirtualServer.DisposeExpiredSessions(connectionIdentity);
						ResponseFactory.connectionsPerUser.Counters[connectionIdentity] = num;
					}
				}
			}
			if (num == -1)
			{
				return true;
			}
			if (num <= this.session.Server.MaxConnectionsPerUser)
			{
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.Message = "DisposeExpiredSessions" + (this.session.Server.MaxConnectionsPerUser - num + 1);
				}
				this.Session.LogInformation("User {0} had {1} expired connections. Sessions cleaned up.", new object[]
				{
					this.UserName,
					this.session.Server.MaxConnectionsPerUser - num + 1
				});
				return true;
			}
			if (this.Session.LightLogSession != null)
			{
				this.Session.LightLogSession.ErrorMessage = "UserConnectionLimitReached";
			}
			proxyLogin.AuthenticationError = "UserConnectionLimitReached";
			ProtocolBaseServices.LogEvent(this.protocolUser.UserExceededNumberOfConnectionsEventTuple, this.UserName, new string[]
			{
				this.UserName,
				this.session.Server.MaxConnectionsPerUser.ToString()
			});
			this.Session.LogInformation("User {0} has more open connections than allowed.", new object[]
			{
				this.UserName
			});
			return false;
		}

		private ProtocolResponse AuthenticationDone(ResponseFactory.AuthenticationResult authenticationResult)
		{
			IProxyLogin proxyLogin = (IProxyLogin)this.IncompleteRequest;
			ProtocolResponse result;
			try
			{
				if (authenticationResult == ResponseFactory.AuthenticationResult.success)
				{
					if (this.IsWellKnownAccount(this.serverContext))
					{
						this.SetSessionError("AuthFailed:WellKnownAccount");
						authenticationResult = ResponseFactory.AuthenticationResult.failure;
					}
					else
					{
						try
						{
							if (this.clientSecurityContext != null)
							{
								((IDisposable)this.clientSecurityContext).Dispose();
								this.clientSecurityContext = null;
							}
							this.clientSecurityContext = this.GetClientSecurityContext(this.serverContext);
							proxyLogin.UserName = this.serverContext.UserName;
							this.Session.LogInformation("After successful ProcessAuth, going to try to connect from {1} with username {0}", new object[]
							{
								proxyLogin.UserName,
								ProtocolBaseServices.ServerRoleService
							});
							if (this.serverContext.AuthorizationIdentity != null)
							{
								this.Mailbox = Encoding.ASCII.GetString(this.serverContext.AuthorizationIdentity);
								if (string.Equals(this.Mailbox, this.UserName, StringComparison.OrdinalIgnoreCase))
								{
									this.Mailbox = null;
								}
								if (!string.IsNullOrEmpty(this.Mailbox) && this.Session.LightLogSession != null)
								{
									this.Session.LightLogSession.Message = string.Format("Auth:User:{0},Mbx:{1}", this.UserName, this.Mailbox);
								}
							}
							if (this.authenticationMechanism == AuthenticationMechanism.Plain)
							{
								proxyLogin.Password = this.serverContext.Password;
							}
							else
							{
								this.ActualCafeAuthDone = true;
							}
							bool flag = false;
							try
							{
								flag = this.TryToConnect(this.clientSecurityContext);
							}
							finally
							{
								ResponseFactory.AuthenticationResult authenticationResult2 = ((ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox || this.ActualCafeAuthDone) && ResponseFactory.CheckOnlyAuthenticationStatusEnabled && !ProtocolBaseServices.AuthErrorReportEnabled(this.UserName)) ? ResponseFactory.AuthenticationResult.authenticatedButFailed : ResponseFactory.AuthenticationResult.failure;
								authenticationResult = (flag ? ResponseFactory.AuthenticationResult.success : authenticationResult2);
							}
						}
						catch (Exception ex)
						{
							if (!this.Session.CheckNonCriticalException(ex))
							{
								throw;
							}
							this.SetSessionError(ex);
							if (!ResponseFactory.CheckOnlyAuthenticationStatusEnabled || ProtocolBaseServices.AuthErrorReportEnabled(this.UserName))
							{
								return this.ProcessException(this.incompleteRequest, ex, this.AuthenticationFailureString);
							}
							authenticationResult = ResponseFactory.AuthenticationResult.authenticatedButFailed;
						}
						finally
						{
							using (this.clientSecurityContext)
							{
							}
						}
					}
				}
				result = this.AuthenticationDone(this.incompleteRequest, authenticationResult);
			}
			finally
			{
				if (this.serverContext != null)
				{
					this.serverContext.Dispose();
					this.serverContext = null;
				}
				this.incompleteRequest = null;
				if (authenticationResult != ResponseFactory.AuthenticationResult.authenticatedAsCafe)
				{
					this.Session.SetMaxCommandLength(this.Session.Server.MaxCommandLength);
				}
			}
			return result;
		}

		private void ProxyConnectionComplete(IAsyncResult iar)
		{
			try
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.Session.SessionId, "User {0} entering ResponseFactory.ProxyConnectionComplete.", this.Session.GetUserNameForLogging());
				ResponseFactory.ProxyConnectioInfo proxyConnectioInfo = (ResponseFactory.ProxyConnectioInfo)iar.AsyncState;
				try
				{
					proxyConnectioInfo.Socket.EndConnect(iar);
				}
				catch (SocketException ex)
				{
					if (proxyConnectioInfo.Socket.AddressFamily == AddressFamily.InterNetworkV6)
					{
						this.Session.LogInformation("ConnectionComplete: try IPv4", new object[0]);
						this.StartProxyConnection(proxyConnectioInfo.Host, proxyConnectioInfo.Port, AddressFamily.InterNetwork);
					}
					else
					{
						if (this.Session.LightLogSession != null)
						{
							this.Session.LightLogSession.ExceptionCaught = ex;
						}
						this.Session.LogInformation("ConnectionComplete: SocketException: {0}", new object[]
						{
							ex
						});
						if (this.connectionCreated != null)
						{
							this.connectionCreated.Set();
						}
					}
					return;
				}
				catch (ArgumentException ex2)
				{
					if (proxyConnectioInfo.Socket.AddressFamily == AddressFamily.InterNetworkV6)
					{
						this.Session.LogInformation("ConnectionComplete: try IPv4", new object[0]);
						this.StartProxyConnection(proxyConnectioInfo.Host, proxyConnectioInfo.Port, AddressFamily.InterNetwork);
					}
					else
					{
						if (this.Session.LightLogSession != null)
						{
							this.Session.LightLogSession.ExceptionCaught = ex2;
						}
						this.Session.LogInformation("ConnectionComplete: ArgumentException: {0}", new object[]
						{
							ex2
						});
						if (this.connectionCreated != null)
						{
							this.connectionCreated.Set();
						}
					}
					return;
				}
				if (this.Session.Disposed)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug(this.Session.SessionId, "Incoming session is disposed, nothing to do.");
					if (this.connectionCreated != null)
					{
						this.connectionCreated.Set();
					}
				}
				else
				{
					ProxySession proxySession = this.NewProxySession(new NetworkConnection(proxyConnectioInfo.Socket, 4096));
					ProtocolBaseServices.SessionTracer.TraceDebug<long, IPEndPoint, IPEndPoint>(this.Session.SessionId, "New proxy Tcp connection {0} opened from {1} to {2}.", proxySession.SessionId, proxySession.RemoteEndPoint, proxySession.LocalEndPoint);
				}
			}
			finally
			{
				ProtocolBaseServices.InMemoryTraceOperationCompleted(this.session.SessionId);
			}
		}

		private bool StartProxyConnection(string host, int port, AddressFamily addressFamily)
		{
			Socket socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
			ResponseFactory.ProxyConnectioInfo proxyConnectioInfo = default(ResponseFactory.ProxyConnectioInfo);
			proxyConnectioInfo.Socket = socket;
			proxyConnectioInfo.Host = host;
			proxyConnectioInfo.Port = port;
			try
			{
				socket.BeginConnect(host, port, new AsyncCallback(this.ProxyConnectionComplete), proxyConnectioInfo);
			}
			catch (SocketException ex)
			{
				if (addressFamily == AddressFamily.InterNetworkV6)
				{
					this.Session.LogInformation("StartProxyConnection: try IPv4", new object[0]);
					return this.StartProxyConnection(host, port, AddressFamily.InterNetwork);
				}
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ExceptionCaught = ex;
				}
				this.Session.LogInformation("StartProxyConnection: SocketException: {0}", new object[]
				{
					ex
				});
				return false;
			}
			catch (ArgumentException ex2)
			{
				if (addressFamily == AddressFamily.InterNetworkV6)
				{
					this.Session.LogInformation("StartProxyConnection: try IPv4", new object[0]);
					return this.StartProxyConnection(host, port, AddressFamily.InterNetwork);
				}
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.ExceptionCaught = ex2;
				}
				this.Session.LogInformation("StartProxyConnection: ArgumentException: {0}", new object[]
				{
					ex2
				});
				return false;
			}
			return true;
		}

		private void FindOwaServer(ExchangePrincipal exchangePrincipal)
		{
			this.owaServer = null;
			if (this.ForceICalForCalendarRetrievalOption)
			{
				return;
			}
			switch (this.session.Server.CalendarItemRetrievalOption)
			{
			case CalendarItemRetrievalOptions.iCalendar:
				return;
			case CalendarItemRetrievalOptions.intranetUrl:
			case CalendarItemRetrievalOptions.InternetUrl:
			{
				ClientAccessType clientAccessType = (this.session.Server.CalendarItemRetrievalOption == CalendarItemRetrievalOptions.InternetUrl) ? ClientAccessType.External : ClientAccessType.Internal;
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs", "FindOwaServer", 4647);
				IList<OwaService> list = currentServiceTopology.FindAll<OwaService>(exchangePrincipal, clientAccessType, "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ResponseFactory.cs", "FindOwaServer", 4649);
				foreach (OwaService owaService in list)
				{
					if (!ResponseFactory.IgnoreNonProvisionedServersEnabled || !owaService.ServerFullyQualifiedDomainName.Equals(owaService.Url.Host, StringComparison.OrdinalIgnoreCase))
					{
						this.owaServer = owaService.Url.ToString();
						break;
					}
				}
				if (this.owaServer == null)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug<ClientAccessType, string>(this.session.SessionId, "Unable to find {0} OWA server url for user {1}.", clientAccessType, exchangePrincipal.MailboxInfo.DisplayName);
					ProtocolBaseServices.LogEvent(this.protocolUser.OwaServerNotFoundEventTuple, exchangePrincipal.MailboxInfo.DisplayName, new string[]
					{
						clientAccessType.ToString(),
						exchangePrincipal.MailboxInfo.DisplayName
					});
				}
				break;
			}
			case CalendarItemRetrievalOptions.Custom:
				this.owaServer = this.session.Server.OwaServer;
				break;
			}
			if (this.Session.Server.LiveIdBasicAuthReplacement && !string.IsNullOrEmpty(this.owaServer))
			{
				this.owaServer = this.owaServer + (this.owaServer.EndsWith("/") ? string.Empty : "/") + this.DefaultAcceptedDomainName + "/";
			}
		}

		private void ResetRecipientCache(object state)
		{
			OutboundConversionOptions outboundConversionOptions = state as OutboundConversionOptions;
			if (outboundConversionOptions != null && this.okToResetRecipientCache)
			{
				lock (this.options)
				{
					this.options.RecipientCache = null;
				}
			}
		}

		private bool TryGetActivityContextStat(ActivityOperationType operationType, out double value)
		{
			value = 0.0;
			if (this.Session.ActivityScope != null)
			{
				IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> statistics = this.Session.ActivityScope.Statistics;
				foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in statistics)
				{
					if (keyValuePair.Key.ActivityOperationType == operationType)
					{
						TotalOperationStatistics totalOperationStatistics = keyValuePair.Value as TotalOperationStatistics;
						if (totalOperationStatistics != null)
						{
							value = totalOperationStatistics.Total;
							return true;
						}
						AverageOperationStatistics averageOperationStatistics = keyValuePair.Value as AverageOperationStatistics;
						if (averageOperationStatistics != null)
						{
							value = (double)averageOperationStatistics.CumulativeAverage;
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private AuthenticationContext CreateServerAuthenticationContext()
		{
			IProxyLogin proxyLogin = (IProxyLogin)this.incompleteRequest;
			if (!this.Session.Server.LiveIdBasicAuthReplacement)
			{
				return new AuthenticationContext(this.Session.Server.ExtendedProtectionConfig, this.Session.Connection.ChannelBindingToken);
			}
			ProtocolBaseServices.FaultInjectionTracer.TraceTest<bool>(3515231549U, ref ResponseFactory.UseClientIpTestMocks);
			if (ResponseFactory.UseClientIpTestMocks)
			{
				proxyLogin.LiveIdBasicAuth = new LiveIdBasicAuthenticationMock(new LiveIdBasicAuthentication());
				ResponseFactory.CheckOnlyAuthenticationStatusEnabled = false;
			}
			else
			{
				proxyLogin.LiveIdBasicAuth = new LiveIdBasicAuthentication();
			}
			proxyLogin.LiveIdBasicAuth.AllowLiveIDOnlyAuth = true;
			proxyLogin.LiveIdBasicAuth.ApplicationName = "Microsoft.Exchange.PopImap";
			if (this.Session.Connection.RemoteEndPoint.Address != null)
			{
				proxyLogin.LiveIdBasicAuth.UserIpAddress = this.Session.Connection.RemoteEndPoint.Address.ToString();
			}
			else
			{
				proxyLogin.LiveIdBasicAuth.UserIpAddress = null;
			}
			if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.cafe && this.SkipAuthOnCafeEnabled)
			{
				return new AuthenticationContext(new ExternalProxyAuthentication(this.SkipAuthenticationOnCafe));
			}
			return new AuthenticationContext(new ExternalProxyAuthentication(proxyLogin.LiveIdBasicAuth.GetCommonAccessToken));
		}

		private string GetAuthenticationFailureString(SecurityStatus status)
		{
			string text = status.ToString();
			IProxyLogin proxyLogin = (IProxyLogin)this.IncompleteRequest;
			if (proxyLogin != null && proxyLogin.LiveIdBasicAuth != null && !string.IsNullOrEmpty(proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage))
			{
				text = text + "-" + proxyLogin.LiveIdBasicAuth.LastRequestErrorMessage.Replace('"', '\'').Replace("\r\n", " ");
			}
			return text;
		}

		private ClientSecurityContext GetClientSecurityContext(AuthenticationContext serverContext)
		{
			if (!this.Session.Server.LiveIdBasicAuthReplacement)
			{
				WindowsIdentity windowsIdentity = serverContext.DetachIdentity();
				this.userSid = windowsIdentity.User;
				return new ClientSecurityContext(windowsIdentity);
			}
			if (serverContext.CommonAccessToken != "SkipAuthenticationOnCafeToken")
			{
				if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox)
				{
					this.accountValidationContext = (AccountValidationContextBase)serverContext.AccountValidationContext;
				}
				return this.GetClientSecurityContext(serverContext.CommonAccessToken);
			}
			return null;
		}

		private ClientSecurityContext GetClientSecurityContext(string catToken)
		{
			CommonAccessToken commonAccessToken = CommonAccessToken.Deserialize(catToken);
			if (commonAccessToken.ExtensionData.ContainsKey("UserSid"))
			{
				this.userSid = new SecurityIdentifier(commonAccessToken.ExtensionData["UserSid"]);
			}
			if (commonAccessToken.ExtensionData.ContainsKey("Puid"))
			{
				this.userPuid = commonAccessToken.ExtensionData["Puid"];
			}
			if (commonAccessToken.ExtensionData.ContainsKey("MemberName"))
			{
				this.UserName = commonAccessToken.ExtensionData["MemberName"];
			}
			BackendAuthenticator backendAuthenticator = null;
			IPrincipal principal = null;
			try
			{
				string text;
				BackendAuthenticator.GetAuthIdentifier(commonAccessToken, ref backendAuthenticator, out text);
				bool wantAuthIdentifier = text != null;
				BackendAuthenticator.Rehydrate(commonAccessToken, ref backendAuthenticator, wantAuthIdentifier, out text, out principal);
			}
			catch (BackendRehydrationException sessionError)
			{
				this.SetSessionError(sessionError);
				return null;
			}
			IIdentity identity = principal.Identity;
			if (identity == null)
			{
				return null;
			}
			return identity.CreateClientSecurityContext(true);
		}

		private string GetCommonAccessToken(ADUser adUser)
		{
			CommonAccessToken commonAccessToken = null;
			if (this.Session.Server.LiveIdBasicAuthReplacement)
			{
				LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Create(adUser);
				if (liveIdBasicTokenAccessor != null)
				{
					commonAccessToken = liveIdBasicTokenAccessor.GetToken();
				}
			}
			else
			{
				WindowsIdentity windowsIdentity = new WindowsIdentity(adUser.UserPrincipalName);
				if (windowsIdentity == null || windowsIdentity.IsAnonymous)
				{
					throw new InvalidOperationException(string.Format("Unable to find windows Identity, {0}.", adUser.PrimarySmtpAddress));
				}
				WindowsTokenAccessor windowsTokenAccessor = WindowsTokenAccessor.Create(windowsIdentity);
				if (windowsTokenAccessor != null)
				{
					commonAccessToken = windowsTokenAccessor.GetToken();
				}
			}
			if (commonAccessToken != null)
			{
				return commonAccessToken.Serialize();
			}
			return null;
		}

		private bool ExtractCafeHostname(byte[] buf, int offset, int size, out int authBlobSize, out string cafeHost)
		{
			authBlobSize = 0;
			cafeHost = null;
			for (int i = 0; i < size; i++)
			{
				if (buf[offset + i] == 0)
				{
					authBlobSize = i;
					cafeHost = Encoding.ASCII.GetString(buf, offset + i + 1, size - i - 1);
					this.Session.LogInformation("Processing Exchange Auth from cafe server {0}", new object[]
					{
						cafeHost
					});
					return true;
				}
			}
			this.Session.LogInformation("Null character not found in Exchange Auth authblob", new object[0]);
			return false;
		}

		private bool ExtractClientIP(byte[] buf, int offset, int size, out int authBlobSize)
		{
			authBlobSize = size;
			IProxyLogin proxyLogin = (IProxyLogin)this.IncompleteRequest;
			bool flag = ResponseFactory.GetClientAccessRulesEnabled() || ProtocolBaseServices.GCCEnabledWithKeys;
			if (flag)
			{
				int num = 0;
				List<string> list = new List<string>();
				for (int i = 0; i < size; i++)
				{
					if (buf[offset + i] == 0)
					{
						int num2 = i + 1;
						if (num == 0)
						{
							num = num2;
							authBlobSize = num - 1;
						}
						else
						{
							int num3 = num;
							num = num2;
							list.Add(Encoding.ASCII.GetString(buf, offset + num3, num - num3 - 1));
						}
					}
				}
				list.Add(Encoding.ASCII.GetString(buf, offset + num, size - num));
				if (list.Count >= 2)
				{
					string authString = list[1];
					if (ProtocolBaseServices.GCCEnabledWithKeys && !GccUtils.IsValidAuthString(authString))
					{
						this.SetSessionError("InvalidServerAuthString");
						return false;
					}
					proxyLogin.ClientIp = list[0];
					this.Session.LogInformation("Setting client IP {0}", new object[]
					{
						proxyLogin.ClientIp
					});
					if (this.Session.LightLogSession != null)
					{
						this.Session.LightLogSession.ClientIp = proxyLogin.ClientIp;
					}
					if (proxyLogin.LiveIdBasicAuth != null)
					{
						proxyLogin.LiveIdBasicAuth.UserIpAddress = proxyLogin.ClientIp;
					}
					if (list.Count >= 3)
					{
						string text = list[2];
						if (this.ActivityId == Guid.Empty && !string.IsNullOrWhiteSpace(text))
						{
							Guid empty = Guid.Empty;
							if (Guid.TryParse(text, out empty))
							{
								this.ActivityId = empty;
							}
							else
							{
								this.Session.LogInformation(string.Format("Ignore invalid CAFE Activity ID ({0})", text), new object[0]);
							}
							if (this.Session.LightLogSession != null)
							{
								this.Session.LightLogSession.CafeActivityId = text;
							}
						}
					}
					proxyLogin.ClientPort = ((list.Count >= 4) ? list[3] : string.Empty);
					proxyLogin.AuthenticationType = ((list.Count >= 5) ? list[4] : string.Empty);
				}
			}
			else
			{
				for (int j = 0; j < size; j++)
				{
					if (buf[offset + j] == 0)
					{
						this.Session.LogInformation("AuthBlob has unexpected extra tokens", new object[0]);
						if (this.Session.LightLogSession != null)
						{
							this.Session.LightLogSession.ErrorMessage = "InvalidAuthBlob";
						}
						proxyLogin.AuthenticationError = "InvalidAuthBlob";
						return false;
					}
				}
			}
			return true;
		}

		private void ReleaseResources()
		{
			ProxySession proxySession = null;
			lock (this)
			{
				string connectionIdentity = this.protocolUser.ConnectionIdentity;
				if (!string.IsNullOrEmpty(connectionIdentity))
				{
					ResponseFactory.connectionsPerUser.Remove(connectionIdentity);
				}
				if (this.store != null)
				{
					try
					{
						((IDisposable)this.store).Dispose();
					}
					catch (LocalizedException ex)
					{
						ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.Session.SessionId, "Exception caught while disposing store. {0}", ex.ToString());
					}
					this.store = null;
				}
				if (this.Session.Budget != null)
				{
					try
					{
						this.Session.Budget.Dispose();
					}
					catch (LocalizedException ex2)
					{
						ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.Session.SessionId, "Exception caught while disposing budget. {0}", ex2.ToString());
					}
					this.Session.Budget = null;
				}
				if (this.Session.ActivityScope != null)
				{
					try
					{
						this.Session.ActivityScope.Dispose();
					}
					catch (LocalizedException ex3)
					{
						ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.Session.SessionId, "Exception caught while disposing ActivityScope. {0}", ex3.ToString());
					}
					this.Session.ActivityScope = null;
				}
				if (this.serverContext != null)
				{
					this.serverContext.Dispose();
					this.serverContext = null;
				}
				if (this.connectionCreated != null)
				{
					this.connectionCreated.Close();
					this.connectionCreated = null;
				}
				if (this.Session.ProxySession != null)
				{
					proxySession = this.Session.ProxySession;
					this.Session.ProxySession = null;
				}
				if (this.Session.LightLogSession != null)
				{
					this.Session.LightLogSession.Budget = null;
					this.Session.LightLogSession.ActivityScope = null;
				}
			}
			if (proxySession != null)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug(this.Session.SessionId, "Calling disposeSession.Dispose()");
				proxySession.Dispose();
			}
		}

		public const int InitialBackOffDelay = 2;

		public const string DefaultUserDomain = "";

		public const int BackOffShift = 2;

		public const int MaximumBackOffDelay = 1024;

		public const int MaxBulkSize = 256;

		public const string ClientAccessRulesCapability = "CLIENTACCESSRULES";

		public const string Xproxy3Capability = "XPROXY3";

		public const string SkipAuthenticationOnCafeToken = "SkipAuthenticationOnCafeToken";

		private const string AssemblyVersion = "15.00.1497.012";

		public static bool UseClientIpTestMocks = false;

		protected static readonly Regex AuthErrorParser = new Regex("\\[Error=\"?(?<authError>[^\"]+)\"?( Proxy=(?<proxy>.+))?\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly FileVersionInfo currentXsoVersion = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(MailboxSession)).Location);

		private static readonly char[] wordDelimiter = new char[]
		{
			' '
		};

		private static readonly TimeSpan DefaultRecipientCacheResetInterval = TimeSpan.FromMinutes(3.0);

		private static readonly TimeSpan DefaultMinPopImapThreshold = TimeSpan.FromMinutes(10.0);

		private static MruDictionaryCache<OrganizationId, string> defaultAcceptedDomainTable = new MruDictionaryCache<OrganizationId, string>(10, 50000, 5);

		private static RefCountTable<string> connectionsPerUser = new RefCountTable<string>();

		private static int[] commandProcessingTimeSamples = new int[1024];

		private static LatencyDetectionContextFactory latencyDetectionContextFactory;

		private static int insertionIndex;

		private static int latencySum;

		private static int numSamples;

		private static object lockObject = new object();

		private static ExTimeZone currentExTimeZone;

		protected bool clientAccessRulesSupportedByTargetServer;

		private ProtocolRequest incompleteRequest;

		private bool disposed;

		private MailboxSession store;

		private ClientSecurityContext clientSecurityContext;

		private OutboundConversionOptions options;

		private InboundConversionOptions inboundOptions;

		private ProtocolSession session;

		private string userName;

		private SecurityIdentifier userSid;

		private string userPuid;

		private string catToken;

		private AutoResetEvent connectionCreated;

		private EncryptionType? proxyEncryptionType;

		private int preAuthCommands;

		private int failedCommands;

		private uint loginAttempts;

		private AuthenticationMechanism authenticationMechanism;

		private uint invalidCommands;

		private AuthenticationContext serverContext;

		private AccountValidationContextBase accountValidationContext;

		private ProtocolUser protocolUser;

		private string owaServer;

		private Timer recipientCacheResetTimer;

		private bool okToResetRecipientCache = true;

		private DisposeTracker disposeTracker;

		private Stopwatch stopwatch;

		private UserConfigurationManager customStorageManager;

		public enum AuthenticationResult
		{
			success,
			failure,
			authenticatedButFailed,
			authenticatedAsCafe,
			cancel
		}

		protected struct ProxyConnectioInfo
		{
			public Socket Socket;

			public string Host;

			public int Port;
		}

		protected class SessionDisconnectedException : LocalizedException
		{
			public SessionDisconnectedException() : base(LocalizedString.Empty)
			{
			}
		}
	}
}
