using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class TestSupport
	{
		public static void SetZerobox()
		{
			TestSupport.s_zerobox = true;
		}

		public static string UseLocalMachineNameOnZerobox(string serverName)
		{
			if (!TestSupport.s_zerobox)
			{
				return serverName;
			}
			return Environment.MachineName;
		}

		public static bool IsCatalogSeedDisabled()
		{
			return TestSupport.s_zerobox;
		}

		public static bool IsZerobox()
		{
			return TestSupport.s_zerobox;
		}

		private static bool s_zerobox;
	}
}
