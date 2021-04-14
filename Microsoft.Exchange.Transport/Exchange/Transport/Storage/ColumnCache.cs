using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal abstract class ColumnCache
	{
		public bool Dirty
		{
			get
			{
				return (this.flags & ColumnCache.States.Dirty) == ColumnCache.States.Dirty;
			}
			set
			{
				if (value)
				{
					this.flags |= ColumnCache.States.Dirty;
					return;
				}
				this.flags &= ~ColumnCache.States.Dirty;
			}
		}

		public bool HasValue
		{
			get
			{
				return (this.flags & ColumnCache.States.Null) != ColumnCache.States.Null;
			}
			set
			{
				if (value)
				{
					this.flags &= ~ColumnCache.States.Null;
				}
				else
				{
					this.flags |= ColumnCache.States.Null;
					this.SetValueToDefault();
				}
				this.Dirty = true;
			}
		}

		public virtual void CloneFrom(ColumnCache from)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			if (this == from)
			{
				throw new ArgumentException(Strings.CircularClone, "from");
			}
			this.flags = from.flags;
		}

		protected abstract void SetValueToDefault();

		private ColumnCache.States flags = ColumnCache.States.Null;

		[Flags]
		protected enum States
		{
			Null = 1,
			Dirty = 2
		}
	}
}
