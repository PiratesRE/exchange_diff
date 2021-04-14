using System;

namespace Microsoft.Exchange.Common.Cache
{
	[Serializable]
	internal abstract class CachableItem
	{
		public abstract long ItemSize { get; }

		public virtual bool IsExpired(DateTime currentTime)
		{
			return false;
		}
	}
}
