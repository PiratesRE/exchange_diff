using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class DataInsightsPublisher
	{
		public DataInsightsPublisher()
		{
			if (Settings.EnableStreamInsightPush)
			{
				this.traceContext = new TracingContext(null)
				{
					LId = this.GetHashCode(),
					Id = base.GetType().GetHashCode()
				};
				DataContractSerializerEncoder<ProbeResultRawData> dataContractSerializerEncoder = new DataContractSerializerEncoder<ProbeResultRawData>();
				WTFDiagnostics.TraceDebug<string, string>(WTFLog.Core, this.traceContext, "Creating new BatchingUploader ServerAddress:{0} InstanceName:{1}", Settings.StreamInsightServerAddress, Settings.InstanceName, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\DataInsightsPublisher.cs", 45);
				this.uploader = new BatchingUploader<ProbeResultRawData>(dataContractSerializerEncoder, Settings.StreamInsightServerAddress, 20000, TimeSpan.FromSeconds(10.0), 100, 2, 3, true, Settings.InstanceName, Settings.EnableOAuth, Settings.OAuthTenantName, Settings.OAuthAppId, Settings.OAuthCertificateName, Settings.OAuthSymmetricKey, false);
			}
		}

		~DataInsightsPublisher()
		{
			if (this.uploader != null)
			{
				this.uploader.Dispose();
			}
		}

		public void PublishToInsightsEngine(ProbeResult result)
		{
			try
			{
				if (this.uploader != null)
				{
					WTFDiagnostics.TraceDebug<string, ResultType, string>(WTFLog.Core, this.traceContext, "Attempting to upload result to DataInsight: ResultName={0} ResultType={1} InstanceName={2}", result.ResultName, result.ResultType, Settings.InstanceName, null, "PublishToInsightsEngine", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\DataInsightsPublisher.cs", 93);
					if (!this.uploader.TryEnqueueItem(new ProbeResultRawData
					{
						CreatedTimeInUtc = result.ExecutionEndTime,
						Name = result.ResultName,
						OperationCode = Convert.ToInt32(result.ResultType),
						AMInstanceName = ((!Settings.IsCortex) ? Settings.InstanceName : ((!string.IsNullOrEmpty(Settings.CortexDataPartitionRingKey)) ? Settings.CortexDataPartitionRingKey : Settings.InstanceName)),
						Latency = result.SampleValue,
						Error = result.Error,
						Exception = result.Exception,
						FailureCategory = result.FailureCategory,
						StateAttribute1 = result.StateAttribute1,
						StateAttribute2 = result.StateAttribute2,
						StateAttribute3 = result.StateAttribute3,
						StateAttribute4 = result.StateAttribute4,
						StateAttribute5 = result.StateAttribute5,
						StateAttribute6 = result.StateAttribute6,
						StateAttribute7 = result.StateAttribute7,
						StateAttribute8 = result.StateAttribute8,
						StateAttribute9 = result.StateAttribute9,
						StateAttribute10 = result.StateAttribute10,
						StateAttribute11 = result.StateAttribute11,
						StateAttribute12 = result.StateAttribute12,
						StateAttribute13 = result.StateAttribute13,
						StateAttribute14 = result.StateAttribute14,
						StateAttribute15 = result.StateAttribute15,
						StateAttribute18 = result.StateAttribute18,
						StateAttribute21 = result.StateAttribute21,
						StateAttribute22 = result.StateAttribute22,
						StateAttribute23 = result.StateAttribute23,
						ExecutionContext = result.ExecutionContext,
						FailureContext = result.FailureContext,
						ResultId = result.ResultId,
						IsCortex = Settings.IsCortex,
						DataPartition = Settings.DataPartition
					}))
					{
						WTFDiagnostics.TraceError<string>(WTFLog.Core, this.traceContext, "Unable to add data to the Insights uploader. Queue size reached or the Insights engine Uri {0} is not receiving data", Settings.StreamInsightServerAddress, null, "PublishToInsightsEngine", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\DataInsightsPublisher.cs", 138);
					}
				}
				else
				{
					WTFDiagnostics.TraceDebug(WTFLog.Core, this.traceContext, "Skipping upload to DataInsights. Uploader object is null.", null, "PublishToInsightsEngine", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\DataInsightsPublisher.cs", 147);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(WTFLog.Core, this.traceContext, "Publish to Insights Engine failed with error {0}", ex.Message, null, "PublishToInsightsEngine", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\DataInsightsPublisher.cs", 157);
			}
		}

		private readonly BatchingUploader<ProbeResultRawData> uploader;

		private TracingContext traceContext;
	}
}
