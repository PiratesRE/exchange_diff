using System;
using Microsoft.Exchange.Data.Directory.DirSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncProperty<T> : ADDirSyncProperty<T>, ISyncProperty
	{
		internal SyncProperty(T value) : base(value)
		{
		}

		private SyncProperty(T value, ADDirSyncPropertyState state) : base(value, state)
		{
		}

		public new static SyncProperty<T> NoChange
		{
			get
			{
				return new SyncProperty<T>(default(T), ADDirSyncPropertyState.NoChange);
			}
		}

		public static implicit operator SyncProperty<T>(T value)
		{
			return new SyncProperty<T>(value);
		}

		public bool HasValue
		{
			get
			{
				return base.State != ADDirSyncPropertyState.NoChange;
			}
		}

		public object GetValue()
		{
			return base.Value;
		}

		public override string ToString()
		{
			if (!this.HasValue)
			{
				return "<Not Present>";
			}
			if (base.Value != null && base.Value == null)
			{
				return "<null>";
			}
			T value = base.Value;
			return value.ToString();
		}
	}
}
