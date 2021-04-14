using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class RpsAuthZLogger : ConfigurationCoreLogger<RpsAuthZLogger>
	{
		static RpsAuthZLogger()
		{
			RequestDetailsLoggerBase<RpsAuthZLogger>.AdditionalLoggerGetterForGetCurrent = (() => RpsAuthZLogger.currentThreadLogger);
			RequestDetailsLoggerBase<RpsAuthZLogger>.AdditionalLoggerSetterForSetCurrent = delegate(RpsAuthZLogger logger)
			{
				RpsAuthZLogger.currentThreadLogger = logger;
			};
		}

		protected override void InitializeLogger()
		{
			ExTraceGlobals.InstrumentationTracer.TraceDebug((long)this.GetHashCode(), "Create RpsAuthZLogger.");
			ActivityContext.RegisterMetadata(typeof(RpsCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(RpsAuthZMetadata));
			base.InitializeLogger();
		}

		protected override RequestLoggerConfig GetRequestLoggerConfig()
		{
			List<KeyValuePair<string, Enum>> columns = new List<KeyValuePair<string, Enum>>
			{
				new KeyValuePair<string, Enum>("DateTime", ActivityReadonlyMetadata.EndTime),
				new KeyValuePair<string, Enum>("StartTime", ActivityReadonlyMetadata.StartTime),
				new KeyValuePair<string, Enum>("RequestId", ActivityReadonlyMetadata.ActivityId),
				new KeyValuePair<string, Enum>("MajorVersion", ServiceCommonMetadata.ServerVersionMajor),
				new KeyValuePair<string, Enum>("MinorVersion", ServiceCommonMetadata.ServerVersionMinor),
				new KeyValuePair<string, Enum>("BuildVersion", ServiceCommonMetadata.ServerVersionBuild),
				new KeyValuePair<string, Enum>("RevisionVersion", ServiceCommonMetadata.ServerVersionRevision),
				new KeyValuePair<string, Enum>("ServerHostName", ServiceCommonMetadata.ServerHostName),
				new KeyValuePair<string, Enum>("IsAuthorized", RpsAuthZMetadata.IsAuthorized),
				new KeyValuePair<string, Enum>("Operation", RpsAuthZMetadata.Operation),
				new KeyValuePair<string, Enum>("Action", RpsAuthZMetadata.Action),
				new KeyValuePair<string, Enum>("AuthenticationType", ActivityStandardMetadata.AuthenticationType),
				new KeyValuePair<string, Enum>("AuthenticatedUser", ServiceCommonMetadata.AuthenticatedUser),
				new KeyValuePair<string, Enum>("Organization", ActivityStandardMetadata.TenantId),
				new KeyValuePair<string, Enum>("ManagedOrganization", ConfigurationCoreMetadata.ManagedOrganization),
				new KeyValuePair<string, Enum>("SessionId", RpsCommonMetadata.SessionId),
				new KeyValuePair<string, Enum>("Function", RpsAuthZMetadata.Function),
				new KeyValuePair<string, Enum>("AuthorizeUser", RpsAuthZMetadata.AuthorizeUser),
				new KeyValuePair<string, Enum>("AuthorizeOperation", RpsAuthZMetadata.AuthorizeOperation),
				new KeyValuePair<string, Enum>("GetQuota", RpsAuthZMetadata.GetQuota),
				new KeyValuePair<string, Enum>("WSManOperationComplete", RpsAuthZMetadata.WSManOperationComplete),
				new KeyValuePair<string, Enum>("WSManUserComplete", RpsAuthZMetadata.WSManUserComplete),
				new KeyValuePair<string, Enum>("WSManQuotaComplete", RpsAuthZMetadata.WSManQuotaComplete),
				new KeyValuePair<string, Enum>("ValidateConnectionLimit", RpsAuthZMetadata.ValidateConnectionLimit),
				new KeyValuePair<string, Enum>("GetApplicationPrivateData", RpsAuthZMetadata.GetApplicationPrivateData),
				new KeyValuePair<string, Enum>("GetInitialSessionState", RpsAuthZMetadata.GetInitialSessionState),
				new KeyValuePair<string, Enum>("VariantConfigurationSnapshot", RpsAuthZMetadata.VariantConfigurationSnapshot),
				new KeyValuePair<string, Enum>("CmdletFlightEnabled", RpsAuthZMetadata.CmdletFlightEnabled),
				new KeyValuePair<string, Enum>("CmdletFlightDisabled", RpsAuthZMetadata.CmdletFlightDisabled),
				new KeyValuePair<string, Enum>("ServerActiveRunspaces", RpsAuthZMetadata.ServerActiveRunspaces),
				new KeyValuePair<string, Enum>("ServerActiveUsers", RpsAuthZMetadata.ServerActiveUsers),
				new KeyValuePair<string, Enum>("UserBudgetOnStart", RpsAuthZMetadata.UserBudgetOnStart),
				new KeyValuePair<string, Enum>("TenantBudgetOnStart", RpsAuthZMetadata.TenantBudgetOnStart),
				new KeyValuePair<string, Enum>("ContributeToFailFast", RpsCommonMetadata.ContributeToFailFast),
				new KeyValuePair<string, Enum>("ActivityContextLifeTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("TotalTime", ConfigurationCoreMetadata.TotalTime),
				new KeyValuePair<string, Enum>("GenericLatency", RpsCommonMetadata.GenericLatency),
				new KeyValuePair<string, Enum>("GenericInfo", ServiceCommonMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("GenericErrors", ServiceCommonMetadata.GenericErrors)
			};
			return new RequestLoggerConfig("Rps AuthZ Logs", "Rps_AuthZ_", "RpsAuthZLogs", "ConfigurationCoreLogger.LogFolder", RpsAuthZLogger.LogPath.Value, LoggerSettings.LogFileAgeInDays, (long)(LoggerSettings.MaxLogDirectorySizeInGB * 1024 * 1024 * 1024), (long)(LoggerSettings.MaxLogFileSizeInMB * 1024 * 1024), ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		private const string LogType = "Rps AuthZ Logs";

		private const string LogFilePrefix = "Rps_AuthZ_";

		private const string LogComponent = "RpsAuthZLogs";

		private static readonly Lazy<string> LogPath = new Lazy<string>(() => Path.Combine(ConfigurationCoreLogger<RpsHttpLogger>.DefaultLogFolderPath, LoggerSettings.LogSubFolderName, "AuthZ"));

		[ThreadStatic]
		private static RpsAuthZLogger currentThreadLogger;
	}
}
