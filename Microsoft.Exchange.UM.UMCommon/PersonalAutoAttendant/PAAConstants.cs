using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PAAConstants
	{
		internal static readonly Version CurrentVersion = new Version(14, 0, 0, 0);

		internal static readonly TimeSpan PAAEvaluationTimeout = TimeSpan.FromSeconds(3.0);

		internal static readonly TimeSpan PAAGreetingDownloadTimeout = TimeSpan.FromSeconds(1.0);

		internal static readonly int PhoneNumberCallerIdEvaluationCost = 0;

		internal static readonly int ADContactCallerIdEvaluationCost = 1;

		internal static readonly int ContactItemCallerIdEvaluationCost = 2;

		internal static readonly int PersonaContactCallerIdEvaluationCost = 2;

		internal static readonly int ContactFolderCallerIdEvaluationCost = 2;
	}
}
