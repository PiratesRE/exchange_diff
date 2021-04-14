using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	internal abstract class BaseComponent : IPipelineComponent, IDocumentProcessor, INotifyFailed
	{
		protected BaseComponent() : this(null)
		{
		}

		protected BaseComponent(IPipelineComponentConfig config)
		{
			InferenceCommonUtility.ConfigTryParseHelper<bool>(config, new InferenceCommonUtility.TryParseFunction<bool>(bool.TryParse), "AsyncComponent", out this.isAsynchronous, this.DiagnosticsSession, false);
			this.DiagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("BaseComponent", ExTraceGlobals.SynchronousComponentBaseTracer, (long)this.GetHashCode());
		}

		public event EventHandler<FailedEventArgs> Failed;

		public abstract string Description { get; }

		public abstract string Name { get; }

		public void ProcessDocument(IDocument document, object context)
		{
			IAsyncResult asyncResult = this.InternalBeginProcess(document, null, context, false);
			this.EndProcess(asyncResult);
		}

		public virtual IAsyncResult BeginProcess(IDocument document, AsyncCallback callback, object context)
		{
			return this.InternalBeginProcess(document, callback, context, this.isAsynchronous);
		}

		public virtual void EndProcess(IAsyncResult asyncResult)
		{
			this.DiagnosticsSession.TraceDebug("Called end process", new object[0]);
			((AsyncResult)asyncResult).End();
		}

		protected abstract void InternalProcessDocument(DocumentContext data);

		protected void OnFailed(FailedEventArgs e)
		{
			EventHandler<FailedEventArgs> failed = this.Failed;
			if (failed != null)
			{
				failed(this, e);
			}
		}

		private IAsyncResult InternalBeginProcess(IDocument document, AsyncCallback callback, object context, bool isAsynchronous)
		{
			Util.ThrowOnNullArgument(document, "document");
			AsyncResult asyncResult = new AsyncResult(callback, context);
			this.DiagnosticsSession.TraceDebug<IIdentity>("Called begin process - {0} ", document.Identity);
			DocumentContext documentContext = new DocumentContext(document, asyncResult);
			if (isAsynchronous)
			{
				ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.ProcessDocument)), documentContext);
			}
			else
			{
				this.ProcessDocument(documentContext);
			}
			return asyncResult;
		}

		private void ProcessDocument(object context)
		{
			DocumentContext documentContext = context as DocumentContext;
			Util.ThrowOnNullArgument(documentContext, "context");
			IDocument document = documentContext.Document;
			Util.ThrowOnNullArgument(document, "document");
			AsyncResult asyncResult = documentContext.AsyncResult;
			this.DiagnosticsSession.TraceDebug<IIdentity>("Called begin process - {0} ", document.Identity);
			ComponentException ex = null;
			try
			{
				this.InternalProcessDocument(documentContext);
			}
			catch (OperationFailedException ex2)
			{
				ex = ex2;
			}
			catch (ComponentFailedException ex3)
			{
				this.OnFailed(new FailedEventArgs(ex3));
				ex = ex3;
			}
			catch (Exception ex4)
			{
				if (ex4 is OutOfMemoryException || ex4 is StackOverflowException || ex4 is ThreadAbortException)
				{
					throw;
				}
				if (!(ex4 is QuotaExceededException))
				{
					this.DiagnosticsSession.SendInformationalWatsonReport(ex4, null);
				}
				ex = new PoisonComponentException(ex4);
			}
			if (ex != null)
			{
				this.DiagnosticsSession.TraceError<Type, string>("Received an exception of type = {0} and message = {1} ", ex.GetType(), ex.Message);
				asyncResult.SetAsCompleted(ex);
				return;
			}
			asyncResult.SetAsCompleted();
		}

		protected readonly IDiagnosticsSession DiagnosticsSession;

		private readonly bool isAsynchronous;
	}
}
