using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal abstract class DataInternalComponent : IDataWithinRowComponent, IDataObjectComponent
	{
		protected DataInternalComponent(DataRow dataRow)
		{
			this.dataRow = dataRow;
		}

		public abstract bool PendingDatabaseUpdates { get; }

		public virtual int PendingDatabaseUpdateCount
		{
			get
			{
				if (!this.PendingDatabaseUpdates)
				{
					return 0;
				}
				return 1;
			}
		}

		public virtual bool WithinParentRow
		{
			get
			{
				return true;
			}
		}

		protected DataRow DataRow
		{
			get
			{
				return this.dataRow;
			}
		}

		public abstract void LoadFromParentRow(DataTableCursor cursor);

		public abstract void SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback);

		public virtual void Cleanup()
		{
		}

		public virtual void MinimizeMemory()
		{
		}

		public abstract void CloneFrom(IDataObjectComponent other);

		[NonSerialized]
		private readonly DataRow dataRow;
	}
}
