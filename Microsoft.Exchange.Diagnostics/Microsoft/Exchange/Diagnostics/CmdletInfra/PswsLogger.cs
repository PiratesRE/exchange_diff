using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class PswsLogger : ConfigurationCoreLogger<PswsLogger>
	{
		protected override void InitializeLogger()
		{
			ExTraceGlobals.InstrumentationTracer.TraceDebug((long)this.GetHashCode(), "Create PswsLogger.");
			ActivityContext.RegisterMetadata(typeof(RpsCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(RpsAuthZMetadata));
			ActivityContext.RegisterMetadata(typeof(RpsCmdletMetadata));
			ActivityContext.RegisterMetadata(typeof(PswsMetadata));
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
				new KeyValuePair<string, Enum>("AuthenticationType", ActivityStandardMetadata.AuthenticationType),
				new KeyValuePair<string, Enum>("IsAuthenticated", ServiceCommonMetadata.IsAuthenticated),
				new KeyValuePair<string, Enum>("AuthenticatedUser", ServiceCommonMetadata.AuthenticatedUser),
				new KeyValuePair<string, Enum>("Organization", ActivityStandardMetadata.TenantId),
				new KeyValuePair<string, Enum>("ManagedOrganization", ConfigurationCoreMetadata.ManagedOrganization),
				new KeyValuePair<string, Enum>("ClientIpAddress", ServiceCommonMetadata.ClientIpAddress),
				new KeyValuePair<string, Enum>("ServerHostName", ServiceCommonMetadata.ServerHostName),
				new KeyValuePair<string, Enum>("FrontEndServer", ConfigurationCoreMetadata.FrontEndServer),
				new KeyValuePair<string, Enum>("UrlHost", ConfigurationCoreMetadata.UrlHost),
				new KeyValuePair<string, Enum>("Method", ServiceCommonMetadata.HttpMethod),
				new KeyValuePair<string, Enum>("HttpStatus", ServiceCommonMetadata.HttpStatus),
				new KeyValuePair<string, Enum>("SubStatus", ConfigurationCoreMetadata.SubStatus),
				new KeyValuePair<string, Enum>("ErrorCode", ServiceCommonMetadata.ErrorCode),
				new KeyValuePair<string, Enum>("IsAuthorized", RpsAuthZMetadata.IsAuthorized),
				new KeyValuePair<string, Enum>("AuthorizeUser", RpsAuthZMetadata.AuthorizeUser),
				new KeyValuePair<string, Enum>("GetInitialSessionState", RpsAuthZMetadata.GetInitialSessionState),
				new KeyValuePair<string, Enum>("VariantConfigurationSnapshot", RpsAuthZMetadata.VariantConfigurationSnapshot),
				new KeyValuePair<string, Enum>("CmdletFlightEnabled", RpsAuthZMetadata.CmdletFlightEnabled),
				new KeyValuePair<string, Enum>("CmdletFlightDisabled", RpsAuthZMetadata.CmdletFlightDisabled),
				new KeyValuePair<string, Enum>("GetQuota", RpsAuthZMetadata.GetQuota),
				new KeyValuePair<string, Enum>("UrlStem", ConfigurationCoreMetadata.UrlStem),
				new KeyValuePair<string, Enum>("UrlQuery", ConfigurationCoreMetadata.UrlQuery),
				new KeyValuePair<string, Enum>("RequestBytes", ServiceCommonMetadata.RequestSize),
				new KeyValuePair<string, Enum>("ClientInfo", ActivityStandardMetadata.ClientInfo),
				new KeyValuePair<string, Enum>("SerialLizationLevel", PswsMetadata.SerializationLevel),
				new KeyValuePair<string, Enum>("ClientApplication", PswsMetadata.ClientApplication),
				new KeyValuePair<string, Enum>("IsProxy", PswsMetadata.IsProxy),
				new KeyValuePair<string, Enum>("ProxyFullSerialzation", PswsMetadata.ProxyFullSerialzation),
				new KeyValuePair<string, Enum>("EncodeDecodeKey", PswsMetadata.EncodeDecodeKey),
				new KeyValuePair<string, Enum>("CultureInfo", PswsMetadata.CultureInfo),
				new KeyValuePair<string, Enum>("TenantOrganization", PswsMetadata.TenantOrganization),
				new KeyValuePair<string, Enum>("ProcessId", RpsCmdletMetadata.ProcessId),
				new KeyValuePair<string, Enum>("ThreadId", RpsCmdletMetadata.ThreadId),
				new KeyValuePair<string, Enum>("ExecutingUserSid", RpsCmdletMetadata.ExecutingUserSid),
				new KeyValuePair<string, Enum>("EffectiveOrganization", RpsCmdletMetadata.EffectiveOrganization),
				new KeyValuePair<string, Enum>("UserServicePlan", RpsCmdletMetadata.UserServicePlan),
				new KeyValuePair<string, Enum>("IsAdmin", RpsCmdletMetadata.IsAdmin),
				new KeyValuePair<string, Enum>("ServerActiveRunspaces", RpsAuthZMetadata.ServerActiveRunspaces),
				new KeyValuePair<string, Enum>("ServerActiveUsers", RpsAuthZMetadata.ServerActiveUsers),
				new KeyValuePair<string, Enum>("AuthZUserBudgetOnStart", RpsAuthZMetadata.UserBudgetOnStart),
				new KeyValuePair<string, Enum>("CmdletUserBudgetOnStart", RpsCmdletMetadata.UserBudgetOnStart),
				new KeyValuePair<string, Enum>("Cmdlet", RpsCmdletMetadata.Cmdlet),
				new KeyValuePair<string, Enum>("Parameters", RpsCmdletMetadata.Parameters),
				new KeyValuePair<string, Enum>("CmdletUniqueId", RpsCmdletMetadata.CmdletUniqueId),
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
				new KeyValuePair<string, Enum>("ProvisioningLayerLatency", RpsCmdletMetadata.ProvisioningLayerLatency),
				new KeyValuePair<string, Enum>("ErrorType", RpsCmdletMetadata.ErrorType),
				new KeyValuePair<string, Enum>("ExecutionResult", RpsCmdletMetadata.ExecutionResult),
				new KeyValuePair<string, Enum>("CmdletTotalTime", RpsCmdletMetadata.TotalTime),
				new KeyValuePair<string, Enum>("ActivityContextLifeTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("TotalTime", ConfigurationCoreMetadata.TotalTime),
				new KeyValuePair<string, Enum>("ContributeToFailFast", RpsCommonMetadata.ContributeToFailFast),
				new KeyValuePair<string, Enum>("CacheHitCount", RpsCmdletMetadata.CacheHitCount),
				new KeyValuePair<string, Enum>("CacheMissCount", RpsCmdletMetadata.CacheMissCount),
				new KeyValuePair<string, Enum>("Exception", ServiceCommonMetadata.ExceptionName),
				new KeyValuePair<string, Enum>("GenericLatency", RpsCommonMetadata.GenericLatency),
				new KeyValuePair<string, Enum>("GenericInfo", ServiceCommonMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("GenericErrors", ServiceCommonMetadata.GenericErrors),
				new KeyValuePair<string, Enum>("CPU", ConfigurationCoreMetadata.CPU),
				new KeyValuePair<string, Enum>("Memory", ConfigurationCoreMetadata.Memory)
			};
			return new RequestLoggerConfig("Psws Logs", "Psws_", "PswsLogs", "ConfigurationCoreLogger.LogFolder", PswsLogger.LogPath.Value, LoggerSettings.LogFileAgeInDays, (long)(LoggerSettings.MaxLogDirectorySizeInGB * 1024 * 1024 * 1024), (long)(LoggerSettings.MaxLogFileSizeInMB * 1024 * 1024), ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		private const string LogType = "Psws Logs";

		private const string LogFilePrefix = "Psws_";

		private const string LogComponent = "PswsLogs";

		private static readonly Lazy<string> LogPath = new Lazy<string>(() => Path.Combine(ConfigurationCoreLogger<PswsLogger>.DefaultLogFolderPath, LoggerSettings.LogSubFolderName));
	}
}
