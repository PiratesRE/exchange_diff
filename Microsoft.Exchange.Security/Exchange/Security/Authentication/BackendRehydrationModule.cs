using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	public class BackendRehydrationModule : IHttpModule
	{
		internal static ExEventLog EventLogger
		{
			get
			{
				return BackendRehydrationModule.eventLogger.Member;
			}
		}

		public void Init(HttpApplication application)
		{
			application.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		public void Dispose()
		{
		}

		protected virtual bool UseAuthIdentifierCache
		{
			get
			{
				return false;
			}
		}

		protected virtual bool NeedTokenRehydration(HttpContext context)
		{
			return true;
		}

		private void OnAuthenticateRequest(object source, EventArgs args)
		{
			ExTraceGlobals.BackendRehydrationTracer.TraceFunction((long)this.GetHashCode(), "[BackendRehydrationModule::OnAuthenticateRequest] Entering");
			HttpContext httpContext = HttpContext.Current;
			if (httpContext.Request.IsAuthenticated)
			{
				if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string, Type>((long)this.GetHashCode(), "[BackendRehydrationModule::OnAuthenticateRequest] Current authenticated user is {0} of type {1}.", BackendRehydrationModule.GetCheapIdentityName(httpContext.User.Identity), httpContext.User.Identity.GetType());
				}
				Exception ex = null;
				bool isUnhandledException = false;
				HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
				try
				{
					this.ProcessRequest(httpContext);
				}
				catch (CommonAccessTokenException ex2)
				{
					isUnhandledException = true;
					ex = ex2;
				}
				catch (BackendRehydrationException ex3)
				{
					if (ex3.InnerException is SecurityException || ex3.InnerException is UnauthorizedAccessException || ex3.InnerException is LowPasswordConfidenceException)
					{
						statusCode = HttpStatusCode.Forbidden;
						if (ex3.InnerException is LowPasswordConfidenceException)
						{
							httpContext.Response.AppendToLog(string.Format("LiveIdBasicLog={0};LiveIdBasicAuthResult={1}", ex3.Message, LiveIdAuthResult.LowPasswordConfidence));
						}
					}
					else
					{
						isUnhandledException = true;
					}
					ex = ex3;
				}
				catch (InvalidOAuthTokenException ex4)
				{
					ex = ex4;
					OAuthErrorCategory errorCategory = ex4.ErrorCategory;
					if (errorCategory != OAuthErrorCategory.InternalError)
					{
						statusCode = HttpStatusCode.Unauthorized;
					}
					else
					{
						isUnhandledException = true;
					}
					MSDiagnosticsHeader.AppendInvalidOAuthTokenExceptionToBackendResponse(httpContext, ex4);
				}
				catch (SystemException ex5)
				{
					ex = ex5;
				}
				catch (ADTransientException ex6)
				{
					ex = ex6;
				}
				catch (DataValidationException ex7)
				{
					ex = ex7;
				}
				catch (DataSourceOperationException ex8)
				{
					ex = ex8;
				}
				if (ex != null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceError<Exception>((long)this.GetHashCode(), "[BackendRehydrationModule::OnAuthenticateRequest] Unexpected error occured. Exception: {0}", ex);
					BackendRehydrationModule.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_BackendRehydrationError, string.Empty, new object[]
					{
						HttpContext.Current.Request.ApplicationPath,
						httpContext.User.Identity.GetSafeName(true),
						ex
					});
					WinRMInfo.SetFailureCategoryInfo(httpContext.Response.Headers, FailureCategory.BackendRehydration, ex.GetType().Name);
					HttpLogger.SafeAppendGenericError("BackendRehydrationModule", ex.ToString(), isUnhandledException);
					httpContext.Response.StatusCode = (int)statusCode;
					httpContext.ApplicationInstance.CompleteRequest();
				}
			}
			ExTraceGlobals.BackendRehydrationTracer.TraceDebug((long)this.GetHashCode(), "[BackendRehydrationModule::OnAuthenticateRequest] Exiting");
		}

		private void ProcessRequest(HttpContext httpContext)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				CommonAccessToken token;
				if (this.TryGetCommonAccessToken(httpContext, stopwatch, out token))
				{
					long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					try
					{
						BackendAuthenticator backendAuthenticator = null;
						string value = null;
						bool flag = true;
						bool flag2 = false;
						if (this.UseAuthIdentifierCache)
						{
							BackendAuthenticator.GetAuthIdentifier(token, ref backendAuthenticator, out value);
							flag2 = string.IsNullOrEmpty(value);
							flag = (flag2 || this.NeedTokenRehydration(httpContext));
						}
						if (flag)
						{
							IPrincipal principal = null;
							string text = null;
							IAccountValidationContext accountValidationContext = null;
							if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
							{
								accountValidationContext = (httpContext.Items["AccountValidationContext"] as IAccountValidationContext);
							}
							BackendAuthenticator.Rehydrate(token, ref backendAuthenticator, flag2, out text, out principal, ref accountValidationContext);
							if (accountValidationContext != null)
							{
								httpContext.Items["AccountValidationContext"] = accountValidationContext;
							}
							if (flag2)
							{
								value = text;
							}
							string text2 = null;
							string text3 = null;
							if (principal != null && principal.Identity != null)
							{
								text2 = BackendRehydrationModule.GetCheapIdentityName(principal.Identity);
								text3 = principal.Identity.GetType().ToString();
								if (!CompositeIdentityBuilder.TryHandleRehydratedIdentity(httpContext, principal.Identity))
								{
									ExTraceGlobals.BackendRehydrationTracer.TraceDebug<BackendAuthenticator, string, string>(0L, "[BackendRehydrationModule::ProcessRequest] Authenticator {0} rehydrated identity {1} of type {2}, which was NOT added to the CompositeIdentity.", backendAuthenticator, text2, text3);
								}
								else
								{
									ExTraceGlobals.BackendRehydrationTracer.TraceDebug<BackendAuthenticator, string, string>(0L, "[BackendRehydrationModule::ProcessRequest] Authenticator {0} rehydrated identity {1} of type {2}, which was added to the CompositeIdentity.", backendAuthenticator, text2, text3);
								}
							}
							ExTraceGlobals.BackendRehydrationTracer.TraceDebug<BackendAuthenticator, string, string>(0L, "[BackendRehydrationModule::ProcessRequest] Authenticator {0} rehydrated identity {1} of type {2}.", backendAuthenticator, text2, text3);
							BackendRehydrationModule.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_BackendRehydrationRehydrated, string.Empty, new object[]
							{
								HttpContext.Current.Request.ApplicationPath,
								text2,
								text3,
								backendAuthenticator.GetType()
							});
							httpContext.User = principal;
						}
						else
						{
							httpContext.User = BackendRehydrationModule.defaultSecurityPrincipal;
						}
						if (!string.IsNullOrEmpty(value))
						{
							httpContext.Items["BEAuthIdentifier"] = value;
						}
						if (backendAuthenticator != null)
						{
							httpContext.Items["BackEndAuthenticator"] = backendAuthenticator.GetType().Name;
						}
					}
					finally
					{
						httpContext.Items["BERehydrationLatency"] = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
					}
				}
			}
			finally
			{
				httpContext.Items["TotalBERehydrationModuleLatency"] = stopwatch.ElapsedMilliseconds;
			}
		}

		private bool TryGetCommonAccessToken(HttpContext httpContext, Stopwatch stopwatch, out CommonAccessToken token)
		{
			token = (httpContext.Items["Item-CommonAccessToken"] as CommonAccessToken);
			if (token != null)
			{
				if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string>((long)this.GetHashCode(), "[BackendRehydrationModule::TryGetCommonAccessToken] Token type is {0}", token.TokenType);
				}
				return true;
			}
			string text = httpContext.Request.Headers["X-CommonAccessToken"];
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string>((long)this.GetHashCode(), "[BackendRehydrationModule::TryGetCommonAccessToken] Serialized token content is {0}", text);
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			bool flag;
			try
			{
				flag = this.IsTokenSerializationAllowed(httpContext.User.Identity as WindowsIdentity);
			}
			finally
			{
				httpContext.Items["BEValidateCATRightsLatency"] = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
			}
			if (!flag)
			{
				string safeName = httpContext.User.Identity.GetSafeName(true);
				if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceError<string>((long)this.GetHashCode(), "[BackendRehydrationModule::TryGetCommonAccessToken] Current authenticated user {0} does not have the permission to serialize security token.", safeName);
				}
				BackendRehydrationModule.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_BackendRehydrationNoTokenSerializationPermission, safeName, new object[]
				{
					HttpContext.Current.Request.ApplicationPath,
					safeName
				});
				throw new BackendRehydrationException(SecurityStrings.SourceServerNoTokenSerializationPermission(safeName));
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			try
			{
				token = CommonAccessToken.Deserialize(text);
				if (token == null)
				{
					return false;
				}
			}
			finally
			{
				httpContext.Items["CATDeserializationLatency"] = stopwatch.ElapsedMilliseconds - elapsedMilliseconds2;
			}
			if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string>((long)this.GetHashCode(), "[BackendRehydrationModule::TryGetCommonAccessToken] Token type is {0}", token.TokenType);
			}
			httpContext.Items["Item-CommonAccessToken"] = token;
			return true;
		}

		private bool IsTokenSerializationAllowed(WindowsIdentity windowsIdentity)
		{
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				bool flag = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3053858109U, ref flag);
				if (flag)
				{
					return true;
				}
			}
			if (windowsIdentity == null || windowsIdentity.User == null)
			{
				return false;
			}
			SecurityIdentifier user = windowsIdentity.User;
			try
			{
				BackendRehydrationModule.syncLock.EnterReadLock();
				if (BackendRehydrationModule.verifiedCallers.Contains(user))
				{
					return true;
				}
			}
			finally
			{
				try
				{
					BackendRehydrationModule.syncLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			bool flag2 = false;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(windowsIdentity))
			{
				flag2 = LocalServer.AllowsTokenSerializationBy(clientSecurityContext);
			}
			if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string, SecurityIdentifier, bool>((long)this.GetHashCode(), "[BackendRehydrationModule::ProcessRequest] Verified token serialization rights for {0} with SID {1}. Result = {2}", BackendRehydrationModule.GetCheapIdentityName(windowsIdentity), user, flag2);
			}
			if (flag2)
			{
				try
				{
					BackendRehydrationModule.syncLock.EnterWriteLock();
					BackendRehydrationModule.verifiedCallers.Add(user);
				}
				finally
				{
					try
					{
						BackendRehydrationModule.syncLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			return flag2;
		}

		private static string GetCheapIdentityName(IIdentity identity)
		{
			if (identity == null)
			{
				return null;
			}
			WindowsIdentity windowsIdentity = identity as WindowsIdentity;
			if (windowsIdentity == null)
			{
				return identity.GetSafeName(true);
			}
			if (ExTraceGlobals.BackendRehydrationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return identity.GetSafeName(true);
			}
			if (windowsIdentity.User != null)
			{
				return windowsIdentity.User.ToString();
			}
			return windowsIdentity.ToString();
		}

		public const string RehydrationModuleKey = "TotalBERehydrationModuleLatency";

		public const string RehydrateLatencyKey = "BERehydrationLatency";

		public const string ValidateCATRightsLatencyKey = "BEValidateCATRightsLatency";

		public const string CATDeserializationLatencyKey = "CATDeserializationLatency";

		public const string BackendAuthenticatorNameKey = "BackEndAuthenticator";

		public const string BackendAuthenticationIdentifierNameKey = "BEAuthIdentifier";

		public static readonly string EventSourceName = "MsExchange BackEndRehydration";

		private static HashSet<SecurityIdentifier> verifiedCallers = new HashSet<SecurityIdentifier>();

		private static ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

		private static LazyMember<ExEventLog> eventLogger = new LazyMember<ExEventLog>(() => new ExEventLog(ExTraceGlobals.AuthenticationTracer.Category, BackendRehydrationModule.EventSourceName));

		private static readonly GenericPrincipal defaultSecurityPrincipal = new GenericPrincipal(new GenericIdentity("Anonymous"), Array<string>.Empty);
	}
}
