using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Engine
{
	internal class DocumentTracker : IDocumentTracker, IDiagnosable
	{
		internal DocumentTracker()
		{
			this.tracingContext = this.GetHashCode();
			this.TrackedDocuments = new HashSet<DocumentTracker.DocumentInfo>();
			this.RetriablePoisonDocumentsToStamp = new HashSet<long>();
			this.PermanentPoisonDocumentsToStamp = new HashSet<long>();
			this.PermanentPoisonDocuments = new HashSet<long>();
		}

		public int PoisonDocumentsCount
		{
			get
			{
				int count;
				lock (this.lockObject)
				{
					count = this.PermanentPoisonDocuments.Count;
				}
				return count;
			}
		}

		internal HashSet<DocumentTracker.DocumentInfo> TrackedDocuments { get; private set; }

		internal HashSet<long> RetriablePoisonDocumentsToStamp { get; private set; }

		internal HashSet<long> PermanentPoisonDocumentsToStamp { get; private set; }

		internal HashSet<long> PermanentPoisonDocuments { get; private set; }

		public void Initialize(IFailedItemStorage failedItemStorage)
		{
			if (!this.initialized)
			{
				lock (this.lockObject)
				{
					this.PermanentPoisonDocuments = new HashSet<long>(failedItemStorage.GetPoisonDocuments());
					this.initialized = true;
					this.tracer.TraceDebug<int>((long)this.tracingContext, "Permanent Poison Documents collection initialized with count: {0}", this.PermanentPoisonDocuments.Count);
				}
			}
		}

		public void RecordDocumentProcessing(Guid instanceId, Guid correlationId, long docId)
		{
			DocumentTracker.DocumentInfo item = new DocumentTracker.DocumentInfo(instanceId, correlationId, docId);
			lock (this.lockObject)
			{
				this.TrackedDocuments.Remove(item);
				this.TrackedDocuments.Add(item);
				this.tracer.TraceDebug<Guid>((long)this.tracingContext, "Tracking document with CorrelationId: {0}", correlationId);
				this.tracer.TraceDebug<int>((long)this.tracingContext, "Total number of documents tracked by this Tracker: {0}", this.TrackedDocuments.Count);
			}
		}

		public void RecordDocumentProcessingComplete(Guid correlationId, long docId, bool isTrackedAsPoison)
		{
			lock (this.lockObject)
			{
				foreach (DocumentTracker.DocumentInfo documentInfo in this.TrackedDocuments)
				{
					if (documentInfo.CorrelationId.Equals(correlationId))
					{
						this.TrackedDocuments.Remove(documentInfo);
						break;
					}
				}
				if (isTrackedAsPoison)
				{
					this.RetriablePoisonDocumentsToStamp.Remove(docId);
					if (this.PermanentPoisonDocumentsToStamp.Remove(docId))
					{
						this.PermanentPoisonDocuments.Add(docId);
					}
				}
			}
		}

		public void MarkCurrentlyTrackedDocumentsAsPoison()
		{
			lock (this.lockObject)
			{
				foreach (DocumentTracker.DocumentInfo documentInfo in this.TrackedDocuments)
				{
					if (!this.PermanentPoisonDocumentsToStamp.Contains(documentInfo.DocId) && !this.PermanentPoisonDocuments.Contains(documentInfo.DocId))
					{
						this.RetriablePoisonDocumentsToStamp.Add(documentInfo.DocId);
						this.tracer.TraceDebug<Guid>((long)this.tracingContext, "Moving currently tracked document to be considered poisonous. CorrelationId: {0}", documentInfo.CorrelationId);
					}
				}
				this.TrackedDocuments.Clear();
			}
		}

		public void MarkDocumentAsPoison(long docId)
		{
			if (IndexId.IsWatermarkIndexId(docId))
			{
				return;
			}
			lock (this.lockObject)
			{
				this.PermanentPoisonDocumentsToStamp.Add(docId);
				this.RetriablePoisonDocumentsToStamp.Remove(docId);
			}
		}

		public void MarkDocumentAsRetriablePoison(long docId)
		{
			if (IndexId.IsWatermarkIndexId(docId))
			{
				return;
			}
			lock (this.lockObject)
			{
				if (!this.PermanentPoisonDocumentsToStamp.Contains(docId))
				{
					this.RetriablePoisonDocumentsToStamp.Add(docId);
				}
			}
		}

		public int ShouldDocumentBeStampedWithError(long docId)
		{
			int result = 0;
			lock (this.lockObject)
			{
				if (this.PermanentPoisonDocumentsToStamp.Contains(docId))
				{
					result = EvaluationErrorsHelper.MakePermanentError(EvaluationErrors.PoisonDocument);
				}
				else if (this.RetriablePoisonDocumentsToStamp.Contains(docId))
				{
					result = EvaluationErrorsHelper.MakeRetriableError(EvaluationErrors.PoisonDocument);
				}
			}
			return result;
		}

		public bool ShouldDocumentBeSkipped(long docId)
		{
			return this.PermanentPoisonDocuments.Contains(docId);
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			foreach (DocumentTracker.DocumentInfo documentInfo in this.TrackedDocuments)
			{
				int mailboxNumber = IndexId.GetMailboxNumber(documentInfo.DocId);
				int documentId = IndexId.GetDocumentId(documentInfo.DocId);
				XElement xelement2 = new XElement("DocumentInformation");
				xelement2.Add(new XElement("FlowInstance", documentInfo.InstanceId));
				xelement2.Add(new XElement("MailboxNumber", mailboxNumber));
				xelement2.Add(new XElement("DocumentId", documentId));
				xelement2.Add(new XElement("CorrelationId", documentInfo.CorrelationId));
				xelement2.Add(new XElement("TimeStamp", documentInfo.TimeStamp));
				xelement.Add(xelement2);
			}
			return xelement;
		}

		public string GetDiagnosticComponentName()
		{
			return base.GetType().Name;
		}

		private readonly Trace tracer = ExTraceGlobals.DocumentTrackerOperatorTracer;

		private readonly int tracingContext;

		private readonly object lockObject = new object();

		private bool initialized;

		public class DocumentInfo : IEquatable<DocumentTracker.DocumentInfo>
		{
			public DocumentInfo(Guid instanceId, Guid correlationId, long docId)
			{
				this.DocId = docId;
				this.CorrelationId = correlationId;
				this.InstanceId = instanceId;
				this.TimeStamp = DateTime.UtcNow;
			}

			public long DocId { get; private set; }

			public Guid CorrelationId { get; private set; }

			public Guid InstanceId { get; private set; }

			public DateTime TimeStamp { get; private set; }

			public bool Equals(DocumentTracker.DocumentInfo other)
			{
				return this.InstanceId.Equals(other.InstanceId);
			}

			public override int GetHashCode()
			{
				return this.InstanceId.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("InstanceId: {0}, DocumentId: {1}, CorrelationId: {2},", this.InstanceId, this.DocId, this.CorrelationId);
			}
		}
	}
}
