using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RmsClientManagerLog
	{
		public static void Start()
		{
			Server localServer = null;
			RmsClientManagerLog.rmsLogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Rms Client Manager Log", RmsClientManagerLog.Fields);
			RmsClientManagerLog.rmsLog = new Log(RmsClientManagerUtils.GetUniqueFileNameForProcess(RmsClientManagerLog.LogSuffix, true), new LogHeaderFormatter(RmsClientManagerLog.rmsLogSchema), "RmsClientManagerLog");
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 254, "Start", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\rightsmanagement\\RmsClientManagerLog.cs");
				localServer = topologyConfigurationSession.FindLocalServer();
				RmsClientManagerLog.notificationCookie = ADNotificationAdapter.RegisterChangeNotification<Server>(localServer.Id, new ADNotificationCallback(RmsClientManagerLog.HandleConfigurationChange));
			});
			if (!adoperationResult.Succeeded)
			{
				RmsClientManagerLog.Tracer.TraceError<Exception>(0L, "Failed to get the local server.  Unable to load the log configuration. Error {0}", adoperationResult.Exception);
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, adoperationResult.Exception);
			}
			RmsClientManagerLog.Configure(localServer);
		}

		public static void Stop()
		{
			if (RmsClientManagerLog.notificationCookie != null)
			{
				try
				{
					ADNotificationAdapter.UnregisterChangeNotification(RmsClientManagerLog.notificationCookie);
					if (RmsClientManagerLog.rmsLog != null)
					{
						RmsClientManagerLog.rmsLog.Close();
					}
				}
				catch (ObjectDisposedException)
				{
				}
				RmsClientManagerLog.notificationCookie = null;
			}
		}

		public static void Configure(Server serverConfig)
		{
			if (serverConfig == null)
			{
				return;
			}
			RmsClientManagerLog.rmsLogEnabled = serverConfig.IrmLogEnabled;
			if (RmsClientManagerLog.rmsLogEnabled)
			{
				if (serverConfig.IrmLogPath == null || string.IsNullOrEmpty(serverConfig.IrmLogPath.PathName))
				{
					RmsClientManagerLog.rmsLogEnabled = false;
					RmsClientManagerLog.Tracer.TraceError(0L, "Rms Client Manager Log is enabled, but the log path is empty.");
					return;
				}
				if (RmsClientManagerLog.rmsLog != null)
				{
					RmsClientManagerLog.rmsLog.Configure(serverConfig.IrmLogPath.PathName, serverConfig.IrmLogMaxAge, (long)(serverConfig.IrmLogMaxDirectorySize.IsUnlimited ? 0UL : serverConfig.IrmLogMaxDirectorySize.Value.ToBytes()), (long)serverConfig.IrmLogMaxFileSize.ToBytes());
				}
			}
		}

		public static void LogUriEvent(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerLog.RmsClientManagerEvent clientManagerEvent, RmsClientManagerContext context, Uri serverUrl)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(clientManagerFeature, clientManagerEvent, serverUrl, context.OrgId, context.TransactionId, context.ContextStringForm);
		}

		public static void LogAcquireRmsTemplateResult(RmsClientManagerContext context, RmsTemplate template)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Template, RmsClientManagerLog.RmsClientManagerEvent.Success, context.OrgId, context.TransactionId, template.Id.ToString(), context.ContextStringForm);
		}

		public static void LogAcquireRmsTemplateResult(RmsClientManagerContext context, Uri serverUrl, LinkedList<RmsTemplate> templates)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (RmsTemplate rmsTemplate in templates)
			{
				if (!flag)
				{
					stringBuilder.AppendFormat("; {0}", rmsTemplate.Id.ToString());
				}
				else
				{
					stringBuilder.Append(rmsTemplate.Id.ToString());
					flag = false;
				}
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Template, RmsClientManagerLog.RmsClientManagerEvent.Success, serverUrl, context.OrgId, context.TransactionId, stringBuilder.ToString(), context.ContextStringForm);
		}

		public static void LogAcquireRmsTemplateCacheResult(RmsClientManagerContext context, Guid key)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Template, RmsClientManagerLog.RmsClientManagerEvent.Success, RmsClientManagerLog.DummyUriForTemplateCache, context.OrgId, context.TransactionId, key.ToString(), context.ContextStringForm);
		}

		public static void LogAcquireServerInfoResult(RmsClientManagerContext context, ExternalRMSServerInfo serverInfo)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (serverInfo == null)
			{
				RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.ServerInfo, RmsClientManagerLog.RmsClientManagerEvent.Success, context.OrgId, context.TransactionId, RmsClientManagerLog.ServerInfoNotFound, context.ContextStringForm);
				return;
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.ServerInfo, RmsClientManagerLog.RmsClientManagerEvent.Success, context.OrgId, context.TransactionId, string.Format(CultureInfo.InvariantCulture, "KeyUri: {0}; CertificationWSPipeline: {1}; CertificationWSTargetUri: {2}; ServerLicensingWSPipeline: {3}; ServerLicensingWSTargetUri: {4}; ExpiryTime: {5}", new object[]
			{
				(serverInfo.KeyUri == null) ? RmsClientManagerLog.ServerInfoUriNull : serverInfo.KeyUri.ToString(),
				(serverInfo.CertificationWSPipeline == null) ? RmsClientManagerLog.ServerInfoUriNull : serverInfo.CertificationWSPipeline.ToString(),
				(serverInfo.CertificationWSTargetUri == null) ? RmsClientManagerLog.ServerInfoUriNull : serverInfo.CertificationWSTargetUri.ToString(),
				(serverInfo.ServerLicensingWSPipeline == null) ? RmsClientManagerLog.ServerInfoUriNull : serverInfo.ServerLicensingWSPipeline.ToString(),
				(serverInfo.ServerLicensingWSTargetUri == null) ? RmsClientManagerLog.ServerInfoUriNull : serverInfo.ServerLicensingWSTargetUri.ToString(),
				serverInfo.ExpiryTime.Ticks.ToString()
			}), context.ContextStringForm);
		}

		public static void LogAcquireUseLicense(RmsClientManagerContext context, Uri serverUrl, string user)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.UseLicense, RmsClientManagerLog.RmsClientManagerEvent.Acquire, serverUrl, context.OrgId, context.TransactionId, user, context.ContextStringForm);
		}

		public static void LogAcquirePrelicense(RmsClientManagerContext context, Uri serverUrl, string[] identities)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			foreach (string data in identities)
			{
				RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Prelicense, RmsClientManagerLog.RmsClientManagerEvent.Acquire, serverUrl, context.OrgId, context.TransactionId, data, context.ContextStringForm);
			}
		}

		public static void LogAcquirePrelicenseResult(RmsClientManagerContext context, LicenseResponse[] responses)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (responses == null || responses.Length == 0)
			{
				RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Prelicense, RmsClientManagerLog.RmsClientManagerEvent.Success, context.TransactionId, RmsClientManagerLog.PrelicenseNoResult, context.ContextStringForm);
				return;
			}
			foreach (LicenseResponse licenseResponse in responses)
			{
				if (licenseResponse.Exception != null)
				{
					RmsClientManagerLog.LogException(RmsClientManagerLog.RmsClientManagerFeature.Prelicense, context, licenseResponse.Exception);
				}
				else
				{
					RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.Prelicense, RmsClientManagerLog.RmsClientManagerEvent.Success, context.TransactionId, licenseResponse.UsageRights.ToString(), context.ContextStringForm);
				}
			}
		}

		public static void LogAcquireUseLicenseResult(RmsClientManagerContext context, string useLicense)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.UseLicense, RmsClientManagerLog.RmsClientManagerEvent.Success, context.TransactionId, string.IsNullOrEmpty(useLicense) ? RmsClientManagerLog.UseLicenseEmpty : RmsClientManagerLog.UseLicenseExists, context.ContextStringForm);
		}

		public static void LogAcquireRac(RmsClientManagerContext context, Uri serverUrl)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Acquire, serverUrl, context.OrgId, context.TransactionId, RmsClientManagerLog.AcquireServerRac, context.ContextStringForm);
		}

		public static void LogAcquireClc(RmsClientManagerContext context, Uri serverUrl)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Acquire, serverUrl, context.OrgId, context.TransactionId, RmsClientManagerLog.AcquireServerClc);
		}

		public static void LogAcquireRacClc(RmsClientManagerContext context)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Acquire, context.OrgId, context.TransactionId, RmsClientManagerLog.AcquireServerRacClc, context.ContextStringForm);
		}

		public static void LogAcquireRacClcResult(RmsClientManagerContext context, TenantLicensePair tenantLicensePair)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Success, context.OrgId, context.TransactionId, string.Format(CultureInfo.InvariantCulture, "CLC: {0}; RAC: {1}; Version: {2}", new object[]
			{
				(tenantLicensePair.BoundLicenseClc == null) ? RmsClientManagerLog.LicenseEmpty : RmsClientManagerLog.LicenseExists,
				(tenantLicensePair.Rac == null) ? RmsClientManagerLog.LicenseEmpty : RmsClientManagerLog.LicenseExists,
				tenantLicensePair.Version
			}).ToString(NumberFormatInfo.InvariantInfo), context.ContextStringForm);
		}

		public static void LogAcquireRacClcCacheResult(RmsClientManagerContext context, Uri serviceLocation, Uri publishingLocation, byte version)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.RacClc, RmsClientManagerLog.RmsClientManagerEvent.Success, RmsClientManagerLog.DummyUriForRacClcCache, context.OrgId, context.TransactionId, string.Format(CultureInfo.InvariantCulture, "Service Location: {0}; Publishing Location: {1}; Version: {2}", new object[]
			{
				serviceLocation.ToString(),
				publishingLocation.ToString(),
				version.ToString(NumberFormatInfo.InvariantInfo)
			}), context.ContextStringForm);
		}

		public static void LogVerifySignatureResult(RmsClientManagerContext context, string userIdentity)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.SignatureVerification, RmsClientManagerLog.RmsClientManagerEvent.Success, context.OrgId, context.TransactionId, userIdentity, context.ContextStringForm);
		}

		public static void LogDrmInitialization(int machineCertIndex)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			RmsClientManagerContext rmsClientManagerContext = new RmsClientManagerContext(OrganizationId.ForestWideOrgId, null);
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.DrmInitialization, RmsClientManagerLog.RmsClientManagerEvent.Success, default(Guid), string.Format(CultureInfo.InvariantCulture, "Selected machine certificate index: {0}", new object[]
			{
				machineCertIndex.ToString(NumberFormatInfo.InvariantInfo)
			}), rmsClientManagerContext.ContextStringForm);
		}

		public static void LogDrmInitialization(Uri rmsServerUri)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (rmsServerUri == null)
			{
				throw new ArgumentNullException("rmsServerUri");
			}
			RmsClientManagerContext rmsClientManagerContext = new RmsClientManagerContext(OrganizationId.ForestWideOrgId, null);
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.DrmInitialization, RmsClientManagerLog.RmsClientManagerEvent.Success, default(Guid), string.Format(CultureInfo.InvariantCulture, "RMS Server queried for active crypto mode: {0}", new object[]
			{
				rmsServerUri.ToString()
			}), rmsClientManagerContext.ContextStringForm);
		}

		public static void LogDrmInitialization(DRMClientVersionInfo msdrmVersionInfo)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (msdrmVersionInfo == null)
			{
				throw new ArgumentNullException("msdrmVersionInfo");
			}
			RmsClientManagerContext rmsClientManagerContext = new RmsClientManagerContext(OrganizationId.ForestWideOrgId, null);
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.DrmInitialization, RmsClientManagerLog.RmsClientManagerEvent.Success, default(Guid), string.Format(CultureInfo.InvariantCulture, "MSDRM.DLL version: {0}.{1}.{2}.{3}", new object[]
			{
				msdrmVersionInfo.V1.ToString(),
				msdrmVersionInfo.V2.ToString(),
				msdrmVersionInfo.V3.ToString(),
				msdrmVersionInfo.V4.ToString()
			}), rmsClientManagerContext.ContextStringForm);
		}

		public static void LogUrlMalFormatException(UriFormatException ex, string key, string originalUri)
		{
			StringBuilder stringBuilder = null;
			if (ex != null)
			{
				stringBuilder = new StringBuilder(ex.Message.Length + ex.GetType().Name.Length + 3);
				stringBuilder.AppendFormat("{0} [{1}]", ex.Message, ex.GetType().Name);
				if (ex.InnerException != null)
				{
					stringBuilder.AppendFormat("; Inner Exception: {0} [{1}]", ex.InnerException.Message, ex.InnerException.GetType().Name);
				}
			}
			string data = string.Format("RMSO connector Url is a malformat url, regkey value: {0}, originalUri: {1}", key, originalUri);
			RmsClientManagerLog.LogEvent(RmsClientManagerLog.RmsClientManagerFeature.ServerInfo, RmsClientManagerLog.RmsClientManagerEvent.Exception, default(Guid), data, stringBuilder.ToString());
		}

		public static void LogException(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerContext context, Exception ex)
		{
			if (!RmsClientManagerLog.rmsLogEnabled)
			{
				return;
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			StringBuilder stringBuilder = null;
			if (ex != null)
			{
				stringBuilder = new StringBuilder(ex.Message.Length + ex.GetType().Name.Length + 3);
				stringBuilder.AppendFormat("{0} [{1}]", ex.Message, ex.GetType().Name);
				if (ex.InnerException != null)
				{
					stringBuilder.AppendFormat("; Inner Exception: {0} [{1}]", ex.InnerException.Message, ex.InnerException.GetType().Name);
				}
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(RmsClientManagerLog.rmsLogSchema);
			logRowFormatter[1] = clientManagerFeature;
			logRowFormatter[2] = RmsClientManagerLog.RmsClientManagerEvent.Exception;
			Guid transactionId = context.TransactionId;
			logRowFormatter[7] = context.TransactionId.ToString();
			logRowFormatter[6] = context.ContextStringForm;
			if (stringBuilder != null)
			{
				logRowFormatter[5] = stringBuilder.ToString();
			}
			RmsClientManagerLog.Append(logRowFormatter);
		}

		private static void LogEvent(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerLog.RmsClientManagerEvent clientManagerEvent, Uri serverUrl, OrganizationId orgId, Guid transaction, string data, string context)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(RmsClientManagerLog.rmsLogSchema);
			logRowFormatter[1] = clientManagerFeature;
			logRowFormatter[2] = clientManagerEvent;
			if (serverUrl != null)
			{
				logRowFormatter[4] = serverUrl.ToString();
			}
			if (orgId != null)
			{
				logRowFormatter[3] = orgId.ToString();
			}
			logRowFormatter[7] = transaction.ToString();
			logRowFormatter[6] = context;
			logRowFormatter[5] = data;
			RmsClientManagerLog.Append(logRowFormatter);
		}

		private static void LogEvent(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerLog.RmsClientManagerEvent clientManagerEvent, OrganizationId orgId, Guid transaction, string data, string context)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(RmsClientManagerLog.rmsLogSchema);
			logRowFormatter[1] = clientManagerFeature;
			logRowFormatter[2] = clientManagerEvent;
			if (orgId != null)
			{
				logRowFormatter[3] = orgId.ToString();
			}
			logRowFormatter[7] = transaction.ToString();
			logRowFormatter[6] = context;
			logRowFormatter[5] = data;
			RmsClientManagerLog.Append(logRowFormatter);
		}

		private static void LogEvent(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerLog.RmsClientManagerEvent clientManagerEvent, Guid transaction, string data, string context)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(RmsClientManagerLog.rmsLogSchema);
			logRowFormatter[1] = clientManagerFeature;
			logRowFormatter[2] = clientManagerEvent;
			logRowFormatter[7] = transaction.ToString();
			logRowFormatter[6] = context;
			logRowFormatter[5] = data;
			RmsClientManagerLog.Append(logRowFormatter);
		}

		private static void LogEvent(RmsClientManagerLog.RmsClientManagerFeature clientManagerFeature, RmsClientManagerLog.RmsClientManagerEvent clientManagerEvent, Uri serverUrl, OrganizationId orgId, Guid transaction, string context)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(RmsClientManagerLog.rmsLogSchema);
			logRowFormatter[1] = clientManagerFeature;
			logRowFormatter[2] = clientManagerEvent;
			if (serverUrl != null)
			{
				logRowFormatter[4] = serverUrl.ToString();
			}
			if (orgId != null)
			{
				logRowFormatter[3] = orgId.ToString();
			}
			logRowFormatter[7] = transaction.ToString();
			logRowFormatter[6] = context;
			RmsClientManagerLog.Append(logRowFormatter);
		}

		private static void HandleConfigurationChange(ADNotificationEventArgs args)
		{
			try
			{
				if (Interlocked.Increment(ref RmsClientManagerLog.notificationHandlerCount) == 1)
				{
					Server localServer = null;
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1191, "HandleConfigurationChange", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\rightsmanagement\\RmsClientManagerLog.cs");
						localServer = topologyConfigurationSession.FindLocalServer();
					});
					if (!adoperationResult.Succeeded)
					{
						RmsClientManagerLog.Tracer.TraceError<Exception>(0L, "Failed to get the local server.  Unable to reload the log configuration. Error {0}", adoperationResult.Exception);
					}
					else
					{
						RmsClientManagerLog.Configure(localServer);
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref RmsClientManagerLog.notificationHandlerCount);
			}
		}

		private static void Append(LogRowFormatter row)
		{
			RmsClientManagerLog.rmsLog.Append(row, 0);
		}

		private const string LogType = "Rms Client Manager Log";

		private const string LogComponent = "RmsClientManagerLog";

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"feature",
			"event-type",
			"tenant-id",
			"server-url",
			"data",
			"context",
			"transaction-id"
		};

		private static int notificationHandlerCount;

		private static LogSchema rmsLogSchema;

		private static Log rmsLog;

		private static bool rmsLogEnabled;

		private static ADNotificationRequestCookie notificationCookie;

		internal static readonly string LogSuffix = "IRMLOG";

		internal static readonly Uri DummyUriForTemplateCache = new Uri("cache:template");

		internal static readonly Uri DummyUriForRacClcCache = new Uri("cache:racClc");

		internal static readonly string PrelicenseNoResult = "No response";

		internal static readonly string UseLicenseEmpty = "UseLicense: Empty";

		internal static readonly string UseLicenseExists = "UseLicense: Exists";

		internal static readonly string AcquireServerRac = "Server RAC";

		internal static readonly string AcquireServerClc = "Server CLC";

		internal static readonly string AcquireServerRacClc = "Server RAC/CLC";

		internal static readonly string LicenseExists = "Exists";

		internal static readonly string LicenseEmpty = "Empty";

		internal static readonly string ServerInfoNotFound = "Not Found";

		internal static readonly string ServerInfoUriNull = "None";

		private enum Field
		{
			Time,
			Feature,
			EventType,
			TenantId,
			ServerUrl,
			Data,
			Context,
			TransactionId
		}

		internal enum RmsClientManagerFeature
		{
			Prelicense,
			UseLicense,
			RacClc,
			SignatureVerification,
			Template,
			ServerInfo,
			DrmInitialization
		}

		internal enum RmsClientManagerEvent
		{
			Acquire,
			Success,
			Queued,
			Exception
		}
	}
}
