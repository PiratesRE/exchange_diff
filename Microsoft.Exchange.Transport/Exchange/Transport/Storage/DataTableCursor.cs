using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataTableCursor : IDisposeTrackable, IDisposable
	{
		public DataTableCursor(JET_TABLEID tableId, DataConnection connection, DataTable dataTable)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			Interlocked.Increment(ref DataTableCursor.nextId);
			this.tableId = tableId;
			this.dataTable = dataTable;
			this.connection = connection;
			this.audit.Drop(Breadcrumb.NewItem);
			this.connection.Source.PerfCounters.CursorsOpened.Increment();
			this.connection.AddRef();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public JET_TABLEID TableId
		{
			get
			{
				return this.tableId;
			}
		}

		public DataTable Table
		{
			get
			{
				return this.dataTable;
			}
		}

		public JET_SESID Session
		{
			get
			{
				return this.connection.Session;
			}
		}

		public DataConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public bool IsWithinTransaction
		{
			get
			{
				return this.connection.IsWithinTransaction;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public void DeleteCurrentRow()
		{
			try
			{
				Api.JetDelete(this.Session, this.TableId);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void GotoBookmark(byte[] bookmark)
		{
			if (bookmark == null)
			{
				throw new ArgumentNullException("bookmark");
			}
			try
			{
				Api.JetGotoBookmark(this.Session, this.TableId, bookmark, bookmark.Length);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public byte[] GetBookmark()
		{
			try
			{
				return Api.GetBookmark(this.Session, this.TableId);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			return null;
		}

		public void PrereadForward()
		{
			try
			{
				Api.JetSetTableSequential(this.Session, this.TableId, (SetTableSequentialGrbit)1);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void PrereadBackward()
		{
			try
			{
				Api.JetSetTableSequential(this.Session, this.TableId, (SetTableSequentialGrbit)2);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void SetCurrentIndex(string indexName)
		{
			try
			{
				Api.JetSetCurrentIndex(this.Session, this.TableId, indexName);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void MakeKey(params byte[][] keys)
		{
			try
			{
				MakeKeyGrbit grbit = MakeKeyGrbit.NewKey;
				foreach (byte[] data in keys)
				{
					Api.MakeKey(this.Session, this.tableId, data, grbit);
					grbit = MakeKeyGrbit.None;
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public bool TrySeekGE(params byte[][] keys)
		{
			return this.TrySeek(SeekGrbit.SeekGE, keys);
		}

		public bool TrySeek(params byte[][] keys)
		{
			return this.TrySeek(SeekGrbit.SeekEQ, keys);
		}

		public void Seek(params byte[][] keys)
		{
			this.Seek(SeekGrbit.SeekEQ, keys);
		}

		public bool TrySeek()
		{
			return this.TrySeek(SeekGrbit.SeekEQ);
		}

		public bool TrySetIndexUpperRange(params byte[][] keys)
		{
			return this.TrySetIndexRange(SetIndexRangeGrbit.RangeInclusive | SetIndexRangeGrbit.RangeUpperLimit, keys);
		}

		public bool TryRemoveIndexRange()
		{
			return this.TrySetIndexRange(SetIndexRangeGrbit.RangeRemove);
		}

		public bool TryMoveFirst()
		{
			return this.TryMove(JET_Move.First, false);
		}

		public bool TryMoveLast()
		{
			return this.TryMove(JET_Move.Last, false);
		}

		public bool TryMoveNext(bool skipDups = false)
		{
			return this.TryMove(JET_Move.Next, skipDups);
		}

		public bool TryMovePrevious(bool skipDups = false)
		{
			return this.TryMove(JET_Move.Previous, skipDups);
		}

		public bool HasData()
		{
			return this.TryMove((JET_Move)0, false);
		}

		public void MoveBeforeFirst()
		{
			try
			{
				Api.MoveBeforeFirst(this.Session, this.tableId);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void MoveAfterLast()
		{
			try
			{
				Api.MoveAfterLast(this.Session, this.tableId);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void MoveFirst()
		{
			this.Move(JET_Move.First);
		}

		public void MoveLast()
		{
			this.Move(JET_Move.Last);
		}

		public void CancelPrepare()
		{
			this.PrepareUpdate(JET_prep.Cancel);
		}

		public void PrepareUpdate(bool doLock = true)
		{
			this.PrepareUpdate(doLock ? JET_prep.Replace : JET_prep.ReplaceNoLock);
		}

		public void PrepareInsert(bool clone = false, bool deleteOriginal = false)
		{
			this.PrepareUpdate((!clone) ? JET_prep.Insert : (deleteOriginal ? JET_prep.InsertCopyDeleteOriginal : JET_prep.InsertCopy));
		}

		public void Update()
		{
			try
			{
				Api.JetUpdate(this.Session, this.tableId);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public void CreateIndex(string name, string columns)
		{
			try
			{
				Api.JetCreateIndex(this.Session, this.TableId, name, CreateIndexGrbit.IndexIgnoreAnyNull, columns, columns.Length, 90);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		public bool TryCreateIndex(string name, string columns)
		{
			try
			{
				Api.JetCreateIndex(this.Session, this.TableId, name, CreateIndexGrbit.IndexIgnoreAnyNull, columns, columns.Length, 90);
			}
			catch (EsentIndexDuplicateException)
			{
				return false;
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			return true;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DataTableCursor>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public Transaction BeginTransaction()
		{
			return this.connection.BeginTransaction();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (disposing)
			{
				this.audit.Drop(Breadcrumb.CloseItem);
				if (this.connection != null)
				{
					try
					{
						Api.JetCloseTable(this.Session, this.TableId);
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, this.connection.Source))
						{
							throw;
						}
					}
					this.connection.Source.PerfCounters.CursorsClosed.Increment();
					this.Table.ReleaseCursor();
					this.connection.Release();
				}
			}
			this.connection = null;
		}

		private bool TrySeek(SeekGrbit grbit, params byte[][] keys)
		{
			this.MakeKey(keys);
			return this.TrySeek(grbit);
		}

		private void Seek(SeekGrbit grbit, params byte[][] keys)
		{
			this.MakeKey(keys);
			this.Seek(grbit);
		}

		private bool TrySeek(SeekGrbit grbit)
		{
			try
			{
				return Api.TrySeek(this.Session, this.tableId, grbit);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			return false;
		}

		private void Seek(SeekGrbit grbit)
		{
			try
			{
				Api.JetSeek(this.Session, this.tableId, grbit);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		private bool TrySetIndexRange(SetIndexRangeGrbit grbit, params byte[][] keys)
		{
			this.MakeKey(keys);
			return this.TrySetIndexRange(grbit);
		}

		private bool TrySetIndexRange(SetIndexRangeGrbit grbit)
		{
			try
			{
				Api.JetSetIndexRange(this.Session, this.TableId, grbit);
				return true;
			}
			catch (EsentNoCurrentRecordException)
			{
				this.TryRemoveIndexRange();
			}
			catch (EsentInvalidOperationException)
			{
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			return false;
		}

		private bool TryMove(JET_Move where, bool skipDups)
		{
			try
			{
				return Api.TryMove(this.Session, this.tableId, where, skipDups ? MoveGrbit.MoveKeyNE : MoveGrbit.None);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			return false;
		}

		private void Move(JET_Move where)
		{
			try
			{
				Api.JetMove(this.Session, this.tableId, where, MoveGrbit.None);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		private void PrepareUpdate(JET_prep prep)
		{
			try
			{
				Api.JetPrepareUpdate(this.Session, this.tableId, prep);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
		}

		private static int nextId;

		private readonly JET_TABLEID tableId;

		private readonly DataTable dataTable;

		private readonly DisposeTracker disposeTracker;

		private DataConnection connection;

		private Breadcrumbs audit = new Breadcrumbs(64);
	}
}
