using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal abstract class DataRow
	{
		protected DataRow(DataTableView tableView)
		{
			this.audit.Drop(Breadcrumb.NewItem);
			this.tableView = tableView;
			this.dataCache = new DataColumnsCache(this);
			this.AddComponent(this.dataCache);
			this.PerfCounterAttribution = null;
		}

		protected DataRow(DataTable table)
		{
			this.audit.Drop(Breadcrumb.NewItem);
			this.tableView = table.DefaultView;
			this.dataCache = new DataColumnsCache(this);
			this.AddComponent(this.dataCache);
			this.PerfCounterAttribution = null;
		}

		public bool Updating
		{
			get
			{
				return this.updating;
			}
		}

		public DataTable Table
		{
			get
			{
				return this.tableView.Table;
			}
		}

		public DataTableView TableView
		{
			get
			{
				return this.tableView;
			}
		}

		public bool PendingDatabaseUpdates
		{
			get
			{
				if (this.IsDeletePending)
				{
					return true;
				}
				if (this.IsDeleted)
				{
					return false;
				}
				foreach (IDataObjectComponent dataObjectComponent in this.components)
				{
					if (dataObjectComponent.PendingDatabaseUpdates)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool PendingDatabaseUpdateWithinRow
		{
			get
			{
				if (this.IsDeletePending)
				{
					return true;
				}
				if (this.IsDeleted)
				{
					return false;
				}
				foreach (IDataWithinRowComponent dataWithinRowComponent in this.ComponentsWithinRow)
				{
					if (dataWithinRowComponent.PendingDatabaseUpdates)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int DatabaseUpdateCount
		{
			get
			{
				if (!this.IsDeleted)
				{
					int num = 0;
					foreach (IDataObjectComponent dataObjectComponent in this.components)
					{
						num += dataObjectComponent.PendingDatabaseUpdateCount;
					}
					return num;
				}
				if (this.IsDeletePending)
				{
					return 1;
				}
				return 0;
			}
		}

		public virtual bool IsDeleted
		{
			get
			{
				return this.objectState == DataRow.DataRowState.Delete;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.objectState == DataRow.DataRowState.New;
			}
		}

		public bool IsMaterialized
		{
			get
			{
				switch (this.objectState)
				{
				case DataRow.DataRowState.New:
				case DataRow.DataRowState.Delete:
					return false;
				case DataRow.DataRowState.Materialized:
				case DataRow.DataRowState.MoveSource:
					return true;
				}
				return false;
			}
		}

		public string PerfCounterAttribution
		{
			get
			{
				return this.perfCounterAttribution;
			}
			set
			{
				this.perfCounterAttribution = (value ?? "other");
				this.perfCounters = DatabasePerfCounters.GetInstance(this.perfCounterAttribution);
			}
		}

		public DatabasePerfCountersInstance PerfCounters
		{
			get
			{
				return this.perfCounters;
			}
		}

		protected internal DataColumnsCache Columns
		{
			get
			{
				return this.dataCache;
			}
		}

		protected List<IDataObjectComponent> ComponentsAll
		{
			get
			{
				return this.components;
			}
		}

		private bool IsDeletePending
		{
			get
			{
				return this.pendingRowDelete;
			}
			set
			{
				this.pendingRowDelete = value;
			}
		}

		private IEnumerable<IDataExternalComponent> ComponentsExternal
		{
			get
			{
				foreach (IDataObjectComponent component in this.components)
				{
					IDataExternalComponent ret = component as IDataExternalComponent;
					if (ret != null)
					{
						yield return ret;
					}
				}
				yield break;
			}
		}

		private IEnumerable<IDataWithinRowComponent> ComponentsWithinRow
		{
			get
			{
				foreach (IDataObjectComponent component in this.components)
				{
					IDataWithinRowComponent ret = component as IDataWithinRowComponent;
					if (ret != null)
					{
						yield return ret;
					}
				}
				yield break;
			}
		}

		public virtual void MinimizeMemory()
		{
			foreach (IDataObjectComponent dataObjectComponent in this.components)
			{
				dataObjectComponent.MinimizeMemory();
			}
			this.perfCounters.MinimizeMemory.Increment();
		}

		public void SeekCurrent(DataTableCursor cursor)
		{
			if (!this.TrySeekCurrent(cursor))
			{
				throw new DataSeekException(this, cursor, Strings.SeekFailed);
			}
		}

		public bool TrySeekCurrent(DataTableCursor cursor)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			bool flag = false;
			try
			{
				Api.JetSetCurrentIndex(cursor.Session, cursor.TableId, null);
				this.dataCache.MakeKey(cursor);
				flag = Api.TrySeek(cursor.Session, cursor.TableId, SeekGrbit.SeekEQ);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
			this.perfCounters.Seeks.Increment();
			this.audit.Drop(flag ? Breadcrumb.Seek : Breadcrumb.SeekFail);
			return flag;
		}

		public bool TrySeekCurrentPrefix(DataTableCursor cursor, int prefixColumns)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (this.IsDeleted)
			{
				throw new InvalidOperationException(Strings.RowDeleted);
			}
			try
			{
				Api.JetSetCurrentIndex(cursor.Session, cursor.TableId, null);
				this.dataCache.MakeStartPrefixKey(cursor, prefixColumns);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
			bool flag = false;
			try
			{
				Api.JetSeek(cursor.Session, cursor.TableId, SeekGrbit.SeekGE);
				flag = true;
			}
			catch (EsentErrorException)
			{
				DataRow.IgnoredException();
			}
			this.perfCounters.PrefixSeeks.Increment();
			this.audit.Drop(flag ? Breadcrumb.Seek : Breadcrumb.SeekFail);
			return flag;
		}

		public virtual void MarkToDelete()
		{
			this.audit.Drop(Breadcrumb.MarkDeleted);
			switch (this.objectState)
			{
			case DataRow.DataRowState.New:
				this.objectState = DataRow.DataRowState.Delete;
				this.MarkRowClean();
				this.MarkExternalComponentsToDelete();
				this.CleanupInternalComponents();
				return;
			case DataRow.DataRowState.Materialized:
				this.objectState = DataRow.DataRowState.Delete;
				this.IsDeletePending = true;
				this.MarkExternalComponentsToDelete();
				this.CleanupInternalComponents();
				return;
			case DataRow.DataRowState.Delete:
				return;
			default:
				throw new InvalidOperationException(Strings.InvalidDeleteState);
			}
		}

		protected void Reconnect(DataTableCursor cursor)
		{
			if (this.TrySeekCurrent(cursor))
			{
				this.objectState = DataRow.DataRowState.Materialized;
			}
		}

		protected void AddComponent(IDataObjectComponent component)
		{
			this.components.Add(component);
		}

		protected void AddFirstComponent(IDataObjectComponent component)
		{
			this.components.Insert(0, component);
		}

		protected void ReplaceComponent(IDataObjectComponent oldComponent, IDataObjectComponent newComponent)
		{
			int num = this.components.IndexOf(oldComponent);
			if (num < 0)
			{
				throw new ArgumentException("oldComponent is not found in the component list", "oldComponent");
			}
			this.components[num] = newComponent;
		}

		protected void Commit()
		{
			this.Commit(TransactionCommitMode.MediumLatencyLazy);
		}

		protected void Commit(TransactionCommitMode commitMode)
		{
			if (!this.PendingDatabaseUpdates)
			{
				return;
			}
			using (DataConnection dataConnection = this.Table.DataSource.DemandNewConnection())
			{
				using (Transaction transaction = dataConnection.BeginTransaction())
				{
					this.Materialize(transaction);
					transaction.Commit(commitMode);
				}
			}
		}

		protected void Materialize(Transaction transaction)
		{
			if (!this.PendingDatabaseUpdates)
			{
				return;
			}
			using (DataTableCursor dataTableCursor = this.Table.OpenCursor(transaction.Connection))
			{
				this.MaterializeToCursor(transaction, dataTableCursor, null);
			}
		}

		protected void MaterializeToCursor(Transaction transaction, DataTableCursor cursor)
		{
			this.MaterializeToCursor(transaction, cursor, null);
		}

		protected virtual void MaterializeToCursor(Transaction transaction, DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (!this.PendingDatabaseUpdates)
			{
				return;
			}
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (!cursor.IsWithinTransaction)
			{
				throw new InvalidOperationException(Strings.NotInTransaction);
			}
			lock (this)
			{
				if (this.PendingDatabaseUpdates)
				{
					this.audit.Drop(Breadcrumb.MaterializeToRow);
					this.updating = true;
					try
					{
						switch (this.objectState)
						{
						case DataRow.DataRowState.New:
							this.MaterializeSave(transaction, cursor, false, checkpointCallback);
							goto IL_11F;
						case DataRow.DataRowState.Materialized:
							this.MaterializeSave(transaction, cursor, true, null);
							goto IL_11F;
						case DataRow.DataRowState.Delete:
							this.MaterializeDelete(transaction, cursor);
							this.MinimizeMemory();
							goto IL_11F;
						case DataRow.DataRowState.CloneTarget:
							this.MaterializeCloneMove(transaction, cursor, true);
							goto IL_11F;
						case DataRow.DataRowState.MoveSource:
							this.MaterializeSave(transaction, cursor, true, null);
							goto IL_11F;
						case DataRow.DataRowState.MoveTarget:
							if (this.cloneOrMoveSource.Table == this.Table)
							{
								this.MaterializeCloneMove(transaction, cursor, false);
								goto IL_11F;
							}
							this.cloneOrMoveSource.Materialize(transaction);
							this.MaterializeSave(transaction, cursor, false, null);
							goto IL_11F;
						}
						throw new InvalidOperationException(Strings.InvalidRowState);
						IL_11F:;
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
						{
							throw;
						}
					}
					this.updating = false;
				}
			}
		}

		protected void LoadFromCurrentRow(DataTableCursor cursor)
		{
			if (!cursor.IsWithinTransaction)
			{
				throw new InvalidOperationException(Strings.NotInTransaction);
			}
			try
			{
				foreach (IDataWithinRowComponent dataWithinRowComponent in this.ComponentsWithinRow)
				{
					dataWithinRowComponent.LoadFromParentRow(cursor);
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
			this.objectState = DataRow.DataRowState.Materialized;
			this.perfCounters.LoadFromCurrent.Increment();
			this.audit.Drop(Breadcrumb.Loaded);
		}

		protected void SetCloneOrMoveSource(DataRow source, bool clone)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source", Strings.CloneMoveSourceNull);
			}
			if (clone && source.Table != this.Table)
			{
				throw new InvalidOperationException("Can't clone between different tables");
			}
			if (!this.IsNew || this.cloneOrMoveSource != null)
			{
				throw new InvalidOperationException(Strings.CloneMoveTargetNotNew);
			}
			if (source.PendingDatabaseUpdates)
			{
				throw new InvalidOperationException(Strings.CloneMoveSourceModified);
			}
			DataRow.DataRowState dataRowState = source.objectState;
			if (dataRowState == DataRow.DataRowState.Materialized)
			{
				if (clone)
				{
					source.objectState = DataRow.DataRowState.CloneSource;
					this.objectState = DataRow.DataRowState.CloneTarget;
				}
				else
				{
					source.objectState = DataRow.DataRowState.MoveSource;
					this.objectState = DataRow.DataRowState.MoveTarget;
				}
				this.cloneOrMoveSource = source;
				return;
			}
			throw new InvalidOperationException(Strings.CloneMoveSourceNotSaved);
		}

		protected void SourceMoveOrCloneCompleted()
		{
			switch (this.objectState)
			{
			case DataRow.DataRowState.CloneSource:
				this.objectState = DataRow.DataRowState.Materialized;
				return;
			case DataRow.DataRowState.MoveSource:
				this.objectState = DataRow.DataRowState.InvalidatedByMove;
				return;
			}
			throw new InvalidOperationException(Strings.CloneMoveComplete);
		}

		private static void IgnoredException()
		{
		}

		private void MaterializeSave(Transaction transaction, DataTableCursor cursor, bool update, Func<bool> checkpointCallback)
		{
			if (this.PendingDatabaseUpdateWithinRow)
			{
				if (update)
				{
					this.SeekCurrent(cursor);
					checkpointCallback = null;
				}
				Api.JetPrepareUpdate(cursor.Session, cursor.TableId, update ? JET_prep.ReplaceNoLock : JET_prep.Insert);
				bool flag = false;
				try
				{
					this.SaveComponentsWithinRow(cursor, checkpointCallback);
					flag = true;
				}
				finally
				{
					if (flag)
					{
						Api.JetUpdate(cursor.Session, cursor.TableId);
					}
					else
					{
						Api.JetPrepareUpdate(cursor.Session, cursor.TableId, JET_prep.Cancel);
					}
				}
			}
			this.SaveComponentsExternal(transaction);
			this.objectState = DataRow.DataRowState.Materialized;
			if (update)
			{
				this.perfCounters.Update.Increment();
				this.audit.Drop(Breadcrumb.MaterializeUpdate);
				return;
			}
			this.perfCounters.New.Increment();
			this.audit.Drop(Breadcrumb.MaterializeNew);
		}

		private void MaterializeCloneMove(Transaction transaction, DataTableCursor cursor, bool clone)
		{
			this.cloneOrMoveSource.SeekCurrent(cursor);
			Api.JetPrepareUpdate(cursor.Session, cursor.TableId, clone ? JET_prep.InsertCopy : JET_prep.InsertCopyDeleteOriginal);
			bool flag = false;
			try
			{
				this.SaveComponentsWithinRow(cursor, null);
				flag = true;
			}
			finally
			{
				if (flag)
				{
					Api.JetUpdate(cursor.Session, cursor.TableId);
				}
				else
				{
					Api.JetPrepareUpdate(cursor.Session, cursor.TableId, JET_prep.Cancel);
				}
			}
			this.SaveComponentsExternal(transaction);
			this.objectState = DataRow.DataRowState.Materialized;
			this.cloneOrMoveSource.SourceMoveOrCloneCompleted();
			this.cloneOrMoveSource = null;
			if (clone)
			{
				this.perfCounters.Clone.Increment();
				this.audit.Drop(Breadcrumb.CloneItem);
				return;
			}
			this.perfCounters.Move.Increment();
			this.audit.Drop(Breadcrumb.Moved);
		}

		private void MaterializeDelete(Transaction transaction, DataTableCursor cursor)
		{
			this.SeekCurrent(cursor);
			Api.JetDelete(cursor.Session, cursor.TableId);
			foreach (IDataExternalComponent dataExternalComponent in this.ComponentsExternal)
			{
				dataExternalComponent.SaveToExternalRow(transaction);
			}
			this.MarkRowClean();
			this.perfCounters.Delete.Increment();
			this.audit.Drop(Breadcrumb.Deleted);
		}

		private void SaveComponentsWithinRow(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			foreach (IDataWithinRowComponent dataWithinRowComponent in this.ComponentsWithinRow)
			{
				dataWithinRowComponent.SaveToParentRow(cursor, checkpointCallback);
			}
		}

		private void SaveComponentsExternal(Transaction transaction)
		{
			foreach (IDataExternalComponent dataExternalComponent in this.ComponentsExternal)
			{
				dataExternalComponent.SaveToExternalRow(transaction);
			}
		}

		private void MarkRowClean()
		{
			this.IsDeletePending = false;
		}

		private void MarkExternalComponentsToDelete()
		{
			foreach (IDataExternalComponent dataExternalComponent in this.ComponentsExternal)
			{
				dataExternalComponent.MarkToDelete();
			}
		}

		private void CleanupInternalComponents()
		{
			foreach (IDataWithinRowComponent dataWithinRowComponent in this.ComponentsWithinRow)
			{
				dataWithinRowComponent.Cleanup();
			}
		}

		protected Breadcrumbs audit = new Breadcrumbs(8);

		protected DatabasePerfCountersInstance perfCounters;

		private readonly DataTableView tableView;

		private string perfCounterAttribution;

		private DataColumnsCache dataCache;

		private DataRow cloneOrMoveSource;

		private bool updating;

		private bool pendingRowDelete;

		private DataRow.DataRowState objectState = DataRow.DataRowState.New;

		private List<IDataObjectComponent> components = new List<IDataObjectComponent>();

		protected enum DataRowState
		{
			Undefined,
			New,
			Materialized,
			Delete,
			CloneSource,
			CloneTarget,
			MoveSource,
			MoveTarget,
			InvalidatedByMove
		}
	}
}
