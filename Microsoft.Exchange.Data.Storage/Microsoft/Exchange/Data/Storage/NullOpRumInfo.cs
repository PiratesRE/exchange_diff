using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullOpRumInfo : RumInfo
	{
		private NullOpRumInfo() : base(RumType.None, null)
		{
		}

		public static NullOpRumInfo CreateInstance()
		{
			return new NullOpRumInfo();
		}
	}
}
