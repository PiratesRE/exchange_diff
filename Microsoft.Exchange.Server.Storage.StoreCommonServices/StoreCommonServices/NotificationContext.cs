using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class NotificationContext : DisposableBase
	{
		public NotificationContext(INotificationSession session)
		{
			this.session = session;
		}

		public static NotificationContext Current
		{
			get
			{
				return NotificationContext.currentContext;
			}
			set
			{
				NotificationContext.currentContext = value;
			}
		}

		public INotificationSession Session
		{
			get
			{
				return this.session;
			}
		}

		public bool HasPendingEvents
		{
			get
			{
				return this.eventQueue != null || this.dictionaryOfEventQueues != null;
			}
		}

		private int VisitCookie { get; set; }

		internal static void AssignCompletionPort(IoCompletionPort completionPort, uint completionKey)
		{
			using (LockManager.Lock(NotificationContext.pendingContextList))
			{
				if (completionPort != null && !completionPort.IsInvalid)
				{
					NotificationContext.completionPort = completionPort;
					NotificationContext.completionKey = completionKey;
				}
				else
				{
					NotificationContext.completionPort = null;
					NotificationContext.completionKey = 0U;
				}
			}
		}

		internal void EnqueueEvent(NotificationEvent nev)
		{
			using (LockManager.Lock(this, LockManager.LockType.NotificationContext))
			{
				Queue<NotificationEvent> queue;
				if (this.eventQueue == null && this.dictionaryOfEventQueues == null)
				{
					queue = (this.eventQueue = new Queue<NotificationEvent>(16));
				}
				else if (this.eventQueue != null)
				{
					NotificationEvent notificationEvent = this.eventQueue.Peek();
					if (nev.MailboxNumber == notificationEvent.MailboxNumber && nev.MdbGuid == notificationEvent.MdbGuid)
					{
						queue = this.eventQueue;
					}
					else
					{
						this.dictionaryOfEventQueues = new Dictionary<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>>(8);
						this.dictionaryOfEventQueues.Add(new NotificationContext.MailboxIdentifier(notificationEvent.MdbGuid, notificationEvent.MailboxNumber), this.eventQueue);
						this.eventQueue = null;
						queue = new Queue<NotificationEvent>(16);
						this.dictionaryOfEventQueues.Add(new NotificationContext.MailboxIdentifier(nev.MdbGuid, nev.MailboxNumber), queue);
					}
				}
				else
				{
					NotificationContext.MailboxIdentifier key = new NotificationContext.MailboxIdentifier(nev.MdbGuid, nev.MailboxNumber);
					if (!this.dictionaryOfEventQueues.TryGetValue(key, out queue))
					{
						queue = new Queue<NotificationEvent>(16);
						this.dictionaryOfEventQueues.Add(key, queue);
					}
				}
				if (!queue.Contains(nev))
				{
					queue.Enqueue(nev);
					if (this.eventQueue != null && this.eventQueue.Count == 1)
					{
						using (LockManager.Lock(NotificationContext.pendingContextList))
						{
							this.pendingListNode = NotificationContext.pendingContextList.AddLast(this);
							this.VisitCookie = 0;
						}
					}
					using (LockManager.Lock(NotificationContext.pendingContextList))
					{
						if (NotificationContext.completionPort != null)
						{
							NotificationContext.completionPort.PostQueuedCompletionStatus(1U, NotificationContext.completionKey, IntPtr.Zero);
						}
					}
				}
			}
		}

		public NotificationEvent PeekEvent()
		{
			NotificationEvent result = null;
			using (LockManager.Lock(this, LockManager.LockType.NotificationContext))
			{
				if (this.eventQueue != null)
				{
					result = this.eventQueue.Peek();
				}
				else if (this.dictionaryOfEventQueues != null)
				{
					using (Dictionary<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>>.Enumerator enumerator = this.dictionaryOfEventQueues.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>> keyValuePair = enumerator.Current;
							result = keyValuePair.Value.Peek();
						}
					}
				}
			}
			return result;
		}

		public NotificationEvent DequeueEvent(Guid mdbGuid, int mailboxNumber)
		{
			NotificationEvent result = null;
			using (LockManager.Lock(this, LockManager.LockType.NotificationContext))
			{
				if (this.eventQueue != null)
				{
					NotificationEvent notificationEvent = this.eventQueue.Peek();
					if (notificationEvent.MailboxNumber == mailboxNumber && notificationEvent.MdbGuid == mdbGuid)
					{
						result = this.eventQueue.Dequeue();
						if (this.eventQueue.Count == 0)
						{
							if (this.pendingListNode != null)
							{
								using (LockManager.Lock(NotificationContext.pendingContextList))
								{
									NotificationContext.pendingContextList.Remove(this.pendingListNode);
								}
								this.pendingListNode = null;
							}
							this.eventQueue = null;
						}
					}
				}
				else if (this.dictionaryOfEventQueues != null)
				{
					NotificationContext.MailboxIdentifier key = new NotificationContext.MailboxIdentifier(mdbGuid, mailboxNumber);
					Queue<NotificationEvent> queue;
					if (this.dictionaryOfEventQueues.TryGetValue(key, out queue))
					{
						result = queue.Dequeue();
						if (queue.Count == 0)
						{
							this.dictionaryOfEventQueues.Remove(key);
							if (this.dictionaryOfEventQueues.Count == 1)
							{
								using (Dictionary<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>>.Enumerator enumerator = this.dictionaryOfEventQueues.GetEnumerator())
								{
									if (enumerator.MoveNext())
									{
										KeyValuePair<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>> keyValuePair = enumerator.Current;
										this.eventQueue = keyValuePair.Value;
										this.dictionaryOfEventQueues = null;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationContext>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				using (LockManager.Lock(this, LockManager.LockType.NotificationContext))
				{
					if (this.pendingListNode != null)
					{
						if (this.eventQueue != null)
						{
							this.eventQueue.Clear();
							this.eventQueue = null;
						}
						else if (this.dictionaryOfEventQueues != null)
						{
							this.dictionaryOfEventQueues.Clear();
							this.dictionaryOfEventQueues = null;
						}
						using (LockManager.Lock(NotificationContext.pendingContextList))
						{
							NotificationContext.pendingContextList.Remove(this.pendingListNode);
						}
						this.pendingListNode = null;
					}
					this.session = null;
				}
			}
		}

		public static NotificationContext GetNextUnvisitedPendingContext(int visitCookie)
		{
			NotificationContext notificationContext = null;
			using (LockManager.Lock(NotificationContext.pendingContextList))
			{
				LinkedListNode<NotificationContext> first = NotificationContext.pendingContextList.First;
				if (first != null)
				{
					notificationContext = first.Value;
					if (notificationContext.VisitCookie == visitCookie)
					{
						notificationContext = null;
					}
					else
					{
						notificationContext.VisitCookie = visitCookie;
						NotificationContext.pendingContextList.RemoveFirst();
						NotificationContext.pendingContextList.AddLast(first);
					}
				}
			}
			return notificationContext;
		}

		private const int AvgEventsQueuedPerNotificationContext = 16;

		private const int AvgMailboxesPerNotificationContext = 8;

		[ThreadStatic]
		private static NotificationContext currentContext;

		private static LinkedList<NotificationContext> pendingContextList = new LinkedList<NotificationContext>();

		private static IoCompletionPort completionPort = null;

		private static uint completionKey = 0U;

		private INotificationSession session;

		private Queue<NotificationEvent> eventQueue;

		private Dictionary<NotificationContext.MailboxIdentifier, Queue<NotificationEvent>> dictionaryOfEventQueues;

		private LinkedListNode<NotificationContext> pendingListNode;

		private struct MailboxIdentifier : IEquatable<NotificationContext.MailboxIdentifier>
		{
			internal MailboxIdentifier(Guid databaseGuid, int mailboxNumber)
			{
				this.databaseGuid = databaseGuid;
				this.mailboxNumber = mailboxNumber;
			}

			public Guid DatabaseGuid
			{
				get
				{
					return this.databaseGuid;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public override int GetHashCode()
			{
				return this.mailboxNumber;
			}

			public override bool Equals(object obj)
			{
				return obj is NotificationContext.MailboxIdentifier && this.Equals((NotificationContext.MailboxIdentifier)obj);
			}

			public bool Equals(NotificationContext.MailboxIdentifier compare)
			{
				return this.mailboxNumber == compare.mailboxNumber && this.databaseGuid == compare.databaseGuid;
			}

			private Guid databaseGuid;

			private int mailboxNumber;
		}
	}
}
