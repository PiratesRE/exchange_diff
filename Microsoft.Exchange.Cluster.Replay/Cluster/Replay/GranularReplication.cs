using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class GranularReplication
	{
		public static string FormPartialLogFileName(string prefix, long generation)
		{
			return EseHelper.MakeLogfileName(prefix, ".jsl", generation);
		}

		public static string FormShortAcllLogFileName(string prefix, long generation)
		{
			return EseHelper.MakeLogfileName(prefix, ".acll", generation);
		}

		public static bool IsEnabled()
		{
			return !RegistryParameters.DisableGranularReplication;
		}

		public const string JetShadowLogFileExtension = "jsl";

		public const string JetShadowLogFileExtensionWithDot = ".jsl";

		public const string AcllBlockModeExtensionWithDot = ".acll";
	}
}
