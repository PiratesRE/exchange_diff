using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaServerLogger : ExtensibleLogger
	{
		public OwaServerLogger() : base(new OwaServerLogConfiguration())
		{
			ActivityContext.RegisterMetadata(typeof(OwaServerLogger.LoggerData));
		}

		public static void Initialize()
		{
			if (OwaServerLogger.instance == null)
			{
				OwaServerLogger.instance = new OwaServerLogger();
			}
		}

		public static void AppendToLog(ILogEvent logEvent)
		{
			if (OwaServerLogger.instance != null)
			{
				OwaServerLogger.instance.LogEvent(logEvent);
			}
		}

		internal static void LogUserContextData(HttpContext httpContext, RequestDetailsLogger logger)
		{
			if (OwaServerLogger.instance == null || !OwaServerLogger.instance.Configuration.IsLoggingEnabled)
			{
				return;
			}
			IMailboxContext mailboxContext = UserContextManager.GetMailboxContext(httpContext, null, false);
			if (mailboxContext != null && mailboxContext.ExchangePrincipal != null)
			{
				if (mailboxContext.IsExplicitLogon && mailboxContext.LogonIdentity != null)
				{
					logger.Set(OwaServerLogger.LoggerData.User, mailboxContext.LogonIdentity.PrimarySmtpAddress);
				}
				logger.Set(OwaServerLogger.LoggerData.PrimarySmtpAddress, mailboxContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress);
				logger.Set(OwaServerLogger.LoggerData.MailboxGuid, mailboxContext.ExchangePrincipal.MailboxInfo.MailboxGuid);
				logger.Set(OwaServerLogger.LoggerData.RecipientType, mailboxContext.ExchangePrincipal.RecipientTypeDetails);
				Guid tenantGuid = mailboxContext.ExchangePrincipal.MailboxInfo.OrganizationId.GetTenantGuid();
				if (tenantGuid != Guid.Empty)
				{
					logger.Set(OwaServerLogger.LoggerData.TenantGuid, tenantGuid);
				}
				string text = mailboxContext.Key.UserContextId.ToString(CultureInfo.InvariantCulture);
				logger.Set(OwaServerLogger.LoggerData.UserContext, text);
				if (!OwaServerLogger.TryAppendToIISLog(httpContext.Response, "&{0}={1}", new object[]
				{
					UserContextCookie.UserContextCookiePrefix,
					text
				}))
				{
					ExTraceGlobals.RequestTracer.TraceWarning<Guid, string>((long)httpContext.GetHashCode(), "RequestId: {0}; Error appending UserContext '{1}' to IIS log.", logger.ActivityScope.ActivityId, text);
				}
				UserContext userContext = mailboxContext as UserContext;
				if (userContext != null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.LogTenantInfo.Enabled)
				{
					if (!string.IsNullOrEmpty(userContext.LogEventCommonData.TenantDomain))
					{
						OwaServerLogger.TryAppendToIISLog(httpContext.Response, "&{0}={1}", new object[]
						{
							"domain",
							userContext.LogEventCommonData.TenantDomain
						});
						logger.Set(OwaServerLogger.LoggerData.Tenant, userContext.LogEventCommonData.TenantDomain);
					}
					if (userContext.IsBposUser && !string.IsNullOrEmpty(userContext.BposSkuCapability))
					{
						OwaServerLogger.TryAppendToIISLog(httpContext.Response, "&{0}={1}", new object[]
						{
							"bpossku",
							userContext.BposSkuCapability
						});
						logger.Set(OwaServerLogger.LoggerData.ServicePlan, userContext.BposSkuCapability);
					}
					if (!string.IsNullOrEmpty(userContext.LogEventCommonData.Flights))
					{
						logger.Set(OwaServerLogger.LoggerData.Flights, userContext.LogEventCommonData.Flights);
					}
					if (!string.IsNullOrEmpty(userContext.LogEventCommonData.Features))
					{
						logger.Set(OwaServerLogger.LoggerData.Features, userContext.LogEventCommonData.Features);
					}
					if (userContext.UserCulture != null && !string.IsNullOrEmpty(userContext.UserCulture.Name))
					{
						logger.Set(OwaServerLogger.LoggerData.UserContextCulture, userContext.UserCulture.Name);
					}
				}
			}
			UserContextStatistics userContextStatistics = UserContextManager.GetUserContextStatistics(httpContext);
			if (userContextStatistics != null)
			{
				logger.Set(OwaServerLogger.LoggerData.UserContextLatency, userContextStatistics.AcquireLatency);
				logger.Set(OwaServerLogger.LoggerData.UserContextCreated, userContextStatistics.Created ? 1 : 0);
			}
		}

		internal static void LogHttpContextData(HttpContext httpContext, RequestDetailsLogger logger)
		{
			if (OwaServerLogger.instance == null || !OwaServerLogger.instance.Configuration.IsLoggingEnabled)
			{
				return;
			}
			if (logger.Get(ExtensibleLoggerMetadata.EventId) == null)
			{
				if (!logger.ActivityScope.Statistics.Any<KeyValuePair<OperationKey, OperationStatistics>>())
				{
					return;
				}
				OwaServerLogger.SetEventId(httpContext, logger);
			}
			logger.Set(OwaServerLogger.LoggerData.ContentLength, httpContext.Request.ContentLength);
			logger.Set(ServiceLatencyMetadata.HttpPipelineLatency, httpContext.Items[ServiceLatencyMetadata.HttpPipelineLatency]);
			NameValueCollection headers = httpContext.Request.Headers;
			string value = headers["X-OWA-ActionId"];
			if (!string.IsNullOrEmpty(value))
			{
				logger.Set(OwaServerLogger.LoggerData.ClientActionId, value);
			}
			string value2 = headers["X-OWA-ActionName"];
			if (!string.IsNullOrEmpty(value2))
			{
				logger.Set(OwaServerLogger.LoggerData.ClientActionName, value2);
			}
			string value3 = headers["X-EXT-ClientName"];
			if (!string.IsNullOrEmpty(value3))
			{
				logger.Set(OwaServerLogger.LoggerData.ExternalClientName, value3);
			}
			string value4 = headers["X-EXT-CorrelationId"];
			if (!string.IsNullOrEmpty(value4))
			{
				logger.Set(OwaServerLogger.LoggerData.ExternalCorrelationId, value4);
			}
			string sourceCafeServer = CafeHelper.GetSourceCafeServer(httpContext.Request);
			if (!string.IsNullOrEmpty(sourceCafeServer))
			{
				logger.Set(OwaServerLogger.LoggerData.FrontEndServer, sourceCafeServer);
			}
			string value5 = headers["X-OWA-OfflineRejectCode"];
			if (!string.IsNullOrEmpty(value5))
			{
				logger.Set(OwaServerLogger.LoggerData.OfflineRejectCode, value5);
			}
			string text = headers["logonLatency"];
			bool flag = UserContextUtilities.IsDifferentMailbox(httpContext);
			if (!string.IsNullOrEmpty(text) || flag)
			{
				IMailboxContext mailboxContext = UserContextManager.GetMailboxContext(httpContext, null, false);
				if (!string.IsNullOrEmpty(text))
				{
					logger.Set(OwaServerLogger.LoggerData.LogonLatencyName, text);
					string userContext = string.Empty;
					if (mailboxContext != null)
					{
						userContext = mailboxContext.Key.UserContextId.ToString(CultureInfo.InvariantCulture);
					}
					string[] keys = new string[]
					{
						"LGN.L"
					};
					string[] values = new string[]
					{
						text
					};
					Datapoint datapoint = new Datapoint(DatapointConsumer.Analytics, "LogonLatency", DateTime.UtcNow.ToString("o"), keys, values);
					ClientLogEvent logEvent = new ClientLogEvent(datapoint, userContext);
					OwaClientLogger.AppendToLog(logEvent);
				}
				if (flag && mailboxContext is SharedContext && httpContext.Items.Contains("CallContext"))
				{
					CallContext callContext = httpContext.Items["CallContext"] as CallContext;
					logger.Set(OwaServerLogger.LoggerData.User, callContext.GetEffectiveAccessingSmtpAddress());
				}
			}
			string value6 = httpContext.Items["BackEndAuthenticator"] as string;
			if (!string.IsNullOrEmpty(value6))
			{
				logger.Set(OwaServerLogger.LoggerData.BackendAuthenticator, value6);
			}
			object obj = httpContext.Items["TotalBERehydrationModuleLatency"];
			if (obj != null)
			{
				logger.Set(OwaServerLogger.LoggerData.RehydrationModuleLatency, obj);
			}
			string value7 = headers["X-OWA-Test-PassThruProxy"];
			if (!string.IsNullOrEmpty(value7))
			{
				logger.Set(OwaServerLogger.LoggerData.PassThroughProxy, value7);
			}
			string value8 = headers["X-SuiteServiceProxyOrigin"];
			if (!string.IsNullOrEmpty(value8))
			{
				logger.Set(OwaServerLogger.LoggerData.SuiteServiceProxyOrigin, value8);
			}
			HttpCookie httpCookie = httpContext.Request.Cookies["ClientId"];
			if (httpCookie != null)
			{
				logger.Set(OwaServerLogger.LoggerData.ClientId, httpCookie.Value);
			}
		}

		protected override ICollection<KeyValuePair<string, object>> GetComponentSpecificData(IActivityScope activityScope, string eventId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(20);
			IEnumerable<KeyValuePair<string, object>> formattableMetadata = activityScope.GetFormattableMetadata(OwsLogRegistry.GetRegisteredValues(eventId));
			foreach (KeyValuePair<string, object> keyValuePair in formattableMetadata)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			ExtensibleLogger.CopyPIIProperty(activityScope, dictionary, OwaServerLogger.LoggerData.PrimarySmtpAddress, "PSA");
			ExtensibleLogger.CopyPIIProperty(activityScope, dictionary, OwaServerLogger.LoggerData.User, "user");
			ExtensibleLogger.CopyProperties(activityScope, dictionary, OwaServerLogger.EnumToShortKeyMapping);
			if (Globals.LogErrorDetails)
			{
				ExtensibleLogger.CopyProperty(activityScope, dictionary, ServiceCommonMetadata.GenericErrors, "ErrInfo");
				string property = activityScope.GetProperty(ServiceCommonMetadata.ErrorCode);
				if ((!string.IsNullOrEmpty(activityScope.GetProperty(ServiceCommonMetadata.GenericErrors)) || (!string.IsNullOrEmpty(property) && property != "Success" && property != "0")) && HttpContext.Current != null)
				{
					string key = OwaServerLogger.EnumToShortKeyMapping[OwaServerLogger.LoggerData.UserAgent];
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, HttpContext.Current.Request.UserAgent);
					}
				}
			}
			return dictionary;
		}

		internal static void LogWcfLatency(HttpContext httpContext)
		{
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			int num = (int)httpContext.Items[ServiceLatencyMetadata.HttpPipelineLatency];
			getRequestDetailsLogger.Set(OwaServerLogger.LoggerData.WcfLatency, (int)getRequestDetailsLogger.ActivityScope.TotalMilliseconds - num);
		}

		protected override bool IsInterestingEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			return base.IsInterestingEvent(activityScope, eventType) && (activityScope.GetProperty(ExtensibleLoggerMetadata.EventId) != null || activityScope.Statistics.Any<KeyValuePair<OperationKey, OperationStatistics>>());
		}

		private static void SetEventId(HttpContext httpContext, RequestDetailsLogger logger)
		{
			RequestContext requestContext = RequestContext.Get(httpContext);
			string value;
			if (requestContext == null)
			{
				value = "OwaRequest";
			}
			else if (requestContext.RequestType == OwaRequestType.ServiceRequest)
			{
				value = (httpContext.Request.Headers[OWADispatchOperationSelector.Action] ?? requestContext.RequestType.ToString());
			}
			else
			{
				value = requestContext.RequestType.ToString();
			}
			logger.Set(ExtensibleLoggerMetadata.EventId, value);
			logger.Set(OwaServerLogger.LoggerData.ClientActionName, httpContext.Request.CurrentExecutionFilePath.ToLower());
		}

		private static bool TryAppendToIISLog(HttpResponse response, string format, params object[] args)
		{
			string param = string.Format(format, args);
			try
			{
				response.AppendToLog(param);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		internal const string PrimarySmtpAddressKey = "PSA";

		private const string UserKey = "user";

		private const string OfflineRejectCodeKey = "X-OWA-OfflineRejectCode";

		private const string ExternalClientNameKey = "X-EXT-ClientName";

		private const string ExternalCorrelationIdKey = "X-EXT-CorrelationId";

		private const string LogonLatencyHeaderName = "logonLatency";

		private const string BposSku = "bpossku";

		private const string DomainName = "domain";

		private const string ErrorCodeSuccess = "Success";

		private const string ErrorCodeSuccessAsZero = "0";

		public static readonly Dictionary<Enum, string> EnumToShortKeyMapping = new Dictionary<Enum, string>
		{
			{
				OwaServerLogger.LoggerData.MailboxGuid,
				"MG"
			},
			{
				OwaServerLogger.LoggerData.TenantGuid,
				"TG"
			},
			{
				OwaServerLogger.LoggerData.UserContext,
				UserContextCookie.UserContextCookiePrefix
			},
			{
				OwaServerLogger.LoggerData.CanaryStatus,
				"CN.S"
			},
			{
				OwaServerLogger.LoggerData.CanaryValidationBegin,
				"CN.B"
			},
			{
				OwaServerLogger.LoggerData.CanaryValidationEnd,
				"CN.E"
			},
			{
				OwaServerLogger.LoggerData.CanaryCreationTime,
				"CN.T"
			},
			{
				OwaServerLogger.LoggerData.CanaryLogData,
				"CN.L"
			},
			{
				OwaServerLogger.LoggerData.ClientActionId,
				"ActionId"
			},
			{
				OwaServerLogger.LoggerData.ClientActionName,
				"CAN"
			},
			{
				OwaServerLogger.LoggerData.BackendAuthenticator,
				"BEA"
			},
			{
				OwaServerLogger.LoggerData.GetOWAMiniRecipientBegin,
				"GOM.B"
			},
			{
				OwaServerLogger.LoggerData.GetOWAMiniRecipientEnd,
				"GOM.E"
			},
			{
				OwaServerLogger.LoggerData.RehydrationModuleLatency,
				"RHML"
			},
			{
				OwaServerLogger.LoggerData.OnPostAuthorizeRequestBegin,
				"PST.B"
			},
			{
				OwaServerLogger.LoggerData.OnPostAuthorizeRequestEnd,
				"PST.E"
			},
			{
				OwaServerLogger.LoggerData.OnPostAuthorizeRequestLatency,
				"PST.L"
			},
			{
				OwaServerLogger.LoggerData.OnPostAuthorizeRequestLatencyDetails,
				"PST.D"
			},
			{
				OwaServerLogger.LoggerData.OwaMessageInspectorReceiveRequestBegin,
				"OMB.B"
			},
			{
				OwaServerLogger.LoggerData.OwaMessageInspectorReceiveRequestEnd,
				"OMB.E"
			},
			{
				OwaServerLogger.LoggerData.OwaMessageInspectorEndRequestBegin,
				"OME.B"
			},
			{
				OwaServerLogger.LoggerData.OwaMessageInspectorEndRequestEnd,
				"OME.E"
			},
			{
				OwaServerLogger.LoggerData.OwaRequestPipelineLatency,
				"ORP.L"
			},
			{
				OwaServerLogger.LoggerData.UserContextLoadBegin,
				"UCL.B"
			},
			{
				OwaServerLogger.LoggerData.UserContextLoadEnd,
				"UCL.E"
			},
			{
				OwaServerLogger.LoggerData.UserContextLatency,
				"UC.L"
			},
			{
				OwaServerLogger.LoggerData.UserContextCreated,
				"UC.C"
			},
			{
				OwaServerLogger.LoggerData.UserContextCulture,
				"UC.CUL"
			},
			{
				OwaServerLogger.LoggerData.IsMowaClient,
				"Mowa"
			},
			{
				OwaServerLogger.LoggerData.IsOfflineEnabled,
				"Off"
			},
			{
				OwaServerLogger.LoggerData.ContentLength,
				"CNTL"
			},
			{
				OwaServerLogger.LoggerData.CorrelationId,
				"CorrelationID"
			},
			{
				OwaServerLogger.LoggerData.FrontEndServer,
				"FE"
			},
			{
				OwaServerLogger.LoggerData.WcfLatency,
				"WCF"
			},
			{
				OwaServerLogger.LoggerData.OfflineRejectCode,
				"ORC"
			},
			{
				OwaServerLogger.LoggerData.Tenant,
				"DOM"
			},
			{
				OwaServerLogger.LoggerData.AppCache,
				"AC"
			},
			{
				OwaServerLogger.LoggerData.ServicePlan,
				"SKU"
			},
			{
				OwaServerLogger.LoggerData.LogonLatencyName,
				"LGN.L"
			},
			{
				ServiceCommonMetadata.ErrorCode,
				"Err"
			},
			{
				ServiceCommonMetadata.IsDuplicatedAction,
				"Dup"
			},
			{
				BudgetMetadata.BeginBudgetConnections,
				"BMD.BBC"
			},
			{
				BudgetMetadata.EndBudgetConnections,
				"BMD.EBC"
			},
			{
				BudgetMetadata.BeginBudgetHangingConnections,
				"BMD.BBHC"
			},
			{
				BudgetMetadata.EndBudgetHangingConnections,
				"BMD.EBHC"
			},
			{
				BudgetMetadata.BeginBudgetAD,
				"BMD.BBAD"
			},
			{
				BudgetMetadata.EndBudgetAD,
				"BMD.EBAD"
			},
			{
				BudgetMetadata.BeginBudgetCAS,
				"BMD.BBCAS"
			},
			{
				BudgetMetadata.EndBudgetCAS,
				"BMD.EBCAS"
			},
			{
				BudgetMetadata.BeginBudgetRPC,
				"BMD.BBRPC"
			},
			{
				BudgetMetadata.EndBudgetRPC,
				"BMD.EBRPC"
			},
			{
				BudgetMetadata.BeginBudgetFindCount,
				"BMD.BBFC"
			},
			{
				BudgetMetadata.EndBudgetFindCount,
				"BMD.EBFC"
			},
			{
				BudgetMetadata.BeginBudgetSubscriptions,
				"BMD.BBS"
			},
			{
				BudgetMetadata.EndBudgetSubscriptions,
				"BMD.EBS"
			},
			{
				BudgetMetadata.ThrottlingDelay,
				"Thr"
			},
			{
				BudgetMetadata.ThrottlingRequestType,
				"BMD.TRT"
			},
			{
				ServiceTaskMetadata.ADCount,
				"CmdAD.C"
			},
			{
				ServiceTaskMetadata.ADLatency,
				"CmdAD.L"
			},
			{
				ServiceTaskMetadata.RpcCount,
				"CmdRPC.C"
			},
			{
				ServiceTaskMetadata.RpcLatency,
				"CmdRPC.L"
			},
			{
				ServiceTaskMetadata.WatsonReportCount,
				"CmdWR.C"
			},
			{
				ServiceTaskMetadata.ServiceCommandBegin,
				"SCmd.B"
			},
			{
				ServiceTaskMetadata.ServiceCommandEnd,
				"SCmd.E"
			},
			{
				ServiceLatencyMetadata.CoreExecutionLatency,
				"CmdT"
			},
			{
				ServiceLatencyMetadata.RecipientLookupLatency,
				"RLL"
			},
			{
				ServiceLatencyMetadata.ExchangePrincipalLatency,
				"EPL"
			},
			{
				ServiceLatencyMetadata.HttpPipelineLatency,
				"HPL"
			},
			{
				OwaServerLogger.LoggerData.CallContextInitBegin,
				"CC.B"
			},
			{
				OwaServerLogger.LoggerData.CallContextInitEnd,
				"CC.E"
			},
			{
				ServiceLatencyMetadata.CallContextInitLatency,
				"CC.L"
			},
			{
				ServiceLatencyMetadata.PreExecutionLatency,
				"PreL"
			},
			{
				OwaServerLogger.LoggerData.Flights,
				"FLT"
			},
			{
				OwaServerLogger.LoggerData.Features,
				"FTR"
			},
			{
				OwaServerLogger.LoggerData.ExternalClientName,
				"ECN"
			},
			{
				OwaServerLogger.LoggerData.ExternalCorrelationId,
				"ECI"
			},
			{
				OwaServerLogger.LoggerData.PassThroughProxy,
				"PTP"
			},
			{
				OwaServerLogger.LoggerData.IsRequest,
				"Rqt"
			},
			{
				OwaServerLogger.LoggerData.SuiteServiceProxyOrigin,
				"SSPOrigin"
			},
			{
				OwaServerLogger.LoggerData.RequestStartTime,
				"RQST"
			},
			{
				OwaServerLogger.LoggerData.RequestEndTime,
				"RQET"
			},
			{
				OwaServerLogger.LoggerData.UserAgent,
				"UA"
			},
			{
				OwaServerLogger.LoggerData.ClientBuildVersion,
				"cbld"
			},
			{
				OwaServerLogger.LoggerData.RequestVersion,
				"ReqVer"
			},
			{
				OwaServerLogger.LoggerData.RecipientType,
				"RecT"
			},
			{
				OwaServerLogger.LoggerData.UsingWcfHttpHandler,
				"WCFH"
			},
			{
				OwaServerLogger.LoggerData.ClientId,
				"ClientId"
			}
		};

		private static OwaServerLogger instance;

		internal enum LoggerData
		{
			UserContext,
			PrimarySmtpAddress,
			Tenant,
			ServicePlan,
			LogonLatencyName,
			MailboxGuid,
			TenantGuid,
			CallContextInitBegin,
			CallContextInitEnd,
			CanaryStatus,
			CanaryCreationTime,
			CanaryValidationBegin,
			CanaryValidationEnd,
			CanaryLogData,
			ClientActionId,
			ClientActionName,
			AppCache,
			BackendAuthenticator,
			RehydrationModuleLatency,
			IsMowaClient,
			IsOfflineEnabled,
			ContentLength,
			GetOWAMiniRecipientBegin,
			GetOWAMiniRecipientEnd,
			CorrelationId,
			FrontEndServer,
			OwaMessageInspectorReceiveRequestBegin,
			OwaMessageInspectorReceiveRequestEnd,
			OwaMessageInspectorEndRequestBegin,
			OwaMessageInspectorEndRequestEnd,
			OnPostAuthorizeRequestBegin,
			OnPostAuthorizeRequestEnd,
			OnPostAuthorizeRequestLatency,
			OnPostAuthorizeRequestLatencyDetails,
			OwaRequestPipelineLatency,
			UserContextLoadBegin,
			UserContextLoadEnd,
			UserContextLatency,
			UserContextCreated,
			WcfLatency,
			OfflineRejectCode,
			Flights,
			Features,
			ExternalClientName,
			ExternalCorrelationId,
			MailboxSessionDuration,
			MailboxSessionLockTime,
			UserContextDipose1,
			UserContextDipose2,
			UserContextDipose3,
			UserContextDipose4,
			PassThroughProxy,
			IsRequest,
			SuiteServiceProxyOrigin,
			RequestStartTime,
			RequestEndTime,
			UserAgent,
			ClientBuildVersion,
			RequestVersion,
			RecipientType,
			UsingWcfHttpHandler,
			User,
			ClientId,
			UserContextCulture
		}
	}
}
