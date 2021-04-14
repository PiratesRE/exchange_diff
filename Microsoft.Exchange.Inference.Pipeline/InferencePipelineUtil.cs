using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.Pipeline;

namespace Microsoft.Exchange.Inference.Pipeline
{
	internal static class InferencePipelineUtil
	{
		public static Pipeline CreateAndStartTrainingPipeline(string dbGuid, IDiagnosticsSession session, string pipelineDefinitionFile, string pipelineInstanceName, Version pipelineVersion, out PipelineContext trainingPipelineContext)
		{
			Util.ThrowOnNullOrEmptyArgument(dbGuid, "dbGuid");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullOrEmptyArgument(pipelineDefinitionFile, "pipelineDefinitionFile");
			Util.ThrowOnNullOrEmptyArgument(pipelineInstanceName, "pipelineInstanceName");
			Util.ThrowOnNullArgument(pipelineVersion, "pipelineVersion");
			string text = Path.Combine(InferencePipelineUtil.ExecutingAssemblyLocation, pipelineDefinitionFile);
			session.TraceDebug<string>("Loading pipeline definition from {0}", text);
			PipelineDefinition pipelineDefinition = PipelineDefinition.LoadFromFile(text);
			session.TraceDebug<string, int, int>("Loaded pipeline definition with Name = {0}, MaxConcurrency = {1}, Component Count = {2}", pipelineDefinition.Name, pipelineDefinition.MaxConcurrency, pipelineDefinition.Components.Length);
			session.TraceDebug<string>("Creating training pipeline context with version - {0}", pipelineVersion.ToString());
			trainingPipelineContext = new PipelineContext();
			string instance = string.Format("{0}-{1}", pipelineInstanceName, dbGuid);
			trainingPipelineContext.SetProperty<Version>(DocumentSchema.PipelineVersion, pipelineVersion);
			trainingPipelineContext.SetProperty<bool>(InferencePipelineUtil.AbortProcessing, false);
			trainingPipelineContext.SetProperty<string>(DocumentSchema.PipelineInstanceName, pipelineInstanceName);
			session.TraceDebug("Create the training pipeline", new object[0]);
			Pipeline pipeline = new Pipeline(pipelineDefinition, instance, trainingPipelineContext, null);
			session.TraceDebug("Calling prepareToStart", new object[0]);
			IAsyncResult asyncResult = pipeline.BeginPrepareToStart(null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			pipeline.EndPrepareToStart(asyncResult);
			session.TraceDebug("Calling Start", new object[0]);
			asyncResult = pipeline.BeginStart(null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			pipeline.EndStart(asyncResult);
			session.TraceDebug("Training pipeline successfully created and started", new object[0]);
			return pipeline;
		}

		public static void StopAndDisposeTrainingPipeline(IDiagnosticsSession session, Pipeline pipeline)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(pipeline, "pipeline");
			session.TraceDebug("Calling Stop on the pipeline", new object[0]);
			IAsyncResult asyncResult = pipeline.BeginStop(null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			pipeline.EndStop(asyncResult);
			session.TraceDebug("Pipeline successfully stopped", new object[0]);
			pipeline.Dispose();
		}

		public static void SetAbortOnProcessing(PipelineContext trainingPipelineContext)
		{
			Util.ThrowOnNullArgument(trainingPipelineContext, "trainingPipelineContext");
			trainingPipelineContext.SetProperty<bool>(InferencePipelineUtil.AbortProcessing, true);
		}

		public static bool IsAbortOnProcessingRequested(PipelineContext trainingPipelineContext)
		{
			Util.ThrowOnNullArgument(trainingPipelineContext, "trainingPipelineContext");
			object obj;
			return trainingPipelineContext.TryGetProperty(InferencePipelineUtil.AbortProcessing, out obj) && (bool)obj;
		}

		private static readonly string ExecutingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private static readonly PropertyDefinition AbortProcessing = new SimplePropertyDefinition("AbortProcessing", typeof(bool));
	}
}
