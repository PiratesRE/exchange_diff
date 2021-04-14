using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal abstract class ColumnCache<T> : ColumnCache
	{
		public virtual T Value
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
				base.HasValue = true;
			}
		}

		public override void CloneFrom(ColumnCache from)
		{
			base.CloneFrom(from);
			if (from.HasValue)
			{
				this.data = ((ColumnCache<T>)from).data;
			}
		}

		protected override void SetValueToDefault()
		{
			this.data = default(T);
		}

		protected T data = default(T);
	}
}
