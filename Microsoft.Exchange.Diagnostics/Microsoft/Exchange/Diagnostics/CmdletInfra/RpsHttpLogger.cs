using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class RpsHttpLogger : ConfigurationCoreLogger<RpsHttpLogger>
	{
		protected override void InitializeLogger()
		{
			ExTraceGlobals.InstrumentationTracer.TraceDebug((long)this.GetHashCode(), "Create RpsHttpLogger.");
			ActivityContext.RegisterMetadata(typeof(RpsCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(RpsHttpMetadata));
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
				new KeyValuePair<string, Enum>("ClientRequestId", ActivityStandardMetadata.ClientRequestId),
				new KeyValuePair<string, Enum>("UrlHost", ConfigurationCoreMetadata.UrlHost),
				new KeyValuePair<string, Enum>("UrlStem", ConfigurationCoreMetadata.UrlStem),
				new KeyValuePair<string, Enum>("AuthenticationType", ActivityStandardMetadata.AuthenticationType),
				new KeyValuePair<string, Enum>("IsAuthenticated", ServiceCommonMetadata.IsAuthenticated),
				new KeyValuePair<string, Enum>("AuthenticatedUser", ServiceCommonMetadata.AuthenticatedUser),
				new KeyValuePair<string, Enum>("Organization", ActivityStandardMetadata.TenantId),
				new KeyValuePair<string, Enum>("ManagedOrganization", ConfigurationCoreMetadata.ManagedOrganization),
				new KeyValuePair<string, Enum>("ClientIpAddress", ServiceCommonMetadata.ClientIpAddress),
				new KeyValuePair<string, Enum>("ServerHostName", ServiceCommonMetadata.ServerHostName),
				new KeyValuePair<string, Enum>("FrontEndServer", ConfigurationCoreMetadata.FrontEndServer),
				new KeyValuePair<string, Enum>("HttpStatus", ServiceCommonMetadata.HttpStatus),
				new KeyValuePair<string, Enum>("SubStatus", ConfigurationCoreMetadata.SubStatus),
				new KeyValuePair<string, Enum>("ErrorCode", ServiceCommonMetadata.ErrorCode),
				new KeyValuePair<string, Enum>("Action", RpsHttpMetadata.Action),
				new KeyValuePair<string, Enum>("CommandId", RpsHttpMetadata.CommandId),
				new KeyValuePair<string, Enum>("CommandName", RpsHttpMetadata.CommandName),
				new KeyValuePair<string, Enum>("SessionId", RpsCommonMetadata.SessionId),
				new KeyValuePair<string, Enum>("ShellId", RpsHttpMetadata.ShellId),
				new KeyValuePair<string, Enum>("FailFast", RpsHttpMetadata.FailFast),
				new KeyValuePair<string, Enum>("ContributeToFailFast", RpsCommonMetadata.ContributeToFailFast),
				new KeyValuePair<string, Enum>("RequestBytes", ServiceCommonMetadata.RequestSize),
				new KeyValuePair<string, Enum>("ClientInfo", ActivityStandardMetadata.ClientInfo),
				new KeyValuePair<string, Enum>("CPU", ConfigurationCoreMetadata.CPU),
				new KeyValuePair<string, Enum>("Memory", ConfigurationCoreMetadata.Memory),
				new KeyValuePair<string, Enum>("ActivityContextLifeTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("TotalTime", ConfigurationCoreMetadata.TotalTime),
				new KeyValuePair<string, Enum>("UrlQuery", ConfigurationCoreMetadata.UrlQuery),
				new KeyValuePair<string, Enum>("GenericLatency", RpsCommonMetadata.GenericLatency),
				new KeyValuePair<string, Enum>("GenericInfo", ServiceCommonMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("GenericErrors", ServiceCommonMetadata.GenericErrors)
			};
			return new RequestLoggerConfig("Rps Http Logs", "Rps_Http_", "RpsHttpLogs", "ConfigurationCoreLogger.LogFolder", RpsHttpLogger.LogPath.Value, LoggerSettings.LogFileAgeInDays, (long)(LoggerSettings.MaxLogDirectorySizeInGB * 1024 * 1024 * 1024), (long)(LoggerSettings.MaxLogFileSizeInMB * 1024 * 1024), ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		private const string LogType = "Rps Http Logs";

		private const string LogFilePrefix = "Rps_Http_";

		private const string LogComponent = "RpsHttpLogs";

		internal static readonly Lazy<string> LogPath = new Lazy<string>(() => Path.Combine(ConfigurationCoreLogger<RpsHttpLogger>.DefaultLogFolderPath, LoggerSettings.LogSubFolderName, "Http"));
	}
}
