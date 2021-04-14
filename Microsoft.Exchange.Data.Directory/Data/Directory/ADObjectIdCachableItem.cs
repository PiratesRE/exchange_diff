using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADObjectIdCachableItem : CachableItem
	{
		internal ADObjectIdCachableItem(ADObjectId value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = value;
		}

		internal ADObjectId Value { get; private set; }

		public override long ItemSize
		{
			get
			{
				return (long)((this.Value.DistinguishedName ?? string.Empty).Length * 2 + 16);
			}
		}
	}
}
