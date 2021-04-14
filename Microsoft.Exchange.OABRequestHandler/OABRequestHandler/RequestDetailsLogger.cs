using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.OABRequestHandler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RequestDetailsLogger : RequestDetailsLoggerBase<RequestDetailsLogger>
	{
		public static bool TryAppendToIISLog(HttpResponse response, string format, params object[] args)
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

		public override void Commit()
		{
			ActivityContext.SetThreadScope(base.ActivityScope);
			ServiceCommonMetadataPublisher.PublishMetadata();
			base.Commit();
		}

		protected override void InitializeLogger()
		{
			ActivityContext.RegisterMetadata(typeof(OABDownloadRequestMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceLatencyMetadata));
			base.InitializeLogger();
		}

		protected override RequestLoggerConfig GetRequestLoggerConfig()
		{
			List<KeyValuePair<string, Enum>> columns = new List<KeyValuePair<string, Enum>>
			{
				new KeyValuePair<string, Enum>("DateTime", ActivityReadonlyMetadata.EndTime),
				new KeyValuePair<string, Enum>("RequestId", ActivityReadonlyMetadata.ActivityId),
				new KeyValuePair<string, Enum>("MajorVersion", ServiceCommonMetadata.ServerVersionMajor),
				new KeyValuePair<string, Enum>("MinorVersion", ServiceCommonMetadata.ServerVersionMinor),
				new KeyValuePair<string, Enum>("BuildVersion", ServiceCommonMetadata.ServerVersionBuild),
				new KeyValuePair<string, Enum>("RevisionVersion", ServiceCommonMetadata.ServerVersionRevision),
				new KeyValuePair<string, Enum>("ClientRequestId", ActivityStandardMetadata.ClientRequestId),
				new KeyValuePair<string, Enum>("AuthenticationType", ActivityStandardMetadata.AuthenticationType),
				new KeyValuePair<string, Enum>("IsAuthenticated", ServiceCommonMetadata.IsAuthenticated),
				new KeyValuePair<string, Enum>("AuthenticatedUser", ServiceCommonMetadata.AuthenticatedUser),
				new KeyValuePair<string, Enum>("Organization", ActivityStandardMetadata.TenantId),
				new KeyValuePair<string, Enum>("Domain", OABDownloadRequestMetadata.Domain),
				new KeyValuePair<string, Enum>("UserAgent", ActivityStandardMetadata.ClientInfo),
				new KeyValuePair<string, Enum>("ClientIpAddress", ServiceCommonMetadata.ClientIpAddress),
				new KeyValuePair<string, Enum>("OfflineAddressBookGuid", OABDownloadRequestMetadata.OfflineAddressBookGuid),
				new KeyValuePair<string, Enum>("FileType", OABDownloadRequestMetadata.FileRequested),
				new KeyValuePair<string, Enum>("HttpStatus", ServiceCommonMetadata.HttpStatus),
				new KeyValuePair<string, Enum>("FailureCode", OABDownloadRequestMetadata.FailureCode),
				new KeyValuePair<string, Enum>("LastRequestedTime", OABDownloadRequestMetadata.LastRequestedTime),
				new KeyValuePair<string, Enum>("LastTouchedTime", OABDownloadRequestMetadata.LastTouchedTime),
				new KeyValuePair<string, Enum>("NoOfRequestsOutStanding", OABDownloadRequestMetadata.NoOfRequestsOutStanding),
				new KeyValuePair<string, Enum>("TotalRequestTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("GenericInfo", ServiceCommonMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("AuthenticationErrors", ServiceCommonMetadata.AuthenticationErrors),
				new KeyValuePair<string, Enum>("GenericErrors", ServiceCommonMetadata.GenericErrors),
				new KeyValuePair<string, Enum>("OrganizationStatus", ActivityStandardMetadata.TenantStatus),
				new KeyValuePair<string, Enum>("IsAddressListDeleted", OABDownloadRequestMetadata.IsAddressListDeleted)
			};
			return new RequestLoggerConfig("OAB download Protocol Logs", "OABDownload_", "OABDownloadProtocolLogs", "RequestDetailsLogger.OABDownloadProtocolLogFolder", RequestDetailsLogger.DefaultLogFolderPath, TimeSpan.FromDays((double)RequestDetailsLogger.MaxLogRetentionInDays.Value), (long)RequestDetailsLogger.MaxLogDirectorySizeInGB.Value * 1024L * 1024L * 1024L, (long)RequestDetailsLogger.MaxLogFileSizeInMB.Value * 1024L * 1024L, ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		private const string LogType = "OAB download Protocol Logs";

		private const string LogFilePrefix = "OABDownload_";

		private const string LogComponent = "OABDownloadProtocolLogs";

		private const string CustomLogFolderPathAppSettingsKey = "RequestDetailsLogger.OABDownloadProtocolLogFolder";

		private static readonly IntAppSettingsEntry MaxLogRetentionInDays = new IntAppSettingsEntry("MaxLogRetentionInDays", 30, ExTraceGlobals.CommonAlgorithmTracer);

		private static readonly IntAppSettingsEntry MaxLogDirectorySizeInGB = new IntAppSettingsEntry("MaxLogDirectorySizeInGB", 5, ExTraceGlobals.CommonAlgorithmTracer);

		private static readonly IntAppSettingsEntry MaxLogFileSizeInMB = new IntAppSettingsEntry("MaxLogFileSizeInMB", 10, ExTraceGlobals.CommonAlgorithmTracer);

		private static readonly string DefaultLogFolderPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\OABDownload");
	}
}
