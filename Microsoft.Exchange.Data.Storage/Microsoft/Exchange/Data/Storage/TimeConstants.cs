using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TimeConstants
	{
		public const int MinutesPerDay = 1440;

		public const int MinutesPerHour = 60;

		public static readonly TimeSpan OneDay = new TimeSpan(1, 0, 0, 0, 0);
	}
}
