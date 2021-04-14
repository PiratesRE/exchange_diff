using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class ColumnCacheByteArray : ColumnCache<byte[]>
	{
		public override byte[] Value
		{
			get
			{
				if (base.Value == null)
				{
					return null;
				}
				byte[] array = new byte[base.Value.Length];
				base.Value.CopyTo(array, 0);
				return array;
			}
			set
			{
				if (!ArrayComparer<byte>.Comparer.Equals(this.Value, value))
				{
					if (value != null)
					{
						base.Value = value;
						return;
					}
					base.HasValue = false;
				}
			}
		}
	}
}
