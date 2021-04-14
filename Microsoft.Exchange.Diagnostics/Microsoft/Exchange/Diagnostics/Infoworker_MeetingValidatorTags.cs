using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Infoworker_MeetingValidatorTags
	{
		public const int Validator = 0;

		public const int ConsistencyChecks = 1;

		public const int CompareToAttendee = 2;

		public const int CompareToOrganizer = 3;

		public const int Fixup = 4;

		public const int PFD = 5;

		public static Guid guid = new Guid("7CCC3078-AE21-4CF6-B241-3EE7A8439681");
	}
}
