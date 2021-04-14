using System;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	internal class ADDirSyncProperty<T>
	{
		public ADDirSyncProperty(T value) : this(value, ADDirSyncPropertyState.NewValue)
		{
		}

		protected ADDirSyncProperty(T value, ADDirSyncPropertyState state)
		{
			this.value = value;
			this.State = state;
		}

		public static ADDirSyncProperty<T> NoChange
		{
			get
			{
				return new ADDirSyncProperty<T>(default(T), ADDirSyncPropertyState.NoChange);
			}
		}

		public ADDirSyncPropertyState State { get; private set; }

		public T Value
		{
			get
			{
				if (this.State != ADDirSyncPropertyState.NewValue)
				{
					throw new InvalidOperationException(DirectoryStrings.ValueNotAvailableForUnchangedProperty);
				}
				return this.value;
			}
		}

		private readonly T value;
	}
}
