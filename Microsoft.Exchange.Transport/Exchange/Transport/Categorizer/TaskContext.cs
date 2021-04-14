using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class TaskContext : IDisposable
	{
		internal TaskContext(int stage, TransportMailItem subjectMailItem, int latestMimeVersion, WeakReference lastKnownMimeDocument, Job job, IMExSession mexSession, AgentLatencyTracker agentLatencyTracker, AcceptedDomainCollection acceptedDomains)
		{
			this.Stage = stage;
			this.subjectTransportMailItem = subjectMailItem;
			this.latestMimeVersion = latestMimeVersion;
			this.lastKnownMimeDocument = lastKnownMimeDocument;
			this.job = job;
			if (stage >= this.job.Stages.Count)
			{
				throw new ArgumentOutOfRangeException("stage");
			}
			if (mexSession == null && agentLatencyTracker != null)
			{
				throw new InvalidOperationException("Non-null agentLatencyTracker for a null MEx session");
			}
			if (mexSession != null && agentLatencyTracker == null)
			{
				throw new InvalidOperationException("Null agentLatencyTracker for a non-null MEx session");
			}
			if (acceptedDomains == null)
			{
				throw new ArgumentNullException("acceptedDomains");
			}
			this.mexSession = mexSession;
			this.agentLatencyTracker = agentLatencyTracker;
			this.acceptedDomains = acceptedDomains;
			if (mexSession == null)
			{
				this.mexSession = MExEvents.GetExecutionContext(subjectMailItem, this.acceptedDomains, new Action(this.TrackAsyncMessage), new Action(this.TrackAsyncMessageCompleted), new Func<bool>(this.SavePoisonContext));
				this.agentLatencyTracker = new AgentLatencyTracker(this.mexSession);
			}
		}

		public TaskContext FriendNextTaskContext
		{
			get
			{
				return this.nextTaskContext;
			}
			set
			{
				this.nextTaskContext = value;
			}
		}

		internal IMExSession MexSession
		{
			get
			{
				return this.mexSession;
			}
		}

		internal AgentLatencyTracker AgentLatencyTracker
		{
			get
			{
				return this.agentLatencyTracker;
			}
		}

		internal TransportMailItem SubjectMailItem
		{
			get
			{
				return this.subjectTransportMailItem;
			}
		}

		internal bool MessageDeferred
		{
			get
			{
				return this.messageDeferred;
			}
			set
			{
				this.messageDeferred = value;
				if (this.messageDeferred)
				{
					this.job.MarkDeferred(this.SubjectMailItem);
				}
			}
		}

		internal TimeSpan DeferTime
		{
			get
			{
				return this.deferTime;
			}
			set
			{
				this.deferTime = value;
			}
		}

		public static void ReleaseItem(TransportMailItem mailItem)
		{
			Job.ReleaseItem(mailItem);
		}

		public TaskCompletion Invoke()
		{
			TaskCompletion result;
			using (new MailItemTraceFilter(this.subjectTransportMailItem))
			{
				this.SavePoisonContext();
				this.BeginTrackStageLatency(this.subjectTransportMailItem);
				if (this.subjectTransportMailItem.ActivityScope != null)
				{
					ActivityContext.SetThreadScope(this.subjectTransportMailItem.ActivityScope);
				}
				TaskCompletion taskCompletion = TaskCompletion.Completed;
				try
				{
					taskCompletion = this.job.Stages[this.Stage].Handler(this.subjectTransportMailItem, this);
				}
				catch (Exception e)
				{
					if (!Components.CategorizerComponent.HandleComponentException(e, this))
					{
						throw;
					}
				}
				if (taskCompletion == TaskCompletion.Completed)
				{
					this.TaskCompletedSync();
				}
				result = taskCompletion;
			}
			return result;
		}

		public void TaskCompletedSync()
		{
			this.TaskCompleted(false);
		}

		public void TaskCompletedAsync()
		{
			this.TaskCompleted(true);
		}

		public void TaskRetired()
		{
			this.Dispose();
		}

		public bool TryGetDeferToken(TransportMailItem mailItem, out AcquireToken deferToken)
		{
			return this.job.TryGetDeferToken(mailItem, out deferToken);
		}

		public void ChainItemToNext(TransportMailItem transportMailItem)
		{
			if (this.completed)
			{
				throw new InvalidOperationException("Cannot invoke ChainToNext for a completed task");
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "ChainItemToNext (msgId={0})", transportMailItem.RecordId);
			this.EndTrackStageLatency(transportMailItem);
			ActivityContext.ClearThreadScope();
			this.job.EnqueuePendingTask(this.Stage + 1, transportMailItem, this.latestMimeVersion, this.lastKnownMimeDocument, this.mexSession, this.agentLatencyTracker, this.acceptedDomains);
			this.mexSession = null;
			this.agentLatencyTracker = null;
		}

		public TransportMailItem ForkItem(TransportMailItem continueTransportMailItem, IList<MailRecipient> continueRecipients)
		{
			if (continueRecipients.Count == 0)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug((long)this.GetHashCode(), "0 continue recipients to Fork.");
				return null;
			}
			List<MailRecipient> list = new List<MailRecipient>();
			MailRecipientCollection recipients = continueTransportMailItem.Recipients;
			for (int i = 0; i < recipients.Count; i++)
			{
				MailRecipient mailRecipient = recipients[i];
				if (mailRecipient.IsActive && !continueRecipients.Contains(mailRecipient))
				{
					list.Add(mailRecipient);
				}
			}
			if (list.Count == 0)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug((long)this.GetHashCode(), "0 defer recipients to Fork.");
				return null;
			}
			TransportMailItem transportMailItem = continueTransportMailItem.NewCloneWithoutRecipients();
			foreach (MailRecipient mailRecipient2 in list)
			{
				mailRecipient2.MoveTo(transportMailItem);
			}
			transportMailItem.CommitLazy();
			this.agentLatencyTracker.EndTrackingCurrentEvent(transportMailItem.LatencyTracker);
			this.ChainItemToSelf(transportMailItem, this.mexSession);
			return transportMailItem;
		}

		internal TransportMailItemWrapper CreatePublicWrapper(bool canAddRecipients)
		{
			if (this.currentTransportMailItemWrapper != null)
			{
				throw new InvalidOperationException("Can not create public TransportMailItemWrapper while a wrapper still exists");
			}
			this.currentTransportMailItemWrapper = new TransportMailItemWrapper(this.subjectTransportMailItem, this.mexSession, canAddRecipients);
			return this.currentTransportMailItemWrapper;
		}

		public void ClosePublicWrapper()
		{
			if (this.currentTransportMailItemWrapper == null)
			{
				throw new InvalidOperationException("No public TransportMailItemWrapper exists to close");
			}
			this.currentTransportMailItemWrapper.CloseWrapper();
			this.currentTransportMailItemWrapper = null;
		}

		public void ChainItemToSelf(TransportMailItem transportMailItem, IMExSession mexSession)
		{
			if (this.completed)
			{
				throw new InvalidOperationException("Cannot invoke ChainItemToSelf for a completed task");
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug<long>((long)this.GetHashCode(), "ChainItemToSelf (msgId={0})", transportMailItem.RecordId);
			this.EndTrackStageLatency(transportMailItem);
			ActivityContext.ClearThreadScope();
			IMExSession session = null;
			AgentLatencyTracker agentLatencyTracker = null;
			if (mexSession != null)
			{
				session = MExEvents.CloneExecutionContext(mexSession);
				agentLatencyTracker = new AgentLatencyTracker(session);
			}
			Components.CategorizerComponent.MailItemBifurcatedInCategorizer(transportMailItem);
			this.job.EnqueuePendingTask(this.Stage, transportMailItem, 0, null, session, agentLatencyTracker, this.acceptedDomains);
		}

		public void ChainItemToSelf(TransportMailItem transportMailItem)
		{
			this.ChainItemToSelf(transportMailItem, this.mexSession);
		}

		public void BeginTrackStageLatency(TransportMailItem mailItem)
		{
			if (this.job.Stages[this.Stage].LatencyComponent != LatencyComponent.None)
			{
				LatencyTracker.BeginTrackLatency(this.job.Stages[this.Stage].LatencyComponent, mailItem.LatencyTracker);
			}
		}

		public void EndTrackStageLatency(TransportMailItem mailItem)
		{
			if (this.job.Stages[this.Stage].LatencyComponent != LatencyComponent.None)
			{
				LatencyTracker.EndTrackLatency(this.job.Stages[this.Stage].LatencyComponent, mailItem.LatencyTracker);
				return;
			}
			if (this.agentLatencyTracker != null)
			{
				this.agentLatencyTracker.EndTrackingCurrentEvent(mailItem.LatencyTracker);
			}
		}

		public void SaveMimeVersion(TransportMailItem mailItem)
		{
			this.latestMimeVersion = mailItem.MimeDocument.Version;
			this.lastKnownMimeDocument = new WeakReference(mailItem.MimeDocument);
		}

		public void PromoteHeadersIfChanged(TransportMailItem mailItem)
		{
			object objA = (this.lastKnownMimeDocument == null) ? null : this.lastKnownMimeDocument.Target;
			if (!object.ReferenceEquals(objA, mailItem.MimeDocument) || this.latestMimeVersion != mailItem.MimeDocument.Version)
			{
				this.SaveMimeVersion(mailItem);
				mailItem.UpdateCachedHeaders();
			}
		}

		private void TaskCompleted(bool async)
		{
			if (this.completed)
			{
				throw new InvalidOperationException("Cannot call Completed more than one for a Task");
			}
			this.completed = true;
			this.Dispose();
			this.job.RunningTaskCompleted(this, async);
		}

		private bool SavePoisonContext()
		{
			return TransportMailItem.SetPoisonContext(this.subjectTransportMailItem.RecordId, this.subjectTransportMailItem.InternetMessageId, MessageProcessingSource.Categorizer);
		}

		private void TrackAsyncMessage()
		{
			TransportMailItem.TrackAsyncMessage(this.subjectTransportMailItem.InternetMessageId);
		}

		private void TrackAsyncMessageCompleted()
		{
			TransportMailItem.TrackAsyncMessageCompleted(this.subjectTransportMailItem.InternetMessageId);
		}

		public void Dispose()
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug((long)this.GetHashCode(), "Dispose TaskContext object");
			if (this.mexSession != null)
			{
				this.agentLatencyTracker.Dispose();
				this.agentLatencyTracker = null;
				MExEvents.FreeExecutionContext(this.mexSession);
				this.mexSession = null;
			}
		}

		public readonly int Stage;

		private readonly TransportMailItem subjectTransportMailItem;

		private readonly Job job;

		private IMExSession mexSession;

		private AgentLatencyTracker agentLatencyTracker;

		private AcceptedDomainCollection acceptedDomains;

		private bool messageDeferred;

		private TimeSpan deferTime;

		private TaskContext nextTaskContext;

		private bool completed;

		private TransportMailItemWrapper currentTransportMailItemWrapper;

		private int latestMimeVersion = int.MinValue;

		private WeakReference lastKnownMimeDocument;
	}
}
