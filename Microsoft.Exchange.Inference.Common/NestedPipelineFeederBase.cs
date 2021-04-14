using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal abstract class NestedPipelineFeederBase : BaseComponent, IStartStopPipelineComponent, IPipelineComponent, IDocumentProcessor, INotifyFailed, IStartStop, IDisposable
	{
		internal NestedPipelineFeederBase(IPipelineComponentConfig config, IPipelineContext context, IPipeline pipeline)
		{
			Util.ThrowOnNullArgument(pipeline, "pipeline");
			this.Config = config;
			this.Context = context;
			this.NestedPipeline = pipeline;
		}

		internal IPipelineComponentConfig ComponentConfig
		{
			get
			{
				return this.Config;
			}
		}

		internal IPipelineContext PipelineContext
		{
			get
			{
				return this.Context;
			}
		}

		public IAsyncResult BeginPrepareToStart(AsyncCallback callback, object context)
		{
			return this.CompleteAsyncResult(callback, context);
		}

		public IAsyncResult BeginStart(AsyncCallback callback, object context)
		{
			return this.CompleteAsyncResult(callback, context);
		}

		public IAsyncResult BeginStop(AsyncCallback callback, object context)
		{
			return this.CompleteAsyncResult(callback, context);
		}

		public void EndPrepareToStart(IAsyncResult asyncResult)
		{
			((AsyncResult)asyncResult).End();
		}

		public void EndStart(IAsyncResult asyncResult)
		{
			((AsyncResult)asyncResult).End();
		}

		public void EndStop(IAsyncResult asyncResult)
		{
			((AsyncResult)asyncResult).End();
		}

		public virtual void Dispose()
		{
		}

		private AsyncResult CompleteAsyncResult(AsyncCallback callback, object context)
		{
			AsyncResult asyncResult = new AsyncResult(callback, context);
			asyncResult.SetAsCompleted();
			return asyncResult;
		}

		protected readonly IPipeline NestedPipeline;

		private readonly IPipelineComponentConfig Config;

		private readonly IPipelineContext Context;
	}
}
