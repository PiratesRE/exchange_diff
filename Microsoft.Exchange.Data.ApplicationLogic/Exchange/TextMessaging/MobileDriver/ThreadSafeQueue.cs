using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class ThreadSafeQueue<T> : DisposeTrackableBase
	{
		public ThreadSafeQueue() : this(true)
		{
		}

		public ThreadSafeQueue(bool eventBased)
		{
			this.Lock = new ReaderWriterLockSlim();
			this.LinkedList = new LinkedList<T>();
			if (eventBased)
			{
				this.DataAvailable = new ManualResetEvent(false);
			}
		}

		public ManualResetEvent DataAvailable { get; private set; }

		private LinkedList<T> LinkedList { get; set; }

		private ReaderWriterLockSlim Lock { get; set; }

		private bool EventPaused { get; set; }

		public void PauseEvent()
		{
			base.CheckDisposed();
			try
			{
				this.Lock.EnterUpgradeableReadLock();
				if (!this.EventPaused)
				{
					try
					{
						this.Lock.EnterWriteLock();
						this.EventPaused = true;
						this.ResetDataAvailableEvent();
					}
					finally
					{
						try
						{
							this.Lock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			finally
			{
				try
				{
					this.Lock.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void ResumeEvent()
		{
			base.CheckDisposed();
			try
			{
				this.Lock.EnterUpgradeableReadLock();
				if (this.EventPaused)
				{
					try
					{
						this.Lock.EnterWriteLock();
						this.EventPaused = false;
						if (0 < this.LinkedList.Count)
						{
							this.SetDataAvailableEvent();
						}
					}
					finally
					{
						try
						{
							this.Lock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			finally
			{
				try
				{
					this.Lock.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void Enqueue(T item)
		{
			base.CheckDisposed();
			try
			{
				this.Lock.EnterWriteLock();
				this.LinkedList.AddLast(item);
				this.SetDataAvailableEvent();
			}
			finally
			{
				try
				{
					this.Lock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public bool Dequeue(out T item)
		{
			item = default(T);
			base.CheckDisposed();
			bool result;
			try
			{
				this.Lock.EnterUpgradeableReadLock();
				int count = this.LinkedList.Count;
				if (1 <= count)
				{
					item = this.LinkedList.First.Value;
				}
				try
				{
					this.Lock.EnterWriteLock();
					if (1 <= count)
					{
						this.LinkedList.RemoveFirst();
					}
					if (1 >= count)
					{
						this.ResetDataAvailableEvent();
					}
				}
				finally
				{
					try
					{
						this.Lock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				result = (1 <= count);
			}
			finally
			{
				try
				{
					this.Lock.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		public IList<T> Dequeue(Predicate<T> evaluateIndividually, Predicate<IList<T>> evaluateEntirely)
		{
			base.CheckDisposed();
			IList<T> result;
			try
			{
				this.Lock.EnterUpgradeableReadLock();
				int count = this.LinkedList.Count;
				if (count == 0)
				{
					try
					{
						this.Lock.EnterWriteLock();
						this.ResetDataAvailableEvent();
					}
					finally
					{
						try
						{
							this.Lock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
					result = new T[0];
				}
				else
				{
					List<LinkedListNode<T>> list = new List<LinkedListNode<T>>(count);
					List<T> list2 = new List<T>(count);
					for (LinkedListNode<T> linkedListNode = this.LinkedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
					{
						if (evaluateIndividually == null || evaluateIndividually(linkedListNode.Value))
						{
							list.Add(linkedListNode);
							list2.Add(linkedListNode.Value);
						}
					}
					IList<T> list3 = new ReadOnlyCollection<T>(list2);
					if (evaluateEntirely != null && !evaluateEntirely(list3))
					{
						result = new T[0];
					}
					else
					{
						try
						{
							this.Lock.EnterWriteLock();
							foreach (LinkedListNode<T> node in list)
							{
								this.LinkedList.Remove(node);
							}
							if (list.Count == count)
							{
								this.ResetDataAvailableEvent();
							}
						}
						finally
						{
							try
							{
								this.Lock.ExitWriteLock();
							}
							catch (SynchronizationLockException)
							{
							}
						}
						result = list3;
					}
				}
			}
			finally
			{
				this.Lock.ExitUpgradeableReadLock();
			}
			return result;
		}

		public int Count
		{
			get
			{
				base.CheckDisposed();
				int count;
				try
				{
					this.Lock.EnterReadLock();
					count = this.LinkedList.Count;
				}
				finally
				{
					try
					{
						this.Lock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				return count;
			}
		}

		private void SetDataAvailableEvent()
		{
			if (this.DataAvailable != null && !this.EventPaused)
			{
				this.DataAvailable.Set();
			}
		}

		private void ResetDataAvailableEvent()
		{
			if (this.DataAvailable != null)
			{
				this.DataAvailable.Reset();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ThreadSafeQueue<T>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.DataAvailable != null)
				{
					this.DataAvailable.Close();
					this.DataAvailable = null;
				}
				if (this.Lock != null)
				{
					this.Lock.Dispose();
					this.Lock = null;
				}
				GC.SuppressFinalize(this);
			}
		}
	}
}
