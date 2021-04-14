using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Exchange.Autodiscover
{
	internal class RequestDetailsLogger : RequestDetailsLoggerBase<RequestDetailsLogger>
	{
		public static string StreamInsightEngineURI
		{
			get
			{
				string text = ConfigurationManager.AppSettings["AutoDiscoverStreamInsightEngineURI"];
				if (text == null)
				{
					return "http://xsi-exo-pre-autod.cloudapp.net:10200/autodiscover/dcs/";
				}
				return text;
			}
		}

		public static int StreamInsightUploaderQueueSize
		{
			get
			{
				int num;
				if (int.TryParse(ConfigurationManager.AppSettings["AutoDiscoverStreamInsightUploaderQueueSize"], out num))
				{
					num = ((num > 1) ? num : 200);
				}
				else
				{
					num = 200;
				}
				return num;
			}
		}

		internal static bool StreamInsightUploaderEnabled
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Autodiscover.StreamInsightUploader.Enabled;
			}
		}

		public void AppendAuthError(string key, string value)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendAuthenticationError(this, key, value);
		}

		public void SetRedirectionType(RedirectionType redirectionType)
		{
			this.Set(AutoDiscoverMetadata.RedirectType, (int)redirectionType);
		}

		protected override void InitializeLogger()
		{
			ActivityContext.RegisterMetadata(typeof(AutoDiscoverMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceLatencyMetadata));
			ActivityContext.RegisterMetadata(typeof(BudgetMetadata));
			ThreadPool.QueueUserWorkItem(new WaitCallback(RequestDetailsLogger.CommitLogLines));
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
				new KeyValuePair<string, Enum>("UserAgent", ActivityStandardMetadata.ClientInfo),
				new KeyValuePair<string, Enum>("ClientIpAddress", ServiceCommonMetadata.ClientIpAddress),
				new KeyValuePair<string, Enum>("ServerHostName", ServiceCommonMetadata.ServerHostName),
				new KeyValuePair<string, Enum>("FrontEndServer", AutoDiscoverMetadata.FrontEndServer),
				new KeyValuePair<string, Enum>("SoapAction", ActivityStandardMetadata.Action),
				new KeyValuePair<string, Enum>("HttpStatus", ServiceCommonMetadata.HttpStatus),
				new KeyValuePair<string, Enum>("ErrorCode", ServiceCommonMetadata.ErrorCode),
				new KeyValuePair<string, Enum>("RedirectType", AutoDiscoverMetadata.RedirectType),
				new KeyValuePair<string, Enum>("BeginBudgetConnections", BudgetMetadata.BeginBudgetConnections),
				new KeyValuePair<string, Enum>("EndBudgetConnections", BudgetMetadata.EndBudgetConnections),
				new KeyValuePair<string, Enum>("BeginBudgetAD", BudgetMetadata.BeginBudgetAD),
				new KeyValuePair<string, Enum>("EndBudgetAD", BudgetMetadata.EndBudgetAD),
				new KeyValuePair<string, Enum>("BeginBudgetCAS", BudgetMetadata.BeginBudgetCAS),
				new KeyValuePair<string, Enum>("EndBudgetCAS", BudgetMetadata.EndBudgetCAS),
				new KeyValuePair<string, Enum>("ThrottlingPolicy", BudgetMetadata.ThrottlingPolicy),
				new KeyValuePair<string, Enum>("TotalDCRequestCount", BudgetMetadata.TotalDCRequestCount),
				new KeyValuePair<string, Enum>("TotalDCRequestLatency", BudgetMetadata.TotalDCRequestLatency),
				new KeyValuePair<string, Enum>("HttpPipelineLatency", ServiceLatencyMetadata.HttpPipelineLatency),
				new KeyValuePair<string, Enum>("CheckAccessCoreLatency", ServiceLatencyMetadata.CheckAccessCoreLatency),
				new KeyValuePair<string, Enum>("CoreExecutionLatency", ServiceLatencyMetadata.CoreExecutionLatency),
				new KeyValuePair<string, Enum>("CallerADLatency", ServiceLatencyMetadata.CallerADLatency),
				new KeyValuePair<string, Enum>("RequestedUserADLatency", ServiceLatencyMetadata.RequestedUserADLatency),
				new KeyValuePair<string, Enum>("ExchangePrincipalLatency", ServiceLatencyMetadata.ExchangePrincipalLatency),
				new KeyValuePair<string, Enum>("TotalRequestTime", ActivityReadonlyMetadata.TotalMilliseconds),
				new KeyValuePair<string, Enum>("GenericInfo", ServiceCommonMetadata.GenericInfo),
				new KeyValuePair<string, Enum>("AuthenticationErrors", ServiceCommonMetadata.AuthenticationErrors),
				new KeyValuePair<string, Enum>("GenericErrors", ServiceCommonMetadata.GenericErrors),
				new KeyValuePair<string, Enum>("Puid", ActivityStandardMetadata.Puid)
			};
			return new RequestLoggerConfig("Autodiscover Protocol Logs", "Autod_", "AutodiscoverProtocolLogs", "RequestDetailsLogger.AutodiscoverProtocolLogFolder", RequestDetailsLogger.DefaultLogFolderPath, TimeSpan.FromDays((double)RequestDetailsLogger.MaxLogRetentionInDays.Value), (long)(RequestDetailsLogger.MaxLogDirectorySizeInGB.Value * 1024 * 1024 * 1024), (long)(RequestDetailsLogger.MaxLogFileSizeInMB.Value * 1024 * 1024), ServiceCommonMetadata.GenericInfo, columns, Enum.GetValues(typeof(ServiceLatencyMetadata)).Length);
		}

		public void AsyncCommit()
		{
			if (!base.IsDisposed)
			{
				ServiceCommonMetadataPublisher.PublishMetadata();
				BudgetMetadataPublisher.PublishMetadata();
				string value = this.Get(ServiceCommonMetadata.LiveIdBasicLog);
				string value2 = this.Get(ServiceCommonMetadata.LiveIdBasicError);
				string value3 = this.Get(ServiceCommonMetadata.LiveIdNegotiateError);
				if (string.Equals(this.Get(ServiceCommonMetadata.HttpStatus), "401", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(this.Get(ServiceCommonMetadata.AuthenticatedUser)) && string.Equals(this.Get(ActivityStandardMetadata.AuthenticationType), "Anonymous", StringComparison.OrdinalIgnoreCase))
				{
					base.ExcludeLogEntry();
				}
				else
				{
					if (!string.IsNullOrEmpty(value))
					{
						this.AppendAuthError("LiveIdBasicLog", value);
					}
					if (!string.IsNullOrEmpty(value2))
					{
						this.AppendAuthError("LiveIdBasicError", value2);
					}
					if (!string.IsNullOrEmpty(value3))
					{
						this.AppendAuthError("LiveIdNegotiateError", value3);
					}
					if (string.Compare(this.Get(ServiceCommonMetadata.HttpStatus), "200", StringComparison.OrdinalIgnoreCase) == 0 && string.IsNullOrEmpty(this.Get(AutoDiscoverMetadata.RedirectType)))
					{
						this.SetRedirectionType(RedirectionType.FullResponse);
					}
				}
				RequestDetailsLogger.fileIoQueue.Enqueue(this);
				RequestDetailsLogger.workerSignal.Set();
			}
		}

		internal static void FlushQueuedFileWrites()
		{
			RequestDetailsLogger.workerSignal.Set();
		}

		private static void CommitLogLines(object state)
		{
			for (;;)
			{
				if (RequestDetailsLogger.fileIoQueue.Count <= 0)
				{
					RequestDetailsLogger.workerSignal.WaitOne();
				}
				else
				{
					RequestDetailsLogger requestDetailsLogger = RequestDetailsLogger.fileIoQueue.Dequeue() as RequestDetailsLogger;
					if (requestDetailsLogger != null && !requestDetailsLogger.IsDisposed)
					{
						try
						{
							requestDetailsLogger.Dispose();
						}
						catch (Exception exception)
						{
							Common.ReportException(exception, requestDetailsLogger, null);
						}
					}
				}
			}
		}

		private void PopulateStreamInsightDataObject(out AutoDiscoverRawData autoDiscoverRawData)
		{
			autoDiscoverRawData = new AutoDiscoverRawData();
			string text;
			base.TryGetLogColumnValueByEnum(ActivityReadonlyMetadata.EndTime, out text);
			autoDiscoverRawData.RequestTime = text;
			base.TryGetLogColumnValueByEnum(ActivityStandardMetadata.ClientInfo, out text);
			autoDiscoverRawData.UserAgent = text;
			base.TryGetLogColumnValueByEnum(ActivityStandardMetadata.Action, out text);
			autoDiscoverRawData.SoapAction = text;
			base.TryGetLogColumnValueByEnum(ServiceCommonMetadata.HttpStatus, out text);
			autoDiscoverRawData.HttpStatus = text;
			base.TryGetLogColumnValueByEnum(ServiceCommonMetadata.ErrorCode, out text);
			autoDiscoverRawData.ErrorCode = text;
			base.TryGetLogColumnValueByEnum(ServiceCommonMetadata.GenericErrors, out text);
			autoDiscoverRawData.GenericErrors = text;
		}

		protected override void UploadDataToStreamInsight()
		{
			string streamInsightEngineURI = RequestDetailsLogger.StreamInsightEngineURI;
			int streamInsightUploaderQueueSize = RequestDetailsLogger.StreamInsightUploaderQueueSize;
			try
			{
				if (RequestDetailsLogger.StreamInsightUploaderEnabled && !string.IsNullOrEmpty(streamInsightEngineURI))
				{
					if (RequestDetailsLogger.uploader == null)
					{
						DataContractSerializerEncoder<AutoDiscoverRawData> dataContractSerializerEncoder = new DataContractSerializerEncoder<AutoDiscoverRawData>();
						RequestDetailsLogger.uploader = new BatchingUploader<AutoDiscoverRawData>(dataContractSerializerEncoder, streamInsightEngineURI, streamInsightUploaderQueueSize, TimeSpan.FromSeconds(30.0), 1000, 1, 3, true, "", false, null, null, null, null, false);
					}
					AutoDiscoverRawData autoDiscoverRawData;
					this.PopulateStreamInsightDataObject(out autoDiscoverRawData);
					if (!RequestDetailsLogger.uploader.TryEnqueueItem(autoDiscoverRawData))
					{
						Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_StreamInsightsDataUploadFailed, RequestDetailsLoggerBase<RequestDetailsLogger>.RequestLoggerConfig.LogComponent + "_StreamInsightsDataUploadFailed", new object[]
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.RequestLoggerConfig.LogComponent,
							streamInsightEngineURI
						});
					}
				}
			}
			catch (Exception ex)
			{
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_StreamInsightsDataUploadFailed, RequestDetailsLoggerBase<RequestDetailsLogger>.RequestLoggerConfig.LogComponent + "_StreamInsightsDataUploadFailed", new object[]
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.RequestLoggerConfig.LogComponent,
					streamInsightEngineURI,
					ex.Message,
					ex.StackTrace
				});
			}
		}

		private const string StreamInsightEngineURIKey = "AutoDiscoverStreamInsightEngineURI";

		private const string StreamInsightUploaderQueueSizeKey = "AutoDiscoverStreamInsightUploaderQueueSize";

		private const int DefaultStreamInsightUploaderQueueSize = 200;

		private const string DefaultStreamInsightEngineURI = "http://xsi-exo-pre-autod.cloudapp.net:10200/autodiscover/dcs/";

		private const string LogType = "Autodiscover Protocol Logs";

		private const string LogFilePrefix = "Autod_";

		private const string LogComponent = "AutodiscoverProtocolLogs";

		private const string CustomLogFolderPathAppSettingsKey = "RequestDetailsLogger.AutodiscoverProtocolLogFolder";

		private static Queue fileIoQueue = Queue.Synchronized(new Queue());

		private static AutoResetEvent workerSignal = new AutoResetEvent(false);

		private static readonly IntAppSettingsEntry MaxLogRetentionInDays = new IntAppSettingsEntry("MaxLogRetentionInDays", 30, ExTraceGlobals.CommonAlgorithmTracer);

		private static readonly IntAppSettingsEntry MaxLogDirectorySizeInGB = new IntAppSettingsEntry("MaxLogDirectorySizeInGB", 5, ExTraceGlobals.CommonAlgorithmTracer);

		private static readonly IntAppSettingsEntry MaxLogFileSizeInMB = new IntAppSettingsEntry("MaxLogFileSizeInMB", 10, ExTraceGlobals.CommonAlgorithmTracer);

		private static BatchingUploader<AutoDiscoverRawData> uploader;

		private static readonly string DefaultLogFolderPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Autodiscover");
	}
}
