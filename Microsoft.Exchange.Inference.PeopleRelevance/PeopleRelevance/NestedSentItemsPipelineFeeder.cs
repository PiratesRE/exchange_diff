using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Inference.MdbCommon;
using Microsoft.Exchange.Inference.Pipeline;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.Pipeline;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NestedSentItemsPipelineFeeder : NestedPipelineFeederBase
	{
		internal NestedSentItemsPipelineFeeder(IPipelineComponentConfig config, IPipelineContext context, IPipeline pipeline) : base(config, context, pipeline)
		{
			this.DiagnosticsSession.Tracer = ExTraceGlobals.NestedSentItemsPipelineFeederTracer;
			this.DiagnosticsSession.ComponentName = "NestedSentItemsPipelineFeeder";
		}

		public override string Description
		{
			get
			{
				return "NestedSentItemsPipelineFeeder is responsible for feeding FullDocuments to the nested sent items pipeline.";
			}
		}

		public override string Name
		{
			get
			{
				return "NestedSentItemsPipelineFeeder";
			}
		}

		protected override void InternalProcessDocument(DocumentContext data)
		{
			object obj = null;
			if (data.Document.TryGetProperty(PeopleRelevanceSchema.IsBasedOnRecipientInfoData, out obj) && (bool)obj)
			{
				this.DiagnosticsSession.TraceDebug("RecipientInfo-based relevance is enabled. Skip processing SentItems", new object[0]);
				return;
			}
			DocumentProcessingContext documentProcessingContext = data.AsyncResult.AsyncState as DocumentProcessingContext;
			Util.ThrowOnConditionFailed(documentProcessingContext != null, "Context object passed in async call is not of type DocumentProcessingContext");
			Util.ThrowOnMismatchType<Document>(data.Document, "data.Document");
			Document document = data.Document as Document;
			this.DiagnosticsSession.TraceDebug<IIdentity>("Processing document - {0}", document.Identity);
			IList<IDocument> list = null;
			IDocument property = document.GetProperty<IDocument>(PeopleRelevanceSchema.SentItemsTrainingSubDocument);
			this.DiagnosticsSession.TraceDebug<int>("Count of nested documents in the training set - {0}", property.NestedDocuments.Count);
			if (document.TryGetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, out obj))
			{
				document.SetProperty(PeopleRelevanceSchema.TimeWindowNumberAtLastRun, (long)obj);
			}
			bool flag = false;
			foreach (IDocument document2 in property.NestedDocuments)
			{
				if (base.PipelineContext != null && InferencePipelineUtil.IsAbortOnProcessingRequested((PipelineContext)base.PipelineContext))
				{
					flag = true;
					break;
				}
				this.DiagnosticsSession.TraceDebug<IIdentity>("Processing mini document - {0}", document2.Identity);
				using (MdbDocument mdbDocument = MdbInferenceFactory.Current.CreateFullDocument(document2, documentProcessingContext, MdbPeoplePropertyMap.Instance, PeopleRelevanceDocumentFactory.SentItemsFullDocumentProperties))
				{
					this.DiagnosticsSession.TraceDebug<IIdentity>("Creating full document succeeded - {0}", mdbDocument.Identity);
					if (document.TryGetProperty(PeopleRelevanceSchema.ContactList, out obj))
					{
						mdbDocument.SetProperty(PeopleRelevanceSchema.ContactList, obj);
					}
					if (document.TryGetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, out obj))
					{
						mdbDocument.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, obj);
						mdbDocument.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, document.GetProperty<long>(PeopleRelevanceSchema.CurrentTimeWindowNumber));
					}
					if (document.TryGetProperty(PeopleRelevanceSchema.MailboxOwner, out obj))
					{
						mdbDocument.SetProperty(PeopleRelevanceSchema.MailboxOwner, obj);
					}
					this.DiagnosticsSession.TraceDebug<IIdentity>("Pushing full document to the nested pipeline - {0}", mdbDocument.Identity);
					try
					{
						this.NestedPipeline.ProcessDocument(mdbDocument, documentProcessingContext);
						this.DiagnosticsSession.TraceDebug<IIdentity>("Processing full document in the nested pipeline completed without exceptions - {0}", mdbDocument.Identity);
						if (mdbDocument.TryGetProperty(PeopleRelevanceSchema.SentTime, out obj))
						{
							document2.SetProperty(PeopleRelevanceSchema.SentTime, obj);
						}
						else
						{
							this.DiagnosticsSession.TraceDebug<IIdentity>("Missing sent time property in full document after completing nested pipeline - {0}", mdbDocument.Identity);
						}
						IDictionary<string, IInferenceRecipient> property2 = mdbDocument.GetProperty<IDictionary<string, IInferenceRecipient>>(PeopleRelevanceSchema.ContactList);
						document.SetProperty(PeopleRelevanceSchema.ContactList, property2);
						ExDateTime property3 = mdbDocument.GetProperty<ExDateTime>(PeopleRelevanceSchema.CurrentTimeWindowStartTime);
						document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, property3);
						long property4 = mdbDocument.GetProperty<long>(PeopleRelevanceSchema.CurrentTimeWindowNumber);
						document.SetProperty(PeopleRelevanceSchema.CurrentTimeWindowNumber, property4);
					}
					catch (ComponentException ex)
					{
						if (list == null)
						{
							list = new List<IDocument>(property.NestedDocuments.Count);
						}
						this.DiagnosticsSession.TraceError<IIdentity, string>("Received an operation failed exception while processing mini document - {0} message- {1}", document2.Identity, ex.Message);
						this.DiagnosticsSession.TraceDebug<IIdentity>("Adding the mini document - {0} to the failed Documents collection", document2.Identity);
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, string.Format("U={0} - Received an operation failed exception while processing mini document - {1} message- {2}", documentProcessingContext.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, document2.Identity, ex.Message), new object[0]);
						list.Add(document2);
					}
				}
			}
			if (flag)
			{
				throw new AbortOnProcessingRequestedException();
			}
			int num = 0;
			if (list != null && list.Count > 0)
			{
				num = list.Count;
				this.DiagnosticsSession.TraceDebug<int>("Count of failed Documents - {0}", num);
				property.RemoveDocuments(list);
				if (property.NestedDocuments.Count == 0)
				{
					throw new NestedDocumentCountZeroException();
				}
			}
			this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - NestedSentItemsPipeline: {1} messages processed successfully, {2} messages failed", documentProcessingContext.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, property.NestedDocuments.Count, num), new object[0]);
		}

		[Conditional("DEBUG")]
		private void SleepToAttachDebugger()
		{
			if (PeopleRelevanceConfig.Instance.SleepTimeBeforeInferenceProcessingStarts > TimeSpan.Zero)
			{
				Thread.Sleep(PeopleRelevanceConfig.Instance.SleepTimeBeforeInferenceProcessingStarts);
			}
		}

		private const string ComponentName = "NestedSentItemsPipelineFeeder";

		private const string ComponentDescription = "NestedSentItemsPipelineFeeder is responsible for feeding FullDocuments to the nested sent items pipeline.";
	}
}
