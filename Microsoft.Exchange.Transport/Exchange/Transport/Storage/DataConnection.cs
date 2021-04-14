using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataConnection : IDisposeTrackable, IDisposable
	{
		private DataConnection(JET_INSTANCE instance, DataSource source)
		{
			Interlocked.Increment(ref DataConnection.nextId);
			int num = 0;
			try
			{
				try
				{
					num++;
					Api.JetBeginSession(instance, out this.session, null, null);
					num++;
					Api.JetOpenDatabase(this.session, source.DatabasePath, null, out this.database, OpenDatabaseGrbit.None);
					num++;
					this.source = source;
					this.source.AddRef();
				}
				finally
				{
					switch (num)
					{
					case 2:
						Api.JetEndSession(this.session, EndSessionGrbit.None);
						break;
					case 3:
						this.opened = true;
						break;
					}
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, source))
				{
					throw;
				}
			}
			this.disposeTracker = this.GetDisposeTracker();
		}

		public JET_SESID Session
		{
			get
			{
				this.ThrowIfClosed();
				return this.session;
			}
		}

		public JET_DBID Database
		{
			get
			{
				this.ThrowIfClosed();
				return this.database;
			}
		}

		public DataSource Source
		{
			get
			{
				this.ThrowIfClosed();
				return this.source;
			}
		}

		public bool IsWithinTransaction
		{
			get
			{
				return this.pendingTransactions > 0;
			}
		}

		public static DataConnection Open(JET_INSTANCE instance, DataSource source)
		{
			DataConnection dataConnection = new DataConnection(instance, source);
			if (!dataConnection.opened)
			{
				dataConnection.Dispose(false);
				dataConnection = null;
			}
			else
			{
				dataConnection.references = 1;
			}
			return dataConnection;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DataConnection>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public int AddRef()
		{
			return Interlocked.Increment(ref this.references);
		}

		public int Release()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			int num = Interlocked.Decrement(ref this.references);
			if (num == 0)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			return num;
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Close()
		{
			this.Release();
			if (this.opened)
			{
				throw new InvalidOperationException(Strings.ConnectionInUse);
			}
		}

		public Transaction BeginTransaction()
		{
			return Transaction.New(this);
		}

		internal void NoPendingTransactions()
		{
			if (this.pendingTransactions > 0)
			{
				throw new InvalidOperationException(Strings.PendingTransactions);
			}
		}

		internal void TrackStartTransaction()
		{
			this.ThrowIfClosed();
			Interlocked.Increment(ref this.pendingTransactions);
		}

		internal void TrackRemoveTransaction()
		{
			this.ThrowIfClosedOrNoPendingTransaction();
			Interlocked.Decrement(ref this.pendingTransactions);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.opened)
			{
				if (disposing)
				{
					this.ThrowIfClosed();
					this.NoPendingTransactions();
					this.source.TrackTryConnectionClose();
					try
					{
						Api.JetCloseDatabase(this.session, this.database, CloseDatabaseGrbit.None);
						Api.JetEndSession(this.session, EndSessionGrbit.None);
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, this.source))
						{
							throw;
						}
					}
					this.source.Release();
				}
				this.source = null;
				this.opened = false;
			}
		}

		private void ThrowIfClosed()
		{
			if (!this.opened)
			{
				throw new ObjectDisposedException("DataConnection");
			}
		}

		private void ThrowIfClosedOrNoPendingTransaction()
		{
			this.ThrowIfClosed();
			if (this.pendingTransactions <= 0)
			{
				throw new InvalidOperationException(Strings.NotInTransaction);
			}
		}

		private static int nextId;

		private readonly JET_SESID session;

		private readonly JET_DBID database;

		private readonly DisposeTracker disposeTracker;

		private DataSource source;

		private bool opened;

		private int pendingTransactions;

		private int references;
	}
}
