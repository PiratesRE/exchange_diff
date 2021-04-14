using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class ColumnCacheValueType<T> : ColumnCache<T> where T : IEquatable<T>
	{
		public override T Value
		{
			get
			{
				if (!base.HasValue)
				{
					return default(T);
				}
				return this.data;
			}
			set
			{
				if (!base.HasValue || !this.data.Equals(value))
				{
					base.Value = value;
				}
			}
		}
	}
}
