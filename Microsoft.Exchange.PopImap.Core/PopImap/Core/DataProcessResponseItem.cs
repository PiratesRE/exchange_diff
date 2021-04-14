using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class DataProcessResponseItem : IResponseItem, IDisposeTrackable, IDisposable
	{
		public DataProcessResponseItem() : this(null, null, null, false)
		{
		}

		public DataProcessResponseItem(object stateData, Action<ProtocolSession, DataProcessResponseItem> dataProcessDelegate) : this(stateData, dataProcessDelegate, null, true)
		{
		}

		public DataProcessResponseItem(object stateData, Action<ProtocolSession, DataProcessResponseItem> dataProcessDelegate, Action<DataProcessResponseItem> dataCleanupDelegate) : this(stateData, dataProcessDelegate, dataCleanupDelegate, true)
		{
		}

		public DataProcessResponseItem(object stateData, Action<ProtocolSession, DataProcessResponseItem> dataProcessDelegate, Action<DataProcessResponseItem> dataCleanupDelegate, bool shouldConnectToStore)
		{
			this.responseQueue = new ResponseQueue(5, false);
			this.disposeTracker = this.GetDisposeTracker();
			this.stateData = stateData;
			this.dataProcessDelegate = dataProcessDelegate;
			this.dataCleanupDelegate = dataCleanupDelegate;
			this.shouldConnectToStore = shouldConnectToStore;
		}

		public Action<ProtocolSession, DataProcessResponseItem> LateDataProcessDelegate
		{
			get
			{
				return this.dataProcessDelegate;
			}
		}

		public object StateData
		{
			get
			{
				return this.stateData;
			}
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return null;
			}
		}

		public void BindData(object stateData, Action<ProtocolSession, DataProcessResponseItem> dataProcessDelegate, bool shouldConnectToStore)
		{
			this.stateData = stateData;
			this.dataProcessDelegate = dataProcessDelegate;
			this.dataCleanupDelegate = null;
			this.shouldConnectToStore = shouldConnectToStore;
			this.responseQueue.Clear();
		}

		public int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			ProtocolSession protocolSession = session as ProtocolSession;
			if (protocolSession == null)
			{
				throw new ArgumentException("session is not a valid ProtocolSession");
			}
			buffer = null;
			offset = 0;
			IStandardBudget perCallBudget = null;
			if (this.dataProcessDelegate != null)
			{
				bool flag = false;
				bool flag2 = false;
				ResponseFactory responseFactory = protocolSession.ResponseFactory;
				if (protocolSession.Disconnected || responseFactory == null)
				{
					return 0;
				}
				try
				{
					ActivityContext.SetThreadScope(protocolSession.ActivityScope);
					if (this.shouldConnectToStore)
					{
						Monitor.Enter(responseFactory.Store);
						flag2 = true;
					}
					if (this.shouldConnectToStore)
					{
						try
						{
							perCallBudget = responseFactory.AcquirePerCommandBudget();
						}
						catch (OverBudgetException exception)
						{
							responseFactory.LogHandledException(exception);
							return 0;
						}
						catch (ADTransientException exception2)
						{
							responseFactory.LogHandledException(exception2);
							return 0;
						}
					}
					try
					{
						if (this.shouldConnectToStore && !responseFactory.IsStoreConnected)
						{
							responseFactory.ConnectToTheStore();
							flag = true;
						}
						this.dataProcessDelegate(protocolSession, this);
					}
					catch (StorageTransientException exception3)
					{
						responseFactory.LogHandledException(exception3);
						return 0;
					}
					catch (StoragePermanentException exception4)
					{
						responseFactory.LogHandledException(exception4);
						return 0;
					}
				}
				catch (Exception exception5)
				{
					if (protocolSession.CheckNonCriticalException(exception5))
					{
						responseFactory.LogHandledException(exception5);
						return 0;
					}
					throw;
				}
				finally
				{
					if (this.dataCleanupDelegate != null)
					{
						this.dataCleanupDelegate(this);
					}
					if (flag)
					{
						responseFactory.DisconnectFromTheStore();
					}
					if (flag2)
					{
						Monitor.Exit(responseFactory.Store);
					}
					protocolSession.EnforceMicroDelayAndDisposeCostHandles(perCallBudget);
				}
				this.dataProcessDelegate = null;
				this.dataCleanupDelegate = null;
				if (!this.responseQueue.IsSending)
				{
					return 0;
				}
			}
			return this.responseQueue.GetNextChunk(session, out buffer, out offset);
		}

		public void EnqueueResponseItem(IResponseItem responseItem)
		{
			this.responseQueue.Enqueue(responseItem);
			if (!this.responseQueue.IsSending)
			{
				this.responseQueue.DequeueForSend();
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DataProcessResponseItem>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.responseQueue != null)
				{
					this.responseQueue.Dispose();
					this.responseQueue = null;
				}
			}
		}

		private Action<ProtocolSession, DataProcessResponseItem> dataProcessDelegate;

		private Action<DataProcessResponseItem> dataCleanupDelegate;

		private object stateData;

		private ResponseQueue responseQueue;

		private bool shouldConnectToStore;

		private DisposeTracker disposeTracker;
	}
}
