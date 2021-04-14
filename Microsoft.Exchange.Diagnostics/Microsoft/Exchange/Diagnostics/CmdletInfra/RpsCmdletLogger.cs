using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class RpsCmdletLogger : ConfigurationCoreLogger<RpsCmdletLogger>
	{
		static RpsCmdletLogger()
		{
			RequestDetailsLoggerBase<RpsCmdletLogger>.AdditionalLoggerGetterForGetCurrent = (() => RpsCmdletLogger.currentThreadLogger);
			RequestDetailsLoggerBase<RpsCmdletLogger>.AdditionalLoggerSetterForSetCurrent = delegate(RpsCmdletLogger logger)
			{
				RpsCmdletLogger.currentThreadLogger = logger;
			};
		}

		internal static string CurrentProcessLogPath
		{
			get
			{
				if (LoggerSettings.LogSubFolderName == "Others" && "powershell.exe".Equals(LoggerSettings.ProcessName, StringComparison.OrdinalIgnoreCase))
				{
					return Path.Combine(ConfigurationCoreLogger<RpsHttpLogger>.DefaultLogFolderPath, "LocalPowerShell", "Cmdlet");
				}
				return RpsCmdletLogger.LogPath.Value;
			}
		}

		private static string CurrentProcessLogFilePrefix
		{
			get
			{
				if (LoggerSettings.LogSubFolderName != "Others")
				{
					return RpsCmdletLogger.logFilePrefix;
				}
				return string.Concat(new object[]
				{
					LoggerSettings.ProcessName,
					"_",
					LoggerSettings.ProcessId,
					"_Cmdlet_"
				});
			}
		}

		protected override void InitializeLogger()
		{
			ExTraceGlobals.InstrumentationTracer.TraceDebug((long)this.GetHashCode(), "Create CmdletLogger.");
			RpsCmdletLogger.logPrefix = (LoggerHelper.IsPswsCmdletDirectInvoke ? "PswsDI" : "Rps");
			RpsCmdletLogger.logType = RpsCmdletLogger.logPrefix + " Cmdlet Logs";
			RpsCmdletLogger.logFilePrefix = RpsCmdletLogger.logPrefix + "_Cmdlet_";
			RpsCmdletLogger.logComponent = RpsCmdletLogger.logPrefix + "CmdletLogs";
			ActivityContext.RegisterMetadata(typeof(RpsCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(RpsCmdletMetadata));
			base.InitializeLogger();
		}

		protected override RequestLoggerConfig GetRequestLoggerConfig()
		{
			List<KeyValuePair<string, Enum>> columns = new List<KeyValuePair<string, Enum>>
			{
				new KeyValuePair<string, Enum>("DateTime", RpsCmdletMetadata.EndTime),
				new KeyValuePair<string, Enum>("StartTime", RpsCmdletMetadata.StartTime),
				new KeyValuePair<string, Enum>("RequestId", ActivityReadonlyMetadata.ActivityId),
				new KeyValuePair<string, Enum>("ClientRequestId", ActivityStandardMetadata.ClientRequestId),
				new KeyValuePair<string, Enum>("MajorVersion", ServiceCommonMetadata.ServerVersionMajor),
				new KeyValuePair<string, Enum>("MinorVersion", ServiceCommonMetadata.ServerVersionMinor),
				new KeyValuePair<string, Enum>("BuildVersion", ServiceCommonMetadata.ServerVersionBuild),
				new KeyValuePair<string, Enum>("RevisionVersion", ServiceCommonMetadata.ServerVersionRevision),
				new KeyValuePair<string, Enum>("ServerHostName", ServiceCommonMetadata.ServerHostName),
				new KeyValuePair<string, Enum>("ProcessId", RpsCmdletMetadata.ProcessId),
				new KeyValuePair<string, Enum>("ProcessName", RpsCmdletMetadata.ProcessName),
				new KeyValuePair<string, Enum>("ThreadId", RpsCmdletMetadata.ThreadId),
				new KeyValuePair<string, Enum>("CultureInfo", RpsCmdletMetadata.CultureInfo),
				new KeyValuePair<string, Enum>("Organization", RpsCmdletMetadata.TenantId),
				new KeyValuePair<string, Enum>("AuthenticatedUser", RpsCmdletMetadata.AuthenticatedUser),
				new KeyValuePair<string, Enum>("ExecutingUserSid", RpsCmdletMetadata.ExecutingUserSid),
				new KeyValuePair<string, Enum>("EffectiveOrganization", RpsCmdletMetadata.EffectiveOrganization),
				new KeyValuePair<string, Enum>("UserServicePlan", RpsCmdletMetadata.UserServicePlan),
				new KeyValuePair<string, Enum>("IsAdmin", RpsCmdletMetadata.IsAdmin),
				new KeyValuePair<string, Enum>("ClientApplication", RpsCmdletMetadata.ClientApplication),
				new KeyValuePair<string, Enum>("Cmdlet", RpsCmdletMetadata.Cmdlet),
				new KeyValuePair<string, Enum>("Parameters", RpsCmdletMetadata.Parameters),
				new KeyValuePair<string, Enum>("CmdletUniqueId", RpsCmdletMetadata.CmdletUniqueId),
				new KeyValuePair<string, Enum>("UserBudgetOnStart", RpsCmdletMetadata.UserBudgetOnStart),
				new KeyValuePair<string, Enum>("ContributeToFailFast", RpsCmdletMetadata.ContributeToFailFast),
				new KeyValuePair<string, Enum>("RunspaceSettingsCreationHint", RpsCmdletMetadata.RunspaceSettingsCreationHint),
				new KeyValuePair<string, Enum>("ADViewEntireForest", RpsCmdletMetadata.ADViewEntireForest),
				new KeyValuePair<string, Enum>("ADRecipientViewRoot", RpsCmdletMetadata.ADRecipientViewRoot),
				new KeyValuePair<string, Enum>("ADConfigurationDomainControllers", RpsCmdletMetadata.ADConfigurationDomainControllers),
				new KeyValuePair<string, Enum>("ADPreferredGlobalCatalogs", RpsCmdletMetadata.ADPreferredGlobalCatalogs),
				new KeyValuePair<string, Enum>("ADPreferredDomainControllers", RpsCmdletMetadata.ADPreferredDomainControllers),
				new KeyValuePair<string, Enum>("ADUserConfigurationDomainController", RpsCmdletMetadata.ADUserConfigurationDomainController),
				new KeyValuePair<string, Enum>("ADUserPreferredGlobalCatalog", RpsCmdletMetadata.ADUserPreferredGlobalCatalog),
				new KeyValuePair<string, Enum>("ADuserPreferredDomainControllers", RpsCmdletMetadata.ADUserPreferredDomainControllers),
				new KeyValuePair<string, Enum>("ThrottlingInfo", RpsCmdletMetadata.ThrottlingInfo),
				new KeyValuePair<string, Enum>("DelayInfo", RpsCmdletMetadata.DelayInfo),
				new KeyValuePair<string, Enum>("ThrottlingDelay", RpsCmdletMetadata.ThrottlingDelay),
				new KeyValuePair<string, Enum>("IsOutputObjectRedacted", RpsCmdletMetadata.IsOutputObjectRedacted),
				new KeyValuePair<string, Enum>("CmdletProxyStage", RpsCmdletMetadata.CmdletProxyStage),
				new KeyValuePair<string, Enum>("CmdletProxyRemoteServer", RpsCmdletMetadata.CmdletProxyRemoteServer),
				new KeyValuePair<string, Enum>("CmdletProxyRemoteServerVersion", RpsCmdletMetadata.CmdletProxyRemoteServerVersion),
				new KeyValuePair<string, Enum>("CmdletProxyMethod", RpsCmdletMetadata.CmdletProxyMethod),
				new KeyValuePair<string, Enum>("ProxiedObjectCount", RpsCmdletMetadata.ProxiedObjectCount),
				new KeyValuePair<string, Enum>("CmdletProxyLatency", RpsCmdletMetadata.CmdletProxyLatency),
				new KeyValuePair<string, Enum>("OutputObjectCount", RpsCmdletMetadata.OutputObjectCount),
				new KeyValuePair<string, Enum>("ParameterBinding", RpsCmdletMetadata.ParameterBinding),
				new KeyValuePair<string, Enum>("BeginProcessing", RpsCmdletMetadata.BeginProcessing),
				new KeyValuePair<string, Enum>("ProcessRecord", RpsCmdletMetadata.ProcessRecord),
				new KeyValuePair<string, Enum>("EndProcessing", RpsCmdletMetadata.EndProcessing),
				new KeyValuePair<string, Enum>("StopProcessing", RpsCmdletMetadata.StopProcessing),
				new KeyValuePair<string, Enum>("BizLogic", RpsCmdletMetadata.BizLogic),
				new KeyValuePair<string, Enum>("PowerShellLatency", RpsCmdletMetadata.PowerShellLatency),
				new KeyValuePair<string, Enum>("UserInteractionLatency", RpsCmdletMetadata.UserInteractionLatency),
				new KeyValuePair<string, Enum>("ProvisioningLayerLatency", RpsCmdletMetadata.ProvisioningLayerLatency),
				new KeyValuePair<string, Enum>("ActivityContextLifeTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("TotalTime", RpsCmdletMetadata.TotalTime),
				new KeyValuePair<string, Enum>("ErrorType", RpsCmdletMetadata.ErrorType),
				new KeyValuePair<string, Enum>("ExecutionResult", RpsCmdletMetadata.ExecutionResult),
				new KeyValuePair<string, Enum>("CacheHitCount", RpsCmdletMetadata.CacheHitCount),
				new KeyValuePair<string, Enum>("CacheMissCount", RpsCmdletMetadata.CacheMissCount),
				new KeyValuePair<string, Enum>("GenericLatency", RpsCommonMetadata.GenericLatency),
				new KeyValuePair<string, Enum>("GenericInfo", RpsCmdletMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("GenericErrors", RpsCmdletMetadata.GenericErrors)
			};
			return new RequestLoggerConfig(RpsCmdletLogger.logType, RpsCmdletLogger.CurrentProcessLogFilePrefix, RpsCmdletLogger.logComponent, "ConfigurationCoreLogger.LogFolder", RpsCmdletLogger.CurrentProcessLogPath, LoggerSettings.LogFileAgeInDays, (long)(LoggerSettings.MaxLogDirectorySizeInGB * 1024 * 1024 * 1024), (long)(LoggerSettings.MaxLogFileSizeInMB * 1024 * 1024), ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		private static readonly Lazy<string> LogPath = new Lazy<string>(() => Path.Combine(ConfigurationCoreLogger<RpsHttpLogger>.DefaultLogFolderPath, LoggerSettings.LogSubFolderName, "Cmdlet"));

		private static string logPrefix;

		private static string logType;

		private static string logFilePrefix;

		private static string logComponent;

		[ThreadStatic]
		private static RpsCmdletLogger currentThreadLogger;
	}
}
