using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AttendeeRumInfo : RumInfo
	{
		private AttendeeRumInfo() : base(RumType.None, null)
		{
		}

		protected AttendeeRumInfo(RumType type, ExDateTime? originalStartTime) : base(type, originalStartTime)
		{
		}
	}
}
