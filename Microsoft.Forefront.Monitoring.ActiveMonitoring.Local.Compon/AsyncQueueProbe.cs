using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Hygiene.AsyncQueue;
using Microsoft.Exchange.Hygiene.Data.AsyncQueue;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class AsyncQueueProbe : ProbeWorkItem
	{
		public int RetryItemBatchSize { get; set; }

		public int InProgressBatchSize { get; set; }

		public int RetryItemFetchBackSeconds { get; set; }

		public int InProgressFetchBackSeconds { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<DateTime>(ExTraceGlobals.DNSTracer, base.TraceContext, "AsyncQueueProbe: DoWork start, currentTime={0}", DateTime.UtcNow, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbe.cs", 56);
			Stopwatch stopwatch = Stopwatch.StartNew();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			try
			{
				this.LoadWorkContext(base.Definition.ExtensionAttributes);
				WTFDiagnostics.TraceInformation<string, int, int, int, int>(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "xml:{0},RetryItemBatchSize={1},InProgressBatchSize={2},RetryItemFetchBackSeconds={3},InProgressFetchBackSeconds={4} ", base.Definition.ExtensionAttributes, this.RetryItemBatchSize, this.InProgressBatchSize, this.RetryItemFetchBackSeconds, this.InProgressFetchBackSeconds, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbe.cs", 65);
				AsyncQueueSession asyncQueueSession = new AsyncQueueSession();
				ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
				{
					ExeConfigFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Microsoft.Exchange.Hygiene.AsyncQueueDaemon.exe.config")
				};
				Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
				ConfigurationData configurationData = (ConfigurationData)configuration.GetSection(ConfigurationData.AsyncQueueSectionName);
				ConfigurationData.Instance = configurationData;
				DateTime utcNow = DateTime.UtcNow;
				int partitionCount = asyncQueueSession.PartitionCount;
				stringBuilder.AppendLine(string.Join("; ", new string[]
				{
					"Number of partitions = " + partitionCount,
					"RetryItemBatchSize = " + this.RetryItemBatchSize,
					"InProgressBatchSize = " + this.InProgressBatchSize,
					"RetryItemFetchBackSeconds = " + this.RetryItemFetchBackSeconds,
					"InProgressFetchBackSeconds = " + this.InProgressFetchBackSeconds,
					"DateTime.UtcNow = " + utcNow
				}));
				for (int i = 0; i < partitionCount; i++)
				{
					List<AsyncQueueProbeResult> asyncProbeResults = asyncQueueSession.GetAsyncProbeResults(i, this.RetryItemBatchSize, this.InProgressBatchSize, this.RetryItemFetchBackSeconds, this.InProgressFetchBackSeconds);
					if (asyncProbeResults.Count > 0)
					{
						stringBuilder2.AppendLine(string.Format("Partition Number = {0}; GetAsyncProbeResults(...).Count = {1}", i, asyncProbeResults.Count));
					}
					foreach (AsyncQueueProbeResult asyncQueueProbeResult in asyncProbeResults)
					{
						TimeSpan stepMaxExecutionTime = configurationData.GetStepMaxExecutionTime(asyncQueueProbeResult.OwnerID, asyncQueueProbeResult.StepName);
						bool flag = utcNow - asyncQueueProbeResult.ChangedDatetime > stepMaxExecutionTime;
						if ((asyncQueueProbeResult.Status == 40 && flag) || asyncQueueProbeResult.Status == 30 || asyncQueueProbeResult.Status == 50 || asyncQueueProbeResult.Status == 60)
						{
							stringBuilder2.AppendLine(string.Join("; ", new string[]
							{
								string.Concat(new object[]
								{
									"Status = ",
									asyncQueueProbeResult.Status,
									"RequestId = ",
									asyncQueueProbeResult.RequestId,
									"StepName = ",
									asyncQueueProbeResult.StepName,
									"StepNumber = ",
									asyncQueueProbeResult.StepNumber,
									"CreatedDateTime = ",
									asyncQueueProbeResult.CreatedDatetime,
									"ChangedDateTime = ",
									asyncQueueProbeResult.ChangedDatetime,
									"utcNow - ChangedDateTime = ",
									utcNow - asyncQueueProbeResult.ChangedDatetime,
									"maxAllowedExecutionTime = ",
									stepMaxExecutionTime,
									"exceededMaxAllowedExecutionTime = ",
									flag
								})
							}));
						}
					}
					if (asyncProbeResults.Count > 0)
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "over due info:{0}", stringBuilder2.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbe.cs", 123);
						throw new AsyncQueueMonitorException(string.Format("Detected over due Partition Number = {0}", i));
					}
					if (cancellationToken.IsCancellationRequested)
					{
						stringBuilder.AppendLine("Cancellation requested.");
						break;
					}
				}
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "Exception:{0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbe.cs", 136);
				throw;
			}
			finally
			{
				base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				base.Result.ExecutionContext = stringBuilder.ToString();
				base.Result.FailureContext = stringBuilder2.ToString();
				WTFDiagnostics.TraceInformation(ExTraceGlobals.AsyncEngineTracer, base.TraceContext, "AsyncQueueProbe: DoWork end", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\AsyncQueue\\Probes\\AsyncQueueProbe.cs", 144);
			}
		}

		private void LoadWorkContext(string workContextXml)
		{
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentNullException("workContextXml");
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(workContextXml);
			XmlElement xmlElement = Utils.CheckXmlElement(safeXmlDocument.SelectSingleNode("//ProbeParam"), "//ProbeParam");
			this.RetryItemBatchSize = int.Parse(xmlElement.GetAttribute("RetryItemBatchSize", string.Empty));
			this.InProgressBatchSize = int.Parse(xmlElement.GetAttribute("InProgressBatchSize", string.Empty));
			this.RetryItemFetchBackSeconds = int.Parse(xmlElement.GetAttribute("RetryItemFetchBackSeconds", string.Empty));
			this.InProgressFetchBackSeconds = int.Parse(xmlElement.GetAttribute("InProgressFetchBackSeconds", string.Empty));
		}
	}
}
