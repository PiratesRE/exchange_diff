using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class ErrorHandlerUtilities
	{
		internal static void LogExceptionCodeInIIS(RequestContext requestContext, Exception exception)
		{
			string str;
			if (!ErrorHandlerUtilities.ExceptionCodeMap.TryGetValue(exception.GetType(), out str))
			{
				str = "UE:" + exception.GetType().ToString();
			}
			try
			{
				if (requestContext != null && requestContext.HttpContext != null && requestContext.HttpContext.Response != null)
				{
					requestContext.HttpContext.Response.AppendToLog("&ex=" + str);
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<Exception, Exception>(0L, "Couldn't append exception to IIS log, Exception to log: {0}. New exception: {1}", exception, arg);
			}
		}

		internal static void RecordException(RequestContext requestContext, Exception exception)
		{
			HttpContext httpContext = requestContext.HttpContext;
			ErrorHandlerUtilities.LogExceptionCodeInIIS(requestContext, exception);
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null && getRequestDetailsLogger.ActivityScope != null)
			{
				getRequestDetailsLogger.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, exception.ToString());
			}
			else
			{
				ExTraceGlobals.CoreTracer.TraceDebug<bool, bool, Exception>(0L, "Couldn't append exception to server log. Logger is null: {0}, logger.ActivityScope is null: {1}, Exception: {2}", getRequestDetailsLogger == null, getRequestDetailsLogger != null && getRequestDetailsLogger.ActivityScope == null, exception);
			}
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("ErrorHandlerUtilities", null, "RecordException", exception.ToString()));
			try
			{
				Exception baseException = ErrorHandlerUtilities.GetBaseException(exception);
				httpContext.Response.Headers.Add("X-OWA-Error", baseException.GetType().FullName);
				if (baseException is WrongServerException)
				{
					string value = ((WrongServerException)baseException).RightServerToString();
					if (!string.IsNullOrEmpty(value))
					{
						httpContext.Response.Headers.Add(WellKnownHeader.XDBMountedOnServer, value);
					}
				}
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<HttpException, Exception>(0L, "Failed to add error headers to the response. {0}. Original exception: {1}", arg, exception);
			}
		}

		internal static Exception GetBaseException(Exception exception)
		{
			while (exception.InnerException != null && exception.InnerException.GetType().FullName.StartsWith("Microsoft.Exchange"))
			{
				exception = exception.InnerException;
			}
			return exception;
		}

		internal static void HandleException(RequestContext requestContext, Exception exception)
		{
			if (exception is HttpException && (exception.InnerException is SlabManifestException || exception.InnerException is FlightConfigurationException))
			{
				exception = exception.InnerException;
			}
			ErrorHandlerUtilities.RecordException(requestContext, exception);
			ExTraceGlobals.CoreTracer.TraceDebug<Type, Exception>(0L, "Exception: Type: {0} Error: {1}.", exception.GetType(), exception);
			HttpContext httpContext = requestContext.HttpContext;
			HttpUtilities.MakePageNoCacheNoStore(httpContext.Response);
			if (exception is HttpException)
			{
				HttpUtilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
				return;
			}
			if (exception is OwaInvalidRequestException || exception is OwaInvalidIdFormatException)
			{
				HttpUtilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
				return;
			}
			if (exception is MailboxInSiteFailoverException && requestContext.UserContext != null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0}'s mailbox in-site failover occurs.", requestContext.UserContext.ExchangePrincipal.LegacyDn);
				if (requestContext.UserContext != null)
				{
					requestContext.UserContext.DisconnectMailboxSession();
				}
			}
			if (exception is MailboxCrossSiteFailoverException || exception is WrongServerException)
			{
				if (requestContext.UserContext != null)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0}'s mailbox cross-site failover occurs.", requestContext.UserContext.ExchangePrincipal.LegacyDn);
				}
				UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(httpContext);
				if (userContextCookie != null)
				{
					HttpUtilities.DeleteCookie(httpContext.Response, userContextCookie.CookieName);
				}
			}
			if (exception is OverBudgetException)
			{
				OverBudgetException ex = (OverBudgetException)exception;
				httpContext.Response.AppendToLog(string.Format("&OverBudget({0}/{1}),Owner:{2}[{3}]", new object[]
				{
					ex.IsServiceAccountBudget ? "ServiceAccount" : "Normal",
					ex.PolicyPart,
					ex.Owner,
					ex.Snapshot
				}));
			}
			ErrorInformation exceptionHandlingInformation = ErrorHandlerUtilities.GetExceptionHandlingInformation(exception, requestContext);
			try
			{
				if (!requestContext.ErrorSent)
				{
					requestContext.ErrorSent = true;
					httpContext.Response.Clear();
					try
					{
						if (RequestDispatcherUtilities.GetRequestType(httpContext.Request) != OwaRequestType.ServiceRequest && RequestDispatcherUtilities.GetRequestType(httpContext.Request) != OwaRequestType.Oeh && !httpContext.Request.Path.Contains(OwaUrl.SessionDataPage.ImplicitUrl))
						{
							StringBuilder stringBuilder = new StringBuilder("/owa/auth/errorfe.aspx");
							stringBuilder.Append("?");
							stringBuilder.Append("httpCode");
							stringBuilder.Append("=");
							stringBuilder.Append(500);
							if (exceptionHandlingInformation.SharePointApp)
							{
								stringBuilder.Append("&sharepointapp=true");
							}
							if (exceptionHandlingInformation.SiteMailbox)
							{
								stringBuilder.Append("&sm=true");
							}
							if (exceptionHandlingInformation.GroupMailboxDestination != null)
							{
								stringBuilder.Append("&gm=");
								stringBuilder.Append(HttpUtility.UrlEncode(exceptionHandlingInformation.GroupMailboxDestination));
							}
							if (exceptionHandlingInformation.MessageId != null)
							{
								stringBuilder.Append("&");
								stringBuilder.Append("msg");
								stringBuilder.Append("=");
								stringBuilder.Append((long)exceptionHandlingInformation.MessageId.Value);
								if (!string.IsNullOrWhiteSpace(exceptionHandlingInformation.MessageParameter))
								{
									stringBuilder.Append("&");
									stringBuilder.Append("msgParam");
									stringBuilder.Append("=");
									stringBuilder.Append(HttpUtility.UrlEncode(exceptionHandlingInformation.MessageParameter));
								}
							}
							if (!string.IsNullOrWhiteSpace(httpContext.Response.Headers["X-OWA-Error"]))
							{
								stringBuilder.Append("&owaError=");
								stringBuilder.Append(httpContext.Response.Headers["X-OWA-Error"]);
							}
							stringBuilder.Append("&owaVer=");
							stringBuilder.Append(Globals.ApplicationVersion);
							stringBuilder.Append("&be=");
							stringBuilder.Append(Environment.MachineName);
							stringBuilder.Append("&ts=");
							stringBuilder.Append(DateTime.UtcNow.ToFileTimeUtc());
							if (!string.IsNullOrWhiteSpace(exceptionHandlingInformation.Lids))
							{
								httpContext.Response.AppendToLog(string.Format("&lids={0}", exceptionHandlingInformation.Lids));
							}
							if (exceptionHandlingInformation.SupportLevel != null && exceptionHandlingInformation.SupportLevel != SupportLevel.Unknown)
							{
								httpContext.Response.AppendHeader("X-OWASuppLevel", exceptionHandlingInformation.SupportLevel.ToString());
								httpContext.Response.AppendToLog(string.Format("&{0}={1}", "suplvl", exceptionHandlingInformation.SupportLevel.ToString()));
							}
							httpContext.Response.Redirect(stringBuilder.ToString(), false);
						}
						else
						{
							httpContext.Response.Write(exceptionHandlingInformation.Message);
							httpContext.Response.StatusCode = 500;
							if (exceptionHandlingInformation.MessageId != null)
							{
								httpContext.Response.AddHeader(WellKnownHeader.XOWAErrorMessageID, exceptionHandlingInformation.MessageId.ToString());
							}
							httpContext.Response.TrySkipIisCustomErrors = true;
							httpContext.Response.Flush();
						}
						httpContext.ApplicationInstance.CompleteRequest();
					}
					catch (HttpException arg)
					{
						ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to flush and send response to client after submitting watson and rendering error page. {0}", arg);
					}
				}
			}
			finally
			{
				if (exceptionHandlingInformation.SendWatsonReport && Globals.SendWatsonReports)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending watson report");
					ReportOptions options = (exception is AccessViolationException || exception is InvalidProgramException || exception is TypeInitializationException) ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None;
					ExWatson.SendReport(exception, options, null);
				}
				if (exception is AccessViolationException)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Shutting down OWA due to unrecoverable exception");
					ErrorHandlerUtilities.TerminateProcess();
				}
				else if ((exception is InvalidProgramException || exception is TypeInitializationException) && Interlocked.Exchange(ref ErrorHandlerUtilities.queuedDelayedRestart, 1) == 0)
				{
					new Thread(new ThreadStart(ErrorHandlerUtilities.DelayedRestartUponUnexecutableCode)).Start();
				}
				httpContext.Response.End();
			}
		}

		internal static void RegisterForUnhandledExceptions()
		{
			ExWatson.Init("E12IIS");
			AppDomain.CurrentDomain.UnhandledException += ErrorHandlerUtilities.HandleUnhandledException;
		}

		internal static void RecycleAppPool()
		{
			string appPoolName = Environment.GetEnvironmentVariable("APP_POOL_ID");
			try
			{
				Thread thread = new Thread(delegate()
				{
					try
					{
						using (ServerManager serverManager = new ServerManager())
						{
							ApplicationPool applicationPool = serverManager.ApplicationPools[appPoolName];
							if (applicationPool != null)
							{
								applicationPool.Recycle();
							}
						}
					}
					catch (Exception arg2)
					{
						ExTraceGlobals.CoreTracer.TraceError<string, Exception>(0L, "An exception happened while attempting to recycle {0}. Exception: {1}.", appPoolName, arg2);
					}
				});
				thread.Start();
				thread.Join(TimeSpan.FromSeconds(20.0));
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, Exception>(0L, "An exception while kicking off an async recycle of {0}. Exception: {1}.", appPoolName, arg);
			}
		}

		internal static void TerminateProcess()
		{
			try
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					currentProcess.Kill();
				}
			}
			catch (Win32Exception)
			{
			}
			Environment.Exit(1);
		}

		private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.IsTerminating)
			{
				ErrorHandlerUtilities.RecycleAppPool();
			}
			ExWatson.HandleException(sender, e);
		}

		private static ErrorInformation GetExceptionHandlingInformation(Exception exception, RequestContext requestContext)
		{
			bool sendWatsonReport = false;
			Strings.IDs? messageId = null;
			string messageParameter = null;
			string lids = null;
			SupportLevel? supportLevel = null;
			IMailboxContext mailboxContext = (requestContext != null) ? requestContext.UserContext : null;
			string text = string.Empty;
			if (mailboxContext != null)
			{
				text = mailboxContext.PrimarySmtpAddress.ToString();
			}
			string message;
			if (exception is OwaNotSupportedException)
			{
				message = exception.Message;
			}
			else if (exception is OwaIdentityException)
			{
				sendWatsonReport = false;
				message = exception.Message;
			}
			else if (exception is OwaExistentNotificationPipeException)
			{
				message = Strings.GetLocalizedString(1295605912);
				messageId = new Strings.IDs?(1295605912);
			}
			else if (exception is OwaNotificationPipeException)
			{
				message = Strings.GetLocalizedString(-771052428);
				messageId = new Strings.IDs?(-771052428);
				sendWatsonReport = false;
			}
			else if (exception is OwaOperationNotSupportedException)
			{
				message = exception.Message;
			}
			else if (exception is OwaADObjectNotFoundException)
			{
				OwaADUserNotFoundException ex = exception as OwaADUserNotFoundException;
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
				if (ex != null && !string.IsNullOrWhiteSpace(ex.UserName))
				{
					message = string.Format(Strings.GetLocalizedString(-765910865), ex.UserName);
					messageId = new Strings.IDs?(-765910865);
					messageParameter = ex.UserName;
				}
				else
				{
					message = Strings.GetLocalizedString(-950823100);
					messageId = new Strings.IDs?(-950823100);
				}
			}
			else if (exception is OwaLockTimeoutException || exception is BailOutException)
			{
				message = Strings.GetLocalizedString(-116001901);
				messageId = new Strings.IDs?(-116001901);
				if (requestContext != null)
				{
					requestContext.HttpContext.Response.AppendToLog("&s=ReqTimeout");
				}
			}
			else if (exception is ObjectExistedException)
			{
				message = Strings.GetLocalizedString(-1399945920);
				messageId = new Strings.IDs?(-1399945920);
			}
			else if (exception is MailboxInSiteFailoverException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(26604436);
				messageId = new Strings.IDs?(26604436);
				supportLevel = new SupportLevel?(SupportLevel.Transient);
			}
			else if (exception is MailboxCrossSiteFailoverException || exception is WrongServerException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(26604436);
				messageId = new Strings.IDs?(26604436);
				supportLevel = new SupportLevel?(SupportLevel.Transient);
			}
			else if (exception is MailboxInTransitException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(-1739093686);
				messageId = new Strings.IDs?(-1739093686);
				supportLevel = new SupportLevel?(SupportLevel.Transient);
			}
			else if (exception is ResourceUnhealthyException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(198161982);
				messageId = new Strings.IDs?(198161982);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorResourceUnhealthy, string.Empty, new object[]
				{
					text,
					exception.ToString()
				});
			}
			else if (exception is ConnectionFailedPermanentException || exception is ServerNotFoundException)
			{
				message = string.Format(Strings.GetLocalizedString(-765910865), text);
				messageId = new Strings.IDs?(-765910865);
				messageParameter = text;
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is ConnectionFailedTransientException || exception is MailboxOfflineException)
			{
				if (exception.InnerException is MapiExceptionLogonFailed && mailboxContext.IsExplicitLogon)
				{
					message = Strings.GetLocalizedString(882888134);
					messageId = new Strings.IDs?(882888134);
					supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
				}
				else
				{
					message = Strings.GetLocalizedString(198161982);
					messageId = new Strings.IDs?(198161982);
					supportLevel = new SupportLevel?(SupportLevel.Transient);
				}
			}
			else if (exception is SendAsDeniedException)
			{
				message = Strings.GetLocalizedString(2059222100);
				messageId = new Strings.IDs?(2059222100);
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is ADTransientException)
			{
				message = Strings.GetLocalizedString(634294555);
				messageId = new Strings.IDs?(634294555);
				supportLevel = new SupportLevel?(SupportLevel.Transient);
			}
			else if (exception is ADOperationException)
			{
				message = Strings.GetLocalizedString(-256207770);
				messageId = new Strings.IDs?(-256207770);
				supportLevel = new SupportLevel?(SupportLevel.Unknown);
			}
			else if (exception is DataValidationException)
			{
				message = Strings.GetLocalizedString(-256207770);
				messageId = new Strings.IDs?(-256207770);
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is SaveConflictException || exception is OwaSaveConflictException)
			{
				message = Strings.GetLocalizedString(-482397486);
				messageId = new Strings.IDs?(-482397486);
			}
			else if (exception is FolderSaveException)
			{
				message = Strings.GetLocalizedString(1487149567);
				messageId = new Strings.IDs?(1487149567);
			}
			else if (exception is ObjectValidationException)
			{
				message = Strings.GetLocalizedString(-1670564952);
				messageId = new Strings.IDs?(-1670564952);
			}
			else if (exception is CorruptDataException)
			{
				message = Strings.GetLocalizedString(-1670564952);
				messageId = new Strings.IDs?(-1670564952);
			}
			else if (exception is Microsoft.Exchange.Data.Storage.QuotaExceededException || exception is MessageTooBigException)
			{
				message = Strings.GetLocalizedString(-640701623);
				messageId = new Strings.IDs?(-640701623);
			}
			else if (exception is SubmissionQuotaExceededException)
			{
				message = Strings.GetLocalizedString(178029729);
				messageId = new Strings.IDs?(178029729);
			}
			else if (exception is MessageSubmissionExceededException)
			{
				message = Strings.GetLocalizedString(-1381793955);
				messageId = new Strings.IDs?(-1381793955);
			}
			else if (exception is AttachmentExceededException)
			{
				message = Strings.GetLocalizedString(-2137146650);
				messageId = new Strings.IDs?(-2137146650);
			}
			else if (exception is ResourcesException || exception is NoMoreConnectionsException)
			{
				message = Strings.GetLocalizedString(-639453714);
				messageId = new Strings.IDs?(-639453714);
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is AccountDisabledException)
			{
				message = Strings.GetLocalizedString(531497785);
				messageId = new Strings.IDs?(531497785);
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is AccessDeniedException)
			{
				message = Strings.GetLocalizedString(995407892);
				messageId = new Strings.IDs?(995407892);
				AccessDeniedException ex2 = (AccessDeniedException)exception;
				if (ex2.InnerException != null)
				{
					Exception innerException = ex2.InnerException;
					if (innerException is MapiExceptionPasswordChangeRequired || innerException is MapiExceptionPasswordExpired)
					{
						message = Strings.GetLocalizedString(540943741);
						messageId = new Strings.IDs?(540943741);
					}
				}
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is InvalidLicenseException)
			{
				message = string.Format(Strings.GetLocalizedString(468041898), requestContext.UserContext.MailboxIdentity.SafeGetRenderableName());
				messageId = new Strings.IDs?(468041898);
				messageParameter = requestContext.UserContext.MailboxIdentity.SafeGetRenderableName();
				sendWatsonReport = false;
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is TenantAccessBlockedException)
			{
				message = Strings.GetLocalizedString(1045420842);
				messageId = new Strings.IDs?(1045420842);
				sendWatsonReport = false;
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is PropertyErrorException)
			{
				message = Strings.GetLocalizedString(641346049);
				messageId = new Strings.IDs?(641346049);
			}
			else if (exception is OwaInvalidOperationException)
			{
				message = Strings.GetLocalizedString(641346049);
				messageId = new Strings.IDs?(641346049);
			}
			else if (exception is VirusDetectedException)
			{
				message = Strings.GetLocalizedString(-589723291);
				messageId = new Strings.IDs?(-589723291);
			}
			else if (exception is VirusScanInProgressException)
			{
				message = Strings.GetLocalizedString(-1019777596);
				messageId = new Strings.IDs?(-1019777596);
			}
			else if (exception is VirusMessageDeletedException)
			{
				message = Strings.GetLocalizedString(1164605313);
				messageId = new Strings.IDs?(1164605313);
			}
			else if (exception is OwaExplicitLogonException)
			{
				message = Strings.GetLocalizedString(882888134);
				messageId = new Strings.IDs?(882888134);
				sendWatsonReport = false;
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is NoReplicaException)
			{
				message = Strings.GetLocalizedString(1179266056);
				messageId = new Strings.IDs?(1179266056);
			}
			else if (exception is TooManyObjectsOpenedException)
			{
				message = Strings.GetLocalizedString(-1763248954);
				messageId = new Strings.IDs?(-1763248954);
				supportLevel = new SupportLevel?(SupportLevel.User);
			}
			else if (exception is OwaUserHasNoMailboxAndNoLicenseAssignedException)
			{
				message = Strings.GetLocalizedString(115127791);
				messageId = new Strings.IDs?(115127791);
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is UserHasNoMailboxException)
			{
				message = Strings.GetLocalizedString(-765910865);
				messageId = new Strings.IDs?(-765910865);
				messageParameter = exception.Data["PrimarySmtpAddress"].ToString();
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is StorageTransientException)
			{
				message = Strings.GetLocalizedString(-238819799);
				messageId = new Strings.IDs?(-238819799);
				if (exception.InnerException is MapiExceptionRpcServerTooBusy)
				{
					sendWatsonReport = false;
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorMailboxServerTooBusy, string.Empty, new object[]
					{
						text,
						exception.ToString()
					});
				}
				supportLevel = new SupportLevel?(SupportLevel.Transient);
			}
			else if (exception is RulesTooBigException)
			{
				message = Strings.GetLocalizedString(-791981113);
				messageId = new Strings.IDs?(-791981113);
			}
			else if (exception is DuplicateActionException)
			{
				message = Strings.GetLocalizedString(-555068615);
				messageId = new Strings.IDs?(-555068615);
			}
			else if (exception is ConversionFailedException && ((ConversionFailedException)exception).ConversionFailureReason == ConversionFailureReason.CorruptContent)
			{
				message = Strings.GetLocalizedString(-1670564952);
				messageId = new Strings.IDs?(-1670564952);
			}
			else if (exception is IOException && ErrorHandlerUtilities.IsDiskFullException(exception))
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(-1729839551);
				messageId = new Strings.IDs?(-1729839551);
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is StoragePermanentException)
			{
				message = Strings.GetLocalizedString(861904327);
				messageId = new Strings.IDs?(861904327);
				if (exception.InnerException is MapiPermanentException)
				{
					DiagnosticContext diagCtx = ((MapiPermanentException)exception.InnerException).DiagCtx;
					if (diagCtx != null)
					{
						lids = diagCtx.ToCompactString();
					}
				}
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is TransientException)
			{
				message = Strings.GetLocalizedString(-1729839551);
				messageId = new Strings.IDs?(-1729839551);
				supportLevel = new SupportLevel?(SupportLevel.Transient);
				if (exception.InnerException is MapiRetryableException)
				{
					DiagnosticContext diagCtx2 = ((MapiRetryableException)exception.InnerException).DiagCtx;
					if (diagCtx2 != null)
					{
						lids = diagCtx2.ToCompactString();
					}
				}
			}
			else if (exception is HttpException)
			{
				HttpException ex3 = (HttpException)exception;
				message = string.Format(Strings.GetLocalizedString(1331629462), ex3.GetHttpCode());
				messageId = new Strings.IDs?(1331629462);
				messageParameter = ex3.GetHttpCode().ToString();
			}
			else if (exception is OverBudgetException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(1856724252);
				messageId = new Strings.IDs?(1856724252);
			}
			else if (exception is COMException || exception.InnerException is COMException)
			{
				sendWatsonReport = !ErrorHandlerUtilities.ShouldIgnoreException((exception is COMException) ? exception : exception.InnerException);
				message = Strings.GetLocalizedString(641346049);
				messageId = new Strings.IDs?(641346049);
				supportLevel = new SupportLevel?(SupportLevel.EscalateToSupport);
			}
			else if (exception is ThreadAbortException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(641346049);
				messageId = new Strings.IDs?(641346049);
			}
			else if (exception is FaultException || exception is InvalidSerializedAccessTokenException)
			{
				sendWatsonReport = false;
				message = exception.Message;
			}
			else if (exception is NonExistentMailboxException)
			{
				sendWatsonReport = false;
				message = exception.Message;
				supportLevel = new SupportLevel?(SupportLevel.TenantAdmin);
			}
			else if (exception is SlabManifestException || exception is FlightConfigurationException)
			{
				sendWatsonReport = false;
				message = Strings.GetLocalizedString(2099558169);
				messageId = new Strings.IDs?(2099558169);
			}
			else
			{
				sendWatsonReport = true;
				message = Strings.GetLocalizedString(641346049);
				messageId = new Strings.IDs?(641346049);
			}
			string empty = string.Empty;
			Strings.IDs? ds = null;
			bool siteMailbox = false;
			bool flag = ErrorHandlerUtilities.RemedyExceptionHandlingError(exception, requestContext, out empty, out ds, out siteMailbox);
			if (flag)
			{
				message = empty;
				messageId = ds;
			}
			string groupMailboxDestination = ErrorHandlerUtilities.GetGroupMailboxDestination(exception, requestContext);
			return new ErrorInformation
			{
				Exception = exception,
				Message = message,
				MessageId = messageId,
				MessageParameter = messageParameter,
				SendWatsonReport = sendWatsonReport,
				SharePointApp = flag,
				SiteMailbox = siteMailbox,
				GroupMailboxDestination = groupMailboxDestination,
				Lids = lids,
				SupportLevel = supportLevel
			};
		}

		private static bool IsDiskFullException(Exception e)
		{
			return Marshal.GetHRForException(e) == -2147024784;
		}

		private static void DelayedRestartUponUnexecutableCode()
		{
			Thread.Sleep(90000);
			OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaRestartingAfterFailedLoad, string.Empty, new object[0]);
			ErrorHandlerUtilities.TerminateProcess();
		}

		private static bool ShouldIgnoreException(Exception exception)
		{
			COMException ex = exception as COMException;
			return ex != null && (ex.ErrorCode == -2147023901 || ex.ErrorCode == -2147024832 || ex.ErrorCode == -2147024895 || ex.ErrorCode == -2147024890 || ex.ErrorCode == -2147023667);
		}

		private static bool RemedyExceptionHandlingError(Exception exception, RequestContext requestContext, out string error, out Strings.IDs? errorMessageId, out bool siteMailbox)
		{
			DateTime? dateTime = null;
			error = string.Empty;
			errorMessageId = null;
			siteMailbox = ErrorHandlerUtilities.IsSharePointAppRequest(requestContext, out dateTime);
			if (siteMailbox)
			{
				if ((exception is ConnectionFailedTransientException && exception.InnerException is MapiExceptionLogonFailed) || (exception is AccessDeniedException || (exception is OwaExplicitLogonException && exception.InnerException is AccessDeniedException)) || (exception is InvalidSerializedAccessTokenException && exception.InnerException is ObjectNotFoundException))
				{
					error = Strings.GetLocalizedString(-1076784851);
					errorMessageId = new Strings.IDs?(-1076784851);
					return true;
				}
				bool flag = dateTime == null || dateTime.Value.AddMinutes(30.0).ToUniversalTime() > DateTime.UtcNow;
				if (flag)
				{
					error = Strings.GetLocalizedString(825706319);
					errorMessageId = new Strings.IDs?(825706319);
					return true;
				}
			}
			return false;
		}

		private static bool IsSharePointAppRequest(RequestContext requestContext, out DateTime? whenMailboxCreated)
		{
			bool result = false;
			whenMailboxCreated = null;
			if (requestContext != null)
			{
				if (requestContext.UserContext != null && requestContext.UserContext.ExchangePrincipal != null)
				{
					result = (requestContext.UserContext.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox);
					whenMailboxCreated = requestContext.UserContext.ExchangePrincipal.MailboxInfo.WhenMailboxCreated;
				}
				else if (requestContext.HttpContext != null)
				{
					result = UserContextUtilities.IsSharePointAppRequest(requestContext.HttpContext.Request);
				}
			}
			return result;
		}

		private static string GetGroupMailboxDestination(Exception exception, RequestContext requestContext)
		{
			if (exception is UserHasNoMailboxException && requestContext != null && requestContext.HttpContext != null && requestContext.HttpContext.Request != null && string.Equals(requestContext.HttpContext.Request.QueryString["src"], "Mail", StringComparison.OrdinalIgnoreCase) && string.Equals(requestContext.HttpContext.Request.QueryString["type"], "MG", StringComparison.OrdinalIgnoreCase))
			{
				return requestContext.HttpContext.Request.QueryString["to"];
			}
			return null;
		}

		private const string UnknownExceptionPrefix = "UE:";

		private const string SharePointAppErrorPageQueryStringString = "&sharepointapp=true";

		private const int ErrorAborted = -2147023901;

		private const int ErrorNonExistentConnection = -2147023667;

		private const int ErrorNetworkNameNotAvailable = -2147024832;

		private const int ErrorInvalidHandle = -2147024890;

		private const int ErrorIncorrectFunction = -2147024895;

		private static readonly Dictionary<Type, string> ExceptionCodeMap = new Dictionary<Type, string>
		{
			{
				typeof(OwaInvalidRequestException),
				"E002"
			},
			{
				typeof(OwaInvalidIdFormatException),
				"E003"
			},
			{
				typeof(OwaNotSupportedException),
				"E010"
			},
			{
				typeof(OwaExistentNotificationPipeException),
				"E012"
			},
			{
				typeof(OwaNotificationPipeException),
				"E013"
			},
			{
				typeof(OwaOperationNotSupportedException),
				"E014"
			},
			{
				typeof(OwaADObjectNotFoundException),
				"E015"
			},
			{
				typeof(OwaLockTimeoutException),
				"E017"
			},
			{
				typeof(OwaSaveConflictException),
				"E022"
			},
			{
				typeof(OwaInvalidOperationException),
				"E027"
			},
			{
				typeof(OwaExplicitLogonException),
				"E031"
			},
			{
				typeof(OwaNotificationPipeWriteException),
				"E051"
			},
			{
				typeof(OwaADUserNotFoundException),
				"E052"
			},
			{
				typeof(SlabManifestException),
				"E053"
			},
			{
				typeof(FlightConfigurationException),
				"E054"
			},
			{
				typeof(OwaUserHasNoMailboxAndNoLicenseAssignedException),
				"E055"
			},
			{
				typeof(MailboxInSiteFailoverException),
				"E101"
			},
			{
				typeof(MailboxCrossSiteFailoverException),
				"E102"
			},
			{
				typeof(ObjectNotFoundException),
				"E104"
			},
			{
				typeof(ObjectValidationException),
				"E105"
			},
			{
				typeof(CorruptDataException),
				"E106"
			},
			{
				typeof(PropertyValidationException),
				"E107"
			},
			{
				typeof(InvalidSharingMessageException),
				"E108"
			},
			{
				typeof(InvalidSharingDataException),
				"E109"
			},
			{
				typeof(InvalidExternalSharingInitiatorException),
				"E110"
			},
			{
				typeof(VirusDetectedException),
				"E111"
			},
			{
				typeof(VirusScanInProgressException),
				"E112"
			},
			{
				typeof(VirusMessageDeletedException),
				"E113"
			},
			{
				typeof(NoReplicaException),
				"E114"
			},
			{
				typeof(StorageTransientException),
				"E116"
			},
			{
				typeof(RulesTooBigException),
				"E117"
			},
			{
				typeof(DuplicateActionException),
				"E118"
			},
			{
				typeof(ObjectExistedException),
				"E119"
			},
			{
				typeof(MailboxInTransitException),
				"E120"
			},
			{
				typeof(ConnectionFailedPermanentException),
				"E121"
			},
			{
				typeof(ConnectionFailedTransientException),
				"E122"
			},
			{
				typeof(MailboxOfflineException),
				"E123"
			},
			{
				typeof(SendAsDeniedException),
				"E124"
			},
			{
				typeof(RecurrenceFormatException),
				"E125"
			},
			{
				typeof(OccurrenceTimeSpanTooBigException),
				"E126"
			},
			{
				typeof(Microsoft.Exchange.Data.Storage.QuotaExceededException),
				"E127"
			},
			{
				typeof(MessageTooBigException),
				"E128"
			},
			{
				typeof(SubmissionQuotaExceededException),
				"E129"
			},
			{
				typeof(MessageSubmissionExceededException),
				"E130"
			},
			{
				typeof(AttachmentExceededException),
				"E131"
			},
			{
				typeof(ResourcesException),
				"E132"
			},
			{
				typeof(NoMoreConnectionsException),
				"E133"
			},
			{
				typeof(AccountDisabledException),
				"E134"
			},
			{
				typeof(AccessDeniedException),
				"E135"
			},
			{
				typeof(StoragePermanentException),
				"E136"
			},
			{
				typeof(ServerNotFoundException),
				"E137"
			},
			{
				typeof(SaveConflictException),
				"E138"
			},
			{
				typeof(FolderSaveException),
				"E139"
			},
			{
				typeof(OccurrenceCrossingBoundaryException),
				"E140"
			},
			{
				typeof(RecurrenceEndDateTooBigException),
				"E142"
			},
			{
				typeof(RecurrenceStartDateTooSmallException),
				"E143"
			},
			{
				typeof(RecurrenceHasNoOccurrenceException),
				"E144"
			},
			{
				typeof(PropertyErrorException),
				"E145"
			},
			{
				typeof(ConversionFailedException),
				"E146"
			},
			{
				typeof(RightsManagementPermanentException),
				"E147"
			},
			{
				typeof(DataValidationException),
				"E148"
			},
			{
				typeof(InvalidObjectOperationException),
				"E149"
			},
			{
				typeof(TooManyObjectsOpenedException),
				"E150"
			},
			{
				typeof(ADTransientException),
				"E301"
			},
			{
				typeof(ADOperationException),
				"E302"
			},
			{
				typeof(OverBudgetException),
				"E303"
			},
			{
				typeof(ResourceUnhealthyException),
				"E304"
			},
			{
				typeof(COMException),
				"E401"
			},
			{
				typeof(ThreadAbortException),
				"E402"
			},
			{
				typeof(InvalidOperationException),
				"E403"
			},
			{
				typeof(NullReferenceException),
				"E404"
			},
			{
				typeof(OutOfMemoryException),
				"E405"
			},
			{
				typeof(ArgumentException),
				"E406"
			},
			{
				typeof(IndexOutOfRangeException),
				"E407"
			},
			{
				typeof(ArgumentOutOfRangeException),
				"E408"
			},
			{
				typeof(HttpException),
				"E409"
			},
			{
				typeof(ArgumentNullException),
				"E410"
			}
		};

		private static int queuedDelayedRestart;
	}
}
