using System;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal class ResolvedGroup : CachableItem
	{
		public ResolvedGroup() : this(Guid.Empty)
		{
		}

		public ResolvedGroup(Guid groupGuid)
		{
			this.groupGuid = groupGuid;
		}

		public Guid GroupGuid
		{
			get
			{
				return this.groupGuid;
			}
		}

		public override long ItemSize
		{
			get
			{
				return 16L;
			}
		}

		private const int GuidLength = 16;

		private Guid groupGuid;
	}
}
