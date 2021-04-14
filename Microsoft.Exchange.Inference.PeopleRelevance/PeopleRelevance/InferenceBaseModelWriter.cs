using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal abstract class InferenceBaseModelWriter<TInferenceModelDataBinder, TModelItem> : BaseComponent where TInferenceModelDataBinder : IInferenceModelDataBinder<TModelItem> where TModelItem : InferenceBaseModelItem
	{
		protected InferenceBaseModelWriter(IPipelineComponentConfig config, IPipelineContext context)
		{
			Util.ThrowOnNullArgument(context, "context");
			this.Config = config;
			this.Context = context;
		}

		protected override void InternalProcessDocument(DocumentContext data)
		{
			this.DiagnosticsSession.TraceDebug<IIdentity>("Processing document - {0}", data.Document.Identity);
			TInferenceModelDataBinder modelDataBinder = this.GetModelDataBinder(data.AsyncResult.AsyncState);
			this.DiagnosticsSession.TraceDebug<string>("Got the model data binder", modelDataBinder.GetType().ToString());
			TModelItem modelItem = this.GetModelItem(modelDataBinder);
			this.DiagnosticsSession.TraceDebug("Got the inference model item", new object[0]);
			this.PrepareModelItem(data.Document, modelItem);
			modelItem.LastModifiedTime = ExDateTime.Now.UniversalTime;
			ExAssert.RetailAssert(modelItem.Version != null, "Required Inference model item Version is not set");
			this.WriteModelItem(modelDataBinder, modelItem);
		}

		protected abstract void PrepareModelItem(IDocument document, TModelItem modelItem);

		protected abstract TInferenceModelDataBinder GetModelDataBinder(object context);

		protected abstract void WriteModelItem(TInferenceModelDataBinder dataBinder, TModelItem modelItem);

		protected abstract TModelItem GetModelItem(TInferenceModelDataBinder dataBinder);

		protected readonly IPipelineComponentConfig Config;

		protected readonly IPipelineContext Context;
	}
}
