using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AirSyncHandler
{
	public class Handler : IHttpAsyncHandler, IHttpHandler
	{
		public Handler()
		{
			if (Handler.initialized)
			{
				return;
			}
			lock (Handler.classMutex)
			{
				if (!Handler.initialized)
				{
					GlobalSettings.ForceLoadAllSettings();
					ObjectSchema instance = ObjectSchema.GetInstance<AirSyncConditionalHandlerSchema>();
					BaseConditionalRegistration.Initialize("ActiveSync", instance, instance, new Dictionary<string, string>
					{
						{
							"device",
							"DeviceID,DeviceType,ProtocolVersion"
						},
						{
							"default",
							"SmtpAddress,TenantName,DisplayName,MailboxServer,DeviceID,Cmd,ElapsedTime,HttpStatus,EASStatus,Exception"
						}
					});
					if (GlobalSettings.FullServerVersion)
					{
						Handler.serverVersion = "15.00.1497.012";
					}
					else
					{
						Handler.serverVersion = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
						{
							15,
							0
						});
					}
					Handler.ResetAllPerfCounters();
					int id;
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						id = currentProcess.Id;
					}
					AirSyncCounters.PID.RawValue = (long)id;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.ThreadPoolTracer, this, "Starting Handler() initialization");
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						Handler.runningAsLocalSystem = current.User.IsWellKnown(WellKnownSidType.LocalSystemSid);
					}
					ThrottlingPerfCounterWrapper.Initialize(BudgetType.Eas);
					ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.EAS);
					int maxThreadCount = GlobalSettings.MaxWorkerThreadsPerProc * Environment.ProcessorCount;
					UserWorkloadManager.Initialize(maxThreadCount, GlobalSettings.MaxRequestsQueued, GlobalSettings.MaxRequestsQueued, TimeSpan.FromDays(1.0), null);
					ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents();
					ADUserCache.Start();
					MailboxSessionCache.Start();
					DeviceBehaviorCache.Start();
					DeviceClassCache.Instance.Start();
					PingCommand.PingHbiMonitor.Instance.Initialize(GlobalSettings.HeartbeatSampleSize, GlobalSettings.HeartbeatAlertThreshold);
					SyncCommand.SyncHbiMonitor.Instance.Initialize(GlobalSettings.HeartbeatSampleSize, GlobalSettings.HeartbeatAlertThreshold);
					if (!RatePerfCounters.Initialize(Constants.ExceptionPerfCounters, Constants.LatencyPerfCounters))
					{
						AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoPerfCounterTimer, new string[0]);
					}
					Handler.initialized = true;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.ThreadPoolTracer, this, "Handler() initialization is complete");
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_AirSyncLoaded, new string[]
					{
						id.ToString(CultureInfo.InvariantCulture)
					});
				}
			}
		}

		public static string ServerVersion
		{
			get
			{
				return Handler.serverVersion;
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback asyncCallback, object extraData)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			TimeTracker timeTracker = new TimeTracker();
			IAsyncResult asyncResult;
			using (timeTracker.Start(TimeId.HandlerBeginProcessRequest))
			{
				this.result = new LazyAsyncResult(this, extraData, asyncCallback);
				IAirSyncUser airSyncUser = null;
				WindowsImpersonationContext windowsImpersonationContext = null;
				Guid relatedActivityId = Guid.Empty;
				try
				{
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						if (current.ImpersonationLevel != TokenImpersonationLevel.None)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "BeginProcessRequest: de-impersonate is called.");
							windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
						}
					}
					try
					{
						this.watch = Stopwatch.StartNew();
						this.context = new AirSyncContext(httpContext);
						this.context.Tracker = timeTracker;
						this.context.ActivityScope = ActivityContext.DeserializeFrom(httpContext.Request, null);
						relatedActivityId = this.context.ActivityScope.ActivityId;
						using (ExPerfTrace.RelatedActivity(relatedActivityId))
						{
							if (this.context.Request.WasProxied)
							{
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Received proxied request from user {0}.", this.context.Request.ProxyHeader);
							}
							if (this.context.Request.WasBasicAuthProxied)
							{
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Received Basic auth proxied request from user {0}.", this.GetLogonUserName(httpContext));
							}
							else
							{
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Received request from user {0}.", this.GetLogonUserName(httpContext));
							}
							string hostHeaderInfo = this.context.Request.HostHeaderInfo;
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Host, hostHeaderInfo);
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeReceived, utcNow);
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ProtocolVersion, this.context.Request.Version);
							AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Creating Result object", 23595);
							AirSyncCounters.NumberOfRequests.Increment();
							AirSyncCounters.CurrentNumberOfRequests.Increment();
							this.requestStartTime = utcNow;
							this.TraceRequestInfo();
							if (!Handler.runningAsLocalSystem)
							{
								AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_RunAsLocalSystem, null, new string[0]);
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NotRunningAsLocalSystem");
								this.context.Response.IssueErrorResponse(HttpStatusCode.InternalServerError, StatusCode.ServerError);
								this.CompleteRequestWithDelay(this.context.Response);
								return this.result;
							}
							if (!ADNotificationManager.Started)
							{
								ADNotificationManager.Start(this.context);
							}
							if (this.context.Request.Version == 10)
							{
								return this.DeprecateVersion10();
							}
							if (this.context.Request.WasProxied || this.context.Request.WasBasicAuthProxied)
							{
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Request was proxied from host {0}.", this.context.Request.UserHostName);
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ProxyingFrom, this.context.Request.UserHostName);
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ProtocolVersion, this.context.Request.Version);
								AirSyncCounters.NumberOfIncomingProxyRequests.Increment();
							}
							if (this.context.Request.CommandType == CommandType.ProxyLogin)
							{
								this.context.Response.HttpStatusCode = this.HandleProxyLoginCommand();
								this.result.InvokeCallback();
								return this.result;
							}
							airSyncUser = new AirSyncUser(this.context);
							this.context.User = airSyncUser;
							if (this.context.Request.Version == 160 && !GlobalSettings.EnableV160 && !this.context.User.Features.IsEnabled(EasFeature.EnableV160))
							{
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BlockV160");
								this.context.Response.IssueErrorResponse(HttpStatusCode.HttpVersionNotSupported, StatusCode.None);
							}
							if (airSyncUser.ExchangePrincipal != null)
							{
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.UserSmtpAddress, airSyncUser.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
							}
							if (GlobalSettings.IsWindowsLiveIDEnabled)
							{
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.OrganizationId, airSyncUser.OrganizationId.ToString());
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.PUID, airSyncUser.ADUser.NetID.ToString());
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.OrganizationType, airSyncUser.IsConsumerOrganizationUser ? "1" : "0");
							}
							if (airSyncUser.ADUser == null || airSyncUser.ExchangePrincipal == null)
							{
								if (GlobalSettings.IsPartnerHostedOnly && !string.IsNullOrEmpty(GlobalSettings.ExternalProxy))
								{
									ProxyHandler.ProxiedRequestInfo proxyinfo = new ProxyHandler.ProxiedRequestInfo(this.context.User.Name, new Uri(httpContext.Request.Url.Scheme + "://" + GlobalSettings.ExternalProxy + GlobalSettings.ProxyVirtualDirectory));
									return this.ProxyRequest(httpContext, GlobalSettings.ExternalProxy, proxyinfo);
								}
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserPrincipalCouldNotBeFound");
								this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserPrincipalCouldNotBeFound);
								this.CompleteRequestWithDelay(this.context.Response);
								return this.result;
							}
							else
							{
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.MailboxServer, airSyncUser.ExchangePrincipal.MailboxInfo.Location.ServerFqdn);
								this.perUserTracingEnabled = AirSyncDiagnostics.CheckAndSetThreadTracing(airSyncUser.ExchangePrincipal.LegacyDn);
								AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Checking user access from using AirSync", 31787);
								if (!airSyncUser.IsEnabled || Handler.IsWellKnownAccount(airSyncUser.Identity))
								{
									AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "User has been disabled from using AirSync");
									AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UserHasBeenDisabled, "UserHasBeenDisabled: " + airSyncUser.Name, new string[]
									{
										airSyncUser.Name
									});
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserDisabledForSync");
									this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserDisabledForSync);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								if (this.BlockedByClientAccessRules(airSyncUser, httpContext))
								{
									AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "User has been disabled by Client Access Rules");
									if (GlobalSettings.ClientAccessRulesLogPeriodicEvent)
									{
										AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UserHasBeenBlocked, "UserHasBeenBlocked: " + airSyncUser.Name, new string[]
										{
											airSyncUser.Name
										});
									}
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserDisabledByClientAccessRules");
									this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserDisabledForSync);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Reading mailbox server name and version for user {0}", airSyncUser.Name);
								AirSyncDiagnostics.TracePfd<int, string>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Reading mailbox server name and version for user {1}", 17451, airSyncUser.Name);
								string serverFqdn = airSyncUser.ExchangePrincipal.MailboxInfo.Location.ServerFqdn;
								bool flag = !airSyncUser.ExchangePrincipal.MailboxInfo.Location.IsLegacyServer();
								int num = airSyncUser.ExchangePrincipal.MailboxInfo.Location.ServerVersion;
								if (string.IsNullOrEmpty(serverFqdn))
								{
									AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "No mailbox server for user {0} could be found.", airSyncUser.Name);
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoMailboxServer");
									this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserHasNoMailbox);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								airSyncUser.MailboxIsOnE12Server = flag;
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "User's mailbox server is {0}", serverFqdn);
								AirSyncDiagnostics.TracePfd<int, string>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Checking to see if user is blocked from accessing mailbox server {1}", 25643, serverFqdn);
								if (num < Server.E2k3MinVersion)
								{
									AirSyncDiagnostics.TraceError<string, int>(ExTraceGlobals.RequestsTracer, this, "User blocked from from accessing mailbox {0}, version of server is {1}, minimum version supported is 6.5", serverFqdn, num);
									AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_BadProxyServerVersion, "BadProxyServerVersion: " + serverFqdn + airSyncUser.Name, new string[]
									{
										serverFqdn,
										airSyncUser.Name,
										num.ToString(CultureInfo.InvariantCulture)
									});
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadProxyServerVersion");
									this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.ServerError);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "User's mailbox server version is {0}.", num);
								if ((GlobalSettings.BlockLegacyMailboxes && !flag) || (GlobalSettings.BlockNewMailboxes && flag) || serverFqdn == null)
								{
									AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "GlobalSettings has blocked user from from accessing mailbox {0}", serverFqdn);
									if (flag)
									{
										AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UserOnNewMailboxCannotSync, "UserOnNewMailboxCannotSync: " + serverFqdn + airSyncUser.Name, new string[]
										{
											airSyncUser.Name,
											serverFqdn
										});
										this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserOnNewMailboxCannotSync");
										this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserOnNewMailboxCannotSync);
									}
									else
									{
										AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UserOnLegacyMailboxCannotSync, "UserOnLegacyMailboxCannotSync: " + serverFqdn + airSyncUser.Name, new string[]
										{
											airSyncUser.Name,
											serverFqdn
										});
										this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserOnLegacyMailboxCannotSync");
										this.context.Response.IssueErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserOnLegacyMailboxCannotSync);
									}
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Checking to see if user is on a legacy back-end server", 21547);
								if (!flag)
								{
									bool flag2 = true;
									try
									{
										flag2 = this.LegacyUserAllowedToSync(false, serverFqdn, airSyncUser.ADUser.PrimarySmtpAddress.ToString(), airSyncUser.ADUser.PrimarySmtpAddress.Domain, false);
									}
									catch (WebException)
									{
									}
									if (!flag2)
									{
										AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Stamped property on mailbox has blocked user from from accessing mailbox {0}", serverFqdn);
										AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ClientDisabledFromSyncEvent, airSyncUser.Name + " lock", new string[]
										{
											airSyncUser.Name
										});
										this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "TiSyncStateLocked");
										this.context.Response.IssueErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.SyncStateLocked);
										this.CompleteRequestWithDelay(this.context.Response);
										return this.result;
									}
								}
								ProxyHandler.ProxiedRequestInfo proxiedRequestInfo = null;
								if (this.RequestMustBeProxied(serverFqdn, out proxiedRequestInfo))
								{
									this.context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.ProxyToServer, proxiedRequestInfo.RemoteUri.ToString());
									return this.ProxyRequest(httpContext, serverFqdn, proxiedRequestInfo);
								}
								if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.ForceSingleNameSpaceUsage.Enabled && !string.Equals(hostHeaderInfo, "localhost", StringComparison.InvariantCultureIgnoreCase) && DeviceCapability.DeviceCanHandleRedirect(this.context))
								{
									Uri uri = null;
									try
									{
										uri = FrontEndLocator.GetFrontEndEasUrl(airSyncUser.ExchangePrincipal);
									}
									catch (ServerNotFoundException ex)
									{
										AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "No FrontEnd Url found for user {0}. exception:{1}", airSyncUser.Name, ex.ToString());
										uri = null;
									}
									if (uri != null)
									{
										if (!string.Equals(this.context.Request.HostHeaderInfo, uri.Host, StringComparison.InvariantCultureIgnoreCase) && GlobalSettings.ValidSingleNamespaceUrls.Contains(uri.Host.ToLower()))
										{
											this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MisconfiguredDeviceSNS");
											this.context.ProtocolLogger.SetValue(ProtocolLoggerData.RedirectTo, uri.ToString());
											AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "User is not using SNS url. Issue a 451 to user {0}, url :{1}.", airSyncUser.Name, uri.ToString());
											throw new IncorrectUrlRequestException((HttpStatusCode)451, "X-MS-Location", uri.ToString());
										}
									}
									else
									{
										this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoCafeServiceSkipRDirToSNS");
										AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "No external services found so skip issuing 451 response.");
									}
								}
								this.context.Request.ParseAndValidateHeaders();
								this.cmd = this.CreateCommandObject();
								if (this.cmd == null)
								{
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CommandNotSupported");
									this.context.Response.IssueErrorResponse(HttpStatusCode.NotImplemented, StatusCode.CommandNotSupported);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								this.cmd.Context = this.context;
								this.cmd.Context.Response.AppendHeader("MS-Server-ActiveSync", Handler.ServerVersion);
								this.cmd.LazyAsyncResult = this.result;
								this.cmd.PerUserTracingEnabled = this.perUserTracingEnabled;
								if (this.context.Request.Version < this.cmd.MinVersion || this.context.Request.Version > this.cmd.MaxVersion)
								{
									AirSyncDiagnostics.TraceError<int>(ExTraceGlobals.RequestsTracer, this, "Protocol version {0} is not supported by the command!", this.context.Request.Version);
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "VersionNotSupported");
									this.context.Response.IssueErrorResponse(HttpStatusCode.BadRequest, StatusCode.VersionNotSupported);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "Using protocol version {0}.", this.context.Request.Version);
								if (this.context.Request.ContentLength != 0 && this.context.Request.ContentType == null)
								{
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MissingContentType");
									this.context.Response.IssueErrorResponse(HttpStatusCode.BadRequest, StatusCode.First140Error);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								DeviceBehavior deviceBehavior;
								TimeSpan t;
								if (this.context.Request.CommandType != CommandType.Options && DeviceBehaviorCache.TryGetValue(airSyncUser.ADUser.OriginalId.ObjectGuid, this.context.Request.DeviceIdentity, out deviceBehavior) && deviceBehavior.IsDeviceAutoBlocked(null, out t) == DeviceAccessStateReason.OutOfBudgets)
								{
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceIsAutoBlockedOnOOB");
									this.context.Response.IssueErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.None);
									IAsyncCommand asyncCommand = this as IAsyncCommand;
									uint heartbeatInterval;
									if (asyncCommand != null && (ulong)(heartbeatInterval = asyncCommand.GetHeartbeatInterval()) > (ulong)((long)GlobalSettings.ErrorResponseDelay))
									{
										this.context.Response.TimeToRespond = this.context.RequestTime.AddSeconds(heartbeatInterval);
									}
									this.context.Response.AppendHeader("X-MS-ASThrottle", DeviceAccessStateReason.OutOfBudgets.ToString());
									if (t > TimeSpan.Zero)
									{
										this.context.Response.AppendHeader("Retry-After", t.TotalSeconds.ToString(), false);
									}
									this.context.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.AccessStateAndReason, "BlockedABO");
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								int num2 = 0;
								AirSyncDiagnostics.FaultInjectionTracer.TraceTest<int>(3003526461U, ref num2);
								if (num2 != 0)
								{
									AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "Fault injection value for 3003526461 is {0}", num2);
									if (num2 != 441 && num2 != 449 && num2 != 451)
									{
										throw new ApplicationException("Unsupported injected error code " + num2);
									}
									this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FaultInjectedError");
									this.context.Response.IssueErrorResponse((HttpStatusCode)num2, StatusCode.None);
									this.CompleteRequestWithDelay(this.context.Response);
									return this.result;
								}
								else
								{
									AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Handing the request to the Command-derived class", 26667);
									AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Handing the request to the Command-derived class");
								}
							}
						}
					}
					finally
					{
						ActivityContext.ClearThreadScope();
					}
					if (!this.cmd.ScheduleTask())
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "this.cmd.ScheduleTask() returns false!");
						return this.result;
					}
				}
				catch (Exception ex2)
				{
					using (ExPerfTrace.RelatedActivity(relatedActivityId))
					{
						AirSyncUtility.ProcessException(ex2, (this.cmd != null) ? this.cmd : this, this.context);
						if (this.context.Response.XmlDocument != null)
						{
							this.context.Response.IssueWbXmlResponse();
						}
						this.CompleteRequestWithDelay(this.context.Response);
					}
				}
				finally
				{
					if (this.perUserTracingEnabled)
					{
						AirSyncDiagnostics.ClearThreadTracing();
					}
					if (windowsImpersonationContext != null)
					{
						windowsImpersonationContext.Undo();
						windowsImpersonationContext.Dispose();
					}
					AirSyncDiagnostics.InMemoryTraceOperationCompleted();
				}
				asyncResult = this.result;
			}
			return asyncResult;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			ITimeEntry timeEntry = null;
			WindowsImpersonationContext windowsImpersonationContext = null;
			try
			{
				timeEntry = this.context.Tracker.Start(TimeId.HandlerEndProcessRequest);
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (current.ImpersonationLevel != TokenImpersonationLevel.None)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "EndProcessRequest: de-impersonate is called.");
						windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
					}
				}
				if (this.perUserTracingEnabled)
				{
					AirSyncDiagnostics.SetThreadTracing();
				}
				else
				{
					AirSyncDiagnostics.ClearThreadTracing();
				}
				ActivityContext.SetThreadScope(this.context.ActivityScope);
				AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "EndProcessRequest called on instance {0}.", this.GetHashCode());
				AirSyncDiagnostics.TraceDebug<TimeSpan>(ExTraceGlobals.RequestsTracer, this, "This request took {0}", ExDateTime.UtcNow.Subtract(this.requestStartTime));
				AirSyncCounters.CurrentNumberOfRequests.Decrement();
				long num = (this.watch == null) ? 0L : this.watch.ElapsedMilliseconds;
				if (this.cmd != null)
				{
					if (this.cmd.RequestWaitWatch != null)
					{
						long elapsedMilliseconds = this.cmd.RequestWaitWatch.ElapsedMilliseconds;
						RatePerfCounters.IncrementLatencyPerfCounter(3, num);
						if (elapsedMilliseconds < num)
						{
							RatePerfCounters.IncrementLatencyPerfCounter(2, num - elapsedMilliseconds);
						}
					}
					else
					{
						RatePerfCounters.IncrementLatencyPerfCounter(2, num);
					}
				}
				this.TraceResponseInfo();
				if (this.context != null && this.context.ActivityScope != null)
				{
					this.AddRequestToCache(this.context.ActivityScope.ActivityId);
				}
				if (this.context.Principal != null && this.context.Principal.Identity != null)
				{
					IDisposable disposable = this.context.Principal.Identity as IDisposable;
					if (disposable != null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Disposing this.context.Principal.Identity");
						IIdentity identity = new GenericIdentity(this.GetLogonUserName(null));
						disposable.Dispose();
						this.context.Principal = new GenericPrincipal(identity, null);
					}
				}
				if (this.context.Request != null && this.context.Request.LogonUserIdentity != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Disposing this.context.Request.LogonUserIdentity");
					this.context.Request.LogonUserIdentity.Dispose();
				}
			}
			catch (Exception ex)
			{
				AirSyncDiagnostics.TraceDebug<Exception>(ExTraceGlobals.RequestsTracer, this, "Exception was thrown in EndProcessRequest, exception: '{0}'", ex);
				if (GlobalSettings.SendWatsonReport)
				{
					AirSyncUtility.LogCompressedStackTrace(ex, this.context);
					using (this.context.Tracker.Start(TimeId.HandlerSendWatson))
					{
						AirSyncDiagnostics.SendWatson(ex);
					}
				}
				throw;
			}
			finally
			{
				if (timeEntry != null)
				{
					timeEntry.Dispose();
				}
				List<string> headerValues = this.context.Response.GetHeaderValues("X-MS-BackOffDuration");
				if (headerValues == null || headerValues.Count == 0)
				{
					BackOffValue backOffValueAndReason = this.GetBackOffValueAndReason();
					this.context.Response.AppendHeader("X-MS-BackOffDuration", backOffValueAndReason.ToString(), false);
					if (GlobalSettings.AddBackOffReasonHeader && backOffValueAndReason.BackOffDuration > 0.0)
					{
						this.context.Response.AppendHeader("X-MS-BackOffReason", backOffValueAndReason.BackOffReason, false);
					}
					this.context.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.SuggestedBackOffValue, backOffValueAndReason.ToString());
					if (backOffValueAndReason.BackOffDuration > 0.0)
					{
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.BackOffReason, backOffValueAndReason.BackOffReason);
					}
				}
				if (this.context.User != null)
				{
					this.context.User.DisposeBudget();
					this.context.User.SetBudgetDiagnosticValues(false);
				}
				List<ConditionalResults> list = ConditionalRegistrationCache.Singleton.Evaluate(this.context);
				if (list != null && list.Count > 0)
				{
					foreach (ConditionalResults hit in list)
					{
						ConditionalRegistrationLog.Save(hit);
					}
				}
				if (this.cmd != null)
				{
					this.cmd.Dispose();
					this.cmd = null;
				}
				this.context.WriteActivityContextData();
				this.context.ProtocolLogger.RecordData(this.context.Response);
				this.context.Tracker.Clear();
				if (GlobalSettings.WriteProtocolLogDiagnostics || (this.context.User != null && this.context.User.IsMonitoringTestUser) || (this.context.Request.CommandType != CommandType.Options && this.context.DeviceIdentity.IsDeviceType("TestActiveSyncConnectivity")))
				{
					this.context.Response.AppendHeader("X-MS-Diagnostics", this.context.ProtocolLogger.ToString());
				}
				if (this.context.User != null && this.context.User.ClientSecurityContextWrapper != null)
				{
					this.context.User.ClientSecurityContextWrapper.Dispose();
				}
				if (this.completionTimer != null)
				{
					this.completionTimer.Dispose();
					this.completionTimer = null;
				}
				if (this.perUserTracingEnabled)
				{
					AirSyncDiagnostics.ClearThreadTracing();
				}
				AirSyncDiagnostics.InMemoryTraceOperationCompleted();
				if (windowsImpersonationContext != null)
				{
					windowsImpersonationContext.Undo();
					windowsImpersonationContext.Dispose();
				}
				this.context.ActivityScope.End();
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "ProcessRequest called. This should not happen.");
			throw new ApplicationException("Should never be called");
		}

		private static void ResetAllPerfCounters()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in AirSyncCounters.AllCounters)
			{
				exPerformanceCounter.RawValue = 0L;
			}
		}

		private static int GetMajorVersionFromVersionNumber(int versionNumber)
		{
			return versionNumber >> 22 & 63;
		}

		private static bool IsNewerThanExchange2007(int versionNumber)
		{
			int majorVersionFromVersionNumber = Handler.GetMajorVersionFromVersionNumber(versionNumber);
			return majorVersionFromVersionNumber > Server.Exchange2007MajorVersion;
		}

		private static bool IsWellKnownAccount(IIdentity identity)
		{
			if (identity == null)
			{
				return false;
			}
			if (identity is WindowsIdentity && ((WindowsIdentity)identity).IsAnonymous)
			{
				return true;
			}
			SecurityIdentifier securityIdentifier = identity.GetSecurityIdentifier();
			return securityIdentifier.Value.EndsWith("-500", StringComparison.Ordinal) || securityIdentifier.Value.EndsWith("-501", StringComparison.Ordinal) || securityIdentifier.IsWellKnown(WellKnownSidType.BuiltinGuestsSid) || securityIdentifier.IsWellKnown(WellKnownSidType.AccountGuestSid);
		}

		private bool BlockedByClientAccessRules(IAirSyncUser user, HttpContext httpContext)
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.ActiveSyncClientAccessRulesEnabled.Enabled && ClientAccessRulesUtils.ShouldBlockConnection(user.OrganizationId, ClientAccessRulesUtils.GetUsernameFromIdInformation(user.ADUser.WindowsLiveID, user.ADUser.MasterAccountSid, user.ADUser.Sid, user.ADUser.Id), ClientAccessProtocol.ExchangeActiveSync, ClientAccessRulesUtils.GetRemoteEndPointFromContext(httpContext), ClientAccessAuthenticationMethod.BasicAuthentication, user.ADUser, delegate(ClientAccessRulesEvaluationContext context)
			{
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.BlockingClientAccessRuleName, context.CurrentRule.Name);
			}, delegate(double latency)
			{
				if (latency > GlobalSettings.ClientAccessRulesLatencyThreshold)
				{
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ClientAccessRulesLatency, latency.ToString());
				}
			});
		}

		private BackOffValue GetBackOffValueAndReason()
		{
			if (this.context.User == null)
			{
				return BackOffValue.NoBackOffValue;
			}
			BackOffValue budgetBackOffValue = this.context.User.GetBudgetBackOffValue();
			BackOffValue backOffValue = null;
			DeviceBehavior deviceBehavior;
			if (this.context.Request.CommandType != CommandType.Options && this.context.User.DeviceBehaviorCacheGuid != Guid.Empty && this.context.DeviceIdentity != null && DeviceBehaviorCache.TryGetValue(this.context.User.DeviceBehaviorCacheGuid, this.context.DeviceIdentity, out deviceBehavior))
			{
				backOffValue = deviceBehavior.GetAutoBlockBackOffTime();
			}
			if (backOffValue != null)
			{
				return BackOffValue.GetEffectiveBackOffValue(budgetBackOffValue, backOffValue);
			}
			return budgetBackOffValue;
		}

		private Command CreateCommandObject()
		{
			Command command;
			string arg;
			switch (this.context.Request.CommandType)
			{
			case CommandType.Options:
				command = new OptionsCommand();
				arg = "Options";
				break;
			case CommandType.GetHierarchy:
				command = new GetHierarchyCommand();
				arg = "GetHierarchy";
				break;
			case CommandType.Sync:
				command = new SyncCommand();
				arg = "Sync";
				break;
			case CommandType.GetItemEstimate:
				command = new GetItemEstimateCommand();
				arg = "GetItemEstimate";
				break;
			case CommandType.FolderSync:
				command = new FolderSyncCommand();
				arg = "FolderSync";
				break;
			case CommandType.FolderUpdate:
				command = new FolderUpdateCommand();
				arg = "FolderUpdate";
				break;
			case CommandType.FolderDelete:
				command = new FolderDeleteCommand();
				arg = "FolderDelete";
				break;
			case CommandType.FolderCreate:
				command = new FolderCreateCommand();
				arg = "FolderCreate";
				break;
			case CommandType.CreateCollection:
				command = new CreateCollectionCommand();
				arg = "CreateCollection";
				break;
			case CommandType.MoveCollection:
				command = new MoveCollectionCommand();
				arg = "MoveCollection";
				break;
			case CommandType.DeleteCollection:
				command = new DeleteCollectionCommand();
				arg = "DeleteCollection";
				break;
			case CommandType.GetAttachment:
				command = new GetAttachmentCommand();
				arg = "GetAttachment";
				break;
			case CommandType.MoveItems:
				command = new MoveItemsCommand();
				arg = "MoveItems";
				break;
			case CommandType.MeetingResponse:
				command = new MeetingResponseCommand();
				arg = "MeetingResponse";
				break;
			case CommandType.SendMail:
				command = new SendMailCommand();
				arg = "SendMail";
				break;
			case CommandType.SmartReply:
				command = new SmartReplyCommand();
				arg = "SmartReply";
				break;
			case CommandType.SmartForward:
				command = new SmartForwardCommand();
				arg = "SmartForward";
				break;
			case CommandType.Search:
				command = new SearchCommand();
				arg = "Search";
				break;
			case CommandType.Settings:
				command = new SettingsCommand();
				arg = "Settings";
				break;
			case CommandType.Ping:
				command = new PingCommand();
				arg = "Ping";
				break;
			case CommandType.ItemOperations:
				command = new ItemOperationsCommand();
				arg = "ItemOperations";
				break;
			case CommandType.Provision:
				command = new ProvisionCommand();
				arg = "Provision";
				break;
			case CommandType.ResolveRecipients:
				command = new ResolveRecipientsCommand();
				arg = "ResolveRecipients";
				break;
			case CommandType.ValidateCert:
				command = new ValidateCertCommand();
				arg = "ValidateCert";
				break;
			default:
				return null;
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Received command: {0}", arg);
			return command;
		}

		private HttpStatusCode HandleProxyLoginCommand()
		{
			HttpStatusCode httpStatusCode;
			using (this.context.Tracker.Start(TimeId.HandlerHandleProxyLogin))
			{
				if (this.context.Request.ContentType != "text/xml")
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Failing ProxyLogin request because of wrong content type: {0}", this.context.Request.ContentType);
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "WrongProxyLoginContentType");
					httpStatusCode = HttpStatusCode.Forbidden;
				}
				else if (!this.context.Request.WasProxied)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Failing ProxyLogin request because of missing X-EAS-Proxy header.");
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MissingEasProxyHeader");
					httpStatusCode = HttpStatusCode.BadRequest;
				}
				else
				{
					using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(this.context.Request.LogonUserIdentity))
					{
						if (!LocalServer.AllowsTokenSerializationBy(clientSecurityContext))
						{
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "User {0} does not have permission to do token serialization.", this.GetLogonUserName(null));
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LackingExtendedRights");
							return HttpStatusCode.Forbidden;
						}
					}
					AirSyncUserSecurityContext securityAccessToken;
					try
					{
						securityAccessToken = AirSyncUserSecurityContext.Deserialize(this.context.Request.InputStream);
					}
					catch (XmlException arg)
					{
						AirSyncDiagnostics.TraceDebug<XmlException>(ExTraceGlobals.RequestsTracer, this, "Failed to parse serialized security context: {0}", arg);
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MalformedCsc");
						return HttpStatusCode.Forbidden;
					}
					ClientSecurityContext clientSecurityContext2 = null;
					try
					{
						clientSecurityContext2 = new ClientSecurityContext(securityAccessToken, AuthzFlags.AuthzSkipTokenGroups);
					}
					catch (AuthzException arg2)
					{
						AirSyncDiagnostics.TraceDebug<AuthzException>(ExTraceGlobals.RequestsTracer, this, "Failed to create security context: {0}", arg2);
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AuthzExceptionAtCsc");
						return HttpStatusCode.Forbidden;
					}
					ClientSecurityContextWrapper clientSecurityContextWrapper = (ClientSecurityContextWrapper)HttpRuntime.Cache.Get(this.context.Request.ProxyHeader);
					if (clientSecurityContextWrapper != null)
					{
						clientSecurityContextWrapper.Dispose();
					}
					HttpRuntime.Cache.Insert(this.context.Request.ProxyHeader, ClientSecurityContextWrapper.FromClientSecurityContext(clientSecurityContext2), null, (DateTime)ExDateTime.Now.AddHours(1.0), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
					httpStatusCode = HttpStatusCode.OK;
				}
			}
			return httpStatusCode;
		}

		private bool LegacyUserAllowedToSync(bool trySecure, string serverName, string mailboxName, string smtpDomain, bool impersonated)
		{
			HttpWebResponse httpWebResponse = null;
			WindowsImpersonationContext windowsImpersonationContext = null;
			bool flag;
			try
			{
				try
				{
					string text = string.Concat(new string[]
					{
						trySecure ? "https" : "http",
						"://",
						serverName,
						"/exchange/",
						mailboxName,
						"/NON_IPM_SUBTREE"
					});
					if (!impersonated)
					{
						WindowsIdentity windowsIdentity = (WindowsIdentity)this.context.User.Identity;
						windowsImpersonationContext = windowsIdentity.Impersonate();
					}
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
					httpWebRequest.Method = "PROPFIND";
					httpWebRequest.Headers.Add("Translate: F");
					httpWebRequest.Headers.Add("Depth: 0");
					httpWebRequest.UserAgent = "EAS-CheckForLock/v1.0";
					byte[] bytes = Encoding.ASCII.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?><a:propfind xmlns:a=\"DAV:\" xmlns:K=\"http://schemas.microsoft.com/mapi/string/{71035549-0739-4dcb-9163-00F0580DBBDF}/AirSync:\"><a:prop><K:AirSyncLock/></a:prop></a:propfind>");
					httpWebRequest.ContentLength = (long)bytes.Length;
					httpWebRequest.ContentType = "text/xml";
					httpWebRequest.Credentials = CredentialCache.DefaultCredentials.GetCredential(new Uri(text), "Kerberos");
					httpWebRequest.PreAuthenticate = true;
					httpWebRequest.AllowAutoRedirect = false;
					CertificateValidationManager.SetComponentId(httpWebRequest, "AirSync");
					try
					{
						using (Stream requestStream = httpWebRequest.GetRequestStream())
						{
							requestStream.Write(bytes, 0, bytes.Length);
						}
						httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					}
					catch (SocketException)
					{
						if (!trySecure)
						{
							return this.LegacyUserAllowedToSync(true, serverName, mailboxName, smtpDomain, true);
						}
						return true;
					}
					catch (IOException)
					{
						if (!trySecure)
						{
							return this.LegacyUserAllowedToSync(true, serverName, mailboxName, smtpDomain, true);
						}
						return true;
					}
					catch (WebException ex)
					{
						if (!trySecure && ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden)
						{
							return this.LegacyUserAllowedToSync(true, serverName, mailboxName, smtpDomain, true);
						}
						return true;
					}
					if (httpWebResponse != null)
					{
						AirSyncDiagnostics.TraceDebug<HttpStatusCode>(ExTraceGlobals.RequestsTracer, this, "DAV response status is {0}.", httpWebResponse.StatusCode);
					}
					if (httpWebResponse != null && (httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently || httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently || httpWebResponse.StatusCode == HttpStatusCode.Found || httpWebResponse.StatusCode == HttpStatusCode.TemporaryRedirect || httpWebResponse.StatusCode == HttpStatusCode.SeeOther))
					{
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				finally
				{
					if (httpWebResponse != null)
					{
						httpWebResponse.Close();
						httpWebResponse = null;
					}
					if (windowsImpersonationContext != null)
					{
						windowsImpersonationContext.Undo();
						windowsImpersonationContext.Dispose();
						windowsImpersonationContext = null;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return flag;
		}

		private bool RequestMustBeProxied(string servername, out ProxyHandler.ProxiedRequestInfo proxyinfo)
		{
			bool flag;
			using (this.context.Tracker.Start(TimeId.HandlerRequestMustBeProxied))
			{
				int majorVersionFromVersionNumber = Handler.GetMajorVersionFromVersionNumber(this.context.User.ExchangePrincipal.MailboxInfo.Location.ServerVersion);
				proxyinfo = null;
				if (majorVersionFromVersionNumber != Server.Exchange2007MajorVersion)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format("Request should be handled by current server and should not be prox-ied for mailbox version other than E12. Current mailbox major version {0}.", majorVersionFromVersionNumber));
					flag = false;
				}
				else
				{
					MobileSyncService mobileSyncService = null;
					IList<MobileSyncService> list = null;
					ServiceTopology serviceTopology = null;
					using (this.context.Tracker.Start(TimeId.HandlerServiceDiscoveryLookupExternal))
					{
						serviceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\AirSync\\src\\Handler\\Handler.cs", "RequestMustBeProxied", 1862);
						list = serviceTopology.FindAll<MobileSyncService>(this.context.User.ExchangePrincipal, ClientAccessType.External, "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\Handler\\Handler.cs", "RequestMustBeProxied", 1865);
					}
					foreach (MobileSyncService mobileSyncService2 in list)
					{
						if (majorVersionFromVersionNumber == Handler.GetMajorVersionFromVersionNumber(mobileSyncService2.ServerVersionNumber))
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Found an external CAS , which can be used for redirect.");
							mobileSyncService = mobileSyncService2;
							break;
						}
					}
					if (mobileSyncService != null && DeviceCapability.DeviceCanHandleRedirect(this.context))
					{
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MisconfiguredDevice");
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.RedirectTo, mobileSyncService.Url.ToString());
						AirSyncCounters.NumberOfWrongCASProxyRequests.Increment();
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Best CAS is internet-facing -- the device is misconfigured.");
						if (mobileSyncService.Url.Equals(ADNotificationManager.ADMobileVirtualDirectory.ExternalUrl))
						{
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ServerExternalUrlConfigError");
							AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ServerExternalUrlConfigurationError, "BadServerExternalUrlConfiguration", new string[]
							{
								mobileSyncService.Url.ToString(),
								mobileSyncService.VirtualDirectoryIdentity
							});
							AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater, null, false);
							throw ex;
						}
						throw new IncorrectUrlRequestException((HttpStatusCode)451, "X-MS-Location", mobileSyncService.Url.ToString());
					}
					else
					{
						mobileSyncService = null;
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Request could not be redirected. We need to proxy request to E12 CAS.");
						IList<MobileSyncService> list2;
						using (this.context.Tracker.Start(TimeId.HandlerServiceDiscoveryLookupInternal))
						{
							list2 = serviceTopology.FindAll<MobileSyncService>(this.context.User.ExchangePrincipal, ClientAccessType.Internal, "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\Handler\\Handler.cs", "RequestMustBeProxied", 1934);
						}
						foreach (MobileSyncService mobileSyncService3 in list2)
						{
							if (majorVersionFromVersionNumber == Handler.GetMajorVersionFromVersionNumber(mobileSyncService3.ServerVersionNumber))
							{
								mobileSyncService = mobileSyncService3;
								break;
							}
						}
						if (mobileSyncService == null)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Missing service configuration; aborting request.");
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserHasNoCAS");
							throw new DiscoveryInfoMissingException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, EASServerStrings.MissingDiscoveryInfoError);
						}
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Proxying to URL {0}." + majorVersionFromVersionNumber, mobileSyncService.Url.AbsoluteUri);
						Uri uri = mobileSyncService.Url;
						if (!GlobalSettings.AllowProxyingWithoutSsl && uri.Scheme != Uri.UriSchemeHttps)
						{
							this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadSchemeNoSsl");
							throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.RemoteServerHasNoSSL, null, false);
						}
						UriBuilder uriBuilder = new UriBuilder(uri);
						if (uriBuilder.Path[uriBuilder.Path.Length - 1] != '/')
						{
							UriBuilder uriBuilder2 = uriBuilder;
							uriBuilder2.Path += '/';
						}
						UriBuilder uriBuilder3 = uriBuilder;
						uriBuilder3.Path += "Proxy";
						uri = uriBuilder.Uri;
						proxyinfo = new ProxyHandler.ProxiedRequestInfo(this.context.User, uri);
						proxyinfo.RequiresImpersonation = false;
						proxyinfo.AdditionalHeaders["X-EAS-Proxy"] = this.context.User.Identity.GetSecurityIdentifier().ToString() + "," + this.context.User.Name;
						proxyinfo.AdditionalHeaders["msExchProxyUri"] = this.context.Request.Url;
						flag = true;
					}
				}
			}
			return flag;
		}

		private void AddRequestToCache(Guid requestId)
		{
			ActiveSyncRequestData activeSyncRequestData = ActiveSyncRequestCache.Instance.Get(requestId);
			activeSyncRequestData.CommandName = ((this.cmd == null) ? "Unknown" : this.cmd.ToString());
			IAirSyncRequest request = this.context.Request;
			if (request.DeviceIdentity == null)
			{
				activeSyncRequestData.DeviceID = "Unknown";
				activeSyncRequestData.DeviceType = "Unknown";
			}
			else
			{
				activeSyncRequestData.DeviceID = request.DeviceIdentity.DeviceId;
				activeSyncRequestData.DeviceType = request.DeviceIdentity.DeviceType;
			}
			activeSyncRequestData.UserAgent = ((request.CommandType == CommandType.Options || request.CommandType == CommandType.Unknown) ? "Unknown" : request.UserAgent);
			activeSyncRequestData.UserEmail = ((this.context.User == null || this.context.User.ExchangePrincipal == null) ? "Unknown" : this.context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			activeSyncRequestData.HttpStatus = this.context.Response.HttpStatusCode;
			activeSyncRequestData.AirSyncStatus = ((this.context.Response.AirSyncStatus == StatusCode.None) ? StatusCode.Success.ToString() : this.context.Response.AirSyncStatus.ToString());
			activeSyncRequestData.StartTime = this.context.RequestTime;
			activeSyncRequestData.ServerName = Environment.MachineName;
			if (activeSyncRequestData.ErrorDetails == null)
			{
				activeSyncRequestData.ErrorDetails = new List<ErrorDetail>();
			}
			string errorMessage;
			if (this.context.ProtocolLogger.TryGetValue<string>(ProtocolLoggerData.Error, out errorMessage))
			{
				activeSyncRequestData.ErrorDetails.Add(new ErrorDetail
				{
					ErrorMessage = errorMessage,
					DeviceID = activeSyncRequestData.DeviceID,
					UserEmail = activeSyncRequestData.UserEmail
				});
				activeSyncRequestData.HasErrors = true;
			}
			ExDateTime value;
			if (this.context.ProtocolLogger.TryGetValue<ExDateTime>(ProtocolLoggerData.TimeReceived, out value))
			{
				activeSyncRequestData.RequestTime = ExDateTime.UtcNow.Subtract(value).TotalMilliseconds;
			}
		}

		private void TraceRequestInfo()
		{
			if (AirSyncDiagnostics.IsTraceEnabled(TraceType.DebugTrace, ExTraceGlobals.RequestsTracer))
			{
				AirSyncDiagnostics.TraceDebug<string, int>(ExTraceGlobals.RequestsTracer, this, "BeginProcessRequest for user {0} on instance {1} ", this.GetLogonUserName(null), this.GetHashCode());
				AirSyncDiagnostics.TraceDebug<ExDateTime>(ExTraceGlobals.RequestsTracer, this, "Time is {0}", this.requestStartTime);
				this.context.Request.TraceHeaders();
			}
			if (AirSyncDiagnostics.IsTraceEnabled(TraceType.DebugTrace, ExTraceGlobals.RawBodyBytesTracer))
			{
				if (this.context.Request.InputStream.Length == 0L)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RawBodyBytesTracer, this, "This request doesn't have a body");
					return;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RawBodyBytesTracer, this, "Raw request body:");
				long position = this.context.Request.InputStream.Position;
				byte[] array = new byte[this.context.Request.InputStream.Length];
				this.context.Request.InputStream.Read(array, 0, (int)this.context.Request.InputStream.Length);
				this.context.Request.InputStream.Position = position;
				AirSyncDiagnostics.TraceBinaryData(ExTraceGlobals.RawBodyBytesTracer, this, array, array.Length);
			}
		}

		private void TraceResponseInfo()
		{
			if (AirSyncDiagnostics.IsTraceEnabled(TraceType.DebugTrace, ExTraceGlobals.RequestsTracer))
			{
				this.context.Response.TraceHeaders();
			}
			if (AirSyncDiagnostics.IsTraceEnabled(TraceType.DebugTrace, ExTraceGlobals.BodyTracer))
			{
				if (this.cmd != null && this.cmd.XmlResponse != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.BodyTracer, this, "Sending the following response (in WBXML):");
					AirSyncDiagnostics.TraceXmlBody(ExTraceGlobals.BodyTracer, null, this.cmd.XmlResponse);
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.BodyTracer, this, "This response doesn't have a WBXML body");
				}
			}
			if (AirSyncDiagnostics.IsTraceEnabled(TraceType.DebugTrace, ExTraceGlobals.RawBodyBytesTracer))
			{
				if (this.cmd != null && this.cmd.XmlResponse != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RawBodyBytesTracer, this, "Raw WBXML response:");
					using (MemoryStream memoryStream = new MemoryStream())
					{
						WbxmlWriter wbxmlWriter = new WbxmlWriter(memoryStream);
						wbxmlWriter.WriteXmlDocument(this.cmd.XmlResponse);
						AirSyncDiagnostics.TraceBinaryData(ExTraceGlobals.RawBodyBytesTracer, this, memoryStream.GetBuffer(), (int)memoryStream.Length);
						return;
					}
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RawBodyBytesTracer, this, "This response doesn't have a body");
			}
		}

		private bool IsVersion10LegacyDevice()
		{
			string userAgent = this.context.Request.UserAgent;
			if (string.IsNullOrEmpty(userAgent))
			{
				return false;
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "UserAgent is {0}.", userAgent);
			foreach (string value in Constants.Version10DeviceUserAgentPrefixes)
			{
				if (userAgent.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private IAsyncResult DeprecateVersion10()
		{
			if (this.IsVersion10LegacyDevice())
			{
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BlockV1Device");
				this.context.Response.IssueErrorResponse(HttpStatusCode.HttpVersionNotSupported, StatusCode.None);
			}
			else
			{
				if (!GlobalSettings.IsMultiTenancyEnabled)
				{
					AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_InvalidFireWallConfiguration, "DeviceFallBack_D5D3FDA2-E1D9-4f0f-A336-DEFD26DC3024", null);
				}
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BlockFallbackDevice");
				this.context.Response.IssueErrorResponse(HttpStatusCode.HttpVersionNotSupported, StatusCode.None);
			}
			this.CompleteRequestWithDelay(this.context.Response);
			return this.result;
		}

		private void CompleteHttpRequestCallback(object state)
		{
			using (this.context.Tracker.Start(TimeId.HandlerCompleteHttpRequestCallback))
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (this.context.ProtocolLogger.TryGetValue<ExDateTime>(ProtocolLoggerData.TimeReceived, out utcNow))
				{
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.FinalElapsedTime, (int)ExDateTime.UtcNow.Subtract(utcNow).TotalMilliseconds);
				}
				this.context.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.TimeCompleted, ExDateTime.UtcNow);
				this.result.InvokeCallback();
			}
		}

		private IAsyncResult ProxyRequest(HttpContext httpContext, string serverName, ProxyHandler.ProxiedRequestInfo proxyinfo)
		{
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "User being proxied to back-end {0}", serverName);
			this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ProxyingTo, proxyinfo.RemoteUri.Host);
			AirSyncCounters.NumberOfOutgoingProxyRequests.Increment();
			this.proxyHandler = new ProxyHandler(this.context.ProtocolLogger);
			this.context.SetDiagnosticValue(ConditionalHandlerIntermediateSchema.ProxyStartTime, ExDateTime.UtcNow);
			return this.proxyHandler.BeginProcessRequest(httpContext, this.result, proxyinfo);
		}

		private void CompleteRequestWithDelay(IAirSyncResponse response)
		{
			using (this.context.Tracker.Start(TimeId.HandlerCompleteRequestWithDelay))
			{
				int num = (int)(this.context.Response.TimeToRespond - ExDateTime.UtcNow).TotalSeconds;
				if (num <= 0 || (this.context.User != null && this.context.User.IsMonitoringTestUser))
				{
					this.CompleteHttpRequestCallback(null);
				}
				else
				{
					this.context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.CompletedWithDelay, TimeSpan.FromSeconds((double)num));
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.CompletionOffset, num * 1000);
					this.completionTimer = new Timer(new TimerCallback(this.CompleteHttpRequestCallback), this, num * 1000, -1);
				}
			}
		}

		private string GetLogonUserName(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				httpContext = HttpContext.Current;
			}
			string memberName = httpContext.GetMemberName();
			if (!string.IsNullOrEmpty(memberName))
			{
				return memberName;
			}
			return this.context.Request.LogonUserName;
		}

		private const string AirSyncLock = "AirSyncLock";

		private const string UnknownDeviceIdentifier = "Unknown";

		private const string DefaultPropertyGroup = "SmtpAddress,TenantName,DisplayName,MailboxServer,DeviceID,Cmd,ElapsedTime,HttpStatus,EASStatus,Exception";

		private const string DevicePropertyGroup = "DeviceID,DeviceType,ProtocolVersion";

		private const string AirSyncLockPropFindReq = "<?xml version=\"1.0\" encoding=\"utf-8\"?><a:propfind xmlns:a=\"DAV:\" xmlns:K=\"http://schemas.microsoft.com/mapi/string/{71035549-0739-4dcb-9163-00F0580DBBDF}/AirSync:\"><a:prop><K:AirSyncLock/></a:prop></a:propfind>";

		private static string serverVersion = null;

		private static object classMutex = new object();

		private static bool initialized;

		private static bool runningAsLocalSystem;

		private Command cmd;

		private IAirSyncContext context;

		private ExDateTime requestStartTime;

		private Stopwatch watch;

		private bool perUserTracingEnabled;

		private ProxyHandler proxyHandler;

		private LazyAsyncResult result;

		private Timer completionTimer;
	}
}
