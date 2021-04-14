using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct TSLID
	{
		private TSLID(ulong id)
		{
			this.id = id;
		}

		public static explicit operator TSLID(ulong id)
		{
			return new TSLID(id);
		}

		public override string ToString()
		{
			return this.id.ToString();
		}

		private ulong id;
	}
}
