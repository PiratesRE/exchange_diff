using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Common.Diagnostics;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.Pipeline;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class InferenceClassificationAgentFactory : StoreDriverDeliveryAgentFactory
	{
		private bool IsEnabled()
		{
			return TransportAppConfig.GetConfigBool("InferenceClassificationAgentEnabledOverride", VariantConfiguration.InvariantNoFlightingSnapshot.MailboxTransport.InferenceClassificationAgent.Enabled);
		}

		private void Initialize()
		{
			DiagnosticsSessionFactory.SetDefaults(Guid.Parse("ebfb4d9d-d5ed-45e5-9f75-e3389bece6fa"), "Inference Classification Agent", "Inference Diagnostics Logs", Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\InferenceClassification"), "Inference_", "InferenceClassificationLogs");
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("InferenceClassificationAgentFactory", null, (long)this.GetHashCode());
			this.CreateClassificationAgentLogger();
			this.CreateClassificationComparisonLogger();
			if (this.isPipelineEnabled)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				PipelineDefinition definition = PipelineDefinition.LoadFromFile(Path.Combine(InferenceClassificationAgentFactory.ExecutingAssemblyLocation, "InferenceClassificationPipelineDefinition.xml"));
				string text = "ClassificationPipeline";
				InferenceModel.GetInstance(text).Reset();
				PipelineContext pipelineContext = new PipelineContext();
				pipelineContext.SetProperty<string>(DocumentSchema.PipelineInstanceName, text);
				this.pipeline = new Pipeline(definition, text, pipelineContext, null);
				IAsyncResult asyncResult = this.pipeline.BeginPrepareToStart(null, null);
				this.pipeline.EndPrepareToStart(asyncResult);
				asyncResult = this.pipeline.BeginStart(null, null);
				this.pipeline.EndStart(asyncResult);
				stopwatch.Stop();
				this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Pipeline creation timespan: {0} ms", new object[]
				{
					stopwatch.ElapsedMilliseconds
				});
			}
		}

		public InferenceClassificationAgentFactory()
		{
			this.isAgentEnabled = this.IsEnabled();
			if (this.isAgentEnabled)
			{
				this.isPipelineEnabled = this.IsPipelineEnabled();
				this.Initialize();
			}
		}

		private bool IsPipelineEnabled()
		{
			return RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference", "ClassificationPipelineEnabled", 0) != 0;
		}

		internal void CreateClassificationAgentLogger()
		{
			ILogConfig logConfig = new LogConfig(true, "InferenceClassificationProperties", "InferenceClassificationProperties", Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\InferenceClassification\\Properties"), new ulong?(ByteQuantifiedSize.FromGB(2UL).ToBytes()), new ulong?(ByteQuantifiedSize.FromMB(10UL).ToBytes()), new TimeSpan?(TimeSpan.FromDays(30.0)), 4096);
			this.classificationAgentLogger = new InferenceClassificationAgentLogger(logConfig);
		}

		internal void CreateClassificationComparisonLogger()
		{
			ILogConfig config = new LogConfig(true, "InferenceClassificationComparisons", "InferenceClassificationComparisons", Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\InferenceClassification\\Comparisons"), new ulong?(ByteQuantifiedSize.FromGB(2UL).ToBytes()), new ulong?(ByteQuantifiedSize.FromMB(10UL).ToBytes()), new TimeSpan?(TimeSpan.FromDays(30.0)), 4096);
			this.classificationComparisonLogger = new InferenceClassificationComparisonLogger(config);
		}

		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new InferenceClassificationAgent(server, this.pipeline, this.isAgentEnabled, this.isPipelineEnabled, this.diagnosticsSession, this.classificationAgentLogger, this.classificationComparisonLogger);
		}

		public override void Close()
		{
			if (this.pipeline != null)
			{
				IAsyncResult asyncResult = this.pipeline.BeginStop(null, null);
				this.pipeline.EndStop(asyncResult);
				this.pipeline.Dispose();
				this.pipeline = null;
			}
			if (this.classificationAgentLogger != null)
			{
				this.classificationAgentLogger.Dispose();
				this.classificationAgentLogger = null;
			}
			if (this.classificationComparisonLogger != null)
			{
				this.classificationComparisonLogger.Dispose();
				this.classificationComparisonLogger = null;
			}
		}

		private const string PipelineDefinitionFileName = "InferenceClassificationPipelineDefinition.xml";

		private const string InferenceAgentEnabled = "InferenceClassificationAgentEnabledOverride";

		private const string RegistryKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference";

		private const string InferenceAgentPipelineEnabled = "ClassificationPipelineEnabled";

		private static readonly string ExecutingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private Pipeline pipeline;

		private readonly bool isAgentEnabled;

		private readonly bool isPipelineEnabled;

		private IDiagnosticsSession diagnosticsSession;

		private InferenceClassificationAgentLogger classificationAgentLogger;

		private InferenceClassificationComparisonLogger classificationComparisonLogger;
	}
}
