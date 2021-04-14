using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MailboxDispatcher : Base, IDisposable
	{
		internal static void SetTestHookForBeginningOfSetAsDead(Action testhook)
		{
			MailboxDispatcher.syncWithTestCodeBeginningOfSetAsDead = testhook;
		}

		internal static void SetTestHookForEndOfSetAsDead(Action testhook)
		{
			MailboxDispatcher.syncWithTestCodeEndOfSetAsDead = testhook;
		}

		private MailboxDispatcher(Guid mailboxGuid, EventControllerPrivate controller, int numberOfAssistants)
		{
			this.MailboxGuid = mailboxGuid;
			this.controller = controller;
			this.assistantDispatchers = new Dictionary<Guid, EventDispatcherPrivate>(numberOfAssistants);
		}

		public Guid MailboxGuid { get; private set; }

		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.controller.DatabaseInfo;
			}
		}

		public bool IsMailboxDead
		{
			get
			{
				return this.decayedEventCounter > 0L;
			}
		}

		public long DecayedEventCounter
		{
			get
			{
				return this.decayedEventCounter;
			}
		}

		public string MailboxDisplayName
		{
			get
			{
				ExchangePrincipal exchangePrincipal = this.mailboxOwner;
				if (exchangePrincipal == null)
				{
					return this.MailboxGuid.ToString();
				}
				return exchangePrincipal.MailboxInfo.DisplayName + " (" + this.MailboxGuid.ToString() + ")";
			}
		}

		public bool Shutdown { get; private set; }

		public bool IsIdle
		{
			get
			{
				foreach (EventDispatcherPrivate eventDispatcherPrivate in this.assistantDispatchers.Values)
				{
					if (!eventDispatcherPrivate.IsIdle)
					{
						return false;
					}
				}
				return true;
			}
		}

		public static MailboxDispatcher CreateFromBookmark(EventControllerPrivate controller, EventAccess eventAccess, MapiEvent[] eventTable, Bookmark mailboxBookmark, Bookmark databaseBookmark)
		{
			MailboxDispatcher mailboxDispatcher = new MailboxDispatcher(mailboxBookmark.Identity, controller, controller.Assistants.Count);
			foreach (AssistantCollectionEntry assistantCollectionEntry in controller.Assistants)
			{
				EventDispatcherPrivate value = new EventDispatcherPrivate(mailboxDispatcher, assistantCollectionEntry, controller, mailboxBookmark[assistantCollectionEntry.Identity]);
				mailboxDispatcher.assistantDispatchers.Add(assistantCollectionEntry.Identity, value);
				assistantCollectionEntry.PerformanceCounters.EventDispatchers.Increment();
			}
			foreach (EventDispatcherPrivate eventDispatcherPrivate in mailboxDispatcher.assistantDispatchers.Values)
			{
				eventDispatcherPrivate.Initialize(eventAccess, eventTable, databaseBookmark[eventDispatcherPrivate.AssistantIdentity]);
			}
			return mailboxDispatcher;
		}

		public static MailboxDispatcher CreateWithoutBookmark(EventControllerPrivate controller, EventAccess eventAccess, Guid mailboxGuid, Bookmark databaseBookmark, bool dispatcherIsUpToDate)
		{
			Bookmark mailboxBookmark = eventAccess.GetMailboxBookmark(mailboxGuid, databaseBookmark, dispatcherIsUpToDate);
			return MailboxDispatcher.CreateFromBookmark(controller, eventAccess, null, mailboxBookmark, databaseBookmark);
		}

		public override string ToString()
		{
			return "MailboxDispatcher for " + this.MailboxDisplayName;
		}

		public void Dispose()
		{
			foreach (EventDispatcherPrivate eventDispatcherPrivate in this.assistantDispatchers.Values)
			{
				eventDispatcherPrivate.Dispose();
			}
			this.DisposeMailboxSession();
		}

		public EventDispatcherPrivate GetAssistantDispatcher(Guid assistantId)
		{
			return this.assistantDispatchers[assistantId];
		}

		public void DispatchEvent(InterestingEvent interestingEvent, MailboxDispatcher.MailboxFilterDelegate mailboxFilterMethod, MailboxDispatcher.DispatchDelegate eventHandlerMethod, string nonLocalizedAssistantName)
		{
			lock (this.mailboxSessionLocker)
			{
				this.LoadMailboxOwner(interestingEvent.MapiEvent.TenantHint, nonLocalizedAssistantName);
				if (!mailboxFilterMethod(this.mailboxOwner))
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Discarding event since the assistant is no longer interested in it after inspecting the mailbox owner.", this);
				}
				else
				{
					this.ConnectMailboxSession(nonLocalizedAssistantName);
					eventHandlerMethod(this.mailboxSession);
				}
			}
		}

		public void UpdateWatermark(Guid assistantId, long eventCounter)
		{
			this.assistantDispatchers[assistantId].CommittedWatermark = eventCounter;
		}

		public void RequestShutdown()
		{
			this.Shutdown = true;
		}

		public void WaitForShutdown()
		{
			foreach (EventDispatcherPrivate eventDispatcherPrivate in this.assistantDispatchers.Values)
			{
				eventDispatcherPrivate.WaitForShutdown();
			}
		}

		public void OnWorkersStarted()
		{
			Interlocked.Increment(ref this.numberOfActiveDispatchers);
		}

		public void OnWorkersClear()
		{
			if (Interlocked.Decrement(ref this.numberOfActiveDispatchers) == 0)
			{
				this.DisposeMailboxSession();
			}
		}

		public void ProcessPolledEvent(MapiEvent mapiEvent)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, long>((long)this.GetHashCode(), "{0}: ProcessPolledEvent {1}", this, mapiEvent.EventCounter);
			if (this.IsMailboxDead)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, long>((long)this.GetHashCode(), "{0}: Dead mailbox; discarding event {1}", this, mapiEvent.EventCounter);
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxDeleted) != (MapiEventTypeFlags)0 || ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxMoveSucceeded) != (MapiEventTypeFlags)0 && (mapiEvent.EventFlags & MapiEventFlags.Source) != MapiEventFlags.None))
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Mailbox may have been deleted or moved away", this);
				if (this.DoesMailboxExist())
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, MapiEvent>((long)this.GetHashCode(), "{0}: Mailbox still exists while processing mapiEvent {1}", this, mapiEvent);
				}
				else
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Mailbox no longer exists on this database", this);
					this.SetAsDeadMailbox(mapiEvent.EventCounter, mapiEvent.EventCounter);
				}
			}
			EmergencyKit emergencyKit = new EmergencyKit(mapiEvent);
			int num = 0;
			foreach (EventDispatcherPrivate eventDispatcherPrivate in this.assistantDispatchers.Values)
			{
				bool flag = eventDispatcherPrivate.IsAssistantInterestedInMailbox(this.mailboxOwner);
				if (flag)
				{
					bool flag2 = eventDispatcherPrivate.ProcessPolledEvent(emergencyKit);
					if (flag2)
					{
						num++;
					}
				}
			}
			if (num > 0)
			{
				this.controller.DatabaseCounters.InterestingEvents.Increment();
				if (num > 1)
				{
					this.controller.DatabaseCounters.EventsInterestingToMultipleAsssitants.Increment();
				}
			}
			this.controller.DatabaseCounters.InterestingEventsBase.Increment();
		}

		public void SetAsDeadMailbox(long problemEventCounter, long decayedEventCounter)
		{
			if (MailboxDispatcher.syncWithTestCodeBeginningOfSetAsDead != null)
			{
				MailboxDispatcher.syncWithTestCodeBeginningOfSetAsDead();
			}
			lock (this.deadMailboxLocker)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, long, long>((long)this.GetHashCode(), "{0}: Setting mailbox dead events [{1}-{2}]", this, problemEventCounter, decayedEventCounter);
				if (this.IsMailboxDead)
				{
					this.decayedEventCounter = Math.Min(this.decayedEventCounter, decayedEventCounter);
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, long>((long)this.GetHashCode(), "{0}: this.decayedEventCounter: {1}", this, this.decayedEventCounter);
				}
				else
				{
					this.decayedEventCounter = decayedEventCounter;
					base.LogEvent(AssistantsEventLogConstants.Tuple_DeadMailbox, null, new object[]
					{
						problemEventCounter,
						decayedEventCounter,
						this.MailboxDisplayName,
						(this.mailboxOwner == null) ? null : this.mailboxOwner.MailboxInfo.OrganizationId.GetTenantGuid().ToString(),
						(this.mailboxOwner == null) ? null : this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
						this.DatabaseInfo.DisplayName
					});
					foreach (EventDispatcherPrivate eventDispatcherPrivate in this.assistantDispatchers.Values)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Clearing event queue", this);
						eventDispatcherPrivate.ClearPendingQueue();
					}
					this.controller.DeadDispatcher(this);
				}
			}
			if (MailboxDispatcher.syncWithTestCodeEndOfSetAsDead != null)
			{
				MailboxDispatcher.syncWithTestCodeEndOfSetAsDead();
			}
		}

		public IList<EventDispatcherPrivate> GetEventDispatcher(Guid? assistantGuid)
		{
			List<EventDispatcherPrivate> list = new List<EventDispatcherPrivate>();
			lock (this.assistantDispatchers)
			{
				foreach (KeyValuePair<Guid, EventDispatcherPrivate> keyValuePair in this.assistantDispatchers)
				{
					if (assistantGuid == null || keyValuePair.Key == assistantGuid)
					{
						list.Add(keyValuePair.Value);
					}
				}
			}
			return list;
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableMailboxDispatcher queryableMailboxDispatcher = queryableObject as QueryableMailboxDispatcher;
			if (queryableMailboxDispatcher != null)
			{
				queryableMailboxDispatcher.MailboxGuid = this.MailboxGuid;
				queryableMailboxDispatcher.DecayedEventCounter = this.decayedEventCounter;
				queryableMailboxDispatcher.NumberOfActiveDispatchers = this.numberOfActiveDispatchers;
				queryableMailboxDispatcher.IsMailboxDead = this.IsMailboxDead;
				queryableMailboxDispatcher.IsIdle = this.IsIdle;
			}
		}

		private void ThrowAppropriateSessionException(Exception e)
		{
			if (this.DoesMailboxExist())
			{
				throw new DisconnectedMailboxException(e);
			}
			throw new DeadMailboxException(e);
		}

		private void ConnectMailboxSession(string nonLocalizedAssistantName)
		{
			this.InvokeAndMapException(delegate
			{
				if (this.mailboxSession == null)
				{
					this.mailboxSession = this.CreateMailboxSession();
				}
			}, nonLocalizedAssistantName);
		}

		private void LoadMailboxOwner(byte[] tenantPartitionHintBlob, string nonLocalizedAssistantName)
		{
			if (this.mailboxOwner == null)
			{
				this.InvokeAndMapException(delegate
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Looking up ExchangePrincipal...", this);
					ADSessionSettings adSettings;
					if (tenantPartitionHintBlob != null && tenantPartitionHintBlob.Length != 0)
					{
						adSettings = ADSessionSettings.FromTenantPartitionHint(TenantPartitionHint.FromPersistablePartitionHint(tenantPartitionHintBlob));
					}
					else
					{
						adSettings = ADSessionSettings.FromRootOrgScopeSet();
					}
					this.mailboxOwner = ExchangePrincipal.FromLocalServerMailboxGuid(adSettings, this.DatabaseInfo.Guid, this.MailboxGuid);
				}, nonLocalizedAssistantName);
			}
		}

		private void InvokeAndMapException(MailboxDispatcher.OpenMailboxDelegate method, string nonLocalizedAssistantName)
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: InvokeAndMapException", this);
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					try
					{
						method();
					}
					catch (WrongServerException ex2)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceError<MailboxDispatcher, WrongServerException>((long)this.GetHashCode(), "{0}: Unable to open session: {1}", this, ex2);
						this.ThrowAppropriateSessionException(ex2);
					}
					catch (ObjectNotFoundException ex3)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceError<MailboxDispatcher, ObjectNotFoundException>((long)this.GetHashCode(), "{0}: User account/mailbox not found: {1}", this, ex3);
						this.ThrowAppropriateSessionException(ex3);
					}
					catch (DataValidationException ex4)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceError<MailboxDispatcher, DataValidationException>((long)this.GetHashCode(), "{0}: User object is not valid: {1}", this, ex4);
						throw new MailboxIneptException(ex4);
					}
				}, nonLocalizedAssistantName);
			}
			catch (AIException ex)
			{
				ExTraceGlobals.EventDispatcherTracer.TraceError<MailboxDispatcher, AIException>((long)this.GetHashCode(), "{0}: Could not open mailbox: {1}", this, ex);
				base.LogEvent(AssistantsEventLogConstants.Tuple_MailboxSessionException, null, new object[]
				{
					this.MailboxDisplayName,
					(this.mailboxOwner == null) ? null : this.mailboxOwner.MailboxInfo.OrganizationId.GetTenantGuid().ToString(),
					(this.mailboxOwner == null) ? null : this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
					this.DatabaseInfo.DisplayName,
					ex
				});
				throw;
			}
		}

		private void DisposeMailboxSession()
		{
			lock (this.mailboxSessionLocker)
			{
				if (this.mailboxSession != null)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Disposing of the mailbox session", this);
					this.mailboxSession.Dispose();
					this.mailboxSession = null;
					this.mailboxOwner = null;
					this.controller.DatabaseCounters.MailboxSessionsInUseByDispatchers.Decrement();
				}
			}
		}

		private bool DoesMailboxExist()
		{
			ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Searching for mailbox", this);
			if (this.MailboxGuid == Guid.Empty)
			{
				return false;
			}
			bool result;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=EBA", null, null, null, null))
			{
				try
				{
					PropValue[][] mailboxTableInfo = exRpcAdmin.GetMailboxTableInfo(this.DatabaseInfo.Guid, this.MailboxGuid, MailboxTableFlags.IncludeSoftDeletedMailbox, new PropTag[]
					{
						PropTag.MailboxMiscFlags
					});
					if (mailboxTableInfo.Length < 1 || mailboxTableInfo[0].Length < 1)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: GetMailboxTableInfo returned invalid response", this);
						return false;
					}
					MailboxMiscFlags @int = (MailboxMiscFlags)mailboxTableInfo[0][0].GetInt(0);
					if ((@int & MailboxMiscFlags.CreatedByMove) != MailboxMiscFlags.None)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Mailbox is the destination mailbox for the move.", this);
						return true;
					}
					if ((@int & (MailboxMiscFlags.DisabledMailbox | MailboxMiscFlags.SoftDeletedMailbox | MailboxMiscFlags.MRSSoftDeletedMailbox)) != MailboxMiscFlags.None)
					{
						ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, string>((long)this.GetHashCode(), "{0}: Mailbox exists in mailbox table, but marked as inaccessible: {1}", this, @int.ToString());
						return false;
					}
				}
				catch (MapiExceptionNotFound arg)
				{
					ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher, MapiExceptionNotFound>((long)this.GetHashCode(), "{0}: Mailbox does not exist: {1}", this, arg);
					return false;
				}
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Mailbox exists", this);
				result = true;
			}
			return result;
		}

		private MailboxSession CreateMailboxSession()
		{
			MailboxSession mailboxSession = null;
			bool flag = false;
			try
			{
				ExTraceGlobals.EventDispatcherTracer.TraceDebug<MailboxDispatcher>((long)this.GetHashCode(), "{0}: Creating mailbox session...", this);
				mailboxSession = this.DatabaseInfo.GetMailbox(this.mailboxOwner, ClientType.EventBased, "EventDispatcher");
				base.TracePfd("PFD AIS {0} {1}: Created mailbox session.", new object[]
				{
					30807,
					this
				});
				mailboxSession.ExTimeZone = ExTimeZone.CurrentTimeZone;
				flag = true;
			}
			finally
			{
				if (!flag && mailboxSession != null)
				{
					mailboxSession.Dispose();
				}
			}
			this.controller.DatabaseCounters.MailboxSessionsInUseByDispatchers.Increment();
			return mailboxSession;
		}

		private static Action syncWithTestCodeBeginningOfSetAsDead;

		private static Action syncWithTestCodeEndOfSetAsDead;

		private Dictionary<Guid, EventDispatcherPrivate> assistantDispatchers;

		private EventControllerPrivate controller;

		private long decayedEventCounter;

		private object deadMailboxLocker = new object();

		private object mailboxSessionLocker = new object();

		private ExchangePrincipal mailboxOwner;

		private MailboxSession mailboxSession;

		private int numberOfActiveDispatchers;

		public delegate bool MailboxFilterDelegate(ExchangePrincipal mailboxOwner);

		public delegate void DispatchDelegate(MailboxSession mailboxSession);

		private delegate void OpenMailboxDelegate();
	}
}
