using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("pdi")]
	internal sealed class PAADurationInfo
	{
		public const string StructNamespace = "pdi";

		public const string StartTime = "st";

		public const string EndTime = "end";

		public const string Days = "dys";

		public const string IsWorking = "fw";

		public const string IsNonWorking = "fnw";

		public const string IsCustom = "fc";

		[OwaEventField("st", true, 0)]
		public int StartTimeMinutes;

		[OwaEventField("end", true, 0)]
		public int EndTimeMinutes;

		[OwaEventField("dys", true, 0)]
		public int DaysOfWeek;

		[OwaEventField("fw", false, true)]
		public bool IsWorkingHours;

		[OwaEventField("fnw", false, false)]
		public bool IsNonWorkingHours;

		[OwaEventField("fc", false, false)]
		public bool IsCustomDuration;
	}
}
